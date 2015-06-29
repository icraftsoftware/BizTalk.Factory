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

using System.ServiceModel.Channels;
using System.Xml;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.ServiceModel.Channels;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.Xml.Xsl
{
	/// <summary>
	/// Translates <see cref="XmlMessage"/>s back and forth to native generic WCF <see cref="Message"/>s by applying an
	/// <see cref="TransformBase"/>-derived type XSLT transform to their payloads.
	/// </summary>
	public class CompositeXlangTranslator<TRequestTransform, TResponseTransform> : XlangTranslator<TRequestTransform, TResponseTransform>
		where TRequestTransform : TransformBase, new()
		where TResponseTransform : TransformBase, new()
	{
		#region Base Class Member Overrides

		/// <summary>
		/// Translates an <see cref="XmlMessage"/> to a generic WCF <see cref="Message"/>.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/> to translate.
		/// </typeparam>
		/// <param name="xmlRequest">
		/// The <see cref="XmlMessage"/> to translate.
		/// </param>
		/// <returns>
		/// The translated generic WCF <see cref="Message"/>.
		/// </returns>
		/// <remarks>
		/// It grabs a reference to the incoming <see cref="XmlMessage"/>-derived request before delegating to base class.
		/// </remarks>
		public override Message CreateMessageRequestFromXmlRequest<TRequest>(TRequest xmlRequest)
		{
			_xmlRequest = xmlRequest;
			return base.CreateMessageRequestFromXmlRequest(xmlRequest);
		}

		/// <summary>
		/// Translates a response <see cref="Message"/>-derived message's body, whose content is given by <paramref
		/// name="reader"/>, and outputs the results to <paramref name="writer"/>.
		/// </summary>
		/// <param name="reader">
		/// The <see cref="XmlReader"/> containing the input document.
		/// </param>
		/// <param name="writer">
		/// The <see cref="XmlWriter"/> that will contain the output of the transform.
		/// </param>
		/// <remarks>
		/// It wraps both the original <see cref="XmlMessage"/>-derived request's <see cref="XmlReader"/> and the <see
		/// cref="Message"/> response's <see cref="XmlReader"/> in a <see cref="CompositeXmlReader"/> before applying the
		/// <typeparamref name="TResponseTransform"/> transform.
		/// </remarks>
		protected override void TranslateResponse(XmlReader reader, XmlWriter writer)
		{
			using (var requestBodyReader = _xmlRequest.GetReaderAtContent())
			using (var compositeReader = CompositeXmlReader.Create(new[] { requestBodyReader, reader }))
			{
				base.TranslateResponse(compositeReader, writer);
			}
		}

		#endregion

		private XmlMessage _xmlRequest;
	}
}
