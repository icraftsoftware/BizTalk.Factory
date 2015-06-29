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

using System.IO;
using Be.Stateless.BizTalk.Dsl;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Stub.Language
{
	// ReSharper disable UnusedTypeParameter

	/// <summary>
	/// Allows to setup the response that has to be returned by the <see cref="IStubService"/> stub service upon either
	/// the reception of some request message or the invocation of some SOAP action.
	/// </summary>
	/// <typeparam name="TContract">
	/// The the service contract to which belong operation that is being setup.
	/// </typeparam>
	/// <typeparam name="TResult">
	/// The type of the result the service contract's operation will produce.
	/// </typeparam>
	public interface IOperationResponseSetup<TContract, TResult> : IHideObjectMembers
		where TContract : class
	{
		/// <summary>
		/// Will setup the content of the <paramref name="file"/> as the response's body stream.
		/// </summary>
		/// <param name="file">
		/// Path of the file to be used as the response's body stream.
		/// </param>
		void Returns(string file);

		/// <summary>
		/// Will setup the <paramref name="stream"/> as the response's body stream.
		/// </summary>
		/// <param name="stream">
		/// The stream to be used as the response's body stream.
		/// </param>
		void Returns(Stream stream);
	}
}
