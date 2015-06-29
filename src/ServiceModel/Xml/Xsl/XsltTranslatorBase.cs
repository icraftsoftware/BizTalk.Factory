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
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using Be.Stateless.ServiceModel.Channels;
using Be.Stateless.Text.Extensions;

namespace Be.Stateless.Xml.Xsl
{
	/// <summary>
	/// Base strategy that translates <see cref="XmlMessage"/>s back and forth to native generic WCF <see
	/// cref="Message"/>s by applying an <see cref="XslCompiledTransform"/> to their payloads.
	/// </summary>
	public abstract class XsltTranslatorBase : IXmlMessageConverter
	{
		#region IXmlMessageConverter Members

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
		public virtual Message CreateMessageRequestFromXmlRequest<TRequest>(TRequest xmlRequest)
			where TRequest : XmlMessage
		{
			if (xmlRequest == null) throw new ArgumentNullException("xmlRequest");

			var builder = new StringBuilder(DEFAULT_CAPACITY);
			using (var xmlReader = xmlRequest.GetReaderAtContent())
			using (var xmlWriter = XmlWriter.Create(builder, RequestXslt.OutputSettings))
			{
				TranslateRequest(xmlReader, xmlWriter);
			}
			var reader = builder.GetReaderAtContent();
			return CreateTranslatedRequestMessage(xmlRequest.Version, xmlRequest.Action, reader);
		}

		/// <summary>
		/// Translates a generic WCF <see cref="Message"/> to an an <see cref="XmlMessage"/>.
		/// </summary>
		/// <typeparam name="TResponse">
		/// The type of the resulting translated <see cref="XmlMessage"/>.
		/// </typeparam>
		/// <param name="response">
		/// The generic WCF <see cref="Message"/> to translate.
		/// </param>
		/// <returns>
		/// The translated <see cref="XmlMessage"/>.
		/// </returns>
		public virtual TResponse CreateXmlResponseFromMessageResponse<TResponse>(Message response)
			where TResponse : XmlMessage, new()
		{
			if (response == null) throw new ArgumentNullException("response");

			var builder = new StringBuilder(DEFAULT_CAPACITY);
			using (var xmlReader = response.GetReaderAtBodyContents())
			using (var xmlWriter = XmlWriter.Create(builder, ResponseXslt.OutputSettings))
			{
				TranslateResponse(xmlReader, xmlWriter);
			}
			using (var xmlReader = builder.GetReaderAtContent())
			{
				var xmlResponse = new TResponse();
				BuildTranslatedResponseXmlMessage(xmlResponse, xmlReader);
				return xmlResponse;
			}
		}

		#endregion

		/// <summary>
		/// <see cref="XslCompiledTransform"/> to apply to the request <see cref="XmlMessage"/>'s body.
		/// </summary>
		protected abstract XslCompiledTransform RequestXslt { get; }

		/// <summary>
		/// <see cref="XslCompiledTransform"/> to apply to get the response <see cref="XmlMessage"/>'s body.
		/// </summary>
		protected abstract XslCompiledTransform ResponseXslt { get; }

		/// <summary>
		/// Loads an <see cref="XmlMessage"/>'s translated XML representation.
		/// </summary>
		/// <param name="xmlResponse">
		/// The <see cref="XmlMessage"/> to load XML content into.
		/// </param>
		/// <param name="xmlReader">
		/// The XML content to load.
		/// </param>
		protected virtual void BuildTranslatedResponseXmlMessage(XmlMessage xmlResponse, XmlReader xmlReader)
		{
			xmlResponse.ReadXml(xmlReader);
		}

		/// <summary>
		/// Creates the translated request <see cref="Message"/> using the specified <paramref name="reader"/>, <paramref
		/// name="action"/> and <paramref name="version"/>.
		/// </summary>
		/// <param name="version">
		/// A <see cref="MessageVersion"/> object that specifies the SOAP version to use for the message. It defaults to
		/// the version of the incoming <see cref="XmlMessage"/> request; see <see cref="XmlMessage.Version"/>.
		/// </param>
		/// <param name="action">
		/// A description of how the message should be processed. It defaults to the action of the incoming <see
		/// cref="XmlMessage"/> request; see <see cref="XmlMessage.Action"/>.
		/// </param>
		/// <param name="reader">
		/// The <see cref="XmlReader"/> object to be used for reading the SOAP message. It defaults to the output of the
		/// <see cref="RequestXslt"/> that has been applied to the body of the request <see cref="XmlMessage"/>.
		/// </param>
		/// <returns>
		/// A <see cref="Message"/> object for the message created.
		/// </returns>
		protected virtual Message CreateTranslatedRequestMessage(MessageVersion version, string action, XmlReader reader)
		{
			return Message.CreateMessage(version, action, reader);
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
		protected virtual void TranslateRequest(XmlReader reader, XmlWriter writer)
		{
			RequestXslt.Transform(reader, writer);
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
		protected virtual void TranslateResponse(XmlReader reader, XmlWriter writer)
		{
			ResponseXslt.Transform(reader, writer);
		}

		private const int DEFAULT_CAPACITY = 1024 * 10;
	}
}
