using Domain.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Infrastructure.Persistence;

public partial class IdentityDbContext
{
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // ApplicationUser to AspNetUsers
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("AspNetUsers", "Identity"); // Map to correct table and schema
            entity.HasKey(e => e.Id);

            entity.Property(e => e.UserName).IsRequired().HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.DisplayName).HasMaxLength(256); // Persist DisplayName

            // Relationships
            entity.HasMany<IdentityUserLogin<string>>().WithOne()
                .HasForeignKey(login => login.UserId)
                .IsRequired();

            entity.HasMany<IdentityUserToken<string>>().WithOne()
                .HasForeignKey(token => token.UserId)
                .IsRequired();

            entity.HasMany<IdentityUserClaim<string>>().WithOne()
                .HasForeignKey(claim => claim.UserId)
                .IsRequired();
        });

        // AspNetUserLogins Table
        modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("AspNetUserLogins", "Identity");
            entity.HasKey(login => new { login.LoginProvider, login.ProviderKey });

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(login => login.UserId)
                .IsRequired();
        });

        // AspNetUserTokens Table
        modelBuilder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("AspNetUserTokens", "Identity");
            entity.HasKey(token => new { token.UserId, token.LoginProvider, token.Name });
        });

        // AspNetUserClaims Table
        modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("AspNetUserClaims", "Identity");
            entity.HasKey(claim => claim.Id);

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(claim => claim.UserId)
                .IsRequired();
        });

        // AspNetUserRoles Table (Not needed but explicitly mapped as ignored)
        modelBuilder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.HasNoKey();
            entity.ToTable((string?)null); // Disable this table as it's not in use
        });
    }
}
