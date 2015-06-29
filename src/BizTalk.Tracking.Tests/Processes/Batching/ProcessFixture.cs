#region Copyright & License

// Copyright © 2013 François Chabot, Yves Dierick
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

using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BTS;
using Be.Stateless.BizTalk.BizMock.Extensions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Explorer;
using Be.Stateless.BizTalk.Pipelines;
using Be.Stateless.BizTalk.Tracking;
using BizMock;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Processes.Batching
{
	[TestFixture]
	public class ProcessFixture : Unit.Process.ProcessFixture
	{
		#region Setup/Teardown

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			// register the batch's EnvelopeSpecName
			using (var cnx = TransientStateDbConnection)
			{
				cnx.Open();
				var cmd = new SqlCommand("usp_bat_RegisterBatch", cnx);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add(new SqlParameter("@envelopeSpecName", Schema<soap_envelope_1__1.Envelope>.AssemblyQualifiedName));
				cmd.ExecuteNonQuery();
			}

			// setup BizTalk Artifacts
			_application = BizTalkServerGroup.Applications["BizTalk.Factory"];
			_xmlReceiveLocation = _application
				.CreateReceiveLocation<XmlReceive>("BizTalk.Factory.RL1.Generic.BizMock.XML", 60);
			_application.ApplyChanges();
		}

		[SetUp]
		public void SetUp()
		{
			BizMockery.Reset(true);
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			//// unregister the batch's EnvelopeSpecName
			//using (var cnx = TransientStateDbConnection)
			//{
			//   cnx.Open();
			//   var cmd = new SqlCommand("usp_bat_UnregisterBatch", cnx);
			//   cmd.CommandType = CommandType.StoredProcedure;
			//   cmd.Parameters.Add(new SqlParameter("@envelopeSpecName", Schema<soap_envelope_1__1.Envelope>.AssemblyQualifiedName));
			//   cmd.ExecuteNonQuery();
			//}

			//// clean up BizTalk Artifacts
			//_xmlReceiveLocation.Delete();
			//_application.ApplyChanges();
		}

		#endregion

		[Test]
		public void AddParts()
		{
			const string filePath = @"Messages\btf2_services_header.xml";

			for (var i = 0; i < 2; i++)
			{
				var interchangeID = Activity.NewActivityId();
				var message = new BizMockMessage(filePath);
				message.Promote(BizTalkFactoryProperties.EnvelopeSpecName, Schema<soap_envelope_1__1.Envelope>.AssemblyQualifiedName);
				message.Promote(BtsProperties.InterchangeID, interchangeID);

				Submit.Request(message).To(_xmlReceiveLocation);

				var process = TrackingDatabase.SingleProcess(p => p.InterchangeID == interchangeID && p.Name == Factory.Processes.Batching.Aggregator);
				Assert.That(process.MessagingSteps.Count, Is.EqualTo(2));

				var inboundMessagingStep = process.MessagingSteps.Single(ms => ms.Status == TrackingStatus.Received);
				Assert.That(inboundMessagingStep.Name, Is.EqualTo(_xmlReceiveLocation.Name));

				var outboundMessagingStep = process.MessagingSteps.Single(ms => ms.Status == TrackingStatus.Sent);
				Assert.That(outboundMessagingStep.Name, Is.EqualTo("BizTalk.Factory.SP1.Batching.AddPart.WCF-SQL"));

				// TODO find the right way: following line should not be there
				BizMockery.Reset(true);
			}
		}

		[Test]
		public void ReleaseBatch()
		{
			const string filePath = @"Messages\ReleaseBatchContent.xml";

			var interchangeID = Activity.NewActivityId();
			var message = new BizMockMessage(filePath);
			message.Promote(BtsProperties.InterchangeID, interchangeID);

			Submit.Request(message).To(_xmlReceiveLocation);

			var process = TrackingDatabase.SingleProcess(p => p.InterchangeID == interchangeID && p.Name == Factory.Processes.Batching.Releaser);
			Assert.That(process.MessagingSteps.Count, Is.GreaterThan(2));

			var inboundMessagingStep = process.MessagingSteps.Single(ms => ms.Status == TrackingStatus.Received);
			Assert.That(inboundMessagingStep.Name, Is.EqualTo(_xmlReceiveLocation.Name));

			Assert.That(process.MessagingSteps.All(ms => ms.Status == TrackingStatus.Sent && ms.Name == "BizTalk.Factory.SP2.Batching.ReleaseBatch.WCF-SQL"));
		}

		private SqlConnection TransientStateDbConnection
		{
			get
			{
				var builder = new SqlConnectionStringBuilder {
					DataSource = "localhost",
					InitialCatalog = "BizTalkFactoryTransientStateDb",
					IntegratedSecurity = true
				};
				return new SqlConnection(builder.ConnectionString);
			}
		}

		private Application _application;
		private ReceiveLocation _xmlReceiveLocation;
	}
}
