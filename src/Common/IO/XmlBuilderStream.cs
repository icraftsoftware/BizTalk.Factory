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
using System.IO;
using System.Linq;
using System.Text;
using Be.Stateless.Extensions;
using Be.Stateless.Xml.Builder;
using Be.Stateless.Xml.Builder.Extensions;
using ConservativeEnumerator = System.Collections.Generic.IEnumerator<Be.Stateless.Xml.Builder.IXmlInformationItemBuilder>;

namespace Be.Stateless.IO
{
	public class XmlBuilderStream : Stream
	{
		public XmlBuilderStream(IXmlElementBuilder node) : this(node, Encoding.UTF8) { }

		public XmlBuilderStream(IXmlElementBuilder node, Encoding encoding)
		{
			if (encoding == null) throw new ArgumentNullException("encoding");
			// ReSharper disable once SuspiciousTypeConversion.Global
			_disposable = node as IDisposable;
			_encoding = encoding;
			var enumerable = node == null
				? Enumerable.Empty<IXmlElementBuilder>()
				: Enumerable.Repeat(node, 1);
			_enumerators = new LinkedList<ConservativeEnumerator>();
			_enumerators.AddLast(enumerable.GetConservativeEnumerator());
		}

		#region Base Class Member Overrides

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override long Length
		{
			get { throw new NotImplementedException(); }
		}

		public override long Position
		{
			get { return _position; }
			set { throw new NotSupportedException(); }
		}

		public override void Close()
		{
			if (_disposable != null) _disposable.Dispose();
			base.Close();
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			var bufferController = new BufferController(buffer, offset, count);
			// try to exhaust backlog if any while keeping any overflowing content
			_backlog = bufferController.Append(_backlog);
			while (bufferController.Availability > 0 && !EOF)
			{
				// append to buffer and keep any overflowing content
				_backlog = bufferController.Append(ReadNextNode(), Encoding);
			}
			_position += bufferController.Count;
			return bufferController.Count;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		#endregion

		public Encoding Encoding
		{
			get { return _encoding; }
		}

		public bool EOF
		{
			get { return _enumerators.Count == 0; }
		}

		private ConservativeEnumerator CurrentEnumerator
		{
			get { return _enumerators.Last.IfNotNull(n => n.Value); }
		}

		private IXmlInformationItemBuilder CurrentNode
		{
			get { return CurrentEnumerator.IfNotNull(e => e.Current); }
		}

		private string ReadNextNode()
		{
			if (CurrentEnumerator.MoveNext())
			{
				if (CurrentNode is IXmlAttributeBuilder)
				{
					return CurrentNode.Prefix.IsNullOrEmpty()
						? string.Format(" {0}=\"{1}\"", CurrentNode.LocalName, CurrentNode.Value)
						: string.Format(" {0}:{1}=\"{2}\"", CurrentNode.Prefix, CurrentNode.LocalName, CurrentNode.Value);
				}
				if (CurrentNode is IXmlElementBuilder)
				{
					if (CurrentNode.HasAttributes())
					{
						var result = CurrentNode.Prefix.IsNullOrEmpty()
							? string.Format("<{0}", CurrentNode.LocalName)
							: string.Format("<{0}:{1}", CurrentNode.Prefix, CurrentNode.LocalName);
						_enumerators.AddLast(CurrentNode.GetAttributes().GetConservativeEnumerator());
						return result;
					}

					if (CurrentNode.HasChildNodes())
					{
						var result = CurrentNode.Prefix.IsNullOrEmpty()
							? string.Format("<{0}>", CurrentNode.LocalName)
							: string.Format("<{0}:{1}>", CurrentNode.Prefix, CurrentNode.LocalName);
						_enumerators.AddLast(CurrentNode.GetChildNodes().GetConservativeEnumerator());
						return result;
					}
					return CurrentNode.Prefix.IsNullOrEmpty()
						? string.Format("<{0} />", CurrentNode.LocalName)
						: string.Format("<{0}:{1} />", CurrentNode.Prefix, CurrentNode.LocalName);
				}
				if (CurrentNode is IXmlTextBuilder)
				{
					return CurrentNode.Value;
				}
				throw new NotImplementedException();
			}

			if (CurrentNode is IXmlAttributeBuilder)
			{
				_enumerators.RemoveLast();
				if (CurrentNode.HasChildNodes())
				{
					_enumerators.AddLast(CurrentNode.GetChildNodes().GetConservativeEnumerator());
					return ">";
				}
				return " />";
			}

			_enumerators.RemoveLast();

			if (CurrentNode is IXmlElementBuilder)
			{
				return CurrentNode.Prefix.IsNullOrEmpty()
					? string.Format("</{0}>", CurrentNode.LocalName)
					: string.Format("</{0}:{1}>", CurrentNode.Prefix, CurrentNode.LocalName);
			}

			return null;
		}

		private readonly IDisposable _disposable;
		private readonly Encoding _encoding;
		private readonly LinkedList<ConservativeEnumerator> _enumerators;
		private byte[] _backlog;
		private int _position;
	}
}
