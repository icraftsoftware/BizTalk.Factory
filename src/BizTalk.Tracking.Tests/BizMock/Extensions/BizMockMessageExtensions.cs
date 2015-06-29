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

using System.IO;
using Be.Stateless.BizTalk.ContextProperties;
using BizMock;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.BizMock.Extensions
{
	public static class BizMockMessageExtensions
	{
		public static void Promote<T>(this BizMockMessage message, MessageContextProperty<T, string> property, string value)
			where T : MessageContextPropertyBase, new()
		{
			if (value != null) message.Promote(property.Name, property.Namespace, value);
		}

		public static string ReadBodyAsString(this BizMockMessage message)
		{
			using (var reader = File.OpenText(message.Files[0]))
			{
				return reader.ReadToEnd();
			}
		}

		public static void Promote<T, TV>(this BizMockMessage message, MessageContextProperty<T, TV> property, TV value)
			where T : MessageContextPropertyBase, new()
			where TV : struct
		{
			message.Promote(property.Name, property.Namespace, value);
		}
	}
}
