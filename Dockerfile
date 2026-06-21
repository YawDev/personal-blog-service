# syntax=docker/dockerfile:1

# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution + project files first so `restore` is cached unless deps change
COPY personal-blog-service.sln ./
COPY PersonalBlog.Api/PersonalBlog.Api.csproj PersonalBlog.Api/
COPY PersonalBlog.Core/PersonalBlog.Core.csproj PersonalBlog.Core/
COPY PersonalBlog.Infrastructure/PersonalBlog.Infrastructure.csproj PersonalBlog.Infrastructure/
COPY PersonalBlog.Models/PersonalBlog.Models.csproj PersonalBlog.Models/
RUN dotnet restore PersonalBlog.Api/PersonalBlog.Api.csproj

# Copy the rest of the source and publish
COPY . .
RUN dotnet publish PersonalBlog.Api/PersonalBlog.Api.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

# The aspnet:9.0 image already defaults to port 8080; set explicitly for clarity
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "PersonalBlog.Api.dll"]
