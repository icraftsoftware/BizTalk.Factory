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

using System.Xml;
using Be.Stateless.Area.Income;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.Simple;
using Be.Stateless.BizTalk.Pipelines;

namespace Be.Stateless.Area
{
	internal class SampleApplicationWithArea : ApplicationBinding<NamingConvention>
	{
		public SampleApplicationWithArea()
		{
			ReceivePorts.Add(new Invoice.TaxAgencyReceivePort());
			SendPorts.Add(new BankSendPort());
			Timestamp = XmlConvert.ToDateTime("2015-02-17T22:51:04+01:00", XmlDateTimeSerializationMode.Local);
		}
	}

	namespace Invoice
	{
		internal class TaxAgencyReceivePort : ReceivePort<NamingConvention>
		{
			public TaxAgencyReceivePort()
			{
				Name = ReceivePortName.Offwards("Job");
				ReceiveLocations.Add(new TaxAgencyReceiveLocation());
			}
		}

		internal class TaxAgencyReceiveLocation : ReceiveLocation<NamingConvention>
		{
			public TaxAgencyReceiveLocation()
			{
				Name = ReceiveLocationName.About("AddPart").FormattedAs.Xml;
				ReceivePipeline = new ReceivePipeline<BatchReceive>();
				Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\files\drops"; });
				Transport.Host = "Host";
			}
		}
	}

	namespace Income
	{
		internal class BankSendPort : SendPort<NamingConvention>
		{
			public BankSendPort()
			{
				Name = SendPortName.Towards("Job").About("Notification").FormattedAs.Xml;
				SendPipeline = new SendPipeline<PassThruTransmit>();
				Transport.Adapter = new FileAdapter.Outbound(a => { a.DestinationFolder = @"c:\files\drops"; });
				Transport.Host = "Host";
			}
		}

		internal class BankReceiveLocation : ReceiveLocation<NamingConvention>
		{
			public BankReceiveLocation()
			{
				Name = ReceiveLocationName.About("AddPart").FormattedAs.Xml;
				ReceivePipeline = new ReceivePipeline<BatchReceive>();
				Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\files\drops"; });
				Transport.Host = "Host";
			}
		}
	}
}

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention.Simple
{
	internal class SampleApplication : ApplicationBinding<NamingConvention>
	{
		public SampleApplication()
		{
			Name = ApplicationName.Is("Simple.SampleApplication");
			SendPorts.Add(UnitTestSendPort);
			SendPorts.Add(new StandaloneSendPort());
			ReceivePorts.Add(BatchReceivePort);
			ReceivePorts.Add(new StandaloneReceivePort());
			Timestamp = XmlConvert.ToDateTime("2015-02-17T22:51:04+01:00", XmlDateTimeSerializationMode.Local);
		}

		internal IReceivePort<NamingConvention> BatchReceivePort
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

		internal ISendPort<NamingConvention> UnitTestSendPort
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

		private IReceivePort<NamingConvention> _receivePort;

		private ISendPort<NamingConvention> _sendPort;
	}

	internal class StandaloneReceivePort : ReceivePort<NamingConvention>
	{
		public StandaloneReceivePort()
		{
			Name = ReceivePortName.Offwards("Job");
			ReceiveLocations.Add(new StandaloneReceiveLocation());
		}
	}

	internal class StandaloneReceiveLocation : ReceiveLocation<NamingConvention>
	{
		public StandaloneReceiveLocation()
		{
			Name = ReceiveLocationName.About("AddPart").FormattedAs.Xml;
			ReceivePipeline = new ReceivePipeline<BatchReceive>();
			Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\files\drops"; });
			Transport.Host = "Host";
		}
	}

	internal class StandaloneSendPort : SendPort<NamingConvention>
	{
		public StandaloneSendPort()
		{
			Name = SendPortName.Towards("Job").About("Notification").FormattedAs.Xml;
			SendPipeline = new SendPipeline<PassThruTransmit>();
			Transport.Adapter = new FileAdapter.Outbound(a => { a.DestinationFolder = @"c:\files\drops"; });
			Transport.Host = "Host";
		}
	}
}
