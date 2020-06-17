/*
Copyright © 2012 François Chabot, Yves Dierick

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

USE [master]
GO

/****** Object:  Database [BizTalkFactoryTransientStateDb]    Script Date: 05/08/2012 11:13:29 ******/
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'BizTalkFactoryTransientStateDb')
BEGIN
   ALTER DATABASE [BizTalkFactoryTransientStateDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
   DROP DATABASE [BizTalkFactoryTransientStateDb]
END
GO

CREATE DATABASE [BizTalkFactoryTransientStateDb]
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET COMPATIBILITY_LEVEL = 100
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BizTalkFactoryTransientStateDb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET ANSI_NULL_DEFAULT OFF
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET ANSI_NULLS OFF
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET ANSI_PADDING OFF
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET ANSI_WARNINGS OFF
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET ARITHABORT OFF
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET AUTO_CLOSE OFF
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET AUTO_CREATE_STATISTICS ON
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET AUTO_SHRINK OFF
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET AUTO_UPDATE_STATISTICS ON
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET CURSOR_CLOSE_ON_COMMIT OFF
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET CURSOR_DEFAULT  GLOBAL
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET CONCAT_NULL_YIELDS_NULL OFF
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET NUMERIC_ROUNDABORT OFF
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET QUOTED_IDENTIFIER OFF
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET RECURSIVE_TRIGGERS OFF
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET  DISABLE_BROKER
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET DATE_CORRELATION_OPTIMIZATION OFF
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET TRUSTWORTHY OFF
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET PARAMETERIZATION SIMPLE
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET READ_COMMITTED_SNAPSHOT OFF
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET HONOR_BROKER_PRIORITY OFF
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET  READ_WRITE
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET RECOVERY FULL
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET  MULTI_USER
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET PAGE_VERIFY CHECKSUM
GO

ALTER DATABASE [BizTalkFactoryTransientStateDb] SET DB_CHAINING OFF
GO

USE [BizTalkFactoryTransientStateDb]
GO

/****** [BizTalkFactoryTransientStateDb] Object:  USER and ROLES     Script Date: 05/08/2012 11:13:29 ******/
CREATE ROLE [BTS_USERS] AUTHORIZATION [dbo]
GO

CREATE USER [$(BizTalkApplicationUserGroup)] FOR LOGIN [$(BizTalkApplicationUserGroup)]
EXEC dbo.sp_addrolemember @rolename=N'BTS_USERS', @membername=N'$(BizTalkApplicationUserGroup)'
GO

CREATE USER [$(BizTalkIsolatedHostUserGroup)] FOR LOGIN [$(BizTalkIsolatedHostUserGroup)]
EXEC dbo.sp_addrolemember @rolename=N'BTS_USERS', @membername=N'$(BizTalkIsolatedHostUserGroup)'
GO

CREATE USER [$(BizTalkServerAdministratorGroup)] FOR LOGIN [$(BizTalkServerAdministratorGroup)]
EXEC dbo.sp_addrolemember N'db_owner', N'$(BizTalkServerAdministratorGroup)'
GO

EXEC sp_addrolemember N'db_datareader', N'BTS_USERS'
GO

EXEC sp_addrolemember N'db_datawriter', N'BTS_USERS'
GO
