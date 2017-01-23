#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Xml
{
	/// <summary>
	/// Resolves assembly-embedded XML resources named by a Uniform Resource Identifier (URI).
	/// </summary>
	/// <remarks>
	/// The set of Uniform Resource Identifier (URI) that is supported is as follows:
	/// <list type="bullet">
	/// <item>
	/// <c>map://type/&lt;BizTalk Map's strong type name&gt;</c>, for instance
	/// <c>map://type/Be.Stateless.BizTalk.Unit.Transform.IdentityTransform, Be.Stateless.BizTalk.Unit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=3707daa0b119fc14</c>
	/// </item>
	/// <item>
	/// <c>embedded://resource/&lt;embedded resource's full name&gt;</c>, for instance
	/// <c>embedded://resource/Be.Stateless.BizTalk.Xml.Data.Included.xsl</c>
	/// </item>
	/// <item>
	/// <c>embedded://resource/&lt;embedded resource's name&gt;</c>, for instance
	/// <c>embedded://resource/Imported.xsl</c>
	/// </item>
	/// <item>
	/// <c>a file's absolute path without scheme</c>, for instance
	/// <c>C:\Files\Projects\be.stateless\BizTalkFactory\src\BizTalk.Common.Tests\Xml\Imported.xsl</c>. Notice that this
	/// is natively supported by the <see cref="XmlUrlResolver"/> from which this class derives.
	/// </item>
	/// </list>
	/// </remarks>
	public class EmbeddedXmlResolver : XmlUrlResolver
	{
		public EmbeddedXmlResolver(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			_type = type;
		}

		#region Base Class Member Overrides

		/// <summary>
		/// Maps a URI to an object that contains the actual resource.
		/// </summary>
		/// <param name="absoluteUri">
		/// The URI returned from <see cref="ResolveUri"/>.
		/// </param>
		/// <param name="role">
		/// Currently not used.
		/// </param>
		/// <param name="ofObjectToReturn">
		/// The type of object to return.
		/// </param>
		/// <returns>
		/// One of <see cref="Stream"/>, <see cref="XmlReader"/>, or <see cref="IXPathNavigable"/>.
		/// </returns>
		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			// must return one of Stream, XmlReader, or IXPathNavigable
			if (absoluteUri.Scheme == MAP_SCHEME && absoluteUri.Host == TYPE_HOST)
			{
				var typeName = Uri.UnescapeDataString(absoluteUri.Segments[1]);
				var type = Type.GetType(typeName, true);
				var transform = (TransformBase) Activator.CreateInstance(type);
				// http://stackoverflow.com/questions/11864564/xslcompiledtransform-and-custom-xmlurlresolver-an-entry-with-the-same-key-alre
				using (var reader = XmlReader.Create(new StringReader(transform.XmlContent), new XmlReaderSettings(), absoluteUri.GetLeftPart(UriPartial.Authority)))
				{
					// http://stackoverflow.com/questions/1440023/can-i-assign-a-baseuri-to-an-xdocument
					var xDocument = XDocument.Load(reader, LoadOptions.SetBaseUri);
					// XDocument and XPathNavigator do not implement IDisposable while XmlReader does; to avoid IDisposable
					// issues, it is therefore simpler to return an XPathNavigator
					return xDocument.CreateNavigator();
				}
			}

			if (absoluteUri.Scheme == EMBEDDED_SCHEME && absoluteUri.Host == RESOURCE_HOST)
			{
				var assembly = _type.Assembly;
				// first look for a resource referenced by a simple name and if not found (i.e. null) by a full name
				var stream = assembly.GetManifestResourceStream(_type, absoluteUri.Segments[1])
					?? assembly.GetManifestResourceStream(absoluteUri.Segments[1]);
				return stream;
			}

			return base.GetEntity(absoluteUri, role, ofObjectToReturn);
		}

		/// <summary>
		/// Resolves the absolute URI from the base and relative URIs.
		/// </summary>
		/// <param name="baseUri">
		/// The base URI used to resolve the relative URI.
		/// </param>
		/// <param name="relativeUri">
		/// The URI to resolve. The URI can be absolute or relative. If absolute, this value effectively replaces the
		/// baseUri value. If relative, it combines with the baseUri to make an absolute URI.
		/// </param>
		/// <returns>
		/// The absolute URI or null if the relative URI cannot be resolved.
		/// </returns>
		public override Uri ResolveUri(Uri baseUri, string relativeUri)
		{
			var uri = new Uri(relativeUri, UriKind.RelativeOrAbsolute);
			if (uri.Scheme == MAP_SCHEME && uri.Host == TYPE_HOST)
			{
				return uri;
			}

			if (uri.Scheme == EMBEDDED_SCHEME && uri.Host == RESOURCE_HOST)
			{
				return uri;
			}

			return base.ResolveUri(baseUri, relativeUri);
		}

		#endregion

		private const string EMBEDDED_SCHEME = "embedded";
		private const string MAP_SCHEME = "map";
		private const string RESOURCE_HOST = "resource";
		private const string TYPE_HOST = "type";

		private readonly Type _type;
	}
}
