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

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.Extensions;
using Be.Stateless.Xml.Builder;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.Xml
{
	[TestFixture]
	public class XmlBuilderReaderFixture
	{
		#region Setup/Teardown

		[OneTimeSetUp]
		public void TestFixtureSetUp()
		{
			IdentityTransform = new XslCompiledTransform(true);
			using (var xmlReader = XmlReader.Create(ResourceManager.Load("Be.Stateless.Xml.Xsl.XsltIdentity.xslt")))
			{
				IdentityTransform.Load(xmlReader);
			}
		}

		#endregion

		[Test]
		public void DisposeIXmlElementBuilder()
		{
			var builderMock = new Mock<IDisposable>();
			using (new XmlBuilderReader(builderMock.As<IXmlElementBuilder>().Object)) { }
			builderMock.Verify(b => b.Dispose());
		}

		[Test]
		[TestCaseSource(typeof(XmlBuilderTestCasesFactory), "XmlElementBuilders")]
		public void XmlReaderConformanceOnOuterXml(XmlElementBuilder builder, string expected)
		{
			AssertOuterXmlConformance(new XmlBuilderReader(builder), expected.IsNullOrEmpty() ? EmptyXmlReader.Create() : XmlReader.Create(new StringReader(expected)));
			AssertOuterXmlConformance(expected.IsNullOrEmpty() ? EmptyXmlReader.Create() : XmlReader.Create(new StringReader(expected)), new XmlBuilderReader(builder));
			AssertOuterXmlContent(new XmlBuilderReader(builder), expected.IsNullOrEmpty() ? EmptyXmlReader.Create() : XmlReader.Create(new StringReader(expected)));
			AssertOuterXmlInvocations(new XmlBuilderReader(builder), expected.IsNullOrEmpty() ? EmptyXmlReader.Create() : XmlReader.Create(new StringReader(expected)));
		}

		[Test]
		[TestCaseSource(typeof(XmlBuilderTestCasesFactory), "XmlElementBuilders")]
		public void XmlReaderConformanceOnRead(XmlElementBuilder builder, string expected)
		{
			AssertReadConformance(new XmlBuilderReader(builder), expected.IsNullOrEmpty() ? EmptyXmlReader.Create() : XmlReader.Create(new StringReader(expected)));
			AssertReadConformance(expected.IsNullOrEmpty() ? EmptyXmlReader.Create() : XmlReader.Create(new StringReader(expected)), new XmlBuilderReader(builder));
		}

		[Test]
		[TestCaseSource(typeof(XmlBuilderTestCasesFactory), "XmlElementBuilders")]
		public void XmlReaderConformanceOnTransform(XmlElementBuilder builder, string expected)
		{
			AssertTransformConformance(new XmlBuilderReader(builder), expected.IsNullOrEmpty() ? EmptyXmlReader.Create() : XmlReader.Create(new StringReader(expected)));
			AssertTransformConformance(expected.IsNullOrEmpty() ? EmptyXmlReader.Create() : XmlReader.Create(new StringReader(expected)), new XmlBuilderReader(builder));
			AssertTransformResult(new XmlBuilderReader(builder), expected.IsNullOrEmpty() ? EmptyXmlReader.Create() : XmlReader.Create(new StringReader(expected)));
			AssertTransformInvocations(new XmlBuilderReader(builder), expected.IsNullOrEmpty() ? EmptyXmlReader.Create() : XmlReader.Create(new StringReader(expected)));
		}

		private static XslCompiledTransform IdentityTransform { get; set; }

		private static void AssertOuterXmlConformance(XmlReader actual, XmlReader expected)
		{
			Assert.That(
				() => {
					using (var verifier = new XmlReaderConformanceVerifier(actual, expected))
					{
						verifier.MoveToContent();
						verifier.ReadOuterXml();
					}
				},
				Throws.Nothing);
		}

		private static void AssertOuterXmlContent(XmlReader actual, XmlReader expected)
		{
			using (actual)
			using (expected)
			{
				actual.MoveToContent();
				expected.MoveToContent();
				Assert.That(actual.ReadOuterXml(), Is.EqualTo(expected.ReadOuterXml()));
			}
		}

		private static void AssertOuterXmlInvocations(XmlReader actual, XmlReader expected)
		{
			var actualSpy = new XmlReaderSpy(actual);
			var expectedSpy = new XmlReaderSpy(expected);
			using (actualSpy)
			using (expectedSpy)
			{
				actualSpy.MoveToContent();
				expectedSpy.MoveToContent();
				actualSpy.ReadOuterXml();
				expectedSpy.ReadOuterXml();
			}
			Assert.That(actualSpy.Invocations, Is.EqualTo(expectedSpy.Invocations));
		}

		private static void AssertReadConformance(XmlReader actual, XmlReader expected)
		{
			Assert.That(
				() => {
					using (var verifier = new XmlReaderConformanceVerifier(expected, actual))
					{
						while (verifier.Read())
						{
							if (verifier.NodeType != XmlNodeType.Element) continue;
							while (verifier.MoveToNextAttribute())
							{
								verifier.ReadAttributeValue();
							}
							verifier.MoveToContent();
						}
					}
				},
				Throws.Nothing);
		}

		private static void AssertTransformConformance(XmlReader actual, XmlReader expected)
		{
			Assert.That(
				() => {
					using (var verifier = new XmlReaderConformanceVerifier(actual, expected))
					{
						using (var writer = XmlWriter.Create(new StringBuilder()))
						{
							IdentityTransform.Transform(verifier, writer);
						}
					}
				},
				Throws.Nothing);
		}

		private static void AssertTransformInvocations(XmlReader actual, XmlReader expected)
		{
			var actualSpy = new XmlReaderSpy(actual);
			var expectedSpy = new XmlReaderSpy(expected);
			using (actualSpy)
			using (expectedSpy)
			{
				using (var writer = XmlWriter.Create(new StringBuilder()))
				{
					IdentityTransform.Transform(expectedSpy, writer);
				}
				using (var writer = XmlWriter.Create(new StringBuilder()))
				{
					IdentityTransform.Transform(actualSpy, writer);
				}
			}
			Assert.That(actualSpy.Invocations, Is.EqualTo(expectedSpy.Invocations));
		}

		private static void AssertTransformResult(XmlReader actual, XmlReader expected)
		{
			using (actual)
			using (expected)
			{
				var expectedbuilder = new StringBuilder();
				using (var writer = XmlWriter.Create(expectedbuilder))
				{
					IdentityTransform.Transform(expected, writer);
				}
				var actualbuilder = new StringBuilder();
				using (var writer = XmlWriter.Create(actualbuilder))
				{
					IdentityTransform.Transform(actual, writer);
				}
				Assert.That(actualbuilder.ToString(), Is.EqualTo(expectedbuilder.ToString()));
			}
		}
	}
}
