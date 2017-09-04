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
using System.Linq;
using System.Text.RegularExpressions;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.BizTalk.Xml.Xsl.Extensions;
using Be.Stateless.Extensions;
using Microsoft.Dia;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	/// <summary>
	/// Resolve the path to BizTalk maps' custom XSLT.
	/// </summary>
	public static class MapCustomXsltPathResolver
	{
		/// <summary>
		/// Resolve the path to a BizTalk map's custom XSLT.
		/// </summary>
		/// <param name="type">
		/// The BizTalk map for which the path to the custom XSLT should be resolved.
		/// </param>
		/// <param name="path">
		/// The resolved path to the custom XSLT of the BizTalk map passed.
		/// </param>
		/// <returns>
		/// <c>true</c> if the path to the source of the custom XSLT could be resolved; <c>false</c> otherwise.
		/// </returns>
		/// <remarks>
		/// <para>
		/// For its path to be resolved, the custom XSLT source file must reside, by convention, next to its <c>.btm</c>
		/// file and have the same name except for the extension that must either be <c>.xsl</c> or <c>.xslt</c>.
		/// </para>
		/// <para>
		/// Technically, <see cref="MapCustomXsltPathResolver"/> will first try lo locate the <c>.pdb</c> file of the
		/// transform type's assembly. If successful, it will then try to retrieve from the debug information the path of
		/// the generated <c>.btm.cs</c> source file of the <see cref="TransformBase"/>-derived class. If successful, it
		/// will finally try to locate an <c>.xsl</c> or <c>.xslt</c> file next to the <c>.btm.cs</c> (i.e. with the same
		/// name except for the <c>.btm.cs</c> extension).
		/// </para>
		/// </remarks>
		public static bool TryResolveCustomXsltPath(this Type type, out string path)
		{
			if (!type.IsTransform()) throw new ArgumentException("Type is not a TransformBase derived Type instance.", "type");

			path = null;
			string btmSourceFilePath;
			if (!type.TryResolveBtmClassSourceFilePath(out btmSourceFilePath)) return false;

			// probe for .xsl
			path = Regex.Replace(btmSourceFilePath, @"\.(btm\.)?cs$", ".xsl", RegexOptions.IgnoreCase);
			if (File.Exists(path)) return true;
			// probe for .xslt as well
			if (!File.Exists(path + 't')) return false;
			path += 't';
			return true;
		}

		/// <summary>
		/// Resolve the path to a BizTalk map class source file, i.e. the .btm.cs compiler generated source file.
		/// </summary>
		/// <param name="type">
		/// The BizTalk map type for which the path to the source file should be resolved.
		/// </param>
		/// <param name="path">
		/// The resolved path to the source BizTalk map type.
		/// </param>
		/// <returns>
		/// <c>true</c> if the path to the source file of the BizTalk map type could be resolved; <c>false</c> otherwise.
		/// </returns>
		public static bool TryResolveBtmClassSourceFilePath(this Type type, out string path)
		{
			if (!type.IsTransform()) throw new ArgumentException("Type is not a TransformBase derived Type instance.", "type");

			path = null;
			string pdbFilePath;
			if (!type.TryResolvePdbFilePath(out pdbFilePath)) return false;

			var source = (IDiaDataSource) new DiaSourceClass();
			source.loadDataFromPdb(pdbFilePath);

			IDiaSession session;
			source.openSession(out session);

			IDiaSymbol methodSymbol;
			// ReSharper disable once PossibleNullReferenceException
			var metadataToken = type.GetProperty("XmlContent").GetGetMethod().MetadataToken;
			session.findSymbolByToken((uint) metadataToken, SymTagEnum.SymTagFunction, out methodSymbol);
			if (methodSymbol == null) return false;

			IDiaEnumLineNumbers lineNumbers;
			session.findLinesByRVA(methodSymbol.relativeVirtualAddress, 1, out lineNumbers);
			foreach (IDiaLineNumber ln in lineNumbers)
			{
				path = ln.sourceFile.fileName;
				if (!path.IsNullOrEmpty()) return true;
			}
			return false;
		}

		/// <summary>
		/// Resolve the path to an XSLT embedded in a custom BizTalk map, given this map's type to initiate probing.
		/// </summary>
		/// <param name="type">
		/// The BizTalk map type to be used to initiate the probing.
		/// </param>
		/// <param name="resource">
		/// The name of the resource to probe for its source XSLT file.
		/// </param>
		/// <param name="path">
		/// The resolved path to the embedded XSLT.
		/// </param>
		/// <returns>
		/// <c>true</c> if the path to the source of the embedded XSLT could be resolved; <c>false</c> otherwise.
		/// </returns>
		/// <seealso cref="EmbeddedXmlResolver"/>
		public static bool TryResolveEmbeddedXsltResourceSourceFilePath(this Type type, string resource, out string path)
		{
			path = null;
			string classPath;
			if (!type.TryResolveBtmClassSourceFilePath(out classPath)) return false;

			var manifestResourceNames = type.Assembly.GetManifestResourceNames();
			// first, search for a namespace-qualified name match, then for non namespace-qualified name match, then fallback on actual argument's value
			var qualifiedResourceName =
				manifestResourceNames.SingleOrDefault(n => n.Equals(string.Format("{0}.{1}", type.Namespace, resource), StringComparison.Ordinal))
					?? manifestResourceNames.SingleOrDefault(n => n.Equals(resource, StringComparison.Ordinal))
						?? resource;
			var resourceNamespace = qualifiedResourceName.RTrimToChar('.').RTrimToChar('.');
			resource = qualifiedResourceName.Substring(resourceNamespace.Length + 1);

			// ReSharper disable once PossibleNullReferenceException
			var referenceTypeNamespace = type.FullName.Substring(0, type.FullName.Length - type.Name.Length - 1);
			var commonPath = new[] { referenceTypeNamespace, resourceNamespace }.CommonPath(".");

			var trailingTypePathSegments = referenceTypeNamespace
				.Substring(commonPath.Length)
				.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
			var trailingResourcePathSegments = resourceNamespace
				.Substring(commonPath.Length)
				.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);

			// ReSharper disable once AssignNullToNotNullAttribute
			var sourceXsltFilePath = Path.Combine(
				Path.GetDirectoryName(classPath),
				string.Join(@"\", Enumerable.Repeat("..", trailingTypePathSegments.Length)),
				string.Join(@"\", trailingResourcePathSegments),
				resource);
			if (!File.Exists(sourceXsltFilePath)) return false;

			path = sourceXsltFilePath;
			return true;
		}

		private static bool TryResolvePdbFilePath(this Type type, out string path)
		{
			path = Regex.Replace(new Uri(type.Assembly.CodeBase).AbsolutePath, @"\.dll", ".pdb", RegexOptions.IgnoreCase);
			return File.Exists(path);
		}
	}
}
