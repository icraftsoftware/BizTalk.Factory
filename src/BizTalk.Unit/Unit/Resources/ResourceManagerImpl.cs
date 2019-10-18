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

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

namespace Be.Stateless.BizTalk.Unit.Resources
{
	internal class ResourceManagerImpl : IResourceManager
	{
		internal ResourceManagerImpl(Assembly assembly)
		{
			if (assembly == null) throw new ArgumentNullException("assembly");
			_assembly = assembly;
		}

		public ResourceManagerImpl(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			_assembly = type.Assembly;
			_type = type;
		}

		#region IResourceManager Members

		public Stream Load(string name)
		{
			var actualName = _type != null ? ResolveTypeScopedManifestResourceName(name) : ResolveAssemblyScopedManifestResourceName(name);
			var stream = _assembly.GetManifestResourceStream(actualName);
			if (stream == null) throw new FileNotFoundException(string.Format("Cannot find resource '{0}' in assembly {1}.", name, _assembly.FullName), name);
			return stream;
		}

		public TR Load<TR>(string name, Func<Stream, TR> deserializer)
		{
			using (var stream = Load(name))
			{
				return deserializer(stream);
			}
		}

		public string LoadString(string name)
		{
			return Load(
				name,
				stream => {
					using (var reader = new StreamReader(stream))
					{
						return reader.ReadToEnd();
					}
				});
		}

		public XslCompiledTransform LoadTransform(string name)
		{
			return Load(
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

		public string LoadXmlString(string name)
		{
			return Load(
				name,
				stream => {
					var xmlDocument = new XmlDocument();
					xmlDocument.Load(stream);
					return xmlDocument.OuterXml;
				});
		}

		#endregion

		private string ResolveAssemblyScopedManifestResourceName(string name)
		{
			var manifestResourceNames = _assembly.GetManifestResourceNames();
			return manifestResourceNames.SingleOrDefault(n => n.Equals(name, StringComparison.Ordinal))
				?? manifestResourceNames.SingleOrDefault(n => n.EndsWith(name, StringComparison.Ordinal))
					?? name;
		}

		private string ResolveTypeScopedManifestResourceName(string name)
		{
			var manifestResourceNames = _assembly.GetManifestResourceNames();
			return manifestResourceNames.SingleOrDefault(n => n.Equals(string.Format("{0}.{1}", _type.Namespace, name), StringComparison.Ordinal))
				?? name;
		}

		private readonly Assembly _assembly;
		private readonly Type _type;
	}
}
