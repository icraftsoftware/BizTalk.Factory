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
using System.Xml;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.BizTalkFactory;
using Be.Stateless.BizTalk.Dsl.Binding.Subscription;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.Orchestrations.Dummy;
using Be.Stateless.BizTalk.Pipelines;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Unit.Resources;
using NUnit.Framework;
using SampleNamingConventions = Be.Stateless.BizTalk.Dsl.Binding.Convention.BizTalkFactory.NamingConvention<
	Be.Stateless.BizTalk.Dsl.Binding.Convention.Party,
	Be.Stateless.BizTalk.Dsl.Binding.Convention.MessageName>;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	[TestFixture]
	public class ApplicationBindingSerializerFixture
	{
		[Test]
		public void GenerateBindingsWithAreaConventions()
		{
			var applicationBinding = new Finance.SampleApplication() {
				Timestamp = XmlConvert.ToDateTime("2015-02-17T22:51:04+01:00", XmlDateTimeSerializationMode.Local)
			};
			var applicationBindingSerializer = ((IBindingSerializerFactory) applicationBinding).GetBindingSerializer();

			var binding = applicationBindingSerializer.Serialize();

			Assert.That(binding, Is.EqualTo(ResourceManager.LoadString("Data.bindings.with.area.conventions.xml")));
		}

		[Test]
		public void GenerateBindingsWithConventions()
		{
			var applicationBinding = new SampleApplication {
				Timestamp = XmlConvert.ToDateTime("2015-02-17T22:51:04+01:00", XmlDateTimeSerializationMode.Local)
			};
			var applicationBindingSerializer = ((IBindingSerializerFactory) applicationBinding).GetBindingSerializer();

			var binding = applicationBindingSerializer.Serialize();

			Assert.That(binding, Is.EqualTo(ResourceManager.LoadString("Data.bindings.with.conventions.xml")));
		}

		[Test]
		public void GenerateBindingsWithoutConventions()
		{
			var applicationBinding = new ApplicationBinding(
				a => {
					a.Name = "Unconventional application name";
					a.Description = "Some useless test application.";
					a.ReceivePorts.Add(
						new ReceivePort(
							p => {
								p.Name = "Unconventional receive port name";
								p.Description = "Some useless test receive port.";
								p.ReceiveLocations.Add(
									new ReceiveLocation(
										l => {
											l.Name = "Unconventional receive location name";
											l.Description = "Some useless test receive location.";
											l.ReceivePipeline = new ReceivePipeline<PassThruReceive>();
											l.Transport.Adapter = new InboundFileAdapter(t => { t.ReceiveFolder = @"c:\files\drops"; });
											l.Transport.Host = "Receive Host";
										})
									);
							})
						);
					a.SendPorts.Add(
						new SendPort(
							p => {
								p.Name = "Unconventional send port name";
								p.Description = "Some useless test send port.";
								p.SendPipeline = new SendPipeline<PassThruTransmit>();
								p.Transport.Adapter = new OutboundFileAdapter(t => { t.DestinationFolder = @"c:\files\drops"; });
								p.Transport.Host = "Send Host";
							})
						);
				}) {
					Timestamp = XmlConvert.ToDateTime("2015-02-18T22:51:04+01:00", XmlDateTimeSerializationMode.Local)
				};
			var applicationBindingSerializer = ((IBindingSerializerFactory) applicationBinding).GetBindingSerializer();

			var binding = applicationBindingSerializer.Serialize();

			Assert.That(binding, Is.EqualTo(ResourceManager.LoadString("Data.bindings.without.conventions.xml")));
		}

		private class SampleApplication : ApplicationBinding<SampleNamingConventions>
		{
			public SampleApplication()
			{
				// do not assign application Name and rely on SampleNamingConventions default rule
				Orchestrations.Add(
					new ProcessOrchestrationBinding(
						o => {
							o.ReceivePort = new TestApplication.OneWayReceivePort();
							o.RequestResponsePort = new TestApplication.TwoWayReceivePort();
							o.SendPort = new TestApplication.OneWaySendPort();
							o.SolicitResponsePort = new TestApplication.TwoWaySendPort();
							o.Host = "Processing Host";
						}));
				ReceivePorts.Add(
					ReceivePort(
						p => {
							p.Name = ReceivePortName.Offwards(Party.Customer);
							p.ReceiveLocations
								.Add(
									ReceiveLocation(
										l => {
											l.Name = ReceiveLocationName.About(MessageName.Invoice).FormattedAs.Xml;
											l.Enable = false;
											l.ReceivePipeline = new ReceivePipeline<XmlReceive>();
											l.Transport.Adapter = new InboundFileAdapter(t => { t.ReceiveFolder = @"c:\files\drops"; });
											l.Transport.Host = Host.RECEIVING_HOST;
										}),
									ReceiveLocation(
										l => {
											l.Name = ReceiveLocationName.About(MessageName.CreditNote).FormattedAs.Edi;
											l.Enable = false;
											l.ReceivePipeline = new ReceivePipeline<XmlReceive>();
											l.Transport.Adapter = new InboundFileAdapter(t => { t.ReceiveFolder = @"c:\files\drops"; });
											l.Transport.Host = Host.RECEIVING_HOST;
										})
								);
						}),
					ReceivePort(
						p => {
							p.Name = ReceivePortName.Offwards(Party.Customer);
							p.Description = "Receives ledgers from customers";
							p.ReceiveLocations.Add(
								ReceiveLocation(
									l => {
										l.Name = ReceiveLocationName.About(MessageName.Statement).FormattedAs.Csv;
										l.Enable = true;
										l.ReceivePipeline = new ReceivePipeline<PassThruReceive>(pl => { pl.Decoder<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; }); });
										l.SendPipeline = new SendPipeline<PassThruTransmit>(pl => { pl.PreAssembler<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; }); });
										l.Transport.Adapter = new InboundFileAdapter(t => { t.ReceiveFolder = @"c:\files\drops"; });
										l.Transport.Host = Host.RECEIVING_HOST;
									}));
						}),
					new TaxAgencyReceivePort());
				SendPorts.Add(
					new BankSendPort(),
					SendPort(
						p => {
							p.Name = SendPortName.Towards(Party.Customer).About(MessageName.Statement).FormattedAs.Csv;
							p.SendPipeline = new SendPipeline<PassThruTransmit>(pl => { pl.PreAssembler<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; }); });
							p.ReceivePipeline = new ReceivePipeline<PassThruReceive>(pl => { pl.Decoder<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; }); });
							p.Transport.Adapter = new OutboundFileAdapter(t => { t.DestinationFolder = @"c:\files\drops"; });
							p.Transport.RetryPolicy = Convention.BizTalkFactory.RetryPolicy.LongRunning;
							p.Transport.Host = Host.SENDING_HOST;
						}));
			}
		}

		internal class BankSendPort : SendPort<NamingConvention<Party, MessageName>>
		{
			public BankSendPort()
			{
				Name = SendPortName.Towards(Party.Bank).About(MessageName.CreditNote).FormattedAs.Edi;
				SendPipeline = new SendPipeline<XmlTransmit>();
				Transport.Adapter = new OutboundFileAdapter(t => { t.DestinationFolder = @"c:\files\drops"; });
				Transport.Host = Host.SENDING_HOST;
				Transport.RetryPolicy = Convention.BizTalkFactory.RetryPolicy.LongRunning;
				Filter = new Filter(() => BtsProperties.MessageType == Schema<Batch.Content>.MessageType);
			}
		}

		internal class TaxAgencyReceivePort : ReceivePort<NamingConvention<Party, MessageName>>
		{
			public TaxAgencyReceivePort()
			{
				Name = ReceivePortName.Offwards(Party.TaxAgency);
				ReceiveLocations.Add(
					ReceiveLocation(
						l => {
							l.Name = ReceiveLocationName.About(MessageName.Statement).FormattedAs.Xml;
							l.Enable = false;
							l.ReceivePipeline = new ReceivePipeline<PassThruReceive>(
								pl => {
									pl.Decoder<PolicyRunnerComponent>(
										c => {
											c.Enabled = true;
											c.ExecutionMode = PluginExecutionMode.Deferred;
											c.Policy = Policy<Policies.Send.Claim.ProcessResolver>.Name;
										});
								});
							l.Transport.Adapter = new InboundFileAdapter(t => { t.ReceiveFolder = @"c:\files\drops"; });
							l.Transport.Host = Host.ISOLATED_HOST;
							l.Transport.Schedule = new Schedule {
								StartDate = new DateTime(2015, 2, 17),
								StopDate = new DateTime(2015, 2, 17).AddDays(12),
								ServiceWindow = new ServiceWindow {
									StartTime = new Time(13, 15),
									StopTime = new Time(14, 15)
								}
							};
						})
					);
			}
		}
	}
}

namespace Be.Stateless.Finance
{
	internal class SampleApplication : ApplicationBinding<NamingConvention<Party, MessageName>>
	{
		public SampleApplication()
		{
			ReceivePorts.Add(new Invoice.TaxAgencyReceivePort());
			SendPorts.Add(new Invoice.BankSendPort());
		}
	}

	namespace Invoice
	{
		internal class BankSendPort : ApplicationBindingSerializerFixture.BankSendPort { }

		internal class TaxAgencyReceivePort : ApplicationBindingSerializerFixture.TaxAgencyReceivePort { }
	}
}
