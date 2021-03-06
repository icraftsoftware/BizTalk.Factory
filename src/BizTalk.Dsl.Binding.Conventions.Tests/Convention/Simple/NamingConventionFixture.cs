﻿#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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

using Be.Stateless.Area;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding.Subscription;
using Be.Stateless.BizTalk.Install;
using Be.Stateless.BizTalk.Unit.Resources;
using Microsoft.BizTalk.B2B.PartnerManagement;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention.Simple
{
	[TestFixture]
	public class NamingConventionFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			BindingGenerationContext.TargetEnvironment = "ANYTHING";
		}

		[TearDown]
		public void TearDown()
		{
			BindingGenerationContext.TargetEnvironment = null;
		}

		#endregion

		[Test]
		public void ConventionalApplicationBindingSupportsBindingGeneration()
		{
			var applicationBindingSerializer = ((IBindingSerializerFactory) new SampleApplication()).GetBindingSerializer();

			var binding = applicationBindingSerializer.Serialize();

			Assert.That(binding, Is.EqualTo(ResourceManager.LoadString("Data.bindings.xml")));
		}

		[Test]
		public void ConventionalApplicationBindingWithAreaSupportsBindingGeneration()
		{
			var applicationBindingSerializer = ((IBindingSerializerFactory) new SampleApplicationWithArea()).GetBindingSerializer();

			var binding = applicationBindingSerializer.Serialize();

			Assert.That(binding, Is.EqualTo(ResourceManager.LoadString("Data.bindings.with.area.xml")));
		}

		[Test]
		public void ConventionalReceivePortNameCanBeReferencedInSubscriptionFilter()
		{
			var receivePort = new SampleApplication().BatchReceivePort;
			var filter = new Filter(() => BtsProperties.ReceivePortName == receivePort.Name);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BtsProperties.ReceivePortName.Type.FullName,
						(int) FilterOperator.Equals,
						((ISupportNamingConvention) receivePort).Name)));
		}

		[Test]
		public void ConventionalSendPortNameCanBeReferencedInSubscriptionFilter()
		{
			var sendPort = new SampleApplication().UnitTestSendPort;
			var filter = new Filter(() => BtsProperties.SendPortName == sendPort.Name);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BtsProperties.SendPortName.Type.FullName,
						(int) FilterOperator.Equals,
						((ISupportNamingConvention) sendPort).Name)));
		}

		[Test]
		public void ConventionalStandaloneReceivePortNameCanBeReferencedInSubscriptionFilter()
		{
			var receivePort = new SampleApplication().ReceivePorts.Find<StandaloneReceivePort>();
			var filter = new Filter(() => BtsProperties.ReceivePortName == receivePort.Name);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BtsProperties.ReceivePortName.Type.FullName,
						(int) FilterOperator.Equals,
						((ISupportNamingConvention) receivePort).Name)));
		}
	}
}
