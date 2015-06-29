#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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

		public XsltArgumentList(System.Xml.Xsl.XsltArgumentList arguments)
		{
			Copy(arguments, this);
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

		public System.Xml.Xsl.XsltArgumentList Union(System.Xml.Xsl.XsltArgumentList arguments)
		{
			var union = Clone();
			if (arguments != null) Copy(arguments, union);
			return union;
		}
	}
}
