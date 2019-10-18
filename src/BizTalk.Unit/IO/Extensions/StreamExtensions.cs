#region Copyright & License

// Copyright © 2012 - 2019 François Chabot
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
using System.Xml;
using Be.Stateless.BizTalk.Xml;
using Microsoft.BizTalk.Streaming;
using XmlTranslatorStream = Be.Stateless.BizTalk.Streaming.XmlTranslatorStream;

namespace Be.Stateless.BizTalk.IO.Extensions
{
	public static class StreamExtensions
	{
		/// <summary>
		/// Applies a set of <see cref="XmlNamespaceTranslation"/> translations to an XML <see cref="Stream"/>.
		/// </summary>
		/// <param name="stream">
		/// The XML <see cref="Stream"/> to be translated.
		/// </param>
		/// <param name="translations">
		/// The set of <see cref="XmlNamespaceTranslation"/> translations to apply.
		/// </param>
		/// <returns>
		/// The translated <see cref="Stream"/>.
		/// </returns>
		public static Stream Translate(this Stream stream, XmlNamespaceTranslation[] translations)
		{
			return new ReadOnlySeekableStream(new XmlTranslatorStream(XmlReader.Create(stream), translations));
		}
	}
}
