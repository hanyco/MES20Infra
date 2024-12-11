using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Infrastructure.Persistence;

public class AspNetUser : IdentityUser
{
    public string? DisplayName { get; set; }
}

public class AspNetUserClaim : IdentityUserClaim<string>;

public class AspNetUserLogin : IdentityUserLogin<string>;

public class AspNetUserToken : IdentityUserToken<string>;

public class IdentityDbContext : IdentityDbContext<AspNetUser>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public IdentityDbContext()
    {

    }

    public virtual DbSet<AccessPermission> AccessPermissions { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=MesInfra;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetUs__3214EC0734F38367");

            entity.HasIndex(e => e.NormalizedUserName, "IX_AspNetUsers_UserName")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.ToTable(nameof(AspNetUser), "Identity");
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            //entity.HasKey(e => e.Id).HasName("PK__AspNetUs__3214EC078E82EDE6");
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            //entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }).HasName("PK__AspNetUs__2B2C5B526632C5C0");
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            //entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name }).HasName("PK__AspNetUs__8CC49841BFF9BBDC");
        });

        _ = modelBuilder.Entity<AccessPermission>(entity =>
        {
            _ = entity.ToTable(nameof(this.AccessPermissions), "infra");

            _ = entity.Property(e => e.AccessScope).HasMaxLength(50);
            _ = entity.Property(e => e.AccessType).HasMaxLength(50);
            _ = entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            _ = entity.Property(e => e.EntityType).HasMaxLength(50);
            _ = entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            _ = entity.Property(e => e.UserId).HasMaxLength(128);
        });

        //// AspNetUserLogins Table
        _ = modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
        {
            _ = entity.ToTable(nameof(AspNetUserLogin), "Identity");
            _ = entity.HasKey(login => new { login.LoginProvider, login.ProviderKey });

            _ = entity.HasOne<AspNetUser>()
                .WithMany()
                .HasForeignKey(login => login.UserId)
                .IsRequired();
        });

        // AspNetUserTokens Table
        _ = modelBuilder.Entity<IdentityUserToken<string>>(entity =>
        {
            _ = entity.ToTable(nameof(AspNetUserToken), "Identity");
            _ = entity.HasKey(token => new { token.UserId, token.LoginProvider, token.Name });
        });

        // AspNetUserClaims Table
        _ = modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
        {
            _ = entity.ToTable(nameof(AspNetUserClaim), "Identity");
            //_ = entity.HasKey(claim => claim.Id);

            _ = entity.HasOne<AspNetUser>()
                .WithMany()
                .HasForeignKey(claim => claim.UserId)
                .IsRequired();
        });

        // AspNetUserRoles Table (Not needed but explicitly mapped as ignored)
        _ = modelBuilder.Entity<IdentityUserRole<string>>(entity =>
        {
            _ = entity.HasNoKey();
            _ = entity.ToTable((string?)null); // Disable this table as it's not in use
        });
    }
}