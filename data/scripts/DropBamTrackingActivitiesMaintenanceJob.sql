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

USE [msdb]
GO

/****** Object:  Job [BAM Tracking Activities Maintenance]    Script Date: 08/05/2013 15:27:55 ******/
DECLARE @JobId uniqueidentifier
IF  EXISTS (SELECT job_id FROM msdb.dbo.sysjobs_view WHERE name = N'BAM Tracking Activities Maintenance')
BEGIN
	SELECT @JobId = job_id FROM msdb.dbo.sysjobs_view WHERE name = N'BAM Tracking Activities Maintenance'
	EXEC msdb.dbo.sp_delete_job @job_id=@JobId, @delete_unused_schedule=1
END
GO
