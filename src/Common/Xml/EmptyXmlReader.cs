#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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

namespace Be.Stateless.Xml
{
	/// <summary>
	/// Special case of an <see cref="XmlReader"/> with no XML content.
	/// </summary>
	public class EmptyXmlReader : XmlReader
	{
		/// <summary>
		/// Creates a new <see cref="EmptyXmlReader"/> instance.
		/// </summary>
		public static XmlReader Create()
		{
			return new EmptyXmlReader();
		}

		private EmptyXmlReader()
		{
			_state = ReadState.Initial;
		}

		#region Base Class Member Overrides

		public override int AttributeCount
		{
			get { return 0; }
		}

		public override string BaseURI
		{
			get { return string.Empty; }
		}

		public override void Close()
		{
			_state = ReadState.Closed;
		}

		public override int Depth
		{
			get { return 0; }
		}

		public override bool EOF
		{
			get { return _state == ReadState.EndOfFile; }
		}

		public override string GetAttribute(string name)
		{
			return null;
		}

		public override string GetAttribute(string name, string namespaceUri)
		{
			return null;
		}

		public override string GetAttribute(int index)
		{
			throw new ArgumentOutOfRangeException("index");
		}

		public override bool HasValue
		{
			get { return false; }
		}

		public override bool IsEmptyElement
		{
			get { return false; }
		}

		public override string LocalName
		{
			get { return string.Empty; }
		}

		public override string LookupNamespace(string prefix)
		{
			return null;
		}

		public override bool MoveToAttribute(string name)
		{
			return false;
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			return false;
		}

		public override bool MoveToElement()
		{
			return false;
		}

		public override bool MoveToFirstAttribute()
		{
			return false;
		}

		public override bool MoveToNextAttribute()
		{
			return false;
		}

		public override string NamespaceURI
		{
			get { return string.Empty; }
		}

		public override XmlNameTable NameTable
		{
			get { return _table; }
		}

		public override XmlNodeType NodeType
		{
			get { return XmlNodeType.None; }
		}

		public override string Prefix
		{
			get { return string.Empty; }
		}

		public override bool Read()
		{
			_state = ReadState.EndOfFile;
			return false;
		}

		public override bool ReadAttributeValue()
		{
			return false;
		}

		public override ReadState ReadState
		{
			get { return _state; }
		}

		public override void ResolveEntity()
		{
			throw new InvalidOperationException();
		}

		public override string Value
		{
			get { return string.Empty; }
		}

		#endregion

		private readonly XmlNameTable _table = new NameTable();
		private ReadState _state;
	}
}
