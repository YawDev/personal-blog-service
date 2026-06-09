# Scaffold a single table's model class from the DB.
# usage: pwsh scaffold.ps1 email_post_send_events
param([Parameter(Mandatory)] [string]$Table)

$appsettings = Get-Content "PersonalBlog.Api/appsettings.Development.json" -Raw | ConvertFrom-Json
$connString  = $appsettings.ConnectionStrings.DefaultConnection

dotnet ef dbcontext scaffold $connString Npgsql.EntityFrameworkCore.PostgreSQL `
  -t $Table `
  --output-dir ../PersonalBlog.Models/DatabaseModels `
  --context-dir _scaffold `
  --force `
  --project PersonalBlog.Infrastructure `
  --startup-project PersonalBlog.Api
