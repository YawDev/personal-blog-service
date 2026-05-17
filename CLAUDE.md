# Personal Blog — Backend Service

## Tech Stack
- **Runtime**: .NET (ASP.NET Core Web API)
- **Database**: PostgreSQL via Entity Framework Core (Npgsql)
- **Auth**: ASP.NET Core Identity + JWT Bearer tokens
- **Mapping**: AutoMapper
- **API Docs**: Swagger/OpenAPI

## Solution Structure

```
personal-blog-service.sln
├── PersonalBlog.Api          # HTTP layer — controllers, contracts, middleware, filters
├── PersonalBlog.Core         # Business logic, service interfaces, DTOs, exceptions
├── PersonalBlog.Infrastructure # EF Core DbContext, repositories, migrations
└── PersonalBlog.Models       # Database models, enums, error models
```

## Project Responsibilities

### PersonalBlog.Api
- **Controllers**: `AuthenticationController`, `BlogController`
- **ActionFilters**: `IdentityFilterAttribute` — validates that the JWT sub claim matches the `{id}` route param before allowing access; stores the resolved identity in `HttpContext.Items["AuthenticatedIdentity"]`
- **Middleware**: `ExceptionHandlingMiddleware` — centralised exception → HTTP response mapping
- **Contracts**: typed request/response shapes (separate from DTOs)
- **Mapping**: `MapperProfile` — AutoMapper profiles between contracts and DTOs

### PersonalBlog.Core
- **Interfaces**: `IBlogService`, `IAuthenticationService`, `ITokenService`, `IUserIdentityService`, `IBlogRepository`, `IDraftRepository`, `IUserRepository`
- **Services**: `BlogService`, `AuthenticationService`, `TokenService`, `UserIdentityService`
- **DTOs**: internal transfer objects between layers
- **Exceptions**: `BadRequestException`, `FailedAuthenticationException`, `UnauthorizedException`, `UserNotFoundException`

### PersonalBlog.Infrastructure
- **DbContext**: `PersonalBlogDbContext` extends `IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>`
- **Tables**: `blog_users`, `posts`, `drafts` + all ASP.NET Identity tables
- **Repositories**: `BlogRepository`, `DraftRepository`, `UserRepository`
- **Migrations**: EF Core migrations in `Migrations/`

### PersonalBlog.Models
- **Database models**: `ApplicationUser`, `BlogUser`, `Post`, `Draft`
- **Enums**: `UserRole` (`admin`, `user`) — stored as a Postgres enum `user_role`

## API Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/blogs` | Anonymous | Get all published posts |
| GET | `/blogs/{id}` | Anonymous | Get post by ID |
| POST | `/blogs/create/{id}` | IdentityFilter | Create a new post (id = user GUID) |
| POST | `/api/auth/login` | Anonymous | Login — sets HttpOnly JWT cookie (30 min) |
| POST | `/api/auth/register` | Anonymous | Register new user |
| POST | `/api/auth/logout` | Authenticated | Sign out |
| GET | `/api/auth/user/{id}` | IdentityFilter | Get user info |
| GET | `/api/auth/identity/{id}` | IdentityFilter | Get identity info |

## Authentication Flow
1. `POST /api/auth/login` validates credentials, returns a `LoginResponse` with user data, and sets an `access_token` HttpOnly cookie (Secure, SameSite=None, 30 min expiry).
2. Protected endpoints use `[IdentityFilter]` which extracts the JWT sub claim, resolves the identity user, and compares it to the `{id}` route param — throws `UnauthorizedException` on mismatch.
3. JWT config lives in `appsettings.json` under `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`.

## Database Models

**Post** — `posts` table
- `Id` (Guid), `Title`, `Content`, `Preview` (max 500), `Dateposted`, `Userid` (FK → blog_users), `Createddate`, `Lastmodifieddate`

**Draft** — `drafts` table
- `Id` (Guid), `Title` (max 200), `Content`, `Preview` (max 500), `Userid` (FK → blog_users), `Createdon`, `Lastmodifieddate`

**BlogUser** — `blog_users` table
- `Id` (Guid), `Username` (unique, max 50), `Email` (unique, max 320), `Displayname` (max 100), `Avatar`, `Role` (UserRole enum), `Createddate`, `Lastmodifieddate`

**ApplicationUser** — ASP.NET Identity user (linked to BlogUser by shared Guid Id)

## Implementation Status
- **Done**: Auth (register, login, logout, identity resolution), GET all posts, GET post by ID, POST create post
- **Not implemented** (`throw new NotImplementedException()`): draft CRUD, update post, delete post, get posts by user

## CORS
Allowed origins are loaded from `CorsOriginSettings:DomainList` in config (array of strings). Policy name: `AllowFrontend`.

## Running Locally
- Requires PostgreSQL connection string at `ConnectionStrings:DefaultConnection` in `appsettings.Development.json`
- Requires `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` in config
- Run with `dotnet run` from `PersonalBlog.Api/`
- Swagger UI available at `/swagger` in Development
