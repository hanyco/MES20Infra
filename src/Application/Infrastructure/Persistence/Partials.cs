using Domain.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Infrastructure.Persistence;
partial class IdentityDbContext
{
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("AspNetUsers", "Identity");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserName).IsRequired().HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.HasMany<IdentityUserLogin<string>>().WithOne()
                .HasForeignKey(e => e.UserId).IsRequired();
            entity.HasMany<IdentityUserToken<string>>().WithOne()
                .HasForeignKey(e => e.UserId).IsRequired();
            entity.HasMany<IdentityUserClaim<string>>().WithOne()
                .HasForeignKey(e => e.UserId).IsRequired();
        });


        modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.HasKey(login => new { login.LoginProvider, login.ProviderKey });
            entity.ToTable("AspNetUserLogins", "Identity");
        });

        modelBuilder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.HasKey(token => new { token.UserId, token.LoginProvider, token.Name });
            entity.ToTable("AspNetUserTokens", "Identity");
        });

        modelBuilder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.HasNoKey();
            entity.ToTable((string?)null);
        });

    }
}
