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

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace Be.Stateless.BizTalk.Xml.Xsl.Extensions
{
	internal static class XslCompiledTransformExtensions
	{
		/// <summary>
		/// Executes the transform using the <see cref="Stream"/> as input document specified and outputs the results to
		/// an <see cref="XmlWriter"/>.
		/// </summary>
		/// <param name="this">
		/// The <see cref="XslCompiledTransform"/> transform to execute.
		/// </param>
		/// <param name="stream">
		/// A <see cref="Stream"/> containing the input document.
		/// </param>
		/// <param name="arguments">
		/// An <see cref="T:System.Xml.Xsl.XsltArgumentList"/> containing the namespace-qualified arguments used as input
		/// to the transform.
		/// </param>
		/// <param name="writer">
		/// The <see cref="XmlWriter"/> to which to output.
		/// </param>
		internal static void Transform(this XslCompiledTransform @this, Stream stream, XsltArgumentList arguments, XmlWriter writer)
		{
			using (var xmlReader = XmlReader.Create(stream, new XmlReaderSettings { CloseInput = true }))
			{
				@this.Transform(xmlReader, arguments, writer);
			}
		}

		/// <summary>
		/// Executes the transform using a compound of <see cref="Stream"/>s as input document specified and outputs the
		/// results to an <see cref="XmlWriter"/>. The <paramref name="streams"/> compound will be aggregated in a 
		/// composite XML structure by the <see cref="CompositeXmlReader"/>.
		/// </summary>
		/// <param name="this">
		/// The <see cref="XslCompiledTransform"/> transform to execute.
		/// </param>
		/// <param name="streams">
		/// The input <see cref="Stream"/> compound.
		/// </param>
		/// <param name="arguments">
		/// An <see cref="T:System.Xml.Xsl.XsltArgumentList"/> containing the namespace-qualified arguments used as input
		/// to the transform.
		/// </param>
		/// <param name="writer">
		/// The <see cref="XmlWriter"/> to which to output.
		/// </param>
		/// <seealso cref="CompositeXmlReader"/>
		internal static void Transform(this XslCompiledTransform @this, IEnumerable<Stream> streams, XsltArgumentList arguments, XmlWriter writer)
		{
			using (var xmlReader = CompositeXmlReader.Create(streams, new XmlReaderSettings { CloseInput = true }))
			{
				@this.Transform(xmlReader, arguments, writer);
			}
		}
	}
}
