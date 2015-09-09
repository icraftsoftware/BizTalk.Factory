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
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Be.Stateless.IO;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Component.Extensions
{
	[TestFixture]
	public class MicroPipelineComponentXmlReaderExtensionsFixture
	{
		[Test]
		public void DeserializeDummyMicroPipelineComponentWithCustomXmlSerialization()
		{
			var microPipelineComponentType = typeof(DummyMicroPipelineComponentWithCustomXmlSerialization);
			var xml = string.Format("<mComponent name=\"{0}\" />", microPipelineComponentType.AssemblyQualifiedName);
			using (var reader = XmlReader.Create(new StringStream(xml)))
			{
				var microPipelineComponent = reader.DeserializeMicroPipelineComponent();
				Assert.That(microPipelineComponent, Is.TypeOf(microPipelineComponentType));
				Assert.That(reader.EOF);
			}
		}

		[Test]
		public void DeserializeDummyMicroPipelineComponentWithDefaultXmlSerialization()
		{
			var microPipelineComponentType = typeof(DummyMicroPipelineComponentWithDefaultXmlSerialization);
			var xml = string.Format("<mComponent name=\"{0}\" />", microPipelineComponentType.AssemblyQualifiedName);
			using (var reader = XmlReader.Create(new StringStream(xml)))
			{
				var microPipelineComponent = reader.DeserializeMicroPipelineComponent();
				Assert.That(microPipelineComponent, Is.TypeOf(microPipelineComponentType));
				Assert.That(reader.EOF);
			}
		}

		[Test]
		public void DeserializeDummyMicroPipelineComponentWithVerboseCustomXmlSerialization()
		{
			var microPipelineComponentType = typeof(DummyMicroPipelineComponentWithCustomXmlSerialization);
			var xml = string.Format("<mComponent name=\"{0}\"></mComponent>", microPipelineComponentType.AssemblyQualifiedName);
			using (var reader = XmlReader.Create(new StringStream(xml)))
			{
				var microPipelineComponent = reader.DeserializeMicroPipelineComponent();
				Assert.That(microPipelineComponent, Is.TypeOf(microPipelineComponentType));
				Assert.That(reader.EOF);
			}
		}

		[Test]
		public void DeserializeThrowsWhenNotMicroPipelineComponent()
		{
			var qualifiedName = GetType().AssemblyQualifiedName;
			var xml = string.Format("<mComponent name=\"{0}\" />", qualifiedName);
			using (var reader = XmlReader.Create(new StringStream(xml)))
			{
				// ReSharper disable once AccessToDisposedClosure
				Assert.That(
					() => reader.DeserializeMicroPipelineComponent(),
					Throws.TypeOf<ConfigurationErrorsException>()
						.With.Message.EqualTo(string.Format("{0} does not implement {1}.", qualifiedName, typeof(IMicroPipelineComponent).Name)));
			}
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Required by XML serialization")]
		public class DummyMicroPipelineComponentWithDefaultXmlSerialization : IMicroPipelineComponent
		{
			#region IMicroPipelineComponent Members

			public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Required by XML serialization")]
		public class DummyMicroPipelineComponentWithCustomXmlSerialization : IMicroPipelineComponent, IXmlSerializable
		{
			#region IMicroPipelineComponent Members

			public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region IXmlSerializable Members

			public XmlSchema GetSchema()
			{
				return null;
			}

			public void ReadXml(XmlReader reader) { }

			public void WriteXml(XmlWriter writer)
			{
				throw new NotImplementedException();
			}

			#endregion
		}
	}
}
