#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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

namespace Be.Stateless.Reflection
{
	[TestFixture]
	public class ReflectorFixture
	{
		[Test]
		public void SetInterfaceReadOnlyProperty()
		{
			Reflector.SetProperty(Stub.Instance, "TargetEnvironment", "DEV");
			Assert.That(Stub.Instance.TargetEnvironment, Is.EqualTo("DEV"));
		}

		[Test]
		public void SetInterfaceReadOnlyPropertyManually()
		{
			Stub.Instance.GetType().GetProperty("TargetEnvironment").SetValue(Stub.Instance, "DEV");
			Assert.That(Stub.Instance.TargetEnvironment, Is.EqualTo("DEV"));
		}

		[Test]
		public void SetPrivateProperty()
		{
			Reflector.SetProperty(Stub.Instance, "Property", "DEV");
			Assert.That(((Stub) Stub.Instance).Property, Is.EqualTo("DEV"));
		}

		private interface IStub
		{
			string TargetEnvironment { get; }
		}

		private class Stub : IStub
		{
			private Stub() { }

			#region IStub Members

			public string TargetEnvironment { get; private set; }

			#endregion

			public string Property { get; private set; }

			internal static readonly IStub Instance = new Stub();
		}
	}
}
