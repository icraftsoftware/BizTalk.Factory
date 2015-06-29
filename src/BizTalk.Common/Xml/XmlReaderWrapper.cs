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

namespace Be.Stateless.BizTalk.Xml
{
	/// <summary>
	/// <see cref="XmlReader"/> wrapper that delegates all operations to the wrapped <see cref="XmlReader"/>-derived
	/// instance.
	/// </summary>
	public abstract class XmlReaderWrapper : XmlReader
	{
		protected XmlReaderWrapper(XmlReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");
			_reader = reader;
		}

		#region Base Class Member Overrides

		public override int AttributeCount
		{
			get { return _reader.AttributeCount; }
		}

		public override string BaseURI
		{
			get { return _reader.BaseURI; }
		}

		public override int Depth
		{
			get { return _reader.Depth; }
		}

		public override bool EOF
		{
			get { return _reader.EOF; }
		}

		public override bool HasValue
		{
			get { return _reader.HasValue; }
		}

		public override bool IsEmptyElement
		{
			get { return _reader.IsEmptyElement; }
		}

		public override string LocalName
		{
			get { return _reader.LocalName; }
		}

		public override XmlNameTable NameTable
		{
			get { return _reader.NameTable; }
		}

		public override string NamespaceURI
		{
			get { return _reader.NamespaceURI; }
		}

		public override XmlNodeType NodeType
		{
			get { return _reader.NodeType; }
		}

		public override string Prefix
		{
			get { return _reader.Prefix; }
		}

		public override ReadState ReadState
		{
			get { return _reader.ReadState; }
		}

		public override string Value
		{
			get { return _reader.Value; }
		}

		public override void Close()
		{
			_reader.Close();
		}

		public override string GetAttribute(string name)
		{
			return _reader.GetAttribute(name);
		}

		public override string GetAttribute(string name, string ns)
		{
			return _reader.GetAttribute(name, ns);
		}

		public override string GetAttribute(int i)
		{
			return _reader.GetAttribute(i);
		}

		public override string LookupNamespace(string prefix)
		{
			return _reader.LookupNamespace(prefix);
		}

		public override bool MoveToAttribute(string name)
		{
			return _reader.MoveToAttribute(name);
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			return _reader.MoveToAttribute(name, ns);
		}

		public override bool MoveToElement()
		{
			return _reader.MoveToElement();
		}

		public override bool MoveToFirstAttribute()
		{
			return _reader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			return _reader.MoveToNextAttribute();
		}

		public override bool Read()
		{
			return _reader.Read();
		}

		public override bool ReadAttributeValue()
		{
			return _reader.ReadAttributeValue();
		}

		public override void ResolveEntity()
		{
			_reader.ResolveEntity();
		}

		#endregion

		protected XmlReader InnerReader
		{
			get { return _reader; }
			set { _reader = value; }
		}

		private XmlReader _reader;
	}
}
