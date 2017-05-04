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
using System.Collections.Generic;
using System.Linq;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.BizTalk.Unit.Process;
using Be.Stateless.IO.Extensions;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Processes.Batch
{
	[TestFixture]
	public class AggregateProcessFixture : CompletedProcessFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			BatchAdapter.RegisterBatch(_envelopeSpecName, null, true, 3);
		}

		[TearDown]
		public void TearDown()
		{
			BatchAdapter.ClearParts();
			BatchAdapter.UnregisterBatch(_envelopeSpecName, null);
		}

		#endregion

		[Test]
		public void AddPartToBatch()
		{
			BatchAdapter.CreatePartMessage(_envelopeSpecName, null).DropToFolder(DropFolders.INPUT_FOLDER, "part.xml.part");

			var process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Areas.Batch.Processes.Aggregate
					&& p.BeginTime > StartTime);
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.UnitTest.Batch.AddPart.FILE.XML"
					&& s.MessageType == new SchemaMetadata<Any>().MessageType
					&& s.Status == TrackingStatus.Received
					&& _envelopeSpecName.StartsWith(s.Value1, StringComparison.Ordinal)
					&& s.Value3 == null);
			var addPartMessage = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.Batch.AddPart.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Any>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1, StringComparison.Ordinal)
					&& s.Value3 == null);

			var part = BatchAdapter.Parts.Single();
			Assert.That(part.MessagingStepActivityId, Is.EqualTo(addPartMessage.ActivityID));
			Assert.That(part.Partition, Is.EqualTo("0"));
		}

		[Test]
		public void AddPartToPartitionedBatch()
		{
			BatchAdapter.CreatePartMessage(_envelopeSpecName, "partition-z").DropToFolder(DropFolders.INPUT_FOLDER, "part.xml.part");

			var process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Areas.Batch.Processes.Aggregate
					&& p.BeginTime > StartTime
					&& p.Value3 == "partition-z");
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.UnitTest.Batch.AddPart.FILE.XML"
					&& s.MessageType == new SchemaMetadata<Any>().MessageType
					&& s.Status == TrackingStatus.Received
					&& _envelopeSpecName.StartsWith(s.Value1, StringComparison.Ordinal)
					&& s.Value3 == "partition-z");
			var addPartMessage = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.Batch.AddPart.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Any>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1, StringComparison.Ordinal)
					&& s.Value3 == "partition-z");

			var part = BatchAdapter.Parts.Single();
			Assert.That(part.MessagingStepActivityId, Is.EqualTo(addPartMessage.ActivityID));
			Assert.That(part.Partition, Is.EqualTo("partition-z"));
		}

		protected override IEnumerable<string> InputFolders
		{
			get { return new[] { DropFolders.INPUT_FOLDER }; }
		}

		private static readonly string _envelopeSpecName = typeof(Envelope).GetMetadata().DocumentSpec.DocSpecStrongName;
	}
}
