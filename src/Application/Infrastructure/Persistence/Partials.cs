using Domain.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Infrastructure.Persistence;

public partial class IdentityDbContext
{
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<ApplicationUser>(entity =>
        {
            _ = entity.ToTable("ApplicationUsers", "Identity")
                .HasKey(e => e.Id);
            _ = entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(256);
            _ = entity.Property(e => e.NormalizedUserName)
                .HasMaxLength(256);
            _ = entity.Property(e => e.Email)
                .HasMaxLength(256);
            _ = entity.Property(e => e.NormalizedEmail)
                .HasMaxLength(256);

            _ = entity.HasMany<IdentityUserLogin<string>>().WithOne()
                .HasForeignKey(e => e.UserId).IsRequired();
            _ = entity.HasMany<IdentityUserToken<string>>().WithOne()
                .HasForeignKey(e => e.UserId).IsRequired();
            _ = entity.HasMany<IdentityUserClaim<string>>().WithOne()
                .HasForeignKey(e => e.UserId).IsRequired();
        });

        _ = modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
        {
            _ = entity.HasKey(login => new { login.LoginProvider, login.ProviderKey });
            _ = entity.ToTable("AspNetUserLogins", "Identity");
            _ = entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(login => login.UserId)
                .IsRequired();
        });

        _ = modelBuilder.Entity<IdentityUserToken<string>>(entity =>
        {
            _ = entity.HasKey(token => new { token.UserId, token.LoginProvider, token.Name });
            _ = entity.ToTable("AspNetUserTokens", "Identity");
        });

        _ = modelBuilder.Entity<IdentityUserRole<string>>(entity =>
        {
            _ = entity.HasNoKey();
            _ = entity.ToTable((string?)null);
        });
    }

}
