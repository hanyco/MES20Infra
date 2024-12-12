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
        base.OnModelCreating(modelBuilder);

        // AspNetUser table mapping
        _ = modelBuilder.Entity<AspNetUser>(entity =>
        {
            _ = entity.ToTable(nameof(AspNetUser), "Identity");
            //entity.HasKey(e => e.Id).HasName("PK_AspNetUsers");

            _ = entity.HasIndex(e => e.NormalizedUserName, "IX_AspNetUsers_UserName")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            _ = entity.Property(e => e.DisplayName).HasMaxLength(256);
        });

        // AspNetUserClaim table mapping
        _ = modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            _ = entity.ToTable(nameof(AspNetUserClaim), "Identity");
            //entity.HasKey(e => e.Id).HasName("PK_AspNetUserClaims");

            _ = entity.HasOne<AspNetUser>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .IsRequired();
        });

        // AspNetUserLogin table mapping
        _ = modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            _ = entity.ToTable(nameof(AspNetUserLogin), "Identity");
            //entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }).HasName("PK_AspNetUserLogins");

            _ = entity.HasOne<AspNetUser>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .IsRequired();
        });

        // AspNetUserToken table mapping
        _ = modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            _ = entity.ToTable(nameof(AspNetUserToken), "Identity");
            //entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name }).HasName("PK_AspNetUserTokens");
        });

        // AccessPermissions table mapping
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

        // Ignoring AspNetUserRoles table
        _ = modelBuilder.Entity<IdentityUserRole<string>>(entity =>
        {
            _ = entity.HasNoKey();
            _ = entity.ToTable((string?)null); // Disable this table as it's not in use
        });
    }
}
