/*
Copyright © 2012 - 2014 François Chabot, Yves Dierick

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

/****** Object:  Database [BizTalkFactoryMgmtDb]    Script Date: 05/08/2012 11:13:29 ******/
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'BizTalkFactoryMgmtDb')
BEGIN
   ALTER DATABASE [BizTalkFactoryMgmtDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
   DROP DATABASE [BizTalkFactoryMgmtDb]
END
GO

CREATE DATABASE [BizTalkFactoryMgmtDb]
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET COMPATIBILITY_LEVEL = 100
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BizTalkFactoryMgmtDb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET ANSI_NULL_DEFAULT OFF
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET ANSI_NULLS OFF
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET ANSI_PADDING OFF
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET ANSI_WARNINGS OFF
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET ARITHABORT OFF
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET AUTO_CLOSE OFF
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET AUTO_CREATE_STATISTICS ON
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET AUTO_SHRINK OFF
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET AUTO_UPDATE_STATISTICS ON
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET CURSOR_CLOSE_ON_COMMIT OFF
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET CURSOR_DEFAULT  GLOBAL
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET CONCAT_NULL_YIELDS_NULL OFF
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET NUMERIC_ROUNDABORT OFF
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET QUOTED_IDENTIFIER OFF
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET RECURSIVE_TRIGGERS OFF
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET  DISABLE_BROKER
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET DATE_CORRELATION_OPTIMIZATION OFF
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET TRUSTWORTHY OFF
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET PARAMETERIZATION SIMPLE
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET READ_COMMITTED_SNAPSHOT OFF
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET HONOR_BROKER_PRIORITY OFF
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET  READ_WRITE
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET RECOVERY FULL
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET  MULTI_USER
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET PAGE_VERIFY CHECKSUM
GO

ALTER DATABASE [BizTalkFactoryMgmtDb] SET DB_CHAINING OFF
GO

USE [BizTalkFactoryMgmtDb]
GO

/****** [BizTalkFactoryMgmtDb] Object:  USER and ROLES     Script Date: 05/08/2012 11:13:29 ******/
CREATE ROLE [BTS_USERS] AUTHORIZATION [dbo]
GO

CREATE USER [$(BizTalkApplicationUserGroup)] FOR LOGIN [$(BizTalkApplicationUserGroup)]
EXEC dbo.sp_addrolemember @rolename=N'BTS_USERS', @membername=N'$(BizTalkApplicationUserGroup)'
GO

CREATE USER [$(BizTalkIsolatedHostUserGroup)] FOR LOGIN [$(BizTalkIsolatedHostUserGroup)]
EXEC dbo.sp_addrolemember @rolename=N'BTS_USERS', @membername=N'$(BizTalkIsolatedHostUserGroup)'
GO

CREATE USER [$(BizTalkServerAdministratorGroup)] FOR LOGIN [$(BizTalkServerAdministratorGroup)]
EXEC dbo.sp_addrolemember @rolename=N'db_owner', @membername=N'$(BizTalkServerAdministratorGroup)'
GO

EXEC dbo.sp_addrolemember @rolename=N'db_datareader', @membername=N'BTS_USERS'
GO
