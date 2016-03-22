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

using System;
using System.Xml.Serialization;
using Microsoft.BizTalk.Adapter.Framework;
using Microsoft.BizTalk.Adapter.FtpAdapter;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class FtpAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// The FTP adapter exchanges data between an FTP server and Microsoft BizTalk Server, and allows for the
		/// integration of vital data stored on a variety of platforms with line-of-business applications. The adapter can
		/// connect to the FTP server via SOCKS4 or SOCKS5 proxy server.
		/// </summary>
		/// <remarks>
		/// The FTP adapter can transfer a large number of files from an FTP server to BizTalk Server. It can also
		/// transfer files as part of an orchestration.
		/// </remarks>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa561215.aspx">FTP Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa559267.aspx">What Is the FTP Adapter?</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa561032.aspx">Configuring the FTP Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa578371.aspx">FTP Commands that are Required by the FTP Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa561727.aspx">FTP Adapter Security Recommendations</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa561990.aspx">Best Practices for the FTP Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/ff629768.aspx">Enhancements to the FTP Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa546802.aspx">How to Configure an FTP Send Port</seealso>
		[XmlRoot("Config")]
		public class Outbound : FtpAdapter, IOutboundAdapter
		{
			public Outbound() { }

			public Outbound(Action<Outbound> adapterConfigurator) : this()
			{
				adapterConfigurator(this);
			}

			#region Base Class Member Overrides

			protected override string GetAddress()
			{
				throw new NotImplementedException();
			}

			#endregion

			#region Base Class Member Overrides

			protected override void Save(IPropertyBag propertyBag)
			{
				throw new NotImplementedException();
			}

			protected override void Validate()
			{
				var config = Serialize();
				var validator = new FtpAdapterManagement();
				validator.ValidateConfiguration(ConfigType.TransmitLocation, config);
			}

			#endregion
		}

		#endregion
	}
}
