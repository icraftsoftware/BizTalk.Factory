#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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

using BTF2Schemas;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Unit.RuleEngine;
using Microsoft.XLANGs.BaseTypes;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.RuleEngine.Dsl
{
	[TestFixture]
	public class SchemaAndTransformRuleFixture : PolicyFixture
	{
		[Test]
		public void MessageTypeResolution()
		{
			RuleEngine.Facts
				.Assert(Context.Property(BtsProperties.MessageType).WithValue(Schema<btf2_services_header>.MessageType));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(RoutingProperties.CorrelationToken).WithValue(Schema<btf2_envelope>.MessageType).HasBeenWritten());
		}

		[Test]
		public void TransformTypeResolution()
		{
			RuleEngine.Facts
				.Assert(Context.Property(BtsProperties.MessageType).WithValue("Z_IDOC#SIX"))
				.Assert(Context.Property(BtsProperties.InboundTransportType).WithValue("WCF-SAP"));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(ResolvedProperties.ProcessName).WithValue(TestProcesses.One).HasBeenWritten())
				.Verify(Context.Property(ResolvedProperties.MapTypeName).WithValue(Transform<TransformBase>.MapTypeName).HasBeenWritten());
		}

		public RuleSet RuleSet
		{
			get { return _ruleset ?? (_ruleset = new WriteFromContextRuleSet()); }
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
							() => Context.Write(RoutingProperties.CorrelationToken, Schema<btf2_envelope>.MessageType)
						)
					);

				Rules.Add(
					Rule("TransformType")
						.If(
							() => Context.Read(BtsProperties.MessageType) == "Z_IDOC#SIX" && Context.Read(BtsProperties.InboundTransportType) == "WCF-SAP"
						)
						.Then(
							() => Context.Write(ResolvedProperties.ProcessName, TestProcesses.One),
							() => Context.Write(ResolvedProperties.MapTypeName, Transform<TransformBase>.MapTypeName)
						)
					);
			}
		}

		private RuleSet _ruleset;
	}
}