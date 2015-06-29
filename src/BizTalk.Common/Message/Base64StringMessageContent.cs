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
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Message
{
	/// <summary>
	/// Message type to use when one needs to send a base 64 encoded text message from an orchestration.
	/// </summary>
	/// <see href = "http://msdn.microsoft.com/en-us/library/ee253435" />
	[CustomFormatter(typeof(StringContentFormatter))]
	[Serializable]
	public class Base64StringMessageContent : StringMessageContent
	{
		public Base64StringMessageContent(string content) : base(content)
		{
		}

		public static implicit operator Base64StringMessageContent(string content)
		{
			return new Base64StringMessageContent(content);
		}

		protected override byte[] GetBytes()
		{
			return Convert.FromBase64String(_content);
		}
	}
}