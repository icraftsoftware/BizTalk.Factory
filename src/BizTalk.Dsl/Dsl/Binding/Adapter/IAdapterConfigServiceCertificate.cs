#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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
	public interface IAdapterConfigServiceCertificate
	{
		/// <summary>
		/// Specify the thumbprint of the X.509 certificate that a receive location or send port uses to authenticate
		/// the service.
		/// </summary>
		/// <remarks>
		/// <list type="bullet">
		/// <item>
		/// Inbound &#8212; Specify the thumbprint of the X.509 certificate for this receive location that the clients
		/// use to authenticate the service. The certificate to be used for this property must be installed into the My
		/// store in the Current User location.
		/// </item>
		/// <item>
		/// Outbound &#8212; Specify the thumbprint of the X.509 certificate for authenticating the service to which
		/// this send port sends messages. The certificate to be used for this property must be installed into the
		/// Other People store in the Local Machine location.
		/// </item>
		/// </list>
		/// It defaults to an <see cref="string.Empty"/> string.
		/// </remarks>
		string ServiceCertificate { get; set; }
	}
}
