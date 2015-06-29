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
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using Be.Stateless.BizTalk.Unit.ServiceModel;
using Be.Stateless.BizTalk.Unit.ServiceModel.Extensions;
using Be.Stateless.IO;
using Be.Stateless.ServiceModel.Channels;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.ServiceModel
{
	[SimpleServiceHostActivator(typeof(SimpleServiceHost<CalculatorService, ICalculatorService>), typeof(BasicHttpBinding), CalculatorService.URI)]
	[StubServiceHostActivator]
	[TestFixture]
	public class CalculatorServiceRelayFixture
	{
		[Test]
		public void GetMetadata()
		{
			// How to: Import Metadata into Service Endpoints, see http://msdn.microsoft.com/en-us/library/ms733780(v=vs.100).aspx
			var mexAddress = new EndpointAddress(CalculatorService.URI + "/mex");
			var mexClient = new MetadataExchangeClient(mexAddress) { ResolveMetadataReferences = true };
			var metaSet = mexClient.GetMetadata();

			var importer = new WsdlImporter(metaSet);
			var contracts = importer.ImportAllContracts();
			var generator = new ServiceContractGenerator();
			foreach (var contract in contracts)
			{
				generator.GenerateServiceContractType(contract);
			}
			Assert.That(generator.Errors.Count, Is.EqualTo(0), "There were errors during code compilation.");
		}

		[Test]
		public void RelayAsyncMessage()
		{
			StubServiceHost.FindDefaultService<ICalculatorService>()
				.Setup(s => s.BeginMultiply(It.IsAny<XmlCalculatorRequest>(), It.IsAny<AsyncCallback>(), It.IsAny<object>()))
				.Returns(new StringStream(string.Format(CALCULATOR_RESPONSE_XML, 2)));

			ICalculatorService client = null;
			try
			{
				client = SimpleServiceClient<CalculatorService, ICalculatorService>.Create();

				var calculatorResult = Task<XmlCalculatorResponse>.Factory
					.FromAsync(client.BeginMultiply, client.EndMultiply, new XmlCalculatorRequest(CALCULATOR_REQUEST_XML), null)
					.Result;
				Assert.AreEqual(string.Format(CALCULATOR_RESPONSE_XML, 2), calculatorResult.RawXmlBody);
				client.Close();
			}
			catch (Exception)
			{
				if (client != null) client.Abort();
				throw;
			}
		}

		[Test]
		public void RelayAsyncMessageTimesOut()
		{
			StubServiceHost.FindDefaultService<ICalculatorService>()
				.Setup(s => s.BeginDivide(It.IsAny<XmlCalculatorRequest>(), It.IsAny<AsyncCallback>(), It.IsAny<object>()))
				.Callback(() => Thread.Sleep(3000))
				.Returns(new StringStream(string.Format(CALCULATOR_RESPONSE_XML, 2)));

			var client = SimpleServiceClient<CalculatorService, ICalculatorService>.Create();
			Assert.That(
				() => Task<XmlCalculatorResponse>.Factory
					.FromAsync(client.BeginDivide, client.EndDivide, new XmlCalculatorRequest(CALCULATOR_REQUEST_XML), null)
					.Result,
				Throws.TypeOf<AggregateException>()
					.With.InnerException.TypeOf<FaultException<ExceptionDetail>>()
					.With.InnerException.Message.Contains("has exceeded the allotted timeout"));
			client.Close();
		}

		[Test]
		public void RelayCompositeMessage()
		{
			const string RESPONSE_XML = "<CalculatorResponse xmlns=\"urn:services.stateless.be:unit:calculator\">" +
				"<s0:Result xmlns:s0=\"urn:services.stateless.be:unit:calculator\">one</s0:Result>" +
				"<s0:Result xmlns:s0=\"urn:services.stateless.be:unit:calculator\">two</s0:Result>" +
				"</CalculatorResponse>";

			StubServiceHost.FindDefaultService<ICalculatorService>()
				.Setup(s => s.Add(It.IsAny<XmlCalculatorRequest>()))
				.Returns(new StringStream(RESPONSE_XML));

			ICalculatorService client = null;
			try
			{
				client = SimpleServiceClient<CalculatorService, ICalculatorService>.Create();
				var calculatorResult = client.Add(new XmlCalculatorRequest(CALCULATOR_REQUEST_XML));
				Assert.AreEqual(RESPONSE_XML, calculatorResult.RawXmlBody);
				client.Close();
			}
			catch (Exception)
			{
				if (client != null) client.Abort();
				throw;
			}
		}

		[Test]
		public void RelaySyncMessage()
		{
			StubServiceHost.FindDefaultService<ICalculatorService>()
				.Setup(s => s.Add(It.IsAny<XmlCalculatorRequest>()))
				.Returns(new StringStream(string.Format(CALCULATOR_RESPONSE_XML, 3)));

			ICalculatorService client = null;
			try
			{
				client = SimpleServiceClient<CalculatorService, ICalculatorService>.Create();
				var calculatorResult = client.Add(new XmlCalculatorRequest(CALCULATOR_REQUEST_XML));
				Assert.AreEqual(string.Format(CALCULATOR_RESPONSE_XML, 3), calculatorResult.RawXmlBody);
				client.Close();
			}
			catch (Exception)
			{
				if (client != null) client.Abort();
				throw;
			}
		}

		[Test]
		public void RelaySyncMessageTimesOut()
		{
			StubServiceHost.FindDefaultService<ICalculatorService>()
				.Setup(s => s.Subtract(It.IsAny<XmlCalculatorRequest>()))
				.Callback(() => Thread.Sleep(3000))
				.Returns(new StringStream(string.Format(CALCULATOR_RESPONSE_XML, 1)));

			var client = SimpleServiceClient<CalculatorService, ICalculatorService>.Create();
			Assert.That(
				() => client.Subtract(new XmlCalculatorRequest(CALCULATOR_REQUEST_XML)),
				Throws.TypeOf<FaultException<ExceptionDetail>>()
					.With.Message.Contains("The request channel timed out while waiting for a reply"));
			client.Close();
		}

		private const string CALCULATOR_REQUEST_XML = "<CalculatorRequest xmlns=\"urn:services.stateless.be:unit:calculator\">" +
			"<s0:Arguments xmlns:s0=\"urn:services.stateless.be:unit:calculator\">" +
			"<s0:Term>2</s0:Term>" +
			"<s0:Term>1</s0:Term>" +
			"</s0:Arguments>" +
			"</CalculatorRequest>";

		private const string CALCULATOR_RESPONSE_XML = "<CalculatorResponse xmlns=\"urn:services.stateless.be:unit:calculator\">" +
			"<s0:Result xmlns:s0=\"urn:services.stateless.be:unit:calculator\">{0}</s0:Result>" +
			"</CalculatorResponse>";
	}
}
