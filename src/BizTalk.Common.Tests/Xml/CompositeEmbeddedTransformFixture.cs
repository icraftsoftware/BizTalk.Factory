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

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Schema;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.BizTalk.Unit.Transform;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Xml
{
	[TestFixture]
	public class CompositeEmbeddedTransformFixture : ClosedTransformFixture<CompositeEmbeddedTransform>
	{
		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void InvalidTransformResultThrows()
		{
			using (var stream = ResourceManager.Load("Data.BatchContent.xml"))
			{
				var setup = Given.Message(stream).Transform.OutputsXml().ConformingTo<Any>().WithStrictConformanceLevel();
				Assert.That(() => setup.Execute(), Throws.InstanceOf<XmlSchemaValidationException>());
			}
		}

		[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
		public override bool TryResolveXsltPath(out string path)
		{
			if (typeof(CompositeEmbeddedTransform).TryResolveBtmClassSourceFilePath(out path))
			{
				path = Path.Combine(Path.GetDirectoryName(path), @"Data\CompositeEmbeddedTransform.xsl");
				return File.Exists(path);
			}
			path = null;
			return false;
		}
	}
}
