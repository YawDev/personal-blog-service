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

## API Endpoints & Flows

### `POST /api/auth/register` — Register
```
Client sends { username, email, password }
  ↓ map to CreateIdentityDTO
  ↓ check username not already taken
  ↓ create ApplicationUser (AspNetUsers table)
  ↓ create BlogUser linked by IdentityUserId (blog_users table)
  ↓ return 200
```

---

### `POST /api/auth/login` — Login
```
Client sends { email, password }
  ↓ validate credentials via Identity
  ↓ generate JWT access token (30 min)
  ↓ generate refresh token, save to DB
  ↓ set access_token HttpOnly cookie (30 min)
  ↓ set refresh_token HttpOnly cookie (7 days)
  ↓ return 200 + user info
```

---

### `POST /api/auth/refresh` — Token Refresh
```
POST /api/auth/refresh
  ↓ read refresh_token cookie
  ↓ find in DB → validate IsActive
  ↓ mark old as IsUsed = true
  ↓ generate new access token + new refresh token
  ↓ save new refresh token to DB
  ↓ set both as HttpOnly cookies
  ↓ return 200
```

Refresh tokens are rotated on every use. A token that is expired, revoked, or already used returns `401`.

**References:**
- [How to Implement Refresh Tokens and Token Revocation in ASP.NET Core](https://antondevtips.com/blog/how-to-implement-refresh-tokens-and-token-revocation-in-aspnetcore)
- [Refresh Tokens in ASP.NET Core — Medium](https://medium.com/@roshanj100/refresh-tokens-in-asp-net-core-the-key-to-secure-and-seamless-sessions-8b33324568e3)
- [Refresh Token Rotation - Auth0 Docs](https://auth0.com/docs/secure/tokens/refresh-tokens/refresh-token-rotation)

---

### `POST /api/auth/logout` — Logout
```
Authenticated request
  ↓ SignOutAsync via SignInManager
  ↓ return 200
```

---

### `GET /api/auth/me` — Get Current User
```
Authenticated request (JWT cookie)
  ↓ extract ApplicationUser.Id from NameIdentifier claim
  ↓ fetch identity user from DB
  ↓ return 200 + user info
```

---

### `GET /api/auth/user/{id}` — Get User Info
```
IdentityFilter validates JWT sub claim matches {id}
  ↓ fetch user by id
  ↓ return 200 + user info
```

---

### `GET /api/auth/identity/{id}` — Get Identity Info
```
IdentityFilter validates JWT sub claim matches {id}
  ↓ retrieve pre-validated identity from HttpContext.Items
  ↓ return 200 + identity info
```

---

### `PUT /api/account/edit/{id}` — Edit Account
```
Authenticated request
  ↓ validate JWT sub claim matches {id}
  ↓ update user details
  ↓ return 200 + { isUpdated, userGuid }
```

---

### `GET /blogs` — Get All Posts
```
Anonymous
  ↓ fetch all published posts
  ↓ return 200 + posts list
```

---

### `GET /blogs/{id}` — Get Post by ID
```
Anonymous
  ↓ fetch post by id
  ↓ return 200 + post
```

---

### `POST /blogs/create/{id}` — Create Post
```
IdentityFilter validates JWT sub claim matches {id}
  ↓ map request to post DTO
  ↓ resolve BlogUser.Id from ApplicationUser.Id
  ↓ save post to DB
  ↓ return 200
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
