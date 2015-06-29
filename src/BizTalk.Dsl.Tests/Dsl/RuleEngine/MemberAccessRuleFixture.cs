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

using System;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Unit.RuleEngine;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.RuleEngine
{
	[TestFixture]
	public class MemberAccessRuleFixture : PolicyFixture<MemberAccessRuleFixture.MemberAccessRuleSet>
	{
		[Test]
		public void ExecuteConstantToLambdaToExpressionRule()
		{
			Facts.Assert(Context.Property(FileProperties.ReceivedFileName).WithValue("no-folder-file.csv"));

			ExecutePolicy();

			Facts.Verify(Context.Property(BizTalkFactoryProperties.OutboundTransportLocation).WithValue("folder/no-folder-file.csv").HasBeenWritten());
		}

		[Test]
		public void ExecutePropertyMember()
		{
			Facts.Assert(Context.Property(FileProperties.ReceivedFileName).WithValue("token.csv"));

			ExecutePolicy();

			Facts.Verify(Context.Property(BizTalkFactoryProperties.CorrelationToken).WithValue("computed value!").HasBeenWritten());
		}

		public class MemberAccessRuleSet : RuleSet
		{
			public MemberAccessRuleSet()
			{
				Name = GetType().Name;

				Rules.Add(
					Rule("Constant.ToLambda.ToExpression")
						.If(() => Regex.IsMatch(Context.Read(FileProperties.ReceivedFileName), "no-folder"))
						.Then(_setOutboundTransportLocation("folder/"))
					);

				Rules.Add(
					Rule("ConstantEquivalentPropertyMember")
						.If(() => Regex.IsMatch(Context.Read(FileProperties.ReceivedFileName), "token"))
						.Then(() => Context.Write(BizTalkFactoryProperties.CorrelationToken, Token))
					);
			}

			private string Token
			{
				get
				{
					var builder = new StringBuilder();
					builder.Append("computed ");
					builder.Append("value!");
					return builder.ToString();
				}
			}

			private static readonly Func<string, Expression<Action>> _setOutboundTransportLocation = path => () => Context.Write(
				BizTalkFactoryProperties.OutboundTransportLocation,
				Path.Combine(path, Path.GetFileName(Context.Read(FileProperties.ReceivedFileName))));
		}
	}
}
