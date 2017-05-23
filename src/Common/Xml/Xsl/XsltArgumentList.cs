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
using System.Collections;
using System.Xml;
using Be.Stateless.Linq.Extensions;
using Be.Stateless.Reflection;

namespace Be.Stateless.Xml.Xsl
{
	/// <summary>
	/// Cloneable <see cref="T:System.Xml.Xsl.XsltArgumentList"/>.
	/// </summary>
	public class XsltArgumentList : System.Xml.Xsl.XsltArgumentList, ICloneable
	{
		private static void Copy(System.Xml.Xsl.XsltArgumentList source, System.Xml.Xsl.XsltArgumentList target)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (target == null) throw new ArgumentNullException("target");

			var parameters = (Hashtable) Reflector.GetField(source, "parameters");
			foreach (DictionaryEntry entry in parameters)
			{
				var qn = (XmlQualifiedName) entry.Key;
				target.AddParam(qn.Name, qn.Namespace, entry.Value);
			}
			var extensions = (Hashtable) Reflector.GetField(source, "extensions");
			foreach (DictionaryEntry entry in extensions)
			{
				target.AddExtensionObject((string) entry.Key, entry.Value);
			}
		}

		public XsltArgumentList() { }

		/// <summary>
		/// Copy constructor that instantiates a new <see cref="XsltArgumentList"/>.
		/// </summary>
		/// <param name="arguments">
		/// The <see cref="System.Xml.Xsl.XsltArgumentList"/> to copy.
		/// </param>
		public XsltArgumentList(System.Xml.Xsl.XsltArgumentList arguments)
		{
			Copy(arguments, this);
		}

		/// <summary>
		/// Instantiate a new <see cref="XsltArgumentList"/>.
		/// </summary>
		/// <param name="splatteredArguments">
		/// The arguments to store in the list. The array must contain a number of items that is a multiple of 3. Each
		/// series of 3 items contains, in that order, an XSL parameter name, the namespace URI of the parameter, and its
		/// value.
		/// </param>
		public XsltArgumentList(params object[] splatteredArguments)
		{
			if (splatteredArguments == null) throw new ArgumentNullException("splatteredArguments");
			if (splatteredArguments.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", "splatteredArguments");
			if (splatteredArguments.Length % 3 != 0) throw new ArgumentException("Value must be a collection containing a number of items that is multiple of 3.", "splatteredArguments");
			for (var i = 0; i < splatteredArguments.Length; i += 3)
			{
				AddParam(
					name: (string) splatteredArguments[i],
					namespaceUri: (string) splatteredArguments[i + 1],
					parameter: splatteredArguments[i + 2]);
			}
		}

		/// <summary>
		/// Instantiate a new <see cref="XsltArgumentList"/>.
		/// </summary>
		/// <param name="arguments">
		/// The <see cref="XsltArgument"/> arguments to store in the list.
		/// </param>
		public XsltArgumentList(params XsltArgument[] arguments)
		{
			if (arguments == null) throw new ArgumentNullException("arguments");
			if (arguments.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", "arguments");
			arguments.Each(a => AddParam(a.Name, a.NamespaceUri, a.Value));
		}

		#region ICloneable Members

		object ICloneable.Clone()
		{
			return Clone();
		}

		#endregion

		public XsltArgumentList Clone()
		{
			var target = new XsltArgumentList();
			Copy(this, target);
			return target;
		}

		public System.Xml.Xsl.XsltArgumentList Add(XsltArgument argument)
		{
			AddParam(argument.Name, argument.NamespaceUri, argument.Value);
			return this;
		}

		/// <summary>
		/// Adds custom transform arguments to the transform argument list.
		/// </summary>
		/// <param name="splatteredArguments">
		/// The arguments to add to the list. The array must contain a number of items that is a multiple of 3. Each
		/// series of 3 items contains, in that order, an XSL parameter name, the namespace URI of the parameter, and its
		/// value.
		/// </param>
		/// <returns>
		/// A new list being the result of the union.
		/// </returns>
		public System.Xml.Xsl.XsltArgumentList Union(object[] splatteredArguments)
		{
			if (splatteredArguments == null) throw new ArgumentNullException("splatteredArguments");
			if (splatteredArguments.Length % 3 != 0) throw new ArgumentException("Value must be a collection containing a number of items that is multiple of 3.", "splatteredArguments");

			var union = Clone();
			for (var i = 0; i < splatteredArguments.Length; i += 3)
			{
				union.AddParam(
					name: (string) splatteredArguments[i],
					namespaceUri: (string) splatteredArguments[i + 1],
					parameter: splatteredArguments[i + 2]);
			}
			return union;
		}

		/// <summary>
		/// Adds custom transform arguments to the transform argument list.
		/// </summary>
		/// <param name="arguments">
		/// The arguments to add to the list.
		/// </param>
		/// <returns>
		/// A new list being the result of the union.
		/// </returns>
		public System.Xml.Xsl.XsltArgumentList Union(params XsltArgument[] arguments)
		{
			if (arguments == null) throw new ArgumentNullException("arguments");

			var union = Clone();
			arguments.Each(a => union.AddParam(a.Name, a.NamespaceUri, a.Value));
			return union;
		}

		/// <summary>
		/// Adds custom transform arguments to the transform argument list.
		/// </summary>
		/// <param name="arguments"></param>
		/// <returns>
		/// A new list being the result of the union.
		/// </returns>
		public System.Xml.Xsl.XsltArgumentList Union(System.Xml.Xsl.XsltArgumentList arguments)
		{
			var union = Clone();
			if (arguments != null) Copy(arguments, union);
			return union;
		}
	}
}
