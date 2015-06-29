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

/****** Object:  Operator [BizTalk Server Operator]    Script Date: 08/05/2013 13:26:29 ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.sysoperators WHERE name = N'BizTalk Server Operator')
EXEC msdb.dbo.sp_add_operator @name=N'BizTalk Server Operator', 
      @enabled=1, 
      @weekday_pager_start_time=90000, 
      @weekday_pager_end_time=180000, 
      @saturday_pager_start_time=90000, 
      @saturday_pager_end_time=180000, 
      @sunday_pager_start_time=90000, 
      @sunday_pager_end_time=180000, 
      @pager_days=0, 
      @email_address=N'${BizTalkServerOperatorEmail}', 
      @category_name=N'[Uncategorized]'
GO
