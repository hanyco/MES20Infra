USE [master]
GO
/****** Object:  Database [MesInfra]    Script Date: 9/2/2023 7:57:13 PM ******/
CREATE DATABASE [MesInfra]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'MesInfra', FILENAME = N'D:\Dev\Sql\Data\MesInfra.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'MesInfra_log', FILENAME = N'D:\Dev\Sql\Data\MesInfra_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
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
/****** Object:  Schema [infra]    Script Date: 9/2/2023 7:57:13 PM ******/
CREATE SCHEMA [infra]
GO
/****** Object:  Schema [sec]    Script Date: 9/2/2023 7:57:13 PM ******/
CREATE SCHEMA [sec]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [dbo].[Person]    Script Date: 9/2/2023 7:57:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Person](
	[Id] [bigint] NOT NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[DateOfBirth] [date] NOT NULL,
	[Height] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[CqrsSegregate]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [infra].[CrudCode]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [infra].[Dto]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [infra].[EntitySecurity]    Script Date: 9/2/2023 7:57:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[EntitySecurity](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
	[SecurityDescriptorId] [uniqueidentifier] NOT NULL,
	[IsEnabled] [bit] NULL,
 CONSTRAINT [PK_EntitySecurity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[Functionality]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [infra].[Module]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [infra].[Property]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [infra].[SecurityClaim]    Script Date: 9/2/2023 7:57:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[SecurityClaim](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ClaimType] [nvarchar](50) NOT NULL,
	[ClaimValue] [nvarchar](50) NULL,
	[SecurityDescriptorId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_SecurityClaim_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[SecurityDescriptor]    Script Date: 9/2/2023 7:57:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[SecurityDescriptor](
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[IncludeChildren] [bit] NOT NULL,
	[Strategy] [tinyint] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[IsEnabled] [bit] NULL,
 CONSTRAINT [PK_SecurityDescriptor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[SystemMenu]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [infra].[Translation]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [infra].[UiBootstrapPosition]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [infra].[UiComponent]    Script Date: 9/2/2023 7:57:13 PM ******/
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
 CONSTRAINT [PK_UiComponent] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[UiComponentAction]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [infra].[UiComponentProperty]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [infra].[UiPage]    Script Date: 9/2/2023 7:57:13 PM ******/
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
/****** Object:  Table [infra].[UiPageComponent]    Script Date: 9/2/2023 7:57:13 PM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 9/2/2023 7:57:13 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 9/2/2023 7:57:13 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 9/2/2023 7:57:13 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 9/2/2023 7:57:13 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 9/2/2023 7:57:13 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [EmailIndex]    Script Date: 9/2/2023 7:57:13 PM ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 9/2/2023 7:57:13 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CqrsSegregate_ResultDtoId]    Script Date: 9/2/2023 7:57:13 PM ******/
CREATE NONCLUSTERED INDEX [IX_CqrsSegregate_ResultDtoId] ON [infra].[CqrsSegregate]
(
	[ResultDtoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Dto_ModuleId]    Script Date: 9/2/2023 7:57:13 PM ******/
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
ALTER TABLE [infra].[EntitySecurity] ADD  CONSTRAINT [DF_EntitySecurity_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [infra].[Functionality] ADD  CONSTRAINT [DF_Functionality_Guid]  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[Module] ADD  CONSTRAINT [DF_Module_Guid]  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[Property] ADD  CONSTRAINT [DF_Property_Guid]  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[SecurityClaim] ADD  CONSTRAINT [DF_SecurityClaim_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [infra].[SecurityDescriptor] ADD  CONSTRAINT [DF_SecurityDescriptor_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [infra].[SystemMenu] ADD  CONSTRAINT [DF_SystemMenu_Guid]  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[UiComponent] ADD  CONSTRAINT [DF_UiComponent_Guid]  DEFAULT (newsequentialid()) FOR [Guid]
GO
ALTER TABLE [infra].[UiPage] ADD  CONSTRAINT [DF_UiPage_Guid]  DEFAULT (newsequentialid()) FOR [Guid]
GO
ALTER TABLE [infra].[UiPageComponent] ADD  CONSTRAINT [DF_UiPageComponent_Guid]  DEFAULT (newsequentialid()) FOR [Guid]
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
ALTER TABLE [infra].[EntitySecurity]  WITH CHECK ADD  CONSTRAINT [FK_EntitySecurity_SecurityDescriptor] FOREIGN KEY([SecurityDescriptorId])
REFERENCES [infra].[SecurityDescriptor] ([Id])
GO
ALTER TABLE [infra].[EntitySecurity] CHECK CONSTRAINT [FK_EntitySecurity_SecurityDescriptor]
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
ALTER TABLE [infra].[SecurityClaim]  WITH CHECK ADD  CONSTRAINT [FK_SecurityClaim_SecurityDescriptor1] FOREIGN KEY([SecurityDescriptorId])
REFERENCES [infra].[SecurityDescriptor] ([Id])
GO
ALTER TABLE [infra].[SecurityClaim] CHECK CONSTRAINT [FK_SecurityClaim_SecurityDescriptor1]
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
