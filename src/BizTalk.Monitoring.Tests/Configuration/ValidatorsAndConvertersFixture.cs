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

using NUnit.Framework;

namespace Be.Stateless.BizTalk.Monitoring.Configuration
{
	[TestFixture]
	public class ValidatorsAndConvertersFixture
	{
		[Test]
		public void NonEmptyStringValidatorCanValidate()
		{
			var validator = ValidatorsAndConverters.NonEmptyStringValidator;
			Assert.That(validator.CanValidate(typeof(string)));
		}

		[Test]
		public void NonEmptyStringValidatorCannotValidate()
		{
			var validator = ValidatorsAndConverters.NonEmptyStringValidator;
			Assert.That(validator.CanValidate(typeof(object)), Is.False);
		}

		[Test]
		public void NonEmptyStringValidatorIsASingleton()
		{
			var validator1 = ValidatorsAndConverters.NonEmptyStringValidator;
			Assert.That(validator1, Is.Not.Null);
			var validator2 = ValidatorsAndConverters.NonEmptyStringValidator;
			Assert.That(validator1, Is.Not.Null);
			Assert.That(validator1, Is.SameAs(validator2));
		}

		[Test]
		public void NonEmptyStringValidatorThrows()
		{
			var validator = ValidatorsAndConverters.NonEmptyStringValidator;
			Assert.That(
				// ReSharper disable once AssignNullToNotNullAttribute
				() => validator.Validate(null),
				Throws.ArgumentException.With.Message.EqualTo("The string must be at least 1 characters long."));
		}

		[Test]
		public void NonEmptyStringValidatorValidates()
		{
			var validator = ValidatorsAndConverters.NonEmptyStringValidator;
			Assert.That(() => validator.Validate("a"), Throws.Nothing);
		}
	}
}
