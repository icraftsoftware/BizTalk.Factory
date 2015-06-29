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

using System;
using Be.Stateless.BizTalk.ContextProperties;
using Microsoft.BizTalk.B2B.PartnerManagement;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Subscription
{
	[TestFixture]
	public class FilterFixture
	{
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
		public void NonMessageContextPropertyBasedFilterIsNotSupported()
		{
			var filter = new Filter(() => GetType().Name == "any value");

			Assert.That(
				() => filter.ToString(),
				Throws.TypeOf<NotSupportedException>()
					.With.Message.EqualTo(
						"Cannot translate MemberExpression \"value(Be.Stateless.BizTalk.Dsl.Binding.Subscription.FilterFixture).GetType().Name\" because MemberAccess node is not supported."));
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
	}
}
