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
using System.Linq;
using Be.Stateless.BizTalk.Dsl.RuleEngine.Extensions;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.RuleEngine
{
	[TestFixture]
	public class ProcessNameAttributeFixture
	{
		[Test]
		public void GetProcessNamesFromTypeList()
		{
			var expected = new[] {
				typeof(TestProcesses).FullName + ".One",
				typeof(TestProcesses).FullName + ".Two",
				typeof(ReflectableProcessNames).FullName + ".ProcessOne"
			};

			var processNames = ProcessNameAttribute.GetProcessNames(
				new[] {
					typeof(TestProcesses),
					typeof(ProcessNameAttributeFixture),
					typeof(ReflectableProcessNames)
				});

			Assert.That(processNames, Is.EquivalentTo(expected));
		}

		[Test]
		public void GetProcessNamesFromTypeListReturnsEmptyListWhenNoTypeIsAProcessDescriptor()
		{
			var processNames = ProcessNameAttribute.GetProcessNames(new[] { typeof(string), typeof(ProcessNameAttributeFixture) });

			Assert.That(processNames.Any(), Is.False);
		}

		[Test]
		public void GetProcessNamesFromType()
		{
			var expected = new[] {
				typeof(TestProcesses).FullName + ".One",
				typeof(TestProcesses).FullName + ".Two"
			};

			var processNames = typeof(TestProcesses).GetProcessNames();

			Assert.That(processNames, Is.EquivalentTo(expected));
		}

		[Test]
		public void GetProcessNamesFromTypeThrowsIfNotProcessDescriptor()
		{
			Assert.That(
				() => typeof(ProcessNameAttributeFixture).GetProcessNames(),
				Throws.ArgumentException.With.Message.StartsWith(
					"ProcessNameAttributeFixture does not declare any ProcessNameAttribute-qualified property or field."));
		}

		[Test]
		public void GetProcessNameThrowsForMemberOfNonStaticType()
		{
			var propertyInfo = typeof(UnreflectableProcessNames).GetProperty("ProcessOne");
			Assert.That(
				() => propertyInfo.GetProcessName(),
				Throws.InstanceOf<NotSupportedException>().With.Message.EqualTo(
					"UnreflectableProcessNames is not static, ProcessNameAttribute can only be applied to members of static classes."));
		}

		[Test]
		public void GetProcessNameThrowsForNonStringMembers()
		{
			var propertyInfo = typeof(ReflectableProcessNames).GetField("ProcessThree");
			Assert.That(
				() => propertyInfo.GetProcessName(),
				Throws.InstanceOf<NotSupportedException>().With.Message.EqualTo(
					"ProcessThree is not of string type, process names can only be reflected from string members."));
		}

		[Test]
		public void GetProcessNameThrowsForUnqualifiedMembers()
		{
			var propertyInfo = typeof(ReflectableProcessNames).GetProperty("ProcessTwo");
			Assert.That(
				() => propertyInfo.GetProcessName(),
				Throws.InstanceOf<NotSupportedException>().With.Message.EqualTo(
					"ProcessTwo has not been qualified, process names can only be reflected from members upon which the ProcessNameAttribute has been applied."));
		}

		[Test]
		public void IsProcessNameReturnsFalseForMemberOfNonStaticType()
		{
			var propertyInfo = typeof(UnreflectableProcessNames).GetProperty("ProcessOne");
			Assert.That(propertyInfo.IsProcessName(), Is.False);
		}

		[Test]
		public void IsProcessNameReturnsFalseForNonstringMembers()
		{
			var propertyInfo = typeof(ReflectableProcessNames).GetField("ProcessThree");
			Assert.That(propertyInfo.IsProcessName(), Is.False);
		}

		[Test]
		public void IsProcessNameReturnsFalseForUnqualifiedMembers()
		{
			var propertyInfo = typeof(ReflectableProcessNames).GetProperty("ProcessTwo");
			Assert.That(propertyInfo.IsProcessName(), Is.False);
		}

		[Test]
		public void ProcessNameIsAssignedBackToMember()
		{
			var propertyInfo = typeof(ReflectableProcessNames).GetProperty("ProcessOne");
			var processName = propertyInfo.GetProcessName();
			Assert.That(ReflectableProcessNames.ProcessOne, Is.EqualTo(processName));
		}

		[Test]
		public void ProcessNameIsTypePlusMemberNames()
		{
			var propertyInfo = typeof(ReflectableProcessNames).GetProperty("ProcessOne");
			Assert.That(propertyInfo.IsProcessName(), Is.True);
			Assert.That(propertyInfo.GetProcessName(), Is.EqualTo(typeof(ReflectableProcessNames).FullName + "." + "ProcessOne"));
		}

#pragma warning disable 169, 649
		// ReSharper disable InconsistentNaming
		// ReSharper disable UnusedAutoPropertyAccessor.Global
		// ReSharper disable UnusedAutoPropertyAccessor.Local
		// ReSharper disable UnusedMember.Local
		public static class ReflectableProcessNames
		{
			[ProcessName]
			public static string ProcessOne { get; set; }

			public static string ProcessTwo { get; set; }

			[ProcessName]
			public static int ProcessThree;
		}

		private class UnreflectableProcessNames
		{
			[ProcessName]
			public string ProcessOne { get; set; }
		}

		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedAutoPropertyAccessor.Local
		// ReSharper restore UnusedAutoPropertyAccessor.Global
		// ReSharper restore InconsistentNaming
#pragma warning restore 169, 649
	}
}
