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
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Be.Stateless.Extensions;
using Be.Stateless.Xml.Builder;
using Be.Stateless.Xml.Builder.Extensions;
using ConservativeEnumerator = System.Collections.Generic.IEnumerator<Be.Stateless.Xml.Builder.IXmlInformationItemBuilder>;

namespace Be.Stateless.Xml
{
	public class XmlBuilderReader : XmlReader
	{
		public XmlBuilderReader(IXmlElementBuilder node)
		{
			// ReSharper disable once SuspiciousTypeConversion.Global
			_disposable = node as IDisposable;
			var enumerable = node == null
				? Enumerable.Empty<IXmlElementBuilder>()
				: Enumerable.Repeat(node, 1);
			_enumerators = new LinkedList<ConservativeEnumerator>();
			_enumerators.AddLast(enumerable.GetConservativeEnumerator());
			_nodeType = XmlNodeType.None;
			_table = new NameTable();
			_namespaceManager = new XmlNamespaceManager(_table);
		}

		#region Base Class Member Overrides

		/// <summary>
		/// Gets the number of attributes on the current node.
		/// </summary>
		/// <returns>
		/// The number of attributes on the current node.
		/// </returns>
		/// <remarks>
		/// This property is relevant to <see cref="XmlNodeType.DocumentType"/>, <see cref="XmlNodeType.Element"/>, and
		/// <see cref="XmlNodeType.XmlDeclaration"/> nodes only. Other node types do not have attributes.
		/// </remarks>
		public override int AttributeCount
		{
			get { return CurrentNode.GetAttributes().Count(); }
		}

		/// <summary>
		/// Gets the base URI of the current node.
		/// </summary>
		/// <returns>
		/// The base URI of the current node.
		/// </returns>
		/// <remarks>
		/// A networked XML document is comprised of chunks of data aggregated using various W3C standard inclusion
		/// mechanisms and therefore contains nodes that come from different places. DTD entities are an example of this,
		/// but this is not limited to DTDs. The <see cref="BaseURI"/> tells you where these nodes came from. If there is
		/// no base URI for the nodes being returned (for example, they were parsed from an in-memory string),
		/// <c>String.Empty</c> is returned.
		/// </remarks>
		public override string BaseURI
		{
			get { return string.Empty; }
		}

		/// <summary>
		/// Gets a value indicating whether this reader can parse and resolve entities.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the reader can parse and resolve entities; <c>false</c> otherwise.
		/// </returns>
		public override bool CanResolveEntity
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the depth of the current node in the XML document.
		/// </summary>
		/// <returns>
		/// The depth of the current node in the XML document.
		/// </returns>
		public override int Depth
		{
			get { return _enumerators.Count > 0 ? _enumerators.Count - 1 : 0; }
		}

		/// <summary>
		/// Gets a value indicating whether the reader is positioned at the end of the stream.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the reader is positioned at the end of the stream; <c>false</c> otherwise.
		/// </returns>
		public override bool EOF
		{
			get { return _state == ReadState.EndOfFile; }
		}

		/// <summary>
		/// Gets a value indicating whether the current node has any attributes.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the current node has attributes; <c>false</c> otherwise.
		/// </returns>
		public override bool HasAttributes
		{
			get { return CurrentNode.HasAttributes(); }
		}

		/// <summary>
		/// Gets a value indicating whether the current node is an empty element (for example, &lt;MyElement /&gt;).
		/// </summary>
		/// <returns>
		/// <c>true</c> if the current node is an element (<see cref="NodeType"/> equals <see cref="XmlNodeType"/>.<see
		/// cref="XmlNodeType.Element"/>) that ends with /&gt;; <c>false</c> otherwise.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This property enables you to determine the difference between the following:
		/// <list type="bullet">
		/// <item>
		/// <![CDATA[<item num="123"/>]]>, for which <see cref="IsEmptyElement"/> is <c>true</c>;
		/// </item>
		/// <item>
		/// <![CDATA[<item num="123"></item>]]>, for which <see cref="IsEmptyElement"/> is <c>false</c> although element
		/// content is empty.
		/// </item>
		/// </list>
		/// A corresponding <see cref="XmlNodeType.EndElement"/> node is not generated for empty elements.
		/// </para>
		/// <para>
		/// If default content has been added to an element due to schema validation, <see cref="IsEmptyElement"/> still
		/// returns <c>true</c>. It has no bearing on whether or not the element has a default value. In other words, <see
		/// cref="IsEmptyElement"/> simply reports whether or not the element in the source document had an end element
		/// tag.
		/// </para>
		/// </remarks>
		public override bool IsEmptyElement
		{
			get { return !CurrentNode.HasChildNodes(); }
		}

		/// <summary>
		/// Gets the local name of the current node.
		/// </summary>
		/// <returns>
		/// The name of the current node with the prefix removed. For example, <see cref="LocalName"/> is <c>book</c> for
		/// the element &lt;bk:book&gt;. For node types that do not have a name (like <see cref="XmlNodeType.Text"/>, <see
		/// cref="XmlNodeType.Comment"/>, and so on), this property returns <c>String.Empty</c>.
		/// </returns>
		public override string LocalName
		{
			get { return CurrentNode.LocalName; }
		}

		/// <summary>
		/// Gets the <see cref="XmlNameTable"/> associated with this implementation.
		/// </summary>
		/// <returns>
		/// The <see cref="XmlNameTable"/> enabling you to get the atomized version of a string within the node.
		/// </returns>
		/// <remarks>
		/// All node and attribute names returned from XmlReader are atomized using the <see cref="XmlNameTable"/>. When
		/// the same name is returned multiple times (for example, <c>Customer</c>), then the same <see cref="string"/>
		/// object will be returned for that name. This makes it possible for you to write efficient code that does object
		/// comparisons on these strings instead of expensive string comparisons
		/// </remarks>
		public override XmlNameTable NameTable
		{
			get { return _table; }
		}

		/// <summary>
		/// Gets the namespace URI (as defined in the W3C Namespace specification) of the node on which the reader is
		/// positioned.
		/// </summary>
		/// <returns>
		/// The namespace URI of the current node; otherwise an empty string.
		/// </returns>
		/// <remarks>
		/// This property is relevant to <see cref="XmlNodeType.Attribute"/> and <see cref="XmlNodeType.Element"/> nodes
		/// only.
		/// </remarks>
		public override string NamespaceURI
		{
			get
			{
				return CurrentNode.NamespaceUri
					?? LookupNamespace(Prefix)
						?? (CurrentNode is IXmlAttributeBuilder
							// LookupNamespace(LocalName) for an attribute in case it is for instance xmlns
							? LookupNamespace(LocalName) ?? string.Empty
							: string.Empty);
			}
		}

		/// <summary>
		/// Gets the type of the current node.
		/// </summary>
		/// <returns>
		/// One of the <see cref="XmlNodeType"/> values representing the type of the current node.
		/// </returns>
		public override XmlNodeType NodeType
		{
			get { return _nodeType; }
		}

		/// <summary>
		/// Gets the namespace prefix associated with the current node.
		/// </summary>
		/// <returns>
		/// The namespace prefix associated with the current node.
		/// </returns>
		public override string Prefix
		{
			get { return CurrentNode.Prefix ?? string.Empty; }
		}

		/// <summary>
		/// Gets the state of the reader.
		/// </summary>
		/// <returns>
		/// One of the <see cref="ReadState"/> values.
		/// </returns>
		public override ReadState ReadState
		{
			get { return _state; }
		}

		/// <summary>
		/// Gets the text value of the current node.
		/// </summary>
		/// <returns>
		/// The value returned depends on the <see cref="NodeType"/> of the node. The following table lists node types
		/// that have a value to return. All other node types return <c>String.Empty</c>.
		/// <list type="table">
		/// <listheader>
		/// <term><see cref="XmlNodeType"/></term>
		/// <description>Value</description>
		/// </listheader>
		/// <item>
		/// <term><see cref="XmlNodeType.Attribute"/></term>
		/// <description>The value of the attribute.</description>
		/// </item>
		/// <item>
		/// <term><see cref="XmlNodeType.CDATA"/></term>
		/// <description>The content of the CDATA section.</description>
		/// </item>
		/// <item>
		/// <term><see cref="XmlNodeType.Comment"/></term>
		/// <description>The content of the comment.</description>
		/// </item>
		/// <item>
		/// <term><see cref="XmlNodeType.DocumentType"/></term>
		/// <description>The internal subset.</description>
		/// </item>
		/// <item>
		/// <term><see cref="XmlNodeType.ProcessingInstruction"/></term>
		/// <description>The entire content, excluding the target.</description>
		/// </item>
		/// <item>
		/// <term><see cref="XmlNodeType.SignificantWhitespace"/></term>
		/// <description>The white space between markup in a mixed content model.</description>
		/// </item>
		/// <item>
		/// <term><see cref="XmlNodeType.Text"/></term>
		/// <description>The content of the text node.</description>
		/// </item>
		/// <item>
		/// <term><see cref="XmlNodeType.Whitespace"/></term>
		/// <description>The white space between markup.</description>
		/// </item>
		/// <item>
		/// <term><see cref="XmlNodeType.XmlDeclaration"/></term>
		/// <description>The content of the declaration.</description>
		/// </item>
		/// </list>
		/// </returns>
		public override string Value
		{
			get { return CurrentNode.Value ?? string.Empty; }
		}

		/// <summary>
		/// Changes the <see cref="ReadState"/> to <see cref="System.Xml.ReadState.Closed"/>.
		/// </summary>
		public override void Close()
		{
			_state = ReadState.Closed;
			if (_disposable != null) _disposable.Dispose();
		}

		/// <summary>
		/// Gets the value of the attribute with the specified <see cref="XmlReader.Name"/>.
		/// </summary>
		/// <returns>
		/// The value of the specified attribute. If the attribute is not found or the value is <c>String.Empty</c>,
		/// <c>null</c> is returned.
		/// </returns>
		/// <param name="name">
		/// The qualified name of the attribute.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="name"/> is <c>null</c>.
		/// </exception>
		/// <remarks>
		/// This method does not move the reader.
		/// </remarks>
		public override string GetAttribute(string name)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the value of the attribute with the specified <see cref="XmlReader.LocalName"/> and <see
		/// cref="XmlReader.NamespaceURI"/>.
		/// </summary>
		/// <returns>
		/// The value of the specified attribute. If the attribute is not found or the value is <c>String.Empty</c>,
		/// <c>null</c> is returned.
		/// </returns>
		/// <param name="name">
		/// The local name of the attribute.
		/// </param>
		/// <param name="namespaceUri">
		/// The namespace URI of the attribute.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="name"/> is <c>null</c>.
		/// </exception>
		/// <remarks>
		/// This method does not move the reader.
		/// </remarks>
		public override string GetAttribute(string name, string namespaceUri)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the value of the attribute with the specified index.
		/// </summary>
		/// <returns>
		/// The value of the specified attribute.
		/// </returns>
		/// <param name="i">
		/// The index of the attribute. The index is zero-based.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="i"/> is out of range. It must be non-negative and less than the size of the attribute
		/// collection.
		/// </exception>
		/// <remarks>
		/// This method does not move the reader.
		/// </remarks>
		public override string GetAttribute(int i)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Resolves a namespace prefix in the current element's scope.
		/// </summary>
		/// <returns>
		/// The namespace URI to which the prefix maps or <c>null</c> if no matching prefix is found.
		/// </returns>
		/// <param name="prefix">
		/// The prefix whose namespace URI you want to resolve. To match the default namespace, pass an empty string.
		/// </param>
		public override string LookupNamespace(string prefix)
		{
			var ns = _namespaceManager.LookupNamespace(prefix);
			return ns.IsNullOrEmpty() ? null : ns;
		}

		/// <summary>
		/// Moves to the attribute with the specified <see cref="XmlReader.Name"/>.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the attribute is found; <c>false</c> otherwise. If <c>false</c>, the reader's position does not
		/// change.
		/// </returns>
		/// <param name="name">
		/// The qualified name of the attribute.
		/// </param>
		/// <remarks>
		/// After calling <see cref="MoveToAttribute(string)"/>, the <see cref="XmlReader.Name"/>, <see
		/// cref="XmlReader.NamespaceURI"/>, and <see cref="XmlReader.Prefix"/> properties reflect the properties of that
		/// attribute.
		/// </remarks>
		public override bool MoveToAttribute(string name)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Moves to the attribute with the specified <see cref="XmlReader.LocalName"/> and <see
		/// cref="XmlReader.NamespaceURI"/>.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the attribute is found; <c>false</c> otherwise. If <c>false</c>, the reader's position does not
		/// change.
		/// </returns>
		/// <param name="name">
		/// The local name of the attribute.
		/// </param>
		/// <param name="ns">
		/// The namespace URI of the attribute.
		/// </param>
		/// <remarks>
		/// After calling <see cref="MoveToAttribute(string)"/>, the <see cref="XmlReader.Name"/>, <see
		/// cref="XmlReader.NamespaceURI"/>, and <see cref="XmlReader.Prefix"/> properties reflect the properties of that
		/// attribute.
		/// </remarks>
		public override bool MoveToAttribute(string name, string ns)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// When overridden in a derived class, moves to the element that contains the current attribute node.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the reader is positioned on an attribute (the reader moves to the element that owns the
		/// attribute); <c>false</c> if the reader is not positioned on an attribute (the position of the reader does not
		/// change).
		/// </returns>
		/// <remarks>
		/// Use this method to return to an element after navigating through its attributes. This method moves the reader
		/// to one of the following node types: <see cref="XmlNodeType.DocumentType"/>, <see cref="XmlNodeType.Element"/>,
		/// or <see cref="XmlNodeType.XmlDeclaration"/>.
		/// </remarks>
		public override bool MoveToElement()
		{
			if (CurrentEnumerator is IEnumerator<IXmlAttributeBuilder>)
			{
				_enumerators.RemoveLast();
				_nodeType = XmlNodeType.Element;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Moves to the first attribute.
		/// </summary>
		/// <returns>
		/// <c>true</c> if an attribute exists; <c>false</c> otherwise.
		/// </returns>
		/// <remarks>
		/// If <see cref="MoveToFirstAttribute"/> returns <c>true</c>, the reader moves to the first attribute; otherwise,
		/// the position of the reader does not change.
		/// </remarks>
		public override bool MoveToFirstAttribute()
		{
			return MoveToNextAttribute();
		}

		/// <summary>
		/// Moves to the next attribute.
		/// </summary>
		/// <returns>
		/// <c>true</c> if there is a next attribute; <c>false</c> if there are no more attributes.
		/// </returns>
		/// <remarks>
		/// If the current node is an element node, this method is equivalent to <see cref="MoveToFirstAttribute"/>. If
		/// <see cref="MoveToNextAttribute"/> returns <c>true</c>, the reader moves to the next attribute; otherwise, the
		/// position of the reader does not change.
		/// </remarks>
		public override bool MoveToNextAttribute()
		{
			if (CurrentNode is IXmlTextBuilder && ParentNode is IXmlAttributeBuilder)
			{
				_enumerators.RemoveLast();
			}
			if (CurrentNode is IXmlElementBuilder && CurrentNode.HasAttributes())
			{
				_enumerators.AddLast(CurrentNode.GetAttributes().GetConservativeEnumerator());
			}
			if (CurrentEnumerator is IEnumerator<IXmlAttributeBuilder> && CurrentEnumerator.MoveNext())
			{
				_nodeType = XmlNodeType.Attribute;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Reads the next node from the stream.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the next node was read successfully; <c>false</c> if there are no more nodes to read.
		/// </returns>
		/// <exception cref="XmlException">
		/// An error occurred while parsing the XML.
		/// </exception>
		/// <remarks>
		/// When an <see cref="XmlReader"/> is first created and initialized, there is no information available. You must
		/// call <see cref="Read"/> to read the first node.
		/// </remarks>
		public override bool Read()
		{
			switch (_state)
			{
				case ReadState.Initial:
					_state = ReadState.Interactive;
					goto case ReadState.Interactive;

				case ReadState.Interactive:
					if (ReadNextNode()) return true;
					_state = ReadState.EndOfFile;
					return false;

				default:
					return false;
			}
		}

		/// <summary>
		/// Parses the attribute value into one or more <see cref="XmlNodeType.Text"/>, <see
		/// cref="XmlNodeType.EntityReference"/>, or <see cref="XmlNodeType.EndEntity"/> nodes.
		/// </summary>
		/// <returns>
		/// <c>true</c> if there are nodes to return. <c>false</c> if the reader is not positioned on an attribute node
		/// when the initial call is made or if all the attribute values have been read. An empty attribute, such as,
		/// <c>misc=""</c>, returns <c>true</c>
		/// with a single node with a value of <c>String.Empty</c>.
		/// </returns>
		/// <remarks>
		/// Use this method after calling <see cref="MoveToAttribute(string)"/> to read through the text or entity
		/// reference nodes that make up the attribute value. The <see cref="XmlReader.Depth"/> of the attribute value
		/// nodes is one plus the depth of the attribute node; it increments and decrements by one when you step into and
		/// out of general entity references.
		/// </remarks>
		public override bool ReadAttributeValue()
		{
			if (CurrentNode is IXmlAttributeBuilder)
			{
				var enumerable = Enumerable.Repeat(new XmlTextBuilder { Value = CurrentNode.Value }, 1);
				_enumerators.AddLast(enumerable.GetConservativeEnumerator());
				CurrentEnumerator.MoveNext();
				_nodeType = XmlNodeType.Text;
				return true;
			}
			_enumerators.RemoveLast();
			return false;
		}

		/// <summary>
		/// Resolves the entity reference for <see cref="XmlNodeType.EntityReference"/> nodes.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// The reader is not positioned on an <see cref="XmlNodeType.EntityReference"/> node or this implementation of
		/// the reader cannot resolve entities (<see cref="XmlReader.CanResolveEntity"/> returns <c>false</c>).
		/// </exception>
		/// <remarks>
		/// If the reader is positioned on an <see cref="XmlNodeType.EntityReference"/> node, if <see cref="Read"/> is
		/// called after calling this method, the entity replacement text is parsed. When the entity replacement text is
		/// finished, an <see cref="XmlNodeType.EndEntity"/> node is returned to close the entity reference scope.
		/// </remarks>
		public override void ResolveEntity()
		{
			throw new InvalidOperationException();
		}

		#endregion

		private ConservativeEnumerator CurrentEnumerator
		{
			get { return _enumerators.Last.IfNotNull(n => n.Value); }
		}

		private IXmlInformationItemBuilder CurrentNode
		{
			get { return CurrentEnumerator.IfNotNull(e => e.Current); }
		}

		private ConservativeEnumerator ParentEnumerator
		{
			get { return _enumerators.Last.Previous.IfNotNull(n => n.Value); }
		}

		private IXmlInformationItemBuilder ParentNode
		{
			get { return ParentEnumerator.IfNotNull(e => e.Current); }
		}

		private bool ReadNextNode()
		{
			// attributes must not be taken into account while enumerating nodes
			if (CurrentEnumerator is IEnumerator<IXmlAttributeBuilder>)
			{
				_nodeType = XmlNodeType.Element;
				_enumerators.RemoveLast();
			}

			// ensure child nodes are enumerated
			if (_nodeType == XmlNodeType.Element && CurrentNode.HasChildNodes())
			{
				_enumerators.AddLast(CurrentNode.GetChildNodes().GetConservativeEnumerator());
			}

			// loop so as to skip empty Text nodes
			while (CurrentEnumerator.MoveNext())
			{
				if (CurrentNode is IXmlElementBuilder)
				{
					_nodeType = XmlNodeType.Element;
					return true;
				}
				if (CurrentNode is IXmlTextBuilder)
				{
					if (!CurrentNode.Value.IsNullOrEmpty())
					{
						_nodeType = XmlNodeType.Text;
						return true;
					}
				}
				else
				{
					throw new NotImplementedException();
				}
			}
			_enumerators.RemoveLast();

			// a non-empty element ---its children have just been enumerated--- requires an extra Read() for the closing tag
			if (CurrentNode is IXmlElementBuilder)
			{
				_nodeType = XmlNodeType.EndElement;
				return true;
			}

			_nodeType = XmlNodeType.None;
			return false;
		}

		private readonly IDisposable _disposable;
		private readonly LinkedList<ConservativeEnumerator> _enumerators;
		private readonly XmlNamespaceManager _namespaceManager;
		private readonly XmlNameTable _table;
		private XmlNodeType _nodeType;
		private ReadState _state;
	}
}
