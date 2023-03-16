﻿USE [MesInfra]
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'infra', @level1type=N'TABLE',@level1name=N'UiPageComponent', @level2type=N'COLUMN',@level2name=N'PositionId'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'infra', @level1type=N'TABLE',@level1name=N'UiComponentProperty', @level2type=N'COLUMN',@level2name=N'PositionId'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'infra', @level1type=N'TABLE',@level1name=N'UiComponentAction', @level2type=N'COLUMN',@level2name=N'Id'
GO
ALTER TABLE [infra].[UiPageComponent] DROP CONSTRAINT [FK_UiPageComponent_UiPage]
GO
ALTER TABLE [infra].[UiPageComponent] DROP CONSTRAINT [FK_UiPageComponent_UiComponent]
GO
ALTER TABLE [infra].[UiPageComponent] DROP CONSTRAINT [FK_UiPageComponent_UiBootstrapPosition]
GO
ALTER TABLE [infra].[UiPage] DROP CONSTRAINT [FK_UiPage_Module]
GO
ALTER TABLE [infra].[UiPage] DROP CONSTRAINT [FK_UiPage_Dto]
GO
ALTER TABLE [infra].[UiComponentProperty] DROP CONSTRAINT [FK_UiComponentProperty_UiComponent]
GO
ALTER TABLE [infra].[UiComponentProperty] DROP CONSTRAINT [FK_UiComponentProperty_UiBootstrapPosition]
GO
ALTER TABLE [infra].[UiComponentProperty] DROP CONSTRAINT [FK_UiComponentProperty_Property1]
GO
ALTER TABLE [infra].[UiComponentAction] DROP CONSTRAINT [FK_UiComponentAction_UiComponent]
GO
ALTER TABLE [infra].[UiComponentAction] DROP CONSTRAINT [FK_UiComponentAction_UiBootstrapPosition]
GO
ALTER TABLE [infra].[UiComponentAction] DROP CONSTRAINT [FK_UiComponentAction_CqrsSegregate]
GO
ALTER TABLE [infra].[UiComponent] DROP CONSTRAINT [FK_UiComponent_Property]
GO
ALTER TABLE [infra].[UiComponent] DROP CONSTRAINT [FK_UiComponent_Dto1]
GO
ALTER TABLE [infra].[SecurityClaim] DROP CONSTRAINT [FK_SecurityClaim_SecurityDescriptor1]
GO
ALTER TABLE [infra].[Property] DROP CONSTRAINT [FK_Property_Dto]
GO
ALTER TABLE [infra].[EntitySecurity] DROP CONSTRAINT [FK_EntitySecurity_SecurityDescriptor]
GO
ALTER TABLE [infra].[Dto] DROP CONSTRAINT [FK_Dto_Module_ModuleId]
GO
ALTER TABLE [infra].[Dto] DROP CONSTRAINT [FK_Dto_Functionality]
GO
ALTER TABLE [infra].[CrudCode] DROP CONSTRAINT [FK_CrudCode_Module]
GO
ALTER TABLE [infra].[CqrsSegregate] DROP CONSTRAINT [FK_CqrsSegregate_Module]
GO
ALTER TABLE [infra].[CqrsSegregate] DROP CONSTRAINT [FK_CqrsSegregate_Functionality]
GO
ALTER TABLE [infra].[CqrsSegregate] DROP CONSTRAINT [FK_CqrsSegregate_Dto_ResultDtoId]
GO
ALTER TABLE [infra].[CqrsSegregate] DROP CONSTRAINT [FK_CqrsSegregate_Dto]
GO
ALTER TABLE [dbo].[AspNetUserTokens] DROP CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserLogins] DROP CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserClaims] DROP CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetRoleClaims] DROP CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [infra].[UiPageComponent] DROP CONSTRAINT [DF_UiPageComponent_Guid]
GO
ALTER TABLE [infra].[UiPage] DROP CONSTRAINT [DF_UiPage_Guid]
GO
ALTER TABLE [infra].[UiComponent] DROP CONSTRAINT [DF_UiComponent_Guid]
GO
ALTER TABLE [infra].[SystemMenu] DROP CONSTRAINT [DF_SystemMenu_Guid]
GO
ALTER TABLE [infra].[SecurityDescriptor] DROP CONSTRAINT [DF_SecurityDescriptor_Id]
GO
ALTER TABLE [infra].[SecurityClaim] DROP CONSTRAINT [DF_SecurityClaim_Id]
GO
ALTER TABLE [infra].[Property] DROP CONSTRAINT [DF_Property_Guid]
GO
ALTER TABLE [infra].[Module] DROP CONSTRAINT [DF_Module_Guid]
GO
ALTER TABLE [infra].[Functionality] DROP CONSTRAINT [DF_Functionality_Guid]
GO
ALTER TABLE [infra].[EntitySecurity] DROP CONSTRAINT [DF_EntitySecurity_Id]
GO
ALTER TABLE [infra].[Dto] DROP CONSTRAINT [DF_Dto_Guid]
GO
ALTER TABLE [infra].[CrudCode] DROP CONSTRAINT [DF_CrudCode_Guid]
GO
ALTER TABLE [infra].[CqrsSegregate] DROP CONSTRAINT [DF_CqrsSegregate_Guid]
GO
/****** Object:  Table [infra].[UiPageComponent]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[infra].[UiPageComponent]') AND type in (N'U'))
DROP TABLE [infra].[UiPageComponent]
GO
/****** Object:  Table [infra].[UiPage]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[infra].[UiPage]') AND type in (N'U'))
DROP TABLE [infra].[UiPage]
GO
/****** Object:  Table [infra].[UiComponentProperty]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[infra].[UiComponentProperty]') AND type in (N'U'))
DROP TABLE [infra].[UiComponentProperty]
GO
/****** Object:  Table [infra].[UiComponentAction]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[infra].[UiComponentAction]') AND type in (N'U'))
DROP TABLE [infra].[UiComponentAction]
GO
/****** Object:  Table [infra].[UiComponent]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[infra].[UiComponent]') AND type in (N'U'))
DROP TABLE [infra].[UiComponent]
GO
/****** Object:  Table [infra].[UiBootstrapPosition]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[infra].[UiBootstrapPosition]') AND type in (N'U'))
DROP TABLE [infra].[UiBootstrapPosition]
GO
/****** Object:  Table [infra].[Translation]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[infra].[Translation]') AND type in (N'U'))
DROP TABLE [infra].[Translation]
GO
/****** Object:  Table [infra].[SystemMenu]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[infra].[SystemMenu]') AND type in (N'U'))
DROP TABLE [infra].[SystemMenu]
GO
/****** Object:  Table [infra].[SecurityDescriptor]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[infra].[SecurityDescriptor]') AND type in (N'U'))
DROP TABLE [infra].[SecurityDescriptor]
GO
/****** Object:  Table [infra].[SecurityClaim]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[infra].[SecurityClaim]') AND type in (N'U'))
DROP TABLE [infra].[SecurityClaim]
GO
/****** Object:  Table [infra].[Property]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[infra].[Property]') AND type in (N'U'))
DROP TABLE [infra].[Property]
GO
/****** Object:  Table [infra].[Module]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[infra].[Module]') AND type in (N'U'))
DROP TABLE [infra].[Module]
GO
/****** Object:  Table [infra].[Functionality]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[infra].[Functionality]') AND type in (N'U'))
DROP TABLE [infra].[Functionality]
GO
/****** Object:  Table [infra].[EntitySecurity]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[infra].[EntitySecurity]') AND type in (N'U'))
DROP TABLE [infra].[EntitySecurity]
GO
/****** Object:  Table [infra].[Dto]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[infra].[Dto]') AND type in (N'U'))
DROP TABLE [infra].[Dto]
GO
/****** Object:  Table [infra].[CrudCode]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[infra].[CrudCode]') AND type in (N'U'))
DROP TABLE [infra].[CrudCode]
GO
/****** Object:  Table [infra].[CqrsSegregate]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[infra].[CqrsSegregate]') AND type in (N'U'))
DROP TABLE [infra].[CqrsSegregate]
GO
/****** Object:  Table [dbo].[Person]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Person]') AND type in (N'U'))
DROP TABLE [dbo].[Person]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserTokens]') AND type in (N'U'))
DROP TABLE [dbo].[AspNetUserTokens]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND type in (N'U'))
DROP TABLE [dbo].[AspNetUsers]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserRoles]') AND type in (N'U'))
DROP TABLE [dbo].[AspNetUserRoles]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserLogins]') AND type in (N'U'))
DROP TABLE [dbo].[AspNetUserLogins]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserClaims]') AND type in (N'U'))
DROP TABLE [dbo].[AspNetUserClaims]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetRoles]') AND type in (N'U'))
DROP TABLE [dbo].[AspNetRoles]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetRoleClaims]') AND type in (N'U'))
DROP TABLE [dbo].[AspNetRoleClaims]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 11-Mar-23 23:11:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__EFMigrationsHistory]') AND type in (N'U'))
DROP TABLE [dbo].[__EFMigrationsHistory]
GO
/****** Object:  Schema [sec]    Script Date: 11-Mar-23 23:11:14 ******/
DROP SCHEMA [sec]
GO
/****** Object:  Schema [infra]    Script Date: 11-Mar-23 23:11:14 ******/
DROP SCHEMA [infra]
GO
/****** Object:  Schema [infra]    Script Date: 11-Mar-23 23:11:14 ******/
CREATE SCHEMA [infra]
GO
/****** Object:  Schema [sec]    Script Date: 11-Mar-23 23:11:14 ******/
CREATE SCHEMA [sec]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [dbo].[Person]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [infra].[CqrsSegregate]    Script Date: 11-Mar-23 23:11:14 ******/
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
	[FunctionalityId] [bigint] NULL,
 CONSTRAINT [PK_CqrsSegregate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[CrudCode]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [infra].[Dto]    Script Date: 11-Mar-23 23:11:14 ******/
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
	[FunctionalityId] [bigint] NULL,
 CONSTRAINT [PK_Dto] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[EntitySecurity]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [infra].[Functionality]    Script Date: 11-Mar-23 23:11:14 ******/
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
 CONSTRAINT [PK_Functionality] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[Module]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [infra].[Property]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [infra].[SecurityClaim]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [infra].[SecurityDescriptor]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [infra].[SystemMenu]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [infra].[Translation]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [infra].[UiBootstrapPosition]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [infra].[UiComponent]    Script Date: 11-Mar-23 23:11:14 ******/
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
	[FunctionalityId] [bigint] NULL,
 CONSTRAINT [PK_UiComponent] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[UiComponentAction]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [infra].[UiComponentProperty]    Script Date: 11-Mar-23 23:11:14 ******/
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
/****** Object:  Table [infra].[UiPage]    Script Date: 11-Mar-23 23:11:14 ******/
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
	[FunctionalityId] [bigint] NULL,
 CONSTRAINT [PK_UiPage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[UiPageComponent]    Script Date: 11-Mar-23 23:11:14 ******/
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
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20201206193111_init-database', N'5.0.0')
GO
SET IDENTITY_INSERT [infra].[CqrsSegregate] ON 
GO
INSERT [infra].[CqrsSegregate] ([Id], [Name], [CqrsNameSpace], [SegregateType], [FriendlyName], [Description], [ParamDtoId], [ResultDtoId], [Guid], [Comment], [ModuleId], [CategoryId], [FunctionalityId]) VALUES (33, N'GetPersonDetailsQuery', N'Mes.Hr.Queries', 0, NULL, NULL, 20221, 20222, N'bfc714c8-8717-4ad4-9d98-09f285ccb722', NULL, 5, 1, NULL)
GO
INSERT [infra].[CqrsSegregate] ([Id], [Name], [CqrsNameSpace], [SegregateType], [FriendlyName], [Description], [ParamDtoId], [ResultDtoId], [Guid], [Comment], [ModuleId], [CategoryId], [FunctionalityId]) VALUES (34, N'GetPersonsListQuery', N'Mes.Hr.Queries', 0, NULL, NULL, 20219, 20220, N'd6d14c07-d26d-4484-b69d-530400f9e597', NULL, 5, 1, NULL)
GO
SET IDENTITY_INSERT [infra].[CqrsSegregate] OFF
GO
SET IDENTITY_INSERT [infra].[Dto] ON 
GO
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [FunctionalityId]) VALUES (20219, N'GetPersonListParamsDto', N'Mes.Hr.Models', 5, NULL, N'af325069-e8fb-44f5-87f8-8037bd3a3168', NULL, 1, 0, 0, NULL)
GO
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [FunctionalityId]) VALUES (20220, N'GetPersonListResultDto', N'Mes.Hr.Models', 5, NULL, N'974d9ced-5556-4e5d-8999-a2b714cb9715', NULL, 0, 1, 0, NULL)
GO
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [FunctionalityId]) VALUES (20221, N'GetPersonDetailsParamsDto', N'Mes.Hr.Models', 5, NULL, N'296d5675-9027-47fe-bcea-bffd169c9c46', NULL, 1, 0, 0, NULL)
GO
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [FunctionalityId]) VALUES (20222, N'GetPersonDetailsResultDto', N'Mes.Hr.Models', 5, NULL, N'f1bb429b-72f2-497e-8c2e-eea1aaecc44a', NULL, 0, 1, 0, NULL)
GO
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [FunctionalityId]) VALUES (20223, N'PersonListViewModel', N'Mes.Hr.Models', 5, NULL, N'a4b9a8f8-e89c-453b-80ca-bfcac7d28b33', NULL, 0, 0, 1, NULL)
GO
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [FunctionalityId]) VALUES (20224, N'PersonDetailsViewModel', N'Mes.Hr.Models', 5, NULL, N'788918a7-9083-47cf-93b0-2a97a3d3251c', NULL, 0, 0, 1, NULL)
GO
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [FunctionalityId]) VALUES (20225, N'PersonViewModel', N'Mes.Hr.Models', 5, NULL, N'1c4db1af-7022-4285-9d5f-af7bbad6eb9f', NULL, 0, 0, 1, NULL)
GO
SET IDENTITY_INSERT [infra].[Dto] OFF
GO
SET IDENTITY_INSERT [infra].[Module] ON 
GO
INSERT [infra].[Module] ([Id], [Name], [Guid], [ParentId]) VALUES (1, N'Financial Management', N'249f14de-7938-eb11-a3b1-0015830cbfeb', NULL)
GO
INSERT [infra].[Module] ([Id], [Name], [Guid], [ParentId]) VALUES (2, N'Inventory Management', N'259f14de-7938-eb11-a3b1-0015830cbfeb', NULL)
GO
INSERT [infra].[Module] ([Id], [Name], [Guid], [ParentId]) VALUES (3, N'Supply Chain Management', N'384571e7-7938-eb11-a3b1-0015830cbfeb', NULL)
GO
INSERT [infra].[Module] ([Id], [Name], [Guid], [ParentId]) VALUES (4, N'Customer Relationship Management (CRM)', N'394571e7-7938-eb11-a3b1-0015830cbfeb', NULL)
GO
INSERT [infra].[Module] ([Id], [Name], [Guid], [ParentId]) VALUES (5, N'Human Resources', N'3c508aef-7938-eb11-a3b1-0015830cbfeb', NULL)
GO
INSERT [infra].[Module] ([Id], [Name], [Guid], [ParentId]) VALUES (6, N'System', N'cb7cb73f-753a-ec11-a58f-f832e4b3c927', NULL)
GO
SET IDENTITY_INSERT [infra].[Module] OFF
GO
SET IDENTITY_INSERT [infra].[Property] ON 
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20605, 20220, 3, N'System.Int64', N'Id', NULL, NULL, NULL, 0, NULL, N'295672101001', N'e03bce04-8e85-43d3-ac25-d8804752ccaa', NULL)
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20606, 20220, 1, N'System.String', N'FirstName', NULL, NULL, NULL, 1, NULL, N'295672101002', N'00c32f94-8970-416e-b76c-e8061a206fbc', NULL)
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20607, 20220, 1, N'System.String', N'LastName', NULL, NULL, NULL, 0, NULL, N'295672101003', N'05ba0d1d-fe03-4de1-bb66-73f8356c6bc8', NULL)
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20608, 20221, 3, N'System.Int64', N'Id', NULL, NULL, NULL, 0, NULL, N'295672101001', N'123f32db-e2f1-4369-a5e1-5f905242a675', NULL)
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20609, 20222, 3, N'System.Int64', N'Id', NULL, NULL, NULL, 0, NULL, N'', N'df57b0b3-5707-4b90-8dc0-35e0f060165e', NULL)
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20610, 20222, 1, N'System.String', N'FirstName', NULL, NULL, NULL, 0, NULL, N'', N'8418a5a0-36f7-4452-b0df-4353ab6c18c8', NULL)
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20611, 20222, 1, N'System.String', N'LastName', NULL, NULL, NULL, 0, NULL, N'', N'176517a3-7e02-4eef-a7cb-cb1608583574', NULL)
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20612, 20222, 8, N'System.DateTime', N'DateOfBirth', NULL, NULL, NULL, 1, NULL, N'', N'e353bc20-6c0c-4ac4-b00e-8674fd6548ad', NULL)
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20613, 20222, 2, N'System.Int32', N'Height', NULL, NULL, NULL, 0, NULL, N'', N'0c6798df-fbe3-4794-8d87-9c19722430eb', NULL)
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20614, 20223, 3, N'System.Int64', N'Id', NULL, NULL, NULL, 0, NULL, N'295672101001', N'6336eb2e-5a3a-4a96-bc9a-93b710c370e8', NULL)
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20615, 20223, 1, N'System.String', N'FirstName', NULL, NULL, NULL, 0, NULL, N'295672101002', N'078ca209-7afe-4fc4-a2c4-d7a470a9e558', NULL)
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20616, 20223, 1, N'System.String', N'LastName', NULL, NULL, NULL, 0, NULL, N'295672101003', N'cd5019ea-451b-4c57-9bfa-c02c012a3e88', NULL)
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20617, 20224, 3, N'System.Int64', N'Id', NULL, NULL, NULL, 0, NULL, N'295672101001', N'd5e5cd19-ef8c-4b9b-8d14-070898d79022', NULL)
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20618, 20224, 1, N'System.String', N'FirstName', NULL, NULL, NULL, 0, NULL, N'295672101002', N'0d8f0fc7-ade4-4888-80a7-b9bf0f0ebe8b', NULL)
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20619, 20224, 1, N'System.String', N'LastName', NULL, NULL, NULL, 0, NULL, N'295672101003', N'4e8fb7ac-7fad-4a2d-8048-35a1ca171772', NULL)
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20620, 20224, 8, N'System.DateTime', N'DateOfBirth', NULL, NULL, NULL, 0, NULL, N'295672101004', N'aa31e454-2042-4c4c-a110-6fbcf1ed5fdb', NULL)
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20621, 20224, 2, N'System.Int32', N'Height', NULL, NULL, NULL, 0, NULL, N'295672101005', N'41724298-33cf-487e-b52c-cb7301cd97ef', NULL)
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20622, 20225, 20, N'Mes.Hr.Models.PersonListViewModel', N'ListViewModel', NULL, NULL, NULL, NULL, NULL, N'', N'f3d0c960-1c19-4388-a47c-df90e28f615b', 20223)
GO
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20623, 20225, 20, N'Mes.Hr.Models.PersonDetailsViewModel', N'DetailsViewModel', NULL, NULL, NULL, NULL, NULL, N'', N'9ceddeca-3119-429c-aadc-2f3cb05acd94', 20224)
GO
SET IDENTITY_INSERT [infra].[Property] OFF
GO
INSERT [infra].[SecurityClaim] ([Id], [ClaimType], [ClaimValue], [SecurityDescriptorId]) VALUES (N'208a6b0f-0e7f-4323-84ab-1abe50f71a0d', N'3', N'4', N'995dd434-66cc-418f-b7c4-965f8c22913f')
GO
INSERT [infra].[SecurityDescriptor] ([Id], [IncludeChildren], [Strategy], [Name], [IsEnabled]) VALUES (N'995dd434-66cc-418f-b7c4-965f8c22913f', 0, 1, N'5', 1)
GO
SET IDENTITY_INSERT [infra].[UiBootstrapPosition] ON 
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2554, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2556, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2557, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2558, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2559, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2560, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2561, 0, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2562, 1, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2563, 2, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2564, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2565, NULL, 1, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2566, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2567, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2568, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2569, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2570, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2571, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2572, NULL, 1, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2573, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2574, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2575, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2576, NULL, 1, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2577, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2578, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2579, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2580, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2581, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2582, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2583, NULL, 1, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2584, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2585, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2586, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2587, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2588, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2589, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2590, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2591, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2592, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2593, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2594, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2595, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2596, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2597, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2598, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2599, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2600, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2601, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2602, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2603, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2604, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2605, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2606, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2607, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2608, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2609, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2610, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2611, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2612, NULL, 2, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2613, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2614, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2615, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2616, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2617, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2618, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2619, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2620, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2621, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2622, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2623, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2624, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2625, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2626, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2627, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2628, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2629, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2630, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2631, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2632, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2633, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2634, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2635, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2636, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2637, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2638, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2639, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2640, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2641, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2642, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2643, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2644, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2645, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2646, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2647, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2648, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2649, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2650, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2651, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2652, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2653, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2654, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2655, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2656, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2667, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2668, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2669, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2670, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2671, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2672, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2673, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2696, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2697, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2698, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2699, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2700, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2701, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2702, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2703, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2704, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2705, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2706, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2707, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2708, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2709, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2710, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2711, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2712, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2713, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2714, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2715, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2716, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2717, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2718, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2719, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2720, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2722, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2723, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2724, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2725, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2726, NULL, NULL, NULL, NULL)
GO
INSERT [infra].[UiBootstrapPosition] ([Id], [Order], [Row], [Col], [ColSpan]) VALUES (2727, NULL, 2, NULL, NULL)
GO
SET IDENTITY_INSERT [infra].[UiBootstrapPosition] OFF
GO
SET IDENTITY_INSERT [infra].[UiComponent] ON 
GO
INSERT [infra].[UiComponent] ([Id], [Name], [Guid], [IsEnabled], [Caption], [ClassName], [NameSpace], [PageDataContextId], [PageDataContextPropertyId], [FunctionalityId]) VALUES (5, N'PersonListComponent', N'a7f9e1cf-6cf5-ec11-8165-0015830cbfeb', NULL, NULL, N'PersonComponent', N'Mes.Hr.UI.Components', 20225, 20622, NULL)
GO
INSERT [infra].[UiComponent] ([Id], [Name], [Guid], [IsEnabled], [Caption], [ClassName], [NameSpace], [PageDataContextId], [PageDataContextPropertyId], [FunctionalityId]) VALUES (6, N'PersonDetailsComponent', N'eb0e3efa-6cf5-ec11-8165-0015830cbfeb', NULL, NULL, N'PersonComponent', N'Mes.Hr.UI.Components', 20225, 20623, NULL)
GO
SET IDENTITY_INSERT [infra].[UiComponent] OFF
GO
SET IDENTITY_INSERT [infra].[UiComponentAction] ON 
GO
INSERT [infra].[UiComponentAction] ([Id], [UiComponentId], [CqrsSegregateId], [TriggerTypeId], [EventHandlerName], [PositionId], [Caption], [IsEnabled], [Name]) VALUES (74, 6, NULL, 1, N'SaveButton_Click', 2646, N'Save', 1, N'SaveButton')
GO
SET IDENTITY_INSERT [infra].[UiComponentAction] OFF
GO
SET IDENTITY_INSERT [infra].[UiComponentProperty] ON 
GO
INSERT [infra].[UiComponentProperty] ([Id], [UiComponentId], [CqrsSegregateId], [PropertyId], [ControlTypeId], [PositionId], [Caption], [IsEnabled], [ControlExtraInfo]) VALUES (488, 6, NULL, 20617, 60, 2647, N'Id', 1, NULL)
GO
INSERT [infra].[UiComponentProperty] ([Id], [UiComponentId], [CqrsSegregateId], [PropertyId], [ControlTypeId], [PositionId], [Caption], [IsEnabled], [ControlExtraInfo]) VALUES (489, 6, NULL, 20618, 30, 2648, N'First Name', 1, NULL)
GO
INSERT [infra].[UiComponentProperty] ([Id], [UiComponentId], [CqrsSegregateId], [PropertyId], [ControlTypeId], [PositionId], [Caption], [IsEnabled], [ControlExtraInfo]) VALUES (490, 6, NULL, 20619, 30, 2649, N'Last Name', 1, NULL)
GO
INSERT [infra].[UiComponentProperty] ([Id], [UiComponentId], [CqrsSegregateId], [PropertyId], [ControlTypeId], [PositionId], [Caption], [IsEnabled], [ControlExtraInfo]) VALUES (491, 6, NULL, 20620, 50, 2650, N'Date Of Birth', 1, NULL)
GO
INSERT [infra].[UiComponentProperty] ([Id], [UiComponentId], [CqrsSegregateId], [PropertyId], [ControlTypeId], [PositionId], [Caption], [IsEnabled], [ControlExtraInfo]) VALUES (492, 6, NULL, 20621, 60, 2651, N'Height', 1, NULL)
GO
INSERT [infra].[UiComponentProperty] ([Id], [UiComponentId], [CqrsSegregateId], [PropertyId], [ControlTypeId], [PositionId], [Caption], [IsEnabled], [ControlExtraInfo]) VALUES (493, 5, NULL, 20614, 60, 2654, N'Id', 1, NULL)
GO
INSERT [infra].[UiComponentProperty] ([Id], [UiComponentId], [CqrsSegregateId], [PropertyId], [ControlTypeId], [PositionId], [Caption], [IsEnabled], [ControlExtraInfo]) VALUES (494, 5, NULL, 20615, 30, 2655, N'First Name', 1, NULL)
GO
INSERT [infra].[UiComponentProperty] ([Id], [UiComponentId], [CqrsSegregateId], [PropertyId], [ControlTypeId], [PositionId], [Caption], [IsEnabled], [ControlExtraInfo]) VALUES (495, 5, NULL, 20616, 30, 2656, N'Last Name', 1, NULL)
GO
SET IDENTITY_INSERT [infra].[UiComponentProperty] OFF
GO
SET IDENTITY_INSERT [infra].[UiPage] ON 
GO
INSERT [infra].[UiPage] ([Id], [Name], [ClassName], [Guid], [NameSpace], [ModuleId], [Route], [DtoId], [FunctionalityId]) VALUES (20, N'PersonPage', N'PersonPage', N'06b4a1b9-6df5-ec11-8165-0015830cbfeb', N'Mes.Hr.Pages', 5, N'/person', 20225, NULL)
GO
SET IDENTITY_INSERT [infra].[UiPage] OFF
GO
SET IDENTITY_INSERT [infra].[UiPageComponent] ON 
GO
INSERT [infra].[UiPageComponent] ([Id], [Guid], [PageId], [UiComponentId], [PositionId]) VALUES (59, N'8384c992-4f1b-ed11-87aa-305a3a4744f5', 20, 6, 2724)
GO
INSERT [infra].[UiPageComponent] ([Id], [Guid], [PageId], [UiComponentId], [PositionId]) VALUES (60, N'cce26b9f-4f1b-ed11-87aa-305a3a4744f5', 20, 6, 2725)
GO
INSERT [infra].[UiPageComponent] ([Id], [Guid], [PageId], [UiComponentId], [PositionId]) VALUES (61, N'd4f1f8ac-4f1b-ed11-87aa-305a3a4744f5', 20, 5, 2726)
GO
INSERT [infra].[UiPageComponent] ([Id], [Guid], [PageId], [UiComponentId], [PositionId]) VALUES (62, N'4852ceb8-4f1b-ed11-87aa-305a3a4744f5', 20, 6, 2727)
GO
SET IDENTITY_INSERT [infra].[UiPageComponent] OFF
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
ALTER TABLE [infra].[CqrsSegregate]  WITH CHECK ADD  CONSTRAINT [FK_CqrsSegregate_Functionality] FOREIGN KEY([FunctionalityId])
REFERENCES [infra].[Functionality] ([Id])
GO
ALTER TABLE [infra].[CqrsSegregate] CHECK CONSTRAINT [FK_CqrsSegregate_Functionality]
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
ALTER TABLE [infra].[Dto]  WITH CHECK ADD  CONSTRAINT [FK_Dto_Functionality] FOREIGN KEY([FunctionalityId])
REFERENCES [infra].[Functionality] ([Id])
GO
ALTER TABLE [infra].[Dto] CHECK CONSTRAINT [FK_Dto_Functionality]
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
ALTER TABLE [infra].[Property]  WITH CHECK ADD  CONSTRAINT [FK_Property_Dto] FOREIGN KEY([DtoId])
REFERENCES [infra].[Dto] ([Id])
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
GO
ALTER TABLE [infra].[UiPageComponent] CHECK CONSTRAINT [FK_UiPageComponent_UiBootstrapPosition]
GO
ALTER TABLE [infra].[UiPageComponent]  WITH CHECK ADD  CONSTRAINT [FK_UiPageComponent_UiComponent] FOREIGN KEY([UiComponentId])
REFERENCES [infra].[UiComponent] ([Id])
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
