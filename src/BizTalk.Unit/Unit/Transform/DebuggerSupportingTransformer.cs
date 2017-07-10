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
using System.Text.RegularExpressions;
using Be.Stateless.BizTalk.Streaming.Extensions;
using Be.Stateless.BizTalk.Xml.Xsl;
using Dia2Lib;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	internal class DebuggerSupportingTransformer : Transformer
	{
		public DebuggerSupportingTransformer(Stream[] streams) : base(streams) { }

		#region Base Class Member Overrides

		protected override XslCompiledTransformDescriptor LookupTransformDescriptor(Type transform)
		{
			string sourceXsltFilePath;
			return TryResolveSourceXsltFilePath(transform, out sourceXsltFilePath)
				// only hit this code if a debugger is attached (see TransformFixture<T> static ctor); it should therefore
				// not be a performance issue to bypass the XsltCache in order to, most certainly, debug an XSLT map.
				// besides, XsltCache would only always return an XslCompiledTransformDescriptor and not its
				// DebuggerSupportingXslCompiledTransformDescriptor derived class.
				? new DebuggerSupportingXslCompiledTransformDescriptor(transform, sourceXsltFilePath)
				: base.LookupTransformDescriptor(transform);
		}

		#endregion

		private bool TryResolveSourceXsltFilePath(Type transform, out string path)
		{
			path = null;
			string btmSourceFilePath;
			if (!TryResolveBtmSourceFilePath(transform, out btmSourceFilePath)) return false;

			path = Regex.Replace(btmSourceFilePath, @"\.btm\.cs", ".xsl", RegexOptions.IgnoreCase);
			if (File.Exists(path)) return true;
			// probe for .xslt as well and not only .xsl
			if (!File.Exists(path + 't')) return false;
			path += 't';
			return true;
		}

		private bool TryResolveBtmSourceFilePath(Type type, out string path)
		{
			path = null;
			string pdbFilePath;
			if (!TryResolvePdbFilePath(type, out pdbFilePath)) return false;

			// Visual Studio 2013 Diagnostic Library, i.e. msdia120.dll
			var source = (IDiaDataSource) Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("3bfcea48-620f-4b6b-81f7-b9af75454c7d")));
			source.loadDataFromPdb(pdbFilePath);

			IDiaSession session;
			source.openSession(out session);

			IDiaSymbol methodSymbol;
			// ReSharper disable once PossibleNullReferenceException
			session.findSymbolByToken((uint) type.GetProperty("XmlContent").GetGetMethod().MetadataToken, SymTagEnum.SymTagFunction, out methodSymbol);
			if (methodSymbol == null) return false;

			IDiaEnumLineNumbers lineNumbers;
			session.findLinesByRVA(methodSymbol.relativeVirtualAddress, 1, out lineNumbers);
			foreach (IDiaLineNumber ln in lineNumbers)
			{
				path = ln.sourceFile.fileName;
				return true;
			}
			return false;
		}

		private bool TryResolvePdbFilePath(Type type, out string path)
		{
			path = Regex.Replace(new Uri(type.Assembly.CodeBase).AbsolutePath, @"\.dll", ".pdb", RegexOptions.IgnoreCase);
			return File.Exists(path);
		}
	}
}
