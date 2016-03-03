Get-ChildItem -Filter *.dll |
   ForEach-Object { Get-ChildItem -Path "gac:\$($_.BaseName)" } |
   Where-Object { -not (Compare-Object -ReferenceObject (Get-Content -Path ".\$($_.Name).dll") -DifferenceObject (Get-Content -Path  $_.CodeBase.Substring(8))) } |
   ForEach-Object { Copy-Item -Path ($_.CodeBase.Substring(8)) -Destination . }