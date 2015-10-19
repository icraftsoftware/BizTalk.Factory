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
using System.ComponentModel;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public interface IOrchestrationBinding
	{
		string Description { get; set; }

		string Host { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		IOrchestrationPortBinding[] PortBindings { get; }

		/// <summary>
		/// The state of the orchestration.
		/// </summary>
		/// <remarks>
		/// An orchestration can be in either of the following state:
		/// <list type="bullet">
		/// <item>
		/// <see cref="ServiceState.Unenlisted"/>;
		/// </item>
		/// <item>
		/// <see cref="ServiceState.Enlisted"/>, or equivalently, <see cref="ServiceState.Stopped"/>;
		/// </item>
		/// <item>
		/// <see cref="ServiceState.Started"/>
		/// </item>
		/// </list>
		/// </remarks>
		ServiceState State { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		Type Type { get; }
	}
}
