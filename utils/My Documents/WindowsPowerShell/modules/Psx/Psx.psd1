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

@{
    GUID                  = '217de01f-f2e1-460a-99a4-b8895d0dd071'
    Author                = 'François Chabot'
    CompanyName           = 'be.stateless'
    Copyright             = '(c) 2012 - 2015 be.stateless. All rights reserved.'
    Description           = 'Useful PowerShell function helpers.'
    ModuleToProcess       = 'Psx.psm1'
    ModuleVersion         = '1.0'
    ProcessorArchitecture = 'None'
    PowerShellVersion     = '3.0'
    RequiredModules       = @('Pscx')

    AliasesToExport       = @('*')
    CmdletsToExport       = @()
    FunctionsToExport     = @('Assert-*', 'Get-*', 'Test-*')
    VariablesToExport     = @()
}