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

using System;
using System.Xml.Xsl;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.BizTalk.Xml.Xsl;
using Castle.Core.Internal;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	internal class DebuggerSupportingXslCompiledTransformDescriptor : XslCompiledTransformDescriptor
	{
		public DebuggerSupportingXslCompiledTransformDescriptor(Type transform, string sourceXsltFilePath) : base(transform)
		{
			if (sourceXsltFilePath.IsNullOrEmpty()) throw new ArgumentNullException("sourceXsltFilePath");
			XslCompiledTransform = new XslCompiledTransform(true);
			XslCompiledTransform.Load(sourceXsltFilePath, XsltSettings.TrustedXslt, new EmbeddedXmlResolver(transform));
		}
	}
}
