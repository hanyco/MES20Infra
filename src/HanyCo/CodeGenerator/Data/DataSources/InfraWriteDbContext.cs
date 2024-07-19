using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.Internals.Data.DataSources;

public partial class InfraWriteDbContext : DbContext
{
    public InfraWriteDbContext()
    {
    }

    public InfraWriteDbContext(DbContextOptions<InfraWriteDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CqrsSegregate> CqrsSegregates { get; set; }

    public virtual DbSet<CrudCode> CrudCodes { get; set; }

    public virtual DbSet<Dto> Dtos { get; set; }

    public virtual DbSet<EntityClaim> EntityClaims { get; set; }

    public virtual DbSet<Functionality> Functionalities { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Property> Properties { get; set; }

    public virtual DbSet<SecurityClaim> SecurityClaims { get; set; }

    public virtual DbSet<SystemMenu> SystemMenus { get; set; }

    public virtual DbSet<Translation> Translations { get; set; }

    public virtual DbSet<UiBootstrapPosition> UiBootstrapPositions { get; set; }

    public virtual DbSet<UiComponent> UiComponents { get; set; }

    public virtual DbSet<UiComponentAction> UiComponentActions { get; set; }

    public virtual DbSet<UiComponentProperty> UiComponentProperties { get; set; }

    public virtual DbSet<UiPage> UiPages { get; set; }

    public virtual DbSet<UiPageComponent> UiPageComponents { get; set; }

    public virtual DbSet<UserClaimAccess> UserClaimAccesses { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=.;Database=MesInfra;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CqrsSegregate>(entity =>
        {
            entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Module).WithMany(p => p.CqrsSegregates)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CqrsSegregate_Module");

            entity.HasOne(d => d.ParamDto).WithMany(p => p.CqrsSegregateParamDtos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CqrsSegregate_Dto");

            entity.HasOne(d => d.ResultDto).WithMany(p => p.CqrsSegregateResultDtos).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CrudCode>(entity =>
        {
            entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");

            entity.HasOne(d => d.Module).WithMany(p => p.CrudCodes).HasConstraintName("FK_CrudCode_Module");
        });

        modelBuilder.Entity<Dto>(entity =>
        {
            entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<EntityClaim>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Claim).WithMany(p => p.EntityClaims).HasConstraintName("FK_EntityClaim_SecurityClaim");
        });

        modelBuilder.Entity<Functionality>(entity =>
        {
            entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.DeleteCommand).WithMany(p => p.FunctionalityDeleteCommands)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Functionality_CqrsSegregate4");

            entity.HasOne(d => d.GetAllQuery).WithMany(p => p.FunctionalityGetAllQueries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Functionality_CqrsSegregate1");

            entity.HasOne(d => d.GetByIdQuery).WithMany(p => p.FunctionalityGetByIdQueries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Functionality_CqrsSegregate");

            entity.HasOne(d => d.InsertCommand).WithMany(p => p.FunctionalityInsertCommands)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Functionality_CqrsSegregate2");

            entity.HasOne(d => d.Module).WithMany(p => p.Functionalities)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Functionality_Module");

            entity.HasOne(d => d.SourceDto).WithMany(p => p.Functionalities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Functionality_Dto1");

            entity.HasOne(d => d.UpdateCommand).WithMany(p => p.FunctionalityUpdateCommands)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Functionality_CqrsSegregate3");
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Dto).WithMany(p => p.Properties)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Property_Dto");
        });

        modelBuilder.Entity<SecurityClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_SecurityClaim_1");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<SystemMenu>(entity =>
        {
            entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<Translation>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.LangCode).IsFixedLength();
        });

        modelBuilder.Entity<UiBootstrapPosition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_BootstrapPosition");
        });

        modelBuilder.Entity<UiComponent>(entity =>
        {
            entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");

            entity.HasOne(d => d.PageDataContext).WithMany(p => p.UiComponents).HasConstraintName("FK_UiComponent_Dto1");

            entity.HasOne(d => d.PageDataContextProperty).WithMany(p => p.UiComponents).HasConstraintName("FK_UiComponent_Property");
        });

        modelBuilder.Entity<UiComponentAction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UiComponentAction_1");

            entity.Property(e => e.Id).HasComment("مکان در کامپوننت");

            entity.HasOne(d => d.CqrsSegregate).WithMany(p => p.UiComponentActions).HasConstraintName("FK_UiComponentAction_CqrsSegregate");

            entity.HasOne(d => d.Position).WithMany(p => p.UiComponentActions).HasConstraintName("FK_UiComponentAction_UiBootstrapPosition");

            entity.HasOne(d => d.UiComponent).WithMany(p => p.UiComponentActions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UiComponentAction_UiComponent");
        });

        modelBuilder.Entity<UiComponentProperty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UiComponentProperty_1");

            entity.Property(e => e.PositionId).HasComment("مکان در کامپوننت");

            entity.HasOne(d => d.Position).WithMany(p => p.UiComponentProperties).HasConstraintName("FK_UiComponentProperty_UiBootstrapPosition");

            entity.HasOne(d => d.Property).WithMany(p => p.UiComponentProperties)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UiComponentProperty_Property1");

            entity.HasOne(d => d.UiComponent).WithMany(p => p.UiComponentProperties)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UiComponentProperty_UiComponent");
        });

        modelBuilder.Entity<UiPage>(entity =>
        {
            entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");

            entity.HasOne(d => d.Dto).WithMany(p => p.UiPages).HasConstraintName("FK_UiPage_Dto");

            entity.HasOne(d => d.Module).WithMany(p => p.UiPages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UiPage_Module");
        });

        modelBuilder.Entity<UiPageComponent>(entity =>
        {
            entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.PositionId).HasComment("مکان در پیج");

            entity.HasOne(d => d.Page).WithMany(p => p.UiPageComponents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UiPageComponent_UiPage");

            entity.HasOne(d => d.Position).WithMany(p => p.UiPageComponents)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UiPageComponent_UiBootstrapPosition");

            entity.HasOne(d => d.UiComponent).WithMany(p => p.UiPageComponents).HasConstraintName("FK_UiPageComponent_UiComponent");
        });

        modelBuilder.Entity<UserClaimAccess>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Claim).WithMany(p => p.UserClaimAccesses).HasConstraintName("FK_UserClaimAccess_SecurityClaim");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
