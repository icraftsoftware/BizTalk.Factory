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
    GUID                  = 'e0e8722e-7c45-4b85-8ab6-f7878383045c'
    Author                = 'François Chabot'
    CompanyName           = 'be.stateless'
    Copyright             = '(c) 2012 - 2015 be.stateless. All rights reserved.'
    Description           = 'Commands to build, deploy, and configure BizTalk Server applications via BizTalk Deployment Framework scripts.'
    ModuleToProcess       = 'Btdf.psm1'
    ModuleVersion         = '2.0'
    ProcessorArchitecture = 'None'
    PowerShellVersion     = '4.0'
    RequiredModules       = @('Psx', 'MSBuild')

    AliasesToExport       = @('*')
    CmdletsToExport       = @()
    FunctionsToExport     = @('Build-BizTalkApplication', 'Install-BizTalkApplication', 'Uninstall-BizTalkApplication', 'Update-BizTalkApplication')
    VariablesToExport     = @()
}