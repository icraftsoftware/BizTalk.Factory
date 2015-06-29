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

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Be.Stateless.BizTalk.XPath;
using Microsoft.BizTalk.Streaming;

namespace Be.Stateless.BizTalk.Streaming
{
#pragma warning disable 1584,1711,1572,1581,1580
	/// <summary>
	/// <see cref="XPathMutatorStream"/> factory that goes along with <see cref="XPathExtractorCollection"/> and <see
	/// cref="T:Be.Stateless.BizTalk.Component.ContextPropertyExtractorComponent"/>.
	/// </summary>
#pragma warning restore 1584,1711,1572,1581,1580
	public static class XPathMutatorStreamFactory
	{
		public static XPathMutatorStream Create(Stream stream, IEnumerable<XPathExtractor> extractors, Action<XmlQualifiedName, string, ExtractionMode> extractionDelegate)
		{
			if (stream == null) throw new ArgumentNullException("stream");
			if (extractors == null) throw new ArgumentNullException("extractors");
			if (extractionDelegate == null) throw new ArgumentNullException("extractionDelegate");

			var rxec = new ReactiveXPathExtractorCollection(extractors, extractionDelegate);
			return new XPathMutatorStream(stream, rxec, rxec.OnMatch);
		}
	}
}
