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

using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Tracking.Messaging
{
	[TestFixture]
	public class ProcessNamesFixture
	{
		[Test]
		public void InstanceStringPropertyValuesAreComputed()
		{
			Assert.That(DiscoverableArea.Processes.ProcessOne, Is.EqualTo(typeof(DiscoverableArea).FullName + ".ProcessOne"));
		}

		[Test]
		public void NonPublicPropertyThrows()
		{
			Assert.That(
				() => UndiscoverableNonPublic.Processes.ProcessFour,
				Throws.TypeOf<TypeInitializationException>().With.InnerException.TypeOf<ArgumentException>()
					.With.InnerException.Message.EqualTo(string.Format(MESSAGE, typeof(UndiscoverableNonPublic).FullName)));
		}

		[Test]
		public void NonStringPropertyThrows()
		{
			Assert.That(
				() => UndiscoverableNonString.Processes.ProcessThree,
				Throws.TypeOf<TypeInitializationException>().With.InnerException.TypeOf<ArgumentException>()
					.With.InnerException.Message.EqualTo(string.Format(MESSAGE, typeof(UndiscoverableNonString).FullName)));
		}

		[Test]
		public void StaticPropertyThrows()
		{
			Assert.That(
				() => UndiscoverableStatic.Processes,
				Throws.TypeOf<TypeInitializationException>().With.InnerException.TypeOf<ArgumentException>()
					.With.InnerException.Message.EqualTo(string.Format(MESSAGE, typeof(UndiscoverableStatic).FullName)));
		}

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
		private class DiscoverableArea : ProcessNames<DiscoverableArea>
		{
			public string ProcessOne { get; private set; }
		}

		[SuppressMessage("ReSharper", "UnusedMember.Local")]
		private class UndiscoverableStatic : ProcessNames<UndiscoverableStatic>
		{
			public static string ProcessTwo { get; private set; }
		}

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
		private class UndiscoverableNonString : ProcessNames<UndiscoverableNonString>
		{
			public int ProcessThree { get; private set; }
		}

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
		private class UndiscoverableNonPublic : ProcessNames<UndiscoverableNonPublic>
		{
			internal string ProcessFour { get; private set; }
		}

		const string MESSAGE = "{0} must only declare non-static public string properties.";
	}
}
