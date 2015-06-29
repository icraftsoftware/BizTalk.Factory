#region Copyright & License

# Copyright © 2012 - 2015 François Chabot, Yves Dierick
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
    Build a BizTalk Server Application.
.DESCRIPTION
    This command will build the Visual Studio solution corresponding to the BizTalk Server Application whose name has been given. As usual, the default MSBuild script target will be executed, but it can be overriden through regular MSBuild command-line arguments.
.PARAMETER Application
    The name of BizTalk Server Application whose Visual Studio solution will be built. Notice that tab expansion will suggest all, and only, the names of the BizTalk Server Applications for which an <application-name>.sln file can be found in the current folder.
.PARAMETER UnboundArguments
    This parameter accumulates all the additional arguments and pass them along to MSBuild.
.EXAMPLE
    PS> Build-BizTalkApplication -Application TaxProcessing

    Build the TaxProcessing BizTalk Server Application by invoking the default MSBuild script target for the TaxProcessing.sln solution file.
.EXAMPLE
    PS> Build-BizTalkApplication -Application BizTalk.Factory /t:Rebuild

    Rebuild the BizTalk.Factory BizTalk Server Application by invoking the rebuild MSBuild script target for the BizTalk.Factory.sln solution file.
.COMPONENT
    This command relies on MSBuild.exe and MSBuild scripts.
.NOTES
    © 2012 - 2015 be.stateless.
#>
function Build-BizTalkApplication
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Position=0,Mandatory=$true)]
        [string]
        $Application,

        [Parameter(Mandatory=$false)]
        [int[]]
        $NoWarn,

        [Parameter(Mandatory=$false)]
        [ValidateSet('Quiet', 'Minimal', 'Normal', 'Detailed', 'Diagnostic')]
        [string]
        $Verbosity,

        [Parameter(DontShow,ValueFromRemainingArguments=$true)]
        [object[]]
        $UnboundArguments
    )
    $rsf = Resolve-BizTalkApplicationSolutionFile $Application
    $arguments = @{
        Project = $rsf.MsBuildArg ;
        Action = "Building $($rsf.Name) BizTalk Server Application solution"
    }
    if ($PSBoundParameters.ContainsKey('NoWarn')) {
        $arguments.NoWarn = $NoWarn
    }
    if ($PSBoundParameters.ContainsKey('Verbosity')) {
        $arguments.Verbosity = $Verbosity
    }
    if ($PSBoundParameters.ContainsKey('UnboundArguments')) {
        $arguments.UnboundArguments = $UnboundArguments
    }
    & ${Invoke-MSBuildCore-Delegate} @arguments `
        -Verbose:($PSBoundParameters['Verbose'] -eq $true) `
        -WhatIf:($PSBoundParameters['WhatIf'] -eq $true)
}

<#
.SYNOPSIS
    Deploy and configure a BizTalk Server Application according to the settings of a specific target environment.
.DESCRIPTION
    This command will run the default target, unless specified otherwise, of the BizTalk Deployment Framework MSBuild script for a given BizTalk Server application.
.PARAMETER Application
    The name of the BizTalk Server Application to deploy and configure. Notice that tab expansion will suggest all, and only, the names of the BizTalk Server Applications for which an <application-name>.Deployment.btdfproj file can be found in the current folder or in the .\Deployment subfolder (in this order).
.PARAMETER TargetEnvironment
    The target environment whose settings will be used to deploy and configure the BizTalk Server Application. Notice that tab expansion will suggest all, and only, the target environments defined in the setting file generator of the BizTalk Server Application to deploy. Specifically, it will look into the <application-name>.SettingsFileGenerator.xml file located either in the current folder, the .\EnvironmentSettings subfolder, or the .\Deployment\EnvironmentSettings subfolder (in this order).
.PARAMETER UnboundArguments
    This parameter accumulates all the additional arguments and pass them along to MSBuild.
.EXAMPLE
    PS> Install-BizTalkApplication -Application BizTalk.Factory

    Deploy and configure the BizTalk.Factory BizTalk Server Application according to the settings of the default (typically DEV) target environment defined in the setting file generator of this BizTalk Server Application.
.EXAMPLE
    PS> Install-BizTalkApplication -Application BizTalk.Factory -TargetEnvironment PRD

    Deploy and configure the BizTalk.Factory BizTalk Server Application according to the settings of the PRD target environment defined in the setting file generator of this BizTalk Server Application.
.COMPONENT
    This command relies on BizTalk Deployment Framework MSBuild scripts.
.LINK
    http://biztalkdeployment.codeplex.com/
.NOTES
    © 2012 - 2015 be.stateless.
#>
function Install-BizTalkApplication
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Position=0,Mandatory=$true)]
        [string]
        $Application,

        [Parameter(Position=1)]
        [string]
        $TargetEnvironment,

        [Parameter(Mandatory=$false)]
        [ValidateSet('Quiet', 'Minimal', 'Normal', 'Detailed', 'Diagnostic')]
        [string]
        $Verbosity,

        [Parameter(DontShow,ValueFromRemainingArguments=$true)]
        [object[]]
        $UnboundArguments = @()
    )
    DynamicParam {
        $rdf = Resolve-BizTalkApplicationDeploymentFile $Application
        $parameterDictionary = New-Object -Type System.Management.Automation.RuntimeDefinedParameterDictionary
        & ${Probe-MSBuildProperties-Delegate} $rdf.MsBuildArg 'TargetEnvironment' | & ${New-DynamicParameter-Delegate} | ForEach-Object { $parameterDictionary.Add($_.Name, $_) }
        $parameterDictionary
    }

    begin {
        $properties = @( $parameterDictionary.Keys |
            Where-Object { $parameterDictionary.$_.IsSet } |
            ForEach-Object { $parameterDictionary.$_ } |
            Select-Object -Property Name, Value )
    }
    process {
        #$rdf = Resolve-BizTalkApplicationDeploymentFile $Application
        $rte = Resolve-BizTalkApplicationTargetEnvironment $rdf.Name $TargetEnvironment
        $arguments = @{
            Project = $rdf.MsBuildArg ;
            Action = "Deploying and configuring $($rdf.Name) BizTalk Server Application with $($rte.DisplayName) environment settings"
        }
        if ($PSBoundParameters.ContainsKey('TargetEnvironment')) {
            $UnboundArguments = @('-TargetEnvironment', $rte.Name) + $UnboundArguments
        }
        if ($PSBoundParameters.ContainsKey('Verbosity')) {
            $arguments.Verbosity = $Verbosity
        }
        $arguments.UnboundArguments = @($properties | ForEach-Object { @("-$($_.Name)", $_.Value)}) + $UnboundArguments
        & ${Invoke-MSBuildCore-Delegate} @arguments `
            -Elevated `
            -Verbose:($PSBoundParameters['Verbose'] -eq $true) `
            -WhatIf:($PSBoundParameters['WhatIf'] -eq $true)
    }
}

<#
.SYNOPSIS
    Undeploy a BizTalk Server Application according to the settings of a specific target environment.
.DESCRIPTION
    This command will run the Undeploy target of the BizTalk Deployment Framework MSBuild script for a given BizTalk Server application.
.PARAMETER Application
    The name of the BizTalk Server Application to undeploy. Notice that tab expansion will suggest all, and only, the names of the BizTalk Server Applications for which an <application-name>.Deployment.btdfproj file can be found in the current folder or in the .\Deployment subfolder (in this order).
.PARAMETER TargetEnvironment
    The target environment whose settings will be used to undeploy and cleanup the BizTalk Server Application. Notice that tab expansion will suggest all, and only, the target environments defined in the setting file generator of the BizTalk Server Application to undeploy. Specifically, it will look into the <application-name>.SettingsFileGenerator.xml file located either in the current folder, the .\EnvironmentSettings subfolder, or the .\Deployment\EnvironmentSettings subfolder (in this order).
.PARAMETER UnboundArguments
    This parameter accumulates all the additional arguments and pass them along to MSBuild.
.EXAMPLE
    PS> Uninstall-BizTalkApplication -Application BizTalk.Factory

    Undeploy the BizTalk.Factory BizTalk Server Application and perform any cleanup actions defined in the BizTalk Deployment Framework MSBuild script of this BizTalk Server Application.
.EXAMPLE
    PS> Uninstall-BizTalkApplication -Application BizTalk.Factory -TargetEnvironment PRD

    Undeploy the BizTalk.Factory BizTalk Server Application and perform any cleanup actions defined for the PRD target environment in the BizTalk Deployment Framework MSBuild script of this BizTalk Server Application.
.COMPONENT
    This command relies on BizTalk Deployment Framework MSBuild scripts.
.LINK
    http://biztalkdeployment.codeplex.com/
.NOTES
    © 2013 - 2015 be.stateless.
#>
function Uninstall-BizTalkApplication
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Position=0,Mandatory=$true)]
        [string]
        $Application,

        [Parameter(Position=1)]
        [string]
        $TargetEnvironment,

        [Parameter(Mandatory=$false)]
        [ValidateSet('Quiet', 'Minimal', 'Normal', 'Detailed', 'Diagnostic')]
        [string]
        $Verbosity,

        [Parameter(DontShow,ValueFromRemainingArguments=$true)]
        [object[]]
        $UnboundArguments = @()
    )
    DynamicParam {
        $rdf = Resolve-BizTalkApplicationDeploymentFile $Application
        $parameterDictionary = New-Object -Type System.Management.Automation.RuntimeDefinedParameterDictionary
        & ${Probe-MSBuildProperties-Delegate} $rdf.MsBuildArg 'TargetEnvironment' | & ${New-DynamicParameter-Delegate} | ForEach-Object { $parameterDictionary.Add($_.Name, $_) }
        $parameterDictionary
    }

    begin {
        $properties = @( $parameterDictionary.Keys |
            Where-Object { $parameterDictionary.$_.IsSet } |
            ForEach-Object { $parameterDictionary.$_ } |
            Select-Object -Property Name, Value )
    }
    process {
        #$rdf = Resolve-BizTalkApplicationDeploymentFile $Application
        $rte = Resolve-BizTalkApplicationTargetEnvironment $rdf.Name $TargetEnvironment
        $arguments = @{
            Project = $rdf.MsBuildArg ;
            #TODO ensure only one target is ever passed to Install-BizTalkApplication
            Targets = 'Undeploy' ;
            Action = "Undeploying $($rdf.Name) BizTalk Server Application with $($rte.DisplayName) environment settings"
        }
        if ($PSBoundParameters.ContainsKey('TargetEnvironment')) {
            $UnboundArguments = @('-TargetEnvironment', $rte.Name) + $UnboundArguments
        }
        if ($PSBoundParameters.ContainsKey('Verbosity')) {
            $arguments.Verbosity = $Verbosity
        }
        $arguments.UnboundArguments = @($properties | ForEach-Object { @("-$($_.Name)", $_.Value)}) + $UnboundArguments
        & ${Invoke-MSBuildCore-Delegate} @arguments `
            -Elevated `
            -Verbose:($PSBoundParameters['Verbose'] -eq $true) `
            -WhatIf:($PSBoundParameters['WhatIf'] -eq $true)
    }
}

<#
.SYNOPSIS
    Updates the artifacts, policies, or bindings of a BizTalk Server Application according to the settings of a specific target environment.
.DESCRIPTION
    For a given BizTalk Server Application, this command will run a combination of the following BizTalk Deployment Framework MSBuild script targets: either DeployPolicies, or UpdateOrchestration, or PreProcessBindings and ImportBindings, and in all cases BounceBizTalk.
.PARAMETER Application
    The name of the BizTalk Server Application to update. Notice that tab expansion will suggest all, and only, the names of the BizTalk Server Applications for which an <application-name>.Deployment.btdfproj file can be found in the current folder or in the .\Deployment subfolder (in this order).
.PARAMETER TargetEnvironment
    The target environment whose settings will be used to update the BizTalk Server Application. Notice that tab expansion will suggest all, and only, the target environments defined in the setting file generator of the BizTalk Server Application to undeploy. Specifically, it will look into the <application-name>.SettingsFileGenerator.xml file located either in the current folder, the .\EnvironmentSettings subfolder, or the .\Deployment\EnvironmentSettings subfolder (in this order).
.PARAMETER All
    When this switch is specified, all BizTalk Server Application's artifacts, bindings, and policies are updated.
.PARAMETER Artifacts
    When this switch is specified, only the BizTalk Server Application's artifacts are updated. Note that this switch parameter may be combined with others. This switch is however the default one and is assumed to be set when no other switches are specified. When used in combination with other switch parameters, it has to explicitly specified.
.PARAMETER Bindings
    When this switch is specified, only the BizTalk Server Application's bindings are updated. Note that this switch parameter may be combined with others.
.PARAMETER Policies
    When this switch is specified, only the BizTalk Server Application's policies are updated. Note that this switch parameter may be combined with others.
.PARAMETER Bounce
    When this switch is specified, only the BizTalk Server Host Instances are restarted and the BizTalk Server Application is not updated. It is useless to combine this switch with others as it is always assumed to be set.
.PARAMETER UnboundArguments
    This parameter accumulates all the additional arguments and pass them along to MSBuild.
.EXAMPLE
    PS> Update-BizTalkApplication -Application BizTalk.Factory

    Update the BizTalk Server artifacts for the BizTalk.Factory BizTalk Server Application.
.EXAMPLE
    PS> Update-BizTalkApplication -Application BizTalk.Factory -Artifacts

    Update the BizTalk Server artifacts for the BizTalk.Factory BizTalk Server Application.
.EXAMPLE
    PS> Update-BizTalkApplication -Application BizTalk.Factory -Artifacts -Policies

    Update the BizTalk Server artifacts and policies for the BizTalk.Factory BizTalk Server application.
.EXAMPLE
    PS> Update-BizTalkApplication -Application BizTalk.Factory -Bindings

    Update the bindings for the BizTalk.Factory BizTalk Server application according to the settings defined for the default (typically DEV) target environment in the setting file generator of this BizTalk Server Application.
.EXAMPLE
    PS> Update-BizTalkApplication -Application BizTalk.Factory -TargetEnvironment PRD

    Update the bindings for the BizTalk.Factory BizTalk Server application according to the settings defined for the PRD target environment in the setting file generator of this BizTalk Server Application.
.EXAMPLE
    PS> Update-BizTalkApplication -Application BizTalk.Factory -All

    Update the artifacts, bindings, and policies for the BizTalk.Factory BizTalk Server application according to the settings defined for the default (typically DEV) target environment in the setting file generator of this BizTalk Server Application.

.EXAMPLE
    PS> Update-BizTalkApplication -Application BizTalk.Factory -Bounce

    Bounce the BizTalk Server Host Intances and does not update any of the artifacts, bindings, or policies of the BizTalk Server Application.
.COMPONENT
    This command relies on BizTalk Deployment Framework MSBuild scripts.
.LINK
    http://biztalkdeployment.codeplex.com/
.NOTES
    © 2012 - 2015 be.stateless.
#>
function Update-BizTalkApplication
{
    # http://powershell.com/cs/blogs/tobias/archive/2010/11/18/add-automatic-confirmation-and-whatif-to-your-scripts.aspx
    # http://blogs.msdn.com/b/powershell/archive/2008/12/23/powershell-v2-parametersets.aspx
    [CmdletBinding(DefaultParametersetName='Any',SupportsShouldProcess=$true)]
    param(
        [Parameter(Position=0,Mandatory=$true)]
        [string]
        $Application,

        [Parameter(Position=1)]
        [string]
        $TargetEnvironment,

        [Parameter(ParameterSetName='All')]
        [switch]
        $All,

        [Parameter(ParameterSetName='Any')]
        [switch]
        $Artifacts,

        [Parameter(ParameterSetName='Any')]
        [switch]
        $Bindings,

        [Parameter(ParameterSetName='Any')]
        [switch]
        $Policies,

        [Parameter(ParameterSetName='Any')]
        [switch]
        $Bounce,

        [Parameter(Mandatory=$false)]
        [ValidateSet('Quiet', 'Minimal', 'Normal', 'Detailed', 'Diagnostic')]
        [string]
        $Verbosity,

        [Parameter(DontShow,ValueFromRemainingArguments=$true)]
        [object[]]
        $UnboundArguments = @()
    )
    DynamicParam {
        $rdf = Resolve-BizTalkApplicationDeploymentFile $Application
        $parameterDictionary = New-Object -Type System.Management.Automation.RuntimeDefinedParameterDictionary
        & ${Probe-MSBuildProperties-Delegate} $rdf.MsBuildArg 'TargetEnvironment' | & ${New-DynamicParameter-Delegate} | ForEach-Object { $parameterDictionary.Add($_.Name, $_) }
        $parameterDictionary
    }

    begin {
        $properties = @( $parameterDictionary.Keys |
            Where-Object { $parameterDictionary.$_.IsSet } |
            ForEach-Object { $parameterDictionary.$_ } |
            Select-Object -Property Name, Value )
    }
    process {
        $All = $PsCmdlet.ParameterSetName -eq 'All'
        if ($PsCmdlet.ParameterSetName -eq 'Any') {
            # ensure -Artifacts if no other switch has been set
            $Artifacts = $Artifacts -or (-not $Bindings -and -not $Policies -and -not $Bounce)
        }

        #$rdf = Resolve-BizTalkApplicationDeploymentFile $Application
        $rte = Resolve-BizTalkApplicationTargetEnvironment $rdf.Name $TargetEnvironment

        # make sure BizTalk hosts are always bounced whatever the update
        $task = @{ Actions = @(); Targets = @('BounceBizTalk') }
        if ($All -or $Artifacts) {
            $task.Actions += @('artifacts')
            # order targets to ensure BounceBizTalk is performed last
            # UpdateOrchestration depends on BounceBizTalk, hence latter would not be required
            $task.Targets = @('UpdateOrchestration') + $task.Targets
        }
        if ($All -or $Bindings) {
            $task.Actions += @('bindings')
            # order targets to ensure BounceBizTalk is performed last
            $task.Targets = @('PreProcessBindings', 'ImportBindings') + $task.Targets
        }
        if ($All -or $Policies) {
            $task.Actions += @('policies')
            # order targets to ensure BounceBizTalk is performed last
            $task.Targets = @('DeployPolicies') + $task.Targets
        }

        $arguments = @{
            Project = $rdf.MsBuildArg ;
            Targets = $task.Targets ;
        }
        if ($task.Actions.Length -gt 0) {
            $arguments.Action = "Updating $($rdf.Name) BizTalk Server Application's $(Join-String -Strings $task.Actions -Separator ', ') with $($rte.DisplayName) environment settings then bouncing BizTalk Server host instances"
        } else {
            $arguments.Action = 'Bouncing BizTalk Server host instances'
        }
        if ($PSBoundParameters.ContainsKey('TargetEnvironment')) {
            $UnboundArguments = @('-TargetEnvironment', $rte.Name) + $UnboundArguments
        }
        if ($PSBoundParameters.ContainsKey('Verbosity')) {
            $arguments.Verbosity = $Verbosity
        }
        $arguments.UnboundArguments = @($properties | ForEach-Object { @("-$($_.Name)", $_.Value)}) + $UnboundArguments
        & ${Invoke-MSBuildCore-Delegate} @arguments `
            -Elevated `
            -Verbose:($PSBoundParameters['Verbose'] -eq $true) `
            -WhatIf:($PSBoundParameters['WhatIf'] -eq $true)
    }
}

#region TabExpansion Overrides / Private Probing and Resolution Helper Functions

function Register-TabExpansions
{
    $global:options['CustomArgumentCompleters']['Application'] =  {
        param($commandName, $parameterName, $wordToComplete, $commandAst, $fakeBoundParameter)
        if ($commandName -eq 'Build-BizTalkApplication')
        {
            Probe-BizTalkApplicationSolution $wordToComplete | ForEach-Object {
                New-Object System.Management.Automation.CompletionResult $_, $_, 'ParameterValue', "$_.sln"
            }
        }
        else
        {
            Probe-BizTalkApplicationDeployment $wordToComplete | ForEach-Object {
                New-Object System.Management.Automation.CompletionResult $_, $_, 'ParameterValue', "$_.Deployment.btdfproj"
            }
        }
    }

    $global:options['CustomArgumentCompleters']['TargetEnvironment'] = {
        param($commandName, $parameterName, $wordToComplete, $commandAst, $fakeBoundParameter)
        Probe-BizTalkApplicationTargetEnvironment $fakeBoundParameter.Application $wordToComplete | ForEach-Object {
            New-Object System.Management.Automation.CompletionResult $_, $_, 'ParameterValue', $_
        }
    }
}

function Unregister-TabExpansions
{
    $global:options['CustomArgumentCompleters'].Remove('Application')
    $global:options['CustomArgumentCompleters'].Remove('TargetEnvironment')
}

function Probe-BizTalkApplicationDeployment([string]$pattern)
{
    if ($pattern) { $pattern = $pattern.Trim() }
    $applicationNames = Probe-BizTalkApplicationNames
    $applicationNames | Where-Object { $_ -match $pattern }
}

function Probe-BizTalkApplicationSolution([string]$pattern)
{
    $applicationNames = Probe-BizTalkApplicationNames

    if ($pattern) { $pattern = $pattern.Trim() }
    $files = Get-ChildItem -Path . -Filter "$pattern*.sln" |
        Where-Object { $applicationNames -contains $_.BaseName } |
        Select-Object -ExpandProperty BaseName
    $files | Sort-Object -Unique
}

function Probe-BizTalkApplicationNames()
{
    $projectNames = Get-ChildItem -Path '.', '.\Deployment' -Filter *.btdfproj | ForEach-Object {
        $xml = [xml](Get-Content $_.FullName)
        $nsm = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
        $nsm.AddNamespace('s0', 'http://schemas.microsoft.com/developer/msbuild/2003')
        $xml.SelectSingleNode('//s0:ProjectName', $nsm).InnerText
    }
    @($projectNames) | Sort-Object -Unique
}

function Probe-BizTalkApplicationTargetEnvironment([string]$application, [string]$pattern)
{
    if ($application) { $application = $application.Trim() }
    if ($pattern) { $pattern = $pattern.Trim() }
    $file = (Resolve-BizTalkApplicationSettingsFileGenerator $application).MsBuildArg
    $xml = [xml](Get-Content $file)
    $nsm = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
    $nsm.AddNamespace('s0', 'urn:schemas-microsoft-com:office:spreadsheet')
    # take TargetEnvironment row's 2nd cell too, i.e. the default value, to
    # ensure we list all values as other cells might be empty and rely on the
    # default value
    $te = $xml.SelectNodes("//s0:Worksheet//s0:Row[s0:Cell[1][s0:Data/text() = 'TargetEnvironment']]/s0:Cell[position() > 1]/s0:Data/text()", $nsm)
    $te | Select-Object -ExpandProperty Value -Unique `
        | Where-Object { $_ -match "^$pattern" }
}

function Resolve-BizTalkApplicationDeploymentFile([string]$application)
{
    if ($application) { $application = $application.Trim() }
    $paths = @('.', '.\Deployment') | Where-Object { Test-Path -Path $_ }
    $file = Get-ChildItem -Path $paths -Filter *.btdfproj | Where-Object {
        $xml = [xml](Get-Content $_.FullName)
        $nsm = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
        $nsm.AddNamespace('s0', 'http://schemas.microsoft.com/developer/msbuild/2003')
        $xml.SelectSingleNode('//s0:ProjectName', $nsm).InnerText -eq $application
    } | Get-Unique | Select-Object -First 1
    if (-not(Test-Path -Path $file.FullName)) {
        throw "No BTDF deployment file found for $application BizTalk Server Application!"
    }
    @{ Name = $application ; MsBuildArg = (Resolve-Path $file.FullName -Relative) }
}

function Resolve-BizTalkApplicationSettingsFileGenerator([string]$application)
{
    if ($application) { $application = $application.Trim() }
    $paths = @('.\Deployment\EnvironmentSettings', '.\EnvironmentSettings') |
        Where-Object { Test-Path -Path $_ }
    $file = Get-ChildItem -Path $paths -Filter *SettingsFileGenerator.xml |
        Where-Object { $_.Name -eq 'SettingsFileGenerator.xml' -or $_.Name -eq "$application.SettingsFileGenerator.xml" } |
        Get-Unique
    if (-not(Test-Path $file.FullName)) {
        throw "No settings file generator file found for $application BizTalk Server Application!"
    }
    @{ Name = $application ; MsBuildArg = (Resolve-Path $file.FullName -Relative) }
}

function Resolve-BizTalkApplicationSolutionFile([string]$application)
{
    if ($application) { $application = $application.Trim() }
    $file = "$application.sln"
    if (-not(Test-Path -Path .\$file)) {
        throw "No solution file found for $application BizTalk Server Application!"
    }
    @{ Name = $application ; MsBuildArg = (Resolve-Path $file -Relative) }
}

function Resolve-BizTalkApplicationTargetEnvironment([string]$application, [string]$targetEnvironment)
{
    if ($application) { $application = $application.Trim() }
    $tes = @(Probe-BizTalkApplicationTargetEnvironment $application)

    if ($targetEnvironment) {
        $targetEnvironment = $targetEnvironment.Trim()
        if (-not($targetEnvironment -in $tes)) {
            throw "$targetEnvironment target environment not found for $application BizTalk Server Application!"
        }
        @{ DisplayName = $targetEnvironment ; Name = $targetEnvironment }
    } else {
        # fall back on default target environment as no one was given, which is supposed to be the first one
        @{ DisplayName = "default ($($tes[0]))" }
    }
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

$btsPath = "C:\Program Files{0}\Microsoft BizTalk Server {1}\Tracking" -f (?: { Test-64bitArchitecture } { ' (x86)' } { '' } ), '{0}'
if (Test-Path -Path ($btsPath -f '2010')) {
    $env:path += ';' + ($btsPath -f '2010')
}
elseif (Test-Path -Path ($btsPath -f '2009')) {
    $env:path += ';' + ($btsPath -f '2009')
}

# friend function delegates
${Invoke-MSBuildCore-Delegate} = (& (Get-Module MSBuild) { (Get-Item function:Invoke-MSBuildCore) })
${New-DynamicParameter-Delegate} = (& (Get-Module MSBuild) { (Get-Item function:New-DynamicParameter) })
${Probe-MSBuildProperties-Delegate} = (& (Get-Module MSBuild) { (Get-Item function:Probe-MSBuildProperties) })

New-Alias -Name bba -Value Build-BizTalkApplication
New-Alias -Name iba -Value Install-BizTalkApplication
New-Alias -Name xba -Value Uninstall-BizTalkApplication
New-Alias -Name uba -Value Update-BizTalkApplication

Export-ModuleMember -Alias * -Function *