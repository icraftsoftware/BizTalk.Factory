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
using Be.Stateless.BizTalk.Unit.ServiceModel;
using Be.Stateless.BizTalk.Unit.ServiceModel.Extensions;
using Be.Stateless.ServiceModel.Channels;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.ServiceModel
{
	[SimpleServiceHostActivator(typeof(SimpleServiceHost<CalculatorService, IPerformService>), typeof(BasicHttpBinding), CalculatorService.URI)]
	[StubServiceHostActivator]
	[TestFixture]
	public class PerformServiceRelayFixture
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
		[Ignore("TODO")]
		public void PerformSyncFails() { }

		[Test]
		[Ignore("Need to fix relay for void method")]
		public void PerformSyncSucceeds()
		{
			StubServiceHost.FindDefaultService<IPerformService>()
				.Setup(s => s.Perform(It.IsAny<XlangCalculatorRequest>()));

			IPerformService client = null;
			try
			{
				var request = new XlangCalculatorRequest(CALCULATOR_REQUEST_XML);
				client = SimpleServiceClient<CalculatorService, IPerformService>.Create();
				client.Perform(request);
				client.Close();
			}
			catch (Exception)
			{
				if (client != null) client.Abort();
				throw;
			}
		}

		[Test]
		[Ignore("TODO")]
		public void ProcessAsyncFails() { }

		[Test]
		[Ignore("Need to fix relay for void method")]
		public void ProcessAsyncSucceeds()
		{
			StubServiceHost.FindDefaultService<IPerformServiceSync>()
				.Setup(s => s.Process(It.IsAny<XlangCalculatorRequest>()));

			IPerformServiceSync client = null;
			try
			{
				var request = new XlangCalculatorRequest(CALCULATOR_REQUEST_XML);
				client = Client<IPerformServiceSync>.Create(SimpleServiceHost<CalculatorService, IPerformService>.Endpoint);
				client.Process(request);
				client.Close();
			}
			catch (Exception)
			{
				if (client != null) client.Abort();
				throw;
			}
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
