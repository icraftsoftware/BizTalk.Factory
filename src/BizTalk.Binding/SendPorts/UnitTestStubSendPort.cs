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
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.Simple;
using Be.Stateless.BizTalk.EnvironmentSettings;
using Be.Stateless.BizTalk.MicroPipelines;
using RetryPolicy = Be.Stateless.BizTalk.Dsl.Binding.Convention.RetryPolicy;

namespace Be.Stateless.BizTalk
{
	public class UnitTestStubSendPort : SendPort<NamingConvention>
	{
		public UnitTestStubSendPort()
		{
			Name = SendPortName.Towards("UnitTest").About("Stub").FormattedAs.Xml;
			State = ServiceState.Started;
			SendPipeline = new SendPipeline<XmlTransmit>();
			ReceivePipeline = new ReceivePipeline<XmlReceive>();
			Transport.Adapter = new WcfBasicHttpAdapter.Outbound(a => { a.Address = new EndpointAddress("http://localhost:8000/stubservice"); });
			Transport.Host = CommonSettings.TransmitHost;
			Transport.RetryPolicy = RetryPolicy.RealTime;
		}
	}
}
