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
using System.Text;
using System.Xml;
using Be.Stateless.BizTalk.Component.Extensions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.Text.Extensions;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.MicroComponent
{
	[TestFixture]
	public class XmlTranslatorFixture : MicroPipelineComponentFixture
	{
		[Test]
		public void BuildXmlTranslationSetWithNoTranslationInContext()
		{
			var sut = new XmlTranslator {
				Translations = new XmlTranslationSet {
					Override = false,
					Items = new[] {
						new XmlNamespaceTranslation("sourceUrn1", "urn:test1"),
						new XmlNamespaceTranslation("sourceUrn5", "urn:test5")
					}
				}
			};

			Assert.That(
				sut.BuildXmlTranslationSet(MessageMock.Object),
				Is.EqualTo(
					new XmlTranslationSet {
						Override = false,
						Items = new[] {
							new XmlNamespaceTranslation("sourceUrn1", "urn:test1"),
							new XmlNamespaceTranslation("sourceUrn5", "urn:test5")
						}
					}));
		}

		[Test]
		public void BuildXmlTranslationSetWithTranslationInContext()
		{
			MessageMock.Setup(c => c.GetProperty(BizTalkFactoryProperties.XmlTranslations))
				.Returns(
					XmlTranslationSetConverter.Serialize(
						new XmlTranslationSet {
							Override = true,
							Items = new[] {
								new XmlNamespaceTranslation("sourceUrn5", "urn05")
							}
						}));

			var sut = new XmlTranslator {
				Translations = new XmlTranslationSet {
					Override = false,
					Items = new[] {
						new XmlNamespaceTranslation("sourceUrn1", "urn:test1"),
						new XmlNamespaceTranslation("sourceUrn2", "urn:test2")
					}
				}
			};

			Assert.That(
				sut.BuildXmlTranslationSet(MessageMock.Object),
				Is.EqualTo(
					new XmlTranslationSet {
						Override = true,
						Items = new[] {
							new XmlNamespaceTranslation("sourceUrn5", "urn05")
						}
					}));
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void RoundTripXmlSerialization()
		{
			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
			{
				new XmlTranslator().Serialize(writer);
			}
			using (var reader = builder.GetReaderAtContent())
			{
				Assert.That(() => reader.DeserializeMicroPipelineComponent(), Throws.Nothing);
			}
		}

		[Test]
		public void TranslationsDefaultValue()
		{
			var sut = new XmlTranslator();
			Assert.That(sut.Translations, Is.EqualTo(XmlTranslationSet.Empty));
		}
	}
}
