﻿using Microsoft.AspNetCore.Identity;
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
    private const string SchemaName = "Identity";

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public IdentityDbContext()
    {

    }

    public virtual DbSet<AccessPermission> AccessPermissions { get; set; }

    //public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    //public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    //public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    //public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //// AspNetUser table mapping
        //_ = modelBuilder.Entity<AspNetUser>(entity =>
        //{
        //    _ = entity.ToTable(nameof(AspNetUser), "Identity");

        //    _ = entity.HasIndex(e => e.NormalizedUserName, "IX_AspNetUsers_UserName")
        //        .IsUnique()
        //        .HasFilter("([NormalizedUserName] IS NOT NULL)");

        //    _ = entity.Property(e => e.DisplayName).HasMaxLength(256);
        //});
        //_ = modelBuilder.Ignore<IdentityUser<string>>();

        //// AspNetUserClaim table mapping
        //_ = modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
        //{
        //    _ = entity.ToTable(nameof(AspNetUserClaim), "Identity");

        //    _ = entity.HasOne<AspNetUser>()
        //        .WithMany()
        //        .HasForeignKey(e => e.UserId)
        //        .IsRequired();
        //});

        //// AspNetUserLogin table mapping
        //_ = modelBuilder.Entity<AspNetUserLogin>(entity =>
        //{
        //    _ = entity.ToTable(nameof(AspNetUserLogin), SchemaName);

        //    _ = entity.HasOne<AspNetUser>()
        //        .WithMany()
        //        .HasForeignKey(e => e.UserId)
        //        .IsRequired();
        //});

        //// AspNetUserToken table mapping
        //_ = modelBuilder.Entity<AspNetUserToken>(entity =>
        //{
        //    _ = entity.ToTable(nameof(AspNetUserToken), SchemaName);
        //});

        //// AccessPermissions table mapping
        //_ = modelBuilder.Entity<AccessPermission>(entity =>
        //{
        //    _ = entity.ToTable(nameof(this.AccessPermissions), "infra");

        //    _ = entity.Property(e => e.AccessScope).HasMaxLength(50);
        //    _ = entity.Property(e => e.AccessType).HasMaxLength(50);
        //    _ = entity.Property(e => e.CreatedDate).HasColumnType("datetime");
        //    _ = entity.Property(e => e.EntityType).HasMaxLength(50);
        //    _ = entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
        //    _ = entity.Property(e => e.UserId).HasMaxLength(128);
        //});

        //// Ignoring AspNetUserRoles table
        //_ = modelBuilder.Entity<IdentityUserRole<string>>(entity =>
        //{
        //    _ = entity.ToTable((string?)null); // Disable this table as it's not in use
        //});
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
        {
            options.UseSqlServer("Data Source=.;Initial Catalog=MesInfra;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");
        }
    }
}
