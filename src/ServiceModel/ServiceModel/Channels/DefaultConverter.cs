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

namespace Be.Stateless.ServiceModel.Channels
{
	/// <summary>
	/// A default <see cref="IXmlMessageConverter"/> converter that merely converts from an <see cref="XmlMessage"/> to a
	/// generic WCF <see cref="Message"/> and back.
	/// </summary>
	/// <remarks>
	/// This class acts as a mere converter and consequently does not translate any of the message's SOAP <see
	/// cref="MessageVersion"/>, action, or payload/body.
	/// </remarks>
	public class DefaultConverter : IXmlMessageConverter
	{
		/// <summary>
		/// Singleton instance
		/// </summary>
		public static IXmlMessageConverter Instance
		{
			get { return _instance; }
		}

		private DefaultConverter() { }

		#region IXmlMessageConverter Members

		/// <summary>
		/// Converts an <see cref="XmlMessage"/> to its <see cref="Message"/> equivalent.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/> to convert.
		/// </typeparam>
		/// <param name="xmlRequest">
		/// The <see cref="XmlMessage"/> to convert.
		/// </param>
		/// <returns>
		/// The converted WCF <see cref="Message"/>.
		/// </returns>
		public Message CreateMessageRequestFromXmlRequest<TRequest>(TRequest xmlRequest)
			where TRequest : XmlMessage
		{
			if (xmlRequest == null) throw new ArgumentNullException("xmlRequest");
			return Message.CreateMessage(xmlRequest.Version, xmlRequest.Action, xmlRequest.GetReaderAtContent());
		}

		/// <summary>
		/// Converts a generic WCF <see cref="Message"/> to an an <see cref="XmlMessage"/>.
		/// </summary>
		/// <typeparam name="TResponse">
		/// The type of the resulting converted <see cref="XmlMessage"/>.
		/// </typeparam>
		/// <param name="response">
		/// The generic WCF <see cref="Message"/> to convert.
		/// </param>
		/// <returns>
		/// The converted <see cref="XmlMessage"/>.
		/// </returns>
		public TResponse CreateXmlResponseFromMessageResponse<TResponse>(Message response)
			where TResponse : XmlMessage, new()
		{
			if (response == null) throw new ArgumentNullException("response");
			using (var xmlReader = response.GetReaderAtBodyContents())
			{
				var xmlResponse = new TResponse();
				xmlResponse.ReadXml(xmlReader);
				return xmlResponse;
			}
		}

		#endregion

		private static readonly IXmlMessageConverter _instance = new DefaultConverter();
	}
}
