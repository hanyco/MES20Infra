using System;
using System.Collections.Generic;
using Domain.Identity;


using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Infrastructure.Persistence;

public partial class IdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccessPermission> AccessPermissions { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Controller> Controllers { get; set; }

    public virtual DbSet<ControllerMethod> ControllerMethods { get; set; }

    public virtual DbSet<CqrsSegregate> CqrsSegregates { get; set; }

    public virtual DbSet<Dto> Dtos { get; set; }

    public virtual DbSet<Functionality> Functionalities { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<Property> Properties { get; set; }

    public virtual DbSet<SystemMenu> SystemMenus { get; set; }

    public virtual DbSet<Translation> Translations { get; set; }

    public virtual DbSet<UiBootstrapPosition> UiBootstrapPositions { get; set; }

    public virtual DbSet<UiComponent> UiComponents { get; set; }

    public virtual DbSet<UiComponentAction> UiComponentActions { get; set; }

    public virtual DbSet<UiComponentProperty> UiComponentProperties { get; set; }

    public virtual DbSet<UiPage> UiPages { get; set; }

    public virtual DbSet<UiPageComponent> UiPageComponents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=MesInfra;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccessPermission>(entity =>
        {
            entity.ToTable("AccessPermissions", "infra");

            entity.Property(e => e.AccessScope).HasMaxLength(50);
            entity.Property(e => e.AccessType).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EntityType).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasMaxLength(128);
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetRo__3214EC07D46D4B6A");

            entity.ToTable("AspNetRoles", "Identity");

            entity.HasIndex(e => e.NormalizedName, "IX_AspNetRoles_RoleName")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetRo__3214EC077556A0A6");

            entity.ToTable("AspNetRoleClaims", "Identity");

            entity.Property(e => e.RoleId).HasMaxLength(128);

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__AspNetRol__RoleI__725BF7F6");
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetUs__3214EC077D64BAD9");

            entity.ToTable("AspNetUsers", "Identity");

            entity.HasIndex(e => e.NormalizedUserName, "IX_AspNetUsers_UserName")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.DisplayName).HasMaxLength(256);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK__AspNetUse__RoleI__6CA31EA0"),
                    l => l.HasOne<AspNetUser>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__AspNetUse__UserI__6BAEFA67"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK__AspNetUs__AF2760AD2B656253");
                        j.ToTable("AspNetUserRoles", "Identity");
                        j.IndexerProperty<string>("UserId").HasMaxLength(128);
                        j.IndexerProperty<string>("RoleId").HasMaxLength(128);
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetUs__3214EC079F9089E3");

            entity.ToTable("AspNetUserClaims", "Identity");

            entity.Property(e => e.UserId).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__AspNetUse__UserI__6F7F8B4B");
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }).HasName("PK__AspNetUs__2B2C5B52042E4424");

            entity.ToTable("AspNetUserLogins", "Identity");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);
            entity.Property(e => e.UserId).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__AspNetUse__UserI__753864A1");
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name }).HasName("PK__AspNetUs__8CC498411CFAC034");

            entity.ToTable("AspNetUserTokens", "Identity");

            entity.Property(e => e.UserId).HasMaxLength(128);
            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__AspNetUse__UserI__7814D14C");
        });

        modelBuilder.Entity<Controller>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Controll__3214EC07E36E1F9C");

            entity.ToTable("Controller", "infra");

            entity.Property(e => e.ControllerName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ControllerRoute)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NameSpace)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Module).WithMany(p => p.Controllers)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("FK_Controller_Module");
        });

        modelBuilder.Entity<ControllerMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ApiMetho__3214EC07E6131459");

            entity.ToTable("ControllerMethod", "infra");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ReturnType)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Controller).WithMany(p => p.ControllerMethods)
                .HasForeignKey(d => d.ControllerId)
                .HasConstraintName("FK__ApiMethod__Contr__2B0A656D");
        });

        modelBuilder.Entity<CqrsSegregate>(entity =>
        {
            entity.ToTable("CqrsSegregate", "infra");

            entity.HasIndex(e => e.ModuleId, "IX_CqrsSegregate_ModuleId");

            entity.HasIndex(e => e.ParamDtoId, "IX_CqrsSegregate_ParamDtoId");

            entity.HasIndex(e => e.ResultDtoId, "IX_CqrsSegregate_ResultDtoId");

            entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Module).WithMany(p => p.CqrsSegregates)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CqrsSegregate_Module");

            entity.HasOne(d => d.ParamDto).WithMany(p => p.CqrsSegregateParamDtos)
                .HasForeignKey(d => d.ParamDtoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CqrsSegregate_Dto");

            entity.HasOne(d => d.ResultDto).WithMany(p => p.CqrsSegregateResultDtos)
                .HasForeignKey(d => d.ResultDtoId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Dto>(entity =>
        {
            entity.ToTable("Dto", "infra");

            entity.HasIndex(e => e.ModuleId, "IX_Dto_ModuleId");

            entity.Property(e => e.BaseType).HasMaxLength(1024);
            entity.Property(e => e.Comment).HasMaxLength(50);
            entity.Property(e => e.DbObjectId).HasMaxLength(50);
            entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.NameSpace).HasMaxLength(1024);

            entity.HasOne(d => d.Module).WithMany(p => p.Dtos).HasForeignKey(d => d.ModuleId);
        });

        modelBuilder.Entity<Functionality>(entity =>
        {
            entity.ToTable("Functionality", "infra");

            entity.HasIndex(e => e.DeleteCommandId, "IX_Functionality_DeleteCommandId");

            entity.HasIndex(e => e.GetAllQueryId, "IX_Functionality_GetAllQueryId");

            entity.HasIndex(e => e.GetByIdQueryId, "IX_Functionality_GetByIdQueryId");

            entity.HasIndex(e => e.InsertCommandId, "IX_Functionality_InsertCommandId");

            entity.HasIndex(e => e.ModuleId, "IX_Functionality_ModuleId");

            entity.HasIndex(e => e.SourceDtoId, "IX_Functionality_SourceDtoId");

            entity.HasIndex(e => e.UpdateCommandId, "IX_Functionality_UpdateCommandId");

            entity.Property(e => e.Comment).HasMaxLength(50);
            entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Controller).WithMany(p => p.Functionalities)
                .HasForeignKey(d => d.ControllerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Functionality_Controller");

            entity.HasOne(d => d.DeleteCommand).WithMany(p => p.FunctionalityDeleteCommands)
                .HasForeignKey(d => d.DeleteCommandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Functionality_CqrsSegregate4");

            entity.HasOne(d => d.GetAllQuery).WithMany(p => p.FunctionalityGetAllQueries)
                .HasForeignKey(d => d.GetAllQueryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Functionality_CqrsSegregate1");

            entity.HasOne(d => d.GetByIdQuery).WithMany(p => p.FunctionalityGetByIdQueries)
                .HasForeignKey(d => d.GetByIdQueryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Functionality_CqrsSegregate");

            entity.HasOne(d => d.InsertCommand).WithMany(p => p.FunctionalityInsertCommands)
                .HasForeignKey(d => d.InsertCommandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Functionality_CqrsSegregate2");

            entity.HasOne(d => d.Module).WithMany(p => p.Functionalities)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("FK_Functionality_Module");

            entity.HasOne(d => d.SourceDto).WithMany(p => p.Functionalities)
                .HasForeignKey(d => d.SourceDtoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Functionality_Dto1");

            entity.HasOne(d => d.UpdateCommand).WithMany(p => p.FunctionalityUpdateCommands)
                .HasForeignKey(d => d.UpdateCommandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Functionality_CqrsSegregate3");
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.ToTable("Module", "infra");

            entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("Person");

            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.ToTable("Property", "infra");

            entity.HasIndex(e => e.DtoId, "IX_Property_DtoId");

            entity.Property(e => e.DbObjectId).HasMaxLength(50);
            entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Dto).WithMany(p => p.Properties)
                .HasForeignKey(d => d.DtoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Property_Dto");
        });

        modelBuilder.Entity<SystemMenu>(entity =>
        {
            entity.ToTable("SystemMenu", "infra");

            entity.Property(e => e.Caption).HasMaxLength(1024);
            entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<Translation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Translation", "infra");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.LangCode)
                .HasMaxLength(10)
                .IsFixedLength();
        });

        modelBuilder.Entity<UiBootstrapPosition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_BootstrapPosition");

            entity.ToTable("UiBootstrapPosition", "infra");
        });

        modelBuilder.Entity<UiComponent>(entity =>
        {
            entity.ToTable("UiComponent", "infra");

            entity.HasIndex(e => e.PageDataContextId, "IX_UiComponent_PageDataContextId");

            entity.HasIndex(e => e.PageDataContextPropertyId, "IX_UiComponent_PageDataContextPropertyId");

            entity.Property(e => e.ClassName).HasMaxLength(50);
            entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.PageDataContext).WithMany(p => p.UiComponents)
                .HasForeignKey(d => d.PageDataContextId)
                .HasConstraintName("FK_UiComponent_Dto1");

            entity.HasOne(d => d.PageDataContextProperty).WithMany(p => p.UiComponents)
                .HasForeignKey(d => d.PageDataContextPropertyId)
                .HasConstraintName("FK_UiComponent_Property");
        });

        modelBuilder.Entity<UiComponentAction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UiComponentAction_1");

            entity.ToTable("UiComponentAction", "infra");

            entity.HasIndex(e => e.CqrsSegregateId, "IX_UiComponentAction_CqrsSegregateId");

            entity.HasIndex(e => e.PositionId, "IX_UiComponentAction_PositionId");

            entity.HasIndex(e => e.UiComponentId, "IX_UiComponentAction_UiComponentId");

            entity.Property(e => e.Id).HasComment("مکان در کامپوننت");
            entity.Property(e => e.Caption).HasMaxLength(1024);
            entity.Property(e => e.EventHandlerName).HasMaxLength(1024);
            entity.Property(e => e.Name).HasMaxLength(1024);

            entity.HasOne(d => d.CqrsSegregate).WithMany(p => p.UiComponentActions)
                .HasForeignKey(d => d.CqrsSegregateId)
                .HasConstraintName("FK_UiComponentAction_CqrsSegregate");

            entity.HasOne(d => d.Position).WithMany(p => p.UiComponentActions)
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("FK_UiComponentAction_UiBootstrapPosition");

            entity.HasOne(d => d.UiComponent).WithMany(p => p.UiComponentActions)
                .HasForeignKey(d => d.UiComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UiComponentAction_UiComponent");
        });

        modelBuilder.Entity<UiComponentProperty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UiComponentProperty_1");

            entity.ToTable("UiComponentProperty", "infra");

            entity.HasIndex(e => e.PositionId, "IX_UiComponentProperty_PositionId");

            entity.HasIndex(e => e.PropertyId, "IX_UiComponentProperty_PropertyId");

            entity.HasIndex(e => e.UiComponentId, "IX_UiComponentProperty_UiComponentId");

            entity.Property(e => e.Caption).HasMaxLength(50);
            entity.Property(e => e.PositionId).HasComment("مکان در کامپوننت");

            entity.HasOne(d => d.Position).WithMany(p => p.UiComponentProperties)
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("FK_UiComponentProperty_UiBootstrapPosition");

            entity.HasOne(d => d.Property).WithMany(p => p.UiComponentProperties)
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UiComponentProperty_Property1");

            entity.HasOne(d => d.UiComponent).WithMany(p => p.UiComponentProperties)
                .HasForeignKey(d => d.UiComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UiComponentProperty_UiComponent");
        });

        modelBuilder.Entity<UiPage>(entity =>
        {
            entity.ToTable("UiPage", "infra");

            entity.HasIndex(e => e.DtoId, "IX_UiPage_DtoId");

            entity.HasIndex(e => e.ModuleId, "IX_UiPage_ModuleId");

            entity.Property(e => e.ClassName).HasMaxLength(50);
            entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Dto).WithMany(p => p.UiPages)
                .HasForeignKey(d => d.DtoId)
                .HasConstraintName("FK_UiPage_Dto");

            entity.HasOne(d => d.Module).WithMany(p => p.UiPages)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UiPage_Module");
        });

        modelBuilder.Entity<UiPageComponent>(entity =>
        {
            entity.ToTable("UiPageComponent", "infra");

            entity.HasIndex(e => e.PageId, "IX_UiPageComponent_PageId");

            entity.HasIndex(e => e.PositionId, "IX_UiPageComponent_PositionId");

            entity.HasIndex(e => e.UiComponentId, "IX_UiPageComponent_UiComponentId");

            entity.Property(e => e.Guid).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.PositionId).HasComment("مکان در پیج");

            entity.HasOne(d => d.Page).WithMany(p => p.UiPageComponents)
                .HasForeignKey(d => d.PageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UiPageComponent_UiPage");

            entity.HasOne(d => d.Position).WithMany(p => p.UiPageComponents)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UiPageComponent_UiBootstrapPosition");

            entity.HasOne(d => d.UiComponent).WithMany(p => p.UiPageComponents)
                .HasForeignKey(d => d.UiComponentId)
                .HasConstraintName("FK_UiPageComponent_UiComponent");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
