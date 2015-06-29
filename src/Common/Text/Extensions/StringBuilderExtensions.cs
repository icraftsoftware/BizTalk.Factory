#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
using System.Text;
using System.Xml;
using Be.Stateless.Extensions;
using Be.Stateless.Xml;

namespace Be.Stateless.Text.Extensions
{
	public static class StringBuilderExtensions
	{
		/// <summary>
		/// Returns an <see cref="XmlReader"/> over a <see cref="StringBuilder"/> that contains XML data. Provided that
		/// the content is not empty, the <see cref="XmlReader"/> will be positioned at the first content node.
		/// </summary>
		/// <param name="builder">
		/// The <see cref="StringBuilder"/> containing the XML data.
		/// </param>
		/// <returns>
		/// An <see cref="XmlReader"/> over the XML content of the <see cref="StringBuilder"/>.
		/// </returns>
		public static XmlReader GetReaderAtContent(this StringBuilder builder)
		{
			var xmlReader = builder.ToString().Trim()
				.IfNotNullOrEmpty(s => XmlReader.Create(new StringReader(s), new XmlReaderSettings { CloseInput = true }))
				?? EmptyXmlReader.Create();
			xmlReader.MoveToContent();
			return xmlReader;
		}
	}
}
