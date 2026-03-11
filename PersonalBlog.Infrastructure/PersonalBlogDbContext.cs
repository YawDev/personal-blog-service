

namespace PersonalBlog.Infrastructure.DbContext;

public partial class PersonalBlogDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public PersonalBlogDbContext()
    {
    }

    public PersonalBlogDbContext(DbContextOptions<PersonalBlogDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<BlogUser> BlogUsers { get; set; }

    public virtual DbSet<Draft> Drafts { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum("user_role", new[] { "admin", "user" });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
