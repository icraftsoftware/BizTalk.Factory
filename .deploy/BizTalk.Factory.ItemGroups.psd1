<#
.SYNOPSIS
      .
.DESCRIPTION
      .
.PARAMETER Configuration
      The build configuration under which an assembly has been output when SourceEnvironment is either DEV or BLD, either Debug or Release.
.PARAMETER IncludeTestArtifacts
      .
.EXAMPLE
      C:\PS>
      <Description of example>
.NOTES
      Author: François Chabot
      © 2017 be.stateless.
#>
param(
   [Parameter(Position = 0, Mandatory = $false)]
   [ValidateSet('Debug', 'Release', 'Package')]
   [string]
   $Configuration = 'Debug',

   [Parameter(Mandatory = $false)]
   [switch]
   $IncludeTestArtifacts
)
# path resolution is always relative to this file's folder
@{
   Assemblies         = @(
      @{ Path = Get-ChildItem -Path ..\src\packages\SharpZipLib.0.86.0\lib\20\ICSharpCode.SharpZipLib.dll }
      @{ Path = Get-ChildItem -Path ..\src\packages\log4net.2.0.8\lib\net40-full\log4net.dll }
      @{
         Path = Get-ProjectItem -Project 'BizTalk.Dsl', 'BizTalk.Dsl.Binding.Conventions', 'BizTalk.Explorer', 'Common', 'Extensions', 'Logging', 'ServiceModel' `
            -Include '*.dll' `
            -Configuration $Configuration
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
   Policies           = @(
      @{ Path = Get-ProjectItem -Project 'BizTalk.Policies' -Include '*.dll' -Configuration $Configuration }
   )
   Schemas            = @(
      @{ Path = Get-ProjectItem -Project 'BizTalk.Schemas' -Include '*.dll' -Configuration $Configuration }
   )
   Transforms         = @(
      @{ Path = Get-ProjectItem -Project 'BizTalk.Transforms' -Include '*.dll' -Configuration $Configuration }
   )
}
