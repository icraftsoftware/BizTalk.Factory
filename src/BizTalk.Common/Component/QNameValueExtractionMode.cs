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

using System.Xml.Schema;
using Be.Stateless.BizTalk.XPath;

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// Denotes how the value, which might be a QName, pointed to by an <see cref="QNameValueExtractor"/> is extracted.
	/// This is typically the case for the <see cref="XmlSchema.InstanceNamespace"/> type attribute's value.
	/// </summary>
	/// <remarks>
	/// Extraction can either means
	/// <list type="bullet">
	/// <item>
	/// <see cref="Name" /> &#8212; the name() of the QName value is extracted. This is the default extraction mode.
	/// </item>
	/// <item>
	/// <see cref="LocalName" /> &#8212; only the local-name() of the QName value is extracted.
	/// </item>
	/// </list>
	/// </remarks>
	/// <seealso href="http://www.w3.org/TR/xml-names/#ns-qualnames">Qualified Names</seealso>
	public enum QNameValueExtractionMode
	{
		/// <summary>
		/// Default extraction mode, i.e. <see cref="Name"/>.
		/// </summary>
		Default = Name,

		/// <summary>
		/// Extracts the whole QName, i.e. the equivalent of the name() xpath function.
		/// </summary>
		/// <remarks>
		/// Notice that this extraction mode will keep the xml namespace prefix in the extracted value if present and is
		/// therefore not the recommended way to extract an <see cref="XmlSchema.InstanceNamespace"/> type attribute's if
		/// one wants to write a subscription filter against a type name.
		/// </remarks>
		Name = 0,

		/// <summary>
		/// Extracts only the local part QName, i.e. the equivalent of the local-name() xpath function.
		/// </summary>
		/// <remarks>
		/// This extraction mode usually allows to discard the xml namespace prefix from the <see
		/// cref="XmlSchema.InstanceNamespace"/> type attribute's value and therefore allows to write subscription filters
		/// against xml type names without having to care about varying xml namespace prefixes that can mangle the type
		/// name.
		/// </remarks>
		LocalName = 1,
	}
}
