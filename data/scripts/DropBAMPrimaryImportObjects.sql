/*
Copyright © 2012 - 2013 François Chabot, Yves Dierick

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

USE [BAMPrimaryImport]
GO

/****** Object:  StoredProcedure [dbo].[RemoveDanglingInstances]    Script Date: 09/09/2013 23:09:50 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RemoveDanglingInstances]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[RemoveDanglingInstances]
GO

/****** Object:  View [dbo].[vw_MessagingStepContexts]    Script Date: 03/22/2011 15:19:52 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_MessagingStepContexts]'))
   DROP VIEW [dbo].[vw_MessagingStepContexts]
GO

/****** Object:  View [dbo].[vw_MessagingStepMessages]    Script Date: 03/22/2011 15:19:52 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_MessagingStepMessages]'))
   DROP VIEW [dbo].[vw_MessagingStepMessages]
GO

/****** Revoke IIS BizTalkActivityMonitoring  AppPool identity's read access to BamPrimaryImport Database ******/
EXEC dbo.sp_droprolemember @rolename=N'db_datareader', @membername=N'$(BizTalkApplicationUserGroup)'
GO
