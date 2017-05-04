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

using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Message.Extensions;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.ContextProperties.Extensions
{
	/// <summary>
	/// Fluent-syntax <see cref="IBaseMessage"/> and <see cref="IBaseMessageContext"/> helpers for <see
	/// cref="TrackingProperties"/> context properties.
	/// </summary>
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public static class TrackingPropertiesExtensions
	{
		public static IBaseMessage SetProcessName(this IBaseMessage message, string processName)
		{
			message.SetProperty(TrackingProperties.ProcessName, processName);
			return message;
		}

		public static IBaseMessageContext SetProcessName(this IBaseMessageContext context, string processName)
		{
			context.SetProperty(TrackingProperties.ProcessName, processName);
			return context;
		}

		public static IBaseMessage SetValue1(this IBaseMessage message, string value)
		{
			message.SetProperty(TrackingProperties.Value1, value);
			return message;
		}

		public static IBaseMessageContext SetValue1(this IBaseMessageContext context, string value)
		{
			context.SetProperty(TrackingProperties.Value1, value);
			return context;
		}

		public static IBaseMessage SetValue2(this IBaseMessage message, string value)
		{
			message.SetProperty(TrackingProperties.Value2, value);
			return message;
		}

		public static IBaseMessageContext SetValue2(this IBaseMessageContext context, string value)
		{
			context.SetProperty(TrackingProperties.Value2, value);
			return context;
		}

		public static IBaseMessage SetValue3(this IBaseMessage message, string value)
		{
			message.SetProperty(TrackingProperties.Value3, value);
			return message;
		}

		public static IBaseMessageContext SetValue3(this IBaseMessageContext context, string value)
		{
			context.SetProperty(TrackingProperties.Value3, value);
			return context;
		}
	}
}
