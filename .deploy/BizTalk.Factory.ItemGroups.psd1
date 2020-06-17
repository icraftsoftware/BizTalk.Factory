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
@{
   Application        = @{
      Name        = 'BizTalk.Factory'
      Description = 'Comprehensive Microsoft BizTalk Server Runtime Powered by Be.Stateless.BizTalk.Factory.'
      # References  = @('App1', 'App2')
   }
   Assemblies         = @(
      @{ Path = Get-Item -Path ..\src\packages\SharpZipLib.0.86.0\lib\20\ICSharpCode.SharpZipLib.dll }
      @{ Path = '..\src\packages\log4net.2.0.8\lib\net40-full\log4net.dll' }
      @{ Path = Get-ProjectItem `
            -Project 'BizTalk.Dsl', 'BizTalk.Dsl.Binding.Conventions', 'BizTalk.Explorer', 'Common', 'Extensions', 'Logging', 'ServiceModel' `
            -Include '*.dll' `
            -Configuration $Configuration
      }
   )
   Bindings           = @(
      @{
         Path = Get-ProjectItem -Project 'BizTalk.Binding' -Include '*.dll' -Configuration $Configuration
         # EnvironmentSettingOverridesRootPath = ''
         # AssemblyProbingPaths                = @('path', 'path')
      }
   )
   Components         = @(
      @{ Path = Get-ProjectItem -Project 'BizTalk.Common' -Include '*.dll' -Configuration $Configuration }
   )
   Orchestrations     = @(
      @{
         Path      = Get-ProjectItem -Project 'BizTalk.TestArtifacts' -Include '*.dll' -Configuration $Configuration
         Condition = $IncludeTestArtifacts
      }
   )
   PipelineComponents = @(
      @{ Path = Get-ProjectItem -Project 'BizTalk.Pipeline.Components' -Include '*.dll' -Configuration $Configuration }
   )
   Pipelines          = @(
      @{ Path = Get-ProjectItem -Project 'BizTalk.Pipelines' -Include '*.dll' -Configuration $Configuration }
   )
   Schemas            = @(
      @{ Path = Get-Item -Path '..\lib\BizTalk 2013 R2\Microsoft.Adapters.SAP.BiztalkPropertySchema.dll' }
      @{ Path = Get-ProjectItem -Project 'BizTalk.Schemas' -Include '*.dll' -Configuration $Configuration }
   )
   Transforms         = @(
      @{ Path = Get-ProjectItem -Project 'BizTalk.Transforms' -Include '*.dll' -Configuration $Configuration }
   )
   BamActivityModels  = @(
      @{ Path = Get-Item -Path ..\src\BizTalk.Common\Tracking\ActivityModel\ActivityModel.xml }
   )
   BamIndexes         = @(
      @{ Name = @('BeginTime', 'InterchangeID', 'ProcessName', 'Value1', 'Value2', 'Value3') ; Activity = 'Process' }
      @{ Name = @('MessagingStepActivityID', 'ProcessActivityID') ; Activity = 'ProcessMessagingStep' }
      @{ Name = @('InterchangeID', 'Time', 'Value1', 'Value2', 'Value3') ; Activity = 'MessagingStep' }
   )
}
