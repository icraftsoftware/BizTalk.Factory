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

/****** Include in Backup BizTalk Server job ******/
USE [BizTalkMgmtDb]
GO

MERGE INTO [adm_OtherBackupDatabases] WITH (HOLDLOCK) AS [TARGET]
   USING (SELECT 'BizTalkFactoryTransientStateDb' AS [DatabaseName], '${ProcessingDatabaseServer}' AS [ServerName], '${BTSServer}' AS [BTSServerName]) AS [SOURCE]
   ON [TARGET].[DatabaseName] = [SOURCE].[DatabaseName]
WHEN MATCHED THEN
   UPDATE SET [DefaultDatabaseName] = [SOURCE].[DatabaseName],
              [DatabaseName] = [SOURCE].[DatabaseName],
              [ServerName] = [SOURCE].[ServerName],
              [BTSServerName] = [SOURCE].[BTSServerName]
WHEN NOT MATCHED THEN
   INSERT ([DefaultDatabaseName],
          [DatabaseName],
          [ServerName],
          [BTSServerName])
   VALUES ([SOURCE].[DatabaseName],
          [SOURCE].[DatabaseName],
          [SOURCE].[ServerName],
          [SOURCE].[BTSServerName]);
GO

-- ensure a full backup is taken next time is is scheduled, otherwise backup of log
-- will fail as there will be no previous full backup of new database
EXEC sp_ForceFullBackup
GO
