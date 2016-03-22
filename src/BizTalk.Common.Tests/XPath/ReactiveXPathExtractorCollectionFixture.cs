#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.IO.Extensions;
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
			_matches = new List<Tuple<XmlQualifiedName, string, ExtractionMode>>();
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
				new XPathExtractor(TrackingProperties.Value3.QName, "/letter/*/signature", ExtractionMode.Write),
			};

			var stream = XPathMutatorStreamFactory.Create(
				ResourceManager.Load("Data.UnqualifiedLetter.xml"),
				extractors,
				(propertyName, value, extractionMode) => _matches.Add(Tuple.Create(propertyName, value, extractionMode)));

			stream.Drain();

			Assert.That(
				_matches,
				Is.EqualTo(
					new[] {
						Tuple.Create(TrackingProperties.ProcessName.QName, "inquiry", ExtractionMode.Write),
						Tuple.Create(BizTalkFactoryProperties.SenderName.QName, "info@world.com", ExtractionMode.Promote),
						Tuple.Create(BizTalkFactoryProperties.ReceiverName.QName, "francois.chabot@gmail.com", ExtractionMode.Promote),
						Tuple.Create(TrackingProperties.Value1.QName, "paragraph-one", ExtractionMode.Write),
						Tuple.Create(TrackingProperties.Value2.QName, "King regards,", ExtractionMode.Write),
						Tuple.Create(TrackingProperties.Value3.QName, "John Doe", ExtractionMode.Write)
					}));
		}

		[Test]
		public void MatchForGroup()
		{
			var extractors = new[] {
				new XPathExtractor(TrackingProperties.Value1.QName, "/letter/*/paragraph", ExtractionMode.Write),
				new XPathExtractor(TrackingProperties.Value2.QName, "/letter/*/paragraph", ExtractionMode.Write),
				new XPathExtractor(TrackingProperties.Value3.QName, "/letter/*/paragraph", ExtractionMode.Write),
			};

			var stream = XPathMutatorStreamFactory.Create(
				ResourceManager.Load("Data.UnqualifiedLetter.xml"),
				extractors,
				(propertyName, value, extractionMode) => _matches.Add(Tuple.Create(propertyName, value, extractionMode)));

			stream.Drain();

			Assert.That(
				_matches,
				Is.EqualTo(
					new[] {
						Tuple.Create(TrackingProperties.Value1.QName, "paragraph-one", ExtractionMode.Write),
						Tuple.Create(TrackingProperties.Value2.QName, "paragraph-one", ExtractionMode.Write),
						Tuple.Create(TrackingProperties.Value3.QName, "paragraph-one", ExtractionMode.Write)
					}));
		}

		[Test]
		public void MatchQualified()
		{
			var extractors = new[] {
				new XPathExtractor(TrackingProperties.ProcessName.QName, "/*[local-name()='letter']/*/*[local-name()='subject']", ExtractionMode.Write),
				new XPathExtractor(TrackingProperties.Value1.QName, "/*[local-name()='letter']/*/*[local-name()='paragraph']", ExtractionMode.Write),
				new XPathExtractor(TrackingProperties.Value3.QName, "/*[local-name()='letter']/*/*[local-name()='signature']", ExtractionMode.Write),
			};

			var stream = XPathMutatorStreamFactory.Create(
				ResourceManager.Load("Data.QualifiedLetter.xml"),
				extractors,
				(propertyName, value, extractionMode) => _matches.Add(Tuple.Create(propertyName, value, extractionMode)));

			stream.Drain();

			Assert.That(
				_matches,
				Is.EqualTo(
					new[] {
						Tuple.Create(TrackingProperties.ProcessName.QName, "inquiry", ExtractionMode.Write),
						Tuple.Create(TrackingProperties.Value1.QName, "paragraph-one", ExtractionMode.Write),
						Tuple.Create(TrackingProperties.Value3.QName, "John Doe", ExtractionMode.Write)
					}));
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
				(propertyName, value, extractionMode) => _matches.Add(Tuple.Create(propertyName, value, extractionMode)));

			stream.Drain();

			// !!IMPORTANT!! XPathMutatorStream does not support such expressions and henceforth does not perform any succeeding match
			Assert.That(_matches.Count, Is.EqualTo(0));
		}

		[Test]
		public void MatchWithPositions()
		{
			var extractors = new[] {
				new XPathExtractor(TrackingProperties.Value1.QName, "/letter/*/paragraph[1]", ExtractionMode.Write),
				new XPathExtractor(TrackingProperties.Value2.QName, "/letter/*/paragraph[2]", ExtractionMode.Write),
				new XPathExtractor(TrackingProperties.Value3.QName, "/letter/*/paragraph[3]", ExtractionMode.Write),
			};

			var stream = XPathMutatorStreamFactory.Create(
				ResourceManager.Load("Data.UnqualifiedLetter.xml"),
				extractors,
				(propertyName, value, extractionMode) => _matches.Add(Tuple.Create(propertyName, value, extractionMode)));

			stream.Drain();

			Assert.That(
				_matches,
				Is.EqualTo(
					new[] {
						Tuple.Create(TrackingProperties.Value1.QName, "paragraph-one", ExtractionMode.Write),
						Tuple.Create(TrackingProperties.Value2.QName, "paragraph-two", ExtractionMode.Write),
						Tuple.Create(TrackingProperties.Value3.QName, "paragraph-six", ExtractionMode.Write)
					}));
		}

		private List<Tuple<XmlQualifiedName, string, ExtractionMode>> _matches;
	}
}
