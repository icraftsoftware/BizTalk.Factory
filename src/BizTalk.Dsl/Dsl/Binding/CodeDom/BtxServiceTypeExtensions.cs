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

			var @namespace = new CodeNamespace(type.Namespace);
			@namespace.Imports.Add(new CodeNamespaceImport(typeof(Action<>).Namespace));
			@namespace.Imports.Add(new CodeNamespaceImport(typeof(GeneratedCodeAttribute).Namespace));
			@namespace.Imports.Add(new CodeNamespaceImport(typeof(OrchestrationBindingBase<>).Namespace));
			@namespace.Imports.Add(new CodeNamespaceImport(type.Namespace));
			var @class = new CodeTypeDeclaration(type.Name + ORCHESTRATIONBINDING_SUFFIX) {
				IsClass = true,
				IsPartial = true,
				TypeAttributes = TypeAttributes.NotPublic
			};
			@class.BaseTypes.Add(new CodeTypeReference(typeof(OrchestrationBindingBase<>).Name, new CodeTypeReference(type.Name)));
			@class.CustomAttributes.Add(
				new CodeAttributeDeclaration(
					new CodeTypeReference(typeof(GeneratedCodeAttribute).Name),
					new CodeAttributeArgument(new CodePrimitiveExpression(_assemblyName.Name)),
					new CodeAttributeArgument(new CodePrimitiveExpression(_assemblyName.Version.ToString()))
					));
			@namespace.Types.Add(@class);

			// default constructor
			@class.Members.Add(new CodeConstructor { Attributes = MemberAttributes.Public });

			// constructor that accepts a bindingConfigurator delegate
			// public ctor(Action<ProcessOrchestrationBinding> orchestrationBindingConfigurator)
			// {
			//    orchestrationBindingConfigurator(this);
			//    ((ISupportValidation) this).Validate();
			// }
			var constructor = new CodeConstructor { Attributes = MemberAttributes.Public };
			constructor.Parameters.Add(
				new CodeParameterDeclarationExpression(
					new CodeTypeReference(typeof(Action<>).Name, new CodeTypeReference(@class.Name)),
					"orchestrationBindingConfigurator"));
			constructor.Statements.Add(
				new CodeDelegateInvokeExpression(
					new CodeVariableReferenceExpression("orchestrationBindingConfigurator"),
					new CodeThisReferenceExpression()));
			constructor.Statements.Add(
				new CodeMethodInvokeExpression(
					new CodeCastExpression(typeof(ISupportValidation), new CodeThisReferenceExpression()),
					"Validate"));
			@class.Members.Add(constructor);

			// property for each logical port to bind
			((PortInfo[]) Reflector.GetField(type, "_portInfo"))
				// filter out direct ports
				.Where(p => p.FindAttribute(typeof(DirectBindingAttribute)) == null)
				.Each(
					port => @class.Members.Add(
						new CodeMemberField {
							Attributes = MemberAttributes.Public,
							// C# auto property CodeDom hack http://stackoverflow.com/questions/13679171/how-to-generate-empty-get-set-statements-using-codedom-in-c-sharp
							Name = port.Name + " { get; set; } //",
							Type = new CodeTypeReference(port.Polarity == Polarity.uses ? typeof(ISendPort).Name : typeof(IReceivePort).Name),
						}));

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

		internal const string ORCHESTRATIONBINDING_SUFFIX = "OrchestrationBinding";
		private static readonly AssemblyName _assemblyName = Assembly.GetExecutingAssembly().GetName();

		private static readonly string[] _referencedAssemblyNames = {
			"Microsoft.XLANGs.BizTalk.Engine, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"Microsoft.XLANGs.BizTalk.ProcessInterface, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"Microsoft.XLANGs.Engine, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
		};
	}
}
