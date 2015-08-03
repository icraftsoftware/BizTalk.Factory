#region Copyright & License

# Copyright © 2012 - 2013 François Chabot, Yves Dierick
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

function prompt
{
    (Get-Host).UI.RawUI.WindowTitle = (Get-Location).Path + $windowTitle
    $id = (Get-History -Count 1).Id + 0
    return "${id}>"
}

function TabExpansion2
{
    [CmdletBinding(DefaultParameterSetName = 'ScriptInputSet')]
    Param(
        [Parameter(ParameterSetName = 'ScriptInputSet', Mandatory = $true, Position = 0)]
        [string] $inputScript,

        [Parameter(ParameterSetName = 'ScriptInputSet', Mandatory = $true, Position = 1)]
        [int] $cursorColumn,

        [Parameter(ParameterSetName = 'AstInputSet', Mandatory = $true, Position = 0)]
        [System.Management.Automation.Language.Ast] $ast,

        [Parameter(ParameterSetName = 'AstInputSet', Mandatory = $true, Position = 1)]
        [System.Management.Automation.Language.Token[]] $tokens,

        [Parameter(ParameterSetName = 'AstInputSet', Mandatory = $true, Position = 2)]
        [System.Management.Automation.Language.IScriptPosition] $positionOfCursor,

        [Parameter(ParameterSetName = 'ScriptInputSet', Position = 2)]
        [Parameter(ParameterSetName = 'AstInputSet', Position = 3)]
        [Hashtable] $options = $null
    )

    End
    {
        if ($options -ne $null) {
            $options += $global:options
        }
        else {
            $options = $global:options
        }

        if ($psCmdlet.ParameterSetName -eq 'ScriptInputSet') {
            return [System.Management.Automation.CommandCompletion]::CompleteInput(
                <#inputScript#>  $inputScript,
                <#cursorColumn#> $cursorColumn,
                <#options#>      $options)
        }
        else {
            return [System.Management.Automation.CommandCompletion]::CompleteInput(
                <#ast#>              $ast,
                <#tokens#>           $tokens,
                <#positionOfCursor#> $positionOfCursor,
                <#options#>          $options)
        }
    }
}

<#
 # Main
 #>

if (Test-Elevated) {
    (Get-Host).UI.RawUI.Backgroundcolor = 'Black'
    Clear-Host
}

# setup TabExpansion2 hook points
if (-not $global:options) {
    $global:options = @{ CustomArgumentCompleters = @{}; NativeArgumentCompleters = @{} }
}

# setup Set-Location shortcut functions after TFS workspaces defined locally
if (Get-VisualStudioVersionNumbers) {
    Get-Workspaces -Uri https://tfs.codeplex.com/tfs/TFS14 | New-WorkspaceShortcut
}
Set-Variable -Name windowTitle `
    -Value (' - ' + 'Windows PowerShell' + (?: { Test-32bitProcess } { ' x86' } { '' } ) + (?: { Test-Elevated } { ' (Administrator)' } { '' } )) `
    -Scope 0 `
    -Option Constant
