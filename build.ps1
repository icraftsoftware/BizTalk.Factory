# ensure InvokeBuild module is available
if (-not(Get-Module -ListAvailable | Where-Object Name -eq InvokeBuild)) {
   Install-Module -Name InvokeBuild -Force -Scope CurrentUser
}
if ($?) {
   Invoke-Build -File .\.build\BizTalk.Factory.build.ps1 -Task .
}
