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

using System.ServiceModel;
using System.Text;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public interface IAdapterConfigMessageEncoding
	{
		/// <summary>
		/// Specify the encoder used to encode the SOAP message.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <list type="bullet">
		/// <item>
		/// <see cref="WSMessageEncoding.Mtom"/> &#8212; Use a Message Transmission Optimization Mechanism 1.0 (MTOM)
		/// encoder.
		/// </item>
		/// <item>
		/// <see cref="WSMessageEncoding.Text"/> &#8212; Use a text message encoder.
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// It defaults to <see cref="WSMessageEncoding.Text"/>.
		/// </para>
		/// </remarks>
		WSMessageEncoding MessageEncoding { get; set; }

		/// <summary>
		/// Specify the character set encoding to be used for emitting messages on the binding when the <see
		/// cref="MessageEncoding"/> property is set to <see cref="WSMessageEncoding.Text"/>.
		/// </summary>
		/// <remarks>
		/// It defaults to <see cref="Encoding.UTF8"/>.
		/// </remarks>
		Encoding TextEncoding { get; set; }
	}
}
