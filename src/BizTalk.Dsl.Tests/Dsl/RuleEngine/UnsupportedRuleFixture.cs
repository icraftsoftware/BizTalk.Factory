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
using Be.Stateless.BizTalk.ContextProperties;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.RuleEngine
{
	[TestFixture]
	public class UnsupportedRuleFixture
	{
		[Test]
		public void ExpressionWithNonStaticRootRuleDoesNotTranslate()
		{
			Assert.That(
				() => new ExpressionWithNonStaticRootRuleSet().ToBrl(),
				Throws.InstanceOf<NotSupportedException>().With.Message.EqualTo(
					"Cannot translate MethodCallExpression \"\"xxx\".IndexOf(Read(BtsProperties.ReceivePortName), OrdinalIgnoreCase)\" because " +
						"\"xxx\" is neither null nor a static: writing rules against a specific object/instance is not supported.")
				);
		}

		[Test]
		public void InstanceMethodAccessRuleDoesNotTranslate()
		{
			Assert.That(
				() => new InstanceMethodAccessRuleSet().ToBrl(),
				Throws.InstanceOf<NotSupportedException>().With.Message.EqualTo(
					string.Format(
						"Cannot translate MethodCallExpression \"value({0}).GetToken()\" because value({0}) is neither null nor a static: " +
							"writing rules against a specific object/instance is not supported.",
						typeof(InstanceMethodAccessRuleSet).FullName))
				);
		}

		[Test]
		public void MemberAccessRuleDoesNotTranslate()
		{
			Assert.That(
				() => new MemberAccessRuleSet().ToBrl(),
				Throws.InstanceOf<NotSupportedException>().With.Message.EqualTo(
					"Cannot translate MethodCallExpression \"Read(BtsProperties.ReceivePortName).IndexOf(\"yyy\", OrdinalIgnoreCase)\" because " +
						"Read(BtsProperties.ReceivePortName) is neither null nor a static: writing rules against a specific object/instance is not supported."));
		}

		private class ExpressionWithNonStaticRootRuleSet : RuleSet
		{
			public ExpressionWithNonStaticRootRuleSet()
			{
				Name = GetType().Name;

				Rules.Add(
					Rule("String.IndexOf")
						.If(() => "xxx".IndexOf(Context.Read(BtsProperties.ReceivePortName), StringComparison.OrdinalIgnoreCase) > 0)
						.Then(() => Context.Write(BizTalkFactoryProperties.SenderName, "String.IndexOf"))
					);
			}
		}

		private class InstanceMethodAccessRuleSet : RuleSet
		{
			public InstanceMethodAccessRuleSet()
			{
				Rules.Add(
					Rule("InstanceMethodAccessRule")
						.If(() => true)
						.Then(() => Context.Promote(BizTalkFactoryProperties.EnvelopeSpecName, GetToken()))
					);
			}

			private string GetToken()
			{
				return "not supported.";
			}
		}

		private class MemberAccessRuleSet : RuleSet
		{
			public MemberAccessRuleSet()
			{
				Rules.Add(
					Rule("String.IndexOf")
						.If(() => Context.Read(BtsProperties.ReceivePortName).IndexOf("yyy", StringComparison.OrdinalIgnoreCase) >= 0)
						.Then(() => Context.Write(BizTalkFactoryProperties.ReceiverName, "String.IndexOf"))
					);
			}
		}
	}
}
