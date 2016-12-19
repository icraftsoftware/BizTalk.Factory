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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Xsl;

namespace Be.Stateless.BizTalk.Unit.Resources
{
	public static class ResourceManager
	{
		/// <summary>
		/// Loads and deserializes an <see cref="Stream"/> embedded in the calling assembly.
		/// </summary>
		/// <param name="name">
		/// The name of the <see cref="Stream"/> resource.
		/// </param>
		/// <returns>
		/// The deserialized <see cref="Stream"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Stream Load(string name)
		{
			return Load(Assembly.GetCallingAssembly(), name);
		}

		/// <summary>
		/// Loads and deserializes a resource embedded in the calling assembly.
		/// </summary>
		/// <typeparam name="T">
		/// The type of the deserialized resource.
		/// </typeparam>
		/// <param name="name">
		/// The name of the resource.
		/// </param>
		/// <param name="deserializer">
		/// A delegate to a method that can deserialize the resource from its stream.
		/// </param>
		/// <returns>
		/// The deserialized resource.
		/// </returns>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static T Load<T>(string name, Func<Stream, T> deserializer)
		{
			return Load(Assembly.GetCallingAssembly(), name, deserializer);
		}

		/// <summary>
		/// Loads and deserializes an <see cref="XslCompiledTransform"/> embedded in the calling assembly.
		/// </summary>
		/// <param name="name">
		/// The name of the <see cref="XslCompiledTransform"/> resource.
		/// </param>
		/// <returns>
		/// The deserialized <see cref="XslCompiledTransform"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static XslCompiledTransform LoadCompiledTransform(string name)
		{
			return Load(
				Assembly.GetCallingAssembly(),
				name,
				stream => {
					using (var xmlReader = XmlReader.Create(stream))
					{
						var compiledTransform = new XslCompiledTransform(true);
						compiledTransform.Load(xmlReader, XsltSettings.TrustedXslt, new XmlUrlResolver());
						return compiledTransform;
					}
				});
		}

		/// <summary>
		/// Loads and deserializes an <see cref="string"/> embedded in the calling assembly.
		/// </summary>
		/// <param name="name">
		/// The name of the <see cref="string"/> resource.
		/// </param>
		/// <returns>
		/// The deserialized <see cref="string"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string LoadString(string name)
		{
			return Load(
				Assembly.GetCallingAssembly(),
				name,
				stream => {
					using (var reader = new StreamReader(stream))
					{
						return reader.ReadToEnd();
					}
				});
		}

		/// <summary>
		/// Loads and deserializes an XML <see cref="string"/> embedded in the calling assembly.
		/// </summary>
		/// <param name="name">
		/// The name of the XML <see cref="string"/> resource.
		/// </param>
		/// <returns>
		/// The deserialized XML <see cref="string"/>.
		/// </returns>
		/// <remarks>
		/// Contrasting with <see cref="LoadString"/>, this methods strips the XML string from all new-line and
		/// indentation formatting characters.
		/// </remarks>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string LoadXmlString(string name)
		{
			return Load(
				Assembly.GetCallingAssembly(),
				name,
				stream => {
					var xmlDocument = new XmlDocument();
					xmlDocument.Load(stream);
					return xmlDocument.OuterXml;
				});
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static T Load<T>(Assembly assembly, string name, Func<Stream, T> deserializer)
		{
			var resourceManagerCallerFrame = new StackFrame(2);
			using (var stream = Load(assembly, resourceManagerCallerFrame.GetMethod().DeclaringType, name))
			{
				return deserializer(stream);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Stream Load(Assembly assembly, string name)
		{
			var resourceManagerCallerFrame = new StackFrame(2);
			return Load(assembly, resourceManagerCallerFrame.GetMethod().DeclaringType, name);
		}

		private static Stream Load(Assembly assembly, Type declaringType, string name)
		{
			var manifestResourceNames = assembly.GetManifestResourceNames();
			// first, search for a namespace-qualified name match, then for non namespace-qualified name match, then fallback on actual argument's value
			var actualName = manifestResourceNames.SingleOrDefault(n => n.Equals(string.Format("{0}.{1}", declaringType.Namespace, name), StringComparison.Ordinal))
				?? manifestResourceNames.SingleOrDefault(n => n.Equals(name, StringComparison.Ordinal))
					?? name;
			var stream = assembly.GetManifestResourceStream(actualName);
			if (stream == null) throw new FileNotFoundException(string.Format("Cannot find resource '{0}' in assembly {1}.", name, assembly.FullName), name);
			return stream;
		}
	}
}
