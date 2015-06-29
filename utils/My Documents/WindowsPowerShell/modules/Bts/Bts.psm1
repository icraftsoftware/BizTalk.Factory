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

#region Microsoft BizTalk Server (Assert/Test)

<#
.SYNOPSIS
    Ensures Microsoft BizTalk Server is installed locally.
.DESCRIPTION
    This command will throw if Microsoft BizTalk Server is not installed locally and will silently complete otherwise.
.EXAMPLE
    PS> Assert-BizTalkServer
.EXAMPLE
    PS> Assert-BizTalkServer -Verbose
    With the -Verbose switch, this command will confirm Microsoft BizTalk Server is installed.
.NOTES
    © 2012 be.stateless.
#>
function Assert-BizTalkServer
{
    [CmdletBinding()]
    param()

    if (-not(Test-BizTalkServer)) {
        throw "Microsoft BizTalk Server is not installed!"
    }
    Write-Verbose "Microsoft BizTalk Server is installed."
}

<#
.SYNOPSIS
    Returns whether Microsoft BizTalk Server is installed locally.
.DESCRIPTION
    This command will return $true if the Microsoft BizTalk Server is installed locally.
.EXAMPLE
    PS> Test-BizTalkServer
.NOTES
    © 2012 be.stateless.
#>
function Test-BizTalkServer
{
    [CmdletBinding()]
    param()

    $path = 'HKLM:\SOFTWARE\Microsoft\BizTalk Server\3.0\'
    [bool]((Test-Path $path) -and ((Get-ChildItem $path).Count -gt 0))
}

#endregion

#region BizTalk PowerShell Provider (Add/Assert/Test)

<#
.SYNOPSIS
    Adds the BizTalk PowerShell Provider snap-in to the current session.
.DESCRIPTION
    Adds the BizTalk PowerShell Provider snap-in to the current session and set up BizTalk drives. While setting up the BizTalk drives, the actual location of the BizTalk management database will be discovered through the registry keys and not assumed to be local.
.EXAMPLE
    PS> Add-BizTalkProvider
.EXAMPLE
    PS> Add-BizTalkProvider -Verbose
    With the -Verbose switch, this command will confirm the BizTalk PowerShell Provider snap-in has been added to the current session.
.NOTES
    © 2012 be.stateless.
#>
function Add-BizTalkProvider
{
    [CmdletBinding()]
    param()

    if (-not(Test-BizTalkProvider)) {
        # check among the Windows PowerShell snap-ins that have been registered on this computer
        $registeredSnapin = Get-PSSnapin -Registered | ? { $_.Name -eq "BizTalkFactory.PowerShell.Extensions" }
        if ($registeredSnapin -eq $null) {
            throw "BizTalk PowerShell Provider snap-in is not installed on this computer!"
        }

        Write-Verbose "Adding BizTalk PowerShell Provider snap-in to the current session..."
        # see http://psbiztalk.codeplex.com/workitem/4575, otherise it won't work if MgmtDb is not local
        $InitializeDefaultBTSDrive = $false
        Add-PSSnapIn BizTalkFactory.PowerShell.Extensions

        Write-Verbose "Adding BizTalk PowerShell Drive Provider to the current session..."
        # allow to get to the MgmtDbServer & the MgmtDbName
        $btsAdministration = Get-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\BizTalk Server\3.0\Administration'
        New-PSDrive -Name BizTalk `
            -PSProvider BizTalk `
            -Root BizTalk:\ `
            -Scope Global `
            -Instance $btsAdministration.MgmtDBServer `
            -Database $btsAdministration.MgmtDBName | Out-Default

        # create BizTalk drive's accessor functions
        if (-not (Test-Path 'function:global:BizTalk:')) {
            New-Item -Name 'global:BizTalk:' -Path function: -Value { Set-Location "BizTalk:" } | Out-Default
            New-Item -Name 'global:BizTalk:\' -Path function: -Value { Set-Location "BizTalk:\" } | Out-Default
        }

        Write-Verbose "BizTalk PowerShell Provider snap-in has been added to the current session."

        # register clean up handler should the module be removed from the session
        $MyInvocation.MyCommand.ScriptBlock.Module.OnRemove = {
            Remove-Item -Path function:global:BizTalk:
            Remove-Item -Path function:global:BizTalk:\
            Remove-PSDrive -Name BizTalk
            Remove-PSSnapin BizTalkFactory.PowerShell.Extensions
        }
    }
}

<#
.SYNOPSIS
    Ensures the BizTalk PowerShell Provider snap-in has been added to the current session.
.DESCRIPTION
    This command will throw if the BizTalk PowerShell Provider snap-in has not been added to current session and will silently complete otherwise.
.PARAMETER Force
    Whether to add the BizTalk PowerShell Provider snap-in to the current session if it is not already the case.
.EXAMPLE
    PS> Assert-BizTalkProvider
.EXAMPLE
    PS> Test-BizTalkProvider -Force
.EXAMPLE
    PS> Assert-BizTalkProvider -Verbose
    With the -Verbose switch, this command will confirm the BizTalk PowerShell Provider snap-in has been added to the current session.
.LINK
    http://psbiztalk.codeplex.com/
.NOTES
    © 2012 be.stateless.
#>
function Assert-BizTalkProvider
{
    [CmdletBinding()]
    param(
        [switch]
        $Force
    )

    if (-not(Test-BizTalkProvider)) {
        if (-not($Force)) {
            throw "BizTalk PowerShell Provider snap-in is required to run this Cmdlet!"
        }
        Add-BizTalkProvider
    }
}

<#
.SYNOPSIS
    Returns whether the BizTalk PowerShell Provider snap-in has been added to the current session.
.DESCRIPTION
    This command will return $true if the BizTalk PowerShell Provider snap-in has been added to the current session, or $false otherwise.
.EXAMPLE
    PS> Test-BizTalkProvider
.LINK
    http://psbiztalk.codeplex.com/
.NOTES
    © 2012 be.stateless.
#>
function Test-BizTalkProvider
{
    [CmdletBinding()]
    param()

    # BizTalk PowerShell Provider requires a 32 bit process
    Assert-32bitProcess

    # check among the Windows PowerShell snap-ins that have been added to the current session
    $snapin = Get-PSSnapin | ? { $_.Name -eq "BizTalkFactory.PowerShell.Extensions" }
    [bool]($snapin -ne $null)
}

#endregion

#region BizTalk Server Host (New/Update)

<#
.SYNOPSIS
    Creates a new BizTalk Server host.
.DESCRIPTION
    Creates and configures a new BizTalk Server host.
.PARAMETER Name
    The name of the BizTalk host.
.PARAMETER Type
    The type of the BizTalk host, either InProcess or Isolated.
.PARAMETER Group
    The Windows group used to control access of this host.
.PARAMETER x86
    Whether instances of this host will be 32-bit only processes.
.PARAMETER Default
    Whether this host is to be the default host in the BizTalk Server group or not.
.PARAMETER Tracking
    Wheter to enable the BizTalk Tracking component for this host or not.
.PARAMETER Trusted
    Whether BizTalk should trust this host or not.
.EXAMPLE
    PS> New-BizTalkHost -Name 'Transmit Host' -Type InProcess -Group 'BizTalk Application Users'
.EXAMPLE
    PS> New-BizTalkHost -Name 'Transmit Host' -Type InProcess -Group 'BizTalk Application Users' -x86
.EXAMPLE
    PS> New-BizTalkHost -Name 'Transmit Host' -Type InProcess -Group 'BizTalk Application Users' -x86:$false
.EXAMPLE
    PS> New-BizTalkHost -Name 'Transmit Host' -Type InProcess -Group 'BizTalk Application Users' -Verbose
.EXAMPLE
    PS> New-BizTalkHost -Name 'Transmit Host' -Type InProcess -Group 'BizTalk Application Users' -WhatIf
.LINK
    http://psbiztalk.codeplex.com/Thread/View.aspx?ThreadId=232498
.NOTES
    © 2012 be.stateless.
#>
function New-BizTalkHost
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Mandatory=$true)]
        [string]
        $Name,

        [Parameter(Mandatory=$true)]
        [ValidateSet('InProcess','Isolated')]
        [string]
        $Type,

        [Parameter(Mandatory=$true)]
        [string]
        $Group,

        [switch]
        $x86,

        [switch]
        $Default,

        [switch]
        $Tracking,

        [switch]
        $Trusted
    )
    Assert-BizTalkProvider -Force
    try {
        Push-Location 'BizTalk:\Platform Settings\Hosts'
        if (Test-Path $Name) {
            Write-Host "`t '$Name' host already exists."
        }
        elseif ($PsCmdlet.ShouldProcess("BizTalk Group", "Creating $Type '$Name' host")) {
            Write-Verbose "`t Creating $Type '$Name' host with '$Group' Windows group..."
            New-Item -Path:$Name -HostType:$Type -NtGroupName:$Group -AuthTrusted:$Trusted | Out-Default
            Set-ItemProperty -Path:$Name -Name:Is32BitOnly -Value:$x86 | Out-Default
            if ($Default) { Set-ItemProperty -Path:$Name -Name:IsDefault -Value:$Default | Out-Default }
            Set-ItemProperty -Path:$Name -Name:HostTracking -Value:$Tracking | Out-Default
            Write-Host "`t $Type '$Name' host has been created."
        }
    }
    finally {
        Pop-Location
    }
}

<#
.SYNOPSIS
    Updates the configuration settings of a BizTalk Server host.
.DESCRIPTION
    Updates the configuration settings of a BizTalk Server host.
.PARAMETER Name
    The name of the BizTalk host.
.PARAMETER x86
    Whether instances of this host will be 32-bit only processes.
.PARAMETER Default
    Whether this host is to be the default host in the BizTalk group or not.
.PARAMETER Tracking
    Wheter to enable the BizTalk Tracking component for this host or not.
.PARAMETER Trusted
    Whether BizTalk should trust this host or not.
.EXAMPLE
    PS> Update-BizTalkHost -Name 'Transmit Host' -x86 -Verbose
    With the -Verbose switch, this command will confirm this process is 32 bit.
.EXAMPLE
    PS> Update-BizTalkHost -Name 'Transmit Host' -x86 -Verbose -WhatIf
.EXAMPLE
    PS> Update-BizTalkHost -Name 'Transmit Host' -x86:$false -Verbose
    With the -Verbose switch, this command will confirm this process is not 32 bit.
.NOTES
    © 2012 be.stateless.
#>
function Update-BizTalkHost
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Mandatory=$true)]
        [string]
        $Name,

        [bool]
        $x86,

        [switch]
        $Default,

        [bool]
        $Tracking,

        [bool]
        $Trusted
    )
    Assert-BizTalkProvider -Force
    try {
        Push-Location 'BizTalk:\Platform Settings\Hosts'

        # ensure host exists as it'll throw otherwise
        ($btsHost = Get-Item -Path $Name) | Out-Default

        if ($PSBoundParameters.ContainsKey('x86')) {
            $subject = "'$Name' host's 32-bit only restriction"
            Set-BizTalkHostProperty -Name $Name -Property Is32BitOnly -Value $x86 `
                -ActionToPerform ("{1} {0}" -f $Subject, (?: { $x86 } { 'Enabling' } { 'Disabling' } )) `
                -PerformedAction ("{0} has been {1}" -f $Subject, (?: { $x86 } { 'enabled' } { 'disabled' } ))
        }

        if($Default.IsPresent -and -not $btsHost.IsDefault) {
            Set-BizTalkHostProperty -Name $Name -Property IsDefault -Value $Default `
                -ActionToPerform "Setting '$Name' host as default BizTalk Group host" `
                -PerformedAction "'$Name' host has been set as default BizTalk Group host"
        }

        if ($PSBoundParameters.ContainsKey('Tracking')) {
            $subject = "'$Name' host's Tracking capability"
            Set-BizTalkHostProperty -Name $Name -Property HostTracking -Value $Tracking `
                -ActionToPerform ("{1} {0}" -f $Subject, (?: { $Tracking } { 'Enabling' } { 'Disabling' } )) `
                -PerformedAction ("{0} has been {1}" -f $Subject, (?: { $Tracking } { 'enabled' } { 'disabled' } ))
        }

        if ($PSBoundParameters.ContainsKey('Trusted')) {
            $subject = "'$Name' host's Trusted Authentication"
            Set-BizTalkHostProperty -Name $Name -Property AuthTrusted -Value $Trusted `
                -ActionToPerform ("{1} {0}" -f $Subject, (?: { $Trusted } { 'Enabling' } { 'Disabling' } )) `
                -PerformedAction ("{0} has been {1}" -f $Subject, (?: { $Trusted } { 'enabled' } { 'disabled' } ))
        }
    }
    finally {
        Pop-Location
    }
}

<#
 # Private Helper Functions
 #>
 function Set-BizTalkHostProperty
 {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]
        $Name,

        [Parameter(Mandatory=$true)]
        [string]
        $Property,

        [Parameter(Mandatory=$true)]
        [object]
        $Value,

        [Parameter(Mandatory=$true)]
        [string]
        $ActionToPerform,

        [Parameter(Mandatory=$true)]
        [string]
        $PerformedAction
    )

    $original = Get-ItemProperty -Path $Name -Name $Property
    if ($original -ne $value -and $PsCmdlet.ShouldProcess("BizTalk Group", $ActionToPerform)) {
        Write-Verbose "`t $ActionToPerform..."
        Set-ItemProperty -Path $Name -Name $Property -Value $Value | Out-Default
        Write-Verbose "`t $PerformedAction."
    }
}

#endregion

#region BizTalk Server host instance (Disable/Enable/New/Restart/Start/Stop)

<#
.SYNOPSIS
    Disables a BizTalk Server host instance.
.DESCRIPTION
    Disables a BizTalk Server host instance.
.PARAMETER Name
    The name of the BizTalk host instance to disable.
.PARAMETER Server
    The server on which run the host instance to disable.
.EXAMPLE
    PS> Disable-BizTalkHostInstance -Name 'Transmit Host Instance'
    Disables the BizTalk Server host instance named 'Transmit Host Instance' on the local machine.
.EXAMPLE
    PS> Disable-BizTalkHostInstance -Name 'Transmit Host Instance' -Server 'BizTalkBox'
    Disables the BizTalk Server host instance named 'Transmit Host Instance' on the machine named 'BizTalkBox'.
.NOTES
    © 2012 be.stateless.
#>
function Disable-BizTalkHostInstance
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Mandatory=$true)]
        [string]
        $Name,

        [string]
        $Server = $Env:COMPUTERNAME
    )
    Assert-BizTalkProvider -Force
    try {
        Push-Location 'BizTalk:\Platform Settings\Host Instances'
        $hostInstancePath = "*$Name $Server*"
        $hostInstance = Get-Item $hostInstancePath
        if ($PsCmdlet.ShouldProcess("BizTalk Group", "Disabling '$Name' host instance on '$Server' server")) {
            Write-Verbose "`t '$Name' host instace on '$Server' server is being disabled..."
            Set-ItemProperty -Path $hostInstance.Name -Name IsDisabled -Value $true
            Write-Verbose "`t '$Name' host instace on '$Server' server has been disabled."
        }
    }
    finally {
        Pop-Location
    }
}

<#
.SYNOPSIS
    Enables a BizTalk Server host instance.
.DESCRIPTION
    Enables a BizTalk Server host instance.
.PARAMETER Name
    The name of the BizTalk host instance to enable.
.PARAMETER Server
    The server on which run the host instance to enable.
.EXAMPLE
    PS> Enable-BizTalkHostInstance -Name 'Transmit Host Instance'
    Enables the BizTalk Server host instance named 'Transmit Host Instance' on the local machine.
.EXAMPLE
    PS> Enable-BizTalkHostInstance -Name 'Transmit Host Instance' -Server 'BizTalkBox'
    Enables the BizTalk Server host instance named 'Transmit Host Instance' on the machine named 'BizTalkBox'.
.NOTES
    © 2012 be.stateless.
#>
function Enable-BizTalkHostInstance
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Mandatory=$true)]
        [string]
        $Name,

        [string]
        $Server = $Env:COMPUTERNAME
    )
    Assert-BizTalkProvider -Force
    try {
        Push-Location 'BizTalk:\Platform Settings\Host Instances'
        $hostInstancePath = "*$Name $Server*"
        $hostInstance = Get-Item $hostInstancePath
        if ($PsCmdlet.ShouldProcess("BizTalk Group", "Enabling '$Name' host instance on '$Server' server")) {
            Write-Verbose "`t '$Name' host instace on '$Server' server is being enabled..."
            Set-ItemProperty -Path $hostInstance.Name -Name IsDisabled -Value $false
            Write-Verbose "`t '$Name' host instace on '$Server' server has been enabled."
        }
    }
    finally {
        Pop-Location
    }
}

<#
.SYNOPSIS
    Creates a new BizTalk Server host instance.
.DESCRIPTION
    Creates and configures a new BizTalk Server host instance.
.PARAMETER Name
    The name of the BizTalk host instance.
.PARAMETER User
    The user name, or logon, of the windows account that the host intance to create will use to run.
.PARAMETER Password
    The password of the windows account that the host intance to create will use to run.
.PARAMETER Server
    The server on which will run the host instance to create.
.PARAMETER Started
    Whether to start this host instance upon creation.
.EXAMPLE
    PS> New-BizTalkHost -Name 'Transmit Host' -Type InProcess -Group 'BizTalk Application Users'
.EXAMPLE
    PS> New-BizTalkHost -Name 'Transmit Host' -Type InProcess -Group 'BizTalk Application Users' -Verbose
.EXAMPLE
    PS> New-BizTalkHost -Name 'Transmit Host' -Type InProcess -Group 'BizTalk Application Users' -WhatIf
.EXAMPLE
    PS> New-BizTalkHost -Name 'Transmit Host' -Trusted:$false
.NOTES
    © 2012 be.stateless.
#>
function New-BizTalkHostInstance
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Mandatory=$true)]
        [string]
        $Name,

        [Parameter(Mandatory=$true)]
        [string]
        $User,

        [Parameter(Mandatory=$true)]
        [string]
        $Password,

        [string]
        $Server = $Env:COMPUTERNAME,

        [switch]
        $Started
    )
    Assert-BizTalkProvider -Force
    try {
        Push-Location 'BizTalk:\Platform Settings\Host Instances'
        $hostInstancePath = "*$Name $Server*"
        if (Test-Path $hostInstancePath) {
            Write-Host "`t '$Name' host instance on '$Server' server already exists."
        }
        elseif ($PsCmdlet.ShouldProcess("BizTalk Group", "Creating '$Name' host instance on '$Server' server")) {
            Write-Verbose "`t Creating '$Name' host instance on '$Server' server..."
            $credentials = New-Object -TypeName System.Management.Automation.PSCredential `
                -ArgumentList $User, (ConvertTo-SecureString $Password -AsPlainText -Force)
            New-Item -Path HostInstance -hostName $Name -RunningServer $Server -Credentials $credentials | Out-Default
            Write-Host "`t '$Name' host instance on '$Server' server has been created."
# TODO pass MinIOThreads, MaxIOThreads, MinWorkerThreads, MaxWorkerThreads as arguments
#      typed as array, e.g. @(50, 1000) for both min and max, or @(50) for just min, or @(,1000) for just max
#      set max iff max > current max or min iff min < current min unless -Force switch
#            Initialize-BizTalkHostInstanceThreading -Name $Name -Server $Server
            if ($Started) {
                Start-BizTalkHostInstance -Name $Name -Server $Server
            } else {
                Stop-BizTalkHostInstance -Name $Name -Server $Server
            }
        }
    }
    finally {
        Pop-Location
    }
}

<#
.SYNOPSIS
    Restarts a running BizTalk Server host instance.
.DESCRIPTION
    Restarts a running BizTalk Server host instance. Unless the -Force switch is passed, this command has no effect if the host instance to restart is not already running. In other words, unless the -Force switch is passed, this command will never start a host instance that is not running.
.PARAMETER Name
    The name of the BizTalk host instance to restart.
.PARAMETER Server
    The server on which run the host instance to restart.
.PARAMETER Force
    Force a non running host instance to start.
.EXAMPLE
    PS> Restart-BizTalkHostInstance -Name 'Transmit Host Instance'
    Restarts the BizTalk Server host instance named 'Transmit Host Instance' on the local machine.
.EXAMPLE
    PS> Restart-BizTalkHostInstance -Name 'Transmit Host Instance' -Force -Server 'BizTalkBox'
    Restarts or start the BizTalk Server host instance named 'Transmit Host Instance' on the machine named 'BizTalkBox'.
.NOTES
    © 2012 be.stateless.
#>
function Restart-BizTalkHostInstance
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Mandatory=$true)]
        [string]
        $Name,

        [string]
        $Server = $Env:COMPUTERNAME,

        [switch]
        $Force
    )
    Assert-BizTalkProvider -Force
    try {
        Push-Location 'BizTalk:\Platform Settings\Host Instances'
        $hostInstancePath = "*$Name $Server*"
        $hostInstance = Get-Item $hostInstancePath
        if ($PsCmdlet.ShouldProcess("BizTalk Group", "Restarting '$Name' host instance on '$Server' server")) {
            if ($Force -or $hostInstance.ServiceState -match "Running|Sart Pending") {
                Write-Verbose "`t '$Name' host instace on '$Server' server is being restarted..."
                BizTalkFactory.PowerShell.Extensions\Restart-HostInstance -Path $hostInstancePath
                Write-Verbose "`t '$Name' host instace on '$Server' server has been restarted."
            } else {
                Write-Verbose "`t '$Name' host instace on '$Server' server does not need to be restarted as it is not started."
            }
        }
    }
    finally {
        Pop-Location
    }
}

<#
.SYNOPSIS
    Starts a BizTalk Server host instance.
.DESCRIPTION
    Starts a BizTalk Server host instance.
.PARAMETER Name
    The name of the BizTalk host instance to start.
.PARAMETER Server
    The server on which run the host instance to start.
.EXAMPLE
    PS> Start-BizTalkHostInstance -Name 'Transmit Host Instance'
    Starts the BizTalk Server host instance named 'Transmit Host Instance' on the local machine.
.EXAMPLE
    PS> Start-BizTalkHostInstance -Name 'Transmit Host Instance' -Server 'BizTalkBox'
    Starts the BizTalk Server host instance named 'Transmit Host Instance' on the machine named 'BizTalkBox'.
.NOTES
    © 2012 be.stateless.
#>
function Start-BizTalkHostInstance
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Mandatory=$true)]
        [string]
        $Name,

        [string]
        $Server = $Env:COMPUTERNAME
    )
    if ($PsCmdlet.ShouldProcess("BizTalk Group", "Starting '$Name' host instance on '$Server' server")) {
        Assert-BizTalkProvider -Force
        $hostInstancePath = "*$Name $Server*"
        Write-Verbose "`t '$Name' host instace on '$Server' server is being started..."
        BizTalkFactory.PowerShell.Extensions\Start-HostInstance -Path $hostInstancePath
        Write-Verbose "`t '$Name' host instace on '$Server' server has been started."
    }
}

<#
.SYNOPSIS
    Stops a BizTalk Server host instance.
.DESCRIPTION
    Stops a BizTalk Server host instance.
.PARAMETER Name
    The name of the BizTalk host instance to stop.
.PARAMETER Server
    The server on which run the host instance to stop.
.EXAMPLE
    PS> Stop-BizTalkHostInstance -Name 'Transmit Host Instance'
    Stops the BizTalk Server host instance named 'Transmit Host Instance' on the local machine.
.EXAMPLE
    PS> Stop-BizTalkHostInstance -Name 'Transmit Host Instance' -Server 'BizTalkBox'
    Stops sthe BizTalk Server host instance named 'Transmit Host Instance' on the machine named 'BizTalkBox'.
.NOTES
    © 2012 be.stateless.
#>
function Stop-BizTalkHostInstance
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Mandatory=$true)]
        [string]
        $Name,

        [string]
        $Server = $Env:COMPUTERNAME
    )
    if ($PsCmdlet.ShouldProcess("BizTalk Group", "Stopping '$Name' host instance on '$Server' server")) {
        Assert-BizTalkProvider -Force
        $hostInstancePath = "*$Name $Server*"
        Write-Verbose "`t '$Name' host instace on '$Server' server is being stopped..."
        BizTalkFactory.PowerShell.Extensions\Stop-HostInstance -Path $hostInstancePath
        Write-Verbose "`t '$Name' host instace on '$Server' server has been stopped."
    }
}

<#
 # Private Helper Functions
 #>
# set CLR Hosting parameters in accordance with http://msdn.microsoft.com/en-us/library/aa561380.aspx
# but the values given there are obsolete and the one used here have been given by a Microsoft PFE
function Initialize-BizTalkHostInstanceThreading
{
    [CmdletBinding()]
    param(
        [string]
        $Name,

        [string]
        $Server = $Env:COMPUTERNAME
    )

    Assert-BizTalkProvider -Force
    try {
        Push-Location 'BizTalk:\Platform Settings\Host Instances'
        $hostInstancePath = "*$Name $Server*"
        # ensure host instance exists and is not an Isolated host
        if ((Test-Path $hostInstancePath) -and (Get-Item -Path $hostInstancePath).HostType -ne 'Isolated') {
            Write-Verbose "`t Configuring \\$Server\$Name host instance's CLR threading..."
            $hklm = [Microsoft.Win32.RegistryKey]::OpenRemoteBaseKey('LocalMachine', $Server)
            $serviceKey = $hklm.OpenSubKey(('SYSTEM\CurrentControlSet\Services\BTSSvc$' + $Name), $true)
            $clrKey = $serviceKey.CreateSubKey('CLR Hosting')
            $restartNeeded = $false
            if ($clrKey.GetValue('MaxIOThreads') -lt 1000) {
                $restartNeeded = $true
                $clrKey.SetValue('MaxIOThreads', 1000, 'dword')
            }
            if ($clrKey.GetValue('MaxWorkerThreads') -lt 500) {
                $restartNeeded = $true
                $clrKey.SetValue('MaxWorkerThreads', 500, 'dword')
            }
            if ($clrKey.GetValue('MinIOThreads') -lt 50) {
                $restartNeeded = $true
                $clrKey.SetValue('MinIOThreads', 50, 'dword')
            }
            if ($clrKey.GetValue('MinWorkerThreads') -lt 50) {
                $restartNeeded = $true
                $clrKey.SetValue('MinWorkerThreads', 50, 'dword')
            }
            Write-Verbose "`t \\$Server\$Name host instace's CLR threading has been configured."
            if ($restartNeeded) {
                Restart-BizTalkHostInstance -Name $Name -Server $Server
            }
        }
    }
    finally {
        if ($clrKey -ne $null) {
            $clrKey.Close()
        }
        if ($serviceKey -ne $null) {
            $serviceKey.Close()
        }
        if ($hklm -ne $null) {
            $hklm.Close()
        }
        Pop-Location
    }
}

#endregion

#region BizTalk Server Adapter Handler (New/Remove)

<#
.SYNOPSIS
    Creates a BizTalk Server adapter handler.
.DESCRIPTION
    Creates and configures a BizTalk Server adapter handler.
.PARAMETER Adapter
    The name of the adapter for which a handler has to be created.
.PARAMETER Host
    The name of the host that will run the handler to be created.
.PARAMETER Direction
    The diretion of the handler to be created, either Receive or Send.
.PARAMETER Default
    Whether the handler to be created will be the default adapter's handler.
.EXAMPLE
    PS> New-BizTalkAdapterHandler -Adapter
.NOTES
    © 2012 be.stateless.
#>
function New-BizTalkAdapterHandler
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Mandatory=$true)]
        [string]
        $Adapter,

        [Parameter(Mandatory=$true)]
        [string]
        $Host,

        [Parameter(Mandatory=$true)]
        [ValidateSet('Receive','Send')]
        [string]
        $Direction,

        [switch]
        $Default
    )
    Assert-BizTalkProvider -Force
    try {
        Push-Location 'BizTalk:\Platform Settings\Adapters'
        if (Test-Path $Adapter) {
            Set-Location $Adapter | Out-Default
            $handlerPath = "$Adapter $Direction Handler ($Host)"
            if (Test-Path $handlerPath) {
                Write-Host "`t $Direction $Adapter handler for '$Host' host already exists."
            }
            elseif ($PsCmdlet.ShouldProcess("BizTalk Group", "Creating $Direction $Adapter handler for '$Host' host")) {
                Write-Verbose "`t Creating $Direction $Adapter handler for '$Host' host...";
                New-Item -Path '.\Dummy' -HostName $Host -Direction $Direction | Out-Default
                Write-Host "`t $Direction $Adapter handler for '$Host' host has been created."
                if ($Default -and $Direction -match "Send") {
                    ($handler = Get-Item -Path $handlerPath) | Out-Default
                    if (-not $handler.Default) {
                        Write-Verbose "`t Setting $Direction $Adapter handler for '$Host' host as default..."
                        Set-ItemProperty -Path $handler.Name -Name Default -Value $true | Out-Default
                        Write-Verbose "`t $Direction $Adapter handler for '$Host' host has been set as default."
                    }
                }
            }
        }
        else {
            Write-Warning "$Adapter adapter does not exist."
        }
    }
    finally {
        Pop-Location
    }
}

<#
.SYNOPSIS
    Removes a BizTalk Server adapter handler.
.DESCRIPTION
    Removes a BizTalk Server adapter handler.
.PARAMETER Adapter
    The name of the adapter for which a handler has to be removed.
.PARAMETER Host
    The name of the host that will run the handler to be removed.
.PARAMETER Direction
    The diretion of the handler to be removed, either Receive or Send.
.EXAMPLE
    PS> Remove-BizTalkAdapterHandler -Adapter
.NOTES
    © 2012 be.stateless.
#>
function Remove-BizTalkAdapterHandler
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Mandatory=$true)]
        [string]
        $Adapter,

        [Parameter(Mandatory=$true)]
        [string]
        $Host,

        [Parameter(Mandatory=$true)]
        [ValidateSet('Receive','Send')]
        [string]
        $Direction
    )
    Assert-BizTalkProvider -Force
    try {
        Push-Location 'BizTalk:\Platform Settings\Adapters'
        if (Test-Path $Adapter) {
            Set-Location $Adapter | Out-Default
            $handlerPath = "$Adapter $Direction Handler ($Host)"
            if (-not (Test-Path $handlerPath)) {
                Write-Warning "`t $Direction $Adapter handler for '$Host' host does not exist."
            }
            elseif ($PsCmdlet.ShouldProcess("BizTalk Group", "Removing $Direction $Adapter handler for '$Host' host")) {
                Write-Verbose "`t Removing $Direction $Adapter handler for '$Host' host...";
                Remove-Item -Path $handlerPath | Out-Default
                Write-Host "`t $Direction $Adapter handler for '$Host' host has been removed."
            }
        }
        else {
            Write-Warning "$Adapter adapter does not exist."
        }
    }
    finally {
        Pop-Location
    }
}

#endregion

#region BizTalk Server Application Bindings (Update)

<#
.SYNOPSIS
    Updates the hosts and handlers of a BizTalk Server application.
.DESCRIPTION
    Updates the hosts bound to the Orchestrations of a BizTalk Server application, as well as the handlers bound to both its Receveive Locations and Send Ports. This command has no effects if it has been determined that the hosts and handlers are already up to date.
    To update the hosts and handlers of a BizTalk Server application, the command follows a three-step process: it first exports the application bindings, then updates the hosts and handlers in these bindings, and finally imports them afterwards.
.PARAMETER Application
    The name of the BizTalk Server application whose hosts need to be updated.
.PARAMETER OrchestrationHostUpdates
    The host updates that need to be carried on over the Orchestrations belonging to the BizTalk Server application.
.PARAMETER ReceiveHandlerUpdates
    The receive handler updates that need to be carried on over the Receive Locations belonging to the BizTalk Server application.
.PARAMETER SendHandlerUpdates
    The send handlers updates that need to be carried on over the Send Ports belonging to the BizTalk Server application.


.PARAMETER Started
    Wether the application has to be started or not, i.e. stopped, after its bindings have been updated.
.EXAMPLE
    PS> Update-BizTalkApplicationBindings -Application
.NOTES
    OrchestrationHostUpdates, ReceiveHandlerUpdates, and SendHandlerUpdates
    arguments must all be arrays of tuples, where each tuple has the following
    members:

    - Host: is a string giving the name of new host;

    - DeprecatedHosts: is an array of strings giving the names of the hosts
      that needs to be deprecated in favor of the new one;

    - Adapters: is an array of strings giving the names of the adapters for
      which handlers have to have their host(s) be deprecated. This last member
      is required for both ReceiveHostUpdates and SendHostUpdates, and unused
      for OrchestrationHostUpdates.

      Here is a sample of such a tuple, a triplet:
      @{ `
        Host = 'BiMI_PxHost'; `
        DeprecatedHosts = @('BizTalkServerApplication', 'BizTalkServerIsolated'); `
        Adapters = @('HTTP', 'SOAP') `
      }

    © 2012 be.stateless.
#>
function Update-BizTalkApplicationHosts
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param(
        [Parameter(Mandatory=$true)]
        [string]
        $Application,

        [object[]]
        $OrchestrationHostUpdates,

        [object[]]
        $ReceiveHandlerUpdates,

        [object[]]
        $SendHandlerUpdates,

        [switch]
        $Started
    )
    Assert-BizTalkProvider -Force
    try {
        $hasChanges = $OrchestrationHostUpdates.Length -gt 0 `
            -or $ReceiveHandlerUpdates.Length -gt 0 `
            -or $SendHandlerUpdates -gt 0

        Push-Location "BizTalk:\Applications"
        if ($hasChanges -and (Test-Path $Application)) {
            $bindingsFile = "$env:TEMP\$Application Bindings.{0}.xml"
            Write-Host "`t Analysing '$Application' application's hosts and handlers..."

            Write-Verbose "`t`t Exporting bindings..."
            $originalBindingsFile = $bindingsFile -f 'original'
            Export-Bindings -Path $Application -Destination $originalBindingsFile | Out-Default
            Write-Verbose "`t`t Bindings have been exported."
            # in-memory copy of the XML bindings to update
            $bindings = [xml](Get-Content $originalBindingsFile)
            $dirty = $false

            # Updating orchestration hosts
            Write-Verbose "`t Analysing '$Application' application's hosts bound to Orchestrations..."
            foreach($tuple in $OrchestrationHostUpdates) {
                foreach($deprecatedHost in $tuple.DeprecatedHosts) {
                    $nodes = $bindings.SelectNodes("//ModuleRef//Service/Host[@Name='$deprecatedHost']")
                    foreach($node in $nodes) {
                        $node.SetAttribute("Name", $tuple.Host)
                        $dirty = $true
                    }
                }
            }
            # Updating receive handlers
            Write-Verbose "`t Analysing '$Application' application's handlers bound to Receive Locations..."
            foreach($tuple in $ReceiveHandlerUpdates) {
                $adapterNames = ';' + (($tuple.Adapters | % { $_ }) -join ';') + ';'
                foreach($deprecatedHost in $tuple.DeprecatedHosts) {
                    $predicate = "@Name='$deprecatedHost' and contains('$adapterNames', concat(';', TransportType/@Name, ';'))"
                    $nodes = $bindings.SelectNodes("//ReceiveHandler[$predicate]")
                    foreach($node in $nodes) {
                        $node.SetAttribute("Name", $tuple.Host)
                        $dirty = $true
                    }
                }
            }
            # Updating send handlers
            Write-Verbose "`t Analysing '$Application' application's handlers bound to Send Ports..."
            foreach($tuple in $SendHandlerUpdates) {
                $adapterNames = ';' + (($tuple.Adapters | % { $_ }) -join ';') + ';'
                foreach($deprecatedHost in $tuple.DeprecatedHosts) {
                    $predicate = "@Name='$deprecatedHost' and contains('$adapterNames', concat(';', TransportType/@Name, ';'))"
                    $nodes = $bindings.SelectNodes("//SendHandler[$predicate]")
                    foreach($node in $nodes) {
                        $node.SetAttribute("Name", $tuple.Host)
                        $dirty = $true
                    }
                }
            }

            if($dirty) {
                $updatedBindingsFile = $bindingsFile -f 'updated'
                $bindings.Save($updatedBindingsFile)
                if ($PsCmdlet.ShouldProcess("BizTalk Group", "Updating '$Application' application's hosts and handlers")) {
                    Write-Host "`t Updating '$Application' application's hosts and handlers..."
                    Write-Verbose "`t`t Importing updated bindings..."

#TODO add a confirm switch (or may be already there thanks to SupportsShouldProcess=$true

                    # stop app so as to unenlist ports which must be done prior to import updated bindings
                    Stop-Application -Path $Application -StopOption StopAll | Out-Default
                    Import-Bindings -Path $Application -Source $updatedBindingsFile | Out-Default
                    Write-Verbose "`t`t Updated bindings have been imported."
                    Write-Host "`t '$Application' application's bindings have been updated."
                }
            }
            else {
                Write-Host "`t '$Application' application's hosts and handlers are already up to date."
            }


#TODO keep application in the same state as before patching, that is keep it stopped if it were so, or restart only what was already started (orch, SP and RL)


            if ($started) {
                Write-Verbose "`t`t Starting '$Application' application..."
                Start-Application -Path $Application | Out-Default
                Write-Verbose "`t`t '$Application' application has been started."
            } else {
                Write-Verbose "`t`t Stopping '$Application' application..."
                Stop-Application -Path $Application -StopOption StopAll | Out-Default
                Write-Verbose "`t`t '$Application' application has been stopped."
            }
        }
    }
    finally {
        Pop-Location
    }
}

#endregion

#region BizTalk Server Receive Locations (Resume/Suspend)

<#
.SYNOPSIS
    Starts all BizTalk Server receive locations that were stopped via the Suspend-ReceiveLocations command.
.DESCRIPTION
    Starts all BizTalk Server receive locations that were stopped via the Suspend-ReceiveLocations command.
.EXAMPLE
    PS> Restart-BizTalkReceiveLocations
.NOTES
    © 2012 be.stateless.
#>
function Resume-BizTalkReceiveLocations
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param()

#TODO use workflow for these 2 commands and save snapshot of enabled RL in the workflow state

    if(-not(Test-Path (Join-Path $Env:TEMP "EnabledReceiveLocations.xml"))) {
        throw 'The list of previously enabled receive locations was not found.'
    }
    Write-Host 'Re-enabling all receive locations.'
    $enabledReceiveLocations = Import-Clixml -Path (Join-Path $Env:TEMP "EnabledReceiveLocations.xml")
    $enabledReceiveLocations | % { Enable-ReceiveLocation $_ }
    Write-Host 'All receive locations have been re-enabled.'
}

<#
.SYNOPSIS
    Stops all BizTalk Server receive locations that are enabled.
.DESCRIPTION
    Stops all BizTalk Server receive locations that are enabled and keep a record of which ones were enabled, or in other words, need to be stopped.
.EXAMPLE
    PS> Stop-BizTalkReceiveLocations
.NOTES
    © 2012 be.stateless.
#>
function Suspend-BizTalkReceiveLocations
{
    [CmdletBinding(SupportsShouldProcess=$true)]
    param()

#TODO use workflow for these 2 commands and save snapshot of enabled RL in the workflow state

    Write-Host 'Retrieving the currently enabled receive locations.'
    $enabledReceiveLocations = Get-ChildItem 'BizTalk:\Applications\*\Receive Locations\*' -exclude 'BizTalk*' | ? { $_.Enable } | % { $_.PSPath }
    Export-Clixml -Path (Join-Path $Env:TEMP "EnabledReceiveLocations.xml") -InputObject $enabledReceiveLocations
    Write-Host 'Disabling all receive locations.'
    $enabledReceiveLocations | % { Disable-ReceiveLocation $_ }
    Write-Host 'All receive locations have been disabled.'
}

#endregion

#region BizTalk Server Group (Start/Stop)

# TODO function Start-BizTalkGroup {
#    Write-Host 'Retrieving the BizTalk servers in the BizTalk group...'
#    $computerNames = Get-WmiObject -Namespace 'root\MicrosoftBizTalkServer' `
#        -Class 'MSBTS_Server' | % { $_.Name }
#
#    Write-Host 'Starting the Enterprise SSO service on all BizTalk servers in the group...'
#    $services = $computerNames | % { Get-Service -Name entsso -ComputerName $_ }
#    $services | % { $_.Start() }
#    Write-Host 'The Enterprise SSO service has been started on all BizTalk servers in the group.'
#
#    Write-Host "Starting the 'BizTalk Services' IIS AppPool on all BizTalk servers in the group..."
#    $appPools = Get-WmiObject -Namespace 'root\MicrosoftIISv2' `
#        -Class 'IISApplicationPool' `
#        -ComputerName $computerNames `
#        -Authentication PacketPrivacy | ? { $_.Name -match '/BizTalk Services' }
#    $appPools | % { $_.Start() }
#    Write-Host "The 'BizTalk Services' IIS AppPool has been started on all servers in the group."
#
#    Write-Host 'Starting the host instances on all BizTalk servers in the group...'
#    $hostIntances = ls 'BizTalk:\Platform Settings\Host Instances'
#    $hostIntances | % { BizTalkFactory.PowerShell.Extensions\Start-HostInstance -Path $_.PSPath }
#    Write-Host 'All host instances have been started on all BizTalk servers in the group.'
#}

# TODO function Stop-BizTalkGroup {
#    Write-Host 'Retrieving the BizTalk servers in the BizTalk group...'
#    $computerNames = Get-WmiObject -Namespace 'root\MicrosoftBizTalkServer' `
#        -Class 'MSBTS_Server' | % { $_.Name }
#
#    Write-Host "Stopping the 'BizTalk Services' IIS AppPool on all BizTalk servers in the group..."
#    $appPools = Get-WmiObject -Namespace 'root\MicrosoftIISv2' `
#        -Class 'IISApplicationPool' `
#        -ComputerName $computerNames `
#        -Authentication PacketPrivacy | ? { $_.Name -match '/BizTalk Services' }
#    $appPools | % { $_.Stop() }
#    Write-Host "The 'BizTalk Services' IIS AppPool has been stopped on all servers in the group."
#
#    Write-Host 'Stopping the host instances on all BizTalk servers in the group...'
#    $hostIntances = ls 'BizTalk:\Platform Settings\Host Instances'
#    $hostIntances | % { BizTalkFactory.PowerShell.Extensions\Stop-HostInstance -Path $_.PSPath }
#    Write-Host 'All host instances have been stopped on all BizTalk servers in the group.'
#
#    Write-Host 'Stopping the Enterprise SSO service on all BizTalk servers in the group...'
#    $services = $computerNames | % { Get-Service -Name entsso -ComputerName $_ }
#    $services | % { $_.Stop() }
#    Write-Host 'The Enterprise SSO service has been stopped on all BizTalk servers in the group.'
#}

#endregion

<#
 # MAIN
 #>

# Add-BizTalkProvider

# Have BizTalk Tracking tools available on path, noticeably bm.exe
$p = Get-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\BizTalk Server\3.0\' -ErrorAction SilentlyContinue `
    | Select-Object -ExpandProperty InstallPath
if ($p -ne $null) {
    $env:Path += ";$($p)Tracking"
}

Export-ModuleMember `
    -Function Add-*, Assert-*, Disable-*, Enable-*, New-*, Remove-*, Restart-*, Start-*, Stop-*, Test-*, Update-* `
    -Cmdlet @(Get-Command -Type CmdLet | ? { $_.PSSnapin -match '^BizTalkFactory.PowerShell' } | % { $_.Name })
