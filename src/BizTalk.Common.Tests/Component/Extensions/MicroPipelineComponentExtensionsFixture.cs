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
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Component.Extensions
{
	[TestFixture]
	public class MicroPipelineComponentExtensionsFixture
	{
		[Test]
		public void SerializeDummyMicroPipelineComponentWithCustomXmlSerialization()
		{
			var component = new DummyMicroPipelineComponentWithCustomXmlSerialization();
			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
			{
				component.Serialize(writer);
			}
			Assert.That(
				builder.ToString(),
				Is.EqualTo(string.Format("<mComponent name=\"{0}\" />", component.GetType().AssemblyQualifiedName)));
		}

		[Test]
		public void SerializeDummyMicroPipelineComponentWithDefaultXmlSerialization()
		{
			var component = new DummyMicroPipelineComponentWithDefaultXmlSerialization();
			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
			{
				component.Serialize(writer);
			}
			Assert.That(
				builder.ToString(),
				Is.EqualTo(string.Format("<mComponent name=\"{0}\" />", component.GetType().AssemblyQualifiedName)));
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Required by XML serialization")]
		public class DummyMicroPipelineComponentWithDefaultXmlSerialization : IMicroPipelineComponent
		{
			#region IMicroPipelineComponent Members

			public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
			{
				throw new NotSupportedException();
			}

			#endregion
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Required by XML serialization")]
		public class DummyMicroPipelineComponentWithCustomXmlSerialization : IMicroPipelineComponent, IXmlSerializable
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
				throw new NotSupportedException();
			}

			public void WriteXml(XmlWriter writer) { }

			#endregion
		}
	}
}
