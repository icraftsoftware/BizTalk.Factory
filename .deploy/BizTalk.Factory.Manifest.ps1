#region Copyright & License

# Copyright © 2012 - 2020 François Chabot
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
# http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

#endregion

<#
.SYNOPSIS
      .
.DESCRIPTION
      .
      Path resolution is always relative to this file's folder.
.PARAMETER Configuration
      The build configuration under which an assembly has been output when SourceEnvironment is either DEV or BLD,
      either Debug or Release.
.PARAMETER IncludeTestArtifacts
      .
.EXAMPLE
      C:\PS>
      <Description of example>
.NOTES
      Author: François Chabot
      © 2020 be.stateless.
#>
[CmdletBinding()]
param(
   [Parameter(Mandatory = $false, HelpMessage = 'File Layout Configuration')]
   [ValidateSet('Debug', 'Release', 'Package')]
   [string]
   $Configuration = 'Debug',

   [Parameter(Mandatory = $false)]
   [switch]
   $IncludeTestArtifacts
)

function global:Get-ProjectItem {
   [CmdletBinding()]
   [OutputType([System.IO.FileSystemInfo[]])]
   param(
      [Parameter(Position = 0, Mandatory = $false)]
      [psobject]
      $Path = '..\src',

      [Parameter(Position = 1, Mandatory = $true, ValueFromPipeline = $true)]
      [string[]]
      $Project,

      [Parameter(Position = 2, Mandatory = $true)]
      [ValidateSet('Debug', 'Release')]
      [string]
      $Configuration,

      [Parameter(Position = 3, Mandatory = $false)]
      [string[]]
      $Include = @('*.dll', '*.exe')
   )
   process {
      @(
         $Project | ForEach-Object -Process {
            $item = Get-ChildItem -Path ([System.IO.Path]::Combine($Path, $_, 'bin', $Configuration)) `
               -File `
               -Filter "Be.Stateless.$_.*" `
               -Include $Include -Recurse
            if ($null -eq $item) {
               throw "Project item not found [Path: '$Path', Project: '$Project', Configuration: '$Configuration', Include = '$Include']"
            }
            $item
         }
      )
   }
}

Import-Module Resource.Manifest #-Prefix BizTalk

# TODO
$ManagementDatabaseServer = 'localhost'
$MonitoringDatabaseServer = 'localhost'
$ProcessingDatabaseServer = 'localhost'

ApplicationManifest -Name BizTalk.Factory -Description 'Comprehensive Microsoft BizTalk Server Runtime Powered by Be.Stateless.BizTalk.Factory.' -Build {
   Assembly -Path (Get-Item -Path ..\src\packages\SharpZipLib.0.86.0\lib\20\ICSharpCode.SharpZipLib.dll)
   # TODO include pdb
   # TODO include xml comment
   Assembly -Path '..\src\packages\log4net.2.0.8\lib\net40-full\log4net.dll'
   Assembly -Path (Get-ProjectItem `
         -Project 'BizTalk.Dsl', 'BizTalk.Dsl.Binding.Conventions', 'BizTalk.Explorer', 'Common', 'Extensions', 'Logging', 'ServiceModel' `
         -Include '*.dll' `
         -Configuration $Configuration)

   Binding -Path (Get-ProjectItem -Project 'BizTalk.Binding' -Include '*.dll' -Configuration $Configuration)
   #    - EnvironmentSettingOverridesRootPath = '' `
   #    - AssemblyProbingPaths                = @('path', 'path')

   Component -Path (Get-ProjectItem -Project 'BizTalk.Common' -Include '*.dll' -Configuration $Configuration)

   Orchestration -Path (Get-ProjectItem -Project 'BizTalk.TestArtifacts' -Include '*.dll' -Configuration $Configuration) `
      -Condition ([bool]$IncludeTestArtifacts)

   PipelineComponent -Path (Get-ProjectItem -Project 'BizTalk.Pipeline.Components' -Include '*.dll' -Configuration $Configuration)

   Pipeline -Path (Get-ProjectItem -Project 'BizTalk.Pipelines' -Include '*.dll' -Configuration $Configuration)

   # SAP.BiztalkPropertySchema must be deployed when targetting BTS 2013 but must be ignored when targetting BTS 2020
   Schema -Path (Get-Item -Path '..\lib\BizTalk 2013 R2\Microsoft.Adapters.SAP.BiztalkPropertySchema.dll')
   Schema -Path (Get-ProjectItem -Project 'BizTalk.Schemas' -Include '*.dll' -Configuration $Configuration)

   Transform -Path (Get-ProjectItem -Project 'BizTalk.Transforms' -Include '*.dll' -Configuration $Configuration)

   BamActivityModel -Path (Get-Item -Path ..\src\BizTalk.Common\Tracking\ActivityModel\ActivityModel.xml)

   BamIndex -Name BeginTime, InterchangeID, ProcessName, Value1, Value2, Value3 `
      -Activity Process
   BamIndex -Name MessagingStepActivityID, ProcessActivityID `
      -Activity ProcessMessagingStep
   BamIndex -Name InterchangeID, Time, Value1, Value2, Value3 `
      -Activity MessagingStep

   SqlDatabase -Name BizTalkFactoryMgmtDb -Server $ManagementDatabaseServer `
      -Path ..\data\scripts `
      -EnlistInBizTalkBackupJob `
      -Variables @{
      # When creating a login mapped from a Windows principal, use the format [<domainName>\<loginName>]
      # https://docs.microsoft.com/en-us/sql/t-sql/statements/create-user-transact-sql?view=sql-server-ver15#arguments,
      BizTalkApplicationUserGroup     = "$($env:COMPUTERNAME)\BizTalk Application Users"
      BizTalkIsolatedHostUserGroup    = "$($env:COMPUTERNAME)\BizTalk Isolated Host Users"
      BizTalkServerAdministratorGroup = "$($env:COMPUTERNAME)\BizTalk Server Administrators"
   }
   SqlDatabase -Name BizTalkFactoryTransientStateDb -Server $ProcessingDatabaseServer `
      -Path ..\data\scripts `
      -EnlistInBizTalkBackupJob `
      -Variables @{
      BizTalkApplicationUserGroup     = "$($env:COMPUTERNAME)\BizTalk Application Users"
      BizTalkIsolatedHostUserGroup    = "$($env:COMPUTERNAME)\BizTalk Isolated Host Users"
      BizTalkServerAdministratorGroup = "$($env:COMPUTERNAME)\BizTalk Server Administrators"
   }

   SqlDeploymentScript -Path ..\data\scripts\TurnOffGlobalTracking.sql `
      -Server $ManagementDatabaseServer
   SqlDeploymentScript -Path ..\data\scripts\CreateBizTalkServerOperator.sql `
      -Server $ManagementDatabaseServer `
      -Variables @{ BizTalkServerOperatorEmail = 'biztalk.factory@stateless.be' }
   SqlUndeploymentScript -Path  ..\data\scripts\DropBizTalkServerOperator.sql `
      -Server $ManagementDatabaseServer

   SqlDeploymentScript -Path ..\data\scripts\CreateBAMPrimaryImportObjects.sql `
      -Server $MonitoringDatabaseServer `
      -Variables @{ BizTalkApplicationUserGroup = "$($env:COMPUTERNAME)\BizTalk Application Users"
   }
   SqlUndeploymentScript -Path ..\data\scripts\DropBAMPrimaryImportObjects.sql `
      -Server $MonitoringDatabaseServer `
      -Variables @{ BizTalkApplicationUserGroup = "$($env:COMPUTERNAME)\BizTalk Application Users"
   }

   SqlDeploymentScript -Path ..\data\scripts\CreateBamTrackingActivitiesMaintenanceJob.sql `
      -Server $MonitoringDatabaseServer `
      -Variables @{
      BamArchiveWindowTimeLength  = $([timespan]::FromDays(30).Days)
      BamOnlineWindowTimeLength   = $([timespan]::FromDays(15).Days)
      ClaimStoreCheckOutDirectory = 'C:\Files\Drops\BizTalk.Factory\CheckOut'
      MonitoringDatabaseServer    = $env:COMPUTERNAME
   }
   SqlUndeploymentScript -Path ..\data\scripts\DropBamTrackingActivitiesMaintenanceJob.sql `
      -Server $MonitoringDatabaseServer
}
