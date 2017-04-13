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

Set-StrictMode -Version Latest

<#
.SYNOPSIS
    Removes the bin and obj subfolders from a Visual Studio project, as well as all the files produced by the BizTalk compiler.
.DESCRIPTION
    The obj subfolder will always be cleaned, while the bin subfolder will only be cleaned iff the Visual Studio project is not a website project (i.e. if there is no web.config file in the given folder).
    The command will always try to clear the *.btm.cs, *.btp.cs, and *.xsd.cs files that are produced by the BizTalk compiler.
.PARAMETER Path
    The path to the Visual Studio project.
.EXAMPLE
    Get-ChildItem -Directory | Clear-Project
.EXAMPLE
    Clear-Project -Recurse

    The -Recurse switch is shorthand for the following compound command: Get-ChildItem -Directory | Clear-Project.
.EXAMPLE
    Clear-Project .\BizTalk.Dsl, .\BizTalk.Dsl.Tests
.EXAMPLE
    (gi .\BizTalk.Dsl), (gi .\BizTalk.Dsl.Tests) | Clear-Project -WhatIf
.NOTES
    © 2017 be.stateless.
#>
function Clear-Project
{
    [CmdletBinding(DefaultParametersetName='Single',SupportsShouldProcess=$true)]
    Param(
        [Parameter(Mandatory=$false,ValueFromPipeline=$true,ValueFromPipelinebyPropertyName=$true)]
        [psobject[]]
        $Path,

        [switch]
        $Packages,

        [switch]
        $Recurse

        #TODO switch to also clean .user, .dotsettings.user, etc... files
    )

    begin {
        if ($Path -eq $null) {
            $Path = Get-Item -Path .
        }
        if ($Recurse) {
            $projectPaths = Get-ChildItem -Path $Path -Directory
        }
        else {
            $projectPaths = $Path
        }
    }
    process {
        foreach ($p in $projectPaths) {
            $p = Resolve-Path -Path $p.FullName -Relative
            Write-Verbose "Clearing $p..."
            if (-not(Test-Path -Path $p\web.config) -and (Test-Path -LiteralPath $p\bin)) {
                Remove-Item -LiteralPath $p\bin -Confirm:$false -Force -Recurse
            }
            if (Test-Path -LiteralPath $p\obj) {
                Remove-Item -LiteralPath $p\obj -Confirm:$false -Force -Recurse
            }
            Get-ChildItem -Path $p -Filter *.btm.cs -Recurse | Remove-Item -Confirm:$false
            Get-ChildItem -Path $p -Filter *.btp.cs -Recurse | Remove-Item -Confirm:$false
            Get-ChildItem -Path $p -Filter *.xsd.cs -Recurse | Remove-Item -Confirm:$false
        }
        if ($Packages) {
            $packagesPath = Join-Path -Path $Path .\packages
            if (Test-Path -Path $packagesPath) {
                $packagesPath = Resolve-Path -Path $packagesPath -Relative
                Write-Verbose "Cleaning NuGet packages under $packagesPath..."
                Get-ChildItem -Path $packagesPath -Directory | Remove-Item -Recurse -Force -Confirm:$false
            }
        }
    }
    #end { }
}

<#
.SYNOPSIS
    Probe an MSBuild project file and list either all of its supported targets or only the ones matching a given filter.
.DESCRIPTION
    Probe an MSBuild project file and list all the supported targets that can be called.

    This function does not work with Visual Studio solution files.
.PARAMETER Project
    Project file to probe.
.EXAMPLE
    Get-MSBuildTargets BizTalk.Factory.Deployment.btdfproj
.EXAMPLE
    Get-MSBuildTargets BizTalk.Factory.Deployment.btdfproj binding
    Get-MSBuildTargets -Project BizTalk.Factory.Deployment.btdfproj -Filter binding

    Both commands are equivalent and list the supported targets of the BizTalk.Factory.Deployment.btdfproj project file that matches the 'binding' string.
.LINK
    http://stackoverflow.com/questions/441614/how-to-query-msbuild-file-for-list-of-supported-targets
.NOTES
    © 2015 be.stateless.
#>
function Get-MSBuildTargets
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=$true)]
        [string]
        $Project,

        [Parameter(Position=1,Mandatory=$false)]
        [string]
        $Filter = '.'
    )

    try {
        $msbuildProject = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.LoadProject((Resolve-Path $Project))
        # suggest targerts using a stricter matching criterium first
        $msbuildProject.Targets.Keys | Where-Object { $_ -match "^$Filter" } | Sort-Object
        # suggest targerts using a looser matching criterium next
        $msbuildProject.Targets.Keys | Where-Object { $_ -match "^.+$Filter" } | Sort-Object
    }
    finally {
        [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.UnloadAllProjects()
    }
}

<#
.SYNOPSIS
    Invokes the required/matching version of MSBuild.exe to build a project or solution file.
.DESCRIPTION
    Builds an MSBuild project or solution file by invoking the right version, as specified in the project or solution file, of MSBuild.exe.
    The command is able to dynamically switch between different version of the MSBuild.exe tool without corrupting the environment variables and therefore without requiring PowerShell from being restarted.
.PARAMETER Project
    Project or solution file to build.
.PARAMETER UnboundArguments
    This parameter accumulates all the additional arguments and pass them along to MSBuild.
.EXAMPLE
    PS> Invoke-MSBuild BizTalk.Factory.sln

    Dynamically sets up, if needed, the Visual Studio environment necessary to build the BizTalk.Factory.sln solution file and builds the solution.
.COMPONENT
    This command relies on the Pscx Get-EnvironmentBlock, Pop-EnvironmentBlock, and Push-EnvironmentBlock functions.
.NOTES
    © 2015 be.stateless.
#>
function Invoke-MSBuild
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Position=0,Mandatory=$true)]
        [string]
        $Project,

        [Parameter(Position=1,Mandatory=$false)]
        [string[]]
        $Targets = '',

        [Parameter(Mandatory=$false)]
        [string]
        $VisualStudioVersion = $null,

        [Parameter(Mandatory=$false)]
        [string]
        $ToolsVersion = $null,

        [Parameter(Mandatory=$false)]
        [int[]]
        $NoWarn,

        [Parameter(Mandatory=$false)]
        [ValidateSet('Quiet', 'Minimal', 'Normal', 'Detailed', 'Diagnostic')]
        [string]
        $Verbosity,

        [Parameter(DontShow,ValueFromRemainingArguments=$true)]
        [object[]]
        $UnboundArguments = @()
    )
    # see Get-Help about_Functions_Advanced_Parameters
    # TODO http://poshoholic.com/2007/11/28/powershell-deep-dive-discovering-dynamic-parameters/
    DynamicParam {
        $parameterDictionary = New-Object -Type System.Management.Automation.RuntimeDefinedParameterDictionary
        # only for .*proj project files, i.e. not for .sln files
        if ($Project -match '.*\.\w*proj$') {
            Probe-MSBuildProperties $Project | New-DynamicParameter | ForEach-Object { $parameterDictionary.Add($_.Name, $_) }
        }
        $parameterDictionary
    }

    begin {
        $properties = @( $parameterDictionary.Keys |
            Where-Object { $parameterDictionary.$_.IsSet } |
            ForEach-Object { $parameterDictionary.$_ } |
            Select-Object -Property Name, Value )
    }
    process {
        $arguments = @{ Project = $Project }
        if ($PSBoundParameters.ContainsKey('Targets')) {
            $arguments.Targets = $Targets
        }
        if ($PSBoundParameters.ContainsKey('VisualStudioVersion')) {
            $arguments.VisualStudioVersion = $VisualStudioVersion
        }
        if ($PSBoundParameters.ContainsKey('ToolsVersion')) {
            $arguments.ToolsVersion = $ToolsVersion
        }
        if ($PSBoundParameters.ContainsKey('NoWarn')) {
            $arguments.NoWarn = $NoWarn
        }
        if ($PSBoundParameters.ContainsKey('Verbosity')) {
            $arguments.Verbosity = $Verbosity
        }
        $arguments.UnboundArguments = $UnboundArguments + @($properties | ForEach-Object { @("-$($_.Name)", $_.Value)})
        # see Get-Help about_Splatting
        Invoke-MSBuildCore @arguments `
            -Verbose:($PSBoundParameters['Verbose'] -eq $true) `
            -WhatIf:($PSBoundParameters['WhatIf'] -eq $true)
    }
}

# To invoke this 'private' function from another module, see either of the following references
# http://powershell.com/cs/blogs/tips/archive/2009/09/18/accessing-hidden-module-members.aspx
# http://stackoverflow.com/questions/9382362/view-nested-private-function-definitions-in-powershell
function Invoke-MSBuildCore
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Mandatory=$true)]
        [string]
        $Project,

        [Parameter(Mandatory=$false)]
        [string[]]
        $Targets = $null,

        [Parameter(Mandatory=$false)]
        [string]
        $VisualStudioVersion = '*',

        [Parameter(Mandatory=$false)]
        [string]
        $ToolsVersion = '*',

        [Parameter(Mandatory=$false)]
        [int[]]
        $NoWarn,

        [Parameter(Mandatory=$false)]
        [ValidateSet('Quiet', 'Minimal', 'Normal', 'Detailed', 'Diagnostic')]
        [string]
        $Verbosity,

        [Parameter(Mandatory=$false)]
        [string]
        $Action = 'Invoking MSBuild.exe',

        [Parameter(Mandatory=$false)]
        [switch]
        $Elevated,

        [Parameter(ValueFromRemainingArguments=$true)]
        [object[]]
        $UnboundArguments
    )

    # dump bound and unbound parameters
    #$PSBoundParameters.GetEnumerator() | ForEach-Object { Write-Verbose "Bound Parameter: $_" }
    #$UnboundArguments.GetEnumerator() | ForEach-Object { Write-Verbose "Unbound Parameter: $_" }

    $rvsv = Resolve-VisualStudioVersion $Project $VisualStudioVersion $ToolsVersion
    $msbuildArgs = ''
    if ($PSBoundParameters.ContainsKey('Targets')) {
        $msbuildArgs += " /target:$(Join-String -Strings $Targets -Separator ';')"
    }
    if ($rvsv.Contains('ToolsVersion')) {
        $msbuildArgs += " /toolsversion:$($rvsv.ToolsVersion)"
    }
    if ($PSBoundParameters.ContainsKey('Verbosity')) {
        $msbuildArgs += " /verbosity:$($Verbosity.ToLower())"
    }
    if ($PSBoundParameters.ContainsKey('NoWarn')) {
        $msbuildArgs += " /property:NoWarn=`"$(Join-String -Strings $NoWarn -Separator ',')`""
    }

    # parse unbound arguments into $parsing structure
    $parsing = @{ Parameters = @() ; Verbatim = $null }
    $i = 0
    :parsing while ($UnboundArguments -and $i -lt $UnboundArguments.Length) {
        switch -regex ($UnboundArguments[$i])
        {
            # parse name of brand new parameter
            '^-(\w+)$' {
                $parsing.Parameters += @{ Name = $matches[1] ; Values = @() }
                break
            }
            # parse verbatim command line tail and stop parsing
            '^--%$' {
                $parsing.Verbatim = $UnboundArguments[++$i]
                break parsing
            }
            # parse values of last parsed parameter
            default {
                $parsing.Parameters[$parsing.Parameters.Length - 1].Values += $UnboundArguments[$i]
                break
            }
        }
        $i++
    }

    # pass every $parsing.Parameters to MSBuild as /property:<name>=<value> ...
    $parsing.Parameters | Where-Object { $_.Values.Length -gt 0 } | ForEach-Object {
        $msbuildArgs += " /property:$($_.Name)=`"$(Join-String -Strings $_.Values -Separator ',')`""
    }
    # ... or as /property:<name>=true to MSBuild
    $parsing.Parameters | Where-Object { $_.Values.Length -eq 0 } | ForEach-Object {
        $msbuildArgs += " /property:$($_.Name)=$true"
    }
    # pass $parsing.Verbatim thru to MSBuild
    if ($parsing.Verbatim -ne $null) {
        $msbuildArgs += " $($parsing.Verbatim)"
    }

    if (-not($PSBoundParameters['Verbose'] -eq $true)) {
        if ($msbuildArgs) {
            Write-Host "$Action on $($rvsv.ProjectFile) with$msbuildArgs"
        } else {
            Write-Host "$Action on $($rvsv.ProjectFile)"
        }
    }
    # --% see STOP PARSING subtopic in Get-Help about_Parsing or STOP-PARSING SYMBOL subtopic in Get-Help about_Escape_Characters
    $command = "MSBuild.exe $($rvsv.ProjectFile) --%$msbuildArgs"

    if ($PsCmdlet.ShouldProcess($rvsv.ProjectFile, $Action)) {
        if (($PSBoundParameters['Elevated'] -eq $true)) {
            Assert-Elevated
        }
        if ($rvsv.Contains('VisualStudioVersion')) {
            Switch-VisualStudioEnvironment $rvsv.VisualStudioVersion
        }
        Assert-VisualStudioEnvironment $VisualStudioVersion
        Write-Verbose $command
        Invoke-Expression -Command $command
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    } else {
        Write-Verbose $command
    }
}

<#
.SYNOPSIS
    Returns the numerically sorted version numbers of all the locally installed Visual Studio versions.
.DESCRIPTION
    This command will returns the version numbers of all the locally installed Visual Studio versions sorted either ascendingly or descendingly. Notice that only those versions for which the common tools are deployed and configured will be returned.
.EXAMPLE
    PS> Get-VisualStudioVersionNumbers

    Returns the version numbers of the installed Visual Studio in ascending numerical order.
.EXAMPLE
    PS> Get-VisualStudioVersionNumbers -Descending

    Returns the version numbers of the installed Visual Studio in descending numerical order.
.NOTES
    © 2014 be.stateless.
#>
function Get-VisualStudioVersionNumbers
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0)]
        [switch]
        $Descending
    )
    # http://www.mztools.com/articles/2008/MZ2008003.aspx, but is way too slow
    #Get-ChildItem -Path HKLM:\SOFTWARE\Classes\VisualStudio.DTE.* -Name |
    #    Where-Object { $_ -match '^VisualStudio.DTE.(\d+\.\d+)$' } |
    #    ForEach-Object { $matches[1] } |
    #    Sort-Object { [float]$_ }

    # http://social.technet.microsoft.com/Forums/windowsserver/en-US/9620af9a-0323-460c-b3e8-68a73715f99d/module-scoped-variable
    # cache to avoid looking for registry keys again and again
    $installedVisualStudioVersionNumbers = $MyInvocation.MyCommand.Module.PrivateData['InstalledVisualStudioVersionNumbers']

    if ($installedVisualStudioVersionNumbers -eq $null) {
        $path = ?: { Test-64bitArchitecture } { 'HKLM:\SOFTWARE\Wow6432Node\Microsoft\VisualStudio' } { 'HKLM:\SOFTWARE\Microsoft\VisualStudio' }
        if (Test-Path $path) {
            $installedVisualStudioVersionNumbers = Get-ChildItem -Path $path |
                Where-Object { $_.GetValue('InstallDir') } |
                Select-Object -ExpandProperty PSChildName |
                Where-Object { $_ -match '^\d+\.\d+$' } |
                Where-Object { Test-Path -Path Env:\"$(VisualStudioCommonToolsEnvironmentVariableFromVersionNumber $_)" }
        }
        $installedVisualStudioVersionNumbers = @($installedVisualStudioVersionNumbers)
        $MyInvocation.MyCommand.Module.PrivateData['InstalledVisualStudioVersionNumbers'] = $installedVisualStudioVersionNumbers
    }
    $installedVisualStudioVersionNumbers | Sort-Object { [float]$_ } -Descending:$Descending
}

<#
.SYNOPSIS
    Ensures that an EnvironmentBlock has been setup to operate the tools accompanying a given version of Visual Studio.
.DESCRIPTION
    This command will throw if no EnvironmentBlock has been setup to operate the tools accompanying a given version of Visual Studio and will silently complete otherwise.
.EXAMPLE
    PS> Assert-VisualStudioEnvironment
.EXAMPLE
    PS> Assert-VisualStudioEnvironment 2010

    Asserts that the environment has been specifically setup for Visual Studio 2010.
.EXAMPLE
    PS> Assert-VisualStudioEnvironment *

    Asserts that the environment has been setup for some version of Visual Studio.
.EXAMPLE
    PS> Assert-VisualStudioEnvironment -Verbose
    With the -Verbose switch, this command will confirm this process is 32 bit.
.NOTES
    © 2012 be.stateless.
#>
function Assert-VisualStudioEnvironment
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0)]
        [string]
        $Version = '*'
    )

    if (-not(Test-VisualStudioEnvironment $Version)) {
        if ($Version -eq '*') {
            throw 'Environment has not been setup for any specific version of Visual Studio! Use the Switch-VisualStudioEnvironment command to setup one.'
        }
        throw "Environment has not been setup for Visual Studio $Version!"
    }
    $currentEnvironment = Get-VisualStudioEnvironment | Select-Object -First 1
    Write-Verbose "Environment has already been setup for Visual Studio $($currentEnvironment.Version)."
}

<#
.SYNOPSIS
    Returns an anonymous object describing the EnvironmentBlock that has been set up to operate the tools accompanying a given version of Visual Studio.
.DESCRIPTION
    This command will returns either $null or an anonymous object whose EnvironmentBlock field references the topmost Pscx.EnvironmentBlock.EnvironmentFrame that has been set up, and whose Version field denotes the version of Visual Studio for which the environment has been set up.
.EXAMPLE
    PS> Get-VisualStudioEnvironment

    Returns the topmost EnvironmentBlock descriptor that has been set up for a given version of Visual Studio.
.EXAMPLE
    PS> Get-VisualStudioEnvironment | Select-Object -First 1

    Returns the latest and topmost EnvironmentBlock descriptor that has been set up for a given version of Visual Studio.
.EXAMPLE
    PS> Get-VisualStudioEnvironment | Select-Object -Last 1

    Returns the earliest and bottommost EnvironmentBlock descriptor that has been set up for a given version of Visual Studio.
.COMPONENT
    This command relies on the Pscx Get-EnvironmentBlock function.
.NOTES
    © 2013 be.stateless.
#>
function Get-VisualStudioEnvironment
{
    [CmdletBinding()]
    param()
    $currentEnvironment = Get-EnvironmentBlock | Where-Object -Property Description -Match 'VisualStudioVersion=\d{4}'
    if ($currentEnvironment -ne $null) {
        $currentEnvironment | ForEach-Object {
            $matches = ($_.Description | Select-String -Pattern 'VisualStudioVersion=(?<Version>\d{4})').Matches
            @{ EnvironmentBlock = $_ ; Version = $matches[0].Groups['Version'].Value }
        }
    }
}

<#
.SYNOPSIS
    Clears the environment that has been set up for a version of Visual Studio and all EnvironmentBlocks that have been pushed afterwards.
.DESCRIPTION
    Unless specified otherwise, this command will clear the latest and topmost environment that has been set up for a version of Visual Studio and all the other EnvironmentBlocks, i.e. Pscx.EnvironmentBlock.EnvironmentFrame, that have been pushed afterwards.
.PARAMETER Environment
    The anonymous object whose EnvironmentBlock field references the Pscx.EnvironmentBlock.EnvironmentFrame up to which to clear.
.EXAMPLE
    PS> Clear-VisualStudioEnvironment

    Clears all the EnvironmentBlocks that have been set up after and up to the latest Pscx.EnvironmentBlock.EnvironmentFrame that has been setup for a given version of Visual Studio.
.EXAMPLE
    PS> Clear-VisualStudioEnvironment -WhatIf

    Describes all the EnvironmentBlocks that have been set up and would be cleared.
.EXAMPLE
    PS> Get-VisualStudioEnvironment | Select-Object -Last 1 | Clear-VisualStudioEnvironment

    Clears all the EnvironmentBlocks that have been set up after and up to the first Pscx.EnvironmentBlock.EnvironmentFrame that has been setup for a given version of Visual Studio.
.EXAMPLE
    PS> Clear-VisualStudioEnvironment (Get-VisualStudioEnvironment)[3]

    Clears all the EnvironmentBlocks that have been set up after and up to the given Pscx.EnvironmentBlock.EnvironmentFrame that has been setup for a given version of Visual Studio.
.COMPONENT
    This command relies on the Pscx Get-EnvironmentBlock, Pop-EnvironmentBlock, and Push-EnvironmentBlock functions.
.NOTES
    © 2013 be.stateless.
#>
function Clear-VisualStudioEnvironment
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Position=0,Mandatory=$false,ValueFromPipeline=$true)]
        [object]
        $Environment = $(Get-VisualStudioEnvironment | Select-Object -First 1)
    )
    if ($Environment -ne $null) {
        # fetch Visual Studio's EnvironmentBlock and all EnvironmentBlocks pushed afterwards
        $deprecatedEnvironmentBlocks = Get-EnvironmentBlock | Where-Object Timestamp -ge $Environment.EnvironmentBlock.Timestamp

        Write-Verbose "Clearing former Visual Studio $($Environment.Version) environment..."
        # pop Visual Studio's EnvironmentBlock and all EnvironmentBlocks pushed afterwards
        $deprecatedEnvironmentBlocks | ForEach-Object {
            if ($PsCmdlet.ShouldProcess("Environment variables", "Clearing EnvironmentBlock $($_.Description)")) {
                Pop-EnvironmentBlock
            }
        }
    }
    else {
        Write-Verbose "There is no former Visual Studio environment to clear."
    }
}

<#
.SYNOPSIS
    Sets up the enviroment necessary to operate the tools accompanying a given version of Visual Studio.
.DESCRIPTION
    Sets up the enviroment necessary to operate the tools accompanying a given version of Visual Studio. It moreover clears any environment that had previously been setup for another version of Visual Studio.
.PARAMETER Version
    The version of Visual Studio for which to setup the environment.
.EXAMPLE
    PS> Switch-VisualStudioEnvironment 2008

    Sets up the enviroment necessary to operate Visual Studio 2008 and its accompanying tools.
.EXAMPLE
    PS> Switch-VisualStudioEnvironment 2010

    Sets up the enviroment necessary to operate Visual Studio 2010 and its accompanying tools.
.EXAMPLE
    PS> Switch-VisualStudioEnvironment 2010 -WhatIf

    Describes the clearing and setup steps that would be necessary to setup the enviroment necessary to operate Visual Studio 2010 and its accompanying tools.
.COMPONENT
    This command relies on the Pscx Get-EnvironmentBlock, Pop-EnvironmentBlock, and Push-EnvironmentBlock functions.
.NOTES
    © 2013 be.stateless.
#>
function Switch-VisualStudioEnvironment
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Position=0,Mandatory=$true)]
        [string]
        $Version
    )
    $currentEnvironment = Get-VisualStudioEnvironment | Select-Object -First 1
    if ($currentEnvironment -ne $null -and $currentEnvironment.Version -ne $Version) {
        Get-VisualStudioEnvironment | Select-Object -Last 1 | Clear-VisualStudioEnvironment
    }
    if ($currentEnvironment -eq $null -or $currentEnvironment.Version -ne $Version) {
        Write-Verbose "Setting up environment for Visual Studio $Version..."
        $versionNumber = Get-VisualStudioVersionNumbers | Where-Object { (Translate-VisualStudioVersionNumber $_) -eq $Version }
        if ($versionNumber -eq $null) {
            throw "Version $Version of Visual Studio is not supported (yet)!"
        }

        $path = Get-Item -Path Env:\"$(VisualStudioCommonToolsEnvironmentVariableFromVersionNumber $versionNumber)"
        $batchPath = [System.IO.Path]::GetFullPath("$($path.Value)..\..\VC\vcvarsall.bat")
        if ($PsCmdlet.ShouldProcess("Environment variables", "Pushing EnvironmentBlock for Visual Studio $Version")) {
            Push-EnvironmentBlock -Description "VisualStudioVersion=$Version"
            Invoke-BatchFile $batchPath
        }
    }
    else {
        Write-Verbose "Environment has already been setup for Visual Studio $Version."
    }
}

<#
.SYNOPSIS
    Returns whether an EnvironmentBlock has been setup to operate the tools accompanying a given version of Visual Studio.
.DESCRIPTION
    This command will return $true if an EnvironmentBlock has been setup to operate the tools accompanying a given version of Visual Studio, or $false otherwise.
.EXAMPLE
    PS> Test-VisualStudioEnvironment
.EXAMPLE
    PS> Test-VisualStudioEnvironment 2010

    Tests whether the environment has been setup specifically for Visual Studio 2010.
.EXAMPLE
    PS> Test-VisualStudioEnvironment *

    Tests whether the environment has been setup for some version of Visual Studio.
.NOTES
    © 2012 be.stateless.
#>
function Test-VisualStudioEnvironment
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0)]
        [string]
        $Version = '*'
    )

    $currentEnvironment = Get-VisualStudioEnvironment | Select-Object -First 1
    [bool]($currentEnvironment -ne $null -and ($Version -eq '*' -or $currentEnvironment.Version -eq $Version))
}

#region TabExpansion Overrides / Private Probing and Resolution Helper Functions

function Register-TabExpansions
{
    $global:options['CustomArgumentCompleters']['Project'] =  {
        param($commandName, $parameterName, $wordToComplete, $commandAst, $fakeBoundParameter)
        if ($commandName -eq 'Invoke-MSBuild')
        {
            Probe-ProjectFile $wordToComplete $true | ForEach-Object {
                New-Object System.Management.Automation.CompletionResult $_, $_, 'ParameterValue', $_
            }
        }
        elseif ($commandName -eq 'Get-MSBuildTargets')
        {
            Probe-ProjectFile $wordToComplete | ForEach-Object {
                New-Object System.Management.Automation.CompletionResult $_, $_, 'ParameterValue', $_
            }
        }
    }

    $global:options['CustomArgumentCompleters']['Targets'] =  {
        param($commandName, $parameterName, $wordToComplete, $commandAst, $fakeBoundParameter)
        if ($commandName -eq 'Invoke-MSBuild')
        {
            Get-MSBuildTargets $fakeBoundParameter.Project $wordToComplete | ForEach-Object {
                New-Object System.Management.Automation.CompletionResult $_, $_, 'ParameterValue', $_
            }
        }
    }

    $global:options['CustomArgumentCompleters']['ToolsVersion'] =  {
        param($commandName, $parameterName, $wordToComplete, $commandAst, $fakeBoundParameter)
        if ($commandName -eq 'Invoke-MSBuild')
        {
            Probe-ToolStudioVersions | ForEach-Object {
                New-Object System.Management.Automation.CompletionResult $_, $_, 'ParameterValue', $_
            }
        }
    }

    $global:options['CustomArgumentCompleters']['VisualStudioVersion'] =  {
        param($commandName, $parameterName, $wordToComplete, $commandAst, $fakeBoundParameter)
        if ($commandName -eq 'Invoke-MSBuild')
        {
            Probe-VisualStudioVersions $wordToComplete | ForEach-Object {
                New-Object System.Management.Automation.CompletionResult $_, $_, 'ParameterValue', $_
            }
        }
    }

    $global:options['CustomArgumentCompleters']['Version'] =  {
        param($commandName, $parameterName, $wordToComplete, $commandAst, $fakeBoundParameter)
        if ($commandName -eq 'Switch-VisualStudioEnvironment')
        {
            Probe-VisualStudioVersions $wordToComplete | ForEach-Object {
                New-Object System.Management.Automation.CompletionResult $_, $_, 'ParameterValue', $_
            }
        }
        elseif ($commandName -match '(Assert|Test)\-VisualStudioEnvironment')
        {
            @('*') + @(Probe-VisualStudioVersions $wordToComplete) | ForEach-Object {
                New-Object System.Management.Automation.CompletionResult $_, $_, 'ParameterValue', $_
            }
        }
    }
}

function Unregister-TabExpansions
{
    $global:options['CustomArgumentCompleters'].Remove('Project')
    $global:options['CustomArgumentCompleters'].Remove('Targets')
    $global:options['CustomArgumentCompleters'].Remove('ToolsVersion')
    $global:options['CustomArgumentCompleters'].Remove('VisualStudioVersion')
    $global:options['CustomArgumentCompleters'].Remove('Version')
}

function New-DynamicParameter
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true,Position=0,ValueFromPipeline=$true)]
        [string]
        $Name
    )
    process {
        $attributes = New-Object -Type System.Management.Automation.ParameterAttribute
        $attributes.ParameterSetName = '__AllParameterSets'
        $attributes.Mandatory = $false
        $attributeCollection = New-Object -Type System.Collections.ObjectModel.Collection[System.Attribute]
        $attributeCollection.Add($attributes)
        New-Object -Type System.Management.Automation.RuntimeDefinedParameter($Name, [string], $attributeCollection)
    }
}

function Probe-MSBuildProperties
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=$true)]
        [string]
        $Project,

        [Parameter(Position=1)]
        [string[]]
        $Exclude = @()
    )

    try {
        $msbuildProject = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.LoadProject((Resolve-Path $Project))
        $msbuildProject.ConditionedProperties.Keys | Where-Object { $Exclude -notcontains $_ }| Sort-Object
    }
    finally {
        [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.UnloadAllProjects()
    }
}

function Probe-ProjectFile([string]$pattern, [bool]$includeSolutionFiles = $false)
{
    if ($pattern) {
        $pattern = $pattern.Trim()
        $path = Split-Path $pattern -Parent
        if ($path.Length -lt 1) { $path = '.' }
        $file = Split-Path $pattern -Leaf
    } else {
        $path = '.'
        $file = $null
    }
    if ($includeSolutionFiles) {
        Get-ChildItem -Path "$path\*" -Include "$file*.sln", "$file*.*proj" -Name | % { "$path\$_" }
    } else {
        Get-ChildItem -Path "$path\*" -Include "$file*.*proj" -Name | % { "$path\$_" }
    }
}

function Probe-ToolStudioVersions([string]$pattern)
{
    Get-ChildItem HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions -Name |
        Where-Object { $_ -match '\d+\.\d+' } |
        Sort-Object { [float]$_ }
}

function Probe-VisualStudioVersions([string]$pattern)
{
    Get-VisualStudioVersionNumbers |
        Sort-Object { [double]$_ } |
        ForEach-Object { Translate-VisualStudioVersionNumber $_ } |
        Where-Object { if ($pattern -eq $null) { $true } else { $_ -match $pattern } }
}

function Resolve-ProjectFile([string]$project)
{
    if ($project) { $project = $project.Trim() }
    if (-not(Test-Path .\$project)) {
        throw "$project MSBuild project file not found!"
    }
    @{ ProjectFile = (Resolve-Path $project -Relative) }
}

function Resolve-VisualStudioVersion([string]$project, [string]$visualStudioVersion, [string]$toolsVersion)
{
    $rtv = Resolve-ProjectFile $project

    # determine Visual Studio Version
    if ($visualStudioVersion -and $visualStudioVersion -ne '*') {
        # get it from $visualStudioVersion argument
        $rtv.Add('VisualStudioVersion', $visualStudioVersion)
    }
    else {
        # first try probing $project file as if it were a .sln file
        $pattern = '\s*Microsoft Visual Studio Solution File, Format Version (?<FormatVersion>\d\d\.\d\d)\s*' `
            + '|\s*# Visual Studio (?<Version>\d\d\d\d)\s*' `
            + '|\s*VisualStudioVersion = (?<VersionNumber>\d+\.\d+)[\d.]+\s*' `
            + '|\s*MinimumVisualStudioVersion = (?<MinimumVersionNumber>\d+\.\d+)[\d.]+\s*'
        $matchInfo = Select-String -Pattern $pattern -Path $project
        if ($matchInfo -ne $null)  {
            $matches = @($matchInfo.Matches | Where-Object { $_.Success })
            # rely on a Visual Studio Version ?
            if ($matches.Length -gt 1) {
                $match = $matchInfo.Matches[1]
                $visualStudioVersion = $match.Groups['Version'].Value
                $visualStudioVersion = Probe-VisualStudioVersions | Where-Object { $_ -ge $visualStudioVersion } | Select-Object -First 1
                if ($visualStudioVersion -ne $null) {
                    $rtv.Add('VisualStudioVersion', $visualStudioVersion)
                }
            }
            # if not, rely on a Visual Studio Version Number ?
            if ($rtv.VisualStudioVersion -eq $null -and $matches.Length -gt 2) {
                $match = $matchInfo.Matches[2]
                $versionNumber = $match.Groups['VersionNumber'].Value
                $visualStudioVersion = Translate-VisualStudioVersionNumber ([string] $versionNumber)
                $visualStudioVersion = Probe-VisualStudioVersions | Where-Object { $_ -ge $visualStudioVersion } | Select-Object -First 1
                if ($visualStudioVersion -ne $null) {
                    $rtv.Add('VisualStudioVersion', $visualStudioVersion)
                }
            }
            # if not, rely on a minimum Visual Studio Version Number ?
            if ($rtv.VisualStudioVersion -eq $null -and $matches.Length -gt 3) {
                $match = $matchInfo.Matches[3]
                $minimumVersionNumber = $match.Groups['MinimumVersionNumber'].Value
                $visualStudioVersion = Translate-VisualStudioVersionNumber ([string] $minimumVersionNumber)
                $visualStudioVersion = Probe-VisualStudioVersions | Where-Object { $_ -ge $visualStudioVersion } | Select-Object -First 1
                if ($visualStudioVersion -ne $null) {
                    $rtv.Add('VisualStudioVersion', $visualStudioVersion)
                }
            }
            # if not, rely on a Visual Studio Solution File Format Version
            if ($rtv.VisualStudioVersion -eq $null -and $matches.Length -gt 0) {
                $match = $matchInfo.Matches[0]
                $formatVersion = $match.Groups['FormatVersion'].Value
                $visualStudioVersion = Translate-VisualStudioSolutionFileFormatVersion ([string] $formatVersion)
                $visualStudioVersion = Probe-VisualStudioVersions | Where-Object { $_ -ge $visualStudioVersion } | Select-Object -First 1
                if ($visualStudioVersion -ne $null) {
                    $rtv.Add('VisualStudioVersion', $visualStudioVersion)
                }
            }
        }
        else {
            # next try probing $project file as if it were an MSBuild .*proj file
            $matchInfo = Select-String -Pattern 'Project\s+(.*\s+)?ToolsVersion\s*\=\s*[''"](?<Version>\d+\.\d)[''"]' -Path $project
            if ($matchInfo -ne $null)  {
                $matches = $matchInfo.Matches
                if ($matches -ne $null -and $matches.Success) {
                    $probedVersion = $matches[0].Groups['Version'].Value
                    $visualStudioVersion = Convert-ToolsVersionToVisualStudioVersion ([string] $probedVersion)
                    $visualStudioVersion = Probe-VisualStudioVersions | Where-Object { $_ -ge $visualStudioVersion } | Select-Object -First 1
                    $rtv.Add('VisualStudioVersion', $visualStudioVersion)
                }
            }
        }

    }

    # determine MSBuild's ToolsVersion; only relevant if it has been explicitly given, will be determined by MSBuild otherwise
    if ($toolsVersion -and $toolsVersion -ne '*') {
        # get it from $toolsVersion argument
        $rtv.Add('ToolsVersion', $toolsVersion)
    }

    $rtv
}

function Translate-VisualStudioSolutionFileFormatVersion([string]$formatVersion)
{
    switch -Exact ($formatVersion) {
        '10.00' { '2008' }
        '11.00' { '2010' }
        '12.00' { '2012' }
        default { throw "Visual Studio Solution File Format Version $formatVersion is not supported." }
    }
}

function Translate-VisualStudioVersionNumber([string]$versionNumber)
{
    # http://msdn.microsoft.com/en-us/library/bb398195.aspx
    switch -Exact ($versionNumber) {
        '9.0'  { '2008' }
        '10.0' { '2010' }
        '11.0' { '2012' }
        '12.0' { '2013' }
        default { throw "Visual Studio Version Number $versionNumber is not supported." }
    }
}

function Convert-ToolsVersionToVisualStudioVersion([string]$version)
{
    # http://msdn.microsoft.com/en-us/library/bb383796.aspx
    switch -Exact ($version) {
        '2.0'  { '2005' }
        '3.0'  { '2008' }
        '3.5'  { '2008' }
        '4.0'  { '2010' }
        '4.5'  { '2012' }
        '12.0' { '2013' }
        default { throw "Tools Version $version is not supported and cannot be used to dertermine the version of Visual Studio to use." }
    }
}

function Get-VisualStudioCommonToolsEnvironmentVariableFromVersionNumber([string] $versionNumber)
{
    "VS$($versionNumber.Replace('.',''))COMNTOOLS"
}

#endregion

<#
 # Main
 #>

Register-TabExpansions

# register clean up handler should the module be removed from the session
$MyInvocation.MyCommand.ScriptBlock.Module.OnRemove = {
    Unregister-TabExpansions
}

New-Alias -Name build -Value Invoke-MSBuild

Export-ModuleMember -Alias * -Function *