#region Copyright & License

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

using System;
using System.Xml;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Install;
using Be.Stateless.BizTalk.Pipelines;
using Be.Stateless.BizTalk.Unit.Resources;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	[TestFixture]
	public class ApplicationBindingSerializerFixture
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
		public void GenerateBindingsWithoutConventions()
		{
			var applicationBinding = new ApplicationBinding(
				a => {
					a.Name = "Unconventional application name";
					a.Description = "Some useless test application.";
					a.ReceivePorts.Add(
						new ReceivePort(
							p => {
								p.Name = "Unconventional receive port name";
								p.Description = "Some useless test receive port.";
								p.ReceiveLocations.Add(
									new ReceiveLocation(
										l => {
											l.Name = "Unconventional receive location name";
											l.Description = "Some useless test receive location.";
											l.ReceivePipeline = new ReceivePipeline<PassThruReceive>();
											l.Transport.Adapter = new FileAdapter.Inbound(fa => { fa.ReceiveFolder = @"c:\files\drops"; });
											l.Transport.Host = "Receive Host";
										})
								);
							})
					);
					a.ReceivePorts.Add(
						new ReceivePort(
							p => {
								p.Name = "Control message receive port name";
								p.ReceiveLocations.Add(
									new ReceiveLocation(
										l => {
											l.Name = "Control message receive location name";
											l.ReceivePipeline = new ReceivePipeline<PassThruReceive>();
											l.Transport.Adapter = new HttpAdapter.Inbound(
												ha => {
													ha.Path = "/Control/BTSHTTPReceive.dll";
													ha.ReturnCorrelationHandle = false;
												});
											l.Transport.Host = "Receive Host";
										}));
							}));
					a.SendPorts.Add(
						new SendPort(
							p => {
								p.Name = "Unconventional send port name";
								p.Description = "Some useless test send port.";
								p.SendPipeline = new SendPipeline<PassThruTransmit>();
								p.Transport.Adapter = new FileAdapter.Outbound(fa => { fa.DestinationFolder = @"c:\files\drops"; });
								p.Transport.Host = "Send Host";
							}),
						new SendPort(
							p => {
								p.Name = "Get document send port name";
								p.SendPipeline = new SendPipeline<PassThruTransmit>();
								p.Transport.Adapter = new HttpAdapter.Outbound(
									ha => {
										ha.Url = new Uri("http://localhost:8000/stubservice");
										ha.EnableChunkedEncoding = false;
										ha.MaxRedirects = 0;
										ha.RequestTimeout = TimeSpan.FromMinutes(1);
									});
								p.Transport.Host = "Send Host";
							})
					);
				}) {
				Timestamp = XmlConvert.ToDateTime("2015-02-18T22:51:04+01:00", XmlDateTimeSerializationMode.Local)
			};
			var applicationBindingSerializer = ((IBindingSerializerFactory) applicationBinding).GetBindingSerializer();

			var binding = applicationBindingSerializer.Serialize();

			Assert.That(binding, Is.EqualTo(ResourceManager.LoadString("Data.bindings.xml")));
		}
	}
}
