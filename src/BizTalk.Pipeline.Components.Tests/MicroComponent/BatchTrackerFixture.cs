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
using System.IO;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.BizTalk.Tracking.Messaging;
using Be.Stateless.BizTalk.Unit.Resources;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.MicroComponent
{
	[TestFixture]
	public class BatchTrackerFixture : ActivityTrackerFixture
	{
		#region Setup/Teardown

		[SetUp]
		public new void SetUp()
		{
			_dataStream = ResourceManager.Load("Data.BatchContent.xml");
			MessageMock.Object.BodyPart.Data = _dataStream;

			_batchReleaseProcessActivityTrackerFactory = BatchReleaseProcessActivityTracker.Factory;
		}

		[TearDown]
		public new void TearDown()
		{
			_dataStream.Dispose();
			BatchReleaseProcessActivityTracker.Factory = _batchReleaseProcessActivityTrackerFactory;
		}

		#endregion

		[Test]
		public void TrackBatchReleaseProcessActivity()
		{
			BatchReleaseProcessActivityTrackerMock = new Mock<BatchReleaseProcessActivityTracker>(PipelineContextMock.Object, MessageMock.Object);
			BatchReleaseProcessActivityTracker.Factory = (pipelineContext, message) => BatchReleaseProcessActivityTrackerMock.Object;

			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(Schema<Batch.Content>.MessageType))
				.Returns(Schema<Batch.Content>.DocumentSpec);
			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(Schema<Envelope>.MessageType))
				.Returns(Schema<Envelope>.DocumentSpec);

			var sut = CreateActivityTracker();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			BatchReleaseProcessActivityTrackerMock.Verify(brpat => brpat.TrackActivity(It.IsAny<BatchTrackingContext>()), Times.Once());

			MessageMock.Verify(m => m.SetProperty(TrackingProperties.Value1, new SchemaMetadata<Envelope>().DocumentSpec.DocSpecStrongName));
			MessageMock.Verify(m => m.SetProperty(TrackingProperties.Value2, "environment-tag"));
			MessageMock.Verify(m => m.SetProperty(TrackingProperties.Value3, "p-one"));
		}

		private Mock<BatchReleaseProcessActivityTracker> BatchReleaseProcessActivityTrackerMock { get; set; }

		private Func<IPipelineContext, IBaseMessage, BatchReleaseProcessActivityTracker> _batchReleaseProcessActivityTrackerFactory;
		private Stream _dataStream;

		protected override ActivityTracker CreateActivityTracker()
		{
			return new BatchTracker();
		}
	}
}
