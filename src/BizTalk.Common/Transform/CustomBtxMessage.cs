﻿#region Copyright & License

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
using Microsoft.BizTalk.XLANGs.BTXEngine;
using Microsoft.XLANGs.Core;

namespace Be.Stateless.BizTalk.Transform
{
	[Serializable]
	internal sealed class CustomBtxMessage : BTXMessage
	{
		public CustomBtxMessage(Context context, Stream stream) : base(DEFAULT_MESSAGE_NAME, context)
		{
			context.RefMessage(this);
			AddPart(string.Empty, DEFAULT_PART_NAME);
			this[0].LoadFrom(stream);
		}

		// TODO to be deprecated
		public CustomBtxMessage(string messageName, Context context) : base(messageName, context)
		{
			context.RefMessage(this);
		}

		private const string DEFAULT_MESSAGE_NAME = "transformedMessage";
		private const string DEFAULT_PART_NAME = "Main";
	}
}
