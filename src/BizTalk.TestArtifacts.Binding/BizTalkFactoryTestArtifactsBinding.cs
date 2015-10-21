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

using System.ServiceModel;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.EnvironmentSettings;
using Be.Stateless.BizTalk.Orchestrations.Dummy;
using Be.Stateless.BizTalk.Pipelines;
using Microsoft.Adapters.Sql;
using NamingConvention = Be.Stateless.BizTalk.Dsl.Binding.Convention.BizTalkFactory.NamingConvention<string, string>;

namespace Be.Stateless.BizTalk
{
	public class BizTalkFactoryTestArtifactsBinding : BizTalkFactoryApplicationBinding
	{
		public BizTalkFactoryTestArtifactsBinding()
		{
			SendPorts.Add(
				_twoWaySendPort = SendPort(
					sp => {
						sp.Name = SendPortName.Towards("TestArtifacts").About("Dummy").FormattedAs.None;
						sp.ReceivePipeline = new ReceivePipeline<PassThruReceive>();
						sp.SendPipeline = new SendPipeline<PassThruTransmit>();
						sp.State = ServiceState.Unenlisted;
						sp.Transport.Adapter = new WcfSqlAdapter.Outbound(
							a => {
								a.Address = new SqlAdapterConnectionUri { InitialCatalog = "BizTalkFactoryTransientStateDb", Server = CommonSettings.ProcessingDatabaseServer };
								a.StaticAction = "TypedProcedure/dbo/usp_batch_AddPart";
							});
						sp.Transport.Host = CommonSettings.TransmitHost;
					}));
			ReceivePorts.Add(
				_twoWayReceivePort = ReceivePort(
					rp => {
						rp.Name = ReceivePortName.Offwards("TestArtifacts");
						rp.ReceiveLocations.Add(
							ReceiveLocation(
								rl => {
									rl.Name = ReceiveLocationName.About("Dummy").FormattedAs.None;
									rl.Enabled = false;
									rl.ReceivePipeline = new ReceivePipeline<PassThruReceive>();
									rl.SendPipeline = new SendPipeline<PassThruTransmit>();
									rl.Transport.Adapter = new WcfNetTcpAdapter.Inbound(a => { a.Address = new EndpointAddress("net.tcp://localhost/dummy.svc"); });
									rl.Transport.Host = CommonSettings.ReceiveHost;
								}));
					}));
			Orchestrations.Add(
				new ProcessOrchestrationBinding(
					o => {
						o.ReceivePort = _batchReceivePort;
						o.RequestResponsePort = _twoWayReceivePort;
						o.SendPort = _batchSendPort;
						o.SolicitResponsePort = _twoWaySendPort;
						o.Host = CommonSettings.ReceiveHost;
						o.State = ServiceState.Unenlisted;
					}));
		}

		private readonly IReceivePort<NamingConvention> _twoWayReceivePort;
		private readonly ISendPort<NamingConvention> _twoWaySendPort;
	}
}
