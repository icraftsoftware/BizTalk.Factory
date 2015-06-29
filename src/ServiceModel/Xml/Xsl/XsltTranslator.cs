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
using System.Xml.Xsl;
using Be.Stateless.ServiceModel.Channels;

namespace Be.Stateless.Xml.Xsl
{
	/// <summary>
	/// Translates <see cref="XmlMessage"/>s back and forth to native generic WCF <see cref="Message"/>s by applying an
	/// <see cref="XslCompiledTransform"/> to their payloads.
	/// </summary>
	public class XsltTranslator<TRequestTransform, TResponseTransform> : XsltTranslatorBase
		where TRequestTransform : XsltTransformBase, new()
		where TResponseTransform : XsltTransformBase, new()
	{
		static XsltTranslator()
		{
			// TODO use XsltCache or equivalent
			// keep ref to compiled XSLTs at class level to avoid having to regenerate them and leak dynamic assemblies
			_requestXslt = new TRequestTransform().Xslt;
			_responseXslt = new TResponseTransform().Xslt;
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

		#endregion

		// ReSharper disable StaticFieldInGenericType
		private static readonly XslCompiledTransform _requestXslt;
		private static readonly XslCompiledTransform _responseXslt;
		// ReSharper restore StaticFieldInGenericType
	}
}
