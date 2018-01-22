#region Copyright & License

# Copyright © 2012 - 2018 François Chabot
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

function global:Get-ProjectItem {
   [CmdletBinding()]
   [OutputType([System.IO.FileSystemInfo[]])]
   param(
      [Parameter(Position = 0, Mandatory = $false)]
      [psobject]
      $Path = '..\src',

      [Parameter(Position = 1, Mandatory = $true, ValueFromPipeline = $true)]
      [string[]]
      $Project,

      [Parameter(Position = 2, Mandatory = $true)]
      [ValidateSet('Debug', 'Release')]
      [string]
      $Configuration,

      [Parameter(Position = 3, Mandatory = $false)]
      [string[]]
      $Include = @('*.dll', '*.exe')
   )
   process {
      @(
         $Project | ForEach-Object -Process {
            $item = Get-ChildItem -Path ([System.IO.Path]::Combine($Path, $_, 'bin', $Configuration)) `
               -Filter "Be.Stateless.$_.*" `
               -Include $Include -Recurse | Select-Object -ExpandProperty FullName
            if ($item -eq $null -or $item.Length -eq 0) {
               throw "Project item not found [Path: '$Path', Project: '$Project', Configuration: '$Configuration', Include = '$Include']"
            }
            $item
         }
      )
   }
}
