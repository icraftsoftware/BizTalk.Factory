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

using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Xsl;
using Be.Stateless.BizTalk.Unit.Resources;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Xml
{
	[TestFixture]
	public class EmbeddedXmlResolverFixture
	{
		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ResolveImportedAndIncludedXslt()
		{
			using (var reader = XmlReader.Create(ResourceManager.Load("Data.CompositeTransform.xsl")))
			{
				Assert.That(
					() => new XslCompiledTransform().Load(reader, XsltSettings.TrustedXslt, new EmbeddedXmlResolver(typeof(CompositeTransform))),
					Throws.Nothing);
			}
		}
	}
}
