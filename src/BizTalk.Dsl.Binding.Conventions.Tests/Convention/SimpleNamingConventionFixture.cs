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
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Subscription;
using Be.Stateless.BizTalk.Pipelines;
using Be.Stateless.BizTalk.Unit.Resources;
using Microsoft.BizTalk.B2B.PartnerManagement;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	[TestFixture]
	public class SimpleNamingConventionFixture
	{
		[Test]
		public void ConventionalApplicationBindingSupportsBindingGeneration()
		{
			var applicationBinding = new SampleApplication {
				Timestamp = XmlConvert.ToDateTime("2015-02-17T22:51:04+01:00", XmlDateTimeSerializationMode.Local)
			};
			var applicationBindingSerializer = ((IBindingSerializerFactory) applicationBinding).GetBindingSerializer("PRD");

			var binding = applicationBindingSerializer.Serialize();

			Assert.That(binding, Is.EqualTo(ResourceManager.LoadString("Data.bindings.with.simple.naming.convention.xml")));
		}

		[Test]
		public void ConventionalApplicationBindingWithAreaSupportsBindingGeneration()
		{
			var applicationBinding = new Area.SampleApplication {
				Timestamp = XmlConvert.ToDateTime("2015-02-17T22:51:04+01:00", XmlDateTimeSerializationMode.Local)
			};
			var applicationBindingSerializer = ((IBindingSerializerFactory) applicationBinding).GetBindingSerializer("PRD");

			var binding = applicationBindingSerializer.Serialize();

			Assert.That(binding, Is.EqualTo(ResourceManager.LoadString("Data.bindings.with.simple.naming.convention.and.area.xml")));
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

		private class SampleApplication : ApplicationBinding<SimpleNamingConvention>
		{
			public SampleApplication()
			{
				Name = ApplicationName.Is("BizTalk.Factory");
				SendPorts.Add(UnitTestSendPort);
				ReceivePorts.Add(BatchReceivePort);
				ReceivePorts.Add(StandaloneReceivePort);
			}

			internal IReceivePort<SimpleNamingConvention> BatchReceivePort
			{
				get
				{
					return _receivePort ?? (_receivePort = ReceivePort(
						rp => {
							rp.Name = ReceivePortName.Offwards("Batch");
							rp.ReceiveLocations.Add(
								ReceiveLocation(
									rl => {
										rl.Name = ReceiveLocationName.About("Release").FormattedAs.Xml;
										rl.ReceivePipeline = new ReceivePipeline<BatchReceive>();
										rl.Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\files\drops"; });
										rl.Transport.Host = "Host";
									}));
						}));
				}
			}

			internal StandaloneReceivePort StandaloneReceivePort
			{
				get { return _standaloneReceivePort ?? (_standaloneReceivePort = new StandaloneReceivePort()); }
			}

			internal ISendPort<SimpleNamingConvention> UnitTestSendPort
			{
				get
				{
					return _sendPort ?? (_sendPort = SendPort(
						sp => {
							sp.Name = SendPortName.Towards("UnitTest.Batch").About("Trace").FormattedAs.Xml;
							sp.SendPipeline = new SendPipeline<PassThruTransmit>();
							sp.Transport.Adapter = new FileAdapter.Outbound(a => { a.DestinationFolder = @"C:\Files\Drops\BizTalk.Factory\Trace"; });
							sp.Transport.Host = "Host";
						}));
				}
			}

			private IReceivePort<SimpleNamingConvention> _receivePort;

			private ISendPort<SimpleNamingConvention> _sendPort;

			private StandaloneReceivePort _standaloneReceivePort;
		}

		private class StandaloneReceivePort : ReceivePort<SimpleNamingConvention>
		{
			public StandaloneReceivePort()
			{
				Name = ReceivePortName.Offwards("StandaloneBatch");
				ReceiveLocations.Add(
					ReceiveLocation(
						rl => {
							rl.Name = ReceiveLocationName.About("Release").FormattedAs.Xml;
							rl.ReceivePipeline = new ReceivePipeline<BatchReceive>();
							rl.Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\files\drops"; });
							rl.Transport.Host = "Host";
						}));
			}
		}
	}
}

namespace Be.Stateless.Area
{
	internal class SampleApplication : ApplicationBinding<NamingConvention<Party, MessageName>>
	{
		public SampleApplication()
		{
			ReceivePorts.Add(new Invoice.TaxAgencyReceivePort());
			SendPorts.Add(new Invoice.BankSendPort());
		}
	}

	namespace Invoice
	{
		internal class BankSendPort : NamingConventionFixture.BankSendPort { }

		internal class TaxAgencyReceivePort : NamingConventionFixture.TaxAgencyReceivePort { }
	}
}
