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

Import-Module ItemGroup -Force
Import-Module BizTalk.Deployment -Force
. BizTalk.Deployment.Tasks
. .\BizTalk.Factory.File.Layout.Conventions.ps1

Enter-Build {
   $script:ApplicationName = 'BizTalk.Factory'
   $script:ItemGroups = Import-ItemGroup -Path .\BizTalk.Factory.ItemGroups.psd1 | Expand-ItemGroup
}

task . Deploy
