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

using System;
using Be.Stateless.BizTalk.Policies.Send;
using Microsoft.RuleEngine;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace Be.Stateless.BizTalk.RuleEngine
{
	[TestFixture]
	public class PolicyNameConverterFixture
	{
		[Test]
		public void CanConvertFrom()
		{
			var sut = new PolicyNameConverter();
			Assert.That(sut.CanConvertFrom(typeof(string)));
		}

		[Test]
		public void CanConvertTo()
		{
			var sut = new PolicyNameConverter();
			Assert.That(sut.CanConvertTo(typeof(string)));
		}

		[Test]
		public void ConvertFrom()
		{
			var ruleSetInfo = new FailedProcessResolver().RuleSetInfo;
			var displayName = string.Format("{0}, Version={1}.{2}", ruleSetInfo.Name, ruleSetInfo.MajorRevision, ruleSetInfo.MinorRevision);

			var sut = new PolicyNameConverter();
			var convertedObject = sut.ConvertFrom(displayName);

			Assert.That(convertedObject, Is.InstanceOf<RuleSetInfo>());
			var convertedResultSetInfo = convertedObject as RuleSetInfo;
			// ReSharper disable PossibleNullReferenceException
			Assert.That(convertedResultSetInfo.Name, Is.EqualTo(ruleSetInfo.Name));
			Assert.That(convertedResultSetInfo.MajorRevision, Is.EqualTo(ruleSetInfo.MajorRevision));
			Assert.That(convertedResultSetInfo.MinorRevision, Is.EqualTo(ruleSetInfo.MinorRevision));
			// ReSharper restore PossibleNullReferenceException
		}

		[Test]
		public void ConvertFromThrowsWhenInvalidFormat()
		{
			const string value = "name, 1.0";
			var sut = new PolicyNameConverter();
			Assert.That(
				() => sut.ConvertFrom(value),
				Throws.TypeOf<NotSupportedException>().With.Message.EqualTo(string.Format("'{0}' format is invalid and cannot be parsed into a {1}.", value, typeof(PolicyName).Name)));
		}

		[Test]
		public void ConvertFromThrowsWhenNullOrEmptyString()
		{
			var sut = new PolicyNameConverter();
			Assert.That(
				() => sut.ConvertFrom(""),
				Throws.TypeOf<NotSupportedException>().With.Message.EqualTo(string.Format("Cannot parse a null or empty string into a {0}.", typeof(PolicyName).Name)));
		}

		[Test]
		public void ConvertTo()
		{
			var ruleSetInfo = new FailedProcessResolver().RuleSetInfo;

			var sut = new PolicyNameConverter();
			var displayName = sut.ConvertTo(ruleSetInfo, typeof(string));

			Assert.That(displayName, Is.EqualTo(string.Format("{0}, Version={1}.{2}", ruleSetInfo.Name, ruleSetInfo.MajorRevision, ruleSetInfo.MinorRevision)));
		}
	}
}
