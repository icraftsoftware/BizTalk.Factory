#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using Be.Stateless.Linq.Extensions;
using Be.Stateless.Reflection;
using Microsoft.BizTalk.XLANGs.BTXEngine;
using Microsoft.CSharp;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.XLANGs.Core;

namespace Be.Stateless.BizTalk.Dsl.Binding.CodeDom
{
	internal static class BtxServiceTypeExtensions
	{
		public static CodeCompileUnit ConvertToOrchestrationBindingCodeCompileUnit(this Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			if (!typeof(BTXService).IsAssignableFrom(type)) throw new ArgumentException(string.Format("{0} is not an orchestration type.", type.FullName), "type");

			var ports = (PortInfo[]) Reflector.GetField(type, "_portInfo");
			var unboundPorts = ports
				// filter out direct ports
				.Where(p => p.FindAttribute(typeof(DirectBindingAttribute)) == null)
				.ToArray();

			var @namespace = new CodeNamespace(type.Namespace);
			@namespace.ImportNamespace(typeof(Action<>));
			@namespace.ImportNamespace(typeof(GeneratedCodeAttribute));
			@namespace.ImportNamespace(typeof(OrchestrationBindingBase<>));
			@namespace.ImportNamespace(typeof(PortInfo));
			@namespace.ImportNamespace(type);

			if (unboundPorts.Any())
			{
				var @interface = @namespace.AddBindingInterface(type);
				unboundPorts.Each(port => @interface.AddPortPropertyMember(port));
				var @class = @namespace.AddBindingClass(type, @interface);
				ports.Each(port => @class.AddPortOperationMember(port));
				@class.AddDefaultConstructor();
				@class.AddBindingConfigurationConstructor(@interface);
				unboundPorts.Each(port => @class.AddPortPropertyMember(port, @interface));
			}
			else
			{
				var @class = @namespace.AddBindingClass(type);
				ports.Each(port => @class.AddPortOperationMember(port));
				@class.AddDefaultConstructor();
				@class.AddBindingConfigurationConstructor();
			}

			var compileUnit = new CodeCompileUnit();
			compileUnit.Namespaces.Add(@namespace);
			return compileUnit;
		}

		internal static Assembly CompileToDynamicAssembly(this Type type)
		{
			var compileUnit = type.ConvertToOrchestrationBindingCodeCompileUnit();

			using (var provider = new CSharpCodeProvider())
			{
				var parameters = new CompilerParameters {
					GenerateInMemory = true,
					IncludeDebugInformation = true
				};
				parameters.ReferencedAssemblies.Add(typeof(GeneratedCodeAttribute).Assembly.Location);
				_referencedAssemblyNames.Each(n => parameters.ReferencedAssemblies.Add(Assembly.ReflectionOnlyLoad(n).Location));
				parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
				parameters.ReferencedAssemblies.Add(type.Assembly.Location);

				var results = provider.CompileAssemblyFromDom(parameters, compileUnit);
				if (results.Errors.Count > 0) throw new Exception(results.Errors.Cast<CompilerError>().Aggregate(string.Empty, (k, e) => k + "\r\n" + e.ToString()));

				return results.CompiledAssembly;
			}
		}

		private static readonly string[] _referencedAssemblyNames = {
			"Microsoft.XLANGs.BizTalk.Engine, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"Microsoft.XLANGs.BizTalk.ProcessInterface, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"Microsoft.XLANGs.Engine, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
		};
	}
}
