# personal-blog-service

Personal Blog Service with API endpoints built using .Net 9 with PostgreSQL DB

---

## `ApplicationUser` vs `BlogUser` — Why Two User Types?

> This is the single most confusing part of the codebase for anyone coming in fresh. Read this section first before touching anything auth or user related.

Every registered user in this app is represented by **two separate database records** that are linked together by a shared GUID. They exist for different reasons and are used in different parts of the code. Mixing them up is the fastest way to break auth or lose data.

---

### The short answer

| | `ApplicationUser` | `BlogUser` |
|---|---|---|
| **What it is** | ASP.NET Identity's security record | Our own application profile record |
| **Table** | `AspNetUsers` (Identity-managed) | `blog_users` (our table) |
| **Model file** | `PersonalBlog.Models/DatabaseModels/ApplicationUser.cs` | `PersonalBlog.Models/DatabaseModels/BlogUser.cs` |
| **Owns** | Username, email, password hash, security stamp | Display name, avatar, role, posts, drafts |
| **Used for** | Login, password verification, JWT generation | Blog posts, drafts, profile display |
| **ID in the JWT** | Yes — `ApplicationUser.Id` is the `sub` claim | No |
| **Created together?** | Yes — both are created in one transaction on registration | Yes |

---

### The long answer — why they exist separately

ASP.NET Core Identity is a complete, battle-tested auth framework that handles password hashing, lockout policies, two-factor auth, role claims, and security stamps. It comes with its own rigid table structure (`AspNetUsers`, `AspNetRoles`, etc.). You are not supposed to add arbitrary application columns to it — doing so couples your app data to the auth framework and makes migrations fragile.

So the design here is:

- Let `ApplicationUser` (which just extends `IdentityUser<Guid>`) stay as the Identity framework's record. Don't put application data in it.
- Create a separate `BlogUser` table that holds everything the application actually cares about: the username to display on posts, the avatar, the user role, the relationship to posts and drafts.
- Link them by a foreign key so they are always in sync.

Think of it like a physical ID card vs. a library membership card:
- The **ID card** (`ApplicationUser`) proves who you are and authenticates you. The government (ASP.NET Identity) issues and controls it.
- The **library card** (`BlogUser`) lets you borrow books (create posts). The library (our app) issues it and tracks what you've borrowed.
- Both cards have the same name on them — they are linked — but they serve completely different purposes.

---

### How they are linked

`BlogUser` has a foreign key column called `IdentityUserId` that points to `ApplicationUser.Id`:

```csharp
// PersonalBlog.Models/DatabaseModels/BlogUser.cs

public class BlogUser
{
    public Guid Id { get; set; }              // BlogUser's own primary key (Guid.NewGuid())
    public Guid IdentityUserId { get; set; }  // FK → ApplicationUser.Id  ← THE BRIDGE

    public string Username { get; set; }
    public string Email { get; set; }
    public string? Displayname { get; set; }
    public string? Avatar { get; set; }
    public UserRole Role { get; set; }        // "admin" | "user" (Postgres enum)

    public virtual ICollection<Post> Posts { get; set; }
    public virtual ICollection<Draft> Drafts { get; set; }

    public ApplicationUser IdentityUser { get; set; }  // navigation property → ApplicationUser
}
```

```csharp
// PersonalBlog.Models/DatabaseModels/ApplicationUser.cs

public class ApplicationUser : IdentityUser<Guid>
{
    // Extends IdentityUser<Guid> — all Identity columns (PasswordHash,
    // SecurityStamp, NormalizedUserName, etc.) are inherited automatically.
    // No custom properties needed here yet.
}
```

---

### How they are created together — registration flow

Both records are always created together in a single service call so they can never exist independently:

```
POST /api/auth/register
        │
        ▼
AuthenticationController.Register()
        │  maps RegisterRequest → CreateIdentityDTO
        ▼
AuthenticationService.CreateUserAndIdentity()
        │
        ▼
UserIdentityService.CreateUserAndIdentityAsync()
        │
        ├── 1. Check username doesn't already exist (in ApplicationUsers)
        │
        ├── 2. Create ApplicationUser:
        │       { UserName, Email, NormalizedEmail, NormalizedUserName,
        │         SecurityStamp, PasswordHash }
        │       Saved to AspNetUsers table via UserRepository.CreateIdentityUserAsync()
        │
        └── 3. Create BlogUser:
                { Id = Guid.NewGuid(),
                  IdentityUserId = newIdentity.Id,  ← links to the ApplicationUser above
                  Username, Email, Role = "user",
                  Createddate, Lastmodifieddate }
                Saved to blog_users table via UserRepository.CreateAsync()
```

If step 2 fails, step 3 never runs — no orphaned `BlogUser` without an `ApplicationUser`.

---

### How the IDs flow through the system

This is where most confusion happens. There are **three different GUIDs** in play:

```
ApplicationUser.Id      ← Identity-managed GUID. Lives in AspNetUsers.
                           Stamped into the JWT as the ClaimTypes.NameIdentifier claim.
                           This is what the frontend sees as the "user ID".

BlogUser.Id             ← A separate GUID (Guid.NewGuid() at registration).
                           Used as the primary key of blog_users.
                           Posts.Userid and Drafts.Userid FK reference THIS ID.

BlogUser.IdentityUserId ← Copy of ApplicationUser.Id stored in blog_users.
                           The bridge between the two records.
```

**Concrete flow — user creates a post:**

```
1. User logs in
        │
        ▼
2. TokenService.GenerateAccessToken(applicationUser)
   builds a JWT with:
     sub  = applicationUser.UserName
     NameIdentifier = applicationUser.Id   ← ApplicationUser.Id goes into the token

3. Frontend stores the token in an HttpOnly cookie.
   Frontend also receives the user ID (ApplicationUser.Id) in the login response.

4. User clicks "Create Post" — frontend calls:
     POST /blogs/create/{id}    where {id} = ApplicationUser.Id

5. IdentityFilterAttribute runs:
   - reads ApplicationUser.Id from the JWT NameIdentifier claim
   - compares it to the {id} route parameter
   - if they match: the caller IS the user they claim to be → authorised
   - stores the validated identity in HttpContext.Items["AuthenticatedIdentity"]

6. BlogController.CreatePost() calls BlogService
   BlogService calls BlogRepository to save the Post with:
     Post.Userid = BlogUser.Id   ← NOT ApplicationUser.Id
   
   The repository looks up the BlogUser by:
     BlogUser.IdentityUserId == ApplicationUser.Id
   to find the correct BlogUser.Id to stamp on the post.
```

---

### Which class to use where — quick reference

```
Authentication / Security                  → ApplicationUser
  Password hashing & verification
  JWT token generation (TokenService)
  SignInManager.SignInAsync / SignOutAsync
  IdentityFilter JWT claim extraction

Application data / Business logic          → BlogUser
  Displaying author name on a post
  Fetching a user's posts or drafts
  User role checks (admin vs. user)
  Profile display (avatar, displayname)

Looking up a BlogUser when you have        → UserRepository.GetByIdAsync(identityUserId)
an ApplicationUser.Id (e.g. from the JWT)    Queries: BlogUser WHERE IdentityUserId == id
```

---

### Where each type lives in the database

```
PostgreSQL database
│
├── AspNetUsers          ← managed entirely by ASP.NET Identity
│     Id (Guid)          ← this is ApplicationUser.Id
│     UserName
│     NormalizedUserName
│     Email
│     NormalizedEmail
│     PasswordHash
│     SecurityStamp
│     ...other Identity columns
│
├── blog_users           ← our own table (BlogUser model)
│     id (Guid)          ← BlogUser.Id — primary key (NOT the same as AspNetUsers.Id)
│     identity_user_id   ← BlogUser.IdentityUserId = FK → AspNetUsers.Id
│     username
│     email
│     displayname
│     avatar
│     role               ← Postgres enum: "admin" | "user"
│     createddate
│     lastmodifieddate
│
├── posts
│     id (Guid)
│     userid             ← FK → blog_users.id  (uses BlogUser.Id, NOT ApplicationUser.Id)
│     title, content, preview, ...
│
└── drafts
      id (Guid)
      userid             ← FK → blog_users.id  (same — uses BlogUser.Id)
      title, content, preview, ...
```

---

## Entity Framework Commands

```bash
# Scaffold database models from existing database
dotnet ef dbcontext scaffold "Host=localhost;Database=personal_blog;Username=your_username;Password=your_password" Npgsql.EntityFrameworkCore.PostgreSQL --project PersonalBlog.Models --startup-project PersonalBlog.Api --context-dir . --output-dir DatabaseModels --force

# Create migration
dotnet ef migrations add IdentityAndLink --project PersonalBlog.Infrastructure --startup-project PersonalBlog.Api

# Update database
dotnet ef database update --project PersonalBlog.Infrastructure --startup-project PersonalBlog.Api
```
