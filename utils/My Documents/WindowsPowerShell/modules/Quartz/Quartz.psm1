#region Copyright & License

# Copyright © 2012 - 2017 François Chabot
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

function Get-QuartzAgentInstallationPath
{
    [CmdletBinding()]
    Param()

    if ($script:quartzAgentInstallationPath -eq $null) {
        $path = GetQuartzAgentLocation
        $script:quartzAgentInstallationPath = Split-Path -Path $path -Parent
    }
    $script:quartzAgentInstallationPath
}

function Get-QuartzScheduler
{
    [CmdletBinding()]
    Param()

    if ($script:scheduler -eq $null) {
        Write-Verbose -Message 'Loading Quartz scheduler settings from service host''s config file.'
        $configuration = [System.Configuration.ConfigurationManager]::OpenExeConfiguration((GetQuartzAgentLocation))
        $section = $configuration.GetSection("quartz")
        # convert AppSettings into a NameValueCollection
        $settings = New-Object -TypeName System.Collections.Specialized.NameValueCollection -ArgumentList $section.Settings.Count
        # filter out irrelevant/unnecessary settings, whose some would entail scheduler's initialization errors when performed from within PowerShell
        $section.Settings `
            | Where-Object { $_.Key -notmatch 'quartz\.plugin\.xml\.' } `
            | ForEach-Object { $settings[$_.Key] = $_.Value }

        Write-Verbose -Message 'Constructing Quartz scheduler instance.'
        $schedulerFactory = New-Object -TypeName Quartz.Impl.StdSchedulerFactory
        $schedulerFactory.Initialize($settings)
        $script:scheduler = $schedulerFactory.GetScheduler()
    }
    $script:scheduler
}

function Get-QuartzJob
{
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory=$false,Position=0)]
        [string]
        $Name = '.*',

        [Parameter(Mandatory=$false,Position=1)]
        [string]
        $Group = '.*',

        [Parameter(Mandatory=$false)]
        [switch]
        $Detailed
    )

    begin{
        $scheduler = Get-QuartzScheduler
    }
    process {
        GetQuartzJobKeys |
            Where-Object { $_.Name -match $Name -and $_.Group -match $Group } |
            Where-Object { Test-QuartzJob -JobKey $_ -ThrowOnError } |
            ForEach-Object { if ($Detailed) { $scheduler.GetJobDetail($_) } else { $_ } }
    }
}

function Remove-QuartzJob
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    Param(
        [Parameter(Mandatory=$true,ParameterSetName='vector',Position=0,ValueFromPipeline=$true)]
        [Quartz.JobKey]
        $JobKey,
    
        [Parameter(Mandatory=$true,ParameterSetName='scalar',Position=0,ValueFromPipeline=$false)]
        [string]
        $Name,

        [Parameter(Mandatory=$false,ParameterSetName='scalar',Position=1,ValueFromPipeline=$false)]
        [string]
        $Group = $null
    )

    begin{
        $scheduler = Get-QuartzScheduler
    }
    process {
        if ($PsCmdlet.ParameterSetName -eq 'scalar') {
            $JobKey = New-Object -TypeName Quartz.JobKey -ArgumentList $Name, $Group
        }
        if (Test-QuartzJob -JobKey $JobKey -ThrowOnError) {
            if ($PsCmdlet.ShouldProcess((GetQuartzJobDisplayName $JobKey), 'Delete Job')) {
                if ($scheduler.DeleteJob($JobKey)) {
                    Write-Verbose -Message ('{0} job has been deleted.' -f (GetQuartzJobDisplayName $JobKey))
                } else {
                    Write-Error -Message ('{0} job could not be deleted.' -f (GetQuartzJobDisplayName $JobKey))
                }
            }
        }
    }
}

function Resume-QuartzJob
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    Param(
        [Parameter(Mandatory=$true,ParameterSetName='vector',Position=0,ValueFromPipeline=$true)]
        [Quartz.JobKey]
        $JobKey,
    
        [Parameter(Mandatory=$true,ParameterSetName='scalar',Position=0,ValueFromPipeline=$false)]
        [string]
        $Name,

        [Parameter(Mandatory=$false,ParameterSetName='scalar',Position=1,ValueFromPipeline=$false)]
        [string]
        $Group = $null
    )

    begin{
        $scheduler = Get-QuartzScheduler
    }
    process {
        if ($PsCmdlet.ParameterSetName -eq 'scalar') {
            $JobKey = New-Object -TypeName Quartz.JobKey -ArgumentList $Name, $Group
        }
        if (Test-QuartzJob -JobKey $JobKey -ThrowOnError) {
            if ($PsCmdlet.ShouldProcess((GetQuartzJobDisplayName $JobKey), 'Resume Job')) {
                $scheduler.ResumeJob($JobKey)
                Write-Verbose -Message ('{0} job has been resumed.' -f (GetQuartzJobDisplayName $JobKey))
            }
        }
    }
}

function Suspend-QuartzJob
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    Param(
        [Parameter(Mandatory=$true,ParameterSetName='vector',Position=0,ValueFromPipeline=$true)]
        [Quartz.JobKey]
        $JobKey,
    
        [Parameter(Mandatory=$true,ParameterSetName='scalar',Position=0,ValueFromPipeline=$false)]
        [string]
        $Name,

        [Parameter(Mandatory=$false,ParameterSetName='scalar',Position=1,ValueFromPipeline=$false)]
        [string]
        $Group = $null
    )

    begin{
        $scheduler = Get-QuartzScheduler
    }
    process {
        if ($PsCmdlet.ParameterSetName -eq 'scalar') {
            $JobKey = New-Object -TypeName Quartz.JobKey -ArgumentList $Name, $Group
        }
        if (Test-QuartzJob -JobKey $JobKey -ThrowOnError) {
            if ($PsCmdlet.ShouldProcess((GetQuartzJobDisplayName $JobKey), 'Suspend Job')) {
                $scheduler.PauseJob($JobKey)
                Write-Verbose -Message ('{0} job has been paused.' -f (GetQuartzJobDisplayName $JobKey))
            }
        }
    }
}

function Assert-QuartzJob
{
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory=$true,ParameterSetName='vector',Position=0,ValueFromPipeline=$true)]
        [Quartz.JobKey]
        $JobKey,
    
        [Parameter(Mandatory=$true,ParameterSetName='scalar',Position=0,ValueFromPipeline=$false)]
        [string]
        $Name,

        [Parameter(Mandatory=$false,ParameterSetName='scalar',Position=1,ValueFromPipeline=$false)]
        [string]
        $Group = $null
    )

    begin{
        $scheduler = Get-QuartzScheduler
    }
    process {
        if ($PsCmdlet.ParameterSetName -eq 'scalar') {
            $JobKey = New-Object -TypeName Quartz.JobKey -ArgumentList $Name, $Group
        }
        if (-not $scheduler.CheckExists($JobKey)) {
            Write-Error -Message ('{0}  job not found.' -f (GetQuartzJobDisplayName $JobKey))
        }
    }
}

function Test-QuartzJob
{
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory=$true,ParameterSetName='vector',Position=0,ValueFromPipeline=$true)]
        [Quartz.JobKey]
        $JobKey,
    
        [Parameter(Mandatory=$true,ParameterSetName='scalar',Position=0,ValueFromPipeline=$false)]
        [string]
        $Name,

        [Parameter(Mandatory=$false,ParameterSetName='scalar',Position=1,ValueFromPipeline=$false)]
        [string]
        $Group = $null,

        [Parameter(DontShow,ParameterSetName='vector')]
        [Parameter(DontShow,ParameterSetName='scalar')]
        [switch]
        $ThrowOnError
    )

    begin{
        $scheduler = Get-QuartzScheduler
    }
    process {
        if ($PsCmdlet.ParameterSetName -eq 'scalar') {
            $JobKey = New-Object -TypeName Quartz.JobKey -ArgumentList $Name, $Group
        }
        $scheduler.CheckExists($JobKey)
        if ($ThrowOnError -and -not $scheduler.CheckExists($JobKey)) {
            Write-Error -Message ('{0}  job not found.' -f (GetQuartzJobDisplayName $JobKey))
        }
    }
}

<#
 # Helpers
 #>

function GetQuartzAgent
{
    [CmdletBinding()]
    Param()

    Get-CimInstance -ClassName Win32_Service -Filter 'Name=''QuartzAgent'''
}

function AssertQuartzAgent
{
    [CmdletBinding()]
    param()

    $agent = GetQuartzAgent
    if ($agent -eq $null) {
        Write-Error 'Quart.NET Scheduler Agent is not available on this machine.'
    }
}

function GetQuartzAgentLocation
{
    [CmdletBinding()]
    Param()

    if ($script:quartzAgentLocation -eq $null) {
        $agent = GetQuartzAgent
        $script:quartzAgentLocation = $agent.PathName.Trim('"')
    }
    $script:quartzAgentLocation
}

function GetQuartzJobDisplayName
{
    [CmdletBinding()]
    Param(
        [Quartz.JobKey]
        $JobKey
    )

    if ($JobKey.Group.Length -gt 0) {
        ('[{0}:{1}]' -f $JobKey.Group, $JobKey.Name)
    } else {
        ('[{0}]' -f $JobKey.Name)
    }
}

function GetQuartzJobKeys
{
    [CmdletBinding()]
    Param()
    
    $scheduler = Get-QuartzScheduler
	# https://stackoverflow.com/questions/11975130/generic-type-of-generic-type-in-powershell
    $groupMatcher = [Type]('Quartz.Impl.Matchers.GroupMatcher[[{0}]]' -f [Quartz.JobKey].fullname)
    $anyGroupMatcher = $groupMatcher::AnyGroup()
    $scheduler.GetJobKeys($anyGroupMatcher)
}

<#
 # Main
 #>

# register clean up handler should the module be removed from the session
$MyInvocation.MyCommand.ScriptBlock.Module.OnRemove = {
    $script:quartzAgentLocation = $null
    $script:quartzAgentInstallationPath = $null
    $script:scheduler = $null
}

$quartzAgentLocation = $null
$quartzAgentInstallationPath = $null
$scheduler = $null
AssertQuartzAgent

Export-ModuleMember -Alias * -Function '*-*'
