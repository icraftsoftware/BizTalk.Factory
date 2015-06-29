#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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
using System.Runtime.Serialization;
using Be.Stateless.BizTalk.Dsl.Binding.Diagnostics;
using Be.Stateless.BizTalk.Dsl.Binding.Extensions;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	[Serializable]
	public class BindingException : Exception
	{
		private static string FormatMessage(string message, IProvideSourceFileInformation sourceFileInformation)
		{
			var bindingArtifactDisplayName = sourceFileInformation.GetBindingArtifactDisplayName();
			return string.Format(
				"{0}{1}\r\n{2}, line {3}, column {4}.",
				bindingArtifactDisplayName.IsNullOrEmpty() ? string.Empty : "'" + bindingArtifactDisplayName + "' ",
				message,
				sourceFileInformation.Name,
				sourceFileInformation.Line,
				sourceFileInformation.Column);
		}

		public BindingException(string message) : base(message) { }

		public BindingException(string message, IProvideSourceFileInformation sourceFileInformation)
			: base(FormatMessage(message, sourceFileInformation)) { }

		public BindingException(string message, IProvideSourceFileInformation sourceFileInformation, Exception innerException)
			: base(FormatMessage(message, sourceFileInformation), innerException) { }

		public BindingException(string message, Exception innerException) : base(message, innerException) { }

		protected BindingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
