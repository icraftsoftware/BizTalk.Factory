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
using Be.Stateless.BizTalk.Xml;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	internal class DebuggerSupportingEmbeddedXmlResolver : EmbeddedXmlResolver
	{
		public DebuggerSupportingEmbeddedXmlResolver(Type transform) : base(transform) { }

		#region Base Class Member Overrides

		public override Uri ResolveUri(Uri baseUri, string relativeUri)
		{
			var uri = new Uri(relativeUri, UriKind.RelativeOrAbsolute);
			if (uri.Scheme == MAP_SCHEME && uri.Host == TYPE_HOST)
			{
				var typeName = Uri.UnescapeDataString(uri.Segments[1]);
				var type = Type.GetType(typeName, true);
				string sourceXsltFilePath;
				if (type.TryResolveXsltPath(out sourceXsltFilePath))
				{
					return new Uri(Uri.UriSchemeFile + "://" + sourceXsltFilePath);
				}
			}

			// TODO rewrite to file uri for embedded resource as well
			//if (uri.Scheme == EMBEDDED_SCHEME && uri.Host == RESOURCE_HOST)
			//{
			//	return uri;
			//}

			return base.ResolveUri(baseUri, relativeUri);
		}

		#endregion
	}
}
