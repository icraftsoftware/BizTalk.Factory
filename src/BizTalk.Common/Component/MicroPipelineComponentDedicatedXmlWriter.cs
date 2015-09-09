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

using System.Xml;
using Be.Stateless.BizTalk.Xml;

namespace Be.Stateless.BizTalk.Component
{
	internal class MicroPipelineComponentDedicatedXmlWriter : XmlWriterWrapper
	{
		public MicroPipelineComponentDedicatedXmlWriter(XmlWriter writer, IMicroPipelineComponent component) : base(writer)
		{
			_component = component;
		}

		#region Base Class Member Overrides

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			base.WriteStartElement(prefix, localName, ns);
			// relieve micro pipeline components from having to deal with surrounding mComponent XML element
			if (localName == "mComponent")
			{
				// ReSharper disable once AssignNullToNotNullAttribute
				WriteAttributeString("name", _component.GetType().AssemblyQualifiedName);
			}
		}

		#endregion

		private readonly IMicroPipelineComponent _component;
	}
}
