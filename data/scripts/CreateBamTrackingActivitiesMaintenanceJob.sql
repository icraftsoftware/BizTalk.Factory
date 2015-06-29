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

UPDATE dbo.bam_Metadata_Activities
SET OnlineWindowTimeUnit = 'DAY',
OnlineWindowTimeLength = ${BamOnlineWindowTimeLength}
WHERE ActivityName IN ('Process', 'ProcessingStep', 'ProcessMessagingStep', 'MessagingStep')
GO

USE [msdb]
GO

/****** Object:  Job [BAM Tracking Activities Maintenance]    Script Date: 08/05/2013 15:27:55 ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
DECLARE @JobOwner nvarchar(256)
SELECT @ReturnCode = 0

DECLARE @ExistingJobId uniqueidentifier
IF  EXISTS (SELECT job_id FROM msdb.dbo.sysjobs_view WHERE name = N'BAM Tracking Activities Maintenance')
BEGIN
   SELECT @ExistingJobId = job_id FROM msdb.dbo.sysjobs_view WHERE name = N'BAM Tracking Activities Maintenance'
   EXEC msdb.dbo.sp_delete_job @job_id=@ExistingJobId, @delete_unused_schedule=1
END

/* pick up the same owner as the BTS job */
SELECT  @JobOwner = msdb.dbo.SQLAGENT_SUSER_SNAME(owner_sid)
FROM msdb.dbo.sysjobs_view WHERE name = N'MessageBox_Message_Cleanup_BizTalkMsgBoxDb'

DECLARE @CommandLine nvarchar(256)

/****** Object:  JobCategory [Database Maintenance]    Script Date: 08/05/2013 15:27:55 ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'Database Maintenance' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'Database Maintenance'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'BAM Tracking Activities Maintenance', 
      @enabled=1, 
      @notify_level_eventlog=3, 
      @notify_level_email=2, 
      @notify_level_netsend=0, 
      @notify_level_page=0, 
      @delete_level=0, 
      @description=N'Job that runs BAM SSIS data maintenance packages for the Tracking Activity Model.', 
      @category_name=N'Database Maintenance', 
      @owner_login_name = @JobOwner,
      @notify_email_operator_name=N'BizTalk Server Operator', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

/****** Object:  Step [Purge Archived Tracking Activity Data]    Script Date: 08/05/2013 10:40:30 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Purge Archived Tracking Activity Data', 
      @step_id=1, 
      @cmdexec_success_code=0, 
      @on_success_action=3, 
      @on_success_step_id=0, 
      @on_fail_action=3, 
      @on_fail_step_id=0, 
      @retry_attempts=0, 
      @retry_interval=0, 
      @os_run_priority=0, @subsystem=N'TSQL', 
      @command=N'DECLARE @stmt NVARCHAR(MAX)
SELECT @stmt = ISNULL( @stmt + char(10), '''' ) +
   ''DROP TABLE ['' + name + '']''
FROM sys.tables
WHERE create_date < CONVERT(DATE, DATEADD(D, -${BamArchiveWindowTimeLength}, SYSDATETIME())) AND (
   name like ''bam_MessagingStep_%''
   OR name like ''bam_ProcessingStep_%''
   OR name like ''bam_Process_%''
   OR name like ''bam_ProcessMessagingStep_%''
)

exec sp_executesql @stmt', 
      @database_name=N'BAMArchive', 
      @flags=4
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

/****** Object:  Step [Purge Captured Large Message Bodies]    Script Date: 08/05/2013 15:03:42 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Purge Captured Large Message Bodies', 
      @step_id=2, 
      @cmdexec_success_code=0, 
      @on_success_action=3, 
      @on_success_step_id=0, 
      @on_fail_action=3, 
      @on_fail_step_id=0, 
      @retry_attempts=0, 
      @retry_interval=0, 
      @os_run_priority=0, @subsystem=N'ActiveScripting', 
      @command=N'Set fso = CreateObject("Scripting.FileSystemObject")
For Each folder In fso.GetFolder("${ClaimStoreCheckOutDirectory}").SubFolders
   If DateValue(folder.DateCreated) < DateValue(Now - ${BamArchiveWindowTimeLength}) Then
      fso.DeleteFolder folder.Path
   End If
Next
Set fso = nothing', 
      @database_name=N'VBScript', 
      @flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

/****** Object:  Step [Remove Incomplete Tracking Activity Instances]    Script Date: 02/09/2013 10:27:55 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Remove Incomplete Tracking Activity Instances', 
      @step_id=3, 
      @cmdexec_success_code=0, 
      @on_success_action=3, 
      @on_success_step_id=0, 
      @on_fail_action=2, 
      @on_fail_step_id=0, 
      @retry_attempts=0, 
      @retry_interval=0, 
      @os_run_priority=0, @subsystem=N'TSQL', 
      @command=N'DECLARE @threshold DATETIME
SET @threshold = CONVERT(DATETIME, DATEADD(D, -${BamArchiveWindowTimeLength}, SYSDATETIME()))
EXEC RemoveDanglingInstances
   @ActivityName = ''Process'',
   @DateThreshold = @threshold
EXEC RemoveDanglingInstances
   @ActivityName = ''ProcessingStep'',
   @DateThreshold = @threshold
EXEC RemoveDanglingInstances
   @ActivityName = ''ProcessMessagingStep'',
   @DateThreshold = @threshold
EXEC RemoveDanglingInstances
   @ActivityName = ''MessagingStep'',
   @DateThreshold = @threshold', 
      @database_name=N'BAMPrimaryImport', 
      @flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

/****** Object:  Step [Perform Process Tracking Activity Maintenance]    Script Date: 08/05/2013 15:27:55 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Perform Process Tracking Activity Maintenance', 
      @step_id=4, 
      @cmdexec_success_code=0, 
      @on_success_action=3, 
      @on_success_step_id=0, 
      @on_fail_action=2, 
      @on_fail_step_id=0, 
      @retry_attempts=0, 
      @retry_interval=0, 
      @os_run_priority=0, @subsystem=N'SSIS', 
      @command=N'/SQL "\BAM_DM_Process" /SERVER "${MonitoringDatabaseServer}" /WARNASERROR  /CHECKPOINTING OFF /LOGGER "{94150B25-6AEB-4C0D-996D-D37D1C4FDEDA}";3 /REPORTING E', 
      @database_name=N'master', 
      @flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

/****** Object:  Step [Perform ProcessingStep Tracking Activity Maintenance]    Script Date: 08/05/2013 14:02:46 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Perform ProcessingStep Tracking Activity Maintenance', 
      @step_id=5, 
      @cmdexec_success_code=0, 
      @on_success_action=3, 
      @on_success_step_id=0, 
      @on_fail_action=2, 
      @on_fail_step_id=0, 
      @retry_attempts=0, 
      @retry_interval=0, 
      @os_run_priority=0, @subsystem=N'SSIS', 
      @command=N'/SQL "\BAM_DM_ProcessingStep" /SERVER "${MonitoringDatabaseServer}" /CHECKPOINTING OFF /LOGGER "{94150B25-6AEB-4C0D-996D-D37D1C4FDEDA}";3 /REPORTING E', 
      @database_name=N'master', 
      @flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

/****** Object:  Step [Perform ProcessMessagingStep Tracking Activity Maintenance]    Script Date: 08/05/2013 15:27:55 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Perform ProcessMessagingStep Tracking Activity Maintenance', 
      @step_id=6, 
      @cmdexec_success_code=0, 
      @on_success_action=3, 
      @on_success_step_id=0, 
      @on_fail_action=2, 
      @on_fail_step_id=0, 
      @retry_attempts=0, 
      @retry_interval=0, 
      @os_run_priority=0, @subsystem=N'SSIS', 
      @command=N'/SQL "\BAM_DM_ProcessMessagingStep" /SERVER "${MonitoringDatabaseServer}" /CHECKPOINTING OFF /LOGGER "{94150B25-6AEB-4C0D-996D-D37D1C4FDEDA}";3 /REPORTING E', 
      @database_name=N'master', 
      @flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

/****** Object:  Step [Perform MessagingStep Tracking Activity Maintenance]    Script Date: 08/05/2013 15:27:55 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Perform MessagingStep Tracking Activity Maintenance', 
      @step_id=7, 
      @cmdexec_success_code=0, 
      @on_success_action=3, 
      @on_success_step_id=0, 
      @on_fail_action=2, 
      @on_fail_step_id=0, 
      @retry_attempts=0, 
      @retry_interval=0, 
      @os_run_priority=0, @subsystem=N'SSIS', 
      @command=N'/SQL "\BAM_DM_MessagingStep" /SERVER "${MonitoringDatabaseServer}" /CHECKPOINTING OFF /LOGGER "{94150B25-6AEB-4C0D-996D-D37D1C4FDEDA}";3 /REPORTING E', 
      @database_name=N'master', 
      @flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

/****** Object:  Step [Update BAM Archive Activity Tracking Views]    Script Date: 08/05/2013 12:53:46 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Update BAM Archive Activity Tracking Views', 
      @step_id=8, 
      @cmdexec_success_code=0, 
      @on_success_action=3, 
      @on_success_step_id=0, 
      @on_fail_action=2, 
      @on_fail_step_id=0, 
      @retry_attempts=0, 
      @retry_interval=0, 
      @os_run_priority=0, @subsystem=N'TSQL', 
      @command=N'DECLARE @stmt NVARCHAR(MAX)

-- [bam_Process_AllInstances]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N''[dbo].[bam_Process_AllInstances]''))
DROP VIEW [dbo].[bam_Process_AllInstances]

SET @stmt = NULL
SELECT @stmt = ISNULL( @stmt + CHAR(10) + ''UNION ALL'' + CHAR(10), '''' )
   + ''SELECT * FROM ['' + name + '']''
FROM sys.tables
WHERE name like ''bam_Process_Instances%''
ORDER BY create_date DESC

SELECT @stmt = ''CREATE VIEW [dbo].[bam_Process_AllInstances]'' + CHAR(10) + ''AS'' + CHAR(10) + @stmt
EXEC sp_executesql @stmt

-- [bam_ProcessingStep_AllInstances]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N''[dbo].[bam_ProcessingStep_AllInstances]''))
DROP VIEW [dbo].[bam_ProcessingStep_AllInstances]

SET @stmt = NULL
SELECT @stmt = ISNULL( @stmt + CHAR(10) + ''UNION ALL'' + CHAR(10), '''' )
   + ''SELECT * FROM ['' + name + '']''
FROM sys.tables
WHERE name like ''bam_ProcessingStep_Instances%''
ORDER BY create_date DESC

SELECT @stmt = ''CREATE VIEW [dbo].[bam_ProcessingStep_AllInstances]'' + CHAR(10) + ''AS'' + CHAR(10) + @stmt
EXEC sp_executesql @stmt

-- [bam_ProcessMessagingStep_AllInstances]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N''[dbo].[bam_ProcessMessagingStep_AllInstances]''))
DROP VIEW [dbo].[bam_ProcessMessagingStep_AllInstances]

SET @stmt = NULL
SELECT @stmt = ISNULL( @stmt + CHAR(10) + ''UNION ALL'' + CHAR(10), '''' )
   + ''SELECT * FROM ['' + name + '']''
FROM sys.tables
WHERE name like ''bam_ProcessMessagingStep_Instances%''
ORDER BY create_date DESC

SELECT @stmt = ''CREATE VIEW [dbo].[bam_ProcessMessagingStep_AllInstances]'' + CHAR(10) + ''AS'' + CHAR(10) + @stmt
EXEC sp_executesql @stmt

-- [bam_MessagingStep_AllInstances]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N''[dbo].[bam_MessagingStep_AllInstances]''))
DROP VIEW [dbo].[bam_MessagingStep_AllInstances]

SET @stmt = NULL
SELECT @stmt = ISNULL( @stmt + CHAR(10) + ''UNION ALL'' + CHAR(10), '''' )
   + ''SELECT * FROM ['' + name + '']''
FROM sys.tables
WHERE name like ''bam_MessagingStep_Instances%''
ORDER BY create_date DESC

SELECT @stmt = ''CREATE VIEW [dbo].[bam_MessagingStep_AllInstances]'' + CHAR(10) + ''AS'' + CHAR(10) + @stmt
EXEC sp_executesql @stmt

-- [bam_MessagingStep_AllRelationships]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N''[dbo].[bam_MessagingStep_AllRelationships]''))
DROP VIEW [dbo].[bam_MessagingStep_AllRelationships]

SET @stmt = NULL
SELECT @stmt = ISNULL( @stmt + CHAR(10) + ''UNION ALL'' + CHAR(10), '''' )
   + ''SELECT * FROM ['' + name + '']''
FROM sys.tables
WHERE name like ''bam_MessagingStep_Relationships%''
ORDER BY create_date DESC

SELECT @stmt = ''CREATE VIEW [dbo].[bam_MessagingStep_AllRelationships]'' + CHAR(10) + ''AS'' + CHAR(10) + @stmt
EXEC sp_executesql @stmt

-- [vw_MessagingStepContexts]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N''[dbo].[vw_MessagingStepContexts]''))
DROP VIEW [dbo].[vw_MessagingStepContexts]

SELECT @stmt = ''SELECT ActivityID AS MessagingStepActivityID, LongReferenceData AS EncodedContext'' + CHAR(10)
   + ''FROM [bam_MessagingStep_AllRelationships]'' + CHAR(10)
   + ''WHERE ReferenceType = ''''Ctxt''''''
SELECT @stmt = ''CREATE VIEW [dbo].[vw_MessagingStepContexts]'' + CHAR(10) + ''AS'' + CHAR(10) + @stmt
EXEC sp_executesql @stmt

-- [vw_MessagingStepMessages]
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N''[dbo].[vw_MessagingStepMessages]''))
DROP VIEW [dbo].[vw_MessagingStepMessages]

SELECT @stmt = ''SELECT ActivityID AS MessagingStepActivityID, ReferenceType AS EncodedBodyType, LongReferenceData AS EncodedBody'' + CHAR(10)
   + ''FROM [bam_MessagingStep_AllRelationships]'' + CHAR(10)
   + ''WHERE ReferenceType = ''''Claimed'''' OR ReferenceType = ''''Unclaimed''''''
SELECT @stmt = ''CREATE VIEW [dbo].[vw_MessagingStepMessages]'' + CHAR(10) + ''AS'' + CHAR(10) + @stmt
EXEC sp_executesql @stmt', 
      @database_name=N'BAMArchive', 
      @flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

/****** Object:  Step [Purge Package Logging]    Script Date: 08/05/2013 15:27:55 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Purge Package Logging', 
      @step_id=9, 
      @cmdexec_success_code=0, 
      @on_success_action=1, 
      @on_success_step_id=0, 
      @on_fail_action=2, 
      @on_fail_step_id=0, 
      @retry_attempts=0, 
      @retry_interval=0, 
      @os_run_priority=0, @subsystem=N'TSQL', 
      @command=N'DELETE FROM sysssislog WHERE StartTime < CONVERT(DATE, DATEADD(D, -${BamArchiveWindowTimeLength}, SYSDATETIME()))', 
      @database_name=N'msdb', 
      @flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'Daily', 
      @enabled=1, 
      @freq_type=4, 
      @freq_interval=1, 
      @freq_subday_type=1, 
      @freq_subday_interval=0, 
      @freq_relative_interval=0, 
      @freq_recurrence_factor=0, 
      @active_start_date=20130805, 
      @active_end_date=99991231, 
      @active_start_time=4800, 
      @active_end_time=235959, 
      @schedule_uid=N'0e239400-f91d-4625-ab0a-3758dfdb61dd'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:
GO
