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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Be.Stateless.BizTalk.Explorer;
using Be.Stateless.BizTalk.Factory.Areas;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.BizTalk.Unit.Constraints;
using Be.Stateless.BizTalk.Unit.Process;
using Be.Stateless.IO.Extensions;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Processes.Batch
{
	[TestFixture]
	public class ReleaseProcessFixture : ProcessFixture
	{
		#region Setup/Teardown

		[OneTimeSetUp]
		public void TestFixtureSetUp()
		{
			BatchAdapter.RegisterBatch(_envelopeSpecName, null, true, 3);
			BatchAdapter.RegisterBatch(_envelopeSpecName, "p-one", true, 3);
			BatchAdapter.RegisterBatch(_envelopeSpecName, "p-two", true, 3);
			BatchAdapter.RegisterBatch(_envelopeSpecName, "p-count-limit", true, 3, true);
			BatchAdapter.RegisterBatch("e-one", null, true, 10);
			BatchAdapter.RegisterBatch("e-one", "p-one", true, 10);
			BatchAdapter.RegisterBatch("e-one", "p-two", true, 10);
		}

		[SetUp]
		public void Setup()
		{
			// ensure there is no race condition on BatchAdapter.QueuedControlledReleases assertions and speed up unit
			// tests by preventing batches from being actually released when testing the combinations of control messages
			BatchReleasePort.Disable();
		}

		[TearDown]
		public void TearDown()
		{
			// ensure the batch is enabled
			BatchAdapter.RegisterBatch(_envelopeSpecName, null, true, 3);

			BatchAdapter.ClearQueuedReleases();
			BatchAdapter.ClearParts();
		}

		[OneTimeTearDown]
		public void TestFixtureTearDown()
		{
			BatchAdapter.UnregisterBatch("e-one", "p-two");
			BatchAdapter.UnregisterBatch("e-one", "p-one");
			BatchAdapter.UnregisterBatch("e-one", null);
			BatchAdapter.UnregisterBatch(_envelopeSpecName, "p-count-limit");
			BatchAdapter.UnregisterBatch(_envelopeSpecName, "p-two");
			BatchAdapter.UnregisterBatch(_envelopeSpecName, "p-one");
			BatchAdapter.UnregisterBatch(_envelopeSpecName, null);
		}

		#endregion

		// TODO troubleshoot deadlock AddPart and ReleaseBatch

		[Test]
		public void ControlReleaseAllEnvelopesAndAllPartitions()
		{
			BatchAdapter.AddPart(_envelopeSpecName, null, ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "p-one", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "p-two", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "p-six", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");

			BatchAdapter.AddPart("e-one", null, ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart("e-one", "p-one", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart("e-one", "p-two", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart("e-one", "p-six", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");

			BatchAdapter.CreateReleaseMessage("*", "*").DropToFolder(DropFolders.INPUT_FOLDER, "release_batch.xml");

			// batch controlled release process
			var process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Areas.Batch.Processes.Release
					&& p.BeginTime > StartTime
					&& p.Status == TrackingStatus.Completed
					&& p.Value1 == "*"
					&& p.Value2 == "*");
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.UnitTest.InputMessage.FILE.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Batch.Release>().MessageType
					&& s.Status == TrackingStatus.Received
					&& s.Value1 == "*"
					&& s.Value2 == "*");
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.Batch.QueueControlledRelease.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Batch.Release>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& s.Value1 == "*"
					&& s.Value2 == "*");

			var queuedControlledReleases = BatchAdapter.QueuedControlledReleases.ToArray();
			Assert.That(
				queuedControlledReleases.Count(),
				Is.EqualTo(8));
			Assert.That(
				queuedControlledReleases.Select(qcr => qcr.EnvelopeId).Distinct(),
				Is.EquivalentTo(BatchAdapter.Envelopes.Where(e => e.SpecName == _envelopeSpecName || e.SpecName == "e-one").Select(e => e.Id)));
			Assert.That(
				queuedControlledReleases.Select(qcr => qcr.Partition).Distinct(),
				Is.EquivalentTo(new[] { "0", "p-one", "p-two", "p-six" }));

			Assert.That(BizTalkServiceInstances, Has.No.UncompletedInstances());
		}

		[Test]
		public void ControlReleaseAllEnvelopesAndOnePartitions()
		{
			BatchAdapter.AddPart(_envelopeSpecName, null, ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "p-one", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "p-two", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "p-six", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");

			BatchAdapter.AddPart("e-one", null, ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart("e-one", "p-one", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart("e-one", "p-two", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart("e-one", "p-six", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");

			BatchAdapter.CreateReleaseMessage("*", "p-one").DropToFolder(DropFolders.INPUT_FOLDER, "release_batch.xml");

			// batch controlled release process
			var process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Areas.Batch.Processes.Release
					&& p.BeginTime > StartTime
					&& p.Status == TrackingStatus.Completed
					&& p.Value1 == "*"
					&& p.Value2 == "p-one");
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.UnitTest.InputMessage.FILE.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Batch.Release>().MessageType
					&& s.Status == TrackingStatus.Received
					&& s.Value1 == "*"
					&& s.Value2 == "p-one");
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.Batch.QueueControlledRelease.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Batch.Release>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& s.Value1 == "*"
					&& s.Value2 == "p-one");

			var queuedControlledReleases = BatchAdapter.QueuedControlledReleases.ToArray();
			Assert.That(
				queuedControlledReleases.Count(),
				Is.EqualTo(2));
			Assert.That(
				queuedControlledReleases.Select(qcr => qcr.EnvelopeId),
				Is.EquivalentTo(BatchAdapter.Envelopes.Where(e => e.SpecName == _envelopeSpecName || e.SpecName == "e-one").Select(e => e.Id)));
			Assert.That(
				queuedControlledReleases.Select(qcr => qcr.Partition).Distinct().Single(),
				Is.EqualTo("p-one"));

			Assert.That(BizTalkServiceInstances, Has.No.UncompletedInstances());
		}

		[Test]
		public void ControlReleaseOneEnvelopeAndAllPartitions()
		{
			BatchAdapter.AddPart(_envelopeSpecName, null, ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "p-one", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "p-two", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "p-six", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");

			BatchAdapter.AddPart("e-one", null, ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart("e-one", "p-one", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart("e-one", "p-two", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart("e-one", "p-six", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");

			BatchAdapter.CreateReleaseMessage(_envelopeSpecName, "*").DropToFolder(DropFolders.INPUT_FOLDER, "release_batch.xml");

			// batch controlled release process
			var process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Areas.Batch.Processes.Release
					&& p.BeginTime > StartTime
					&& p.Status == TrackingStatus.Completed
					&& p.Status == TrackingStatus.Completed
					&& _envelopeSpecName.StartsWith(p.Value1)
					&& p.Value2 == "*");
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.UnitTest.InputMessage.FILE.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Batch.Release>().MessageType
					&& s.Status == TrackingStatus.Received
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "*");
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.Batch.QueueControlledRelease.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Batch.Release>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "*");

			var queuedControlledReleases = BatchAdapter.QueuedControlledReleases.ToArray();
			Assert.That(
				queuedControlledReleases.Count(),
				Is.EqualTo(4));
			Assert.That(
				queuedControlledReleases.Select(qcr => qcr.EnvelopeId).Distinct().Single(),
				Is.EqualTo(BatchAdapter.Envelopes.Single(e => e.SpecName == _envelopeSpecName).Id));
			Assert.That(
				queuedControlledReleases.Select(qcr => qcr.Partition),
				Is.EquivalentTo(new[] { "0", "p-one", "p-two", "p-six" }));

			Assert.That(BizTalkServiceInstances, Has.No.UncompletedInstances());
		}

		[Test]
		public void ControlReleaseOneEnvelopeAndNoPartition()
		{
			BatchAdapter.AddPart(_envelopeSpecName, null, ActivityId.NewActivityId(), "<data>some-value</data>");
			BatchAdapter.CreateReleaseMessage(_envelopeSpecName, null).DropToFolder(DropFolders.INPUT_FOLDER, "release_batch.xml");

			// batch controlled release process
			var process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Areas.Batch.Processes.Release
					&& p.BeginTime > StartTime
					&& p.Status == TrackingStatus.Completed
					&& _envelopeSpecName.StartsWith(p.Value1));
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.UnitTest.InputMessage.FILE.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Batch.Release>().MessageType
					&& s.Status == TrackingStatus.Received
					&& _envelopeSpecName.StartsWith(s.Value1));
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.Batch.QueueControlledRelease.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Batch.Release>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1));

			Assert.That(BatchAdapter.QueuedControlledReleases.Count(), Is.EqualTo(1));
			BatchReleasePort.Enable();

			// batch content handling process
			process = TrackingRepository.SingleProcess(
				p => p.Name == Default.Processes.Unidentified
					&& p.BeginTime > StartTime
					&& p.Status == TrackingStatus.Completed
					&& _envelopeSpecName.StartsWith(p.Value1)
					&& p.Value2 == null); // partition 0 did not leak out of the database
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.Batch.Release.WCF-SQL.XML"
					// TODO && s.MessageType == new SchemaMetadata<BatchContent>().MessageType
					&& s.Status == TrackingStatus.Received);
			var envelopeMessagingStep = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.UnitTest.Batch.Trace.FILE.XML"
					&& s.Status == TrackingStatus.Sent
					&& s.MessageType == new SchemaMetadata<Envelope>().MessageType
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == null); // partition 0 did not leak out of the database

			Assert.That(
				envelopeMessagingStep.Message.Body,
				Is.EqualTo("<ns0:Envelope xmlns:ns0=\"urn:schemas.stateless.be:biztalk:envelope:2013:07\"><data>some-value</data></ns0:Envelope>"));

			Assert.That(BizTalkServiceInstances, Has.No.UncompletedInstances());
		}

		[Test]
		public void ControlReleaseOneEnvelopeAndOnePartition()
		{
			BatchAdapter.AddPart(_envelopeSpecName, "partition-z", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.CreateReleaseMessage(_envelopeSpecName, "partition-z").DropToFolder(DropFolders.INPUT_FOLDER, "release_batch.xml");

			// batch controlled release process
			var process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Areas.Batch.Processes.Release
					&& p.BeginTime > StartTime
					&& p.Status == TrackingStatus.Completed
					&& _envelopeSpecName.StartsWith(p.Value1)
					&& p.Value2 == "partition-z");
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.UnitTest.InputMessage.FILE.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Batch.Release>().MessageType
					&& s.Status == TrackingStatus.Received
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "partition-z");
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.Batch.QueueControlledRelease.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Batch.Release>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "partition-z");

			Assert.That(BatchAdapter.QueuedControlledReleases.Count(), Is.EqualTo(1));
			BatchReleasePort.Enable();

			// batch content handling process
			process = TrackingRepository.SingleProcess(
				p => p.Name == Default.Processes.Unidentified
					&& p.BeginTime > StartTime
					&& p.Status == TrackingStatus.Completed
					&& _envelopeSpecName.StartsWith(p.Value1)
					&& p.Value2 == "partition-z");
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.Batch.Release.WCF-SQL.XML"
					// TODO && s.MessageType == new SchemaMetadata<BatchContent>().MessageType
					&& s.Status == TrackingStatus.Received);
			var envelopeMessagingStep = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.UnitTest.Batch.Trace.FILE.XML"
					&& s.Status == TrackingStatus.Sent
					&& s.MessageType == new SchemaMetadata<Envelope>().MessageType
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "partition-z");

			Assert.That(
				envelopeMessagingStep.Message.Body,
				Is.EqualTo("<ns0:Envelope xmlns:ns0=\"urn:schemas.stateless.be:biztalk:envelope:2013:07\"><data>some-partitioned-value</data></ns0:Envelope>"));

			Assert.That(BizTalkServiceInstances, Has.No.UncompletedInstances());
		}

		[Test]
		public void ControlReleaseSkippedWhenBatchHasNoParts()
		{
			BatchAdapter.CreateReleaseMessage(_envelopeSpecName, null).DropToFolder(DropFolders.INPUT_FOLDER, "release_batch.xml");

			// batch controlled release process
			var process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Areas.Batch.Processes.Release
					&& p.BeginTime > StartTime
					&& p.Status == TrackingStatus.Completed);
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.UnitTest.InputMessage.FILE.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Batch.Release>().MessageType
					&& s.Status == TrackingStatus.Received
					&& _envelopeSpecName.StartsWith(s.Value1));
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.Batch.QueueControlledRelease.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Batch.Release>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1));

			Assert.That(BatchAdapter.QueuedControlledReleases.Count(), Is.EqualTo(0));

			Assert.That(BizTalkServiceInstances, Has.No.UncompletedInstances());
		}

		[Test]
		public void ControlReleaseSkippedWhenBatchIsDisabled()
		{
			// register the batch again so as to disable it
			BatchAdapter.RegisterBatch(_envelopeSpecName, null, false, 3);

			BatchAdapter.AddPart(_envelopeSpecName, null, ActivityId.NewActivityId(), "<data>some-value</data>");
			BatchAdapter.CreateReleaseMessage(_envelopeSpecName, null).DropToFolder(DropFolders.INPUT_FOLDER, "release_batch.xml");

			// batch controlled release process
			var process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Areas.Batch.Processes.Release
					&& p.BeginTime > StartTime
					&& p.Status == TrackingStatus.Completed);
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.UnitTest.InputMessage.FILE.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Batch.Release>().MessageType
					&& s.Status == TrackingStatus.Received
					&& _envelopeSpecName.StartsWith(s.Value1));
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.Batch.QueueControlledRelease.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Batch.Release>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1));

			Assert.That(BatchAdapter.QueuedControlledReleases.Count(), Is.EqualTo(0));

			Assert.That(BizTalkServiceInstances, Has.No.UncompletedInstances());
		}

		[Test]
		public void PollForAvailableBatchesToRelease()
		{
			// create three-item partitioned batches, so as to trigger an automatic release via polling receive-location
			BatchAdapter.AddPart(_envelopeSpecName, "one", ActivityId.NewActivityId(), "<data>some-one-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "one", ActivityId.NewActivityId(), "<data>some-one-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "one", ActivityId.NewActivityId(), "<data>some-one-partitioned-value</data>");

			BatchAdapter.AddPart(_envelopeSpecName, "two", ActivityId.NewActivityId(), "<data>some-two-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "two", ActivityId.NewActivityId(), "<data>some-two-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "two", ActivityId.NewActivityId(), "<data>some-two-partitioned-value</data>");

			BatchAdapter.AddPart(_envelopeSpecName, "six", ActivityId.NewActivityId(), "<data>some-six-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "six", ActivityId.NewActivityId(), "<data>some-six-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "six", ActivityId.NewActivityId(), "<data>some-six-partitioned-value</data>");

			BatchReleasePort.Enable();

			// batch content handling processes
			TrackingRepository.SingleProcess(
				p => p.Name == Default.Processes.Unidentified
					&& p.BeginTime > StartTime
					&& p.Status == TrackingStatus.Completed
					&& _envelopeSpecName.StartsWith(p.Value1)
					&& p.Value2 == "one");
			TrackingRepository.SingleProcess(
				p => p.Name == Default.Processes.Unidentified
					&& p.BeginTime > StartTime
					&& p.Status == TrackingStatus.Completed
					&& _envelopeSpecName.StartsWith(p.Value1)
					&& p.Value2 == "two");
			TrackingRepository.SingleProcess(
				p => p.Name == Default.Processes.Unidentified
					&& p.BeginTime > StartTime
					&& p.Status == TrackingStatus.Completed
					&& _envelopeSpecName.StartsWith(p.Value1)
					&& p.Value2 == "six");

			Assert.That(BizTalkServiceInstances, Has.No.UncompletedInstances());
		}

		[Test]
		public void PollForAvailableBatchesWithItemCountSizeLimit()
		{
			// drop more items than twice the item count limit, which is 3, so as to release two batches
			for (var i = 0; i < 8; i++)
			{
				BatchAdapter.AddPart(_envelopeSpecName, "p-count-limit", ActivityId.NewActivityId(), string.Format("<data>count-limit-value-{0}</data>", i));
			}

			BatchReleasePort.Enable();

			// batch content handling processes
			var processesQuery = TrackingRepository.Processes.Where(
				p => p.Name == Default.Processes.Unidentified
					&& p.BeginTime > StartTime
					&& p.Status == TrackingStatus.Completed
					&& _envelopeSpecName.StartsWith(p.Value1)
					&& p.Value2 == "p-count-limit");

			// 2 processes have been tracked as more items than twice the item count limit have been accumulated
			// ReSharper disable PossibleMultipleEnumeration
			Assert.That(() => processesQuery.Count(), Is.EqualTo(2).After(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(1)));
			var processes = processesQuery.ToArray();
			// ReSharper restore PossibleMultipleEnumeration

			// ensure the 2 processes are issued from the same polling, thereby validating that all available batches are
			// released in one shot (i.e., in DEV, one within one second of the other, when polling every 5 seconds)
			Assert.That(processes[0].BeginTime, Is.EqualTo(processes[1].BeginTime).Within(TimeSpan.FromSeconds(1)));

			// each batch is made of 3 parts
			var envelopeMessage1 = processes[0].MessagingSteps.Single(
				s => s.Name == "BizTalk.Factory.SP1.UnitTest.Batch.Trace.FILE.XML"
					&& s.MessageType == new SchemaMetadata<Envelope>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "p-count-limit");
			Assert.That(Regex.Matches(envelopeMessage1.Message.Body, @"<data>count-limit-value-\d</data>").Count, Is.EqualTo(3));
			var envelopeMessage2 = processes[1].MessagingSteps.Single(
				s => s.Name == "BizTalk.Factory.SP1.UnitTest.Batch.Trace.FILE.XML"
					&& s.MessageType == new SchemaMetadata<Envelope>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "p-count-limit");
			Assert.That(Regex.Matches(envelopeMessage2.Message.Body, @"<data>count-limit-value-\d</data>").Count, Is.EqualTo(3));

			// 2 parts have been left in the database
			Assert.That(BatchAdapter.Parts.Count(), Is.EqualTo(2));

			Assert.That(BizTalkServiceInstances, Has.No.UncompletedInstances());
		}

		[Test]
		public void TrackBatchReleasedOnControlMessage()
		{
			BatchAdapter.CreatePartMessage(_envelopeSpecName, "partition-z").DropToFolder(DropFolders.INPUT_FOLDER, "part.xml.part");
			var process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Areas.Batch.Processes.Aggregate
					&& p.BeginTime > StartTime);
			var addPartMessage1 = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.Batch.AddPart.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Any>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "partition-z");

			BatchAdapter.CreatePartMessage(_envelopeSpecName, "partition-z").DropToFolder(DropFolders.INPUT_FOLDER, "part.xml.part");
			process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Areas.Batch.Processes.Aggregate
					// ReSharper disable once AccessToModifiedClosure
					&& p.BeginTime > process.EndTime);
			var addPartMessage2 = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.Batch.AddPart.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Any>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "partition-z");

			BatchAdapter.CreateReleaseMessage(_envelopeSpecName, "partition-z").DropToFolder(DropFolders.INPUT_FOLDER, "release_batch.xml");
			BatchReleasePort.Enable();

			// batch controlled release process
			process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Areas.Batch.Processes.Release
					&& p.BeginTime > StartTime
					&& p.Status == TrackingStatus.Completed
					&& _envelopeSpecName.StartsWith(p.Value1)
					&& p.Value2 == "partition-z");
			// control message
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.UnitTest.InputMessage.FILE.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Batch.Release>().MessageType
					&& s.Status == TrackingStatus.Received
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "partition-z");
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.Batch.QueueControlledRelease.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Batch.Release>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "partition-z");
			// 1st part
			process.SingleMessagingStep(
				s => s.ActivityID == addPartMessage1.ActivityID
					&& s.Name == "BizTalk.Factory.SP1.Batch.AddPart.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Any>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "partition-z");
			// 2nd part
			process.SingleMessagingStep(
				s => s.ActivityID == addPartMessage2.ActivityID
					&& s.Name == "BizTalk.Factory.SP1.Batch.AddPart.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Any>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "partition-z");
			// batch content
			var releaseProcessBatchMessagingStep = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.Batch.Release.WCF-SQL.XML"
					// TODO && s.MessageType == new SchemaMetadata<BatchContent>().MessageType
					&& s.Status == TrackingStatus.Received);

			// batch content handling process
			process = TrackingRepository.SingleProcess(
				p => p.Name == Default.Processes.Unidentified
					&& p.BeginTime > StartTime
					&& p.Status == TrackingStatus.Completed
					&& _envelopeSpecName.StartsWith(p.Value1)
					&& p.Value2 == "partition-z");
			var handlingProcessBatchMessagingStep = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.Batch.Release.WCF-SQL.XML"
					// TODO && s.MessageType == new SchemaMetadata<BatchContent>().MessageType
					&& s.Status == TrackingStatus.Received);

			Assert.That(releaseProcessBatchMessagingStep.ActivityID, Is.EqualTo(handlingProcessBatchMessagingStep.ActivityID));

			// a part is linked to both its aggregate and release processes
			Assert.That(addPartMessage1.Processes.Count(), Is.EqualTo(2));
			Assert.That(addPartMessage1.Processes.SingleOrDefault(p => p.Name == Factory.Areas.Batch.Processes.Aggregate), Is.Not.Null);
			Assert.That(addPartMessage1.Processes.SingleOrDefault(p => p.Name == Factory.Areas.Batch.Processes.Release), Is.Not.Null);

			// a batch is linked to both its release and handling processes
			Assert.That(releaseProcessBatchMessagingStep.Processes.Count(), Is.EqualTo(2));
			Assert.That(releaseProcessBatchMessagingStep.Processes.SingleOrDefault(p => p.Name == Factory.Areas.Batch.Processes.Release), Is.Not.Null);
			Assert.That(releaseProcessBatchMessagingStep.Processes.SingleOrDefault(p => p.Name == Default.Processes.Unidentified), Is.Not.Null);

			Assert.That(BizTalkServiceInstances, Has.No.UncompletedInstances());
		}

		[Test]
		public void TrackBatchReleasedOnPolling()
		{
			BatchAdapter.CreatePartMessage(_envelopeSpecName, "partition-z").DropToFolder(DropFolders.INPUT_FOLDER, "part.xml.part");
			var process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Areas.Batch.Processes.Aggregate
					&& p.BeginTime > StartTime);
			var addPartMessage1 = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.Batch.AddPart.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Any>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "partition-z");

			BatchAdapter.CreatePartMessage(_envelopeSpecName, "partition-z").DropToFolder(DropFolders.INPUT_FOLDER, "part.xml.part");
			process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Areas.Batch.Processes.Aggregate
					// ReSharper disable once AccessToModifiedClosure
					&& p.BeginTime > process.EndTime);
			var addPartMessage2 = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.Batch.AddPart.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Any>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "partition-z");

			BatchAdapter.CreatePartMessage(_envelopeSpecName, "partition-z").DropToFolder(DropFolders.INPUT_FOLDER, "part.xml.part");
			process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Areas.Batch.Processes.Aggregate
					// ReSharper disable once AccessToModifiedClosure
					&& p.BeginTime > process.EndTime);
			var addPartMessage3 = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.Batch.AddPart.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Any>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "partition-z");

			BatchReleasePort.Enable();

			// batch controlled release process
			process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Areas.Batch.Processes.Release
					&& p.BeginTime > StartTime
					&& p.Status == TrackingStatus.Completed);
			// 1st part
			process.SingleMessagingStep(
				s => s.ActivityID == addPartMessage1.ActivityID
					&& s.Name == "BizTalk.Factory.SP1.Batch.AddPart.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Any>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "partition-z");
			// 2nd part
			process.SingleMessagingStep(
				s => s.ActivityID == addPartMessage2.ActivityID
					&& s.Name == "BizTalk.Factory.SP1.Batch.AddPart.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Any>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "partition-z");
			// 3rd part
			process.SingleMessagingStep(
				s => s.ActivityID == addPartMessage3.ActivityID
					&& s.Name == "BizTalk.Factory.SP1.Batch.AddPart.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Any>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "partition-z");
			// batch content
			var releaseProcessBatchMessagingStep = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.Batch.Release.WCF-SQL.XML"
					// TODO && s.MessageType == new SchemaMetadata<BatchContent>().MessageType
					&& s.Status == TrackingStatus.Received);

			// batch content handling process
			process = TrackingRepository.SingleProcess(
				p => p.Name == Default.Processes.Unidentified
					&& p.BeginTime > StartTime
					&& p.Status == TrackingStatus.Completed
					&& _envelopeSpecName.StartsWith(p.Value1)
					&& p.Value2 == "partition-z");
			var handlingProcessBatchMessagingStep = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.Batch.Release.WCF-SQL.XML"
					// TODO && s.MessageType == new SchemaMetadata<BatchContent>().MessageType
					&& s.Status == TrackingStatus.Received);

			Assert.That(releaseProcessBatchMessagingStep.ActivityID, Is.EqualTo(handlingProcessBatchMessagingStep.ActivityID));

			// a part is linked to both its aggregate and release processes
			Assert.That(addPartMessage1.Processes.Count(), Is.EqualTo(2));
			Assert.That(addPartMessage1.Processes.SingleOrDefault(p => p.Name == Factory.Areas.Batch.Processes.Aggregate), Is.Not.Null);
			Assert.That(addPartMessage1.Processes.SingleOrDefault(p => p.Name == Factory.Areas.Batch.Processes.Release), Is.Not.Null);

			// a batch is linked to both its release and handling processes
			Assert.That(releaseProcessBatchMessagingStep.Processes.Count(), Is.EqualTo(2));
			Assert.That(releaseProcessBatchMessagingStep.Processes.SingleOrDefault(p => p.Name == Factory.Areas.Batch.Processes.Release), Is.Not.Null);
			Assert.That(releaseProcessBatchMessagingStep.Processes.SingleOrDefault(p => p.Name == Default.Processes.Unidentified), Is.Not.Null);

			Assert.That(BizTalkServiceInstances, Has.No.UncompletedInstances());
		}

		[Test]
		public void TrackBatchReleasedOnPollingWhenNoMessagingStepActivityIds()
		{
			BatchAdapter.AddPart(_envelopeSpecName, "partition-z", null, "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "partition-z", null, "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "partition-z", null, "<data>some-partitioned-value</data>");

			BatchReleasePort.Enable();

			// batch content handling process
			var process = TrackingRepository.SingleProcess(
				p => p.Name == Default.Processes.Unidentified
					&& p.BeginTime > StartTime
					&& p.Status == TrackingStatus.Completed
					&& _envelopeSpecName.StartsWith(p.Value1)
					&& p.Value2 == "partition-z");
			var releaseProcessBatchMessagingStep = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.Batch.Release.WCF-SQL.XML"
					// TODO && s.MessageType == new SchemaMetadata<BatchContent>().MessageType
					&& s.Status == TrackingStatus.Received);
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.UnitTest.Batch.Trace.FILE.XML"
					&& s.Status == TrackingStatus.Sent
					&& s.MessageType == new SchemaMetadata<Envelope>().MessageType
					&& _envelopeSpecName.StartsWith(s.Value1)
					&& s.Value2 == "partition-z");

			// no batch release process has been created as no parts provide a MessagingStepActivityId that could be used to link a part to its batch
			Assert.That(releaseProcessBatchMessagingStep.Processes.Count(p => p.Name == Factory.Areas.Batch.Processes.Release && p.BeginTime > StartTime), Is.EqualTo(0));

			Assert.That(BizTalkServiceInstances, Has.No.UncompletedInstances());
		}

		protected override IEnumerable<string> InputFolders
		{
			get { return new[] { DropFolders.INPUT_FOLDER }; }
		}

		protected override IEnumerable<string> OutputFolders
		{
			get { return new[] { DropFolders.TRACE_FOLDER }; }
		}

		private static class BatchReleasePort
		{
			public static void Enable()
			{
				var application = BizTalkServerGroup.Applications["BizTalk.Factory"];
				var receiveLocation = application.ReceivePorts["BizTalk.Factory.RP1.Batch"].ReceiveLocations["BizTalk.Factory.RL1.Batch.Release.WCF-SQL.XML"];
				receiveLocation.Enable();
				application.ApplyChanges();
			}

			public static void Disable()
			{
				var application = BizTalkServerGroup.Applications["BizTalk.Factory"];
				var receiveLocation = application.ReceivePorts["BizTalk.Factory.RP1.Batch"].ReceiveLocations["BizTalk.Factory.RL1.Batch.Release.WCF-SQL.XML"];
				receiveLocation.Disable();
				application.ApplyChanges();
			}
		}

		private static readonly string _envelopeSpecName = new SchemaMetadata<Envelope>().DocumentSpec.DocSpecStrongName;
	}
}
