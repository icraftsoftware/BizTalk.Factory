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

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public interface IAdapterConfigClientCertificate
	{
		#region Security Tab - Client Certificate Settings

		/// <summary>
		/// Specify the thumbprint of the X.509 certificate for authenticating this send port to services.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The certificate to be used for this property must be installed into the My store in the Current User
		/// location of the user account for the send handler hosting this send port.
		/// </para>
		/// <para>
		/// It defaults to an <see cref="string.Empty"/> string.
		/// </para>
		/// </remarks>
		string ClientCertificate { get; set; }

		#endregion
	}
}
