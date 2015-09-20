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
using System.ComponentModel;
using System.Text;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.BizTalk.Unit.Component;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.Text;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Component
{
	[TestFixture]
	public class XmlTranslatorComponentFixture : PipelineComponentFixture<XmlTranslatorComponent>
	{
		static XmlTranslatorComponentFixture()
		{
			TypeDescriptor.AddAttributes(typeof(Encoding), new TypeConverterAttribute(typeof(EncodingConverter)));
			TypeDescriptor.AddAttributes(typeof(XmlTranslationSet), new TypeConverterAttribute(typeof(XmlTranslationSetConverter)));
		}

		protected override object GetValueForProperty(string name)
		{
			switch (name)
			{
				case "Encoding":
					return Encoding.GetEncoding("iso-8859-1");
				case "Translations":
					return new XmlTranslationSet {
						Override = true,
						Items = new[] {
							// ensure unique string values
							new XmlNamespaceTranslation("y", "urn:" + Guid.NewGuid().ToString("N")),
							new XmlNamespaceTranslation("z", "urn:" + Guid.NewGuid().ToString("N"))
						}
					};
				default:
					return base.GetValueForProperty(name);
			}
		}
	}
}
