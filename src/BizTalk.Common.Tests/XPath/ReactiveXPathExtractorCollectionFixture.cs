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
using System.IO;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.BizTalk.Unit;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.IO.Extensions;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.XPath
{
	[TestFixture]
	[SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
	public class ReactiveXPathExtractorCollectionFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void TestSetup()
		{
			MessageContextMock = new MessageContextMock();
		}

		#endregion

		[Test]
		public void Match()
		{
			var extractors = new[] {
				new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/from", ExtractionMode.Promote),
				new XPathExtractor(BizTalkFactoryProperties.ReceiverName.QName, "/letter/*/to", ExtractionMode.Promote),
				new XPathExtractor(TrackingProperties.ProcessName.QName, "/letter/*/subject", ExtractionMode.Write),
				new XPathExtractor(TrackingProperties.Value1.QName, "/letter/*/paragraph", ExtractionMode.Write),
				new XPathExtractor(TrackingProperties.Value2.QName, "/letter/*/salutations", ExtractionMode.Write),
				new XPathExtractor(TrackingProperties.Value3.QName, "/letter/*/signature", ExtractionMode.Write)
			};

			var stream = XPathMutatorStreamFactory.Create(
				ResourceManager.Load("Data.UnqualifiedLetter.xml"),
				extractors,
				() => MessageContextMock.Object);

			stream.Drain();

			MessageContextMock.Verify(c => c.Promote(BizTalkFactoryProperties.SenderName, "info@world.com"));
			MessageContextMock.Verify(c => c.Promote(BizTalkFactoryProperties.ReceiverName, "francois.chabot@gmail.com"));
			MessageContextMock.Verify(c => c.SetProperty(TrackingProperties.ProcessName, "inquiry"));
			MessageContextMock.Verify(c => c.SetProperty(TrackingProperties.Value1, "paragraph-one"));
			MessageContextMock.Verify(c => c.SetProperty(TrackingProperties.Value2, "King regards,"));
			MessageContextMock.Verify(c => c.SetProperty(TrackingProperties.Value3, "John Doe"));
		}

		[Test]
		public void MatchAndDemote()
		{
			var extractors = new[] {
				new XPathExtractor(TrackingProperties.Value1.QName, "/letter/*/paragraph[1]", ExtractionMode.Demote),
				new XPathExtractor(TrackingProperties.Value1.QName, "/letter/*/paragraph[2]", ExtractionMode.Demote),
				new XPathExtractor(TrackingProperties.Value1.QName, "/letter/*/paragraph[3]", ExtractionMode.Demote)
			};

			using (var stream = XPathMutatorStreamFactory.Create(ResourceManager.Load("Data.UnqualifiedLetter.xml"), extractors, () => MessageContextMock.Object))
			using (var reader = new StreamReader(stream))
			{
				MessageContextMock.Setup(c => c.GetProperty(TrackingProperties.Value1)).Returns("same-paragraph");
				Assert.That(
					reader.ReadToEnd(),
					Is.EqualTo(
						ResourceManager
							.LoadString("Data.UnqualifiedLetter.xml")
							.Substring(38) // skip xml declaration
							.Replace("paragraph-one", "same-paragraph")
							.Replace("paragraph-two", "same-paragraph")
							.Replace("paragraph-six", "same-paragraph")
						));
			}
		}

		[Test]
		public void MatchForGroup()
		{
			var extractors = new[] {
				new XPathExtractor(TrackingProperties.Value1.QName, "/letter/*/paragraph", ExtractionMode.Write),
				new XPathExtractor(TrackingProperties.Value2.QName, "/letter/*/paragraph", ExtractionMode.Write),
				new XPathExtractor(TrackingProperties.Value3.QName, "/letter/*/paragraph", ExtractionMode.Write)
			};

			var stream = XPathMutatorStreamFactory.Create(
				ResourceManager.Load("Data.UnqualifiedLetter.xml"),
				extractors,
				() => MessageContextMock.Object);

			stream.Drain();

			MessageContextMock.Verify(c => c.SetProperty(TrackingProperties.Value1, "paragraph-one"));
			MessageContextMock.Verify(c => c.SetProperty(TrackingProperties.Value2, "paragraph-one"));
			MessageContextMock.Verify(c => c.SetProperty(TrackingProperties.Value3, "paragraph-one"));
		}

		[Test]
		public void MatchQualified()
		{
			var extractors = new[] {
				new XPathExtractor(TrackingProperties.ProcessName.QName, "/*[local-name()='letter']/*/*[local-name()='subject']", ExtractionMode.Write),
				new XPathExtractor(TrackingProperties.Value1.QName, "/*[local-name()='letter']/*/*[local-name()='paragraph']", ExtractionMode.Write),
				new XPathExtractor(TrackingProperties.Value3.QName, "/*[local-name()='letter']/*/*[local-name()='signature']", ExtractionMode.Write)
			};

			var stream = XPathMutatorStreamFactory.Create(
				ResourceManager.Load("Data.QualifiedLetter.xml"),
				extractors,
				() => MessageContextMock.Object);

			stream.Drain();

			MessageContextMock.Verify(c => c.SetProperty(TrackingProperties.ProcessName, "inquiry"));
			MessageContextMock.Verify(c => c.SetProperty(TrackingProperties.Value1, "paragraph-one"));
			MessageContextMock.Verify(c => c.SetProperty(TrackingProperties.Value3, "John Doe"));
		}

		[Test]
		public void MatchQualifiedWithPosition()
		{
			var extractors = new[] {
				new XPathExtractor(TrackingProperties.Value1.QName, "/*[local-name()='letter']/*/*[local-name()='paragraph'][1]", ExtractionMode.Write),
				new XPathExtractor(TrackingProperties.Value2.QName, "/*[local-name()='letter']/*/*[local-name()='paragraph'][2]", ExtractionMode.Write),
				new XPathExtractor(TrackingProperties.Value3.QName, "/*[local-name()='letter']/*/*[local-name()='paragraph'][3]", ExtractionMode.Write)
			};

			var stream = XPathMutatorStreamFactory.Create(
				ResourceManager.Load("Data.QualifiedLetter.xml"),
				extractors,
				() => MessageContextMock.Object);

			stream.Drain();

			// !!IMPORTANT!! XPathMutatorStream does not support such expressions and henceforth does not perform any succeeding match
			MessageContextMock.Verify(c => c.Promote(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never());
			MessageContextMock.Verify(c => c.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never());
		}

		[Test]
		public void MatchWithPositions()
		{
			var extractors = new[] {
				new XPathExtractor(TrackingProperties.Value1.QName, "/letter/*/paragraph[1]", ExtractionMode.Write),
				new XPathExtractor(TrackingProperties.Value2.QName, "/letter/*/paragraph[2]", ExtractionMode.Write),
				new XPathExtractor(TrackingProperties.Value3.QName, "/letter/*/paragraph[3]", ExtractionMode.Write)
			};

			var stream = XPathMutatorStreamFactory.Create(
				ResourceManager.Load("Data.UnqualifiedLetter.xml"),
				extractors,
				() => MessageContextMock.Object);

			stream.Drain();

			MessageContextMock.Verify(c => c.SetProperty(TrackingProperties.Value1, "paragraph-one"));
			MessageContextMock.Verify(c => c.SetProperty(TrackingProperties.Value2, "paragraph-two"));
			MessageContextMock.Verify(c => c.SetProperty(TrackingProperties.Value3, "paragraph-six"));
		}

		private MessageContextMock MessageContextMock { get; set; }
	}
}
