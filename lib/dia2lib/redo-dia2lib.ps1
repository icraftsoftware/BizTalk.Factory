# https://github.com/gimelfarb/ProductionStackTrace/tree/master/Lib/dia2lib
# https://stackoverflow.com/questions/8365849/making-a-strongly-named-com-interop-assembly-with-a-pfx-file

Switch-VisualStudioEnvironment -Version 2013

# Compile Microsoft Debug Interface Access SDK's IDL to TLB
$idl = Resolve-Path -Path (Join-Path -Path $env:VS120COMNTOOLS -ChildPath '..\..\DIA SDK\idl\dia2.idl')
$inc = Resolve-Path -Path (Join-Path -Path $env:VS120COMNTOOLS -ChildPath '..\..\DIA SDK\include')
midl.exe /tlb dia2lib.tlb /I "$inc" "$idl"

# Convert TLB to Assembly
TlbImp.exe --% dia2lib.tlb /delaysign /publickey:"..\..\src\be.stateless.public.snk" /out:net40\dia2lib.dll /namespace:Microsoft.Dia
# strongly sign assembly if private key is found
if (Test-Path -Path ..\..\src\be.stateless.snk) {
   sn.exe -R net40\dia2lib.dll "..\..\src\be.stateless.snk"
}

# Cleanup files no longer needed
Get-ChildItem -Path dia2.h, dia2_i.c, dia2_p.c, dlldata.c, dia2lib.tlb | Remove-Item
