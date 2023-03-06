using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

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

        public virtual DbSet<BootstrapPosition> BootstrapPositions { get; set; }
        public virtual DbSet<CqrsSegregate> CqrsSegregates { get; set; }
        public virtual DbSet<CrudCode> CrudCodes { get; set; }
        public virtual DbSet<Dto> Dtos { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Property> Properties { get; set; }
        public virtual DbSet<UiComponent> UiComponents { get; set; }
        public virtual DbSet<UiComponentAction> UiComponentActions { get; set; }
        public virtual DbSet<UiComponentProperty> UiComponentProperties { get; set; }
        public virtual DbSet<UiPage> UiPages { get; set; }
        public virtual DbSet<UiPageComponent> UiPageComponents { get; set; }

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
                entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.CqrsSegregates)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CqrsSegregate_Module");

                entity.HasOne(d => d.ParamDto)
                    .WithMany(p => p.CqrsSegregateParamDtos)
                    .HasForeignKey(d => d.ParamDtoId)
                    .HasConstraintName("FK_CqrsSegregate_Dto");
            });

            modelBuilder.Entity<CrudCode>(entity =>
            {
                entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.CrudCodes)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CrudCode_Module");
            });

            modelBuilder.Entity<Dto>(entity =>
            {
                entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");
            });

            modelBuilder.Entity<UiComponent>(entity =>
            {
                entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");

                entity.HasOne(d => d.Dto)
                    .WithMany(p => p.UiComponents)
                    .HasForeignKey(d => d.DtoId)
                    .HasConstraintName("FK_UiComponent_Dto");
            });

            modelBuilder.Entity<UiComponentAction>(entity =>
            {
                entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");

                entity.HasOne(d => d.CqrsSegregation)
                    .WithMany(p => p.UiComponentActions)
                    .HasForeignKey(d => d.CqrsSegregationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UiComponentAction_CqrsSegregate");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.UiComponentActions)
                    .HasForeignKey(d => d.PositionId)
                    .HasConstraintName("FK_UiComponentAction_BootstrapPosition");
            });

            modelBuilder.Entity<UiComponentProperty>(entity =>
            {
                entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.UiComponentProperties)
                    .HasForeignKey(d => d.PositionId)
                    .HasConstraintName("FK_UiComponentProperty_BootstrapPosition");

                entity.HasOne(d => d.Property)
                    .WithMany(p => p.UiComponentProperties)
                    .HasForeignKey(d => d.PropertyId)
                    .HasConstraintName("FK_UiComponentProperty_Property");

                entity.HasOne(d => d.UiComponent)
                    .WithMany(p => p.UiComponentProperties)
                    .HasForeignKey(d => d.UiComponentId)
                    .HasConstraintName("FK_UiComponentProperty_UiComponent");
            });

            modelBuilder.Entity<UiPage>(entity =>
            {
                entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");
            });

            modelBuilder.Entity<UiPageComponent>(entity =>
            {
                entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.UiPageComponents)
                    .HasForeignKey(d => d.PageId)
                    .HasConstraintName("FK_UiPageComponent_UiPage");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.UiPageComponents)
                    .HasForeignKey(d => d.PositionId)
                    .HasConstraintName("FK_UiPageComponent_BootstrapPosition");

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
