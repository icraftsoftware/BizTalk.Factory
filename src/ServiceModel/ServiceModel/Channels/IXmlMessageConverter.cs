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

namespace Be.Stateless.ServiceModel.Channels
{
	/// <summary>
	/// Scaffolding interface meant to help the service relay, together made of the <see cref="ServiceRelay"/> and its
	/// necessary <see cref="ClientRelay"/> counterpart, to convert <see cref="XmlMessage"/>s back and forth to native
	/// generic WCF <see cref="Message"/>s.
	/// </summary>
	public interface IXmlMessageConverter
	{
		/// <summary>
		/// Converts an <see cref="XmlMessage"/> to a generic WCF <see cref="Message"/>.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/> to convert.
		/// </typeparam>
		/// <param name="xmlRequest">
		/// The <see cref="XmlMessage"/> to convert.
		/// </param>
		/// <returns>
		/// The converted generic WCF <see cref="Message"/>.
		/// </returns>
		Message CreateMessageRequestFromXmlRequest<TRequest>(TRequest xmlRequest)
			where TRequest : XmlMessage;

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
		TResponse CreateXmlResponseFromMessageResponse<TResponse>(Message response)
			where TResponse : XmlMessage, new();
	}
}
