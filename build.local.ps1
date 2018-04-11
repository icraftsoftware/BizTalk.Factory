# ensure InvokeBuild module is available
if (-not(Get-Module InvokeBuild -ListAvailable)) {
   Install-Module -Name InvokeBuild -Force -Scope CurrentUser
   Import-Module InvokeBuild
}
if ($?) {
   # see https://github.com/nightroman/Invoke-Build/blob/master/Tasks/Inline/app1.ps1
   Invoke-Build . {
      # import build tasks so as to override and void the AssignBuildNumber task
      .\.build\BizTalk.Factory.build.ps1
      Task AssignBuildNumber { }
      # SYNOPSIS: Build BizTalk.Factory with the same build number and without committing the build tools into nor tagging the Git repository
      Task . Clean, Build, PackAll, Export
   }
}
