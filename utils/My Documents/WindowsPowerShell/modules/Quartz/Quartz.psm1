function Get-QuartzAgentInstallationPath
{
    [CmdletBinding()]
    Param()

    if ($script:quartzAgentInstallationPath -eq $null) {
        $path = GetQuartzAgentExecutableAbsolutePath
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
        $configuration = [System.Configuration.ConfigurationManager]::OpenExeConfiguration((GetQuartzAgentExecutableAbsolutePath))
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

function Get-QuartzJobs
{
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory=$false)]
        [switch]
        $Detailed
    )

    $scheduler = Get-QuartzScheduler
	# https://stackoverflow.com/questions/11975130/generic-type-of-generic-type-in-powershell
    $groupMatcher = [Type]('Quartz.Impl.Matchers.GroupMatcher[[{0}]]' -f [Quartz.JobKey].fullname)
    $anyGroupMatcher = $groupMatcher::AnyGroup()
    $jobKeys = $scheduler.GetJobKeys($anyGroupMatcher)

    if ($Detailed) {
        $jobKeys | ForEach-Object { $scheduler.GetJobDetail($_) }
    }
    else {
        $jobKeys
    }
}

function Get-QuartzJob
{
    [CmdletBinding()]
    Param(
        [Parameter(Position=0,Mandatory=$true)]
        [string]
        $Name,

        [Parameter(Position=1,Mandatory=$false)]
        [string]
        $Group = $null
    )

}

<#
 # Helpers
 #>
function AssertQuartzAgent
{
    [CmdletBinding()]
    param()

    $agent = Get-CimInstance -ClassName Win32_Service -Filter "Name='QuartzAgent'"
    if ($agent -eq $null) {
        throw "Quart.NET Scheduler Agent is not available on this machine."
    }
}

function GetQuartzAgentExecutableAbsolutePath
{
    [CmdletBinding()]
    Param()

    if ($script:quartzAgentExecutableAbsolutePath -eq $null) {
        $path = Get-CimInstance -ClassName Win32_Service -Filter "Name='QuartzAgent'" | Select-Object -ExpandProperty PathName
        $script:quartzAgentExecutableAbsolutePath = $path.Trim('"')
    }
    $script:quartzAgentExecutableAbsolutePath
}

# TODO convert to actual function syntax
$onAssemblyResolveEventHandler = [System.ResolveEventHandler] {
    param($sender, $eventArgs)

    $assemblyName = New-Object -TypeName System.Reflection.AssemblyName -ArgumentList $eventArgs.Name
    $assemblyPath = Join-Path -Path Get-QuartzAgentInstallationPath -ChildPath ($assemblyName.Name + '.dll')
    # resource and XML serializer assemblies that are most probably nonexistent
    if ($assemblyName.Name -match '\.resources$|\.XmlSerializers$') {
        # Write-Verbose -Message ('Skipping resolution of assembly {0}.' -f $eventArgs.Name)
        return $null
    }

    Write-Verbose -Message ('Trying to resolve assembly {0}.' -f $eventArgs.Name)
    if (Test-Path -Path $assemblyPath) {
        Write-Verbose -Message ('Trying to load assembly {0}.' -f $assemblyPath)
        Add-Type -Path $assemblyPath
        $assembly = [System.AppDomain]::CurrentDomain.GetAssemblies() | Where-Object { $_.GetName() -eq $assemblyName } | Select-Object -Unique
        if ($assembly -eq $null) { throw ('Failed to load assembly {0}.' -f $assemblyPath) }
        return $assembly
    }

    if ($eventArgs.Name -ne $null -and $eventArgs.Name.Length -gt 0) {
        Write-Warning -Message ('Cannot resolve assembly {0}' -f $eventArgs.Name)
    }
    return $null
}

<#
 # Main
 #>

# register clean up handler should the module be removed from the session
$MyInvocation.MyCommand.ScriptBlock.Module.OnRemove = {
    # [System.AppDomain]::CurrentDomain.remove_AssemblyResolve($onAssemblyResolveEventHandler)
    $script:scheduler = $null
}

# [System.AppDomain]::CurrentDomain.add_AssemblyResolve($onAssemblyResolveEventHandler)
AssertQuartzAgent
$quartzAgentExecutableAbsolutePath = $null
$quartzAgentInstallationPath = $null
$scheduler = $null

# https://stackoverflow.com/questions/7997725/the-type-initializer-for-quartz-impl-stdschedulerfactory-threw-an-exception
# load dependant assemblies in the reverse order of their depency
Add-Type -Path (Join-Path -Path (Get-QuartzAgentInstallationPath) -ChildPath 'log4net.dll')
Add-Type -Path (Join-Path -Path (Get-QuartzAgentInstallationPath) -ChildPath 'Common.Logging.Core.dll')
Add-Type -Path (Join-Path -Path (Get-QuartzAgentInstallationPath) -ChildPath 'Common.Logging.dll')
Add-Type -Path (Join-Path -Path (Get-QuartzAgentInstallationPath) -ChildPath 'Quartz.dll')
Add-Type -Path (Join-Path -Path (Get-QuartzAgentInstallationPath) -ChildPath 'Be.Stateless.Quartz.dll')
# $error[0].Exception.InnerException

Export-ModuleMember -Alias * -Function '*-*'
