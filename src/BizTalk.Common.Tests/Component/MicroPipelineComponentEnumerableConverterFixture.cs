#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.XPath;
using Be.Stateless.Linq;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Component
{
	[TestFixture]
	public class MicroPipelineComponentEnumerableConverterFixture
	{
		[Test]
		public void CanConvertFrom()
		{
			var sut = new MicroPipelineComponentEnumerableConverter();
			Assert.That(sut.CanConvertFrom(typeof(string)));
		}

		[Test]
		public void CanConvertTo()
		{
			var sut = new MicroPipelineComponentEnumerableConverter();
			Assert.That(sut.CanConvertTo(typeof(string)));
		}

		[Test]
		public void ConvertFrom()
		{
			var xml = string.Format(
				@"<mComponents>
  <mComponent name=""{0}"">
    <Property-One>1</Property-One>
    <Property-Two>2</Property-Two>
  </mComponent>
  <mComponent name=""{1}"" >
    <Property-Six>6</Property-Six>
    <Property-Ten>9</Property-Ten>
  </mComponent>
  <mComponent name=""{2}"">
    <Index>10</Index>
    <Name>DummyTen</Name>
  </mComponent>
</mComponents>",
				typeof(MicroPipelineComponentDummyOne).AssemblyQualifiedName,
				typeof(MicroPipelineComponentDummyTwo).AssemblyQualifiedName,
				typeof(MicroPipelineComponentDummyTen).AssemblyQualifiedName);

			var sut = new MicroPipelineComponentEnumerableConverter();
			var result = sut.ConvertFrom(xml);
			Assert.That(result, Is.Not.Null.And.InstanceOf<IEnumerable<IMicroPipelineComponent>>());

			Assert.That(
				result,
				Is.EquivalentTo(
					new IMicroPipelineComponent[] {
						new MicroPipelineComponentDummyOne(),
						new MicroPipelineComponentDummyTwo(),
						new MicroPipelineComponentDummyTen()
					})
					.Using(new LambdaComparer<IMicroPipelineComponent>((lmc, rmc) => lmc.GetType() == rmc.GetType())));
		}

		[Test]
		public void ConvertFromEmpty()
		{
			var sut = new MicroPipelineComponentEnumerableConverter();

			Assert.That(
				sut.ConvertFrom(string.Empty),
				Is.SameAs(Enumerable.Empty<IMicroPipelineComponent>()));
		}

		[Test]
		public void ConvertFromNull()
		{
			var sut = new MicroPipelineComponentEnumerableConverter();

			Assert.That(
				sut.ConvertFrom(null),
				Is.SameAs(Enumerable.Empty<IMicroPipelineComponent>()));
		}

		[Test]
		public void ConvertTo()
		{
			var list = new List<IMicroPipelineComponent> {
				new MicroPipelineComponentDummyOne(),
				new MicroPipelineComponentDummyTwo(),
				new MicroPipelineComponentDummyTen()
			};

			var sut = new MicroPipelineComponentEnumerableConverter();

			Assert.That(
				sut.ConvertTo(list, typeof(string)),
				Is.EqualTo(
					string.Format(
						"<mComponents>"
							+ "<mComponent name=\"{0}\">"
							+ "<Property-One>one</Property-One><Property-Two>two</Property-Two>"
							+ "</mComponent>"
							+ "<mComponent name=\"{1}\">"
							+ "<Property-Six>six</Property-Six><Property-Ten>ten</Property-Ten>"
							+ "</mComponent>"
							+ "<mComponent name=\"{2}\">"
							+ "<Index>10</Index><Name>DummyTen</Name>"
							+ "</mComponent>"
							+ "</mComponents>",
						typeof(MicroPipelineComponentDummyOne).AssemblyQualifiedName,
						typeof(MicroPipelineComponentDummyTwo).AssemblyQualifiedName,
						typeof(MicroPipelineComponentDummyTen).AssemblyQualifiedName)));
		}

		[Test]
		public void ConvertToNull()
		{
			var sut = new MicroPipelineComponentEnumerableConverter();

			Assert.That(
				sut.ConvertTo(Enumerable.Empty<IMicroPipelineComponent>(), typeof(string)),
				Is.Null);
		}

		[Test]
		public void DeserializeComplexType()
		{
			var xml = string.Format(
				@"<mComponents>
  <mComponent name=""{0}"">
    <Enabled>true</Enabled>
    <Extractors>
      <s0:Properties xmlns:s0=""urn:schemas.stateless.be:biztalk:annotations:2013:01"" xmlns:s1=""urn"">
        <s1:Property1 xpath=""*/some-node"" />
        <s1:Property2 promoted=""true"" xpath=""*/other-node"" />
      </s0:Properties>
    </Extractors>
  </mComponent>
  <mComponent name=""{1}"">
    <Property-One>1</Property-One>
    <Property-Two>2</Property-Two>
  </mComponent>
</mComponents>",
				typeof(DummyContextPropertyExtractorComponent).AssemblyQualifiedName,
				typeof(MicroPipelineComponentDummyOne).AssemblyQualifiedName
				);

			var sut = new MicroPipelineComponentEnumerableConverter();

			var deserialized = sut.ConvertFrom(xml) as IMicroPipelineComponent[];

			Assert.That(
				deserialized,
				Is.EquivalentTo(
					new IMicroPipelineComponent[] {
						new DummyContextPropertyExtractorComponent(),
						new MicroPipelineComponentDummyOne()
					})
					.Using(new LambdaComparer<IMicroPipelineComponent>((lmc, rmc) => lmc.GetType() == rmc.GetType())));

			// ReSharper disable once PossibleNullReferenceException
			var extractorComponent = deserialized[0] as DummyContextPropertyExtractorComponent;
			Assert.That(extractorComponent, Is.Not.Null);
			// ReSharper disable once PossibleNullReferenceException
			Assert.That(extractorComponent.Enabled);
			Assert.That(
				extractorComponent.Extractors,
				Is.EqualTo(
					new[] {
						new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node", ExtractionMode.Write),
						new XPathExtractor(new XmlQualifiedName("Property2", "urn"), "*/other-node", ExtractionMode.Promote)
					}));
		}

		[Test]
		public void SerializeComplexType()
		{
			var component = new DummyContextPropertyExtractorComponent {
				Enabled = true,
				Extractors = new[] {
					new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node", ExtractionMode.Write),
					new XPathExtractor(new XmlQualifiedName("Property2", "urn"), "*/other-node", ExtractionMode.Promote)
				}
			};

			var sut = new MicroPipelineComponentEnumerableConverter();

			Assert.That(
				sut.ConvertTo(new IMicroPipelineComponent[] { component }, typeof(string)),
				Is.EqualTo(
					string.Format(
						"<mComponents>"
							+ "<mComponent name=\"{0}\">"
							+ "<Enabled>true</Enabled>"
							+ "<Extractors>"
							+ "<s0:Properties xmlns:s0=\"urn:schemas.stateless.be:biztalk:annotations:2013:01\" xmlns:s1=\"urn\">"
							+ "<s1:Property1 xpath=\"*/some-node\" />"
							+ "<s1:Property2 promoted=\"true\" xpath=\"*/other-node\" />"
							+ "</s0:Properties>"
							+ "</Extractors>"
							+ "</mComponent>"
							+ "</mComponents>",
						typeof(DummyContextPropertyExtractorComponent).AssemblyQualifiedName)));
		}

		public class DummyContextPropertyExtractorComponent : IMicroPipelineComponent
		{
			#region IMicroPipelineComponent Members

			public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
			{
				throw new NotSupportedException();
			}

			#endregion

			public bool Enabled { get; set; }

			[XmlIgnore]
			public IEnumerable<XPathExtractor> Extractors { get; set; }

			[Browsable(false)]
			[EditorBrowsable(EditorBrowsableState.Never)]
			[XmlElement("Extractors")]
			public XPathExtractorSerializerSurrogate ExtractorSerializerSurrogate
			{
				get { return new XPathExtractorSerializerSurrogate(Extractors); }
				set { Extractors = value.Extractors; }
			}
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Required by XML serialization")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Required by XML serialization")]
		public class MicroPipelineComponentDummyOne : IMicroPipelineComponent, IXmlSerializable
		{
			#region IMicroPipelineComponent Members

			public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
			{
				throw new NotSupportedException();
			}

			#endregion

			#region IXmlSerializable Members

			public XmlSchema GetSchema()
			{
				return null;
			}

			public void ReadXml(XmlReader reader)
			{
				reader.ReadStartElement("Property-One");
				One = reader.ReadContentAsString();
				reader.ReadEndElement();
				reader.ReadStartElement("Property-Two");
				Two = reader.ReadContentAsString();
				reader.ReadEndElement();
			}

			public void WriteXml(XmlWriter writer)
			{
				writer.WriteElementString("Property-One", "one");
				writer.WriteElementString("Property-Two", "two");
			}

			#endregion

			public string One { get; set; }

			public string Two { get; set; }
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Required by XML serialization")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Required by XML serialization")]
		public class MicroPipelineComponentDummyTwo : IMicroPipelineComponent, IXmlSerializable
		{
			#region IMicroPipelineComponent Members

			public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
			{
				throw new NotSupportedException();
			}

			#endregion

			#region IXmlSerializable Members

			public XmlSchema GetSchema()
			{
				return null;
			}

			public void ReadXml(XmlReader reader)
			{
				Six = reader.ReadElementContentAsString("Property-Six", string.Empty);
				Ten = reader.ReadElementContentAsString("Property-Ten", string.Empty);
			}

			public void WriteXml(XmlWriter writer)
			{
				writer.WriteElementString("Property-Six", "six");
				writer.WriteElementString("Property-Ten", "ten");
			}

			#endregion

			public string Six { get; set; }

			public string Ten { get; set; }
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Required by XML serialization")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Required by XML serialization")]
		public class MicroPipelineComponentDummyTen : IMicroPipelineComponent
		{
			public MicroPipelineComponentDummyTen()
			{
				Index = 10;
				Name = "DummyTen";
			}

			#region IMicroPipelineComponent Members

			public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
			{
				throw new NotSupportedException();
			}

			#endregion

			public int Index { get; set; }

			public string Name { get; set; }
		}
	}
}
