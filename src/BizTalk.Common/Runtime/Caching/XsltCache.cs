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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Xsl;
using Be.Stateless.BizTalk.Xml.Xsl;
using Be.Stateless.BizTalk.Xml.Xsl.Extensions;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Runtime.Caching
{
	/// <summary>
	/// Runtime memory cache for the <see cref="XslCompiledTransformDescriptor"/>s wrapping the <see
	/// cref="XslCompiledTransform"/> equivalents of <see cref="TransformBase"/>-derived types.
	/// </summary>
	/// <seealso cref="Cache{TKey,TItem}"/>
	[SuppressMessage("ReSharper", "LocalizableElement")]
	public class XsltCache : Cache<Type, XslCompiledTransformDescriptor>
	{
		/// <summary>
		/// Singleton <see cref="XsltCache"/> instance.
		/// </summary>
		public static XsltCache Instance
		{
			get { return _instance; }
		}

		/// <summary>
		/// Create the singleton <see cref="XsltCache"/> instance.
		/// </summary>
		private XsltCache() { }

		#region Base Class Member Overrides

		protected override string ConvertKeyToString(Type key)
		{
			ValidateKey(key);
			return key.AssemblyQualifiedName;
		}

		protected override XslCompiledTransformDescriptor CreateItem(Type key)
		{
			ValidateKey(key);
			return new XslCompiledTransformDescriptor(new XslCompiledTransformDescriptorBuilder(key));
		}

		#endregion

		#region Helpers

		[Conditional("DEBUG")]
		private void ValidateKey(Type key)
		{
			if (!key.IsTransform()) throw new ArgumentException("Type is not a TransformBase derived Type instance.", "key");
		}

		#endregion

		private static readonly XsltCache _instance = new XsltCache();
	}
}
