
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalBlog.Models.DatabaseModels;

namespace PersonalBlog.Infrastructure;

public partial class PersonalBlogDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public PersonalBlogDbContext()
    {
    }

    public PersonalBlogDbContext(DbContextOptions<PersonalBlogDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BlogUser> BlogUsers { get; set; }
    public virtual DbSet<Draft> Drafts { get; set; }
    public virtual DbSet<Post> Posts { get; set; }
    public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresEnum("user_role", new[] { "admin", "user" });

        modelBuilder.Entity<BlogUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("blog_users_pkey");

            entity.ToTable("blog_users");

            entity.HasIndex(e => e.Email, "blog_users_email_key").IsUnique();

            entity.HasIndex(e => e.Username, "blog_users_username_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Avatar).HasColumnName("avatar");
            entity.Property(e => e.Createddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("createddate");
            entity.Property(e => e.Displayname)
                .HasMaxLength(100)
                .HasColumnName("displayname");
            entity.Property(e => e.Email)
                .HasMaxLength(320)
                .HasColumnName("email");
            entity.Property(e => e.Lastmodifieddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("lastmodifieddate");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
            entity.Property(e => e.Role)
                .HasColumnName("role")
                .HasConversion<string>();
        });

        modelBuilder.Entity<Draft>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("drafts_pkey");

            entity.ToTable("drafts");

            entity.HasIndex(e => e.Createdon, "idx_drafts_created_on").IsDescending();

            entity.HasIndex(e => e.Title, "idx_drafts_title");

            entity.HasIndex(e => e.Userid, "idx_drafts_user");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Createdon)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("createdon");
            entity.Property(e => e.Lastmodifieddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("lastmodifieddate");
            entity.Property(e => e.Preview)
                .HasMaxLength(500)
                .HasColumnName("preview");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Drafts)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("fk_drafts_user");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("posts_pkey");

            entity.ToTable("posts");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Createddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("createddate");
            entity.Property(e => e.Dateposted)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("dateposted");
            entity.Property(e => e.Lastmodifieddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("lastmodifieddate");
            entity.Property(e => e.Preview)
                .HasMaxLength(500)
                .HasColumnName("preview");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("fk_posts_user");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refresh_tokens_pkey");

            entity.ToTable("refresh_tokens");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Token)
                .HasMaxLength(500)
                .HasColumnName("token");
            entity.Property(e => e.IdentityUserId).HasColumnName("identity_user_id");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.IsRevoked).HasColumnName("is_revoked");
            entity.Property(e => e.IsUsed).HasColumnName("is_used");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");

            entity.Ignore(e => e.IsExpired);
            entity.Ignore(e => e.IsActive);

            entity.HasOne(e => e.IdentityUser)
                .WithMany()
                .HasForeignKey(e => e.IdentityUserId)
                .HasConstraintName("fk_refresh_tokens_identity_user");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
