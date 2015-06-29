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

using System;
using System.IO;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Subscription;
using Be.Stateless.BizTalk.Pipelines;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	internal class TestApplication : ApplicationBindingBase<string>
	{
		#region Nested Type: OneWayReceiveLocation

		// ReSharper disable once MemberCanBePrivate.Global
		internal class OneWayReceiveLocation : ReceiveLocationBase<string>
		{
			public OneWayReceiveLocation()
			{
				Name = "OneWayReceiveLocation";
				Description = "Some Useless One-Way Test Receive Location";
				Enable = false;
				ReceivePipeline = new ReceivePipeline<PassThruReceive>(
					pl => pl.Decoder<PolicyRunnerComponent>(
						c => {
							c.Enabled = true;
							c.ExecutionMode = PluginExecutionMode.Deferred;
							c.Policy = Policy<Policies.Send.Claim.ProcessResolver>.Name;
						}));
				Transport.Adapter = new DummyAdapter();

				Transport.Host = "Receive Host Name";
				Transport.Schedule = new Schedule {
					StartDate = DateTime.Today,
					StopDate = DateTime.Today.AddDays(12),
					ServiceWindow = new ServiceWindow {
						StartTime = new Time(13, 15),
						StopTime = new Time(14, 15)
					}
				};
			}
		}

		#endregion

		#region Nested Type: OneWayReceivePort

		internal class OneWayReceivePort : ReceivePortBase<string>
		{
			public OneWayReceivePort()
			{
				Name = "OneWayReceivePort";
				Description = "Some Useless One-Way Test Receive Port";
				ReceiveLocations.Add(new OneWayReceiveLocation());
			}
		}

		#endregion

		#region Nested Type: OneWaySendPort

		// ReSharper disable once MemberCanBePrivate.Global
		internal class OneWaySendPort : SendPortBase<string>
		{
			public OneWaySendPort()
			{
				Name = "OneWaySendPort";
				Description = "Some Useless One-Way Test Send Port";
				Filter = new Filter(() => BtsProperties.MessageType == Schema<Schemas.Xml.Batch.Content>.MessageType);
				Priority = Priority.Highest;
				OrderedDelivery = true;
				StopSendingOnOrderedDeliveryFailure = true;
				SendPipeline = new SendPipeline<PassThruTransmit>(
					sp => sp.PreAssembler<PolicyRunnerComponent>(
						c => {
							c.Enabled = true;
							c.ExecutionMode = PluginExecutionMode.Deferred;
							c.Policy = Policy<Policies.Send.Claim.ProcessResolver>.Name;
						}));
				Transport.Adapter = new DummyAdapter();
				Transport.Host = "Send Host Name";
				Transport.RetryPolicy = new RetryPolicy { Count = 30, Interval = TimeSpan.FromMinutes(60) };
				Transport.ServiceWindow = new ServiceWindow { StartTime = new Time(8, 0), StopTime = new Time(20, 0) };
			}
		}

		#endregion

		#region Nested Type: TwoWayReceiveLocation

		internal class TwoWayReceiveLocation : ReceiveLocationBase<string>
		{
			public TwoWayReceiveLocation()
			{
				Name = "TwoWayReceiveLocation";
				Description = "Some Useless Two-Way Test Receive Location";
				Enable = false;
				ReceivePipeline = new ReceivePipeline<PassThruReceive>();
				SendPipeline = new SendPipeline<PassThruTransmit>(
					pl => pl.PreAssembler<PolicyRunnerComponent>(
						c => {
							c.Enabled = true;
							c.ExecutionMode = PluginExecutionMode.Deferred;
							c.Policy = Policy<Policies.Send.Claim.ProcessResolver>.Name;
						}));
				Transport.Adapter = new DummyAdapter();
				Transport.Host = "Receive Host Name";
			}
		}

		#endregion

		#region Nested Type: TwoWayReceivePort

		internal class TwoWayReceivePort : ReceivePortBase<string>
		{
			public TwoWayReceivePort()
			{
				Name = "TwoWayReceivePort";
				Description = "Some Useless Two-Way Test Receive Port";
				ReceiveLocations.Add(new TwoWayReceiveLocation());
			}
		}

		#endregion

		#region Nested Type: TwoWaySendPort

		internal class TwoWaySendPort : SendPortBase<string>
		{
			public TwoWaySendPort()
			{
				Name = "TwoWaySendPort";
				Description = "Some Useless Two-Way Test Send Port";
				SendPipeline = new SendPipeline<PassThruTransmit>();
				ReceivePipeline = new ReceivePipeline<PassThruReceive>(
					rp => rp.Decoder<PolicyRunnerComponent>(
						c => {
							c.Enabled = true;
							c.ExecutionMode = PluginExecutionMode.Deferred;
							c.Policy = Policy<Policies.Send.Claim.ProcessResolver>.Name;
						}));
				Transport.Adapter = new DummyAdapter();
				Transport.Host = "Send Host Name";
			}
		}

		#endregion

		#region Nested Type: DummyAdapter

		private class DummyAdapter : AdapterBase, IInboundAdapter, IOutboundAdapter, IAdapterBindingSerializerFactory, IDslSerializer
		{
			#region IAdapterBindingSerializerFactory Members

			public IDslSerializer GetAdapterBindingSerializer()
			{
				return this;
			}

			#endregion

			#region IDslSerializer Members

			public string Serialize()
			{
				return null;
			}

			public void Save(string filePath)
			{
				throw new NotSupportedException();
			}

			public void Write(Stream stream)
			{
				throw new NotSupportedException();
			}

			#endregion

			#region IInboundAdapter Members

			public string Address
			{
				get { return @"c:\files\drops\*.xml"; }
			}

			public ProtocolType ProtocolType
			{
				get { return new ProtocolType { Name = "Test Dummy" }; }
			}

			public void Load(IPropertyBag propertyBag)
			{
				throw new NotSupportedException();
			}

			public void Save(IPropertyBag propertyBag)
			{
				throw new NotSupportedException();
			}

			#endregion

			#region Base Class Member Overrides

			protected override void Validate() { }

			#endregion
		}

		#endregion

		public TestApplication()
		{
			Name = "TestApplication";
			Description = "Some Useless Test Application";
		}
	}
}
