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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.Linq.Extensions;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Transform
{
	/// <summary>
	/// A collection of <see cref="XLANGMessage"/> messages.
	/// </summary>
	/// <seealso cref="XlangTransformHelper.Transform(Microsoft.XLANGs.BaseTypes.XLANGMessage,System.Type,Be.Stateless.BizTalk.Tracking.TrackingContext)"/>
	[Serializable]
	public sealed class XlangMessageCollection : LinkedList<XLANGMessage>, IDisposable
	{
		#region Operators

		public static implicit operator XmlReader(XlangMessageCollection collection)
		{
			var xmlReaderSettings = new XmlReaderSettings { CloseInput = true };
			return collection.Count == 1
				? XmlReader.Create(collection.First.Value.AsStream(), xmlReaderSettings)
				: CompositeXmlReader.Create(collection.Select(m => m.AsStream()).ToArray(), xmlReaderSettings);
		}

		#endregion

		public XlangMessageCollection() { }

		public XlangMessageCollection(IEnumerable<XLANGMessage> collection) : base(collection) { }

		private XlangMessageCollection(SerializationInfo info, StreamingContext context) : base(info, context) { }

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			this.Where(m => m != null).Each(m => m.Dispose());
		}

		#endregion

		public void Add(XLANGMessage message)
		{
			AddLast(message);
		}
	}
}
