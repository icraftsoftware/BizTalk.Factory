#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
using Be.Stateless.BizTalk.Unit.RuleEngine;
using BTF2Schemas;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.RuleEngine
{
	[TestFixture]
	public class TypeMemberAccessRuleFixture : PolicyFixture<TypeMemberAccessRuleFixture.TypeMemberAccessRuleSet>
	{
		[Test]
		public void TypeMemberAccessRule()
		{
			Facts.Assert(Context.Property(BtsProperties.MessageType).WithValue(Schema<btf2_envelope>.MessageType));

			ExecutePolicy();

			Facts
				.Verify(Context.Property(BizTalkFactoryProperties.ReceiverName).WithValue("ControlBasedBatcher").HasBeenPromoted())
				.Verify(Context.Property(BizTalkFactoryProperties.EnvelopeSpecName).WithValue(typeof(btf2_envelope).AssemblyQualifiedName).HasBeenPromoted());
		}

		public class TypeMemberAccessRuleSet : RuleSet
		{
			public TypeMemberAccessRuleSet()
			{
				Rules.Add(
					Rule("TypeMemberAccess")
						.If(
							() => Context.Read(BtsProperties.MessageType) == Schema<btf2_envelope>.MessageType
						)
						.Then(
							() => Context.Promote(BizTalkFactoryProperties.ReceiverName, "ControlBasedBatcher"),
							() => Context.Promote(BizTalkFactoryProperties.EnvelopeSpecName, typeof(btf2_envelope).AssemblyQualifiedName)
						)
					);
			}
		}
	}
}
