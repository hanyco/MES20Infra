USE [master]
GO
/****** Object:  Database [MesInfra]    Script Date: 11/29/2023 1:37:37 AM ******/
CREATE DATABASE [MesInfra]
 CONTAINMENT = NONE
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [MesInfra] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MesInfra].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MesInfra] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MesInfra] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MesInfra] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MesInfra] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MesInfra] SET ARITHABORT OFF 
GO
ALTER DATABASE [MesInfra] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [MesInfra] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MesInfra] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MesInfra] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MesInfra] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MesInfra] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MesInfra] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MesInfra] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MesInfra] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MesInfra] SET  DISABLE_BROKER 
GO
ALTER DATABASE [MesInfra] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MesInfra] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MesInfra] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [MesInfra] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [MesInfra] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MesInfra] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [MesInfra] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MesInfra] SET RECOVERY FULL 
GO
ALTER DATABASE [MesInfra] SET  MULTI_USER 
GO
ALTER DATABASE [MesInfra] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [MesInfra] SET DB_CHAINING OFF 
GO
ALTER DATABASE [MesInfra] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [MesInfra] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [MesInfra] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [MesInfra] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [MesInfra] SET QUERY_STORE = OFF
GO
USE [MesInfra]
GO
/****** Object:  Schema [infra]    Script Date: 11/29/2023 1:37:38 AM ******/
CREATE SCHEMA [infra]
GO
/****** Object:  Schema [sec]    Script Date: 11/29/2023 1:37:38 AM ******/
CREATE SCHEMA [sec]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Person]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Person](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[DateOfBirth] [date] NOT NULL,
	[Height] [int] NULL,
 CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[CqrsSegregate]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[CqrsSegregate](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[CqrsNameSpace] [nvarchar](max) NULL,
	[SegregateType] [int] NOT NULL,
	[FriendlyName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[ParamDtoId] [bigint] NOT NULL,
	[ResultDtoId] [bigint] NOT NULL,
	[Guid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[ModuleId] [bigint] NOT NULL,
	[CategoryId] [int] NOT NULL,
 CONSTRAINT [PK_CqrsSegregate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[CrudCode]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[CrudCode](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[DbObjectId] [nvarchar](50) NULL,
	[ModuleId] [bigint] NULL,
	[GetCqrsSegregateId] [bigint] NULL,
	[GetByIdCqrsSegregateId] [bigint] NULL,
	[CreateCqrsSegregateId] [bigint] NULL,
	[UpdateCqrsSegregateId] [bigint] NULL,
	[DeleteCqrsSegregateId] [bigint] NULL,
	[CqrsCodeNameSpace] [nvarchar](max) NOT NULL,
	[DtoCodeNameSpace] [nvarchar](max) NOT NULL,
	[Guid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
 CONSTRAINT [PK_CrudCode] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[Dto]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[Dto](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[NameSpace] [nvarchar](1024) NULL,
	[ModuleId] [bigint] NULL,
	[DbObjectId] [nvarchar](50) NULL,
	[Guid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Comment] [nvarchar](50) NULL,
	[IsParamsDto] [bit] NOT NULL,
	[IsResultDto] [bit] NOT NULL,
	[IsViewModel] [bit] NOT NULL,
	[IsList] [bit] NULL,
 CONSTRAINT [PK_Dto] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[EntityClaim]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[EntityClaim](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
	[ClaimId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_EntityClaim] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[Functionality]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[Functionality](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[ModuleId] [bigint] NULL,
	[Guid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Comment] [nvarchar](50) NULL,
	[GetAllQueryId] [bigint] NOT NULL,
	[GetByIdQueryId] [bigint] NOT NULL,
	[InsertCommandId] [bigint] NOT NULL,
	[UpdateCommandId] [bigint] NOT NULL,
	[DeleteCommandId] [bigint] NOT NULL,
 CONSTRAINT [PK_Functionality] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[Module]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[Module](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Guid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ParentId] [bigint] NULL,
 CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[Property]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[Property](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ParentEntityId] [bigint] NOT NULL,
	[PropertyType] [int] NOT NULL,
	[TypeFullName] [nvarchar](max) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[HasSetter] [bit] NULL,
	[HasGetter] [bit] NULL,
	[IsList] [bit] NULL,
	[IsNullable] [bit] NULL,
	[Comment] [nvarchar](max) NULL,
	[DbObjectId] [nvarchar](50) NULL,
	[Guid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[DtoId] [bigint] NULL,
 CONSTRAINT [PK_Property] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[SecurityClaim]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[SecurityClaim](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Key] [nvarchar](50) NOT NULL,
	[Parent] [uniqueidentifier] NULL,
 CONSTRAINT [PK_SecurityClaim_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[SystemMenu]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[SystemMenu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ParentId] [bigint] NULL,
	[Guid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Caption] [nvarchar](1024) NOT NULL,
	[Uri] [nvarchar](max) NULL,
 CONSTRAINT [PK_SystemMenu] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[Translation]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[Translation](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](max) NOT NULL,
	[LangCode] [nchar](10) NOT NULL,
	[Value] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[UiBootstrapPosition]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[UiBootstrapPosition](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Order] [int] NULL,
	[Row] [int] NULL,
	[Col] [int] NULL,
	[ColSpan] [int] NULL,
 CONSTRAINT [PK_BootstrapPosition] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[UiComponent]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[UiComponent](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[IsEnabled] [bit] NULL,
	[Caption] [nvarchar](max) NULL,
	[ClassName] [nvarchar](50) NOT NULL,
	[NameSpace] [nvarchar](max) NOT NULL,
	[PageDataContextId] [bigint] NULL,
	[PageDataContextPropertyId] [bigint] NULL,
	[IsGrid] [bit] NULL,
 CONSTRAINT [PK_UiComponent] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[UiComponentAction]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[UiComponentAction](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UiComponentId] [bigint] NOT NULL,
	[CqrsSegregateId] [bigint] NULL,
	[TriggerTypeId] [int] NOT NULL,
	[EventHandlerName] [nvarchar](1024) NULL,
	[PositionId] [bigint] NOT NULL,
	[Caption] [nvarchar](1024) NULL,
	[IsEnabled] [bit] NULL,
	[Name] [nvarchar](1024) NULL,
 CONSTRAINT [PK_UiComponentAction_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[UiComponentProperty]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[UiComponentProperty](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UiComponentId] [bigint] NOT NULL,
	[CqrsSegregateId] [bigint] NULL,
	[PropertyId] [bigint] NULL,
	[ControlTypeId] [int] NOT NULL,
	[PositionId] [bigint] NOT NULL,
	[Caption] [nvarchar](50) NOT NULL,
	[IsEnabled] [bit] NULL,
	[ControlExtraInfo] [nvarchar](max) NULL,
 CONSTRAINT [PK_UiComponentProperty_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[UiPage]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[UiPage](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ClassName] [nvarchar](50) NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[NameSpace] [nvarchar](max) NOT NULL,
	[ModuleId] [bigint] NOT NULL,
	[Route] [nvarchar](max) NULL,
	[DtoId] [bigint] NULL,
 CONSTRAINT [PK_UiPage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[UiPageComponent]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[UiPageComponent](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[PageId] [bigint] NOT NULL,
	[UiComponentId] [bigint] NOT NULL,
	[PositionId] [bigint] NULL,
 CONSTRAINT [PK_UiPageComponent] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[UserClaimAccess]    Script Date: 11/29/2023 1:37:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[UserClaimAccess](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[ClaimId] [uniqueidentifier] NOT NULL,
	[AccessType] [int] NOT NULL,
 CONSTRAINT [PK_UserClaimAccess] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20201206193111_init-database', N'5.0.0')
GO
SET IDENTITY_INSERT [infra].[CqrsSegregate] ON 

INSERT [infra].[CqrsSegregate] ([Id], [Name], [CqrsNameSpace], [SegregateType], [FriendlyName], [Description], [ParamDtoId], [ResultDtoId], [Guid], [Comment], [ModuleId], [CategoryId]) VALUES (10102, N'GetAllPersons', N'Test.Hr.Queries', 0, NULL, NULL, 20400, 20401, N'386b565b-3fb0-4ae0-8d02-5e0fabb03e94', NULL, 5, 0)
SET IDENTITY_INSERT [infra].[CqrsSegregate] OFF
GO
SET IDENTITY_INSERT [infra].[Dto] ON 

INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [IsList]) VALUES (20400, N'GetAllPersonParams', N'Test.Hr.Dtos', 5, NULL, N'14b5f49b-61b7-44a4-a59d-7d4ffb817f87', NULL, 1, 0, 0, 0)
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [IsList]) VALUES (20401, N'GetAllPersonResult', N'Test.Hr.Dtos', 5, NULL, N'a316189d-10c1-4e88-8539-dcc414b0cffb', NULL, 0, 1, 0, 0)
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [IsList]) VALUES (20403, N'PersonViewModel', N'Test.Hr.ViewModels', 5, NULL, N'cab6f47b-15c7-41f8-9190-c9821d354d98', NULL, 0, 0, 1, 0)
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [IsList]) VALUES (20404, N'PersonDto', N'Test.Hr', 5, NULL, N'4355bfd5-3e08-443a-b24f-df344fdc8d04', NULL, 1, 0, 0, 0)
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [IsList]) VALUES (20405, N'InsertPersonParamsDto', N'Test.Hr', 5, NULL, N'8012367e-9d3e-4ff0-aef7-d56666286c72', NULL, 0, 0, 0, 0)
SET IDENTITY_INSERT [infra].[Dto] OFF
GO
SET IDENTITY_INSERT [infra].[Module] ON 

INSERT [infra].[Module] ([Id], [Name], [Guid], [ParentId]) VALUES (1, N'Financial Management', N'249f14de-7938-eb11-a3b1-0015830cbfeb', NULL)
INSERT [infra].[Module] ([Id], [Name], [Guid], [ParentId]) VALUES (2, N'Inventory Management', N'259f14de-7938-eb11-a3b1-0015830cbfeb', NULL)
INSERT [infra].[Module] ([Id], [Name], [Guid], [ParentId]) VALUES (3, N'Supply Chain Management', N'384571e7-7938-eb11-a3b1-0015830cbfeb', NULL)
INSERT [infra].[Module] ([Id], [Name], [Guid], [ParentId]) VALUES (4, N'Customer Relationship Management (CRM)', N'394571e7-7938-eb11-a3b1-0015830cbfeb', NULL)
INSERT [infra].[Module] ([Id], [Name], [Guid], [ParentId]) VALUES (5, N'Human Resources', N'3c508aef-7938-eb11-a3b1-0015830cbfeb', NULL)
INSERT [infra].[Module] ([Id], [Name], [Guid], [ParentId]) VALUES (6, N'System', N'cb7cb73f-753a-ec11-a58f-f832e4b3c927', NULL)
SET IDENTITY_INSERT [infra].[Module] OFF
GO
SET IDENTITY_INSERT [infra].[Property] ON 

INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (21011, 20401, 3, N'System.Int64', N'Id', NULL, NULL, NULL, 0, NULL, N'295672101001', N'1b81d937-f069-4cd9-a64b-2a1dcb4e1e8a', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (21012, 20401, 1, N'System.String', N'FirstName', NULL, NULL, NULL, 0, NULL, N'295672101002', N'27aac80f-bfd9-4a8e-bc9f-8344aa4e1a0b', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (21013, 20401, 1, N'System.String', N'LastName', NULL, NULL, NULL, 0, NULL, N'295672101003', N'b9c32ecd-f091-4367-9b38-76eb8e4f4357', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (21014, 20401, 8, N'System.DateTime', N'DateOfBirth', NULL, NULL, NULL, 0, NULL, N'295672101004', N'e66b7748-2412-46e8-b2fd-9033fef2ae30', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (21015, 20401, 2, N'System.Int32', N'Height', NULL, NULL, NULL, 0, NULL, N'295672101005', N'f11b566c-1e2d-4cca-aaf7-4140370bfa4d', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (21021, 20403, 20, N'Test.Hr.Dtos.GetAllPersonResult', N'PersonsDto', NULL, NULL, 1, NULL, NULL, N'', N'46a1c31e-ee4e-41f9-ab18-ce810c939e5d', 20401)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (21022, 20403, 20, N'Test.Hr.Dtos.GetAllPersonResult', N'SelectedPerson', NULL, NULL, NULL, 1, NULL, N'', N'9366821b-db11-4904-98d9-3fc4baa5189b', 20401)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (21023, 20404, 3, N'System.Int64', N'Id', NULL, NULL, NULL, 0, NULL, N'295672101001', N'ff8ed756-90c0-4365-b9eb-6635f2fc9205', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (21024, 20404, 1, N'System.String', N'FirstName', NULL, NULL, NULL, 0, NULL, N'295672101002', N'db5cb82d-78db-4a68-a44f-39e64a72ef26', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (21025, 20404, 1, N'System.String', N'LastName', NULL, NULL, NULL, 0, NULL, N'295672101003', N'58162561-2208-465a-b009-cd27daa3d2fd', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (21026, 20404, 8, N'System.DateTime', N'DateOfBirth', NULL, NULL, NULL, 0, NULL, N'295672101004', N'fd5724e8-5152-4cf8-8a85-11a32f0546ae', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (21027, 20404, 2, N'System.Int32', N'Height', NULL, NULL, NULL, 0, NULL, N'295672101005', N'b132b7d2-b562-4e62-b10f-300a1b745608', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (21028, 20405, 3, N'System.Int64', N'Id', NULL, NULL, NULL, 0, NULL, N'295672101001', N'7d11e649-80a2-4b1f-bff3-c463db9402d5', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (21029, 20405, 1, N'System.String', N'FirstName', NULL, NULL, NULL, 0, NULL, N'295672101002', N'4211ce36-649d-45b9-9361-c04a7094f16c', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (21030, 20405, 1, N'System.String', N'LastName', NULL, NULL, NULL, 0, NULL, N'295672101003', N'714a6c43-fe73-4de0-bcb8-fd041e2a9b7c', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (21031, 20405, 8, N'System.DateTime', N'DateOfBirth', NULL, NULL, NULL, 0, NULL, N'295672101004', N'a8263abb-5764-432a-afd1-6c9cc307d677', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (21032, 20405, 2, N'System.Int32', N'Height', NULL, NULL, NULL, 0, NULL, N'295672101005', N'e5c1af6c-0e08-40e2-a2bd-b3e348e4773f', NULL)
SET IDENTITY_INSERT [infra].[Property] OFF
GO
SET IDENTITY_INSERT [infra].[UiBootstrapPosition] ON 

INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2554, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2556, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2557, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2558, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2559, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2560, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2561, 0, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2562, 1, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2563, 2, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2564, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2565, NULL, 1, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2566, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2567, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2568, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2569, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2570, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2571, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2572, NULL, 1, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2573, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2574, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2575, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2576, NULL, 1, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2577, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2578, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2579, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2580, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2581, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2582, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2583, NULL, 1, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2584, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2585, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2586, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2587, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2588, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2589, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2590, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2591, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2592, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2593, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2594, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2595, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2596, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2597, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2598, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2599, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2600, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2601, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2602, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2603, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2604, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2605, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2606, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2607, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2608, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2609, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2610, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2611, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2612, NULL, 2, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2613, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2614, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2615, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2616, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2617, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2618, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2619, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2620, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2621, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2622, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2623, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2624, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2625, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2626, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2627, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2628, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2629, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2630, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2631, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2632, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2633, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2634, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2635, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2636, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2637, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2638, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2639, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2640, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2641, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2642, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2643, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2644, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2645, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2652, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2653, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2654, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2655, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2656, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2667, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2668, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2669, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2670, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2671, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2672, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2673, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2696, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2697, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2698, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2699, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2700, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2701, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2702, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2703, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2704, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2705, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2706, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2707, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2708, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2709, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2710, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2711, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2712, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2713, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2714, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2715, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2716, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2717, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2718, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2719, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2720, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2722, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2723, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2724, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2725, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2726, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2727, NULL, 2, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2728, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2729, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2730, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2731, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2732, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2733, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2734, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2735, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2736, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2737, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2738, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2739, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2740, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2741, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2742, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2743, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2744, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2745, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2746, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2747, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2748, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2749, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2750, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2751, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2752, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2753, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2754, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2755, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2756, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2757, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2758, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2759, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2760, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2761, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2762, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2763, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2764, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2765, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2766, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2767, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2768, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2769, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2770, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2771, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2772, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2773, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2774, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2775, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2776, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2777, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2778, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2779, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2780, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2781, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2782, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2783, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2784, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2785, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2786, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2787, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2788, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2789, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2790, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2791, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2792, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2798, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2799, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2800, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2801, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2802, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2803, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2804, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2805, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2806, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2807, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2808, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2809, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2810, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2811, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2812, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2813, NULL, NULL, NULL, NULL)
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2814, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [infra].[UiBootstrapPosition] OFF
GO
SET IDENTITY_INSERT [infra].[UiComponent] ON 

INSERT [infra].[UiComponent] ([Id], [Name], [Guid], [IsEnabled], [Caption], [ClassName], [NameSpace], [PageDataContextId], [PageDataContextPropertyId], [IsGrid]) VALUES (8, N'PersonListComponent', N'3ca742fd-291c-46ca-bef2-383b7eaf808f', NULL, NULL, N'PersonComponent', N'Test.Hr.Components', 20403, 21021, 1)
INSERT [infra].[UiComponent] ([Id], [Name], [Guid], [IsEnabled], [Caption], [ClassName], [NameSpace], [PageDataContextId], [PageDataContextPropertyId], [IsGrid]) VALUES (9, N'PersonDetailsComponent', N'ee743973-da2d-4757-8d9d-04ce74e86538', NULL, NULL, N'PersonComponent', N'Test.Hr.Components', 20403, 21022, 0)
SET IDENTITY_INSERT [infra].[UiComponent] OFF
GO
SET IDENTITY_INSERT [infra].[UiComponentProperty] ON 

INSERT [infra].[UiComponentProperty] ([Id], [UiComponentId], [CqrsSegregateId], [PropertyId], [ControlTypeId], [PositionId], [Caption], [IsEnabled], [ControlExtraInfo]) VALUES (573, 8, NULL, 21011, 60, 2805, N'Id', 1, NULL)
INSERT [infra].[UiComponentProperty] ([Id], [UiComponentId], [CqrsSegregateId], [PropertyId], [ControlTypeId], [PositionId], [Caption], [IsEnabled], [ControlExtraInfo]) VALUES (574, 8, NULL, 21012, 30, 2806, N'First Name', 1, NULL)
INSERT [infra].[UiComponentProperty] ([Id], [UiComponentId], [CqrsSegregateId], [PropertyId], [ControlTypeId], [PositionId], [Caption], [IsEnabled], [ControlExtraInfo]) VALUES (575, 8, NULL, 21013, 30, 2807, N'Last Name', 1, NULL)
INSERT [infra].[UiComponentProperty] ([Id], [UiComponentId], [CqrsSegregateId], [PropertyId], [ControlTypeId], [PositionId], [Caption], [IsEnabled], [ControlExtraInfo]) VALUES (576, 8, NULL, 21014, 50, 2808, N'Date Of Birth', 1, NULL)
INSERT [infra].[UiComponentProperty] ([Id], [UiComponentId], [CqrsSegregateId], [PropertyId], [ControlTypeId], [PositionId], [Caption], [IsEnabled], [ControlExtraInfo]) VALUES (577, 8, NULL, 21015, 60, 2809, N'Height', 1, NULL)
INSERT [infra].[UiComponentProperty] ([Id], [UiComponentId], [CqrsSegregateId], [PropertyId], [ControlTypeId], [PositionId], [Caption], [IsEnabled], [ControlExtraInfo]) VALUES (578, 9, NULL, 21011, 60, 2810, N'Id', 1, NULL)
INSERT [infra].[UiComponentProperty] ([Id], [UiComponentId], [CqrsSegregateId], [PropertyId], [ControlTypeId], [PositionId], [Caption], [IsEnabled], [ControlExtraInfo]) VALUES (579, 9, NULL, 21012, 30, 2811, N'First Name', 1, NULL)
INSERT [infra].[UiComponentProperty] ([Id], [UiComponentId], [CqrsSegregateId], [PropertyId], [ControlTypeId], [PositionId], [Caption], [IsEnabled], [ControlExtraInfo]) VALUES (580, 9, NULL, 21013, 30, 2812, N'Last Name', 1, NULL)
INSERT [infra].[UiComponentProperty] ([Id], [UiComponentId], [CqrsSegregateId], [PropertyId], [ControlTypeId], [PositionId], [Caption], [IsEnabled], [ControlExtraInfo]) VALUES (581, 9, NULL, 21014, 50, 2813, N'Date Of Birth', 1, NULL)
INSERT [infra].[UiComponentProperty] ([Id], [UiComponentId], [CqrsSegregateId], [PropertyId], [ControlTypeId], [PositionId], [Caption], [IsEnabled], [ControlExtraInfo]) VALUES (582, 9, NULL, 21015, 60, 2814, N'Height', 1, NULL)
SET IDENTITY_INSERT [infra].[UiComponentProperty] OFF
GO
SET IDENTITY_INSERT [infra].[UiPage] ON 

INSERT [infra].[UiPage] ([Id], [Name], [ClassName], [Guid], [NameSpace], [ModuleId], [Route], [DtoId]) VALUES (21, N'PersonPage', N'PersonPage', N'e6779d84-f9d8-4baa-928b-d3c250b27fc0', N'Test.Hr.Pages', 5, N'/person', NULL)
SET IDENTITY_INSERT [infra].[UiPage] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 11/29/2023 1:37:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 11/29/2023 1:37:38 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 11/29/2023 1:37:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 11/29/2023 1:37:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 11/29/2023 1:37:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [EmailIndex]    Script Date: 11/29/2023 1:37:38 AM ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 11/29/2023 1:37:38 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CqrsSegregate_ResultDtoId]    Script Date: 11/29/2023 1:37:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_CqrsSegregate_ResultDtoId] ON [infra].[CqrsSegregate]
(
	[ResultDtoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Dto_ModuleId]    Script Date: 11/29/2023 1:37:38 AM ******/
CREATE NONCLUSTERED INDEX [IX_Dto_ModuleId] ON [infra].[Dto]
(
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [infra].[CqrsSegregate] ADD  CONSTRAINT [DF_CqrsSegregate_Guid]  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[CrudCode] ADD  CONSTRAINT [DF_CrudCode_Guid]  DEFAULT (newsequentialid()) FOR [Guid]
GO
ALTER TABLE [infra].[Dto] ADD  CONSTRAINT [DF_Dto_Guid]  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[EntityClaim] ADD  CONSTRAINT [DF_EntityClaim_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [infra].[Functionality] ADD  CONSTRAINT [DF_Functionality_Guid]  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[Module] ADD  CONSTRAINT [DF_Module_Guid]  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[Property] ADD  CONSTRAINT [DF_Property_Guid]  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[SecurityClaim] ADD  CONSTRAINT [DF_SecurityClaim_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [infra].[SystemMenu] ADD  CONSTRAINT [DF_SystemMenu_Guid]  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[UiComponent] ADD  CONSTRAINT [DF_UiComponent_Guid]  DEFAULT (newsequentialid()) FOR [Guid]
GO
ALTER TABLE [infra].[UiPage] ADD  CONSTRAINT [DF_UiPage_Guid]  DEFAULT (newsequentialid()) FOR [Guid]
GO
ALTER TABLE [infra].[UiPageComponent] ADD  CONSTRAINT [DF_UiPageComponent_Guid]  DEFAULT (newsequentialid()) FOR [Guid]
GO
ALTER TABLE [infra].[UserClaimAccess] ADD  CONSTRAINT [DF_UserClaimAccess_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [infra].[CqrsSegregate]  WITH CHECK ADD  CONSTRAINT [FK_CqrsSegregate_Dto] FOREIGN KEY([ParamDtoId])
REFERENCES [infra].[Dto] ([Id])
GO
ALTER TABLE [infra].[CqrsSegregate] CHECK CONSTRAINT [FK_CqrsSegregate_Dto]
GO
ALTER TABLE [infra].[CqrsSegregate]  WITH CHECK ADD  CONSTRAINT [FK_CqrsSegregate_Dto_ResultDtoId] FOREIGN KEY([ResultDtoId])
REFERENCES [infra].[Dto] ([Id])
GO
ALTER TABLE [infra].[CqrsSegregate] CHECK CONSTRAINT [FK_CqrsSegregate_Dto_ResultDtoId]
GO
ALTER TABLE [infra].[CqrsSegregate]  WITH CHECK ADD  CONSTRAINT [FK_CqrsSegregate_Module] FOREIGN KEY([ModuleId])
REFERENCES [infra].[Module] ([Id])
GO
ALTER TABLE [infra].[CqrsSegregate] CHECK CONSTRAINT [FK_CqrsSegregate_Module]
GO
ALTER TABLE [infra].[CrudCode]  WITH CHECK ADD  CONSTRAINT [FK_CrudCode_Module] FOREIGN KEY([ModuleId])
REFERENCES [infra].[Module] ([Id])
GO
ALTER TABLE [infra].[CrudCode] CHECK CONSTRAINT [FK_CrudCode_Module]
GO
ALTER TABLE [infra].[Dto]  WITH CHECK ADD  CONSTRAINT [FK_Dto_Module_ModuleId] FOREIGN KEY([ModuleId])
REFERENCES [infra].[Module] ([Id])
GO
ALTER TABLE [infra].[Dto] CHECK CONSTRAINT [FK_Dto_Module_ModuleId]
GO
ALTER TABLE [infra].[EntityClaim]  WITH CHECK ADD  CONSTRAINT [FK_EntityClaim_SecurityClaim] FOREIGN KEY([ClaimId])
REFERENCES [infra].[SecurityClaim] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [infra].[EntityClaim] CHECK CONSTRAINT [FK_EntityClaim_SecurityClaim]
GO
ALTER TABLE [infra].[Functionality]  WITH CHECK ADD  CONSTRAINT [FK_Functionality_CqrsSegregate] FOREIGN KEY([GetAllQueryId])
REFERENCES [infra].[CqrsSegregate] ([Id])
GO
ALTER TABLE [infra].[Functionality] CHECK CONSTRAINT [FK_Functionality_CqrsSegregate]
GO
ALTER TABLE [infra].[Functionality]  WITH CHECK ADD  CONSTRAINT [FK_Functionality_CqrsSegregate1] FOREIGN KEY([GetByIdQueryId])
REFERENCES [infra].[CqrsSegregate] ([Id])
GO
ALTER TABLE [infra].[Functionality] CHECK CONSTRAINT [FK_Functionality_CqrsSegregate1]
GO
ALTER TABLE [infra].[Functionality]  WITH CHECK ADD  CONSTRAINT [FK_Functionality_CqrsSegregate2] FOREIGN KEY([InsertCommandId])
REFERENCES [infra].[CqrsSegregate] ([Id])
GO
ALTER TABLE [infra].[Functionality] CHECK CONSTRAINT [FK_Functionality_CqrsSegregate2]
GO
ALTER TABLE [infra].[Functionality]  WITH CHECK ADD  CONSTRAINT [FK_Functionality_CqrsSegregate3] FOREIGN KEY([UpdateCommandId])
REFERENCES [infra].[CqrsSegregate] ([Id])
GO
ALTER TABLE [infra].[Functionality] CHECK CONSTRAINT [FK_Functionality_CqrsSegregate3]
GO
ALTER TABLE [infra].[Functionality]  WITH CHECK ADD  CONSTRAINT [FK_Functionality_CqrsSegregate4] FOREIGN KEY([DeleteCommandId])
REFERENCES [infra].[CqrsSegregate] ([Id])
GO
ALTER TABLE [infra].[Functionality] CHECK CONSTRAINT [FK_Functionality_CqrsSegregate4]
GO
ALTER TABLE [infra].[Property]  WITH CHECK ADD  CONSTRAINT [FK_Property_Dto] FOREIGN KEY([DtoId])
REFERENCES [infra].[Dto] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [infra].[Property] CHECK CONSTRAINT [FK_Property_Dto]
GO
ALTER TABLE [infra].[UiComponent]  WITH CHECK ADD  CONSTRAINT [FK_UiComponent_Dto1] FOREIGN KEY([PageDataContextId])
REFERENCES [infra].[Dto] ([Id])
GO
ALTER TABLE [infra].[UiComponent] CHECK CONSTRAINT [FK_UiComponent_Dto1]
GO
ALTER TABLE [infra].[UiComponent]  WITH CHECK ADD  CONSTRAINT [FK_UiComponent_Property] FOREIGN KEY([PageDataContextPropertyId])
REFERENCES [infra].[Property] ([Id])
GO
ALTER TABLE [infra].[UiComponent] CHECK CONSTRAINT [FK_UiComponent_Property]
GO
ALTER TABLE [infra].[UiComponentAction]  WITH CHECK ADD  CONSTRAINT [FK_UiComponentAction_CqrsSegregate] FOREIGN KEY([CqrsSegregateId])
REFERENCES [infra].[CqrsSegregate] ([Id])
GO
ALTER TABLE [infra].[UiComponentAction] CHECK CONSTRAINT [FK_UiComponentAction_CqrsSegregate]
GO
ALTER TABLE [infra].[UiComponentAction]  WITH CHECK ADD  CONSTRAINT [FK_UiComponentAction_UiBootstrapPosition] FOREIGN KEY([PositionId])
REFERENCES [infra].[UiBootstrapPosition] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [infra].[UiComponentAction] CHECK CONSTRAINT [FK_UiComponentAction_UiBootstrapPosition]
GO
ALTER TABLE [infra].[UiComponentAction]  WITH CHECK ADD  CONSTRAINT [FK_UiComponentAction_UiComponent] FOREIGN KEY([UiComponentId])
REFERENCES [infra].[UiComponent] ([Id])
GO
ALTER TABLE [infra].[UiComponentAction] CHECK CONSTRAINT [FK_UiComponentAction_UiComponent]
GO
ALTER TABLE [infra].[UiComponentProperty]  WITH CHECK ADD  CONSTRAINT [FK_UiComponentProperty_Property1] FOREIGN KEY([PropertyId])
REFERENCES [infra].[Property] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [infra].[UiComponentProperty] CHECK CONSTRAINT [FK_UiComponentProperty_Property1]
GO
ALTER TABLE [infra].[UiComponentProperty]  WITH CHECK ADD  CONSTRAINT [FK_UiComponentProperty_UiBootstrapPosition] FOREIGN KEY([PositionId])
REFERENCES [infra].[UiBootstrapPosition] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [infra].[UiComponentProperty] CHECK CONSTRAINT [FK_UiComponentProperty_UiBootstrapPosition]
GO
ALTER TABLE [infra].[UiComponentProperty]  WITH CHECK ADD  CONSTRAINT [FK_UiComponentProperty_UiComponent] FOREIGN KEY([UiComponentId])
REFERENCES [infra].[UiComponent] ([Id])
GO
ALTER TABLE [infra].[UiComponentProperty] CHECK CONSTRAINT [FK_UiComponentProperty_UiComponent]
GO
ALTER TABLE [infra].[UiPage]  WITH CHECK ADD  CONSTRAINT [FK_UiPage_Dto] FOREIGN KEY([DtoId])
REFERENCES [infra].[Dto] ([Id])
GO
ALTER TABLE [infra].[UiPage] CHECK CONSTRAINT [FK_UiPage_Dto]
GO
ALTER TABLE [infra].[UiPage]  WITH CHECK ADD  CONSTRAINT [FK_UiPage_Module] FOREIGN KEY([ModuleId])
REFERENCES [infra].[Module] ([Id])
GO
ALTER TABLE [infra].[UiPage] CHECK CONSTRAINT [FK_UiPage_Module]
GO
ALTER TABLE [infra].[UiPageComponent]  WITH CHECK ADD  CONSTRAINT [FK_UiPageComponent_UiBootstrapPosition] FOREIGN KEY([PositionId])
REFERENCES [infra].[UiBootstrapPosition] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [infra].[UiPageComponent] CHECK CONSTRAINT [FK_UiPageComponent_UiBootstrapPosition]
GO
ALTER TABLE [infra].[UiPageComponent]  WITH CHECK ADD  CONSTRAINT [FK_UiPageComponent_UiComponent] FOREIGN KEY([UiComponentId])
REFERENCES [infra].[UiComponent] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [infra].[UiPageComponent] CHECK CONSTRAINT [FK_UiPageComponent_UiComponent]
GO
ALTER TABLE [infra].[UiPageComponent]  WITH CHECK ADD  CONSTRAINT [FK_UiPageComponent_UiPage] FOREIGN KEY([PageId])
REFERENCES [infra].[UiPage] ([Id])
GO
ALTER TABLE [infra].[UiPageComponent] CHECK CONSTRAINT [FK_UiPageComponent_UiPage]
GO
ALTER TABLE [infra].[UserClaimAccess]  WITH CHECK ADD  CONSTRAINT [FK_UserClaimAccess_SecurityClaim] FOREIGN KEY([ClaimId])
REFERENCES [infra].[SecurityClaim] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [infra].[UserClaimAccess] CHECK CONSTRAINT [FK_UserClaimAccess_SecurityClaim]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'مکان در کامپوننت' , @level0type=N'SCHEMA',@level0name=N'infra', @level1type=N'TABLE',@level1name=N'UiComponentAction', @level2type=N'COLUMN',@level2name=N'Id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'مکان در کامپوننت' , @level0type=N'SCHEMA',@level0name=N'infra', @level1type=N'TABLE',@level1name=N'UiComponentProperty', @level2type=N'COLUMN',@level2name=N'PositionId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'مکان در پیج' , @level0type=N'SCHEMA',@level0name=N'infra', @level1type=N'TABLE',@level1name=N'UiPageComponent', @level2type=N'COLUMN',@level2name=N'PositionId'
GO
USE [master]
GO
ALTER DATABASE [MesInfra] SET  READ_WRITE 
GO
