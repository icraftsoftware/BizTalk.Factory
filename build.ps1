# ensure InvokeBuild module is available
if (-not(Get-Module InvokeBuild -ListAvailable)) {
   Install-Module -Name InvokeBuild -Force -Scope CurrentUser
   Import-Module InvokeBuild
}
if ($?) {
   Invoke-Build -File .\.build\BizTalk.Factory.build.ps1 -Task .
}
