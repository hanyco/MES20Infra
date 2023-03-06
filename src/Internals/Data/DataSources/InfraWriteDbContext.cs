using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HanyCo.Infra.Internals.Data.DataSources
{
    public partial class InfraWriteDbContext
    {
        public InfraWriteDbContext()
        {
        }

        public InfraWriteDbContext(DbContextOptions<InfraWriteDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CqrsSegregate> CqrsSegregates { get; set; } = null!;
        public virtual DbSet<CrudCode> CrudCodes { get; set; } = null!;
        public virtual DbSet<Dto> Dtos { get; set; } = null!;
        public virtual DbSet<EntitySecurity> EntitySecurities { get; set; } = null!;
        public virtual DbSet<Functionality> Functionalities { get; set; } = null!;
        public virtual DbSet<Module> Modules { get; set; } = null!;
        public virtual DbSet<Property> Properties { get; set; } = null!;
        public virtual DbSet<SecurityClaim> SecurityClaims { get; set; } = null!;
        public virtual DbSet<SecurityDescriptor> SecurityDescriptors { get; set; } = null!;
        public virtual DbSet<SystemMenu> SystemMenus { get; set; } = null!;
        public virtual DbSet<Translation> Translations { get; set; } = null!;
        public virtual DbSet<UiBootstrapPosition> UiBootstrapPositions { get; set; } = null!;
        public virtual DbSet<UiComponent> UiComponents { get; set; } = null!;
        public virtual DbSet<UiComponentAction> UiComponentActions { get; set; } = null!;
        public virtual DbSet<UiComponentProperty> UiComponentProperties { get; set; } = null!;
        public virtual DbSet<UiPage> UiPages { get; set; } = null!;
        public virtual DbSet<UiPageComponent> UiPageComponents { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.;Database=MesInfra;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CqrsSegregate>(entity =>
            {
                entity.ToTable("CqrsSegregate", "infra");

                entity.HasIndex(e => e.ResultDtoId, "IX_CqrsSegregate_ResultDtoId");

                entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.Functionality)
                    .WithMany(p => p.CqrsSegregates)
                    .HasForeignKey(d => d.FunctionalityId)
                    .HasConstraintName("FK_CqrsSegregate_Functionality");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.CqrsSegregates)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CqrsSegregate_Module");

                entity.HasOne(d => d.ParamDto)
                    .WithMany(p => p.CqrsSegregateParamDtos)
                    .HasForeignKey(d => d.ParamDtoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CqrsSegregate_Dto");

                entity.HasOne(d => d.ResultDto)
                    .WithMany(p => p.CqrsSegregateResultDtos)
                    .HasForeignKey(d => d.ResultDtoId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CrudCode>(entity =>
            {
                entity.ToTable("CrudCode", "infra");

                entity.Property(e => e.DbObjectId).HasMaxLength(50);

                entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Name).HasMaxLength(1024);

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.CrudCodes)
                    .HasForeignKey(d => d.ModuleId)
                    .HasConstraintName("FK_CrudCode_Module");
            });

            modelBuilder.Entity<Dto>(entity =>
            {
                entity.ToTable("Dto", "infra");

                entity.HasIndex(e => e.ModuleId, "IX_Dto_ModuleId");

                entity.Property(e => e.Comment).HasMaxLength(50);

                entity.Property(e => e.DbObjectId).HasMaxLength(50);

                entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameSpace).HasMaxLength(1024);

                entity.HasOne(d => d.Functionality)
                    .WithMany(p => p.Dtos)
                    .HasForeignKey(d => d.FunctionalityId)
                    .HasConstraintName("FK_Dto_Functionality");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.Dtos)
                    .HasForeignKey(d => d.ModuleId);
            });

            modelBuilder.Entity<EntitySecurity>(entity =>
            {
                entity.ToTable("EntitySecurity", "infra");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.SecurityDescriptor)
                    .WithMany(p => p.EntitySecurities)
                    .HasForeignKey(d => d.SecurityDescriptorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EntitySecurity_SecurityDescriptor");
            });

            modelBuilder.Entity<Functionality>(entity =>
            {
                entity.ToTable("Functionality", "infra");

                entity.Property(e => e.Comment).HasMaxLength(50);

                entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.ToTable("Module", "infra");

                entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.ToTable("Property", "infra");

                entity.Property(e => e.DbObjectId).HasMaxLength(50);

                entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.Dto)
                    .WithMany(p => p.Properties)
                    .HasForeignKey(d => d.DtoId)
                    .HasConstraintName("FK_Property_Dto");
            });

            modelBuilder.Entity<SecurityClaim>(entity =>
            {
                entity.ToTable("SecurityClaim", "infra");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.ClaimType).HasMaxLength(50);

                entity.Property(e => e.ClaimValue).HasMaxLength(50);

                entity.HasOne(d => d.SecurityDescriptor)
                    .WithMany(p => p.SecurityClaims)
                    .HasForeignKey(d => d.SecurityDescriptorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SecurityClaim_SecurityDescriptor1");
            });

            modelBuilder.Entity<SecurityDescriptor>(entity =>
            {
                entity.ToTable("SecurityDescriptor", "infra");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<SystemMenu>(entity =>
            {
                entity.ToTable("SystemMenu", "infra");

                entity.Property(e => e.Caption).HasMaxLength(1024);

                entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<Translation>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Translation", "infra");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.LangCode)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<UiBootstrapPosition>(entity =>
            {
                entity.ToTable("UiBootstrapPosition", "infra");
            });

            modelBuilder.Entity<UiComponent>(entity =>
            {
                entity.ToTable("UiComponent", "infra");

                entity.Property(e => e.ClassName).HasMaxLength(50);

                entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.PageDataContext)
                    .WithMany(p => p.UiComponents)
                    .HasForeignKey(d => d.PageDataContextId)
                    .HasConstraintName("FK_UiComponent_Dto1");

                entity.HasOne(d => d.PageDataContextProperty)
                    .WithMany(p => p.UiComponents)
                    .HasForeignKey(d => d.PageDataContextPropertyId)
                    .HasConstraintName("FK_UiComponent_Property");
            });

            modelBuilder.Entity<UiComponentAction>(entity =>
            {
                entity.ToTable("UiComponentAction", "infra");

                entity.Property(e => e.Id).HasComment("مکان در کامپوننت");

                entity.Property(e => e.Caption).HasMaxLength(1024);

                entity.Property(e => e.EventHandlerName).HasMaxLength(1024);

                entity.Property(e => e.Name).HasMaxLength(1024);

                entity.HasOne(d => d.CqrsSegregate)
                    .WithMany(p => p.UiComponentActions)
                    .HasForeignKey(d => d.CqrsSegregateId)
                    .HasConstraintName("FK_UiComponentAction_CqrsSegregate");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.UiComponentActions)
                    .HasForeignKey(d => d.PositionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UiComponentAction_UiBootstrapPosition");

                entity.HasOne(d => d.UiComponent)
                    .WithMany(p => p.UiComponentActions)
                    .HasForeignKey(d => d.UiComponentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UiComponentAction_UiComponent");
            });

            modelBuilder.Entity<UiComponentProperty>(entity =>
            {
                entity.ToTable("UiComponentProperty", "infra");

                entity.Property(e => e.Caption).HasMaxLength(50);

                entity.Property(e => e.PositionId).HasComment("مکان در کامپوننت");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.UiComponentProperties)
                    .HasForeignKey(d => d.PositionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UiComponentProperty_UiBootstrapPosition");

                entity.HasOne(d => d.Property)
                    .WithMany(p => p.UiComponentProperties)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_UiComponentProperty_Property1");

                entity.HasOne(d => d.UiComponent)
                    .WithMany(p => p.UiComponentProperties)
                    .HasForeignKey(d => d.UiComponentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UiComponentProperty_UiComponent");
            });

            modelBuilder.Entity<UiPage>(entity =>
            {
                entity.ToTable("UiPage", "infra");

                entity.Property(e => e.ClassName).HasMaxLength(50);

                entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.Dto)
                    .WithMany(p => p.UiPages)
                    .HasForeignKey(d => d.DtoId)
                    .HasConstraintName("FK_UiPage_Dto");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.UiPages)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UiPage_Module");
            });

            modelBuilder.Entity<UiPageComponent>(entity =>
            {
                entity.ToTable("UiPageComponent", "infra");

                entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.PositionId).HasComment("مکان در پیج");

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.UiPageComponents)
                    .HasForeignKey(d => d.PageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UiPageComponent_UiPage");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.UiPageComponents)
                    .HasForeignKey(d => d.PositionId)
                    .HasConstraintName("FK_UiPageComponent_UiBootstrapPosition");

                entity.HasOne(d => d.UiComponent)
                    .WithMany(p => p.UiPageComponents)
                    .HasForeignKey(d => d.UiComponentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UiPageComponent_UiComponent");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
