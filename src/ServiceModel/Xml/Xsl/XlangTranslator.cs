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
using System.IO;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Xsl;
using Be.Stateless.ServiceModel.Channels;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.Xml.Xsl
{
	/// <summary>
	/// Translates <see cref="XmlMessage"/>s back and forth to native generic WCF <see cref="Message"/>s by applying an
	/// <see cref="TransformBase"/>-derived type XSLT transform to their payloads.
	/// </summary>
	public class XlangTranslator<TRequestTransform, TResponseTransform> : XsltTranslatorBase
		where TRequestTransform : TransformBase, new()
		where TResponseTransform : TransformBase, new()
	{
		static XlangTranslator()
		{
			// TODO use XsltCache or equivalent
			// keep ref to compiled XSLTs at class level to avoid having to regenerate them and leak dynamic assemblies
			var requestTransform = (TransformBase) Activator.CreateInstance<TRequestTransform>();
			_requestArguments = new XsltArgumentList(requestTransform.TransformArgs);
			_requestXslt = LoadXslCompiledTransform(requestTransform);

			var responseTransform = (TransformBase) Activator.CreateInstance<TResponseTransform>();
			_responseArguments = new XsltArgumentList(responseTransform.TransformArgs);
			_responseXslt = LoadXslCompiledTransform(responseTransform);
		}

		private static XslCompiledTransform LoadXslCompiledTransform(TransformBase transform)
		{
			using (var xmlReader = XmlReader.Create(new StringReader(transform.XmlContent), new XmlReaderSettings { CloseInput = true }))
			{
				var xslt =
#if DEBUG
					new XslCompiledTransform(true);
#else
					new XslCompiledTransform();
#endif
				xslt.Load(xmlReader, XsltSettings.TrustedXslt, new XmlUrlResolver());
				return xslt;
			}
		}

		#region Base Class Member Overrides

		/// <summary>
		/// <see cref="XslCompiledTransform"/> to apply to the request <see cref="XmlMessage"/>'s body.
		/// </summary>
		protected override XslCompiledTransform RequestXslt
		{
			get { return _requestXslt; }
		}

		/// <summary>
		/// <see cref="XslCompiledTransform"/> to apply to get the response <see cref="XmlMessage"/>'s body.
		/// </summary>
		protected override XslCompiledTransform ResponseXslt
		{
			get { return _responseXslt; }
		}

		/// <summary>
		/// Translates a request <see cref="XmlMessage"/>-derived message's body, whose content is given by <paramref
		/// name="reader"/>, and outputs the results to <paramref name="writer"/>.
		/// </summary>
		/// <param name="reader">
		/// The <see cref="XmlReader"/> containing the input document.
		/// </param>
		/// <param name="writer">
		/// The <see cref="XmlWriter"/> that will contain the output of the transform.
		/// </param>
		/// <remarks>
		/// This override reuses the extension objects that have been defined inside the <typeparamref
		/// name="TRequestTransform"/>.
		/// </remarks>
		protected override void TranslateRequest(XmlReader reader, XmlWriter writer)
		{
			RequestXslt.Transform(reader, RequestXsltArguments, writer);
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
		/// This override reuses the extension objects that have been defined inside the <typeparamref
		/// name="TResponseTransform"/>.
		/// </remarks>
		protected override void TranslateResponse(XmlReader reader, XmlWriter writer)
		{
			ResponseXslt.Transform(reader, ResponseXsltArguments, writer);
		}

		#endregion

		/// <summary>
		/// The extension objects that have been defined inside the <typeparamref name="TRequestTransform"/>.
		/// </summary>
		protected virtual XsltArgumentList RequestXsltArguments
		{
			get { return _requestArguments; }
		}

		/// <summary>
		/// The extension objects that have been defined inside the <typeparamref name="TResponseTransform"/>.
		/// </summary>
		protected virtual XsltArgumentList ResponseXsltArguments
		{
			get { return _responseArguments; }
		}

		protected const int DEFAULT_CAPACITY = 1024;
		// ReSharper disable StaticFieldInGenericType
		private static readonly XsltArgumentList _requestArguments;
		private static readonly XslCompiledTransform _requestXslt;
		private static readonly XsltArgumentList _responseArguments;
		private static readonly XslCompiledTransform _responseXslt;
		// ReSharper restore StaticFieldInGenericType
	}
}
