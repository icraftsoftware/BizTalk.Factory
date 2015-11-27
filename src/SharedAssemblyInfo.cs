#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.Reflection;
using log4net.Config;

#if DEBUG

[assembly: AssemblyConfiguration("DEBUG")]
[assembly: AssemblyProduct("be.stateless BizTalk Factory (Debug Build)")]

#else

[assembly: AssemblyConfiguration("RELEASE")]
[assembly: AssemblyProduct("be.stateless BizTalk Factory (Release Build)")]

#endif

[assembly: AssemblyCompany("be.stateless")]
[assembly: AssemblyCopyright("Copyright © 2012 - 2015 François Chabot, Yves Dierick")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyTrademark("be.stateless BizTalk Factory")]
[assembly: XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
