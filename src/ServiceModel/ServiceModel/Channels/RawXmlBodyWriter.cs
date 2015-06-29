#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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

using System.IO;
using System.ServiceModel.Channels;
using System.Xml;

namespace Be.Stateless.ServiceModel.Channels
{
	internal class RawXmlBodyWriter : BodyWriter
	{
		public RawXmlBodyWriter(string rawXmlBody)
			: base(true)
		{
			_rawXmlBody = rawXmlBody;
		}

		protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
		{
			// see http://blogs.msdn.com/b/wifry/archive/2007/05/15/wcf-bodywriter-and-raw-xml-problems.aspx
			using (var xmlReader = XmlReader.Create(new StringReader(_rawXmlBody), new XmlReaderSettings { CloseInput = true }))
			{
				writer.WriteNode(xmlReader, true);
			}
		}

		private readonly string _rawXmlBody;
	}
}