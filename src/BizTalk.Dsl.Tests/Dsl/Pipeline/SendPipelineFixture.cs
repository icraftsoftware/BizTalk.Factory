#region Copyright & License

// Copyright © 2012 - 2014 François Chabot, Yves Dierick
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
using Be.Stateless.BizTalk.RuleEngine;
using Be.Stateless.BizTalk.Tracking;
using Microsoft.BizTalk.Component;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Pipeline
{
	[TestFixture]
	public class SendPipelineFixture
	{
		[Test]
		public void SendPipelineDslGrammarVarianceEquivalence()
		{
			var pipelineDocument1 = ((IPipelineSerializerFactory) new TrackingXmlTransmitVariant1()).GetPipelineDesignerDocumentSerializer();
			var pipelineDocument2 = ((IPipelineSerializerFactory) new TrackingXmlTransmitVariant2()).GetPipelineDesignerDocumentSerializer();

			Assert.That(pipelineDocument1.Serialize(), Is.EqualTo(pipelineDocument2.Serialize()));
		}

		[Test]
		public void SerializeThrowsWhenComponentNotFound()
		{
			Assert.That(
				() => new TrackingXmlTransmitVariant1().SecondAssembler<XmlDasmComp>(null),
				Throws.InstanceOf<ArgumentOutOfRangeException>());
		}

		private class TrackingXmlTransmitVariant1 : SendPipeline
		{
			public TrackingXmlTransmitVariant1()
			{
				Description = "XML send pipeline with tracking.";
				Version = new Version(1, 0);
				Stages.PreAssemble
					.AddComponent(
						new FailedMessageRoutingEnablerComponent {
							SuppressRoutingFailureReport = false
						})
					.AddComponent(
						new ActivityTrackerComponent {
							TrackingResolutionPolicy = new PolicyName("ResolutionPolicyName", 2, 2),
							TrackingModes = ActivityTrackingModes.Context
						});
				Stages.Assemble
					.AddComponent(new XmlAsmComp());
			}
		}

		private class TrackingXmlTransmitVariant2 : SendPipeline
		{
			public TrackingXmlTransmitVariant2()
			{
				Description = "XML send pipeline with tracking.";
				Version = new Version(1, 0);
				PreAssemblers
					.Add(
						new FailedMessageRoutingEnablerComponent {
							SuppressRoutingFailureReport = false
						})
					.Add(
						new ActivityTrackerComponent {
							TrackingResolutionPolicy = new PolicyName("ResolutionPolicyName", 2, 2),
							TrackingModes = ActivityTrackingModes.Context
						});
				Assemblers
					.Add(new XmlAsmComp());
			}
		}
	}
}
