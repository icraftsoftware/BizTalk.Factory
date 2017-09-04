#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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

using System.Xml;
using System.Xml.Xsl;
using Be.Stateless.BizTalk.Transform;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Xml.Xsl
{
	/// <summary>
	/// Wrapper for the <see cref="System.Xml.Xsl.XslCompiledTransform"/> equivalent of a <see
	/// cref="TransformBase"/>-derived transform.
	/// </summary>
	public class XslCompiledTransformDescriptor
	{
		/// <summary>
		/// Create a <see cref="XslCompiledTransformDescriptor"/> instance that wraps the <see
		/// cref="System.Xml.Xsl.XslCompiledTransform"/> equivalent of a <see cref="TransformBase"/>-derived transform.
		/// </summary>
		/// <param name="builder">
		/// An <see cref="XslCompiledTransformDescriptorBuilder"/> that knows how to build the various constituents of the
		/// <see cref="XslCompiledTransformDescriptor"/> for a given <see cref="TransformBase"/>-derived transform.
		/// </param>
		public XslCompiledTransformDescriptor(XslCompiledTransformDescriptorBuilder builder)
		{
			Arguments = builder.BuildXsltArgumentList();
			ExtensionRequirements = builder.BuildExtensionRequirements();
			if ((ExtensionRequirements & ExtensionRequirements.MessageContext) == ExtensionRequirements.MessageContext)
			{
				NamespaceResolver = builder.BuildNamespaceResolver();
			}
			XslCompiledTransform = builder.BuildXslCompiledTransform();
		}

		/// <summary>
		/// The cloneable <see cref="T:Be.Stateless.Xml.Xsl.XsltArgumentList"/> equivalent of the <see
		/// cref="TransformBase"/>-derived <see cref="TransformBase.TransformArgs"/>.
		/// </summary>
		/// <remarks>
		/// Relying on the cloneable <see cref="T:Be.Stateless.Xml.Xsl.XsltArgumentList"/> allows a <see
		/// cref="XslCompiledTransformDescriptor"/> not to keep a reference on a <see cref="TransformBase"/> instance.
		/// </remarks>
		public Stateless.Xml.Xsl.XsltArgumentList Arguments { get; private set; }

		/// <summary>
		/// Requirements of a <see cref="XslCompiledTransform"/> in terms of extension objects.
		/// </summary>
		public ExtensionRequirements ExtensionRequirements { get; private set; }

		/// <summary>
		/// The <see cref="IXmlNamespaceResolver"/> that will be passed to the <see cref="BaseMessageContextFunctions"/>
		/// extension object.
		/// </summary>
		/// <remarks>
		/// The <see cref="IXmlNamespaceResolver"/> resolver will automatically resolve all of the namespaces declared in
		/// the stylesheet markup of <see cref="XslCompiledTransform"/> and is therefore suited to help <see
		/// cref="BaseMessageContextFunctions"/> extensions objects to resolve the namespace prefix of the XML Qualified
		/// name passed to <see cref="BaseMessageContextFunctions.Read"/>.
		/// </remarks>
		public IXmlNamespaceResolver NamespaceResolver { get; private set; }

		/// <summary>
		/// The <see cref="System.Xml.Xsl.XslCompiledTransform"/> equivalent of <see cref="TransformBase"/>-derived
		/// transform.
		/// </summary>
		public XslCompiledTransform XslCompiledTransform { get; private set; }
	}
}
