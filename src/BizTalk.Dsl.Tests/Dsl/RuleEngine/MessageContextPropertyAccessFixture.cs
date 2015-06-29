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

using System.Text.RegularExpressions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Unit.RuleEngine;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.RuleEngine
{
	[TestFixture]
	public class MessageContextPropertyAccessFixture : PolicyFixture<MessageContextPropertyAccessFixture.MessageContextPropertyAccessRuleSet>
	{
		[Test]
		public void DirectAccess()
		{
			Facts.Assert(Context.Property(FileProperties.ReceivedFileName).WithValue("some-directAccess-file.csv"));

			ExecutePolicy();

			Facts.Verify(Context.Property(BizTalkFactoryProperties.SenderName).WithValue("directValue").HasBeenPromoted());
		}

		[Test]
		public void IndirectAccess()
		{
			Facts.Assert(Context.Property(FileProperties.ReceivedFileName).WithValue("some-accessViaVariable-file.csv"));

			ExecutePolicy();

			Facts.Verify(Context.Property(BizTalkFactoryProperties.SenderName).WithValue("variableValue").HasBeenPromoted());
		}

		public abstract class PassThruResolver : RuleSet { }

		public class MessageContextPropertyAccessRuleSet : PassThruResolver
		{
			public MessageContextPropertyAccessRuleSet()
			{
				var matchingProperty = FileProperties.ReceivedFileName;
				var routingProperty = BizTalkFactoryProperties.SenderName;

				Rules
					.Add(
						Rule("MessageContextPropertyDirectAccess")
							.If(() => Regex.IsMatch(Context.Read(FileProperties.ReceivedFileName), "directAccess"))
							.Then(() => Context.Promote(BizTalkFactoryProperties.SenderName, "directValue"))
					)
					.Add(
						Rule("MessageContextPropertyAccessViaVariable")
							.If(() => Regex.IsMatch(Context.Read(matchingProperty), "accessViaVariable"))
							.Then(() => Context.Promote(routingProperty, "variableValue"))
					);
			}
		}
	}
}
