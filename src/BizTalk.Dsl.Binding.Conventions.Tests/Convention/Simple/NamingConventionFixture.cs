#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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

using System.Xml;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding.Subscription;
using Be.Stateless.BizTalk.Unit.Resources;
using Microsoft.BizTalk.B2B.PartnerManagement;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention.Simple
{
	[TestFixture]
	public class NamingConventionFixture
	{
		[Test]
		public void ConventionalApplicationBindingSupportsBindingGeneration()
		{
			var applicationBinding = new SampleApplication {
				Timestamp = XmlConvert.ToDateTime("2015-02-17T22:51:04+01:00", XmlDateTimeSerializationMode.Local)
			};
			var applicationBindingSerializer = ((IBindingSerializerFactory) applicationBinding).GetBindingSerializer("PRD");

			var binding = applicationBindingSerializer.Serialize();

			Assert.That(binding, Is.EqualTo(ResourceManager.LoadString("Data.bindings.xml")));
		}

		[Test]
		public void ConventionalApplicationBindingWithAreaSupportsBindingGeneration()
		{
			var applicationBinding = new Area.SampleApplicationWithArea {
				Timestamp = XmlConvert.ToDateTime("2015-02-17T22:51:04+01:00", XmlDateTimeSerializationMode.Local)
			};
			var applicationBindingSerializer = ((IBindingSerializerFactory) applicationBinding).GetBindingSerializer("PRD");

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
			var receivePort = new SampleApplication().StandaloneReceivePort;
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
