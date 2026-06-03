# Re-sync ALL model classes from the DB. Regenerates every entity below.
# Use scaffold.ps1 <table> for adding/updating a single table.
$appsettings = Get-Content "PersonalBlog.Api/appsettings.Development.json" -Raw | ConvertFrom-Json
$connString = $appsettings.ConnectionStrings.DefaultConnection

dotnet ef dbcontext scaffold $connString Npgsql.EntityFrameworkCore.PostgreSQL `
  -t blog_users `
  -t posts `
  -t drafts `
  -t refresh_tokens `
  -t email_post_send_events `
  --output-dir ../PersonalBlog.Models/DatabaseModels `
  --context-dir _scaffold `
  --force `
  --project PersonalBlog.Infrastructure `
  --startup-project PersonalBlog.Api
