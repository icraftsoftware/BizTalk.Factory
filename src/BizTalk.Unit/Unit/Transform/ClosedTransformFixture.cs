#region Copyright & License

// Copyright © 2012 - 2019 François Chabot
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Be.Stateless.BizTalk.Streaming.Extensions;
using Be.Stateless.Linq.Extensions;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	public abstract class ClosedTransformFixture<T> : IMapCustomXsltPathResolver
		where T : TransformBase, new()
	{
		protected ClosedTransformFixture()
		{
			// inject map/transform extensions around XML streams that support XSLT debugging
			if (Debugger.IsAttached)
			{
				StreamExtensions.StreamTransformerFactory = streams => new DebuggerSupportingTransformer(streams, this);
			}
			// provision namespace cache with each of the namespaces and prefixes declared in the XSLT
			using (var sr = new StringReader(new T().XmlContent))
			{
				var navigator = new XPathDocument(sr).CreateNavigator();
				navigator.MoveToFollowing(XPathNodeType.Element);
				_namespaceCache = new Dictionary<string, string>();
				navigator.GetNamespacesInScope(XmlNamespaceScope.All).Each(n => _namespaceCache.Add(n.Key, n.Value));
			}
		}

		#region IMapCustomXsltPathResolver Members

		public virtual bool TryResolveXsltPath(out string path)
		{
			return typeof(T).TryResolveCustomXsltPath(out path);
		}

		#endregion

		protected ITransformFixtureSetup Given(Action<ITransformFixtureInputSetup> inputSetupConfigurator)
		{
			var setup = new TransformFixtureInputSetup(typeof(T), _namespaceCache);
			inputSetupConfigurator(setup);
			return setup;
		}

		private readonly Dictionary<string, string> _namespaceCache;
	}
}
