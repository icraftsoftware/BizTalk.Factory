using System.Reflection;

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG")]
[assembly: AssemblyProduct("Stateless BizTalk Factory (Debug Build)")]
#else
[assembly: AssemblyConfiguration("RELEASE")]
[assembly: AssemblyProduct("Stateless BizTalk Factory (Release Build)")]
#endif

[assembly: AssemblyCompany("Stateless.Be")]
[assembly: AssemblyCopyright("Copyright © Stateless.Be 2012")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyTrademark("Stateless BizTalk Factory")]
