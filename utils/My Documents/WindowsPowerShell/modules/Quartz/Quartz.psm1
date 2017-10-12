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

#region Quartz Scheduler

function Get-QuartzScheduler {
   [CmdletBinding()]
   Param(
      [Parameter(Mandatory = $false, Position = 0)]
      [ValidateNotNullorEmpty()]
      [string]
      $Name = 'QuartzScheduler',

      [Parameter(Mandatory = $false, Position = 1)]
      [ValidateNotNull()]
      [int]
      $Port = 5555
   )

   if ($script:scheduler -eq $null) {
      $settings = New-Object -TypeName System.Collections.Specialized.NameValueCollection
      $settings['quartz.scheduler.instanceName'] = 'BizTalkPlatformScheduler';
      $settings['quartz.scheduler.proxy'] = 'true';
      $settings['quartz.scheduler.proxy.address'] = "tcp://localhost:$Port/$Name";
      Write-Verbose -Message 'Constructing Quartz scheduler instance proxy.'
      $schedulerFactory = New-Object -TypeName Quartz.Impl.StdSchedulerFactory
      $schedulerFactory.Initialize($settings)
      $script:scheduler = $schedulerFactory.GetScheduler()
   }
   $script:scheduler
}

#endregion

#region Quartz Job

function Assert-QuartzJob {
   [CmdletBinding()]
   Param(
      [Parameter(Mandatory = $true, ParameterSetName = 'vector', Position = 0, ValueFromPipeline = $true)]
      [ValidateNotNull()]
      [Quartz.JobKey]
      $JobKey,

      [Parameter(Mandatory = $true, ParameterSetName = 'scalar', Position = 0, ValueFromPipeline = $false)]
      [ValidateNotNullorEmpty()]
      [string]
      $Name,

      [Parameter(Mandatory = $false, ParameterSetName = 'scalar', Position = 1, ValueFromPipeline = $false)]
      [ValidateNotNullorEmpty()]
      [string]
      $Group = $null
   )

   begin {
      $scheduler = Get-QuartzScheduler
   }
   process {
      if ($PsCmdlet.ParameterSetName -eq 'scalar') {
         $JobKey = New-Object -TypeName Quartz.JobKey -ArgumentList $Name, $Group
      }
      if (-not $scheduler.CheckExists($JobKey)) {
         throw ('{0}  job not found.' -f (GetQuartzJobDisplayName $JobKey))
      }
   }
}

function Get-QuartzJob {
   [CmdletBinding()]
   Param(
      [Parameter(Mandatory = $false, Position = 0)]
      [ValidateNotNullorEmpty()]
      [string]
      $Name = '.*',

      [Parameter(Mandatory = $false, Position = 1)]
      [ValidateNotNullorEmpty()]
      [string]
      $Group = '.*',

      [Parameter(Mandatory = $false)]
      [switch]
      $Detailed,

      [Parameter(Mandatory = $false)]
      [switch]
      $Trigger
   )

   begin {
      $scheduler = Get-QuartzScheduler
   }
   process {
      GetQuartzJobKeys |
         Where-Object { $_.Name -match $Name -and $_.Group -match $Group } |
         Where-Object { Test-QuartzJob -JobKey $_ -ThrowOnError } |
         ForEach-Object {
         # https://kevsor1.wordpress.com/2011/11/03/powershell-v2-detecting-verbose-debug-and-other-bound-parameters/
         if (-not $PSBoundParameters['Detailed'].IsPresent -and -not $PSBoundParameters['Trigger'].IsPresent) {
            $_
         } else {
            if ($Detailed) {
               $scheduler.GetJobDetail($_)
            }
            if ($Trigger) {
               $scheduler.GetTriggersOfJob($_) | Select-Object *, @{n = 'State'; e = {$scheduler.GetTriggerState($_.Key)} }
            }
         }
      }
   }
}

function Remove-QuartzJob {
   [CmdletBinding(SupportsShouldProcess = $true)]
   Param(
      [Parameter(Mandatory = $true, ParameterSetName = 'vector', Position = 0, ValueFromPipeline = $true)]
      [ValidateNotNull()]
      [Quartz.JobKey]
      $JobKey,

      [Parameter(Mandatory = $true, ParameterSetName = 'scalar', Position = 0, ValueFromPipeline = $false)]
      [ValidateNotNullorEmpty()]
      [string]
      $Name,

      [Parameter(Mandatory = $false, ParameterSetName = 'scalar', Position = 1, ValueFromPipeline = $false)]
      [ValidateNotNullorEmpty()]
      [string]
      $Group = $null
   )

   begin {
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

function Resume-QuartzJob {
   [CmdletBinding(SupportsShouldProcess = $true)]
   Param(
      [Parameter(Mandatory = $true, ParameterSetName = 'vector', Position = 0, ValueFromPipeline = $true)]
      [ValidateNotNull()]
      [Quartz.JobKey]
      $JobKey,

      [Parameter(Mandatory = $true, ParameterSetName = 'scalar', Position = 0, ValueFromPipeline = $false)]
      [ValidateNotNullorEmpty()]
      [string]
      $Name,

      [Parameter(Mandatory = $false, ParameterSetName = 'scalar', Position = 1, ValueFromPipeline = $false)]
      [ValidateNotNullorEmpty()]
      [string]
      $Group = $null
   )

   begin {
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

function Suspend-QuartzJob {
   [CmdletBinding(SupportsShouldProcess = $true)]
   Param(
      [Parameter(Mandatory = $true, ParameterSetName = 'vector', Position = 0, ValueFromPipeline = $true)]
      [ValidateNotNull()]
      [Quartz.JobKey]
      $JobKey,

      [Parameter(Mandatory = $true, ParameterSetName = 'scalar', Position = 0, ValueFromPipeline = $false)]
      [ValidateNotNullorEmpty()]
      [string]
      $Name,

      [Parameter(Mandatory = $false, ParameterSetName = 'scalar', Position = 1, ValueFromPipeline = $false)]
      [ValidateNotNullorEmpty()]
      [string]
      $Group = $null
   )

   begin {
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

function Test-QuartzJob {
   [CmdletBinding()]
   Param(
      [Parameter(Mandatory = $true, ParameterSetName = 'vector', Position = 0, ValueFromPipeline = $true)]
      [ValidateNotNull()]
      [Quartz.JobKey]
      $JobKey,

      [Parameter(Mandatory = $true, ParameterSetName = 'scalar', Position = 0, ValueFromPipeline = $false)]
      [ValidateNotNullorEmpty()]
      [string]
      $Name,

      [Parameter(Mandatory = $false, ParameterSetName = 'scalar', Position = 1, ValueFromPipeline = $false)]
      [ValidateNotNullorEmpty()]
      [string]
      $Group = $null,

      [Parameter(DontShow, ParameterSetName = 'vector')]
      [Parameter(DontShow, ParameterSetName = 'scalar')]
      [switch]
      $ThrowOnError
   )

   begin {
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

function Update-QuartzJob {
   # https://stackoverflow.com/questions/33611451/update-jobdatamap-doesnt-work-as-expected-in-quartz-net-with-c-sharp
   # Name                          : job1
   # Group                         : group1
   # FullName                      : group1.job1
   # Key                           : group1.job1
   # Description                   : Greets every 5 seconds.
   # JobType                       : QuartzJobs.HelloJob
   # JobDataMap                    : {[key, value]}
   # RequestsRecovery              : False
   # Durable                       : False
   # PersistJobDataAfterExecution  : False
   # ConcurrentExecutionDisallowed : False
   [CmdletBinding()]
   Param(
      [Parameter(Mandatory = $true, ParameterSetName = 'vector', Position = 0, ValueFromPipeline = $true)]
      [ValidateNotNull()]
      [Quartz.JobKey]
      $JobKey,

      [Parameter(Mandatory = $true, ParameterSetName = 'scalar', Position = 0, ValueFromPipeline = $false)]
      [ValidateNotNullorEmpty()]
      [string]
      $Name,

      [Parameter(Mandatory = $false, ParameterSetName = 'scalar', Position = 1, ValueFromPipeline = $false)]
      [ValidateNotNullorEmpty()]
      [string]
      $Group = $null,

      [Parameter(Mandatory = $false, ParameterSetName = 'scalar', Position = 1, ValueFromPipeline = $false)]
      [ValidateNotNullorEmpty()]
      [string]
      $JobType

   )

   begin {
      $scheduler = Get-QuartzScheduler
   }
   process {
      GetQuartzJobKeys |
         Where-Object { $_.Name -match $Name -and $_.Group -match $Group } |
         Where-Object { Test-QuartzJob -JobKey $_ -ThrowOnError } |
         ForEach-Object {
         $_
      }
   }
}

#endregion

#region Quartz Trigger

function Get-QuartzTrigger {
   [CmdletBinding(DefaultParameterSetName = 'simple')]
   Param(
      [Parameter(Mandatory = $true, ParameterSetName = 'vector', Position = 0, ValueFromPipeline = $true)]
      [Quartz.TriggerKey]
      [ValidateNotNull()]
      $TriggerKey,

      [Parameter(Mandatory = $false, ParameterSetName = 'scalar', Position = 0, ValueFromPipeline = $false)]
      [ValidateNotNullorEmpty()]
      [string]
      $Name = '.*',

      [Parameter(Mandatory = $false, ParameterSetName = 'scalar', Position = 1, ValueFromPipeline = $false)]
      [ValidateNotNullorEmpty()]
      [string]
      $Group = '.*',

      [Parameter(Mandatory = $false, ParameterSetName = 'switch')]
      [switch]
      $Detailed
   )

   begin {
      $scheduler = Get-QuartzScheduler
   }
   process {
      GetQuartzTriggerKeys |
         Where-Object { $_.Name -match $Name -and $_.Group -match $Group } |
         Where-Object { Test-QuartzTrigger -TriggerKey $_ -ThrowOnError } |
         ForEach-Object { $scheduler.GetTriggersOfJob($_) }
      # | ForEach-Object { $scheduler.GetTriggerState($_.Key) }
   }
}

function Test-QuartzTrigger {
   [CmdletBinding()]
   Param(
      [Parameter(Mandatory = $true, ParameterSetName = 'vector', Position = 0, ValueFromPipeline = $true)]
      [ValidateNotNull()]
      [Quartz.TriggerKey]
      $TriggerKey,

      [Parameter(Mandatory = $true, ParameterSetName = 'scalar', Position = 0, ValueFromPipeline = $false)]
      [ValidateNotNullorEmpty()]
      [string]
      $Name,

      [Parameter(Mandatory = $false, ParameterSetName = 'scalar', Position = 1, ValueFromPipeline = $false)]
      [ValidateNotNullorEmpty()]
      [string]
      $Group = $null,

      [Parameter(DontShow, ParameterSetName = 'vector')]
      [Parameter(DontShow, ParameterSetName = 'scalar')]
      [switch]
      $ThrowOnError
   )

   begin {
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

#endregion

<#
# Helpers
#>

function AssertQuartzAgent {
   [CmdletBinding()]
   param()

   $agent = Get-Service -Name QuartzAgent -ErrorAction SilentlyContinue
   if ($agent -eq $null) {
      throw 'Quart.NET Scheduler Agent is not installed on this machine.'
   } elseif ($agent.Status -ne 'Running') {
      throw 'Quart.NET Scheduler Agent is not running.'
   }
}

function GetQuartzJobDisplayName {
   [CmdletBinding()]
   Param(
      [Quartz.JobKey]
      [ValidateNotNull()]
      $JobKey
   )

   '[{0}.{1}]' -f $JobKey.Group, $JobKey.Name
}

function GetQuartzJobKeys {
   [CmdletBinding()]
   Param()

   # https://stackoverflow.com/questions/11975130/generic-type-of-generic-type-in-powershell
   $groupMatcher = [Type]('Quartz.Impl.Matchers.GroupMatcher[[{0}]]' -f [Quartz.JobKey].fullname)
   $anyGroupMatcher = $groupMatcher::AnyGroup()
   $scheduler = Get-QuartzScheduler
   $scheduler.GetJobKeys($anyGroupMatcher)
}

function GetQuartzTriggerKeys {
   [CmdletBinding()]
   Param()

   $groupMatcher = [Type]('Quartz.Impl.Matchers.GroupMatcher[[{0}]]' -f [Quartz.TriggerKey].fullname)
   $anyGroupMatcher = $groupMatcher::AnyGroup()
   $scheduler = Get-QuartzScheduler
   $scheduler.GetTriggerKeys($anyGroupMatcher)
}

<#
# Main
#>

# register clean up handler should the module be removed from the session
$MyInvocation.MyCommand.ScriptBlock.Module.OnRemove = {
   $script:scheduler = $null
}

$scheduler = $null

#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO AssertQuartzAgent
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO
#TODO

Export-ModuleMember -Alias * -Function '*-*'
