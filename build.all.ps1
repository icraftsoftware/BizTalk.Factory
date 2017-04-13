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

<#
 First, we get all the sources from version control, and generate a build version number for
 this build. , the MSBuild project will build the BizTalk.Factory.sln solution, for both the
 Debug and Release configurations, and BizTalk.Monitoring.sln, for the Release configuration
 only. Finally, a label is created in TFS with the generated version number.
#>

[CmdletBinding()]
param(
   [Parameter(Position=1,Mandatory=$false)]
   [string[]]
   $Targets = 'Build'
)

Clear-Project -Path .\src -Recurse -Packages

src\.nuget\NuGet.exe restore src\BizTalk.Factory.sln
if (-not $?) { exit $LASTEXITCODE }
src\.nuget\NuGet.exe restore src\BizTalk.Monitoring.sln
if (-not $?) { exit $LASTEXITCODE }

Invoke-MSBuild -Project .\build.proj -Targets $Targets

# Build the NuGet package containing the release version of all assemblies
# Construct NuGet package version from assembly file version
$version = (Get-Item .\src\Common\bin\release\Be.Stateless.Common.dll).VersionInfo
$packageVersion = "$($version.ProductMajorPart).$($version.ProductMinorPart).$($version.ProductBuildPart)"
src\.nuget\NuGet.exe pack src\BizTalk.Factory.nuspec -Version $packageVersion -NonInteractive -NoPackageAnalysis -NoDefaultExcludes
if (-not $?) { exit $LASTEXITCODE }

# Build the MSI installation packages for both configurations
Invoke-MSBuild -Project .\src\Deployment\BizTalk.Factory.Deployment.btdfproj -Targets Installer -Configuration Debug
Invoke-MSBuild -Project .\src\Deployment\BizTalk.Factory.Deployment.btdfproj -Targets Installer -Configuration Release

# Now we copy the interesting build outputs to our dedicated directory structure

if (Test-Path -Path .exports) { Remove-Item -Path .exports -Confirm:$false -Force -Recurse }

# SQL Scripts
New-Item -Path . -Name .exports\data\scripts -ItemType Directory -Force | Out-Null
Copy-Item data\scripts\*.* .exports\data\scripts -Force -PassThru | % { $_.Name }

#NuGet package
Move-Item BizTalk.Factory.$packageVersion.nupkg .exports\BizTalk.Factory.$packageVersion.nupkg -Force -PassThru | % { $_.Name }

# Debug Assemblies
New-Item -Path . -Name .exports\lib\debug -ItemType Directory -Force | Out-Null
Copy-Item src\BizTalk.Common\Tracking\ActivityModel\ActivityModel.xml .exports\lib\debug -Force -PassThru | % { $_.Name }
Get-ChildItem -Path src -Directory -Exclude '.*','*.Tests' `
    | % { Get-ChildItem -Path $_\bin\debug -Filter "*$($_.Name)*" -Include '*.dll','*.pdb', '*.xml' -Exclude '*.ClaimStore.*' -Recurse -ErrorAction Ignore } `
    | Copy-Item -Destination .exports\lib\debug -Force -PassThru | % { $_.Name }
Remove-Item .exports\lib\debug\Be.Stateless.BizTalk.Pipeline.Definitions.* -Force
Copy-Item src\.imports\Be.Stateless.BizTalk.targets .exports\lib\debug -Force -PassThru | % { $_.Name }
Copy-Item src\.imports\Be.Stateless.Dsl.targets .exports\lib\debug -Force -PassThru | % { $_.Name }
Copy-Item src\BizTalk.ClaimStore.Agent\bin\Debug\*.* .exports\lib\debug -Include '*.Agent.exe','*.Agent.pdb','*.template.config' -Force -PassThru | % { $_.Name }
Copy-Item src\Deployment\bin\Debug\Installer\BizTalk.Factory\BizTalk.Factory-1.0.0.msi .exports\BizTalk.Factory.$packageVersion-Debug.msi -Force -PassThru | % { $_.Name }

# Release Assemblies
New-Item -Path . -Name .exports\lib\release -ItemType Directory -Force | Out-Null
Copy-Item src\BizTalk.Common\Tracking\ActivityModel\ActivityModel.xml .exports\lib\release -Force -PassThru | % { $_.Name }
Get-ChildItem -Path src -Directory -Exclude '.*','*.Tests' `
    | % { Get-ChildItem -Path $_\bin\release -Filter "*$($_.Name)*" -Include '*.dll','*.pdb', '*.xml' -Exclude '*.ClaimStore.*' -Recurse -ErrorAction Ignore } `
    | Copy-Item -Destination .exports\lib\release -Force -PassThru | % { $_.Name }
Remove-Item .exports\lib\release\Be.Stateless.BizTalk.Pipeline.Definitions.* -Force
Copy-Item src\.imports\Be.Stateless.BizTalk.targets .exports\lib\release -Force -PassThru | % { $_.Name }
Copy-Item src\.imports\Be.Stateless.Dsl.targets .exports\lib\release -Force -PassThru | % { $_.Name }
Copy-Item src\BizTalk.ClaimStore.Agent\bin\release\*.* .exports\lib\release -Include '*.Agent.exe','*.Agent.pdb','*.template.config' -Force -PassThru | % { $_.Name }
Copy-Item src\Deployment\bin\Release\Installer\BizTalk.Factory\BizTalk.Factory-1.0.0.msi .exports\BizTalk.Factory.$packageVersion-Release.msi -Force -PassThru | % { $_.Name }

# BizTalk.Web.Monitoring.Site
New-Item -Path . -Name .exports\BizTalk.Web.Monitoring.Site -ItemType Directory -Force | Out-Null
Copy-Item src\BizTalk.Web.Monitoring.Site\*.* -Destination .exports\BizTalk.Web.Monitoring.Site -Include 'Global.asax', 'Web.config' -Force -PassThru | % { $_.Name }
New-Item -Path . -Name .exports\BizTalk.Web.Monitoring.Site\bin -ItemType Directory -Force | Out-Null
Copy-Item src\BizTalk.Web.Monitoring.Site\bin\*.* -Destination .exports\BizTalk.Web.Monitoring.Site\bin -Exclude '*.xml' -Force -PassThru | % { $_.Name }
Copy-Item .exports\lib\release\Be.Stateless.BizTalk.Common.* -Destination .exports\BizTalk.Web.Monitoring.Site\bin -Exclude '*.xml' -Force -PassThru | % { $_.Name }
Copy-Item .exports\lib\release\Be.Stateless.BizTalk.Schemas.* -Destination .exports\BizTalk.Web.Monitoring.Site\bin -Exclude '*.xml' -Force -PassThru | % { $_.Name }
Copy-Item .exports\lib\release\Be.Stateless.Logging.* -Destination .exports\BizTalk.Web.Monitoring.Site\bin -Exclude '*.xml' -Force -PassThru | % { $_.Name }
Copy-Item src\BizTalk.Web.Monitoring.Site\_bin_deployableAssemblies\*.* -Destination .exports\BizTalk.Web.Monitoring.Site\bin -Exclude '*.xml' -Force -PassThru | % { $_.Name }
Copy-Item src\BizTalk.Web.Monitoring.Site\Config -Destination .exports\BizTalk.Web.Monitoring.Site -Recurse -Force -PassThru | % { Resolve-Path $_ -Relative }
Copy-Item src\BizTalk.Web.Monitoring.Site\Content -Destination .exports\BizTalk.Web.Monitoring.Site -Recurse -Force -PassThru | % { Resolve-Path $_ -Relative }
New-Item -Path . -Name .exports\BizTalk.Web.Monitoring.Site\Scripts -ItemType Directory -Force | Out-Null
Copy-Item src\BizTalk.Web.Monitoring.Site\Scripts\*.* -Destination .exports\BizTalk.Web.Monitoring.Site\Scripts -Include 'be.stateless.*', '*.contextMenu.js', 'modernizr*.*', '*.min.js' -Force -PassThru | % { $_.Name }
Copy-Item src\BizTalk.Web.Monitoring.Site\Views -Destination .exports\BizTalk.Web.Monitoring.Site -Recurse -Force -PassThru | % { Resolve-Path $_ -Relative }

# Utilities
New-Item -Path . -Name .exports\src\Deployment -ItemType Directory -Force | Out-Null
Copy-Item src\Deployment\BizTalk.Factory.ItemGroups.proj .exports\src\Deployment -Force -PassThru | % { $_.Name }

New-Item -Path . -Name '.exports\utils\Decode Bindings' -ItemType Directory -Force | Out-Null
Copy-Item 'utils\Decode Bindings\bindings-cleaner.xslt' '.exports\utils\Decode Bindings' -Force -PassThru | % { $_.Name }

New-Item -Path . -Name '.exports\utils\Deployment Tools' -ItemType Directory -Force | Out-Null
Copy-Item 'utils\Deployment Tools\adapterXPaths.txt' '.exports\utils\Deployment Tools' -Force -PassThru | % { $_.Name }
Copy-Item src\Deployment\BizTalk.Factory.Deployment.targets '.exports\utils\Deployment Tools' -Force -PassThru | % { $_.Name }
