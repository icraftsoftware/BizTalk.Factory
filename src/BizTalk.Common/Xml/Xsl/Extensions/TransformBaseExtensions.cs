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
using Be.Stateless.BizTalk.Runtime.Caching;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Xml.Xsl.Extensions
{
	public static class TransformBaseExtensions
	{
		/// <summary>
		/// Returns whether the <paramref name="type"/> is a <see cref="TransformBase"/>-derived transform type.
		/// </summary>
		/// <param name="type">
		/// The <see cref="Type"/> to inspect.
		/// </param>
		/// <returns>
		/// <c>true</c> if <paramref name="type"/> is a <see cref="TransformBase"/>-derived <see cref="Type"/>s.
		/// </returns>
		public static bool IsTransform(this Type type)
		{
			return (type != null && type.BaseType == typeof(TransformBase));
		}

		/// <summary>
		/// The <see cref="XmlWriterSettings"/> for a <see cref="TransformBase"/>-derived <see cref="Type"/>s.
		/// </summary>
		/// <param name="type">
		/// The <see cref="TransformBase"/>-derived <see cref="Type"/>.
		/// </param>
		/// <returns>
		/// The <see cref="XmlWriterSettings"/> related to the <see cref="TransformBase"/>-derived <see cref="Type"/>.
		/// </returns>
		public static XmlWriterSettings GetOutputSettings(this Type type)
		{
			if (!type.IsTransform()) throw new ArgumentException("Type is not a TransformBase derived Type instance.", "type");
			return _transformDescriptorFactory(type).XslCompiledTransform.OutputSettings;
		}

		#region Mock's Factory Hook Point

		internal static Func<Type, XslCompiledTransformDescriptor> TransformDescriptorFactory
		{
			get { return _transformDescriptorFactory; }
			set { _transformDescriptorFactory = value; }
		}

		#endregion

		private static Func<Type, XslCompiledTransformDescriptor> _transformDescriptorFactory = type => XsltCache.Instance[type];
	}
}
