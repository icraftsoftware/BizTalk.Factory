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
using System.Xml;
using Microsoft.BizTalk.XLANGs.BTXEngine;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.XLANGs.Core;

namespace Be.Stateless.BizTalk.Transform
{
	[Serializable]
	public sealed class XlangMessage : BTXMessage
	{
		public static XLANGMessage Create(Context context, Stream content)
		{
			var message = new XlangMessage(context, content);
			return message.GetMessageWrapperForUserCode();
		}

		public static XLANGMessage Create(Context context, XmlDocument content)
		{
			var message = new XlangMessage(context, content);
			return message.GetMessageWrapperForUserCode();
		}

		private XlangMessage(Context context, object content) : base("CustomXlangMessage", context)
		{
			context.RefMessage(this);
			AddPart(string.Empty, "Main");
			this[0].LoadFrom(content);
		}
	}

	[Serializable]
	// TODO to be deprecated
	internal sealed class CustomBtxMessage : BTXMessage
	{
		public CustomBtxMessage(string messageName, Context context) : base(messageName, context)
		{
			context.RefMessage(this);
		}
	}
}
