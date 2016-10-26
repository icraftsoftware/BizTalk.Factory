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
using System.ServiceModel.Configuration;
using System.Xml;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.BizTalkFactory;
using Be.Stateless.BizTalk.Dsl.Binding.Subscription;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.Orchestrations.Dummy;
using Be.Stateless.BizTalk.Pipelines;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.BizTalk.Unit.Resources;
using Microsoft.Adapters.OracleDB;
using Microsoft.Adapters.Sql;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.B2B.PartnerManagement;
using NUnit.Framework;
using NamingConvention = Be.Stateless.BizTalk.Dsl.Binding.Convention.BizTalkFactory.NamingConvention<string, string>;
using SampleNamingConventions = Be.Stateless.BizTalk.Dsl.Binding.Convention.BizTalkFactory.NamingConvention<
	Be.Stateless.BizTalk.Dsl.Binding.Convention.BizTalkFactory.Party,
	Be.Stateless.BizTalk.Dsl.Binding.Convention.BizTalkFactory.MessageName>;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention.BizTalkFactory
{
	[TestFixture]
	public class NamingConventionFixture
	{
		[Test]
		public void ComputeAdapterNameResolvesActualProtocolTypeNameForWcfCustomAdapter()
		{
			IAdapter adapter = new CustomAdapterFake<NetTcpBindingElement, CustomRLConfig>();
			Assert.That(NamingConventionDouble.Instance.ComputeAdapterName(adapter), Is.EqualTo("WCF-CustomNetTcp"));

			adapter = new CustomAdapterFake<NetMsmqBindingElement, CustomRLConfig>();
			Assert.That(NamingConventionDouble.Instance.ComputeAdapterName(adapter), Is.EqualTo("WCF-CustomNetMsmq"));

			adapter = new CustomAdapterFake<OracleDBBindingConfigurationElement, CustomRLConfig>();
			Assert.That(NamingConventionDouble.Instance.ComputeAdapterName(adapter), Is.EqualTo("WCF-CustomOracleDB"));

			adapter = new CustomAdapterFake<SqlAdapterBindingConfigurationElement, CustomRLConfig>();
			Assert.That(NamingConventionDouble.Instance.ComputeAdapterName(adapter), Is.EqualTo("WCF-CustomSql"));
		}

		[Test]
		public void ComputeAdapterNameResolvesActualProtocolTypeNameForWcfCustomIsolatedAdapter()
		{
			IAdapter adapter = new CustomIsolatedAdapterFake<NetTcpBindingElement, CustomRLConfig>();
			Assert.That(NamingConventionDouble.Instance.ComputeAdapterName(adapter), Is.EqualTo("WCF-CustomIsolatedNetTcp"));

			adapter = new CustomIsolatedAdapterFake<WSHttpBindingElement, CustomRLConfig>();
			Assert.That(NamingConventionDouble.Instance.ComputeAdapterName(adapter), Is.EqualTo("WCF-CustomIsolatedWsHttp"));

			adapter = new CustomIsolatedAdapterFake<BasicHttpBindingElement, CustomRLConfig>();
			Assert.That(NamingConventionDouble.Instance.ComputeAdapterName(adapter), Is.EqualTo("WCF-CustomIsolatedBasicHttp"));
		}

		[Test]
		public void ConventionalApplicationBindingSupportsBindingGeneration()
		{
			var applicationBinding = new SampleApplication {
				Timestamp = XmlConvert.ToDateTime("2015-02-17T22:51:04+01:00", XmlDateTimeSerializationMode.Local)
			};
			var applicationBindingSerializer = ((IBindingSerializerFactory) applicationBinding).GetBindingSerializer("PRD");

			var binding = applicationBindingSerializer.Serialize();

			Assert.That(binding, Is.EqualTo(ResourceManager.LoadString("Data.bindings.xml")));
		}

		[Test]
		public void ConventionalApplicationBindingWithAreaSupportsBindingGeneration()
		{
			var applicationBinding = new Finance.SampleApplication {
				Timestamp = XmlConvert.ToDateTime("2015-02-17T22:51:04+01:00", XmlDateTimeSerializationMode.Local)
			};
			var applicationBindingSerializer = ((IBindingSerializerFactory) applicationBinding).GetBindingSerializer("PRD");

			var binding = applicationBindingSerializer.Serialize();

			Assert.That(binding, Is.EqualTo(ResourceManager.LoadString("Data.bindings.with.area.xml")));
		}

		[Test]
		public void ConventionalReceivePortNameCanBeReferencedInSubscriptionFilter()
		{
			var receivePort = new ConventionalApplicationBinding().ReceivePort;
			var filter = new Filter(() => BtsProperties.ReceivePortName == receivePort.Name);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BtsProperties.ReceivePortName.Type.FullName,
						(int) FilterOperator.Equals,
						((ISupportNamingConvention) receivePort).Name)));
		}

		[Test]
		public void ConventionalSendPortNameCanBeReferencedInSubscriptionFilter()
		{
			var sendPort = new ConventionalApplicationBinding().SendPort;
			var filter = new Filter(() => BtsProperties.SendPortName == sendPort.Name);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BtsProperties.SendPortName.Type.FullName,
						(int) FilterOperator.Equals,
						((ISupportNamingConvention) sendPort).Name)));
		}

		[Test]
		public void ConventionalStandaloneReceivePortNameCanBeReferencedInSubscriptionFilter()
		{
			var receivePort = new ConventionalApplicationBinding().StandaloneReceivePort;
			var filter = new Filter(() => BtsProperties.ReceivePortName == receivePort.Name);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BtsProperties.ReceivePortName.Type.FullName,
						(int) FilterOperator.Equals,
						((ISupportNamingConvention) receivePort).Name)));
		}

		private class ConventionalApplicationBinding : ApplicationBinding<NamingConvention>
		{
			public ConventionalApplicationBinding()
			{
				Name = ApplicationName.Is("BizTalk.Factory");
				SendPorts.Add(SendPort);
				ReceivePorts.Add(ReceivePort);
				ReceivePorts.Add(StandaloneReceivePort);
			}

			internal IReceivePort<NamingConvention> ReceivePort
			{
				get
				{
					return _receivePort ?? (_receivePort = ReceivePort(
						rp => {
							rp.Name = ReceivePortName.Offwards("Batch");
							rp.ReceiveLocations.Add(
								ReceiveLocation(
									rl => {
										rl.Name = ReceiveLocationName.About("Release").FormattedAs.Xml;
										rl.ReceivePipeline = new ReceivePipeline<BatchReceive>();
										rl.Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\files\drops"; });
										rl.Transport.Host = "Host";
									}));
						}));
				}
			}

			internal ISendPort<NamingConvention> SendPort
			{
				get
				{
					return _sendPort ?? (_sendPort = SendPort(
						sp => {
							sp.Name = SendPortName.Towards("UnitTest.Batch").About("Trace").FormattedAs.Xml;
							sp.SendPipeline = new SendPipeline<PassThruTransmit>();
							sp.Transport.Adapter = new FileAdapter.Outbound(a => { a.DestinationFolder = @"C:\Files\Drops\BizTalk.Factory\Trace"; });
							sp.Transport.Host = "Host";
						}));
				}
			}

			internal ConventionalStandaloneReceivePort StandaloneReceivePort
			{
				get { return _standaloneReceivePort ?? (_standaloneReceivePort = new ConventionalStandaloneReceivePort()); }
			}

			private IReceivePort<NamingConvention> _receivePort;

			private ISendPort<NamingConvention> _sendPort;

			private ConventionalStandaloneReceivePort _standaloneReceivePort;
		}

		private class ConventionalStandaloneReceivePort : ReceivePort<NamingConvention>
		{
			public ConventionalStandaloneReceivePort()
			{
				Name = ReceivePortName.Offwards("StandaloneBatch");
				ReceiveLocations.Add(
					ReceiveLocation(
						rl => {
							rl.Name = ReceiveLocationName.About("Release").FormattedAs.Xml;
							rl.ReceivePipeline = new ReceivePipeline<BatchReceive>();
							rl.Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\files\drops"; });
							rl.Transport.Host = "Host";
						}));
			}
		}

		private class CustomAdapterFake<TBinding, TConfig> : WcfCustomAdapter<TBinding, TConfig>
			where TBinding : StandardBindingElement, new()
			where TConfig : AdapterConfig,
			IAdapterConfigAddress,
			IAdapterConfigIdentity,
			IAdapterConfigBinding,
			IAdapterConfigEndpointBehavior,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigOutboundMessageMarshalling,
			new() { }

		private class CustomIsolatedAdapterFake<TBinding, TConfig> : WcfCustomIsolatedAdapter<TBinding, TConfig>
			where TBinding : StandardBindingElement, new()
			where TConfig : RLConfig,
			IAdapterConfigBinding,
			IAdapterConfigEndpointBehavior,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigOutboundMessageMarshalling,
			new() { }

		private class NamingConventionDouble : NamingConvention<string, string>
		{
			public new string ComputeAdapterName(IAdapter adapter)
			{
				return base.ComputeAdapterName(adapter);
			}

			internal static readonly NamingConventionDouble Instance = new NamingConventionDouble();
		}

		private class SampleApplication : ApplicationBinding<SampleNamingConventions>
		{
			public SampleApplication()
			{
				// do not assign application Name and rely on SampleNamingConventions default rule
				ReceivePorts.Add(
					_oneWayReceivePort = ReceivePort(
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
					_twoWayReceivePort = ReceivePort(
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
												a.InboundOperationType = Microsoft.Adapters.Sql.InboundOperation.XmlPolling;
												a.PolledDataAvailableStatement = "select count(1) from data";
												a.PollingStatement = "select * from data for XML";
												a.PollingInterval = TimeSpan.FromHours(2);
												a.PollWhileDataFound = true;
											});
										l.Transport.Host = Host.RECEIVING_HOST;
									}));
						}),
					new TaxAgencyReceivePort());
				SendPorts.Add(
					_oneWaySendPort = new BankSendPort(),
					_twoWaySendPort = SendPort(
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
							o.ReceivePort = _oneWayReceivePort;
							o.RequestResponsePort = _twoWayReceivePort;
							o.SendPort = _oneWaySendPort;
							o.SolicitResponsePort = _twoWaySendPort;
							//o.ReceivePort = new TestApplication.OneWayReceivePort();
							//o.RequestResponsePort = new TestApplication.TwoWayReceivePort();
							//o.SendPort = new TestApplication.OneWaySendPort();
							//o.SolicitResponsePort = new TestApplication.TwoWaySendPort();
							o.Host = Host.PROCESSING_HOST;
						}));
			}

			readonly IReceivePort<SampleNamingConventions> _oneWayReceivePort;
			readonly BankSendPort _oneWaySendPort;
			readonly IReceivePort<SampleNamingConventions> _twoWayReceivePort;
			readonly ISendPort<SampleNamingConventions> _twoWaySendPort;
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
		internal class BankSendPort : NamingConventionFixture.BankSendPort { }

		internal class TaxAgencyReceivePort : NamingConventionFixture.TaxAgencyReceivePort { }
	}
}
