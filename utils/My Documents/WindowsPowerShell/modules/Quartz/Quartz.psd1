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

@{
    GUID                  = '0e87bb44-c022-4578-b7ee-ca5b3932c3df'
    Author                = 'François Chabot'
    CompanyName           = 'be.stateless'
    Copyright             = '(c) 2012 - 2017 be.stateless. All rights reserved.'
    Description           = 'Quartz.NET Commands.'
    ModuleToProcess       = 'Quartz.psm1'
    ModuleVersion         = '1.0'
    ProcessorArchitecture = 'None'
    PowerShellVersion     = '4.0'
    RequiredModules       = @()

    AliasesToExport       = @()
    CmdletsToExport       = @()
    FunctionsToExport     = @('*-Quartz*')
    VariablesToExport     = @()
}