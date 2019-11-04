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
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Be.Stateless.BizTalk.Message;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.Xml.Extensions;
using Be.Stateless.Xml.Xsl;
using BTF2Schemas;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	[TestFixture]
	public class ClosedTransformFixtureFixture : ClosedTransformFixture<IdentityTransform>
	{
		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void InvalidInputMessageThrowsInputRelatedXmlSchemaValidationExceptionWithEmbeddedXmlContent()
		{
			using (var stream1 = _document.AsStream())
			using (var stream2 = _document.AsStream())
			{
				Assert.That(
					() => Given(
						input => input
							.Arguments(new XsltArgumentList())
							.Context(new MessageContextMock().Object)
							.Message<Envelope>(stream1)
							.Message(stream2))
						.Transform
						.OutputsXml(
							output => output
								.WithValuednessValidationCallback((sender, args) => args.Severity = XmlSeverityType.Warning)
								.WithNoConformanceLevel()),
					Throws.TypeOf<XmlSchemaValidationException>().With.Message.StartsWith("Transform's input message #1 failed 'Envelope' schema validation for the following reason:"));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void InvalidTransformOutputThrowsOutputRelatedXmlSchemaValidationExceptionWithEmbeddedXmlContent()
		{
			using (var stream = _document.AsStream())
			{
				var setup = Given(input => input.Message(stream))
					.Transform
					.OutputsXml(output => output.ConformingTo<Envelope>().ConformingTo<Batch>().WithStrictConformanceLevel());

				Assert.That(
					() => setup.Validate(),
					Throws.TypeOf<XmlSchemaValidationException>().With.Message.StartsWith("Transform's output failed schema(s) validation for the following reason:"));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void InvalidTransformResultThrows()
		{
			using (var stream = MessageFactory.CreateMessage<btf2_services_header>().AsStream())
			{
				var setup = Given(input => input.Message(stream))
					.Transform
					.OutputsXml(output => output.ConformingTo<btf2_services_header>().WithStrictConformanceLevel());

				Assert.That(() => setup.Validate(), Throws.InstanceOf<XmlSchemaValidationException>());
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public void ScalarAssertion()
		{
			using (var stream = _document.AsStream())
			{
				var setup = Given(input => input.Message(stream))
					.Transform
					.OutputsXml(output => output.ConformingTo<btf2_services_header>().WithStrictConformanceLevel());

				var result = setup.Validate();

				result.XmlNamespaceManager.AddNamespace("tns", typeof(btf2_services_header).GetMetadata().TargetNamespace);
				Assert.That(result.SelectSingleNode("//*[1]/tns:sendBy/text()").Value, Is.EqualTo("2012-04-12T12:13:14"));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void SetupTextTransform()
		{
			using (var stream1 = new MemoryStream(Encoding.Unicode.GetBytes(_document.OuterXml)))
			using (var stream2 = new MemoryStream(Encoding.Unicode.GetBytes(_document.OuterXml)))
			{
				var setup = Given(
					input => input
						.Arguments(new XsltArgumentList())
						.Context(new MessageContextMock().Object)
						.Message<Envelope>(stream1)
						.Message(stream2))
					.Transform
					.OutputsText();

				// TODO execute should throw if setup is invalid
				// TODO should fail as output declared in transform is not text nor html
				var result = setup.Validate();

				//TODO assertion on setup
				Assert.Fail("TODO");
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void SetupTextTransformThrowsIfXsltOutputIsNotText()
		{
			Assert.Fail("TODO");
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void SetupValuednessValidationCallbackConfirmingSeverity()
		{
			using (var stream = MessageFactory.CreateMessage<btf2_services_header>().AsStream())
			{
				var setup = Given(input => input.Message(stream))
					.Transform
					.OutputsXml(
						output => output
							.WithValuednessValidationCallback((sender, args) => args.Severity = XmlSeverityType.Error)
							.ConformingTo<btf2_services_header>().WithNoConformanceLevel()
					);

				Assert.That(
					() => setup.Validate(),
					Throws.InstanceOf<XmlException>()
						.With.Message.Contains("/ns0:services/ns0:deliveryReceiptRequest/ns0:sendTo/ns0:address")
						.And.With.Message.Contains("/ns0:services/ns0:deliveryReceiptRequest/ns0:sendBy")
						.And.With.Message.Contains("/ns0:services/ns0:commitmentReceiptRequest/ns0:sendBy")
					);
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void SetupValuednessValidationCallbackDemotingSeverity()
		{
			using (var stream = MessageFactory.CreateMessage<btf2_services_header>().AsStream())
			{
				var setup = Given(input => input.Message(stream))
					.Transform
					.OutputsXml(
						output => output
							.WithValuednessValidationCallback((sender, args) => args.Severity = XmlSeverityType.Warning)
							.ConformingTo<btf2_services_header>().WithNoConformanceLevel()
					);

				Assert.That(() => setup.Validate(), Throws.Nothing);
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void SetupXmlTransform()
		{
			using (var stream1 = new MemoryStream(Encoding.Unicode.GetBytes(_document.OuterXml)))
			using (var stream2 = new MemoryStream(Encoding.Unicode.GetBytes(_document.OuterXml)))
			{
				var setup = Given(
					input => input
						.Arguments(new XsltArgumentList())
						.Context(new MessageContextMock().Object)
						.Message<Envelope>(stream1)
						.Message(stream2))
					.Transform
					.OutputsXml(
						output => output
							.ConformingTo<Any>()
							.ConformingTo<Batch>()
							.WithStrictConformanceLevel());

				// TODO execute should throw if setup is invalid
				var result = setup.Validate();

				//TODO assertion on setup
				Assert.Fail("TODO");
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void SetupXmlTransformWithoutConformanceLevel()
		{
			using (var stream = _document.AsStream())
			{
				var setup = Given(input => input.Message(stream))
					.Transform
					.OutputsXml(output => output.ConformingTo<Envelope>().WithNoConformanceLevel());

				Assert.That(() => setup.Validate(), Throws.Nothing);
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void SetupXmlTransformWithoutConformingSchemaAndConformanceLevel()
		{
			using (var stream = MessageFactory.CreateMessage<btf2_services_header>().AsStream())
			{
				var setup = Given(input => input.Message(stream))
					.Transform
					.OutputsXml(output => output.WithValuednessValidationCallback((s, args) => args.Severity = XmlSeverityType.Warning).WithNoConformanceLevel());

				Assert.That(() => setup.Validate(), Throws.Nothing);
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void SetupXmlTransformWithoutConformingSchemaButWithStrictConformanceLevel()
		{
			using (var stream = _document.AsStream())
			{
				Assert.That(
					() => Given(input => input.Message(stream))
						.Transform
						.OutputsXml(output => output.WithStrictConformanceLevel()),
					Throws.InvalidOperationException.With.Message.EqualTo("At least one XML Schema to which the output must conform to must be setup."));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void SetupXmlTransformWithoutInputMessage()
		{
			Assert.That(
				() => Given(input => { })
					.Transform
					.OutputsXml(output => output.WithStrictConformanceLevel()),
				Throws.InvalidOperationException.With.Message.EqualTo("At least one input message must be setup."));
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void StringJoinAssertion()
		{
			using (var stream = _document.AsStream())
			{
				var setup = Given(input => input.Message(stream))
					.Transform
					.OutputsXml(output => output.ConformingTo<btf2_services_header>().WithStrictConformanceLevel());

				var result = setup.Validate();

				result.XmlNamespaceManager.AddNamespace("tns", typeof(btf2_services_header).GetMetadata().TargetNamespace);
				Assert.That(result.StringJoin("//tns:sendBy"), Is.EqualTo("2012-04-12T12:13:14#2012-04-12T23:22:21"));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ValidTransformResultDoesNotThrow()
		{
			using (var stream = _document.AsStream())
			{
				var setup = Given(input => input.Message(stream))
					.Transform
					.OutputsXml(output => output.ConformingTo<btf2_services_header>().WithStrictConformanceLevel());

				Assert.That(() => setup.Validate(), Throws.Nothing);
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void XPathAssertion()
		{
			using (var stream = _document.AsStream())
			{
				var setup = Given(input => input.Message(stream))
					.Transform
					.OutputsXml(output => output.ConformingTo<btf2_services_header>().WithStrictConformanceLevel());

				var result = setup.Validate();

				result.XmlNamespaceManager.AddNamespace("tns", typeof(btf2_services_header).GetMetadata().TargetNamespace);
				Assert.That(result.Select("//tns:sendBy").Count, Is.EqualTo(2));
			}
		}

		[Test]
		public void XPathNavigatorResultingOfSelectCanBeReusedToSimplifyXPathExpression()
		{
			Assert.Fail("TODO");
		}

		private readonly XmlDocument _document = MessageFactory.CreateMessage<btf2_services_header>(ResourceManager.LoadString("Data.Message.xml"));
	}
}
