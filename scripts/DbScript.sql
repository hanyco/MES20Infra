USE [master]
GO
/****** Object:  Database [MesInfra]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE DATABASE [MesInfra]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'MesInfra', FILENAME = N'I:\Dev\Sql\Data\MesInfra.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'MesInfra_log', FILENAME = N'I:\Dev\Sql\Log\MesInfra_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [MesInfra] SET COMPATIBILITY_LEVEL = 160
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
ALTER DATABASE [MesInfra] SET  ENABLE_BROKER 
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
ALTER DATABASE [MesInfra] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [MesInfra] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [MesInfra] SET RECOVERY SIMPLE 
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
ALTER DATABASE [MesInfra] SET QUERY_STORE = ON
GO
ALTER DATABASE [MesInfra] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [MesInfra]
GO
/****** Object:  Schema [Identity]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE SCHEMA [Identity]
GO
/****** Object:  Schema [infra]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE SCHEMA [infra]
GO
/****** Object:  Table [dbo].[Person]    Script Date: 11/15/2024 10:21:12 PM ******/
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
/****** Object:  Table [Identity].[AspNetRoleClaims]    Script Date: 11/15/2024 10:21:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Identity].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [Identity].[AspNetRoles]    Script Date: 11/15/2024 10:21:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Identity].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [Identity].[AspNetUserClaims]    Script Date: 11/15/2024 10:21:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Identity].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [Identity].[AspNetUserLogins]    Script Date: 11/15/2024 10:21:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Identity].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](128) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [Identity].[AspNetUsers]    Script Date: 11/15/2024 10:21:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Identity].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
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
	[DisplayName] [nvarchar](256) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [Identity].[AspNetUserTokens]    Script Date: 11/15/2024 10:21:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Identity].[AspNetUserTokens](
	[UserId] [nvarchar](128) NOT NULL,
	[LoginProvider] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Value] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[AccessPermissions]    Script Date: 11/15/2024 10:21:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[AccessPermissions](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ParentId] [bigint] NULL,
	[EntityType] [nvarchar](50) NOT NULL,
	[EntityId] [bigint] NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[AccessType] [nvarchar](50) NULL,
	[AccessScope] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NULL,
	[Comment] [nvarchar](max) NULL,
 CONSTRAINT [PK_AccessPermissions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[Controller]    Script Date: 11/15/2024 10:21:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[Controller](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ControllerName] [varchar](255) NOT NULL,
	[ControllerRoute] [varchar](255) NULL,
	[NameSpace] [varchar](255) NULL,
	[IsAnonymousAllow] [bit] NULL,
	[AdditionalUsings] [nvarchar](max) NULL,
	[CtorParams] [nvarchar](max) NULL,
	[ModuleId] [bigint] NOT NULL,
 CONSTRAINT [PK__Controll__3214EC07E36E1F9C] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[ControllerMethod]    Script Date: 11/15/2024 10:21:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[ControllerMethod](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ControllerId] [bigint] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[ReturnType] [varchar](255) NULL,
	[IsAsync] [bit] NULL,
	[Body] [nvarchar](max) NULL,
	[HttpMethods] [nvarchar](max) NULL,
	[Arguments] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[CqrsSegregate]    Script Date: 11/15/2024 10:21:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[CqrsSegregate](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[CqrsNameSpace] [nvarchar](max) NULL,
	[DtoNameSpace] [nvarchar](max) NULL,
	[SegregateType] [int] NOT NULL,
	[FriendlyName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[ParamDtoId] [bigint] NOT NULL,
	[ResultDtoId] [bigint] NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[ModuleId] [bigint] NOT NULL,
	[CategoryId] [int] NOT NULL,
 CONSTRAINT [PK_CqrsSegregate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[Dto]    Script Date: 11/15/2024 10:21:12 PM ******/
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
	[Guid] [uniqueidentifier] NOT NULL,
	[Comment] [nvarchar](50) NULL,
	[IsParamsDto] [bit] NOT NULL,
	[IsResultDto] [bit] NOT NULL,
	[IsViewModel] [bit] NOT NULL,
	[IsList] [bit] NULL,
	[BaseType] [nvarchar](1024) NULL,
 CONSTRAINT [PK_Dto] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[Functionality]    Script Date: 11/15/2024 10:21:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[Functionality](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[ModuleId] [bigint] NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[Comment] [nvarchar](50) NULL,
	[GetAllQueryId] [bigint] NOT NULL,
	[GetByIdQueryId] [bigint] NOT NULL,
	[InsertCommandId] [bigint] NOT NULL,
	[UpdateCommandId] [bigint] NOT NULL,
	[DeleteCommandId] [bigint] NOT NULL,
	[SourceDtoId] [bigint] NOT NULL,
	[ControllerId] [bigint] NOT NULL,
 CONSTRAINT [PK_Functionality] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[Module]    Script Date: 11/15/2024 10:21:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[Module](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[ParentId] [bigint] NULL,
 CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[Property]    Script Date: 11/15/2024 10:21:12 PM ******/
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
	[Guid] [uniqueidentifier] NOT NULL,
	[DtoId] [bigint] NULL,
 CONSTRAINT [PK_Property] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[SystemMenu]    Script Date: 11/15/2024 10:21:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[SystemMenu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ParentId] [bigint] NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[Caption] [nvarchar](1024) NOT NULL,
	[Uri] [nvarchar](max) NULL,
 CONSTRAINT [PK_SystemMenu] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[Translation]    Script Date: 11/15/2024 10:21:12 PM ******/
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
/****** Object:  Table [infra].[UiBootstrapPosition]    Script Date: 11/15/2024 10:21:12 PM ******/
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
/****** Object:  Table [infra].[UiComponent]    Script Date: 11/15/2024 10:21:12 PM ******/
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
/****** Object:  Table [infra].[UiComponentAction]    Script Date: 11/15/2024 10:21:12 PM ******/
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
/****** Object:  Table [infra].[UiComponentProperty]    Script Date: 11/15/2024 10:21:12 PM ******/
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
/****** Object:  Table [infra].[UiPage]    Script Date: 11/15/2024 10:21:12 PM ******/
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
/****** Object:  Table [infra].[UiPageComponent]    Script Date: 11/15/2024 10:21:12 PM ******/
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
/****** Object:  Index [IX_AspNetRoles_RoleName]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_AspNetRoles_RoleName] ON [Identity].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUsers_UserName]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_AspNetUsers_UserName] ON [Identity].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CqrsSegregate_ModuleId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_CqrsSegregate_ModuleId] ON [infra].[CqrsSegregate]
(
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CqrsSegregate_ParamDtoId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_CqrsSegregate_ParamDtoId] ON [infra].[CqrsSegregate]
(
	[ParamDtoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CqrsSegregate_ResultDtoId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_CqrsSegregate_ResultDtoId] ON [infra].[CqrsSegregate]
(
	[ResultDtoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Dto_ModuleId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_Dto_ModuleId] ON [infra].[Dto]
(
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Functionality_DeleteCommandId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_Functionality_DeleteCommandId] ON [infra].[Functionality]
(
	[DeleteCommandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Functionality_GetAllQueryId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_Functionality_GetAllQueryId] ON [infra].[Functionality]
(
	[GetAllQueryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Functionality_GetByIdQueryId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_Functionality_GetByIdQueryId] ON [infra].[Functionality]
(
	[GetByIdQueryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Functionality_InsertCommandId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_Functionality_InsertCommandId] ON [infra].[Functionality]
(
	[InsertCommandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Functionality_ModuleId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_Functionality_ModuleId] ON [infra].[Functionality]
(
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Functionality_SourceDtoId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_Functionality_SourceDtoId] ON [infra].[Functionality]
(
	[SourceDtoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Functionality_UpdateCommandId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_Functionality_UpdateCommandId] ON [infra].[Functionality]
(
	[UpdateCommandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Property_DtoId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_Property_DtoId] ON [infra].[Property]
(
	[DtoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiComponent_PageDataContextId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_UiComponent_PageDataContextId] ON [infra].[UiComponent]
(
	[PageDataContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiComponent_PageDataContextPropertyId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_UiComponent_PageDataContextPropertyId] ON [infra].[UiComponent]
(
	[PageDataContextPropertyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiComponentAction_CqrsSegregateId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_UiComponentAction_CqrsSegregateId] ON [infra].[UiComponentAction]
(
	[CqrsSegregateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiComponentAction_PositionId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_UiComponentAction_PositionId] ON [infra].[UiComponentAction]
(
	[PositionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiComponentAction_UiComponentId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_UiComponentAction_UiComponentId] ON [infra].[UiComponentAction]
(
	[UiComponentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiComponentProperty_PositionId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_UiComponentProperty_PositionId] ON [infra].[UiComponentProperty]
(
	[PositionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiComponentProperty_PropertyId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_UiComponentProperty_PropertyId] ON [infra].[UiComponentProperty]
(
	[PropertyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiComponentProperty_UiComponentId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_UiComponentProperty_UiComponentId] ON [infra].[UiComponentProperty]
(
	[UiComponentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiPage_DtoId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_UiPage_DtoId] ON [infra].[UiPage]
(
	[DtoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiPage_ModuleId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_UiPage_ModuleId] ON [infra].[UiPage]
(
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiPageComponent_PageId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_UiPageComponent_PageId] ON [infra].[UiPageComponent]
(
	[PageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiPageComponent_PositionId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_UiPageComponent_PositionId] ON [infra].[UiPageComponent]
(
	[PositionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiPageComponent_UiComponentId]    Script Date: 11/15/2024 10:21:12 PM ******/
CREATE NONCLUSTERED INDEX [IX_UiPageComponent_UiComponentId] ON [infra].[UiPageComponent]
(
	[UiComponentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [infra].[CqrsSegregate] ADD  CONSTRAINT [DF__CqrsSegreg__Guid__6477ECF3]  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[Dto] ADD  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[Functionality] ADD  CONSTRAINT [DF__Functional__Guid__72C60C4A]  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[Module] ADD  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[Property] ADD  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[SystemMenu] ADD  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[UiComponent] ADD  DEFAULT (newsequentialid()) FOR [Guid]
GO
ALTER TABLE [infra].[UiPage] ADD  DEFAULT (newsequentialid()) FOR [Guid]
GO
ALTER TABLE [infra].[UiPageComponent] ADD  DEFAULT (newsequentialid()) FOR [Guid]
GO
ALTER TABLE [Identity].[AspNetRoleClaims]  WITH CHECK ADD FOREIGN KEY([RoleId])
REFERENCES [Identity].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [infra].[Controller]  WITH CHECK ADD  CONSTRAINT [FK_Controller_Module] FOREIGN KEY([ModuleId])
REFERENCES [infra].[Module] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [infra].[Controller] CHECK CONSTRAINT [FK_Controller_Module]
GO
ALTER TABLE [infra].[ControllerMethod]  WITH CHECK ADD  CONSTRAINT [FK__ApiMethod__Contr__2B0A656D] FOREIGN KEY([ControllerId])
REFERENCES [infra].[Controller] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [infra].[ControllerMethod] CHECK CONSTRAINT [FK__ApiMethod__Contr__2B0A656D]
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
ALTER TABLE [infra].[Dto]  WITH CHECK ADD  CONSTRAINT [FK_Dto_Module_ModuleId] FOREIGN KEY([ModuleId])
REFERENCES [infra].[Module] ([Id])
GO
ALTER TABLE [infra].[Dto] CHECK CONSTRAINT [FK_Dto_Module_ModuleId]
GO
ALTER TABLE [infra].[Functionality]  WITH CHECK ADD  CONSTRAINT [FK_Functionality_Controller] FOREIGN KEY([ControllerId])
REFERENCES [infra].[Controller] ([Id])
GO
ALTER TABLE [infra].[Functionality] CHECK CONSTRAINT [FK_Functionality_Controller]
GO
ALTER TABLE [infra].[Functionality]  WITH CHECK ADD  CONSTRAINT [FK_Functionality_CqrsSegregate] FOREIGN KEY([GetByIdQueryId])
REFERENCES [infra].[CqrsSegregate] ([Id])
GO
ALTER TABLE [infra].[Functionality] CHECK CONSTRAINT [FK_Functionality_CqrsSegregate]
GO
ALTER TABLE [infra].[Functionality]  WITH CHECK ADD  CONSTRAINT [FK_Functionality_CqrsSegregate1] FOREIGN KEY([GetAllQueryId])
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
ALTER TABLE [infra].[Functionality]  WITH CHECK ADD  CONSTRAINT [FK_Functionality_Dto1] FOREIGN KEY([SourceDtoId])
REFERENCES [infra].[Dto] ([Id])
GO
ALTER TABLE [infra].[Functionality] CHECK CONSTRAINT [FK_Functionality_Dto1]
GO
ALTER TABLE [infra].[Functionality]  WITH CHECK ADD  CONSTRAINT [FK_Functionality_Module] FOREIGN KEY([ModuleId])
REFERENCES [infra].[Module] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [infra].[Functionality] CHECK CONSTRAINT [FK_Functionality_Module]
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
