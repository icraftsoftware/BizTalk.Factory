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

using System.IO;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	/// <summary>
	/// Various <see cref="IBaseMessage"/> extension methods that allow for shorter and <b>type-safe</b> statements.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Noticeably, offers <b>type-safe</b> and easier to use <see cref="IBaseMessageContext"/> property getters,
	/// setters, and promoters.
	/// </para>
	/// </remarks>
	public static class BaseMessageExtensions
	{
		public static Stream AsStream(this XLANGMessage message)
		{
			return message[0].AsStream();
		}
	}
}
