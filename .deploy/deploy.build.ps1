param(
   # [Parameter(Position = 0, Mandatory = $true)]
   # [ValidateNotNullOrEmpty()]
   # [string]
   # $ApplicationName,

   [Parameter()]
   [switch]
   $SkipMgmtDbDeployment,

   [Parameter()]
   [switch]
   $SkipInstallUtil,

   [Parameter()]
   [switch]
   $SkipUndeploy,

   [Parameter()]
   [switch]
   $TerminateServiceInstances
)

Import-Module ItemGroup
Import-Module BizTalk.Deployment
. BizTalk.Deployment.Tasks
. .\BizTalk.Factory.File.Layout.Conventions.ps1

Enter-Build {
   $script:ApplicationName = 'BizTalk.Factory'
   $script:ItemGroups = Import-ItemGroup -Path .\BizTalk.Factory.ItemGroups.psd1 | Expand-ItemGroup
}

# TODO works for DEV but what about BLD, ACC and PRD; should be done inside BizTalk.Deployment.Tasks.ps1
Import-Module MSBuild
Switch-VisualStudioEnvironment -Version 2013
use "$($env:WindowsSDK_ExecutablePath_x64)" GacUtil
use "$($env:BTSINSTALLPATH)" BTSTask
use 'Framework\v4.0.30319' InstallUtil

task . Deploy
