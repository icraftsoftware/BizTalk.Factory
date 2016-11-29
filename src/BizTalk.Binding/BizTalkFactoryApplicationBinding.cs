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
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.Transactions;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.Constants;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.Simple;
using Be.Stateless.BizTalk.Dsl.Binding.Subscription;
using Be.Stateless.BizTalk.EnvironmentSettings;
using Be.Stateless.BizTalk.Install;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.MicroPipelines;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.BizTalk.Transforms.ToSql.Procedures.Batch;
using Be.Stateless.BizTalk.Transforms.ToSql.Procedures.Claim;
using Be.Stateless.BizTalk.XPath;
using Be.Stateless.Extensions;
using Microsoft.Adapters.Sql;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using RetryPolicy = Be.Stateless.BizTalk.Dsl.Binding.Convention.RetryPolicy;

namespace Be.Stateless.BizTalk
{
	[SuppressMessage("ReSharper", "FunctionComplexityOverflow")]
	public class BizTalkFactoryApplicationBinding : ApplicationBindingSingleton<BizTalkFactoryApplicationBinding>
	{
		public BizTalkFactoryApplicationBinding()
		{
			Name = ApplicationName.Is("BizTalk.Factory");
			Description = "Library to speed up the development of BizTalk Server applications.";
			SendPorts.Add(
				_batchAddPartSendPort = SendPort(
					sp => {
						sp.Name = SendPortName.Towards("Batch").About("AddPart").FormattedAs.Xml;
						sp.State = ServiceState.Started;
						sp.SendPipeline = new SendPipeline<XmlTransmit>(
							pipeline => {
								pipeline.Encoder<MicroPipelineComponent>(
									pc => {
										pc.Components = new IMicroPipelineComponent[] {
											new ActivityTracker { TrackingResolutionPolicyName = Policy<Policies.Send.Batch.AggregateProcessResolver>.Name },
											new XsltRunner { MapType = typeof(AnyToAddPart) }
										};
									});
							});
						sp.Transport.Adapter = new WcfSqlAdapter.Outbound(
							a => {
								a.Address = new SqlAdapterConnectionUri {
									InitialCatalog = "BizTalkFactoryTransientStateDb",
									Server = CommonSettings.ProcessingDatabaseHostName,
									InstanceName = CommonSettings.ProcessingDatabaseInstanceName
								};
								a.IsolationLevel = IsolationLevel.ReadCommitted;
								a.StaticAction = "TypedProcedure/dbo/usp_batch_AddPart";
							});
						sp.Transport.Host = CommonSettings.TransmitHost;
						sp.Transport.RetryPolicy = RetryPolicy.ShortRunning;
						sp.Filter = new Filter(() => BizTalkFactoryProperties.EnvelopeSpecName != null);
					}),
				SendPort(
					sp => {
						sp.Name = SendPortName.Towards("Batch").About("QueueControlledRelease").FormattedAs.Xml;
						sp.State = ServiceState.Started;
						sp.SendPipeline = new SendPipeline<XmlTransmit>(
							pipeline => {
								pipeline.Encoder<MicroPipelineComponent>(
									pc => {
										pc.Components = new IMicroPipelineComponent[] {
											new ActivityTracker { TrackingResolutionPolicyName = Policy<Policies.Send.Batch.ReleaseProcessResolver>.Name },
											new XsltRunner { MapType = typeof(ReleaseToQueueControlledRelease) }
										};
									});
							});
						sp.Transport.Adapter = new WcfSqlAdapter.Outbound(
							a => {
								a.Address = new SqlAdapterConnectionUri {
									InitialCatalog = "BizTalkFactoryTransientStateDb",
									Server = CommonSettings.ProcessingDatabaseHostName,
									InstanceName = CommonSettings.ProcessingDatabaseInstanceName
								};
								a.IsolationLevel = IsolationLevel.ReadCommitted;
								a.StaticAction = "TypedProcedure/dbo/usp_batch_QueueControlledRelease";
							});
						sp.Transport.Host = CommonSettings.TransmitHost;
						sp.Transport.RetryPolicy = RetryPolicy.ShortRunning;
						sp.Filter = new Filter(() => BtsProperties.MessageType == Schema<Batch.Release>.MessageType);
					}),
				SendPort(
					sp => {
						sp.Name = SendPortName.Towards("Claim").About("CheckIn").FormattedAs.Xml;
						sp.State = ServiceState.Started;
						sp.SendPipeline = new SendPipeline<XmlTransmit>(
							pipeline => {
								pipeline.Encoder<MicroPipelineComponent>(
									pc => {
										pc.Components = new IMicroPipelineComponent[] {
											new ActivityTracker { TrackingResolutionPolicyName = Policy<Policies.Send.Claim.ProcessResolver>.Name },
											new XsltRunner { MapType = typeof(ClaimToCheckIn) }
										};
									});
							});
						sp.Transport.Adapter = new WcfSqlAdapter.Outbound(
							a => {
								a.Address = new SqlAdapterConnectionUri {
									InitialCatalog = "BizTalkFactoryTransientStateDb",
									Server = CommonSettings.ProcessingDatabaseHostName,
									InstanceName = CommonSettings.ProcessingDatabaseInstanceName
								};
								a.IsolationLevel = IsolationLevel.ReadCommitted;
								a.StaticAction = "TypedProcedure/dbo/usp_claim_CheckIn";
							});
						sp.Transport.Host = CommonSettings.TransmitHost;
						sp.Transport.RetryPolicy = RetryPolicy.ShortRunning;
						sp.Filter = new Filter(() => BtsProperties.MessageType == Schema<Claim.CheckIn>.MessageType);
					}),
				SendPort(
					sp => {
						sp.Name = SendPortName.Towards("Sink").About("FailedMessage").FormattedAs.None;
						sp.State = ServiceState.Started;
						sp.SendPipeline = new SendPipeline<PassThruTransmit>(
							pipeline => {
								pipeline.PreAssembler<MicroPipelineComponent>(
									pc => {
										pc.Components = new IMicroPipelineComponent[] {
											new ActivityTracker { TrackingResolutionPolicyName = Policy<Policies.Send.FailedProcessResolver>.Name },
											new MessageConsumer()
										};
									});
							});
						sp.Transport.Adapter = new FileAdapter.Outbound(
							a => {
								a.DestinationFolder = @"C:\Files\Drops\BizTalk.Factory\Out";
								a.FileName = "Failed_%MessageID%.xml";
							});
						sp.Transport.Host = CommonSettings.TransmitHost;
						sp.Transport.RetryPolicy = RetryPolicy.RealTime;
						sp.Filter = new Filter(() => ErrorReportProperties.ErrorType == "FailedMessage");
					})
			);
			ReceivePorts.Add(
				_batchReceivePort = ReceivePort(
					rp => {
						rp.Name = ReceivePortName.Offwards("Batch");
						rp.ReceiveLocations.Add(
							ReceiveLocation(
								rl => {
									rl.Name = ReceiveLocationName.About("Release").FormattedAs.Xml;
									rl.Enabled = true;
									rl.ReceivePipeline = new ReceivePipeline<PassThruReceive>(
										pipeline => {
											pipeline.Decoder<MicroPipelineComponent>(
												pc => {
													pc.Components = new IMicroPipelineComponent[] {
														new BatchTracker(),
														new EnvelopeBuilder(),
														new ContextPropertyExtractor()
													};
												});
										});
									rl.Transport.Adapter = new WcfSqlAdapter.Inbound(
										a => {
											a.Address = new SqlAdapterConnectionUri {
												InboundId = "AvailableBatches",
												InitialCatalog = "BizTalkFactoryTransientStateDb",
												Server = CommonSettings.ProcessingDatabaseHostName,
												InstanceName = CommonSettings.ProcessingDatabaseInstanceName
											};
											a.PolledDataAvailableStatement = "SELECT COUNT(1) FROM vw_batch_NextAvailableBatch";
											a.PollingStatement = "EXEC usp_batch_ReleaseNextBatch";
											a.PollingInterval = BatchReleasePollingInterval;
											a.PollWhileDataFound = true;
											a.InboundOperationType = InboundOperation.XmlPolling;
											a.XmlStoredProcedureRootNodeName = "BodyWrapper";
											a.InboundBodyLocation = InboundMessageBodySelection.UseBodyPath;
											a.InboundBodyPathExpression = "/BodyWrapper/*";
											a.InboundNodeEncoding = MessageBodyFormat.Xml;
											a.ServiceBehaviors = new[] {
												new SqlAdapterInboundTransactionBehavior {
													TransactionIsolationLevel = IsolationLevel.ReadCommitted,
													TransactionTimeout = TimeSpan.FromMinutes(2)
												}
											};
										});
									rl.Transport.Host = CommonSettings.ReceiveHost;
								}));
					}),
				ReceivePort(
					rp => {
						rp.Name = ReceivePortName.Offwards("Claim");
						rp.ReceiveLocations.Add(
							ReceiveLocation(
								rl => {
									rl.Name = ReceiveLocationName.About("CheckOut").FormattedAs.Xml;
									rl.Enabled = true;
									rl.ReceivePipeline = new ReceivePipeline<XmlReceive>(
										pipeline => {
											pipeline.Validator<MicroPipelineComponent>(
												pc => {
													pc.Components = new IMicroPipelineComponent[] {
														new ContextPropertyExtractor(),
														new ActivityTracker()
													};
												});
										});
									rl.Transport.Adapter = new WcfSqlAdapter.Inbound(
										a => {
											a.Address = new SqlAdapterConnectionUri {
												InboundId = "AvailableTokens",
												InitialCatalog = "BizTalkFactoryTransientStateDb",
												Server = CommonSettings.ProcessingDatabaseHostName,
												InstanceName = CommonSettings.ProcessingDatabaseInstanceName
											};
											a.PolledDataAvailableStatement = "SELECT COUNT(1) FROM vw_claim_AvailableTokens";
											a.PollingStatement = "EXEC usp_claim_CheckOut";
											a.PollingInterval = ClaimCheckOutPollingInterval;
											a.InboundOperationType = InboundOperation.XmlPolling;
											a.XmlStoredProcedureRootNodeName = "BodyWrapper";
											a.InboundBodyLocation = InboundMessageBodySelection.UseBodyPath;
											a.InboundBodyPathExpression = "/BodyWrapper/*";
											a.InboundNodeEncoding = MessageBodyFormat.Xml;
											a.ServiceBehaviors = new[] {
												new SqlAdapterInboundTransactionBehavior {
													TransactionIsolationLevel = IsolationLevel.ReadCommitted,
													TransactionTimeout = TimeSpan.FromMinutes(2)
												}
											};
										});
									rl.Transport.Host = CommonSettings.ReceiveHost;
								}));
					})
			);
		}

		#region Base Class Member Overrides

		protected override void ApplyEnvironmentOverrides(string environment)
		{
			if (environment.IsDevelopment() || environment.IsBuild())
			{
				SendPorts.Add(
					SendPort(
						sp => {
							sp.Name = SendPortName.Towards("UnitTest.Claim").About("Redeem").FormattedAs.Xml;
							sp.State = ServiceState.Started;
							sp.SendPipeline = new SendPipeline<PassThruTransmit>(
								pipeline => {
									pipeline.PreAssembler<MicroPipelineComponent>(
										pc => {
											pc.Components = new IMicroPipelineComponent[] {
												new ActivityTracker { TrackingModes = ActivityTrackingModes.Claim }
											};
										});
								});
							sp.Transport.Adapter = new FileAdapter.Outbound(
								a => {
									a.DestinationFolder = @"C:\Files\Drops\BizTalk.Factory\Out";
									a.FileName = "Claim_%MessageID%.xml";
								});
							sp.Transport.Host = CommonSettings.TransmitHost;
							sp.Transport.RetryPolicy = RetryPolicy.RealTime;
							sp.Filter = new Filter(() => BtsProperties.MessageType == Schema<Claim.CheckOut>.MessageType);
						}),
					SendPort(
						sp => {
							sp.Name = SendPortName.Towards("UnitTest").About("Stub").FormattedAs.Xml;
							sp.State = ServiceState.Started;
							sp.SendPipeline = new SendPipeline<XmlTransmit>();
							sp.ReceivePipeline = new ReceivePipeline<XmlReceive>();
							sp.Transport.Adapter = new WcfBasicHttpAdapter.Outbound(a => { a.Address = new EndpointAddress("http://localhost:8000/stubservice"); });
							sp.Transport.Host = CommonSettings.TransmitHost;
							sp.Transport.RetryPolicy = RetryPolicy.RealTime;
						}),
					_batchSendPort = SendPort(
						sp => {
							sp.Name = SendPortName.Towards("UnitTest.Batch").About("Trace").FormattedAs.Xml;
							sp.State = ServiceState.Started;
							sp.SendPipeline = new SendPipeline<PassThruTransmit>(
								pipeline => {
									pipeline.PreAssembler<MicroPipelineComponent>(
										pc => { pc.Components = new IMicroPipelineComponent[] { new ActivityTracker() }; });
								});
							sp.Transport.Adapter = new FileAdapter.Outbound(a => { a.DestinationFolder = @"C:\Files\Drops\BizTalk.Factory\Trace"; });
							sp.Transport.Host = CommonSettings.TransmitHost;
							sp.Transport.RetryPolicy = RetryPolicy.RealTime;
							sp.Filter = new Filter(() => BtsProperties.ReceivePortName == _batchReceivePort.Name);
						})
				);

				ReceivePorts.Add(
					ReceivePort(
						rp => {
							rp.Name = ReceivePortName.Offwards("UnitTest");
							rp.ReceiveLocations.Add(
								ReceiveLocation(
									rl => {
										rl.Name = ReceiveLocationName.About("InputMessage").FormattedAs.Xml;
										rl.Enabled = true;
										rl.ReceivePipeline = new ReceivePipeline<XmlReceive>(
											pipeline => {
												pipeline.Validator<MicroPipelineComponent>(
													pc => {
														pc.Components = new IMicroPipelineComponent[] {
															new ContextPropertyExtractor(),
															new ActivityTracker()
														};
													});
											});
										rl.Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"C:\Files\Drops\BizTalk.Factory\In"; });
										rl.Transport.Host = CommonSettings.ReceiveHost;
									}),
								ReceiveLocation(
									rl => {
										rl.Name = ReceiveLocationName.About("Batch.AddPart").FormattedAs.Xml;
										rl.Enabled = true;
										rl.ReceivePipeline = new ReceivePipeline<XmlReceive>(
											pipeline => {
												pipeline.Validator<MicroPipelineComponent>(
													pc => {
														pc.Components = new IMicroPipelineComponent[] {
															new ContextPropertyExtractor {
																Extractors = new[] {
																	new XPathExtractor(BizTalkFactoryProperties.EnvelopeSpecName, "/*[local-name()='Any']/*[local-name()='EnvelopeSpecName']", ExtractionMode.Promote),
																	new XPathExtractor(BizTalkFactoryProperties.EnvelopePartition, "/*[local-name()='Any']/*[local-name()='EnvelopePartition']"),
																	new XPathExtractor(TrackingProperties.Value1, "/*[local-name()='Any']/*[local-name()='EnvelopeSpecName']"),
																	new XPathExtractor(TrackingProperties.Value2, "/*[local-name()='Any']/*[local-name()='EnvelopePartition']")
																}
															},
															new ActivityTracker()
														};
													});
											});
										rl.Transport.Adapter = new FileAdapter.Inbound(
											a => {
												a.FileMask = "*.xml.part";
												a.ReceiveFolder = @"C:\Files\Drops\BizTalk.Factory\In";
											});
										rl.Transport.Host = CommonSettings.ReceiveHost;
									}),
								ReceiveLocation(
									rl => {
										rl.Name = ReceiveLocationName.About("Claim.Desk").FormattedAs.Xml;
										rl.Enabled = true;
										rl.ReceivePipeline = new ReceivePipeline<XmlReceive>(
											pipeline => {
												pipeline.Validator<MicroPipelineComponent>(
													pc => {
														pc.Components = new IMicroPipelineComponent[] {
															new ContextPropertyExtractor {
																Extractors = new[] {
																	new XPathExtractor(BizTalkFactoryProperties.CorrelationToken, "/*[local-name()='Any']/*[local-name()='CorrelationToken']"),
																	new XPathExtractor(BizTalkFactoryProperties.OutboundTransportLocation, "/*[local-name()='Any']/*[local-name()='OutboundTransportLocation']"),
																	new XPathExtractor(BizTalkFactoryProperties.ReceiverName, "/*[local-name()='Any']/*[local-name()='ReceiverName']"),
																	new XPathExtractor(BizTalkFactoryProperties.SenderName, "/*[local-name()='Any']/*[local-name()='SenderName']"),
																	new XPathExtractor(TrackingProperties.Value1, "/*[local-name()='Any']/*[local-name()='CorrelationToken']"),
																	new XPathExtractor(TrackingProperties.Value2, "/*[local-name()='Any']/*[local-name()='ReceiverName']"),
																	new XPathExtractor(TrackingProperties.Value3, "/*[local-name()='Any']/*[local-name()='SenderName']")
																}
															},
															new ActivityTracker { TrackingModes = ActivityTrackingModes.Claim }
														};
													});
											});
										rl.Transport.Adapter = new FileAdapter.Inbound(
											a => {
												a.FileMask = "*.xml.claim";
												a.ReceiveFolder = @"C:\Files\Drops\BizTalk.Factory\In";
											});
										rl.Transport.Host = CommonSettings.ReceiveHost;
									}),
								ReceiveLocation(
									rl => {
										rl.Name = ReceiveLocationName.About("Claim.Desk").FormattedAs.None;
										rl.Enabled = true;
										rl.ReceivePipeline = new ReceivePipeline<PassThruReceive>(
											pipeline => {
												pipeline.Decoder<MicroPipelineComponent>(
													pc => {
														pc.Components = new IMicroPipelineComponent[] {
															new ActivityTracker { TrackingModes = ActivityTrackingModes.Claim }
														};
													});
											});
										rl.Transport.Adapter = new FileAdapter.Inbound(
											a => {
												a.FileMask = "*.bin.claim";
												a.ReceiveFolder = @"C:\Files\Drops\BizTalk.Factory\In";
											});
										rl.Transport.Host = CommonSettings.ReceiveHost;
									})
							);
						}));
			}
		}

		#endregion

		public ISendPort<NamingConvention> BatchAddPartSendPort
		{
			get { return _batchAddPartSendPort; }
		}

		private TimeSpan BatchReleasePollingInterval
		{
			get
			{
				return BindingGenerationContext.Instance.TargetEnvironment.IsOneOf(TargetEnvironment.DEVELOPMENT, TargetEnvironment.BUILD)
					? TimeSpan.FromSeconds(5)
					: TimeSpan.FromMinutes(15);
			}
		}

		private TimeSpan ClaimCheckOutPollingInterval
		{
			get
			{
				return BindingGenerationContext.Instance.TargetEnvironment.IsOneOf(TargetEnvironment.DEVELOPMENT, TargetEnvironment.BUILD)
					? TimeSpan.FromSeconds(5)
					: TimeSpan.FromMinutes(5);
			}
		}

		protected readonly IReceivePort<NamingConvention> _batchReceivePort;
		private readonly ISendPort<NamingConvention> _batchAddPartSendPort;
		protected ISendPort<NamingConvention> _batchSendPort;
	}
}
