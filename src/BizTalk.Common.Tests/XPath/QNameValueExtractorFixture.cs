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
using System.Xml;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Unit;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.XPath
{
	[TestFixture]
	[SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
	public class QNameValueExtractorFixture
	{
		[Test]
		public void Equality()
		{
			Assert.That(
				new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write, QNameValueExtractionMode.Name),
				Is.EqualTo(new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write, QNameValueExtractionMode.Name)));
		}

		[Test]
		public void ExecuteDemotesLocalNameValueInContext()
		{
			var messageContextMock = new MessageContextMock();
			messageContextMock.Setup(c => c.GetProperty(BizTalkFactoryProperties.ReceiverName)).Returns("new-value");
			var newValue = string.Empty;

			var sut = new QNameValueExtractor(BizTalkFactoryProperties.ReceiverName.QName, "/letter/*/to", ExtractionMode.Demote, QNameValueExtractionMode.LocalName);
			sut.Execute(messageContextMock.Object, "value", ref newValue);

			messageContextMock.Verify(c => c.Promote(BizTalkFactoryProperties.ReceiverName, It.IsAny<string>()), Times.Never);
			Assert.That(newValue, Is.EqualTo("new-value"));
		}

		[Test]
		public void ExecuteDemotesLocalNameValueInContextAndKeepOriginalPrefix()
		{
			var messageContextMock = new MessageContextMock();
			messageContextMock.Setup(c => c.GetProperty(BizTalkFactoryProperties.ReceiverName)).Returns("new-value");
			var newValue = string.Empty;

			var sut = new QNameValueExtractor(BizTalkFactoryProperties.ReceiverName.QName, "/letter/*/to", ExtractionMode.Demote, QNameValueExtractionMode.LocalName);
			sut.Execute(messageContextMock.Object, "ns:value", ref newValue);

			messageContextMock.Verify(c => c.Promote(BizTalkFactoryProperties.ReceiverName, It.IsAny<string>()), Times.Never);
			Assert.That(newValue, Is.EqualTo("ns:new-value"));
		}

		[Test]
		public void ExecutePromotesOrWritesLocalNameValueInContext()
		{
			var messageContextMock = new MessageContextMock();
			var newValue = string.Empty;

			var sut = new QNameValueExtractor(BizTalkFactoryProperties.ReceiverName.QName, "/letter/*/to", ExtractionMode.Promote, QNameValueExtractionMode.LocalName);
			sut.Execute(messageContextMock.Object, "ns:value", ref newValue);

			messageContextMock.Verify(c => c.Promote(BizTalkFactoryProperties.ReceiverName, "value"));
			Assert.That(newValue, Is.Empty);
		}

		[Test]
		public void InequalityOfExtractionMode()
		{
			Assert.That(
				new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write, QNameValueExtractionMode.Name),
				Is.Not.EqualTo(new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Promote, QNameValueExtractionMode.Name)));
		}

		[Test]
		public void InequalityOfProperty()
		{
			Assert.That(
				new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write, QNameValueExtractionMode.Name),
				Is.Not.EqualTo(new QNameValueExtractor(new XmlQualifiedName("prop2", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write, QNameValueExtractionMode.Name)));
		}

		[Test]
		public void InequalityOfQNameValueExtractionMode()
		{
			Assert.That(
				new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write, QNameValueExtractionMode.Name),
				Is.Not.EqualTo(new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write, QNameValueExtractionMode.LocalName)));
		}

		[Test]
		public void InequalityOfType()
		{
			Assert.That(
				new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Promote, QNameValueExtractionMode.Name),
				Is.Not.EqualTo(new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Promote)));

			Assert.That(
				new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Promote),
				Is.Not.EqualTo(new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Promote, QNameValueExtractionMode.Name)));
		}

		[Test]
		public void InequalityOfXPathExpression()
		{
			Assert.That(
				new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write, QNameValueExtractionMode.Name),
				Is.Not.EqualTo(new QNameValueExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'another']", ExtractionMode.Write, QNameValueExtractionMode.Name)));
		}
	}
}
