#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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

using System.ComponentModel;
using System.IO;
using Be.Stateless.BizTalk.Dsl;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Stub
{
	/// <summary>
	/// A scaffolding interface to support setting up response against <see cref="DocumentSpec"/> message type
	/// expectations. It is not meant to be used in user code.
	/// </summary>
	/// <seealso cref="StubService"/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ISolicitResponse : IHideObjectMembers
	{
		/// <summary>
		/// A generic <see cref="Request"/> operation used to support the setting up of the response <see cref="Stream"/>
		/// to be returned upon the invocation of the <see cref="ISolicitResponse.Request"/> operation with a message of
		/// some <see cref="DocumentSpec"/> type.
		/// </summary>
		/// <param name="documentSpec">
		/// The expected <see cref="DocumentSpec"/> of some incoming message for which a response needs to be setup.
		/// </param>
		/// <returns>
		/// The response message <see cref="Stream"/>.
		/// </returns>
		Stream Request(DocumentSpec documentSpec);
	}
}
