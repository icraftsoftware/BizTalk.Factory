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
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Adapter.Wcf.Metadata;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter.Metadata
{
	/// <summary>
	/// Describes one BizTalk Server HTTP Method and URL mapping operation.
	/// </summary>
	[SuppressMessage("ReSharper", "LocalizableElement")]
	public class HttpUrlMappingOperation : BtsHttpUrlMappingOperation
	{
		public HttpUrlMappingOperation(string name, string method, string url)
		{
			if (name.IsNullOrEmpty()) throw new ArgumentException("Argument is null or empty", "name");
			if (method.IsNullOrEmpty()) throw new ArgumentException("Argument is null or empty", "method");
			if (url.IsNullOrEmpty()) throw new ArgumentException("Argument is null or empty", "url");
			Name = name;
			Method = method;
			Url = url;
		}
	}
}
