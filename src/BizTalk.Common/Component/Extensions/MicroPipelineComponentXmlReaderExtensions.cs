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
using System.Xml;
using System.Xml.Serialization;
using Be.Stateless.Xml.Extensions;

namespace Be.Stateless.BizTalk.Component.Extensions
{
	internal static class MicroPipelineComponentXmlReaderExtensions
	{
		internal static IMicroPipelineComponent DeserializeMicroPipelineComponent(this XmlReader reader)
		{
			reader.AssertStartElement("mComponent");
			var microPipelineComponentType = Type.GetType(reader.GetMandatoryAttribute("name"), true);
			if (!typeof(IMicroPipelineComponent).IsAssignableFrom(microPipelineComponentType))
				throw new ConfigurationErrorsException(
					string.Format(
						"{0} does not implement {1}.",
						microPipelineComponentType.AssemblyQualifiedName,
						typeof(IMicroPipelineComponent).Name));

			// reset position to an element as required to call ReadSubtree()
			reader.MoveToElement();
			var microPipelineComponentXmlSubtree = reader.ReadSubtree();

			IMicroPipelineComponent component;
			if (typeof(IXmlSerializable).IsAssignableFrom(microPipelineComponentType))
			{
				component = (IMicroPipelineComponent) Activator.CreateInstance(microPipelineComponentType);
				// relieve micro pipeline components from having to deal with surrounding mComponent XML element
				microPipelineComponentXmlSubtree.MoveToContent();
				reader.ReadStartElement("mComponent");
				((IXmlSerializable) component).ReadXml(microPipelineComponentXmlSubtree);
			}
			else
			{
				var overrides = new XmlAttributeOverrides();
				overrides.Add(microPipelineComponentType, new XmlAttributes { XmlRoot = new XmlRootAttribute("mComponent") });
				var serializer = new XmlSerializer(microPipelineComponentType, overrides);
				component = (IMicroPipelineComponent) serializer.Deserialize(microPipelineComponentXmlSubtree);
			}

			reader.Skip();
			return component;
		}

		internal static bool IsMicroPipelineComponent(this XmlReader reader)
		{
			return reader.IsStartElement("mComponent");
		}
	}
}
