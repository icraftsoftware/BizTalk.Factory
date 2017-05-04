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
	/// cref="ErrorReportProperties"/> context properties.
	/// </summary>
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public static class ErrorReportPropertiesExtensions
	{
		public static bool HasFailed(this IBaseMessage message)
		{
			return message.GetProperty(ErrorReportProperties.ErrorType) != null;
		}

		public static bool HasFailed(this IBaseMessageContext messageContext)
		{
			return messageContext.GetProperty(ErrorReportProperties.ErrorType) != null;
		}
	}
}
