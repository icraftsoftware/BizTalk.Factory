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
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	internal class DebuggerSupportingTransformer : Transformer
	{
		public DebuggerSupportingTransformer(Stream[] streams, IMapCustomXsltPathResolver customXsltPathResolver) : base(streams)
		{
			_customXsltPathResolver = customXsltPathResolver;
		}

		#region Base Class Member Overrides

		/// <summary>
		/// Returns a <see cref="XslCompiledTransformDescriptor"/>-derived instance that enable XSLT debugging if the
		/// source XSLT file of the <see cref="TransformBase"/>-derived <paramref name="transform"/> is found. Returns a
		/// regular <see cref="XslCompiledTransformDescriptor"/> instance otherwise.
		/// </summary>
		/// <param name="transform">
		/// The <see cref="TransformBase"/>-derived type whose <see cref="XslCompiledTransformDescriptor"/> is looked up.
		/// </param>
		/// <returns>
		/// A <see cref="XslCompiledTransformDescriptor"/> that enable XSLT debugging if the source XSLT file has been
		/// found.
		/// </returns>
		protected override XslCompiledTransformDescriptor LookupTransformDescriptor(Type transform)
		{
			string sourceXsltFilePath;
			return _customXsltPathResolver.TryResolveXsltPath(out sourceXsltFilePath)
				// Only hit this code if a debugger is attached (see TransformFixture<T> ctor); it should therefore not be a
				// performance issue to bypass the XsltCache in order to debug an XSLT map. Besides, XsltCache would only
				// always return an XslCompiledTransformDescriptor and not a
				// DebuggerSupportingXslCompiledTransformDescriptor derived instance.
				? new DebuggerSupportingXslCompiledTransformDescriptor(transform, sourceXsltFilePath)
				: base.LookupTransformDescriptor(transform);
		}

		#endregion

		private readonly IMapCustomXsltPathResolver _customXsltPathResolver;
	}
}
