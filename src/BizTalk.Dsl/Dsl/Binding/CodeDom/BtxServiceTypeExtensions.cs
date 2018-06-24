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

			var ports = ((PortInfo[]) Reflector.GetField(type, "_portInfo"))
				// filter out direct ports
				.Where(p => p.FindAttribute(typeof(DirectBindingAttribute)) == null)
				.ToArray();

			var @namespace = new CodeNamespace(type.Namespace);
			@namespace.Imports.Add(new CodeNamespaceImport(typeof(Action<>).Namespace));
			@namespace.Imports.Add(new CodeNamespaceImport(typeof(GeneratedCodeAttribute).Namespace));
			@namespace.Imports.Add(new CodeNamespaceImport(typeof(OrchestrationBindingBase<>).Namespace));
			@namespace.Imports.Add(new CodeNamespaceImport(typeof(PortInfo).Namespace));
			@namespace.Imports.Add(new CodeNamespaceImport(type.Namespace));

			if (ports.Any())
			{
				var @interface = type.BuildOrchestrationBindingInterface(ports);
				@namespace.Types.Add(@interface);

				var @class = type.BuildOrchestrationBindingClass(ports, @interface);
				@namespace.Types.Add(@class);
			}
			else
			{
				var @class = type.BuildOrchestrationBindingClass(ports);
				@namespace.Types.Add(@class);
			}

			var compileUnit = new CodeCompileUnit();
			compileUnit.Namespaces.Add(@namespace);
			return compileUnit;
		}

		private static CodeAttributeDeclaration BuildGeneratedCodeAttribute()
		{
			return new CodeAttributeDeclaration(
				new CodeTypeReference(typeof(GeneratedCodeAttribute).Name),
				new CodeAttributeArgument(new CodePrimitiveExpression(_assemblyName.Name)),
				new CodeAttributeArgument(new CodePrimitiveExpression(_assemblyName.Version.ToString()))
				);
		}

		private static CodeTypeDeclaration BuildOrchestrationBindingInterface(this Type type, PortInfo[] ports)
		{
			var @interface = new CodeTypeDeclaration("I" + type.Name + ORCHESTRATION_BINDING_SUFFIX) {
				TypeAttributes = TypeAttributes.NotPublic,
				// ! IsInterface must be set last !
				IsInterface = true
			};
			@interface.BaseTypes.Add(new CodeTypeReference(typeof(IOrchestrationBinding).Name));
			@interface.CustomAttributes.Add(BuildGeneratedCodeAttribute());

			// property for each logical port to bind
			ports.Each(
				port => {
					@interface.Members.Add(
						new CodeMemberProperty {
							Attributes = MemberAttributes.Public,
							Name = port.Name,
							HasGet = true,
							HasSet = true,
							Type = new CodeTypeReference(port.Polarity == Polarity.uses ? typeof(ISendPort).Name : typeof(IReceivePort).Name)
						});
				});

			return @interface;
		}

		private static CodeTypeDeclaration BuildOrchestrationBindingClass(this Type type, PortInfo[] ports, CodeTypeDeclaration @interface = null)
		{
			var @class = new CodeTypeDeclaration(type.Name + ORCHESTRATION_BINDING_SUFFIX) {
				IsClass = true,
				IsPartial = true,
				TypeAttributes = TypeAttributes.NotPublic
			};
			@class.BaseTypes.Add(new CodeTypeReference(typeof(OrchestrationBindingBase<>).Name, new CodeTypeReference(type.Name)));
			if (@interface != null) @class.BaseTypes.Add(new CodeTypeReference(@interface.Name));
			@class.CustomAttributes.Add(BuildGeneratedCodeAttribute());

			//nested class for each port, which will expose operation names
			//#region Nested Type: SendPort
			//public struct class SendPort
			//{
			//  public struct class Operations
			//  {
			//    public struct class SendOperation
			//    {
			//      public static string Name = "SendOperation";
			//    }
			//  }
			//}
			//#endregion
			ports.Each(
				port => {
					var portStatic = new CodeTypeDeclaration(port.Name) {
						TypeAttributes = TypeAttributes.Public,
						IsStruct = true,
					};
					portStatic.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Nested Type: " + port.Name));
					portStatic.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));

					var operationCollectionStatic = new CodeTypeDeclaration("Operations") {
						TypeAttributes = TypeAttributes.Public,
						IsStruct = true,
					};
					portStatic.Members.Add(operationCollectionStatic);

					port.Operations.Each(
						operation => {
							var operationStatic = new CodeTypeDeclaration(operation.Name) {
								TypeAttributes = TypeAttributes.Public,
								IsStruct = true,
							};
							var nameField = new CodeMemberField {
								Name = "Name",
								Attributes = MemberAttributes.Public | MemberAttributes.Static,
								InitExpression = new CodePrimitiveExpression(operation.Name),
								Type = new CodeTypeReference(typeof(string))
							};
							operationStatic.Members.Add(nameField);
							operationCollectionStatic.Members.Add(operationStatic);
						});

					@class.Members.Add(portStatic);
				});

			// default constructor
			@class.Members.Add(new CodeConstructor { Attributes = MemberAttributes.Public });

			// constructor that accepts a bindingConfigurator delegate
			// public ctor(Action<IProcessOrchestrationBinding> orchestrationBindingConfigurator)
			// {
			//    orchestrationBindingConfigurator(this);
			//    ((ISupportValidation) this).Validate();
			// }
			var constructor = new CodeConstructor { Attributes = MemberAttributes.Public };
			constructor.Parameters.Add(
				new CodeParameterDeclarationExpression(
					new CodeTypeReference(typeof(Action<>).Name, new CodeTypeReference(@interface != null ? @interface.Name : @class.Name)),
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

			// explicit property for each logical port to bind
			//private ISendPort _SendPort;
			//ISendPort IProcessOrchestrationBinding.SendPort
			//{
			//  get { return this._SendPort; }
			//  set { this._SendPort = value; }
			//}
			ports.Each(
				port => {
					var field = new CodeMemberField {
						Attributes = MemberAttributes.Private,
						Name = "_" + port.Name,
						Type = new CodeTypeReference(port.Polarity == Polarity.uses ? typeof(ISendPort).Name : typeof(IReceivePort).Name)
					};
					@class.Members.Add(field);
					var property = new CodeMemberProperty {
						Attributes = MemberAttributes.Public,
						Name = port.Name,
						GetStatements = { new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name)) },
						SetStatements = {
							new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name), new CodePropertySetValueReferenceExpression())
						},
						Type = new CodeTypeReference(port.Polarity == Polarity.uses ? typeof(ISendPort).Name : typeof(IReceivePort).Name)
					};
					if (@interface != null) property.PrivateImplementationType = new CodeTypeReference(@interface.Name);
					@class.Members.Add(property);
				});

			return @class;
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

		internal const string ORCHESTRATION_BINDING_SUFFIX = "OrchestrationBinding";
		private static readonly AssemblyName _assemblyName = Assembly.GetExecutingAssembly().GetName();

		private static readonly string[] _referencedAssemblyNames = {
			"Microsoft.XLANGs.BizTalk.Engine, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"Microsoft.XLANGs.BizTalk.ProcessInterface, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
			"Microsoft.XLANGs.Engine, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
		};
	}
}
