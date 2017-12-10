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
