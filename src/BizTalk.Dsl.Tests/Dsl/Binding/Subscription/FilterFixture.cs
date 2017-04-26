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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Install;
using Be.Stateless.BizTalk.Pipelines;
using Microsoft.BizTalk.B2B.PartnerManagement;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Subscription
{
	[TestFixture]
	public class FilterFixture
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
		public void ConjunctionOfDisjunctionsOfFiltersIsNotSupported()
		{
			const string token1 = "BizTalkFactory.Batcher";
			const int token2 = 3;

			var filter = new Filter(
				() => (BizTalkFactoryProperties.SenderName == token1 || BtsProperties.ActualRetryCount > token2)
					&& BtsProperties.MessageType == Schema<Schemas.Xml.Batch.Content>.MessageType);

			Assert.That(
				() => filter.ToString(),
				Throws.TypeOf<NotSupportedException>()
					.With.Message.EqualTo(
						"Cannot translate FilterStatement \"((BizTalkFactoryProperties.SenderName == \"BizTalkFactory.Batcher\") OrElse (BtsProperties.ActualRetryCount > 3))\" because OrElse node is not supported."));
		}

		[Test]
		public void ConjunctionOfFilters()
		{
			const string senderNameToken = "BizTalkFactory.Batcher";
			const int retryCountToken = 3;
			var filter = new Filter(() => BizTalkFactoryProperties.SenderName == senderNameToken && BtsProperties.ActualRetryCount > retryCountToken);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /><Statement Property=\"{3}\" Operator=\"{4}\" Value=\"{5}\" /></Group></Filter>",
						BizTalkFactoryProperties.SenderName.Type.FullName,
						(int) FilterOperator.Equals,
						senderNameToken,
						BtsProperties.ActualRetryCount.Type.FullName,
						(int) FilterOperator.GreaterThan,
						retryCountToken)));
		}

		[Test]
		public void ConstantFilterIsNotSupported()
		{
			var filter = new Filter(() => false);

			Assert.That(
				() => filter.ToString(),
				Throws.TypeOf<NotSupportedException>()
					.With.Message.EqualTo("Cannot translate FilterPredicate \"False\" because Constant node is not supported."));
		}

		[Test]
		public void DisjunctionOfConjunctionsOfFilters()
		{
			const string token1 = "BizTalkFactory.Batcher";
			const int token2 = 3;

			var filter = new Filter(
				() => BizTalkFactoryProperties.SenderName == token1
					|| (BtsProperties.ActualRetryCount > token2
						&& BtsProperties.MessageType == Schema<Schemas.Xml.Batch.Content>.MessageType));

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group>" +
							"<Group><Statement Property=\"{3}\" Operator=\"{4}\" Value=\"{5}\" />" +
							"<Statement Property=\"{6}\" Operator=\"{7}\" Value=\"{8}\" /></Group></Filter>",
						BizTalkFactoryProperties.SenderName.Type.FullName,
						(int) FilterOperator.Equals,
						token1,
						BtsProperties.ActualRetryCount.Type.FullName,
						(int) FilterOperator.GreaterThan,
						token2,
						BtsProperties.MessageType.Type.FullName,
						(int) FilterOperator.Equals,
						Schema<Schemas.Xml.Batch.Content>.MessageType)));
		}

		[Test]
		public void DisjunctionOfFilters()
		{
			const string senderNameToken = "BizTalkFactory.Batcher";
			const int retryCountToken = 3;
			var filter = new Filter(() => BizTalkFactoryProperties.SenderName == senderNameToken || BtsProperties.ActualRetryCount > retryCountToken);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group><Group><Statement Property=\"{3}\" Operator=\"{4}\" Value=\"{5}\" /></Group></Filter>",
						BizTalkFactoryProperties.SenderName.Type.FullName,
						(int) FilterOperator.Equals,
						senderNameToken,
						BtsProperties.ActualRetryCount.Type.FullName,
						(int) FilterOperator.GreaterThan,
						retryCountToken)));
		}

		[Test]
		public void EqualsBasedFilter()
		{
			const string senderNameToken = "BizTalkFactory.Batcher";
			var filter = new Filter(() => BizTalkFactoryProperties.SenderName == senderNameToken);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BizTalkFactoryProperties.SenderName.Type.FullName,
						(int) FilterOperator.Equals,
						senderNameToken)));
		}

		[Test]
		public void EqualsNullBasedFilterIsNotSupported()
		{
			var filter = new Filter(() => BizTalkFactoryProperties.SenderName == null);

			Assert.That(
				() => filter.ToString(),
				Throws.TypeOf<NotSupportedException>()
					.With.Message.EqualTo(
						"Cannot translate FilterPredicate \"() => (BizTalkFactoryProperties.SenderName == null)\" because filter value can be null only if the operator is exists.")
					.And.InnerException.TypeOf<TpmException>());
		}

		[Test]
		public void EqualsToReceivePortName()
		{
			var receivePort = new SampleApplicationBinding().TestReceivePort;
			var filter = new Filter(() => BtsProperties.ReceivePortName == receivePort.Name);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BtsProperties.ReceivePortName.Type.FullName,
						(int) FilterOperator.Equals,
						((ISupportNamingConvention) receivePort).Name)));
		}

		[Test]
		public void EqualsToSendPortName()
		{
			var sendPort = new SampleApplicationBinding().TestSendPort;
			var filter = new Filter(() => BtsProperties.SendPortName == sendPort.Name);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BtsProperties.SendPortName.Type.FullName,
						(int) FilterOperator.Equals,
						((ISupportNamingConvention) sendPort).Name)));
		}

		[Test]
		public void EqualToStringMember()
		{
			var filter = new Filter(() => BizTalkFactoryProperties.CorrelationToken == Factory.Areas.Default.Processes.Unidentified);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BizTalkFactoryProperties.CorrelationToken.Type.FullName,
						(int) FilterOperator.Equals,
						Factory.Areas.Default.Processes.Unidentified)));
		}

		[Test]
		public void EqualToStringMember2()
		{
			var sampleApplicationBinding = new SampleApplicationBinding();

			var filter = new Filter(() => BizTalkFactoryProperties.CorrelationToken == sampleApplicationBinding.Name);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BizTalkFactoryProperties.CorrelationToken.Type.FullName,
						(int) FilterOperator.Equals,
						sampleApplicationBinding.Name)));
		}

		[Test]
		public void GreaterThanBasedFilter()
		{
			const int retryCountToken = 3;
			var filter = new Filter(() => BtsProperties.ActualRetryCount > retryCountToken);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BtsProperties.ActualRetryCount.Type.FullName,
						(int) FilterOperator.GreaterThan,
						retryCountToken)));
		}

		[Test]
		public void GreaterThanNullBasedFilterIsNotSupported()
		{
			var filter = new Filter(() => BizTalkFactoryProperties.SenderName > null);

			Assert.That(
				() => filter.ToString(),
				Throws.TypeOf<NotSupportedException>()
					.With.Message.EqualTo(
						"Cannot translate FilterPredicate \"() => (BizTalkFactoryProperties.SenderName > null)\" because filter value can be null only if the operator is exists.")
					.And.InnerException.TypeOf<TpmException>());
		}

		[Test]
		public void GreaterThanOrEqualsBasedFilter()
		{
			const int retryCountToken = 3;
			var filter = new Filter(() => BtsProperties.ActualRetryCount >= retryCountToken);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BtsProperties.ActualRetryCount.Type.FullName,
						(int) FilterOperator.GreaterThanOrEquals,
						retryCountToken)));
		}

		[Test]
		public void LessThanBasedFilter()
		{
			const int retryCountToken = 3;
			var filter = new Filter(() => BtsProperties.ActualRetryCount < retryCountToken);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BtsProperties.ActualRetryCount.Type.FullName,
						(int) FilterOperator.LessThan,
						retryCountToken)));
		}

		[Test]
		public void LessThanOrEqualsBasedFilter()
		{
			const int retryCountToken = 3;
			var filter = new Filter(() => BtsProperties.ActualRetryCount <= retryCountToken);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BtsProperties.ActualRetryCount.Type.FullName,
						(int) FilterOperator.LessThanOrEquals,
						retryCountToken)));
		}

		[Test]
		public void MessageTypeBasedFilter()
		{
			var filter = new Filter(() => BtsProperties.MessageType == Schema<Schemas.Xml.Batch.Content>.MessageType);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BtsProperties.MessageType.Type.FullName,
						(int) FilterOperator.Equals,
						Schema<Schemas.Xml.Batch.Content>.MessageType)));
		}

		[Test]
		public void NAryConjunction()
		{
			var filter = new Filter(
				() => BtsProperties.ActualRetryCount > 3
					&& BtsProperties.MessageType == Schema<Schemas.Xml.Batch.Content>.MessageType
					&& BtsProperties.SendPortName == "Dummy port name"
					&& BtsProperties.IsRequestResponse != true);
			Assert.That(() => filter.ToString(), Throws.Nothing);
		}

		[Test]
		public void NAryDisjunction()
		{
			var filter = new Filter(
				() => BizTalkFactoryProperties.SenderName == "BizTalkFactory.Batcher"
					|| BtsProperties.ActualRetryCount > 3
					|| BtsProperties.AckRequired != true
					|| BtsProperties.InboundTransportLocation == "inbound-transport-location");
			Assert.That(() => filter.ToString(), Throws.Nothing);
		}

		[Test]
		public void NonMessageContextPropertyBasedFilterIsNotSupported()
		{
			var filter = new Filter(() => GetType().Name == "any value");

			Assert.That(
				() => filter.ToString(),
				Throws.TypeOf<NotSupportedException>()
					.With.Message.EqualTo(
						"Cannot translate property Expression \"value(Be.Stateless.BizTalk.Dsl.Binding.Subscription.FilterFixture).GetType().Name\" because only MessageContextProperty<T, TR>-derived type's member access expressions are supported."));
		}

		[Test]
		public void NotEqualsBasedFilter()
		{
			const string senderNameToken = "BizTalkFactory.Batcher";
			var filter = new Filter(() => BizTalkFactoryProperties.SenderName != senderNameToken);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BizTalkFactoryProperties.SenderName.Type.FullName,
						(int) FilterOperator.NotEqual,
						senderNameToken)));
		}

		[Test]
		public void NotEqualsNullBasedFilterIsRewrittenAsExistsOperator()
		{
			var filter = new Filter(() => BizTalkFactoryProperties.SenderName != null);

			Assert.That(
				filter.ToString(),
				Is.EqualTo(
					string.Format(
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" /></Group></Filter>",
						BizTalkFactoryProperties.SenderName.Type.FullName,
						(int) FilterOperator.Exists)));
		}

		private class SampleApplicationBinding : ApplicationBinding
		{
			public SampleApplicationBinding()
			{
				Name = "BizTalk.Factory";
				SendPorts.Add(TestSendPort);
				ReceivePorts.Add(TestReceivePort);
			}

			internal IReceivePort<string> TestReceivePort
			{
				get
				{
					return _receivePort ?? (_receivePort = ReceivePort(
						rp => {
							rp.Name = "BizTalk.Factory.RP1.Batch";
							rp.ReceiveLocations.Add(
								ReceiveLocation(
									rl => {
										rl.Name = "BizTalk.Factory.RP1.Batch.Release.FILE.XML";
										rl.ReceivePipeline = new ReceivePipeline<BatchReceive>();
										rl.Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\files\drops"; });
										rl.Transport.Host = "Host";
									}));
						}));
				}
			}

			internal ISendPort<string> TestSendPort
			{
				get
				{
					return _sendPort ?? (_sendPort = SendPort(
						sp => {
							sp.Name = "BizTalk.Factory.SP1.UnitTest.Batch.Trace.FILE.XML";
							sp.SendPipeline = new SendPipeline<PassThruTransmit>();
							sp.Transport.Adapter = new FileAdapter.Outbound(a => { a.DestinationFolder = @"C:\Files\Drops\BizTalk.Factory\Trace"; });
							sp.Transport.Host = "Host";
						}));
				}
			}

			private IReceivePort<string> _receivePort;

			private ISendPort<string> _sendPort;
		}
	}
}
