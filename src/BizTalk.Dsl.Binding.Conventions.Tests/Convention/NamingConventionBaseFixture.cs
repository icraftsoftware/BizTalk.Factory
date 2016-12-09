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

using System;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel.Configuration;
using Be.Stateless.Area;
using Be.Stateless.Area.Income;
using Be.Stateless.Area.Invoice;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.Simple;
using Microsoft.Adapters.OracleDB;
using Microsoft.Adapters.SAP;
using Microsoft.Adapters.Sql;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using CustomBindingElement = Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration.CustomBindingElement;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	[TestFixture]
	[SuppressMessage("ReSharper", "RedundantOverriddenMember")]
	public class NamingConventionBaseFixture
	{
		[Test]
		[SuppressMessage("ReSharper", "RedundantNameQualifier")]
		public void ComputeAdapterNameResolvesActualProtocolTypeNameForWcfCustomAdapter()
		{
			var sut = new NamingConventionDouble();

			IAdapter adapter = new CustomAdapterFake<NetTcpBindingElement, CustomRLConfig>();
			Assert.That(sut.ComputeAdapterNameSpy(adapter), Is.EqualTo("WCF-CustomNetTcp"));

			adapter = new CustomAdapterFake<System.ServiceModel.Configuration.NetMsmqBindingElement, CustomRLConfig>();
			Assert.That(sut.ComputeAdapterNameSpy(adapter), Is.EqualTo("WCF-CustomNetMsmq"));

			adapter = new CustomAdapterFake<Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration.NetMsmqBindingElement, CustomRLConfig>();
			Assert.That(sut.ComputeAdapterNameSpy(adapter), Is.EqualTo("WCF-CustomNetMsmq"));

			adapter = new CustomAdapterFake<OracleDBBindingConfigurationElement, CustomRLConfig>();
			Assert.That(sut.ComputeAdapterNameSpy(adapter), Is.EqualTo("WCF-CustomOracleDB"));

			adapter = new CustomAdapterFake<SqlAdapterBindingConfigurationElement, CustomRLConfig>();
			Assert.That(sut.ComputeAdapterNameSpy(adapter), Is.EqualTo("WCF-CustomSql"));

			adapter = new WcfCustomAdapter.Outbound<CustomBindingElement>(a => a.Binding.Add(new MtomMessageEncodingElement(), new HttpsTransportElement()));
			Assert.That(sut.ComputeAdapterNameSpy(adapter), Is.EqualTo("WCF-CustomHttps"));

			adapter = new WcfCustomAdapter.Outbound<CustomBindingElement>(a => a.Binding.Add(new TcpTransportElement()));
			Assert.That(sut.ComputeAdapterNameSpy(adapter), Is.EqualTo("WCF-CustomTcp"));

			adapter = new WcfCustomAdapter.Outbound<CustomBindingElement>(a => a.Binding.Add(new SAPAdapterExtensionElement()));
			Assert.That(sut.ComputeAdapterNameSpy(adapter), Is.EqualTo("WCF-CustomSap"));

			adapter = new WcfCustomAdapter.Outbound<CustomBindingElement>(a => a.Binding.Add(new SqlAdapterBindingElementExtensionElement()));
			Assert.That(sut.ComputeAdapterNameSpy(adapter), Is.EqualTo("WCF-CustomSql"));

			// notice that OracleDBAdapterExtensionElement cannot be used because it is internal :(
			//adapter = new WcfCustomAdapter.Outbound<CustomBindingElement>(a => a.Binding.Add(new OracleDBAdapterExtensionElement()));
		}

		[Test]
		public void ComputeAdapterNameResolvesActualProtocolTypeNameForWcfCustomIsolatedAdapter()
		{
			var sut = new NamingConventionDouble();

			IAdapter adapter = new CustomIsolatedAdapterFake<NetTcpBindingElement, CustomRLConfig>();
			Assert.That(sut.ComputeAdapterNameSpy(adapter), Is.EqualTo("WCF-CustomIsolatedNetTcp"));

			adapter = new CustomIsolatedAdapterFake<WSHttpBindingElement, CustomRLConfig>();
			Assert.That(sut.ComputeAdapterNameSpy(adapter), Is.EqualTo("WCF-CustomIsolatedWsHttp"));

			adapter = new CustomIsolatedAdapterFake<BasicHttpBindingElement, CustomRLConfig>();
			Assert.That(sut.ComputeAdapterNameSpy(adapter), Is.EqualTo("WCF-CustomIsolatedBasicHttp"));
		}

		[Test]
		public void ComputeApplicationNameReturnsGivenName()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();

			var sut = new NamingConventionDouble { ApplicationName = "SampleApplicationName" };

			Assert.That(sut.ComputeApplicationNameSpy(applicationBindingMock.Object), Is.EqualTo("SampleApplicationName"));
		}

		[Test]
		public void ComputeApplicationNameReturnsTypeNameIfNotGiven()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.Setup(ab => ab.GetType()).Returns(typeof(SampleApplication));

			var sut = new NamingConventionDouble();

			Assert.That(sut.ComputeApplicationNameSpy(applicationBindingMock.Object), Is.EqualTo("SampleApplication"));
		}

		[Test]
		public void ComputeAreaIsCalledForReceiveLocation()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionDouble>>();
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionDouble { Party = "SomeParty" });
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionDouble>>();
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(StandaloneReceiveLocation));
			receiveLocationMock.Setup(rl => rl.Transport).Returns(new ReceiveLocationTransport { Adapter = new FileAdapter.Inbound(t => { }) });

			var namingConventionMock = new Mock<NamingConventionDouble>();
			namingConventionMock.Object.MessageName = "SomeMessage";
			namingConventionMock.Object.MessageFormat = "SomeFormat";

			namingConventionMock.Object.ComputeReceiveLocationNameSpy(receiveLocationMock.Object);

			namingConventionMock.Protected().Verify("ComputeArea", Times.Once(), receivePortMock.Object.GetType());
			namingConventionMock.Protected().Verify("ComputeArea", Times.Once(), receiveLocationMock.Object.GetType());
		}

		[Test]
		public void ComputeAreaIsCalledForReceivePort()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionDouble>>();
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));

			var namingConventionMock = new Mock<NamingConventionDouble>();
			namingConventionMock.Object.Party = "SomeParty";

			namingConventionMock.Object.ComputeReceivePortNameSpy(receivePortMock.Object);

			namingConventionMock.Protected().Verify("ComputeArea", Times.Once(), receivePortMock.Object.GetType());
		}

		[Test]
		public void ComputeAreaIsCalledForSendPort()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionDouble>>();
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));
			sendPortMock.Setup(sp => sp.Transport).Returns(new SendPortTransport { Adapter = new FileAdapter.Outbound(t => { }) });

			var namingConventionMock = new Mock<NamingConventionDouble>();
			namingConventionMock.Object.Party = "SomeParty";
			namingConventionMock.Object.MessageName = "SomeMessage";
			namingConventionMock.Object.MessageFormat = "SomeFormat";

			namingConventionMock.Object.ComputeSendPortNameSpy(sendPortMock.Object);

			namingConventionMock.Protected().Verify("ComputeArea", Times.Once(), sendPortMock.Object.GetType());
		}

		[Test]
		public void ComputeAreaIsNotCalledForApplication()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.Setup(ab => ab.GetType()).Returns(typeof(SampleApplication));

			var namingConventionMock = new Mock<NamingConventionDouble>();
			namingConventionMock.Object.ComputeApplicationNameSpy(applicationBindingMock.Object);

			namingConventionMock.Protected().Verify("ComputeArea", Times.Never(), applicationBindingMock.Object.GetType());
		}

		[Test]
		public void ComputeAreaReturnsFourthTokenOfTypeQualifiedNameMadeOfExactlyFiveTokens()
		{
			var sut = new NamingConventionDouble();
			Assert.That(sut.ComputeAreaSpy(typeof(TaxAgencyReceivePort)), Is.EqualTo("Invoice"));
		}

		[Test]
		public void ComputeAreaReturnsNullWhenTypeQualifiedNameIsNotMadeOfExactlyFiveTokens()
		{
			var sut = new NamingConventionDouble();
			Assert.That(sut.ComputeAreaSpy(typeof(SampleApplication)), Is.Null);
			Assert.That(sut.ComputeAreaSpy(typeof(SampleApplicationWithArea)), Is.Null);
			Assert.That(sut.ComputeAreaSpy(typeof(StandaloneReceivePort)), Is.Null);
		}

		[Test]
		public void ComputeReceiveLocationNameDoesNotRequireAreaToMatchItsReceivePortOneIfItHasNone()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionDouble>>();
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionDouble { Party = "ReceivePortParty" });
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionDouble>>();
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(BankReceiveLocation));
			receiveLocationMock.Setup(rl => rl.Transport).Returns(new ReceiveLocationTransport { Adapter = new FileAdapter.Inbound(t => { }) });

			var sut = new NamingConventionDouble { MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			Assert.That(() => sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object), Throws.Nothing);
		}

		[Test]
		public void ComputeReceiveLocationNameEmbedsApplicationNameAndPartyAndMessageNameAndTransportAndMessageFormat()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionDouble>>();
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionDouble { Party = "SomeParty" });
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionDouble>>();
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(StandaloneReceiveLocation));
			receiveLocationMock.Setup(rl => rl.Transport).Returns(new ReceiveLocationTransport { Adapter = new FileAdapter.Inbound(t => { }) });

			var sut = new NamingConventionDouble { MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			Assert.That(sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object), Is.EqualTo("SomeApplication.RL1.SomeParty.SomeMessage.FILE.SomeFormat"));
		}

		[Test]
		public void ComputeReceiveLocationNameEmbedsArea()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionDouble>>();
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionDouble { Party = "ReceivePortParty" });
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(TaxAgencyReceivePort));

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionDouble>>();
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(StandaloneReceiveLocation));
			receiveLocationMock.Setup(rl => rl.Transport).Returns(new ReceiveLocationTransport { Adapter = new FileAdapter.Inbound(t => { }) });

			var sut = new NamingConventionDouble { MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			Assert.That(sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object), Is.EqualTo("SomeApplication.Invoice.RL1.ReceivePortParty.SomeMessage.FILE.SomeFormat"));
		}

		[Test]
		public void ComputeReceiveLocationNameEmbedsEmptyMessageFormat()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionDouble>>();
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionDouble { Party = "ReceivePortParty" });
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionDouble>>();
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(StandaloneReceiveLocation));
			receiveLocationMock.Setup(rl => rl.Transport).Returns(new ReceiveLocationTransport { Adapter = new FileAdapter.Inbound(t => { }) });

			var sut = new NamingConventionDouble { MessageName = "SomeMessage", MessageFormat = string.Empty };

			Assert.That(sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object), Is.EqualTo("SomeApplication.RL1.ReceivePortParty.SomeMessage.FILE"));
		}

		[Test]
		public void ComputeReceiveLocationNameRequiredPartyDefaultsToItsReceivePortOne()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionDouble>>();
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionDouble { Party = "ReceivePortParty" });
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionDouble>>();
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(StandaloneReceiveLocation));
			receiveLocationMock.Setup(rl => rl.Transport).Returns(new ReceiveLocationTransport { Adapter = new FileAdapter.Inbound(t => { }) });

			var sut = new NamingConventionDouble { MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object);

			Assert.That(sut.Party, Is.EqualTo("ReceivePortParty"));
		}

		[Test]
		public void ComputeReceiveLocationNameRequiresAreaToMatchItsReceivePortOneIfItHasOne()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionDouble>>();
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionDouble { Party = "ReceivePortParty" });
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(TaxAgencyReceivePort));

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionDouble>>();
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(BankReceiveLocation));
			receiveLocationMock.Setup(rl => rl.Transport).Returns(new ReceiveLocationTransport { Adapter = new FileAdapter.Inbound(t => { }) });

			var sut = new NamingConventionDouble { MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			Assert.That(
				() => sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object),
				Throws.TypeOf<NamingConventionException>().With.Message.EqualTo("ReceiveLocation's Area 'Income' does not match its ReceivePort's one 'Invoice'."));
		}

		[Test]
		public void ComputeReceiveLocationNameRequiresMessageName()
		{
			var receivePortMock = new Mock<IReceivePort<NamingConventionDouble>>();
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionDouble { Party = "ReceivePortParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionDouble>>();
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);

			var sut = new NamingConventionDouble();

			Assert.That(
				() => sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object),
				Throws.TypeOf<NamingConventionException>().With.Message.EqualTo("MessageName is required."));
		}

		[Test]
		public void ComputeReceiveLocationNameRequiresNonNullMessageFormat()
		{
			var receivePortMock = new Mock<IReceivePort<NamingConventionDouble>>();
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionDouble { Party = "ReceivePortParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionDouble>>();
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);

			var sut = new NamingConventionDouble { MessageName = "SomeMessage" };

			Assert.That(
				() => sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object),
				Throws.TypeOf<NamingConventionException>().With.Message.EqualTo("A non null MessageFormat is required."));
		}

		[Test]
		public void ComputeReceiveLocationNameRequiresParty()
		{
			var receivePortMock = new Mock<IReceivePort<NamingConventionDouble>>();
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionDouble());

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionDouble>>();
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);

			var sut = new NamingConventionDouble();

			Assert.That(
				() => sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object),
				Throws.TypeOf<NamingConventionException>().With.Message.EqualTo("Party is required."));
		}

		[Test]
		public void ComputeReceiveLocationNameRequiresPartyToMatchItsReceivePortOne()
		{
			var receivePortMock = new Mock<IReceivePort<NamingConventionDouble>>();
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionDouble { Party = "ReceivePortParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionDouble>>();
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);

			var sut = new NamingConventionDouble { Party = "ReceiveLocationParty", MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			Assert.That(
				() => sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object),
				Throws.TypeOf<NamingConventionException>()
					.With.Message.EqualTo("ReceiveLocation's Party 'ReceiveLocationParty' does not match its ReceivePort's one 'ReceivePortParty'."));
		}

		[Test]
		public void ComputeReceiveLocationNameTwoWay()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionDouble>>();
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionDouble { Party = "SomeParty" });
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));
			receivePortMock.Setup(rp => rp.IsTwoWay).Returns(true);

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionDouble>>();
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(StandaloneReceiveLocation));
			receiveLocationMock.Setup(rl => rl.Transport).Returns(new ReceiveLocationTransport { Adapter = new FileAdapter.Inbound(t => { }) });

			var sut = new NamingConventionDouble { MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			Assert.That(sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object), Is.EqualTo("SomeApplication.RL2.SomeParty.SomeMessage.FILE.SomeFormat"));
		}

		[Test]
		public void ComputeReceivePortNameEmbedsApplicationNameAndAreaAndParty()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionDouble>>();
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(TaxAgencyReceivePort));

			var sut = new NamingConventionDouble { Party = "SomeParty" };

			Assert.That(sut.ComputeReceivePortNameSpy(receivePortMock.Object), Is.EqualTo("SomeApplication.Invoice.RP1.SomeParty"));
		}

		[Test]
		public void ComputeReceivePortNameEmbedsApplicationNameAndParty()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionDouble>>();
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));

			var sut = new NamingConventionDouble { Party = "SomeParty" };

			Assert.That(sut.ComputeReceivePortNameSpy(receivePortMock.Object), Is.EqualTo("SomeApplication.RP1.SomeParty"));
		}

		[Test]
		public void ComputeReceivePortNameRequiresParty()
		{
			var receivePortMock = new Mock<IReceivePort<NamingConventionDouble>>();

			var sut = new NamingConventionDouble();

			Assert.That(
				() => sut.ComputeReceivePortNameSpy(receivePortMock.Object),
				Throws.TypeOf<NamingConventionException>().With.Message.EqualTo("Party is required."));
		}

		[Test]
		public void ComputeReceivePortNameTwoWay()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionDouble>>();
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));
			receivePortMock.Setup(rp => rp.IsTwoWay).Returns(true);

			var sut = new NamingConventionDouble { Party = "SomeParty" };

			Assert.That(sut.ComputeReceivePortNameSpy(receivePortMock.Object), Is.EqualTo("SomeApplication.RP2.SomeParty"));
		}

		[Test]
		public void ComputeSendPortNameEmbedsApplicationNameAndPartyAndMessageNameAndTransportAndMessageFormat()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionDouble>>();
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));
			sendPortMock.Setup(sp => sp.Transport).Returns(new SendPortTransport { Adapter = new FileAdapter.Outbound(t => { }) });

			var sut = new NamingConventionDouble { Party = "SomeParty", MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			Assert.That(sut.ComputeSendPortNameSpy(sendPortMock.Object), Is.EqualTo("SomeApplication.SP1.SomeParty.SomeMessage.FILE.SomeFormat"));
		}

		[Test]
		public void ComputeSendPortNameEmbedsArea()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionDouble>>();
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(BankSendPort));
			sendPortMock.Setup(sp => sp.Transport).Returns(new SendPortTransport { Adapter = new FileAdapter.Outbound(t => { }) });

			var sut = new NamingConventionDouble { Party = "SomeParty", MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			Assert.That(sut.ComputeSendPortNameSpy(sendPortMock.Object), Is.EqualTo("SomeApplication.Income.SP1.SomeParty.SomeMessage.FILE.SomeFormat"));
		}

		[Test]
		public void ComputeSendPortNameEmbedsEmptyMessageFormat()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionDouble>>();
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));
			sendPortMock.Setup(sp => sp.Transport).Returns(new SendPortTransport { Adapter = new FileAdapter.Outbound(t => { }) });

			var sut = new NamingConventionDouble { Party = "SomeParty", MessageName = "SomeMessage", MessageFormat = string.Empty };

			Assert.That(sut.ComputeSendPortNameSpy(sendPortMock.Object), Is.EqualTo("SomeApplication.SP1.SomeParty.SomeMessage.FILE"));
		}

		[Test]
		public void ComputeSendPortNameRequiresMessageName()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionDouble>>();
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));

			var sut = new NamingConventionDouble { Party = "SomeParty" };

			Assert.That(
				() => sut.ComputeSendPortNameSpy(sendPortMock.Object),
				Throws.TypeOf<NamingConventionException>().With.Message.EqualTo("MessageName is required."));
		}

		[Test]
		public void ComputeSendPortNameRequiresNonNullMessageFormat()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionDouble>>();
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));

			var sut = new NamingConventionDouble { Party = "SomeParty", MessageName = "SomeMessage" };

			Assert.That(
				() => sut.ComputeSendPortNameSpy(sendPortMock.Object),
				Throws.TypeOf<NamingConventionException>().With.Message.EqualTo("A non null MessageFormat is required."));
		}

		[Test]
		public void ComputeSendPortNameRequiresParty()
		{
			var sendPortMock = new Mock<ISendPort<NamingConventionDouble>>();

			var sut = new NamingConventionDouble();

			Assert.That(
				() => sut.ComputeSendPortNameSpy(sendPortMock.Object),
				Throws.TypeOf<NamingConventionException>().With.Message.EqualTo("Party is required."));
		}

		[Test]
		public void ComputeSendPortNameTwoWay()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionDouble>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionDouble>>();
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));
			sendPortMock.Setup(sp => sp.Transport).Returns(new SendPortTransport { Adapter = new FileAdapter.Outbound(t => { }) });
			sendPortMock.Setup(sp => sp.IsTwoWay).Returns(true);

			var sut = new NamingConventionDouble { Party = "SomeParty", MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			Assert.That(sut.ComputeSendPortNameSpy(sendPortMock.Object), Is.EqualTo("SomeApplication.SP2.SomeParty.SomeMessage.FILE.SomeFormat"));
		}

		internal class NamingConventionDouble : NamingConventionBase<NamingConventionDouble, string, string>, INamingConvention<NamingConventionDouble>
		{
			#region INamingConvention<NamingConventionDouble> Members

			string INamingConvention<NamingConventionDouble>.ComputeApplicationName(IApplicationBinding<NamingConventionDouble> application)
			{
				throw new NotImplementedException();
			}

			string INamingConvention<NamingConventionDouble>.ComputeReceivePortName(IReceivePort<NamingConventionDouble> receivePort)
			{
				throw new NotImplementedException();
			}

			string INamingConvention<NamingConventionDouble>.ComputeReceiveLocationName(IReceiveLocation<NamingConventionDouble> receiveLocation)
			{
				throw new NotImplementedException();
			}

			string INamingConvention<NamingConventionDouble>.ComputeSendPortName(ISendPort<NamingConventionDouble> sendPort)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region Base Class Member Overrides

			protected override string ComputeArea(Type type)
			{
				return base.ComputeArea(type);
			}

			#endregion

			public new string ApplicationName
			{
				get { return base.ApplicationName; }
				set { base.ApplicationName = value; }
			}

			public string ComputeReceivePortNameSpy(IReceivePort<NamingConventionDouble> receivePort)
			{
				return ComputeReceivePortName(receivePort);
			}

			public string ComputeReceiveLocationNameSpy(IReceiveLocation<NamingConventionDouble> receiveLocation)
			{
				return ComputeReceiveLocationName(receiveLocation);
			}

			public string ComputeSendPortNameSpy(ISendPort<NamingConventionDouble> sendPort)
			{
				return ComputeSendPortName(sendPort);
			}

			public string ComputeApplicationNameSpy(IApplicationBinding<NamingConventionDouble> application)
			{
				return ComputeApplicationName(application);
			}

			public string ComputeAdapterNameSpy(IAdapter adapter)
			{
				return ComputeAdapterName(adapter);
			}

			public string ComputeAreaSpy(Type type)
			{
				return ComputeArea(type);
			}
		}

		private class CustomAdapterFake<TBinding, TConfig> : WcfCustomAdapter<TBinding, TConfig>
			where TBinding : StandardBindingElement, new()
			where TConfig : AdapterConfig,
			IAdapterConfigAddress,
			IAdapterConfigIdentity,
			IAdapterConfigBinding,
			IAdapterConfigEndpointBehavior,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigOutboundMessageMarshalling,
			new() { }

		private class CustomIsolatedAdapterFake<TBinding, TConfig> : WcfCustomIsolatedAdapter<TBinding, TConfig>
			where TBinding : StandardBindingElement, new()
			where TConfig : RLConfig,
			IAdapterConfigBinding,
			IAdapterConfigEndpointBehavior,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigOutboundMessageMarshalling,
			new() { }
	}
}
