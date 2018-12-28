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
	/// In the context of BizTalk Server composite orchestrations, allows a sub-orchestration to notify its caller that
	/// it has failed by throwing a <see cref="FailedSubOrchestrationException"/> that identifies it by its <see
	/// cref="Name"/>.
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

		protected FailedSubOrchestrationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			Name = info.GetString("Name");
		}

		#region Base Class Member Overrides

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null) throw new ArgumentNullException("info");
			info.AddValue("Name", Name);
			base.GetObjectData(info, context);
		}

		public override string Message
		{
			get { return string.Format("Orchestration '{0}' failed. {1}", Name, base.Message); }
		}

		#endregion

		/// <summary>
		/// The name of the sub-orchestration that has failed and thrown this exception.
		/// </summary>
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
		public string Name { get; set; }
	}
}
