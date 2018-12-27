#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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
using System.Runtime.Serialization;

namespace Be.Stateless.BizTalk
{
	/// <summary>
	/// Allows a BizTalk Server sub-orchestration, by throwing a <see cref="FailedSubOrchestrationException"/> that
	/// identifies it by its <see cref="Name"/>, to notify its calling composite orchestration that precisely it has
	/// failed.
	/// </summary>
	[Serializable]
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class FailedSubOrchestrationException : Exception
	{
		public FailedSubOrchestrationException(string name, string message) : base(message)
		{
			Name = name;
		}

		public FailedSubOrchestrationException(string name, string message, Exception inner) : base(message, inner)
		{
			Name = name;
		}

		protected FailedSubOrchestrationException(SerializationInfo info, StreamingContext context) : base(info, context) { }

		/// <summary>
		/// The name of the sub orchestration that has failed and thrown this exception.
		/// </summary>
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
		public string Name { get; private set; }
	}
}
