# personal-blog-service

Personal Blog Service with API endpoints built using .Net 9 with PostgreSQL DB

## Entity Framework Commands

```bash
# Scaffold database models from existing database
dotnet ef dbcontext scaffold "Host=localhost;Database=personal_blog;Username=your_username;Password=your_password" Npgsql.EntityFrameworkCore.PostgreSQL --project PersonalBlog.Models --startup-project PersonalBlog.Api --context-dir . --output-dir DatabaseModels --force

# Create migration
dotnet ef migrations add IdentityAndLink --project PersonalBlog.Infrastructure --startup-project PersonalBlog.Api

# Update database
dotnet ef database update --project PersonalBlog.Infrastructure --startup-project PersonalBlog.Api
```
