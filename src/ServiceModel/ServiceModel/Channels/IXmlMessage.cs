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
using System.Xml.Serialization;

namespace Be.Stateless.ServiceModel.Channels
{
	/// <summary>
	/// Represents an <see cref="IXmlSerializable"/> unit of communication between endpoints in a distributed
	/// environment.
	/// </summary>
	public interface IXmlMessage
	{
		/// <summary>
		/// Gets a description of how the message should be processed.
		/// </summary>
		string Action { get; }

		/// <summary>
		/// Gets the SOAP version of the message.
		/// </summary>
		MessageVersion Version { get; }

		/// <summary>
		/// Gets the <see cref="XmlReader"/> that accesses the body content of this message.
		/// </summary>
		/// <returns>
		/// An <see cref="XmlReader"/>.
		/// </returns>
		XmlReader GetReaderAtBodyContents();
	}
}
