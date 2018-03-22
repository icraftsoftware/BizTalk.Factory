$assemblies = Get-ChildItem -Filter *.dll
$inGac = $assemblies | ForEach-Object { Get-ChildItem -Path "gac:\$($_.BaseName)" -ErrorAction Ignore }
$outGac = $assemblies | Where-Object { $_.BaseName -notin $inGac.Name } | ForEach-Object { Get-ChildItem -Path 'C:\Program Files (x86)\Microsoft BizTalk Server 2013 R2' -Filter $_.Name -Recurse }

$inGac | Where-Object { (Get-FileHash -Path ".\$($_.Name).dll").Hash -ne (Get-FileHash -Path $_.CodeBase.Substring(8)).Hash } |
   ForEach-Object { Copy-Item -Path $_.CodeBase.Substring(8) -Destination . } |
   ForEach-Object { Write-Information -MessageData $_.Name -InformationAction Continue }

$outGac | Where-Object { (Get-FileHash -Path ".\$($_.Name)").Hash -ne (Get-FileHash -Path "$($_.FullName)").Hash } |
   ForEach-Object { Copy-Item -Path $_.FullName -Destination . } |
   ForEach-Object { Write-Information -MessageData $_.Name -InformationAction Continue }
