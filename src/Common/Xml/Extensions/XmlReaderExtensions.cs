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
using System.Xml;

namespace Be.Stateless.Xml.Extensions
{
	public static class XmlReaderExtensions
	{
		/// <summary>
		/// Calls <see cref="XmlReader.MoveToContent"/> and tests if the current content node is a start tag or empty
		/// element tag whose <see cref="XmlReader.Name"/> matches the given <paramref name="name"/> argument; throws
		/// otherwise.
		/// </summary>
		/// <param name="reader">
		/// An <see cref="XmlReader"/> object.
		/// </param>
		/// <param name="name">
		/// The qualified name of the element.
		/// </param>
		public static void AssertStartElement(this XmlReader reader, string name)
		{
			if (reader.IsStartElement(name)) return;
			var info = (IXmlLineInfo) reader;
			throw new XmlException(string.Format("Element '{0}' was not found. Line {1}, position {2}.", name, info.LineNumber, info.LinePosition));
		}

		/// <summary>
		/// Gets the value of the attribute with the specified <paramref name="name"/>; throws if the value is either not
		/// found or <see cref="string.Empty"/>.
		/// </summary>
		/// <param name="reader">
		/// An <see cref="XmlReader"/> object.
		/// </param>
		/// <param name="name">
		/// The qualified name of the attribute.
		/// </param>
		/// <returns>
		/// The value of the specified attribute.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="name"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="XmlException">
		/// The attribute is not found or the value is <see cref="string.Empty"/>.
		/// </exception>
		public static string GetMandatoryAttribute(this XmlReader reader, string name)
		{
			if (reader.MoveToAttribute(name)) return reader.Value;
			var info = (IXmlLineInfo) reader;
			throw new XmlException(string.Format("Attribute '{0}' was not found. Line {1}, position {2}.", name, info.LineNumber, info.LinePosition));
		}
	}
}
