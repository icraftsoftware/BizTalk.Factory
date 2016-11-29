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
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.Detailed;
using Be.Stateless.BizTalk.Dsl.Binding.Subscription;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.Orchestrations.Dummy;
using Be.Stateless.BizTalk.Pipelines;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Tracking;
using Microsoft.Adapters.Sql;

namespace Be.Stateless.Finance
{
	internal class SampleApplicationWithArea : ApplicationBinding<NamingConvention<Party, MessageName>>
	{
		public SampleApplicationWithArea()
		{
			ReceivePorts.Add(new Invoice.TaxAgencyReceivePort());
			SendPorts.Add(new Invoice.BankSendPort());
		}
	}

	namespace Invoice
	{
		internal class BankSendPort : BizTalk.Dsl.Binding.Convention.Detailed.BankSendPort { }

		internal class TaxAgencyReceivePort : BizTalk.Dsl.Binding.Convention.Detailed.TaxAgencyReceivePort { }
	}
}

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention.Detailed
{
	internal class SampleApplication : ApplicationBinding<NamingConvention<Party, MessageName>>
	{
		public SampleApplication()
		{
			Name = ApplicationName.Is("Detailed.SampleApplication");
			ReceivePorts.Add(
				CustomerOneWayReceivePort = ReceivePort(
					p => {
						p.Name = ReceivePortName.Offwards(Party.Customer);
						p.ReceiveLocations
							.Add(
								ReceiveLocation(
									l => {
										l.Name = ReceiveLocationName.About(MessageName.Invoice).FormattedAs.Xml;
										l.Enabled = false;
										l.ReceivePipeline = new ReceivePipeline<XmlReceive>();
										l.Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\files\drops"; });
										l.Transport.Host = Host.RECEIVING_HOST;
									}),
								ReceiveLocation(
									l => {
										l.Name = ReceiveLocationName.About(MessageName.CreditNote).FormattedAs.Edi;
										l.Enabled = false;
										l.ReceivePipeline = new ReceivePipeline<XmlReceive>();
										l.Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\files\drops"; });
										l.Transport.Host = Host.RECEIVING_HOST;
									})
							);
					}),
				CustomerTwoWayReceivePort = ReceivePort(
					p => {
						p.Name = ReceivePortName.Offwards(Party.Customer);
						p.Description = "Receives ledgers from customers";
						p.ReceiveLocations.Add(
							ReceiveLocation(
								l => {
									l.Name = ReceiveLocationName.About(MessageName.Statement).FormattedAs.Csv;
									l.Enabled = true;
									l.ReceivePipeline = new ReceivePipeline<PassThruReceive>(pl => { pl.Decoder<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; }); });
									l.SendPipeline = new SendPipeline<PassThruTransmit>(pl => { pl.PreAssembler<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; }); });
									l.Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\files\drops"; });
									l.Transport.Host = Host.RECEIVING_HOST;
								}));
					}),
				ReceivePort(
					p => {
						p.Name = ReceivePortName.Offwards(Party.Bank);
						p.Description = "Receives financial movements from bank";
						p.ReceiveLocations.Add(
							ReceiveLocation(
								l => {
									l.Name = ReceiveLocationName.About(MessageName.Statement).FormattedAs.Xml;
									l.Enabled = true;
									l.ReceivePipeline = new ReceivePipeline<MicroPipelines.XmlReceive>(
										pl => {
											pl.Decoder<MicroPipelineComponent>(
												c => {
													c.Enabled = false;
													c.Components = new IMicroPipelineComponent[] {
														new FailedMessageRoutingEnabler { EnableFailedMessageRouting = true, SuppressRoutingFailureReport = false },
														new ActivityTracker { TrackingModes = ActivityTrackingModes.Claim, TrackingContextCacheDuration = TimeSpan.FromSeconds(120) }
													};
												});
										});
									l.Transport.Adapter = new WcfSqlAdapter.Inbound(
										a => {
											a.Address = new SqlAdapterConnectionUri { InboundId = "FinancialMovements", InitialCatalog = "BankDb", Server = "localhost" };
											a.InboundOperationType = InboundOperation.XmlPolling;
											a.PolledDataAvailableStatement = "select count(1) from data";
											a.PollingStatement = "select * from data for XML";
											a.PollingInterval = TimeSpan.FromHours(2);
											a.PollWhileDataFound = true;
										});
									l.Transport.Host = Host.RECEIVING_HOST;
								}));
					}),
				TaxAgencyOneWayReceivePort = new TaxAgencyReceivePort());
			SendPorts.Add(
				BankOneWaySendPort = new BankSendPort(),
				CustomerTwoWaySendPort = SendPort(
					p => {
						p.Name = SendPortName.Towards(Party.Customer).About(MessageName.Statement).FormattedAs.Csv;
						p.SendPipeline = new SendPipeline<PassThruTransmit>(pl => { pl.PreAssembler<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; }); });
						p.ReceivePipeline = new ReceivePipeline<PassThruReceive>(pl => { pl.Decoder<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; }); });
						p.Transport.Adapter = new FileAdapter.Outbound(a => { a.DestinationFolder = @"c:\files\drops"; });
						p.Transport.RetryPolicy = RetryPolicy.LongRunning;
						p.Transport.Host = Host.SENDING_HOST;
					}));
			Orchestrations.Add(
				new ProcessOrchestrationBinding(
					o => {
						o.ReceivePort = CustomerOneWayReceivePort;
						o.RequestResponsePort = CustomerTwoWayReceivePort;
						o.SendPort = BankOneWaySendPort;
						o.SolicitResponsePort = CustomerTwoWaySendPort;
						o.Host = Host.PROCESSING_HOST;
					}));
		}

		internal IReceivePort<NamingConvention<Party, MessageName>> CustomerOneWayReceivePort { get; set; }

		internal ISendPort<NamingConvention<Party, MessageName>> CustomerTwoWaySendPort { get; set; }

		internal TaxAgencyReceivePort TaxAgencyOneWayReceivePort { get; set; }

		private BankSendPort BankOneWaySendPort { get; set; }

		private IReceivePort<NamingConvention<Party, MessageName>> CustomerTwoWayReceivePort { get; set; }
	}

	internal class BankSendPort : SendPort<NamingConvention<Party, MessageName>>
	{
		public BankSendPort()
		{
			Name = SendPortName.Towards(Party.Bank).About(MessageName.CreditNote).FormattedAs.Edi;
			SendPipeline = new SendPipeline<XmlTransmit>();
			Transport.Adapter = new FileAdapter.Outbound(a => { a.DestinationFolder = @"c:\files\drops"; });
			Transport.Host = Host.SENDING_HOST;
			Transport.RetryPolicy = RetryPolicy.LongRunning;
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
						l.Enabled = false;
						l.ReceivePipeline = new ReceivePipeline<PassThruReceive>(
							pl => {
								pl.Decoder<PolicyRunnerComponent>(
									c => {
										c.Enabled = true;
										c.ExecutionMode = PluginExecutionMode.Deferred;
										c.Policy = Policy<Policies.Send.Claim.ProcessResolver>.Name;
									});
							});
						l.Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\files\drops"; });
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
