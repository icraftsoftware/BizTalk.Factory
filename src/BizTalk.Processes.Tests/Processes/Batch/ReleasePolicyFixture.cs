#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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

using System.Linq;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Tracking;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Processes.Batch
{
	[TestFixture]
	public class ReleasePolicyFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			BatchAdapter.RegisterBatch(_envelopeSpecName, null, true, 3);
			BatchAdapter.RegisterBatch(_envelopeSpecName, "p-one", false, 7);
		}

		[TearDown]
		public void TearDown()
		{
			BatchAdapter.ClearParts();
			BatchAdapter.UnregisterBatch(_envelopeSpecName, "p-one");
			BatchAdapter.UnregisterBatch(_envelopeSpecName, null);
		}

		#endregion

		[Test]
		public void BatchReleaseIsRuledByDefaultPartitionPolicy()
		{
			// insert parts for an envelope that does not have a partition-specific release policy
			BatchAdapter.AddPart(_envelopeSpecName, "partition-two", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "partition-two", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");

			var sut = BatchAdapter.ReleasePolicies.Single();

			Assert.That(sut.EnvelopeId, Is.EqualTo(BatchAdapter.Envelopes.Single(e => e.SpecName == _envelopeSpecName).Id));
			Assert.That(sut.Partition, Is.EqualTo("partition-two"));
			Assert.That(sut.Enabled, Is.True);
			Assert.That(sut.ReleaseOnItemCount, Is.EqualTo(3));
		}

		[Test]
		public void BatchReleaseIsRuledBySpecificPartitionPolicy()
		{
			// insert parts for an envelope that does have a partition-specific release policy
			BatchAdapter.AddPart(_envelopeSpecName, "p-one", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");
			BatchAdapter.AddPart(_envelopeSpecName, "p-one", ActivityId.NewActivityId(), "<data>some-partitioned-value</data>");

			var sut = BatchAdapter.ReleasePolicies.Single();

			Assert.That(sut.EnvelopeId, Is.EqualTo(BatchAdapter.Envelopes.Single(e => e.SpecName == _envelopeSpecName).Id));
			Assert.That(sut.Partition, Is.EqualTo("p-one"));
			Assert.That(sut.Enabled, Is.False);
			Assert.That(sut.ReleaseOnItemCount, Is.EqualTo(7));
		}

		private static readonly string _envelopeSpecName = new SchemaMetadata<Envelope>().DocumentSpec.DocSpecStrongName;
	}
}
