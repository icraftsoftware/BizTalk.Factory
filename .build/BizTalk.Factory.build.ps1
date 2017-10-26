#region Copyright & License

# Copyright © 2012 - 2017 François Chabot, Yves Dierick
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

# https://github.com/nightroman/Invoke-Build

param(
   # Custom build root, still the original $BuildRoot by default.
   $BuildRoot = (Join-Path -Path $BuildRoot -ChildPath '..')
)

# TODO ensure git repo is in sync with remote

task . Clean, Build, PackAll, Tag, Export

task AssignBuildNumber {
   Invoke-MSBuild .\src\AssignBuildNumber\Be.Stateless.AssignBuildNumber.proj
}

task -Name Build -Jobs RestorePackages, AssignBuildNumber, BuildTools, BuildCore

task BuildCore BuildBizTalkFactoryDebug, BuildBizTalkFactoryRelease, BuildBizTalkMonitoringRelease

task BuildBizTalkFactoryDebug {
   Write-Information -MessageData 'Building BizTalk.Factory solution in Debug configuration' -InformationAction Continue
   Invoke-MSBuild .\src\BizTalk.Factory.sln -Targets Rebuild `
      -AssemblyOriginatorKeyFile '..\be.stateless.snk' `
      -Configuration Debug `
      -DelaySign false `
      -NoWarn 1591
}

task BuildBizTalkFactoryRelease {
   Write-Information -MessageData 'Building BizTalk.Factory solution in Release configuration' -InformationAction Continue
   Invoke-MSBuild .\src\BizTalk.Factory.sln -Targets Rebuild `
      -AssemblyOriginatorKeyFile '..\be.stateless.snk' `
      -Configuration Release `
      -DelaySign false `
      -NoWarn 1591
}

task BuildBizTalkMonitoringRelease {
   Write-Information -MessageData 'Building BizTalk.Factory Web Monitoring solution in Release configuration' -InformationAction Continue
   Invoke-MSBuild .\src\BizTalk.Monitoring.sln -Targets Rebuild `
      -AssemblyOriginatorKeyFile '..\be.stateless.snk' `
      -Configuration Release `
      -DelaySign false `
      -NoWarn 1591
}

task BuildTools RestorePackages, AssignBuildNumber, BuildToolsCore, UpdateMSBuildImports

task BuildToolsCore {
   Invoke-MSBuild -Project .\src\BizTalk.Dsl.MSBuild\Be.Stateless.BizTalk.Dsl.MSBuild.csproj -Targets Rebuild `
      -Configuration Debug `
      -DelaySign false `
      -AssemblyOriginatorKeyFile '..\be.stateless.snk'
}

task Clean {
   Clear-Project -Path .\src -Recurse -Packages
}

# SYNOPSIS: Export all build outputs underneath a common .exports root folder
task Export ExportAssemblies, ExportBizTalkMonitoringWebSite, ExportDeploymentTools, ExportMsiPackages, ExportNugetPackage, ExportSqlScripts, ExportUtilities

# SYNOPSIS: Export BizTalk.Factory and BizTalk.Monitoring assemblies
task ExportAssemblies ExportDebugAssemblies, ExportReleaseAssemblies
task ExportDebugAssemblies ExportBizTalkFactoryDebugAssemblies
task ExportReleaseAssemblies ExportBizTalkFactoryReleaseAssemblies

# SYNOPSIS: Export BizTalk.Factory Debug assemblies
task ExportBizTalkFactoryDebugAssemblies BuildBizTalkFactoryDebug, {
   New-Item -Path . -Name .exports\lib\debug -ItemType Directory -Force | Out-Null
   Copy-Item -Path src\BizTalk.Common\Tracking\ActivityModel\ActivityModel.xml -Destination .exports\lib\debug -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }
   Copy-Item -Path src\.imports\Be.Stateless.BizTalk.targets -Destination .exports\lib\debug -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }
   Copy-Item -Path src\.imports\Be.Stateless.Dsl.targets -Destination .exports\lib\debug -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }
   Get-ChildItem -Path src -Directory -Exclude '.*', 'AssignBuildNumber', 'BizTalk.Pipeline.Definitions', '*.Tests' `
      | ForEach-Object -Process { Get-ChildItem -Path $_\bin\Debug -Filter "*$($_.Name)*" -Include '*.dll', '*.pdb', '*.xml' -Exclude '*.ClaimStore.*' -Recurse -ErrorAction Ignore } `
      | Copy-Item -Destination .exports\lib\debug -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }
   Copy-Item -Path src\BizTalk.ClaimStore.Agent\bin\Debug\*.* -Destination .exports\lib\debug -Include '*.Agent.exe', '*.Agent.pdb', '*.template.config' -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }
}

# SYNOPSIS: Export BizTalk.Factory Release assemblies
task ExportBizTalkFactoryReleaseAssemblies BuildBizTalkFactoryRelease, {
   New-Item -Path . -Name .exports\lib\release -ItemType Directory -Force | Out-Null
   Copy-Item -Path src\BizTalk.Common\Tracking\ActivityModel\ActivityModel.xml -Destination .exports\lib\release -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }
   Copy-Item -Path src\.imports\Be.Stateless.BizTalk.targets -Destination .exports\lib\release -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }
   Copy-Item -Path src\.imports\Be.Stateless.Dsl.targets -Destination .exports\lib\release -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }
   Get-ChildItem -Path src -Directory -Exclude '.*', 'AssignBuildNumber', 'BizTalk.Pipeline.Definitions', '*.Tests' `
      | ForEach-Object -Process { Get-ChildItem -Path $_\bin\Release -Filter "*$($_.Name)*" -Include '*.dll', '*.pdb', '*.xml' -Exclude '*.ClaimStore.*' -Recurse -ErrorAction Ignore } `
      | Copy-Item -Destination .exports\lib\release -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }
   Copy-Item -Path src\BizTalk.ClaimStore.Agent\bin\Release\*.* -Destination .exports\lib\release -Include '*.Agent.exe', '*.Agent.pdb', '*.template.config' -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }
}

# SYNOPSIS: Export BizTalk.Monitoring web site
task ExportBizTalkMonitoringWebSite BuildBizTalkMonitoringRelease, {
   New-Item -Path . -Name .exports\BizTalk.Web.Monitoring.Site -ItemType Directory -Force | Out-Null
   Copy-Item -Path src\BizTalk.Web.Monitoring.Site\*.* -Destination .exports\BizTalk.Web.Monitoring.Site -Include 'Global.asax', 'Web.config' -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }

   New-Item -Path . -Name .exports\BizTalk.Web.Monitoring.Site\bin -ItemType Directory -Force | Out-Null
   @( 'src\BizTalk.Web.Monitoring.Site\bin\*.*'
      'src\BizTalk.Web.Monitoring.Site\_bin_deployableAssemblies\*.*'
      '.exports\lib\release\Be.Stateless.BizTalk.Common.*'
      '.exports\lib\release\Be.Stateless.BizTalk.Schemas.*'
      '.exports\lib\release\Be.Stateless.Logging.*' ) `
      | Copy-Item -Destination .exports\BizTalk.Web.Monitoring.Site\bin -Exclude '*.xml' -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }
   @( 'src\BizTalk.Web.Monitoring.Site\Config', 'src\BizTalk.Web.Monitoring.Site\Content' ) `
      | Copy-Item -Destination .exports\BizTalk.Web.Monitoring.Site -Recurse -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData (Resolve-Path -Path $_ -Relative) -InformationAction Continue }

   New-Item -Path . -Name .exports\BizTalk.Web.Monitoring.Site\Scripts -ItemType Directory -Force | Out-Null
   Copy-Item -Path src\BizTalk.Web.Monitoring.Site\Scripts\*.* -Destination .exports\BizTalk.Web.Monitoring.Site\Scripts -Include 'be.stateless.*', '*.contextMenu.js', 'modernizr*.*', '*.min.js' -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }
   Copy-Item -Path src\BizTalk.Web.Monitoring.Site\Views -Destination .exports\BizTalk.Web.Monitoring.Site -Recurse -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData (Resolve-Path -Path $_ -Relative) -InformationAction Continue }
}

task ExportDeploymentTools {
   New-Item -Path . -Name .exports\src\Deployment -ItemType Directory -Force | Out-Null
   Copy-Item -Path src\Deployment\BizTalk.Factory.ItemGroups.proj -Destination .exports\src\Deployment -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }

   New-Item -Path . -Name '.exports\utils\Deployment Tools' -ItemType Directory -Force | Out-Null
   @( 'utils\Deployment Tools\adapterXPaths.txt', 'src\Deployment\BizTalk.Factory.Deployment.targets' ) `
      | Copy-Item -Destination '.exports\utils\Deployment Tools' -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }
}

# SYNOPSIS: Export MSI packages
task ExportMsiPackages GetProductVersion, PackMsi, {
   Copy-Item -Path src\Deployment\bin\Debug\Installer\BizTalk.Factory\BizTalk.Factory-1.0.0.msi -Destination .exports\BizTalk.Factory.$productVersion-Debug.msi -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }
   Copy-Item -Path src\Deployment\bin\Release\Installer\BizTalk.Factory\BizTalk.Factory-1.0.0.msi -Destination .exports\BizTalk.Factory.$productVersion-Release.msi -Force -PassThru  `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }
}

# SYNOPSIS: Export NuGet package
task ExportNugetPackage GetProductVersion, PackNuget, {
   Move-Item -Path BizTalk.Factory.$productVersion.nupkg -Destination .\.exports -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }
}

# SYNOPSIS: Export SQL Server Scripts
task ExportSqlScripts {
   New-Item -Path . -Name .exports\data\scripts -ItemType Directory -Force | Out-Null
   Copy-Item -Path data\scripts\*.* -Destination .exports\data\scripts -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }
}

task ExportUtilities {
   New-Item -Path . -Name '.exports\utils\Decode Bindings' -ItemType Directory -Force | Out-Null
   Copy-Item -Path 'utils\Decode Bindings\bindings-cleaner.xslt' -Destination '.exports\utils\Decode Bindings' -Force -PassThru `
      | ForEach-Object -Process { Write-Information -MessageData $_.Name -InformationAction Continue }
}

task GetProductVersion Build, {
   $script:productVersion = (Get-Item .\src\Common\bin\release\Be.Stateless.Common.dll).VersionInfo.ProductVersion
}

task PackAll PackNuget, PackMsi

# SYNOPSIS: Build MSI Debug and Release packages
task PackMsi BuildBizTalkFactoryDebug, BuildBizTalkFactoryRelease, GetProductVersion, {
   Write-Information -MessageData 'Packing MSI in Debug configuration' -InformationAction Continue
   Invoke-MSBuild -Project .\src\Deployment\BizTalk.Factory.Deployment.btdfproj -Targets Installer -Configuration Debug
   Write-Information -MessageData 'Packing MSI in Release configuration' -InformationAction Continue
   Invoke-MSBuild -Project .\src\Deployment\BizTalk.Factory.Deployment.btdfproj -Targets Installer -Configuration Release
}

# SYNOPSIS: Build NuGet package
task PackNuget BuildBizTalkFactoryDebug, GetProductVersion, {
   Write-Information -MessageData 'Packing Nuget' -InformationAction Continue
   exec { src\.nuget\NuGet.exe pack src\BizTalk.Factory.nuspec -Version $productVersion -NonInteractive -NoPackageAnalysis -NoDefaultExcludes }
}

# SYNOPSIS: Restore NuGet packages
task RestorePackages Clean, {
   if (Test-Path -Path .exports) { Remove-Item -Path .exports -Confirm:$false -Force -Recurse }
   exec { src\.nuget\NuGet.exe restore src\BizTalk.Factory.sln }
   exec { src\.nuget\NuGet.exe restore src\BizTalk.Monitoring.sln }
}

task Tag GetProductVersion, {
   exec { git add src/.imports/* }
   exec { git add src/Version.cs }
   exec { git commit -m "BizTalk.Factory Build Tools $productVersion" }
   exec { git tag s -m "Tagging BizTalk.Factory $productVersion" "V:$productVersion" }
}

# SYNOPSIS: Update MSBuild imports, that is Be.Stateless.BizTalk.Dsl.MSBuild.dll and related assemblies residing in .imports folder.
task UpdateMSBuildImports {
   @('Be.Stateless.BizTalk.Dsl.dll', 'Be.Stateless.BizTalk.Dsl.MSBuild.dll', 'Be.Stateless.Common.dll', 'Be.Stateless.Extensions.dll') `
      | ForEach-Object -Process { Get-ChildItem -Path .\src\BizTalk.Dsl.MSBuild\bin\Debug -Filter $_ } `
      | Copy-Item -Destination .\src\.imports\
}
