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
using System.IO;
using Be.Stateless.BizTalk.Streaming.Extensions;
using Be.Stateless.BizTalk.Xml.Xsl;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	internal class DebuggerSupportingTransformer : Transformer
	{
		public DebuggerSupportingTransformer(Stream[] streams) : base(streams) { }

		#region Base Class Member Overrides

		protected override XslCompiledTransformDescriptor LookupTransformDescriptor(Type transform)
		{
			string sourceXsltFilePath;
			return MapCustomXsltPathResolver.TryResolveXsltPath(transform, out sourceXsltFilePath)
				// only hit this code if a debugger is attached (see TransformFixture<T> static ctor); it should therefore
				// not be a performance issue to bypass the XsltCache in order to, most certainly, debug an XSLT map.
				// besides, XsltCache would only always return an XslCompiledTransformDescriptor and not its
				// DebuggerSupportingXslCompiledTransformDescriptor derived class.
				? new DebuggerSupportingXslCompiledTransformDescriptor(transform, sourceXsltFilePath)
				: base.LookupTransformDescriptor(transform);
		}

		#endregion
	}
}
