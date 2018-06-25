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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Be.Stateless.Linq.Extensions;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.XLANGs.Core;

namespace Be.Stateless.BizTalk.Dsl.Binding.CodeDom
{
	internal static class CodeTypeDeclarationExtensions
	{
		internal static void AddDefaultConstructor(this CodeTypeDeclaration @class)
		{
			if (@class == null) throw new ArgumentNullException("class");
			if (!@class.IsClass) throw new ArgumentException("class");
			@class.Members.Add(new CodeConstructor { Attributes = MemberAttributes.Public });
		}

		internal static void AddBindingConfigurationConstructor(this CodeTypeDeclaration @class)
		{
			if (@class == null) throw new ArgumentNullException("class");
			if (!@class.IsClass) throw new ArgumentException("class");
			@class.AddBindingConfigurationConstructor(@class);
		}

		internal static void AddBindingConfigurationConstructor(this CodeTypeDeclaration @class, CodeTypeDeclaration bindingConfigurationType)
		{
			if (@class == null) throw new ArgumentNullException("class");
			if (!@class.IsClass) throw new ArgumentException("class");
			if (bindingConfigurationType == null) throw new ArgumentNullException("bindingConfigurationType");

			// constructor that accepts a bindingConfigurator delegate
			// public ctor(Action<IProcessOrchestrationBinding> orchestrationBindingConfigurator)
			// {
			//    orchestrationBindingConfigurator(this);
			//    ((ISupportValidation) this).Validate();
			// }
			var constructor = new CodeConstructor { Attributes = MemberAttributes.Public };
			constructor.Parameters.Add(
				new CodeParameterDeclarationExpression(
					new CodeTypeReference(typeof(Action<>).Name, new CodeTypeReference(bindingConfigurationType.Name)),
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
		}

		internal static void AddGeneratedCodeAttribute(this CodeTypeDeclaration type)
		{
			if (type == null) throw new ArgumentNullException("type");

			var generatedCodeAttribute = new CodeAttributeDeclaration(
				new CodeTypeReference(typeof(GeneratedCodeAttribute).Name),
				new CodeAttributeArgument(new CodePrimitiveExpression(_assemblyName.Name)),
				new CodeAttributeArgument(new CodePrimitiveExpression(_assemblyName.Version.ToString())));
			type.CustomAttributes.Add(generatedCodeAttribute);
		}

		internal static void AddPortPropertyMember(this CodeTypeDeclaration @interface, PortInfo port)
		{
			if (@interface == null) throw new ArgumentNullException("interface");
			if (!@interface.IsInterface) throw new ArgumentException("type");
			if (port == null) throw new ArgumentNullException("port");

			// property for each port to bind
			@interface.Members.Add(
				new CodeMemberProperty {
					Attributes = MemberAttributes.Public,
					Name = port.Name,
					HasGet = true,
					HasSet = true,
					Type = new CodeTypeReference(port.Polarity == Polarity.uses ? typeof(ISendPort).Name : typeof(IReceivePort).Name)
				});
		}

		internal static void AddPortPropertyMember(this CodeTypeDeclaration @class, PortInfo port, CodeTypeDeclaration bindingConfigurationInterface)
		{
			if (@class == null) throw new ArgumentNullException("class");
			if (!@class.IsClass) throw new ArgumentException("type");
			if (port == null) throw new ArgumentNullException("port");
			if (bindingConfigurationInterface == null) throw new ArgumentNullException("bindingConfigurationInterface");
			if (!bindingConfigurationInterface.IsInterface) throw new ArgumentException("bindingConfigurationInterface");

			// explicit property for each port to bind
			//private ISendPort _SendPort;
			//ISendPort IProcessOrchestrationBinding.SendPort
			//{
			//  get { return this._SendPort; }
			//  set { this._SendPort = value; }
			//}
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
				Type = new CodeTypeReference(port.Polarity == Polarity.uses ? typeof(ISendPort).Name : typeof(IReceivePort).Name),
				PrivateImplementationType = new CodeTypeReference(bindingConfigurationInterface.Name)
			};
			@class.Members.Add(property);
		}

		[SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
		internal static void AddPortOperationMember(this CodeTypeDeclaration @class, PortInfo port)
		{
			if (@class == null) throw new ArgumentNullException("class");
			if (!@class.IsClass) throw new ArgumentException("class");
			if (port == null) throw new ArgumentNullException("port");

			//nested class for each port and its operation names
			//#region Nested Type: SendPort
			//public struct SendPort
			//{
			//  public struct Operations
			//  {
			//    public struct SendOperation
			//    {
			//      public static string Name = "SendOperation";
			//    }
			//  }
			//}
			//#endregion
			var portStructure = new CodeTypeDeclaration(port.Name) {
				TypeAttributes = TypeAttributes.Public,
				IsStruct = true,
			};
			portStructure.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Nested Type: " + port.Name));
			portStructure.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, string.Empty));
			@class.Members.Add(portStructure);

			var operationCollectionStructure = new CodeTypeDeclaration("Operations") {
				TypeAttributes = TypeAttributes.Public,
				IsStruct = true,
			};
			portStructure.Members.Add(operationCollectionStructure);

			port.Operations.Each(
				operation => {
					var operationStructure = new CodeTypeDeclaration(operation.Name) {
						TypeAttributes = TypeAttributes.Public,
						IsStruct = true,
					};
					operationCollectionStructure.Members.Add(operationStructure);
					var nameField = new CodeMemberField {
						Name = "Name",
						Attributes = MemberAttributes.Public | MemberAttributes.Static,
						InitExpression = new CodePrimitiveExpression(operation.Name),
						Type = new CodeTypeReference(typeof(string))
					};
					operationStructure.Members.Add(nameField);
				});
		}

		internal const string ORCHESTRATION_BINDING_TYPE_NAME_SUFFIX = "OrchestrationBinding";
		private static readonly AssemblyName _assemblyName = Assembly.GetExecutingAssembly().GetName();
	}
}
