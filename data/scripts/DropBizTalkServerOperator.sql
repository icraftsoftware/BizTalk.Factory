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
IF EXISTS (SELECT name FROM msdb.dbo.sysoperators WHERE name = N'BizTalk Server Operator')
EXEC msdb.dbo.sp_delete_operator @name=N'BizTalk Server Operator'
GO
