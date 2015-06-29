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

using System.Xml;
using System.Xml.Xsl;
using Be.Stateless.Resources;

namespace Be.Stateless.Xml.Xsl
{
	/// <summary>
	/// Base class that loads the content of an <see cref="XslCompiledTransform"/> from the <see
	/// cref="XsltTransformBase"/>-derived type's assembly resources.
	/// </summary>
	public abstract class XsltTransformBase
	{
		/// <summary>
		/// Initializes a new instance of a <see cref="XsltTransformBase"/>-derived class.
		/// </summary>
		protected XsltTransformBase()
		{
			_xslt = LoadXslCompiledTransform();
		}

		/// <summary>
		/// The <see cref="XslCompiledTransform"/> whose content has been loaded from the <see
		/// cref="XsltTransformBase"/>-derived type's assembly resources.
		/// </summary>
		public XslCompiledTransform Xslt
		{
			get { return _xslt; }
		}

		private XslCompiledTransform LoadXslCompiledTransform()
		{
			var type = GetType();
			return ResourceManager.Load(
				type.Assembly,
				type.FullName,
				stream => {
					using (var xmlReader = XmlReader.Create(stream))
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
				});
		}

		private readonly XslCompiledTransform _xslt;
	}
}
