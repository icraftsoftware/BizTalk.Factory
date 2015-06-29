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

using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.Dsl.Pipeline;
using Be.Stateless.BizTalk.Policies.Send.Claim;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Tracking;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Utilities;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public class PipelineBindingSerializerFixture
	{
		[Test]
		public void ReceivePipelineDslGrammarVariant1()
		{
			// not fluent-DSL
			var pipeline = new ReceivePipeline<Pipelines.XmlReceive>();
			pipeline.Stages.Decode.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
			};
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().RecoverableInterchangeProcessing = true;
			pipeline.Stages.ResolveParty.Component<PolicyRunnerComponent>().Policy = Policy<ProcessResolver>.Name;
			var binding = ((IPipelineSerializerFactory) pipeline).GetPipelineBindingSerializer().Serialize();

			// fluent-DSL first variant
			var pipeline1 = new ReceivePipeline<Pipelines.XmlReceive>(
				pl => {
					pl.Stages.Decode.Component<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; });
					pl.Stages.Disassemble.Component<XmlDasmComp>(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
							};
							c.RecoverableInterchangeProcessing = true;
						});
					pl.Stages.ResolveParty.Component<PolicyRunnerComponent>(c => { c.Policy = Policy<ProcessResolver>.Name; });
				});
			var binding1 = ((IPipelineSerializerFactory) pipeline1).GetPipelineBindingSerializer().Serialize();

			Assert.That(binding1, Is.EqualTo(binding));
		}

		[Test]
		public void ReceivePipelineDslGrammarVariant2()
		{
			// not fluent-DSL
			var pipeline = new ReceivePipeline<Pipelines.XmlReceive>();
			pipeline.Stages.Decode.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
			};
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().RecoverableInterchangeProcessing = true;
			pipeline.Stages.ResolveParty.Component<PolicyRunnerComponent>().Policy = Policy<ProcessResolver>.Name;
			var binding = ((IPipelineSerializerFactory) pipeline).GetPipelineBindingSerializer().Serialize();

			// fluent-DSL second variant
			var pipeline2 = ReceivePipeline<Pipelines.XmlReceive>.Configure(
				pl => pl
					.Decoder<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; })
					.Disassembler<XmlDasmComp>(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
							};
							c.RecoverableInterchangeProcessing = true;
						})
					.PartyResolver<PolicyRunnerComponent>(c => { c.Policy = Policy<ProcessResolver>.Name; }));
			var binding2 = ((IPipelineSerializerFactory) pipeline2).GetPipelineBindingSerializer().Serialize();

			Assert.That(binding2, Is.EqualTo(binding));
		}

		[Test]
		public void ReceivePipelineDslGrammarVariant3()
		{
			// not fluent-DSL
			var pipeline = new ReceivePipeline<Pipelines.XmlReceive>();
			pipeline.Stages.Decode.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
			};
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().RecoverableInterchangeProcessing = true;
			pipeline.Stages.ResolveParty.Component<PolicyRunnerComponent>().Policy = Policy<ProcessResolver>.Name;
			var binding = ((IPipelineSerializerFactory) pipeline).GetPipelineBindingSerializer().Serialize();

			// fluent-DSL third variant
			var pipeline3 = ReceivePipeline<Pipelines.XmlReceive>.Configure(
				pl => pl
					.FirstDecoder<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; })
					.FirstDisassembler<XmlDasmComp>(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
							};
							c.RecoverableInterchangeProcessing = true;
						})
					.FirstPartyResolver<PolicyRunnerComponent>(c => { c.Policy = Policy<ProcessResolver>.Name; }));
			var binding3 = ((IPipelineSerializerFactory) pipeline3).GetPipelineBindingSerializer().Serialize();

			Assert.That(binding3, Is.EqualTo(binding));
		}

		[Test]
		public void ReceivePipelineDslGrammarVariant4()
		{
			// not fluent-DSL
			var pipeline = new ReceivePipeline<Pipelines.XmlReceive>();
			pipeline.Stages.Decode.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.Decode.Component<ActivityTrackerComponent>().TrackingModes = ActivityTrackingModes.Archive | ActivityTrackingModes.Claim;
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
			};
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().RecoverableInterchangeProcessing = true;
			pipeline.Stages.ResolveParty.Component<PolicyRunnerComponent>().Policy = Policy<ProcessResolver>.Name;
			var binding = ((IPipelineSerializerFactory) pipeline).GetPipelineBindingSerializer().Serialize();

			// fluent-DSL fourth variant
			var pipeline4 = ReceivePipeline<Pipelines.XmlReceive>.Configure(
				pl => {
					pl.Stages.Decode.Components
						.ComponentAt<FailedMessageRoutingEnablerComponent>(0).Configure(c => { c.Enabled = false; })
						.ComponentAt<ActivityTrackerComponent>(4).Configure(c => { c.TrackingModes = ActivityTrackingModes.Archive | ActivityTrackingModes.Claim; });
					pl.Stages.Disassemble.Components.ComponentAt<XmlDasmComp>(0).Configure(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
							};
							c.RecoverableInterchangeProcessing = true;
						});
					pl.Stages.ResolveParty.Components.ComponentAt<PolicyRunnerComponent>(1).Configure(c => { c.Policy = Policy<ProcessResolver>.Name; });
				});
			var binding4 = ((IPipelineSerializerFactory) pipeline4).GetPipelineBindingSerializer().Serialize();

			Assert.That(binding4, Is.EqualTo(binding));
		}

		[Test]
		public void ReceivePipelineDslGrammarVariant5()
		{
			// not fluent-DSL
			var pipeline = new ReceivePipeline<Pipelines.XmlReceive>();
			pipeline.Stages.Decode.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.Decode.Component<ActivityTrackerComponent>().TrackingModes = ActivityTrackingModes.Archive | ActivityTrackingModes.Claim;
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
			};
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().RecoverableInterchangeProcessing = true;
			pipeline.Stages.ResolveParty.Component<PolicyRunnerComponent>().Policy = Policy<ProcessResolver>.Name;
			var binding = ((IPipelineSerializerFactory) pipeline).GetPipelineBindingSerializer().Serialize();

			// fluent-DSL fifth variant
			var pipeline5 = ReceivePipeline<Pipelines.XmlReceive>.Configure(
				pl => {
					pl.Stages.Decode
						.ComponentAt<FailedMessageRoutingEnablerComponent>(0).Configure(c => { c.Enabled = false; })
						.ComponentAt<ActivityTrackerComponent>(4).Configure(c => { c.TrackingModes = ActivityTrackingModes.Archive | ActivityTrackingModes.Claim; });
					pl.Stages.Disassemble.ComponentAt<XmlDasmComp>(0).Configure(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
							};
							c.RecoverableInterchangeProcessing = true;
						});
					pl.Stages.ResolveParty.ComponentAt<PolicyRunnerComponent>(1).Configure(c => { c.Policy = Policy<ProcessResolver>.Name; });
				});
			var binding5 = ((IPipelineSerializerFactory) pipeline5).GetPipelineBindingSerializer().Serialize();

			Assert.That(binding5, Is.EqualTo(binding));
		}

		[Test]
		public void ReceivePipelineDslGrammarVariant6()
		{
			// not fluent-DSL
			var pipeline = new ReceivePipeline<Pipelines.XmlReceive>();
			pipeline.Stages.Decode.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.Decode.Component<ActivityTrackerComponent>().TrackingModes = ActivityTrackingModes.Archive | ActivityTrackingModes.Claim;
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
			};
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().RecoverableInterchangeProcessing = true;
			pipeline.Stages.ResolveParty.Component<PolicyRunnerComponent>().Policy = Policy<ProcessResolver>.Name;
			var binding = ((IPipelineSerializerFactory) pipeline).GetPipelineBindingSerializer().Serialize();

			// fluent-DSL sixth variant
			var pipeline6 = ReceivePipeline<Pipelines.XmlReceive>.Configure(
				pl => pl
					.DecoderAt<FailedMessageRoutingEnablerComponent>(0).Configure(c => { c.Enabled = false; })
					.DecoderAt<ActivityTrackerComponent>(4).Configure(c => { c.TrackingModes = ActivityTrackingModes.Archive | ActivityTrackingModes.Claim; })
					.DisassemblerAt<XmlDasmComp>(0).Configure(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
							};
							c.RecoverableInterchangeProcessing = true;
						})
					.PartyResolverAt<PolicyRunnerComponent>(1).Configure(c => { c.Policy = Policy<ProcessResolver>.Name; }));
			var binding6 = ((IPipelineSerializerFactory) pipeline6).GetPipelineBindingSerializer().Serialize();

			Assert.That(binding6, Is.EqualTo(binding));
		}

		[Test]
		public void ReceivePipelineSerializeIsEmptyWhenDefaultPipelineConfigIsNotOverridden()
		{
			var pipeline = new ReceivePipeline<Microsoft.BizTalk.DefaultPipelines.XMLReceive>();
			var pipelineBindingSerializer = ((IPipelineSerializerFactory) pipeline).GetPipelineBindingSerializer();

			var binding = pipelineBindingSerializer.Serialize();

			Assert.That(binding, Is.Empty);
		}

		[Test]
		public void ReceivePipelineSerializeKeepsOnlyStagesWhoseComponentsDefaultConfigHasBeenOverridden()
		{
			var pipeline = new ReceivePipeline<Microsoft.BizTalk.DefaultPipelines.XMLReceive>(
				pl => pl.Disassembler<XmlDasmComp>(
					c => { c.RecoverableInterchangeProcessing = true; }));
			var pipelineBindingSerializer = ((IPipelineSerializerFactory) pipeline).GetPipelineBindingSerializer();

			var binding = pipelineBindingSerializer.Serialize();

			Assert.That(
				binding,
				Is.EqualTo(
					"<Root><Stages>" +
						"<Stage CategoryId=\"9d0e4105-4cce-4536-83fa-4a5040674ad6\">" +
						"<Components>" +
						"<Component Name=\"Microsoft.BizTalk.Component.XmlDasmComp\">" +
						"<Properties><RecoverableInterchangeProcessing vt=\"11\">-1</RecoverableInterchangeProcessing></Properties>" +
						"</Component>" +
						"</Components>" +
						"</Stage>" +
						"</Stages></Root>"));
		}

		[Test]
		public void ReceivePipelineSerializeKeepsOnlyStagesWhoseComponentsDefaultConfigHasBeenOverridden2()
		{
			var pipeline = ReceivePipeline<Pipelines.XmlReceive>.Configure(
				pl => pl
					.FirstDecoder<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; })
					.FirstDecoder<ActivityTrackerComponent>(
						c => {
							c.Enabled = true;
							c.TrackingModes = ActivityTrackingModes.Archive | ActivityTrackingModes.Claim;
						}));

			var binding = ((IPipelineSerializerFactory) pipeline).GetPipelineBindingSerializer().Serialize();

			Assert.That(
				binding,
				Is.EqualTo(
					"<Root><Stages>" +
						"<Stage CategoryId=\"9d0e4103-4cce-4536-83fa-4a5040674ad6\">" +
						"<Components>" +
						"<Component Name=\"Be.Stateless.BizTalk.Component.FailedMessageRoutingEnablerComponent\">" +
						"<Properties><Enabled vt=\"11\">0</Enabled></Properties>" +
						"</Component>" +
						"<Component Name=\"Be.Stateless.BizTalk.Component.XmlTranslatorComponent\">" +
						"<Properties />" +
						"</Component>" +
						"<Component Name=\"Be.Stateless.BizTalk.Component.ContextPropertyExtractorComponent\">" +
						"<Properties />" +
						"</Component><Component Name=\"Be.Stateless.BizTalk.Component.PolicyRunnerComponent\">" +
						"<Properties />" +
						"</Component>" +
						"<Component Name=\"Be.Stateless.BizTalk.Component.ActivityTrackerComponent\">" +
						"<Properties><Enabled vt=\"11\">-1</Enabled><TrackingModes vt=\"8\">Claim, Archive</TrackingModes></Properties>" +
						"</Component>" +
						"<Component Name=\"Be.Stateless.BizTalk.Component.XsltRunnerComponent\">" +
						"<Properties />" +
						"</Component>" +
						"</Components>" +
						"</Stage>" +
						"</Stages></Root>"));
		}

		[Test]
		public void SendPipelineDslGrammarVariant1()
		{
			// not fluent-DSL
			var pipeline = new SendPipeline<Pipelines.XmlTransmit>();
			pipeline.Stages.PreAssemble.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.Assemble.Component<XmlAsmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
			};
			pipeline.Stages.Assemble.Component<XmlAsmComp>().AddXMLDeclaration = true;
			pipeline.Stages.Encode.Component<PolicyRunnerComponent>().Policy = Policy<ProcessResolver>.Name;
			var binding = ((IPipelineSerializerFactory) pipeline).GetPipelineBindingSerializer().Serialize();

			// fluent-DSL first variant
			var pipeline1 = new SendPipeline<Pipelines.XmlTransmit>(
				pl => {
					pl.Stages.PreAssemble.Component<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; });
					pl.Stages.Assemble.Component<XmlAsmComp>(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
							};
							c.AddXMLDeclaration = true;
						});
					pl.Stages.Encode.Component<PolicyRunnerComponent>(c => { c.Policy = Policy<ProcessResolver>.Name; });
				});
			var binding1 = ((IPipelineSerializerFactory) pipeline1).GetPipelineBindingSerializer().Serialize();

			Assert.That(binding1, Is.EqualTo(binding));
		}

		[Test]
		public void SendPipelineDslGrammarVariant2()
		{
			// not fluent-DSL
			var pipeline = new SendPipeline<Pipelines.XmlTransmit>();
			pipeline.Stages.PreAssemble.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.Assemble.Component<XmlAsmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
			};
			pipeline.Stages.Assemble.Component<XmlAsmComp>().AddXMLDeclaration = true;
			pipeline.Stages.Encode.Component<PolicyRunnerComponent>().Policy = Policy<ProcessResolver>.Name;
			var binding = ((IPipelineSerializerFactory) pipeline).GetPipelineBindingSerializer().Serialize();

			// fluent-DSL second variant
			var pipeline2 = new SendPipeline<Pipelines.XmlTransmit>(
				pl => {
					pl.PreAssembler<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; });
					pl.Assembler<XmlAsmComp>(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
							};
							c.AddXMLDeclaration = true;
						});
					pl.Encoder<PolicyRunnerComponent>(c => { c.Policy = Policy<ProcessResolver>.Name; });
				});
			var binding2 = ((IPipelineSerializerFactory) pipeline2).GetPipelineBindingSerializer().Serialize();

			Assert.That(binding2, Is.EqualTo(binding));
		}

		[Test]
		public void SendPipelineDslGrammarVariant3()
		{
			// not fluent-DSL
			var pipeline = new SendPipeline<Pipelines.XmlTransmit>();
			pipeline.Stages.PreAssemble.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.Assemble.Component<XmlAsmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
			};
			pipeline.Stages.Assemble.Component<XmlAsmComp>().AddXMLDeclaration = true;
			pipeline.Stages.Encode.Component<PolicyRunnerComponent>().Policy = Policy<ProcessResolver>.Name;
			var binding = ((IPipelineSerializerFactory) pipeline).GetPipelineBindingSerializer().Serialize();

			// fluent-DSL third variant
			var pipeline3 = new SendPipeline<Pipelines.XmlTransmit>(
				pl => {
					pl.FirstPreAssembler<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; });
					pl.FirstAssembler<XmlAsmComp>(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
							};
							c.AddXMLDeclaration = true;
						});
					pl.FirstEncoder<PolicyRunnerComponent>(c => { c.Policy = Policy<ProcessResolver>.Name; });
				});
			var binding3 = ((IPipelineSerializerFactory) pipeline3).GetPipelineBindingSerializer().Serialize();

			Assert.That(binding3, Is.EqualTo(binding));
		}

		[Test]
		public void SendPipelineDslGrammarVariant4()
		{
			// not fluent-DSL
			var pipeline = new SendPipeline<Pipelines.XmlTransmit>();
			pipeline.Stages.PreAssemble.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.PreAssemble.Component<ActivityTrackerComponent>().TrackingModes = ActivityTrackingModes.Archive | ActivityTrackingModes.Claim;
			pipeline.Stages.Assemble.Component<XmlAsmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
			};
			pipeline.Stages.Assemble.Component<XmlAsmComp>().AddXMLDeclaration = true;
			pipeline.Stages.Encode.Component<PolicyRunnerComponent>().Policy = Policy<ProcessResolver>.Name;
			var binding = ((IPipelineSerializerFactory) pipeline).GetPipelineBindingSerializer().Serialize();

			// fluent-DSL fourth variant
			var pipeline4 = new SendPipeline<Pipelines.XmlTransmit>(
				pl => {
					pl.Stages.PreAssemble.Components
						.ComponentAt<FailedMessageRoutingEnablerComponent>(0).Configure(c => { c.Enabled = false; })
						.ComponentAt<ActivityTrackerComponent>(3).Configure(c => { c.TrackingModes = ActivityTrackingModes.Archive | ActivityTrackingModes.Claim; });
					pl.Stages.Assemble.Components.ComponentAt<XmlAsmComp>(0).Configure(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
							};
							c.AddXMLDeclaration = true;
						});
					pl.Stages.Encode.Components.ComponentAt<PolicyRunnerComponent>(1).Configure(c => { c.Policy = Policy<ProcessResolver>.Name; });
				});
			var binding4 = ((IPipelineSerializerFactory) pipeline4).GetPipelineBindingSerializer().Serialize();

			Assert.That(binding4, Is.EqualTo(binding));
		}

		[Test]
		public void SendPipelineDslGrammarVariant5()
		{
			// not fluent-DSL
			var pipeline = new SendPipeline<Pipelines.XmlTransmit>();
			pipeline.Stages.PreAssemble.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.PreAssemble.Component<ActivityTrackerComponent>().TrackingModes = ActivityTrackingModes.Archive | ActivityTrackingModes.Claim;
			pipeline.Stages.Assemble.Component<XmlAsmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
			};
			pipeline.Stages.Assemble.Component<XmlAsmComp>().AddXMLDeclaration = true;
			pipeline.Stages.Encode.Component<PolicyRunnerComponent>().Policy = Policy<ProcessResolver>.Name;
			var binding = ((IPipelineSerializerFactory) pipeline).GetPipelineBindingSerializer().Serialize();

			// fluent-DSL fifth variant
			var pipeline5 = new SendPipeline<Pipelines.XmlTransmit>(
				pl => {
					pl.Stages.PreAssemble
						.ComponentAt<FailedMessageRoutingEnablerComponent>(0).Configure(c => { c.Enabled = false; })
						.ComponentAt<ActivityTrackerComponent>(3).Configure(c => { c.TrackingModes = ActivityTrackingModes.Archive | ActivityTrackingModes.Claim; });
					pl.Stages.Assemble.ComponentAt<XmlAsmComp>(0).Configure(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
							};
							c.AddXMLDeclaration = true;
						});
					pl.Stages.Encode.ComponentAt<PolicyRunnerComponent>(1).Configure(c => { c.Policy = Policy<ProcessResolver>.Name; });
				});
			var binding5 = ((IPipelineSerializerFactory) pipeline5).GetPipelineBindingSerializer().Serialize();

			Assert.That(binding5, Is.EqualTo(binding));
		}

		[Test]
		public void SendPipelineDslGrammarVariant6()
		{
			// not fluent-DSL
			var pipeline = new SendPipeline<Pipelines.XmlTransmit>();
			pipeline.Stages.PreAssemble.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.PreAssemble.Component<ActivityTrackerComponent>().TrackingModes = ActivityTrackingModes.Archive | ActivityTrackingModes.Claim;
			pipeline.Stages.Assemble.Component<XmlAsmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
			};
			pipeline.Stages.Assemble.Component<XmlAsmComp>().AddXMLDeclaration = true;
			pipeline.Stages.Encode.Component<PolicyRunnerComponent>().Policy = Policy<ProcessResolver>.Name;
			var binding = ((IPipelineSerializerFactory) pipeline).GetPipelineBindingSerializer().Serialize();

			// fluent-DSL sixth variant
			var pipeline6 = new SendPipeline<Pipelines.XmlTransmit>(
				pl => pl
					.PreAssemblerAt<FailedMessageRoutingEnablerComponent>(0).Configure(c => { c.Enabled = false; })
					.PreAssemblerAt<ActivityTrackerComponent>(3).Configure(c => { c.TrackingModes = ActivityTrackingModes.Archive | ActivityTrackingModes.Claim; })
					.AssemblerAt<XmlAsmComp>(0).Configure(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<Claim.CheckIn>.AssemblyQualifiedName)
							};
							c.AddXMLDeclaration = true;
						})
					.EncoderAt<PolicyRunnerComponent>(1).Configure(c => { c.Policy = Policy<ProcessResolver>.Name; }));
			var binding6 = ((IPipelineSerializerFactory) pipeline6).GetPipelineBindingSerializer().Serialize();

			Assert.That(binding6, Is.EqualTo(binding));
		}

		[Test]
		public void SendPipelineSerializeKeepsOnlyStagesWhoseComponentsDefaultConfigHasBeenOverridden2()
		{
			var pipeline = SendPipeline<Pipelines.XmlTransmit>.Configure(
				pl => pl
					.FirstPreAssembler<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; })
					.FirstPreAssembler<ActivityTrackerComponent>(
						c => {
							c.Enabled = true;
							c.TrackingModes = ActivityTrackingModes.Archive | ActivityTrackingModes.Claim;
						}));

			var binding = ((IPipelineSerializerFactory) pipeline).GetPipelineBindingSerializer().Serialize();

			Assert.That(
				binding,
				Is.EqualTo(
					"<Root><Stages>" +
						"<Stage CategoryId=\"9d0e4101-4cce-4536-83fa-4a5040674ad6\">" +
						"<Components>" +
						"<Component Name=\"Be.Stateless.BizTalk.Component.FailedMessageRoutingEnablerComponent\">" +
						"<Properties><Enabled vt=\"11\">0</Enabled></Properties>" +
						"</Component>" +
						"<Component Name=\"Be.Stateless.BizTalk.Component.ContextPropertyExtractorComponent\">" +
						"<Properties />" +
						"</Component><Component Name=\"Be.Stateless.BizTalk.Component.PolicyRunnerComponent\">" +
						"<Properties />" +
						"</Component>" +
						"<Component Name=\"Be.Stateless.BizTalk.Component.ActivityTrackerComponent\">" +
						"<Properties><Enabled vt=\"11\">-1</Enabled><TrackingModes vt=\"8\">Claim, Archive</TrackingModes></Properties>" +
						"</Component>" +
						"<Component Name=\"Be.Stateless.BizTalk.Component.XsltRunnerComponent\">" +
						"<Properties />" +
						"</Component>" +
						"</Components>" +
						"</Stage>" +
						"</Stages></Root>"));
		}
	}
}
