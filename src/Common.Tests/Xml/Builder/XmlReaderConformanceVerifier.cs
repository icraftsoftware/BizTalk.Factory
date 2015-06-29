#region Copyright & License

// Copyright © 2012 - 2014 François Chabot, Yves Dierick
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
using NUnit.Framework;

namespace Be.Stateless.Xml.Builder
{
	/// <summary>
	/// Asserts that the actual <see cref="XmlReader"/>'s behavior is conformant to the expected <see cref="XmlReader"/> one.
	/// </summary>
	public class XmlReaderConformanceVerifier : XmlReader
	{
		public XmlReaderConformanceVerifier(XmlReader actual, XmlReader expected)
		{
			if (actual == null) throw new ArgumentNullException("actual");
			if (expected == null) throw new ArgumentNullException("expected");
			_actual = actual;
			_expected = expected;
		}

		#region Base Class Member Overrides

		public override int AttributeCount
		{
			get
			{
				Assert.That(_actual.AttributeCount, Is.EqualTo(_expected.AttributeCount));
				return _actual.AttributeCount;
			}
		}

		public override string BaseURI
		{
			get
			{
				Assert.That(_actual.BaseURI, Is.EqualTo(_expected.BaseURI));
				return _actual.BaseURI;
			}
		}

		public override int Depth
		{
			get
			{
				Assert.That(_actual.Depth, Is.EqualTo(_expected.Depth));
				return _actual.Depth;
			}
		}

		public override bool EOF
		{
			get
			{
				Assert.That(_actual.EOF, Is.EqualTo(_expected.EOF));
				return _actual.EOF;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				Assert.That(_actual.IsEmptyElement, Is.EqualTo(_expected.IsEmptyElement));
				return _actual.IsEmptyElement;
			}
		}

		public override string LocalName
		{
			get
			{
				Assert.That(_actual.LocalName, Is.EqualTo(_expected.LocalName));
				return _actual.LocalName;
			}
		}

		public override XmlNameTable NameTable
		{
			get { return _actual.NameTable; }
		}

		public override string NamespaceURI
		{
			get
			{
				Assert.That(_actual.NamespaceURI, Is.EqualTo(_expected.NamespaceURI));
				return _actual.NamespaceURI;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				Assert.That(_actual.NodeType, Is.EqualTo(_expected.NodeType));
				return _actual.NodeType;
			}
		}

		public override string Prefix
		{
			get
			{
				Assert.That(_actual.Prefix, Is.EqualTo(_expected.Prefix));
				return _actual.Prefix;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				Assert.That(_actual.ReadState, Is.EqualTo(_expected.ReadState));
				return _actual.ReadState;
			}
		}

		public override string Value
		{
			get
			{
				Assert.That(_actual.Value, Is.EqualTo(_expected.Value));
				return _actual.Value;
			}
		}

		public override void Close()
		{
			_actual.Close();
			_expected.Close();
			AssertClosedStateConformance();
		}

		public override string GetAttribute(string name)
		{
			Assert.That(_actual.GetAttribute(name), Is.EqualTo(_expected.GetAttribute(name)));
			return _actual.GetAttribute(name);
		}

		public override string GetAttribute(string name, string namespaceUri)
		{
			Assert.That(_actual.GetAttribute(name, namespaceUri), Is.EqualTo(_expected.GetAttribute(name, namespaceUri)));
			return _actual.GetAttribute(name, namespaceUri);
		}

		public override string GetAttribute(int i)
		{
			Assert.That(_actual.GetAttribute(i), Is.EqualTo(_expected.GetAttribute(i)));
			return _actual.GetAttribute(i);
		}

		public override string LookupNamespace(string prefix)
		{
			Assert.That(_actual.LookupNamespace(prefix), Is.EqualTo(_expected.LookupNamespace(prefix)));
			return _actual.LookupNamespace(prefix);
		}

		public override bool MoveToAttribute(string name)
		{
			var result = _actual.MoveToAttribute(name);
			Assert.That(result, Is.EqualTo(_expected.MoveToAttribute(name)));
			AssertStateConformance();
			return result;
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			var result = _actual.MoveToAttribute(name, ns);
			Assert.That(result, Is.EqualTo(_expected.MoveToAttribute(name, ns)));
			AssertStateConformance();
			return result;
		}

		public override bool MoveToElement()
		{
			var result = _actual.MoveToElement();
			Assert.That(result, Is.EqualTo(_expected.MoveToElement()));
			AssertStateConformance();
			return result;
		}

		public override bool MoveToFirstAttribute()
		{
			var result = _actual.MoveToFirstAttribute();
			Assert.That(result, Is.EqualTo(_expected.MoveToFirstAttribute()));
			AssertStateConformance();
			return result;
		}

		public override bool MoveToNextAttribute()
		{
			var result = _actual.MoveToNextAttribute();
			Assert.That(result, Is.EqualTo(_expected.MoveToNextAttribute()));
			AssertStateConformance();
			return result;
		}

		public override bool Read()
		{
			var result = _actual.Read();
			Assert.That(result, Is.EqualTo(_expected.Read()));
			AssertStateConformance();
			return result;
		}

		public override bool ReadAttributeValue()
		{
			var result = _actual.ReadAttributeValue();
			Assert.That(result, Is.EqualTo(_expected.ReadAttributeValue()));
			AssertStateConformance();
			return result;
		}

		public override void ResolveEntity()
		{
			_actual.ResolveEntity();
			_expected.ResolveEntity();
			AssertStateConformance();
		}

		#endregion

		private void AssertClosedStateConformance()
		{
			Assert.That(_actual.BaseURI, Is.EqualTo(_expected.BaseURI));
			Assert.That(_actual.Depth, Is.EqualTo(_expected.Depth));
			Assert.That(_actual.EOF, Is.EqualTo(_expected.EOF));
			Assert.That(_actual.NodeType, Is.EqualTo(_expected.NodeType));
			Assert.That(_actual.ReadState, Is.EqualTo(_expected.ReadState));
		}

		private void AssertStateConformance()
		{
			Assert.That(_actual.EOF, Is.EqualTo(_expected.EOF));
			Assert.That(_actual.NodeType, Is.EqualTo(_expected.NodeType));
			Assert.That(_actual.ReadState, Is.EqualTo(_expected.ReadState));

			switch (_actual.NodeType)
			{
				case XmlNodeType.Attribute:
					Assert.That(_actual.NamespaceURI, Is.EqualTo(_expected.NamespaceURI));
					Assert.That(_actual.HasValue, Is.EqualTo(_expected.HasValue));
					Assert.That(_actual.Value, Is.EqualTo(_expected.Value));
					Assert.That(_actual.ValueType, Is.EqualTo(_expected.ValueType));
					break;
				case XmlNodeType.Element:
					Assert.That(_actual.AttributeCount, Is.EqualTo(_expected.AttributeCount));
					Assert.That(_actual.BaseURI, Is.EqualTo(_expected.BaseURI));
					Assert.That(_actual.Depth, Is.EqualTo(_expected.Depth));
					Assert.That(_actual.HasAttributes, Is.EqualTo(_expected.HasAttributes));
					Assert.That(_actual.HasValue, Is.EqualTo(_expected.HasValue));
					Assert.That(_actual.IsDefault, Is.EqualTo(_expected.IsDefault));
					Assert.That(_actual.IsEmptyElement, Is.EqualTo(_expected.IsEmptyElement));
					Assert.That(_actual.LocalName, Is.EqualTo(_expected.LocalName));
					Assert.That(_actual.Name, Is.EqualTo(_expected.Name));
					Assert.That(_actual.NamespaceURI, Is.EqualTo(_expected.NamespaceURI));
					Assert.That(_actual.Prefix, Is.EqualTo(_expected.Prefix));
					Assert.That(_actual.Value, Is.EqualTo(_expected.Value));
					Assert.That(_actual.ValueType, Is.EqualTo(_expected.ValueType));
					break;
				case XmlNodeType.EndElement:
					Assert.That(_actual.Depth, Is.EqualTo(_expected.Depth));
					break;
				case XmlNodeType.Text:
					Assert.That(_actual.HasValue, Is.EqualTo(_expected.HasValue));
					Assert.That(_actual.Value, Is.EqualTo(_expected.Value));
					Assert.That(_actual.ValueType, Is.EqualTo(_expected.ValueType));
					break;
				case XmlNodeType.None:
					break;
				default:
					throw new NotImplementedException();
			}

			//TODO Assert.That(_actual.CanReadBinaryContent, Is.EqualTo(_expected.CanReadBinaryContent));
			//TODO Assert.That(_actual.CanReadValueChunk, Is.EqualTo(_expected.CanReadValueChunk));
			//TODO Assert.That(_actual.CanResolveEntity, Is.EqualTo(_expected.CanResolveEntity));
			//TODO Assert.That(_actual.QuoteChar, Is.EqualTo(_expected.QuoteChar));
			//TODO Assert.That(_actual.XmlLang, Is.EqualTo(_expected.XmlLang));
			//TODO Assert.That(_actual.XmlSpace, Is.EqualTo(_expected.XmlSpace));
		}

		private readonly XmlReader _actual;
		private readonly XmlReader _expected;
	}
}
