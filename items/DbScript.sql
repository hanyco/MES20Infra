USE [master]
GO
/****** Object:  Database [MesInfra]    Script Date: 9/22/2024 8:33:35 AM ******/
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
/****** Object:  Schema [infra]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE SCHEMA [infra]
GO
/****** Object:  Table [dbo].[Person]    Script Date: 9/22/2024 8:33:35 AM ******/
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
/****** Object:  Table [infra].[CqrsSegregate]    Script Date: 9/22/2024 8:33:35 AM ******/
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
/****** Object:  Table [infra].[CrudCode]    Script Date: 9/22/2024 8:33:35 AM ******/
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
	[Guid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_CrudCode] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [infra].[Dto]    Script Date: 9/22/2024 8:33:35 AM ******/
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
 CONSTRAINT [PK_Dto] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[EntityClaim]    Script Date: 9/22/2024 8:33:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[EntityClaim](
	[Id] [uniqueidentifier] NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
	[ClaimId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_EntityClaim] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[Functionality]    Script Date: 9/22/2024 8:33:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[Functionality](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[ModuleId] [bigint] NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[Comment] [nvarchar](50) NULL,
	[GetAllQueryId] [bigint] NOT NULL,
	[GetByIdQueryId] [bigint] NOT NULL,
	[InsertCommandId] [bigint] NOT NULL,
	[UpdateCommandId] [bigint] NOT NULL,
	[DeleteCommandId] [bigint] NOT NULL,
	[SourceDtoId] [bigint] NOT NULL,
 CONSTRAINT [PK_Functionality] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[Module]    Script Date: 9/22/2024 8:33:35 AM ******/
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
/****** Object:  Table [infra].[Property]    Script Date: 9/22/2024 8:33:35 AM ******/
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
/****** Object:  Table [infra].[SecurityClaim]    Script Date: 9/22/2024 8:33:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[SecurityClaim](
	[Id] [uniqueidentifier] NOT NULL,
	[Key] [nvarchar](50) NOT NULL,
	[Parent] [uniqueidentifier] NULL,
 CONSTRAINT [PK_SecurityClaim_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [infra].[SystemMenu]    Script Date: 9/22/2024 8:33:35 AM ******/
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
/****** Object:  Table [infra].[Translation]    Script Date: 9/22/2024 8:33:35 AM ******/
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
/****** Object:  Table [infra].[UiBootstrapPosition]    Script Date: 9/22/2024 8:33:35 AM ******/
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
/****** Object:  Table [infra].[UiComponent]    Script Date: 9/22/2024 8:33:35 AM ******/
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
/****** Object:  Table [infra].[UiComponentAction]    Script Date: 9/22/2024 8:33:35 AM ******/
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
/****** Object:  Table [infra].[UiComponentProperty]    Script Date: 9/22/2024 8:33:35 AM ******/
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
/****** Object:  Table [infra].[UiPage]    Script Date: 9/22/2024 8:33:35 AM ******/
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
/****** Object:  Table [infra].[UiPageComponent]    Script Date: 9/22/2024 8:33:35 AM ******/
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
/****** Object:  Table [infra].[AccessPermission]    Script Date: 9/22/2024 8:33:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [infra].[AccessPermission](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[ClaimId] [uniqueidentifier] NOT NULL,
	[AccessType] [int] NOT NULL,
 CONSTRAINT [PK_AccessPermission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Person] ON 

INSERT [dbo].[Person] ([Id], [FirstName], [LastName], [DateOfBirth], [Height]) VALUES (1, N'Mohammad', N'Rezai', CAST(N'1977-02-02' AS Date), 170)
INSERT [dbo].[Person] ([Id], [FirstName], [LastName], [DateOfBirth], [Height]) VALUES (2, N'Reza', N'Mohammad', CAST(N'1980-03-03' AS Date), 180)
SET IDENTITY_INSERT [dbo].[Person] OFF
GO
SET IDENTITY_INSERT [infra].[CqrsSegregate] ON 

INSERT [infra].[CqrsSegregate] ([Id], [Name], [CqrsNameSpace], [SegregateType], [FriendlyName], [Description], [ParamDtoId], [ResultDtoId], [Guid], [Comment], [ModuleId], [CategoryId]) VALUES (1, N'GetAllPeopleQuery', N'HumanResource.Queries', 0, N'Get All People Query', NULL, 1, 2, N'9ef286e0-5906-4209-9fac-ea2312818667', N'Auto-generated by Functionality Service.', 5, 1)
INSERT [infra].[CqrsSegregate] ([Id], [Name], [CqrsNameSpace], [SegregateType], [FriendlyName], [Description], [ParamDtoId], [ResultDtoId], [Guid], [Comment], [ModuleId], [CategoryId]) VALUES (2, N'GetByIdPersonQuery', N'HumanResource.Queries', 0, N'Get By Id Person Query', NULL, 3, 4, N'cc2b7bf0-8520-4acf-ba9c-0b6632ea5a95', N'Auto-generated by Functionality Service.', 5, 1)
INSERT [infra].[CqrsSegregate] ([Id], [Name], [CqrsNameSpace], [SegregateType], [FriendlyName], [Description], [ParamDtoId], [ResultDtoId], [Guid], [Comment], [ModuleId], [CategoryId]) VALUES (3, N'InsertPersonCommand', N'HumanResource.Commands', 1, N'Insert Person Command', NULL, 5, 6, N'9c89f6ac-14b5-4490-99d2-98977666554a', N'Auto-generated by Functionality Service.', 5, 0)
INSERT [infra].[CqrsSegregate] ([Id], [Name], [CqrsNameSpace], [SegregateType], [FriendlyName], [Description], [ParamDtoId], [ResultDtoId], [Guid], [Comment], [ModuleId], [CategoryId]) VALUES (4, N'UpdatePersonCommand', N'HumanResource.Commands', 1, N'Update Person Command', NULL, 7, 8, N'41dbb1fd-25d6-464c-8eff-3e6fa9a23392', N'Auto-generated by Functionality Service.', 5, 2)
INSERT [infra].[CqrsSegregate] ([Id], [Name], [CqrsNameSpace], [SegregateType], [FriendlyName], [Description], [ParamDtoId], [ResultDtoId], [Guid], [Comment], [ModuleId], [CategoryId]) VALUES (5, N'DeletePersonCommand', N'HumanResource.Commands', 1, N'Delete Person Command', NULL, 9, 10, N'71661312-3d73-4872-b978-4bcc59502cbb', N'Auto-generated by Functionality Service.', 5, 3)
SET IDENTITY_INSERT [infra].[CqrsSegregate] OFF
GO
SET IDENTITY_INSERT [infra].[Dto] ON 

INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [IsList]) VALUES (1, N'GetAllPeopleQuery', N'HumanResource.Dtos', 5, NULL, N'0c34e2f0-9e96-4ce9-91b9-9070bf914d03', N'Auto-generated by Functionality Service.', 1, 0, 0, 0)
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [IsList]) VALUES (2, N'GetAllPeopleQueryResult', N'HumanResource.Dtos', 5, NULL, N'16e1f438-855c-4e20-8d00-3a336b3bd545', N'Auto-generated by Functionality Service.', 0, 1, 0, 0)
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [IsList]) VALUES (3, N'GetByIdPerson', N'HumanResource.Dtos', 5, NULL, N'49363f99-8eb8-4bec-8554-91903bcce324', N'Auto-generated by Functionality Service.', 1, 0, 0, 0)
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [IsList]) VALUES (4, N'GetByIdPersonResult', N'HumanResource.Dtos', 5, NULL, N'1ff420cc-5238-485f-bfc2-d13958e4b1b2', N'Auto-generated by Functionality Service.', 0, 1, 0, 0)
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [IsList]) VALUES (5, N'InsertPerson', N'HumanResource.Dtos', 5, NULL, N'0e07166f-3c07-4fa5-85ff-0ee4d6706cc4', N'Auto-generated by Functionality Service.', 1, 0, 0, 0)
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [IsList]) VALUES (6, N'InsertPersonResult', N'HumanResource.Dtos', 5, NULL, N'9014a562-5f17-43b4-947c-48952ee91da7', N'Auto-generated by Functionality Service.', 0, 1, 0, 0)
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [IsList]) VALUES (7, N'UpdatePerson', N'HumanResource.Dtos', 5, NULL, N'a2bf418a-3291-41a5-9532-2562f29c71ed', N'Auto-generated by Functionality Service.', 1, 0, 0, 0)
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [IsList]) VALUES (8, N'UpdatePersonResult', N'HumanResource.Dtos', 5, NULL, N'432d572e-b85f-4b5c-8bc0-94ddf61b7fd8', N'Auto-generated by Functionality Service.', 0, 1, 0, 0)
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [IsList]) VALUES (9, N'DeletePerson', N'HumanResource.Dtos', 5, NULL, N'a86e9a13-29db-42c1-bad6-215e3f9ccbce', N'Auto-generated by Functionality Service.', 1, 0, 0, 0)
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [IsList]) VALUES (10, N'DeletePersonResult', N'HumanResource.Dtos', 5, NULL, N'ea7c960e-ad06-420e-9aa5-7fa9c716588b', N'Auto-generated by Functionality Service.', 0, 1, 0, 0)
INSERT [infra].[Dto] ([Id], [Name], [NameSpace], [ModuleId], [DbObjectId], [Guid], [Comment], [IsParamsDto], [IsResultDto], [IsViewModel], [IsList]) VALUES (11, N'PersonDto', N'HumanResources.Dtos', 5, NULL, N'489be0f2-b214-4ab0-b01c-de7735109273', NULL, 0, 0, 0, 0)
SET IDENTITY_INSERT [infra].[Dto] OFF
GO
SET IDENTITY_INSERT [infra].[Functionality] ON 

INSERT [infra].[Functionality] ([Id], [Name], [ModuleId], [Guid], [Comment], [GetAllQueryId], [GetByIdQueryId], [InsertCommandId], [UpdateCommandId], [DeleteCommandId], [SourceDtoId]) VALUES (1, N'PersonFunctionality', 5, N'4354d794-f7f1-4933-8f60-115802502dba', NULL, 1, 2, 3, 4, 5, 11)
SET IDENTITY_INSERT [infra].[Functionality] OFF
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

INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (1, 2, 20, N'HumanResources.Dtos.PersonDto?', N'People', NULL, NULL, 1, 1, NULL, N'-1', N'63cffc98-e1d9-4813-b962-6924aaccea19', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (2, 3, 3, N'System.Int64', N'Id', NULL, NULL, NULL, NULL, N'Auto-generated by Functionality Service.', N'-1', N'6dbe9a89-1d6f-4a15-bf0b-15233a005c08', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (3, 4, 3, N'System.Int64', N'Id', NULL, NULL, NULL, 0, N'Auto-generated by Functionality Service.', N'242099903001', N'f52ed368-cd09-4570-89a3-59564cdb4c06', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (4, 4, 1, N'System.String?', N'FirstName', NULL, NULL, NULL, 1, N'Auto-generated by Functionality Service.', N'242099903002', N'd0b4ccb6-c15d-4d9d-9485-2a9a6017a225', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (5, 4, 1, N'System.String', N'LastName', NULL, NULL, NULL, 0, N'Auto-generated by Functionality Service.', N'242099903003', N'73be20a4-c82f-430a-bae4-87bcc77cb1aa', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (6, 4, 8, N'System.DateTime', N'DateOfBirth', NULL, NULL, NULL, 0, N'Auto-generated by Functionality Service.', N'242099903004', N'3fe622b0-7f5a-404e-a05a-28a727db5d07', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (7, 4, 2, N'System.Int32?', N'Height', NULL, NULL, NULL, 1, N'Auto-generated by Functionality Service.', N'242099903005', N'63f43e8e-67c7-4860-8c67-20d607482fef', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (8, 5, 1, N'System.String?', N'FirstName', NULL, NULL, NULL, 1, N'Auto-generated by Functionality Service.', N'242099903002', N'40971d34-b68c-4207-b7ef-9dba6d7a66cf', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (9, 5, 1, N'System.String', N'LastName', NULL, NULL, NULL, 0, N'Auto-generated by Functionality Service.', N'242099903003', N'838d0d56-a8d4-47f6-b909-0eb1821ef10b', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (10, 5, 8, N'System.DateTime', N'DateOfBirth', NULL, NULL, NULL, 0, N'Auto-generated by Functionality Service.', N'242099903004', N'5362fa77-6b60-4187-810c-4a39617beaae', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (11, 5, 2, N'System.Int32?', N'Height', NULL, NULL, NULL, 1, N'Auto-generated by Functionality Service.', N'242099903005', N'252e1c8d-a8cd-4a29-b88e-569956d94850', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (12, 6, 3, N'System.Int64', N'Id', NULL, NULL, NULL, NULL, N'Auto-generated by Functionality Service.', N'-1', N'57d149d7-8037-4a2e-8078-141ed29e68db', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (13, 7, 3, N'System.Int64', N'Id', NULL, NULL, NULL, 0, N'Auto-generated by Functionality Service.', N'242099903001', N'3c9500c9-028c-4275-8c43-724a03154c4a', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (14, 7, 1, N'System.String?', N'FirstName', NULL, NULL, NULL, 1, N'Auto-generated by Functionality Service.', N'242099903002', N'20ee84f8-55f5-4698-951d-552c8ce0f7c7', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (15, 7, 1, N'System.String', N'LastName', NULL, NULL, NULL, 0, N'Auto-generated by Functionality Service.', N'242099903003', N'98fa4fed-ff6b-4161-83ba-d8c4b7b76b2f', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (16, 7, 8, N'System.DateTime', N'DateOfBirth', NULL, NULL, NULL, 0, N'Auto-generated by Functionality Service.', N'242099903004', N'4f9c3174-6533-48b8-8617-9f0a39053ad5', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (17, 7, 2, N'System.Int32?', N'Height', NULL, NULL, NULL, 1, N'Auto-generated by Functionality Service.', N'242099903005', N'4cac5dfa-efb2-48ba-9473-60ba3af0eab2', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (18, 8, 3, N'System.Int64', N'Id', NULL, NULL, NULL, NULL, N'Auto-generated by Functionality Service.', N'-1', N'b6196df3-b162-4e9d-99b7-37b18bab6a11', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (19, 9, 3, N'System.Int64', N'Id', NULL, NULL, NULL, NULL, N'Auto-generated by Functionality Service.', N'-1', N'707fb4f0-4225-4ec3-b96d-cec270594ad4', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (20, 11, 3, N'System.Int64', N'Id', NULL, NULL, NULL, 0, NULL, N'242099903001', N'6aabb767-0f9e-4d9d-ac98-21a72b6d0aac', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (21, 11, 1, N'System.String?', N'FirstName', NULL, NULL, NULL, 1, NULL, N'242099903002', N'c4768761-97c3-43ac-8894-ede12617f281', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (22, 11, 1, N'System.String', N'LastName', NULL, NULL, NULL, 0, NULL, N'242099903003', N'7a51c7b7-3a40-4e03-b656-1e48792b99fd', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (23, 11, 8, N'System.DateTime', N'DateOfBirth', NULL, NULL, NULL, 0, NULL, N'242099903004', N'caffce8a-8d4c-49cb-b37e-097ea3c83ddd', NULL)
INSERT [infra].[Property] ([Id], [ParentEntityId], [PropertyType], [TypeFullName], [Name], [HasSetter], [HasGetter], [IsList], [IsNullable], [Comment], [DbObjectId], [Guid], [DtoId]) VALUES (24, 11, 2, N'System.Int32?', N'Height', NULL, NULL, NULL, 1, NULL, N'242099903005', N'74483806-3b70-410c-8f40-9841d63d27c6', NULL)
SET IDENTITY_INSERT [infra].[Property] OFF
GO
/****** Object:  Index [IX_CqrsSegregate_ModuleId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_CqrsSegregate_ModuleId] ON [infra].[CqrsSegregate]
(
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CqrsSegregate_ParamDtoId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_CqrsSegregate_ParamDtoId] ON [infra].[CqrsSegregate]
(
	[ParamDtoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CqrsSegregate_ResultDtoId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_CqrsSegregate_ResultDtoId] ON [infra].[CqrsSegregate]
(
	[ResultDtoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CrudCode_ModuleId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_CrudCode_ModuleId] ON [infra].[CrudCode]
(
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Dto_ModuleId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Dto_ModuleId] ON [infra].[Dto]
(
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_EntityClaim_ClaimId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_EntityClaim_ClaimId] ON [infra].[EntityClaim]
(
	[ClaimId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Functionality_DeleteCommandId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Functionality_DeleteCommandId] ON [infra].[Functionality]
(
	[DeleteCommandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Functionality_GetAllQueryId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Functionality_GetAllQueryId] ON [infra].[Functionality]
(
	[GetAllQueryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Functionality_GetByIdQueryId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Functionality_GetByIdQueryId] ON [infra].[Functionality]
(
	[GetByIdQueryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Functionality_InsertCommandId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Functionality_InsertCommandId] ON [infra].[Functionality]
(
	[InsertCommandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Functionality_ModuleId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Functionality_ModuleId] ON [infra].[Functionality]
(
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Functionality_SourceDtoId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Functionality_SourceDtoId] ON [infra].[Functionality]
(
	[SourceDtoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Functionality_UpdateCommandId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Functionality_UpdateCommandId] ON [infra].[Functionality]
(
	[UpdateCommandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Property_DtoId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_Property_DtoId] ON [infra].[Property]
(
	[DtoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiComponent_PageDataContextId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_UiComponent_PageDataContextId] ON [infra].[UiComponent]
(
	[PageDataContextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiComponent_PageDataContextPropertyId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_UiComponent_PageDataContextPropertyId] ON [infra].[UiComponent]
(
	[PageDataContextPropertyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiComponentAction_CqrsSegregateId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_UiComponentAction_CqrsSegregateId] ON [infra].[UiComponentAction]
(
	[CqrsSegregateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiComponentAction_PositionId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_UiComponentAction_PositionId] ON [infra].[UiComponentAction]
(
	[PositionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiComponentAction_UiComponentId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_UiComponentAction_UiComponentId] ON [infra].[UiComponentAction]
(
	[UiComponentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiComponentProperty_PositionId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_UiComponentProperty_PositionId] ON [infra].[UiComponentProperty]
(
	[PositionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiComponentProperty_PropertyId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_UiComponentProperty_PropertyId] ON [infra].[UiComponentProperty]
(
	[PropertyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiComponentProperty_UiComponentId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_UiComponentProperty_UiComponentId] ON [infra].[UiComponentProperty]
(
	[UiComponentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiPage_DtoId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_UiPage_DtoId] ON [infra].[UiPage]
(
	[DtoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiPage_ModuleId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_UiPage_ModuleId] ON [infra].[UiPage]
(
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiPageComponent_PageId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_UiPageComponent_PageId] ON [infra].[UiPageComponent]
(
	[PageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiPageComponent_PositionId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_UiPageComponent_PositionId] ON [infra].[UiPageComponent]
(
	[PositionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UiPageComponent_UiComponentId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_UiPageComponent_UiComponentId] ON [infra].[UiPageComponent]
(
	[UiComponentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AccessPermission_ClaimId]    Script Date: 9/22/2024 8:33:35 AM ******/
CREATE NONCLUSTERED INDEX [IX_AccessPermission_ClaimId] ON [infra].[AccessPermission]
(
	[ClaimId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [infra].[CqrsSegregate] ADD  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[CrudCode] ADD  DEFAULT (newsequentialid()) FOR [Guid]
GO
ALTER TABLE [infra].[Dto] ADD  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[EntityClaim] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [infra].[Functionality] ADD  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[Module] ADD  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[Property] ADD  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[SecurityClaim] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [infra].[SystemMenu] ADD  DEFAULT (newid()) FOR [Guid]
GO
ALTER TABLE [infra].[UiComponent] ADD  DEFAULT (newsequentialid()) FOR [Guid]
GO
ALTER TABLE [infra].[UiPage] ADD  DEFAULT (newsequentialid()) FOR [Guid]
GO
ALTER TABLE [infra].[UiPageComponent] ADD  DEFAULT (newsequentialid()) FOR [Guid]
GO
ALTER TABLE [infra].[AccessPermission] ADD  DEFAULT (newid()) FOR [Id]
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
ALTER TABLE [infra].[AccessPermission]  WITH CHECK ADD  CONSTRAINT [FK_AccessPermission_SecurityClaim] FOREIGN KEY([ClaimId])
REFERENCES [infra].[SecurityClaim] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [infra].[AccessPermission] CHECK CONSTRAINT [FK_AccessPermission_SecurityClaim]
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
