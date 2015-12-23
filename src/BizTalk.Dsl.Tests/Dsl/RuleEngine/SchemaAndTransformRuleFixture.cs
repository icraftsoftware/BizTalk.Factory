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

using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Unit.RuleEngine;
using BTF2Schemas;
using Microsoft.XLANGs.BaseTypes;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.RuleEngine
{
	[TestFixture]
	public class SchemaAndTransformRuleFixture : PolicyFixture<SchemaAndTransformRuleFixture.WriteFromContextRuleSet>
	{
		[Test]
		public void MessageTypeResolution()
		{
			Facts.Assert(Context.Property(BtsProperties.MessageType).WithValue(Schema<btf2_services_header>.MessageType));

			ExecutePolicy();

			Facts
				.Verify(Context.Property(BizTalkFactoryProperties.CorrelationToken).WithValue(Schema<btf2_envelope>.MessageType).HasBeenWritten())
				.Verify(Context.Property(BizTalkFactoryProperties.EnvelopeSpecName).WithValue(new SchemaMetadata<btf2_envelope>().DocumentSpec.DocSpecStrongName).HasBeenWritten())
				.Verify(Context.Property(BizTalkFactoryProperties.EnvelopeSpecName).WithValue(Schema<btf2_envelope>.AssemblyQualifiedName).HasBeenWritten());
		}

		[Test]
		public void TransformTypeResolution()
		{
			Facts
				.Assert(Context.Property(BtsProperties.MessageType).WithValue("Z_IDOC#SIX"))
				.Assert(Context.Property(BtsProperties.InboundTransportType).WithValue("WCF-SAP"));

			ExecutePolicy();

			Facts
				.Verify(Context.Property(TrackingProperties.ProcessName).WithValue(Dummy.Processes.One).HasBeenWritten())
				.Verify(Context.Property(BizTalkFactoryProperties.MapTypeName).WithValue(Transform<TransformBase>.MapTypeName).HasBeenWritten());
		}

		public class WriteFromContextRuleSet : RuleSet
		{
			public WriteFromContextRuleSet()
			{
				Name = GetType().Name;

				Rules.Add(
					Rule("MessageType")
						.If(
							() => Context.Read(BtsProperties.MessageType) == Schema<btf2_endpoints_header>.MessageType
								|| Context.Read(BtsProperties.MessageType) == Schema<btf2_services_header>.MessageType
						)
						.Then(
							() => Context.Write(BizTalkFactoryProperties.CorrelationToken, Schema<btf2_envelope>.MessageType),
							() => Context.Write(BizTalkFactoryProperties.EnvelopeSpecName, Schema<btf2_envelope>.AssemblyQualifiedName)
						)
					);

				Rules.Add(
					Rule("TransformType")
						.If(
							() => Context.Read(BtsProperties.MessageType) == "Z_IDOC#SIX" && Context.Read(BtsProperties.InboundTransportType) == "WCF-SAP"
						)
						.Then(
							() => Context.Write(TrackingProperties.ProcessName, Dummy.Processes.One),
							() => Context.Write(BizTalkFactoryProperties.MapTypeName, Transform<TransformBase>.MapTypeName)
						)
					);
			}
		}
	}
}
