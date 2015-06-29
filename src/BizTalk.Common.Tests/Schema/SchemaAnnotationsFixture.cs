#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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

using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Unit.Transform;
using Be.Stateless.BizTalk.XPath;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Schema
{
	[TestFixture]
	public class SchemaAnnotationsFixture
	{
		[Test]
		public void CanReadAnnotationsFromMicrosoftSoap12Schema()
		{
			Assert.That(() => new SchemaMetadata(typeof(BTS.soap_envelope_1__2.Fault)).Annotations, Throws.Nothing);
		}

		[Test]
		public void ResolveEnvelopingMapFromEnvelopeSchemaWithAnnotations()
		{
			var annotations = new SchemaMetadata(typeof(AnnotatedSchema)).Annotations;

			Assert.That(annotations, Is.Not.TypeOf<SchemaAnnotations.EmptySchemaAnnotations>());
			Assert.That(annotations.EnvelopingMap, Is.EqualTo(typeof(IdentityTransform)));
			Assert.That(annotations.Extractors.Count, Is.EqualTo(1));
			Assert.That(
				annotations.Extractors,
				Is.EquivalentTo(
					new[] {
						new XPathExtractor(TrackingProperties.Value1.QName, "/*[local-name()='Send']/*[local-name()='Message']/*[local-name()='Id']", ExtractionMode.Write)
					}));
		}

		[Test]
		public void ResolveEnvelopingMapFromEnvelopeSchemaWithoutAnnotations()
		{
			var annotations = new SchemaMetadata(typeof(Batch.Content)).Annotations;

			Assert.That(annotations, Is.Not.TypeOf<SchemaAnnotations.EmptySchemaAnnotations>());
			Assert.That(annotations.EnvelopingMap, Is.Null);
			Assert.That(annotations.Extractors.Count, Is.EqualTo(2));
			Assert.That(
				annotations.Extractors,
				Is.EquivalentTo(
					new[] {
						new XPathExtractor(TrackingProperties.Value1.QName, "/*/*[local-name()='EnvelopeSpecName']", ExtractionMode.Write),
						new XPathExtractor(TrackingProperties.Value2.QName, "/*/*[local-name()='Partition']", ExtractionMode.Write)
					}));
		}

		[Test]
		public void ResolveEnvelopingMapFromRegularSchemaWithoutAnnotation()
		{
			var annotations = new SchemaMetadata(typeof(UnannotatedSchema)).Annotations;

			Assert.That(annotations, Is.TypeOf<SchemaAnnotations.EmptySchemaAnnotations>());
			Assert.That(annotations.EnvelopingMap, Is.Null);
		}
	}
}
