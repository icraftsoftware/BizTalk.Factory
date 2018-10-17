#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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

using System.IO;
using System.Text;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.ContextProperties.Extensions;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.Message
{
	[TestFixture]
	public class MockFixture
	{
		[Test]
		public void AnyFunctionCanBeUsedToSetupExtensionMethod()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.GetProperty(BtsProperties.AckRequired)).Returns(() => true);
			message.Setup(m => m.GetProperty(BtsProperties.ActualRetryCount)).Returns(() => 10);
			message.Setup(m => m.GetProperty(BtsProperties.SendPortName)).Returns(() => "send-port-name");

			Assert.That(() => message.Object.GetProperty(BtsProperties.AckRequired), Throws.Nothing);
			Assert.That(message.Object.GetProperty(BtsProperties.AckRequired), Is.True);
			Assert.That(() => message.Object.GetProperty(BtsProperties.ActualRetryCount), Throws.Nothing);
			Assert.That(message.Object.GetProperty(BtsProperties.ActualRetryCount), Is.EqualTo(10));
			Assert.That(() => message.Object.GetProperty(BtsProperties.SendPortName), Throws.Nothing);
			Assert.That(message.Object.GetProperty(BtsProperties.SendPortName), Is.EqualTo("send-port-name"));
		}

		[Test]
		public void AnyPredicateCanBeUsedToSetupPromoteExtensionMethod()
		{
			var context = new Mock<IBaseMessage>(MockBehavior.Strict);
			context.Setup(m => m.Promote(BtsProperties.AckRequired, It.Is<bool>(b => b)));
			context.Setup(m => m.Promote(BtsProperties.ActualRetryCount, It.Is<int>(i => i % 2 == 0)));
			context.Setup(m => m.Promote(BtsProperties.SendPortName, It.Is<string>(s => s.IsQName())));

			Assert.That(() => context.Object.Promote(BtsProperties.AckRequired, true), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.ActualRetryCount, 12), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.SendPortName, "ns:name"), Throws.Nothing);
		}

		[Test]
		public void AnyPredicateCanBeUsedToSetupSetPropertyExtensionMethod()
		{
			var context = new Mock<IBaseMessage>(MockBehavior.Strict);
			context.Setup(m => m.SetProperty(BtsProperties.AckRequired, It.Is<bool>(b => !b)));
			context.Setup(m => m.SetProperty(BtsProperties.ActualRetryCount, It.Is<int>(i => i % 2 != 0)));
			context.Setup(m => m.SetProperty(BtsProperties.SendPortName, It.Is<string>(s => !s.IsQName())));

			Assert.That(() => context.Object.SetProperty(BtsProperties.AckRequired, false), Throws.Nothing);
			Assert.That(() => context.Object.SetProperty(BtsProperties.ActualRetryCount, 11), Throws.Nothing);
			Assert.That(() => context.Object.SetProperty(BtsProperties.SendPortName, "any name"), Throws.Nothing);
		}

		[Test]
		public void AnyValueCanBeUsedToSetupPromoteExtensionMethod()
		{
			var context = new Mock<IBaseMessage>(MockBehavior.Strict);
			context.Setup(m => m.Promote(BtsProperties.AckRequired, It.IsAny<bool>()));
			context.Setup(m => m.Promote(BtsProperties.ActualRetryCount, It.IsAny<int>()));
			context.Setup(m => m.Promote(BtsProperties.SendPortName, It.IsAny<string>()));

			Assert.That(() => context.Object.Promote(BtsProperties.AckRequired, false), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.ActualRetryCount, 11), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.SendPortName, "any-send-port-name"), Throws.Nothing);
		}

		[Test]
		public void AnyValueCanBeUsedToSetupSetPropertyExtensionMethod()
		{
			var context = new Mock<IBaseMessage>(MockBehavior.Strict);
			context.Setup(m => m.SetProperty(BtsProperties.AckRequired, It.IsAny<bool>()));
			context.Setup(m => m.SetProperty(BtsProperties.ActualRetryCount, It.IsAny<int>()));
			context.Setup(m => m.SetProperty(BtsProperties.SendPortName, It.IsAny<string>()));

			Assert.That(() => context.Object.SetProperty(BtsProperties.AckRequired, false), Throws.Nothing);
			Assert.That(() => context.Object.SetProperty(BtsProperties.ActualRetryCount, 11), Throws.Nothing);
			Assert.That(() => context.Object.SetProperty(BtsProperties.SendPortName, "any-send-port-name"), Throws.Nothing);
		}

		[Test]
		public void AnyValueOrPredicateCanBeUsedToVerifyPromoteExtensionMethod()
		{
			var context = new Mock<IBaseMessage>();

			context.Object.Context.Promote(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10);
			context.Object.Context.Promote(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true);
			context.Object.Context.Promote(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name");

			context.Verify(m => m.Promote(BtsProperties.ActualRetryCount, It.IsAny<int>()));
			context.Verify(m => m.Promote(BtsProperties.ActualRetryCount, It.Is<int>(i => i == 10)));
			context.Verify(m => m.Promote(BtsProperties.ActualRetryCount, It.Is<int>(i => i == 10)), Times.Once);
			context.Verify(m => m.Promote(BtsProperties.AckRequired, It.IsAny<bool>()));
			context.Verify(m => m.Promote(BtsProperties.AckRequired, It.Is<bool>(b => b)));
			context.Verify(m => m.Promote(BtsProperties.AckRequired, It.Is<bool>(b => b)), Times.Once);
			context.Verify(m => m.Promote(BtsProperties.SendPortName, It.IsAny<string>()));
			context.Verify(m => m.Promote(BtsProperties.SendPortName, It.Is<string>(s => s == "send-port-name")));
			context.Verify(m => m.Promote(BtsProperties.SendPortName, It.Is<string>(s => s == "send-port-name")), Times.Once);
		}

		[Test]
		public void AnyValueOrPredicateCanBeUsedToVerifySetPropertyExtensionMethod()
		{
			var context = new Mock<IBaseMessage>();

			context.Object.Context.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10);
			context.Object.Context.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true);
			context.Object.Context.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name");

			context.Verify(m => m.SetProperty(BtsProperties.ActualRetryCount, It.IsAny<int>()));
			context.Verify(m => m.SetProperty(BtsProperties.ActualRetryCount, It.Is<int>(i => i == 10)));
			context.Verify(m => m.SetProperty(BtsProperties.ActualRetryCount, It.Is<int>(i => i == 10)), Times.Once);
			context.Verify(m => m.SetProperty(BtsProperties.AckRequired, It.IsAny<bool>()));
			context.Verify(m => m.SetProperty(BtsProperties.AckRequired, It.Is<bool>(b => b)));
			context.Verify(m => m.SetProperty(BtsProperties.AckRequired, It.Is<bool>(b => b)), Times.Once);
			context.Verify(m => m.SetProperty(BtsProperties.SendPortName, It.IsAny<string>()));
			context.Verify(m => m.SetProperty(BtsProperties.SendPortName, It.Is<string>(s => s == "send-port-name")));
			context.Verify(m => m.SetProperty(BtsProperties.SendPortName, It.Is<string>(s => s == "send-port-name")), Times.Once);
		}

		[Test]
		public void BodyPartDataSetupImplicitlySetupOriginalDataStream()
		{
			const string content = "<s1:letter xmlns:s1='urn-one'>" +
				"<s1:headers><s1:subject>inquiry</s1:subject></s1:headers>" +
				"<s1:body><s1:paragraph>paragraph-one</s1:paragraph></s1:body>" +
				"</s1:letter>";
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
			{
				var message = new Mock<IBaseMessage>(MockBehavior.Strict);
				message.Object.BodyPart.Data = inputStream;

				Assert.That(message.Object.BodyPart.GetOriginalDataStream(), Is.SameAs(inputStream));
			}
		}

		[Test]
		[Ignore("MessageMock need to be fixed to support BodyPartName setup.")]
		public void BodyPartNameCanBeSetup()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);

			message.Setup(m => m.BodyPartName).Returns("implicit");

			Assert.That(message.Object.BodyPartName, Is.EqualTo("implicit"));
		}

		[Test]
		public void ContextMockIsImplicitlySetup()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);

			Assert.That(message.Object.Context, Is.Not.Null);
			Assert.That(Mock.Get(message.Object.Context).Behavior, Is.EqualTo(MockBehavior.Strict));
		}

		[Test]
		public void ContextPropertyValueDefaultsToNullWithoutSetupIfMockBehaviorIsLoose()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Loose);

			Assert.That(message.Object.GetProperty(ErrorReportProperties.ErrorType), Is.Null);
			Assert.That(message.Object.HasFailed(), Is.False);
		}

		[Test]
		public void DeletePropertyExtensionMethodCanVerifyContextDeletePropertyExtensionMethodSetup()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Context.DeleteProperty(BtsProperties.AckRequired));
			message.Setup(m => m.Context.DeleteProperty(BtsProperties.ActualRetryCount));
			message.Setup(m => m.Context.DeleteProperty(BtsProperties.SendPortName));

			message.Object.DeleteProperty(BtsProperties.AckRequired);
			message.Object.DeleteProperty(BtsProperties.ActualRetryCount);
			message.Object.DeleteProperty(BtsProperties.SendPortName);

			message.Verify(m => m.DeleteProperty(BtsProperties.AckRequired));
			message.Verify(m => m.DeleteProperty(BtsProperties.ActualRetryCount));
			message.Verify(m => m.DeleteProperty(BtsProperties.SendPortName));
		}

		[Test]
		public void DeletePropertyExtensionMethodCanVerifyVerifiableContextDeletePropertyExtensionMethodSetup()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Context.DeleteProperty(BtsProperties.ActualRetryCount)).Verifiable();
			message.Setup(m => m.Context.DeleteProperty(BtsProperties.AckRequired)).Verifiable();
			message.Setup(m => m.Context.DeleteProperty(BtsProperties.SendPortName)).Verifiable();

			message.Object.DeleteProperty(BtsProperties.ActualRetryCount);
			message.Object.DeleteProperty(BtsProperties.AckRequired);
			message.Object.DeleteProperty(BtsProperties.SendPortName);

			message.Verify();
		}

		[Test]
		public void DeletePropertyExtensionMethodSetupCanBeVerifiedByContextDeletePropertyExtensionMethod()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.DeleteProperty(BtsProperties.AckRequired));
			message.Setup(m => m.DeleteProperty(BtsProperties.ActualRetryCount));
			message.Setup(m => m.DeleteProperty(BtsProperties.SendPortName));

			message.Object.Context.DeleteProperty(BtsProperties.AckRequired);
			message.Object.Context.DeleteProperty(BtsProperties.ActualRetryCount);
			message.Object.Context.DeleteProperty(BtsProperties.SendPortName);

			message.Verify(m => m.Context.DeleteProperty(BtsProperties.AckRequired));
			message.Verify(m => m.Context.DeleteProperty(BtsProperties.ActualRetryCount));
			message.Verify(m => m.Context.DeleteProperty(BtsProperties.SendPortName));
		}

		[Test]
		public void DeletePropertyExtensionMethodVerifiableSetupCanBeVerifiedByContextDeletePropertyExtensionMethod()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.DeleteProperty(BtsProperties.ActualRetryCount)).Verifiable();
			message.Setup(m => m.DeleteProperty(BtsProperties.AckRequired)).Verifiable();
			message.Setup(m => m.DeleteProperty(BtsProperties.SendPortName)).Verifiable();

			message.Object.Context.DeleteProperty(BtsProperties.ActualRetryCount);
			message.Object.Context.DeleteProperty(BtsProperties.AckRequired);
			message.Object.Context.DeleteProperty(BtsProperties.SendPortName);

			message.Verify();
		}

		[Test]
		public void GetPropertyExtensionMethodCanVerifyContextGetPropertyExtensionMethodSetup()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Context.GetProperty(BtsProperties.AckRequired)).Returns(true);
			message.Setup(m => m.Context.GetProperty(BtsProperties.ActualRetryCount)).Returns(10);
			message.Setup(m => m.Context.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			Assert.That(message.Object.GetProperty(BtsProperties.AckRequired), Is.True);
			Assert.That(message.Object.GetProperty(BtsProperties.ActualRetryCount), Is.EqualTo(10));
			Assert.That(message.Object.GetProperty(BtsProperties.SendPortName), Is.EqualTo("send-port-name"));

			message.Verify(m => m.GetProperty(BtsProperties.AckRequired));
			message.Verify(m => m.GetProperty(BtsProperties.ActualRetryCount));
			message.Verify(m => m.GetProperty(BtsProperties.SendPortName));
		}

		[Test]
		public void GetPropertyExtensionMethodCanVerifyVerifiableContextGetPropertyExtensionMethodSetup()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Context.GetProperty(BtsProperties.AckRequired)).Returns(true).Verifiable();
			message.Setup(m => m.Context.GetProperty(BtsProperties.ActualRetryCount)).Returns(10).Verifiable();
			message.Setup(m => m.Context.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name").Verifiable();

			Assert.That(message.Object.GetProperty(BtsProperties.AckRequired), Is.True);
			Assert.That(message.Object.GetProperty(BtsProperties.ActualRetryCount), Is.EqualTo(10));
			Assert.That(message.Object.GetProperty(BtsProperties.SendPortName), Is.EqualTo("send-port-name"));

			message.Verify();
		}

		[Test]
		public void GetPropertyExtensionMethodSetupCanBeVerifiedByContextGetPropertyExtensionMethod()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.GetProperty(BtsProperties.AckRequired)).Returns(true);
			message.Setup(m => m.GetProperty(BtsProperties.ActualRetryCount)).Returns(10);
			message.Setup(m => m.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			Assert.That(message.Object.Context.GetProperty(BtsProperties.AckRequired), Is.True);
			Assert.That(message.Object.Context.GetProperty(BtsProperties.ActualRetryCount), Is.EqualTo(10));
			Assert.That(message.Object.Context.GetProperty(BtsProperties.SendPortName), Is.EqualTo("send-port-name"));

			message.Verify(m => m.Context.GetProperty(BtsProperties.AckRequired));
			message.Verify(m => m.Context.GetProperty(BtsProperties.ActualRetryCount));
			message.Verify(m => m.Context.GetProperty(BtsProperties.SendPortName));
		}

		[Test]
		public void GetPropertyExtensionMethodVerifiableSetupCanBeVerifiedByContextGetPropertyExtensionMethod()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.GetProperty(BtsProperties.AckRequired)).Returns(true).Verifiable();
			message.Setup(m => m.GetProperty(BtsProperties.ActualRetryCount)).Returns(10).Verifiable();
			message.Setup(m => m.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name").Verifiable();

			Assert.That(message.Object.Context.GetProperty(BtsProperties.AckRequired), Is.True);
			Assert.That(message.Object.Context.GetProperty(BtsProperties.ActualRetryCount), Is.EqualTo(10));
			Assert.That(message.Object.Context.GetProperty(BtsProperties.SendPortName), Is.EqualTo("send-port-name"));

			message.Verify();
		}

		[Test]
		public void IsPromotedExtensionMethodFailsWithEitherContextOrMessageIsPromotedExtensionMethodSetup()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Loose);
			message.Setup(m => m.Context.IsPromoted(BtsProperties.AckRequired)).Returns(true);
			message.Setup(m => m.IsPromoted(BtsProperties.ActualRetryCount)).Returns(true);
			message.Setup(m => m.Context.IsPromoted(BtsProperties.SendPortName)).Returns(true);
			message.Setup(m => m.IsPromoted(BtsProperties.TransmitWorkId)).Returns(true);

			Assert.That(message.Object.IsPromoted(BtsProperties.AckRequired), Is.False);
			Assert.That(message.Object.Context.IsPromoted(BtsProperties.ActualRetryCount), Is.False);
			Assert.That(message.Object.Context.IsPromoted(BtsProperties.SendPortName), Is.False);
			Assert.That(message.Object.IsPromoted(BtsProperties.TransmitWorkId), Is.False);

			Assert.That(() => message.Verify(m => m.IsPromoted(BtsProperties.AckRequired)), Throws.Exception);
			Assert.That(() => message.Verify(m => m.IsPromoted(BtsProperties.ActualRetryCount)), Throws.Exception);
			Assert.That(() => message.Verify(m => m.IsPromoted(BtsProperties.SendPortName)), Throws.Exception);
			Assert.That(() => message.Verify(m => m.IsPromoted(BtsProperties.TransmitWorkId)), Throws.Exception);
		}

		[Test]
		public void IsPromotedExtensionMethodRequiresEitherContextOrMessagePromoteExtensionMethodSetup()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Context.Promote(BtsProperties.AckRequired, true));
			message.Setup(m => m.Promote(BtsProperties.ActualRetryCount, 10));
			message.Setup(m => m.Context.Promote(BtsProperties.SendPortName, "send-port-name"));
			message.Setup(m => m.Promote(BtsProperties.TransmitWorkId, "work-id"));

			Assert.That(message.Object.IsPromoted(BtsProperties.AckRequired), Is.True);
			Assert.That(message.Object.Context.IsPromoted(BtsProperties.ActualRetryCount), Is.True);
			Assert.That(message.Object.Context.IsPromoted(BtsProperties.SendPortName), Is.True);
			Assert.That(message.Object.IsPromoted(BtsProperties.TransmitWorkId), Is.True);

			message.Verify(m => m.IsPromoted(BtsProperties.AckRequired));
			message.Verify(m => m.IsPromoted(BtsProperties.ActualRetryCount));
			message.Verify(m => m.IsPromoted(BtsProperties.SendPortName));
			message.Verify(m => m.IsPromoted(BtsProperties.TransmitWorkId));
		}

		[Test]
		public void MoqBugWhereRecursiveMockingOverwritesExplicitSetupIsAscertained()
		{
			var message = new Moq.Mock<IBaseMessage> { DefaultValue = DefaultValue.Empty };
			var context = new Moq.Mock<IBaseMessageContext> { DefaultValue = DefaultValue.Empty };

			message.Setup(m => m.Context).Returns(context.Object);
			Assert.That(message.Object.Context, Is.SameAs(context.Object));

			message.Setup(m => m.Context.CountProperties).Returns(0);
			Assert.That(message.Object.Context, Is.Not.SameAs(context.Object), "Moq bug has been fixed as explicit setup is not overwritten by recursive mocking feature.");
		}

		[Test]
		public void MoqBugWhereRecursiveMockingOverwritesExplicitSetupIsCircumvented()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");
			Assert.That(message.Object.GetProperty(BtsProperties.InboundTransportLocation), Is.Not.Null.Or.Empty);
			var c = message.Object.Context;

			message.Setup(m => m.Context.IsPromoted(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
			Assert.That(message.Object.Context, Is.SameAs(c));
			Assert.That(message.Object.GetProperty(BtsProperties.InboundTransportLocation), Is.Not.Null.Or.Empty);
		}

		[Test]
		public void PromoteExtensionMethodCanVerifyContextPromoteExtensionMethodSetup()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Context.Promote(BtsProperties.AckRequired, true));
			message.Setup(m => m.Context.Promote(BtsProperties.ActualRetryCount, 10));
			message.Setup(m => m.Context.Promote(BtsProperties.SendPortName, "send-port-name"));

			Assert.That(() => message.Object.Promote(BtsProperties.AckRequired, true), Throws.Nothing);
			Assert.That(() => message.Object.Promote(BtsProperties.ActualRetryCount, 10), Throws.Nothing);
			Assert.That(() => message.Object.Promote(BtsProperties.SendPortName, "send-port-name"), Throws.Nothing);

			// notice Promote() setup sets up a context.Read() setup too, i.e. GetProperty() extension method
			Assert.That(message.Object.GetProperty(BtsProperties.AckRequired), Is.True);
			Assert.That(message.Object.GetProperty(BtsProperties.ActualRetryCount), Is.EqualTo(10));
			Assert.That(message.Object.GetProperty(BtsProperties.SendPortName), Is.EqualTo("send-port-name"));

			// notice Promote() setup sets up a context.IsPromoted() setup too, i.e. IsPromoted() extension method
			Assert.That(message.Object.IsPromoted(BtsProperties.AckRequired), Is.True);
			Assert.That(message.Object.IsPromoted(BtsProperties.ActualRetryCount), Is.True);
			Assert.That(message.Object.IsPromoted(BtsProperties.SendPortName), Is.True);

			message.Verify(m => m.Promote(BtsProperties.AckRequired, true));
			message.Verify(m => m.Promote(BtsProperties.AckRequired, true), Times.Once);
			message.Verify(m => m.Promote(BtsProperties.ActualRetryCount, 10));
			message.Verify(m => m.Promote(BtsProperties.ActualRetryCount, 10), Times.Once);
			message.Verify(m => m.Promote(BtsProperties.SendPortName, "send-port-name"));
			message.Verify(m => m.Promote(BtsProperties.SendPortName, "send-port-name"), Times.Once);
		}

		[Test]
		public void PromoteExtensionMethodCanVerifyVerifiableContextPromoteExtensionMethodSetup()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Context.Promote(BtsProperties.ActualRetryCount, 10)).Verifiable();
			message.Setup(m => m.Context.Promote(BtsProperties.AckRequired, true)).Verifiable();
			message.Setup(m => m.Context.Promote(BtsProperties.SendPortName, "send-port-name")).Verifiable();
			message.Setup(m => m.Context.Promote(BtsProperties.ReceivePortName, "receive-port-name")).Verifiable();

			message.Object.Promote(BtsProperties.ActualRetryCount, 10);
			message.Object.Promote(BtsProperties.AckRequired, true);
			message.Object.Promote(BtsProperties.SendPortName, "send-port-name");

			Assert.That(
				() => message.Verify(),
				Throws.InstanceOf<MockException>().With.Message.EqualTo(
					string.Format(
						"The following setups were not matched:\nIBaseMessageContext ctxt => ctxt.Promote(\"{0}\", \"{1}\", \"receive-port-name\")\r\n",
						BtsProperties.ReceivePortName.Name,
						BtsProperties.ReceivePortName.Namespace)));
		}

		[Test]
		public void PromoteExtensionMethodSetupCanBeVerifiedByContextPromoteExtensionMethod()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Promote(BtsProperties.AckRequired, true));
			message.Setup(m => m.Promote(BtsProperties.ActualRetryCount, 10));
			message.Setup(m => m.Promote(BtsProperties.SendPortName, "send-port-name"));

			Assert.That(() => message.Object.Context.Promote(BtsProperties.AckRequired, true), Throws.Nothing);
			Assert.That(() => message.Object.Context.Promote(BtsProperties.ActualRetryCount, 10), Throws.Nothing);
			Assert.That(() => message.Object.Context.Promote(BtsProperties.SendPortName, "send-port-name"), Throws.Nothing);

			// notice Promote() setup sets up a context.Read() setup too, i.e. GetProperty() extension method
			Assert.That(message.Object.Context.GetProperty(BtsProperties.AckRequired), Is.True);
			Assert.That(message.Object.Context.GetProperty(BtsProperties.ActualRetryCount), Is.EqualTo(10));
			Assert.That(message.Object.Context.GetProperty(BtsProperties.SendPortName), Is.EqualTo("send-port-name"));

			// notice Promote() setup sets up a context.IsPromoted() setup too, i.e. IsPromoted() extension method
			Assert.That(message.Object.Context.IsPromoted(BtsProperties.AckRequired), Is.True);
			Assert.That(message.Object.Context.IsPromoted(BtsProperties.ActualRetryCount), Is.True);
			Assert.That(message.Object.Context.IsPromoted(BtsProperties.SendPortName), Is.True);

			message.Verify(m => m.Context.Promote(BtsProperties.AckRequired, true));
			message.Verify(m => m.Context.Promote(BtsProperties.AckRequired, true), Times.Once);
			message.Verify(m => m.Context.Promote(BtsProperties.ActualRetryCount, 10));
			message.Verify(m => m.Context.Promote(BtsProperties.ActualRetryCount, 10), Times.Once);
			message.Verify(m => m.Context.Promote(BtsProperties.SendPortName, "send-port-name"));
			message.Verify(m => m.Context.Promote(BtsProperties.SendPortName, "send-port-name"), Times.Once);
		}

		[Test]
		public void PromoteExtensionMethodVerifiableSetupCanBeVerifiedByContextPromoteExtensionMethod()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Promote(BtsProperties.ActualRetryCount, 10)).Verifiable();
			message.Setup(m => m.Promote(BtsProperties.AckRequired, true)).Verifiable();
			message.Setup(m => m.Promote(BtsProperties.SendPortName, "send-port-name")).Verifiable();
			message.Setup(m => m.Promote(BtsProperties.ReceivePortName, "receive-port-name")).Verifiable();

			message.Object.Context.Promote(BtsProperties.ActualRetryCount, 10);
			message.Object.Context.Promote(BtsProperties.AckRequired, true);
			message.Object.Context.Promote(BtsProperties.SendPortName, "send-port-name");

			Assert.That(
				() => message.Verify(),
				Throws.InstanceOf<MockException>().With.Message.EqualTo(
					string.Format(
						"The following setups were not matched:\nIBaseMessageContext ctxt => ctxt.Promote(\"{0}\", \"{1}\", \"receive-port-name\")\r\n",
						BtsProperties.ReceivePortName.Name,
						BtsProperties.ReceivePortName.Namespace)));
		}

		[Test]
		public void RegularMethodsAndPropertiesSetup()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message
				.Setup(m => m.PartCount)
				.Returns(2);
			message
				.Setup(m => m.GetPart("part"))
				.Returns((IBaseMessagePart) null);

			Assert.That(message.Object.PartCount, Is.EqualTo(2));
			Assert.That(message.Object.GetPart("part"), Is.Null);
		}

		[Test]
		public void SetPropertyExtensionMethodCanVerifyContextSetPropertyExtensionMethodSetup()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Context.SetProperty(BtsProperties.AckRequired, true));
			message.Setup(m => m.Context.SetProperty(BtsProperties.ActualRetryCount, 10));
			message.Setup(m => m.Context.SetProperty(BtsProperties.SendPortName, "send-port-name"));

			message.Object.SetProperty(BtsProperties.AckRequired, true);
			message.Object.SetProperty(BtsProperties.ActualRetryCount, 10);
			message.Object.SetProperty(BtsProperties.SendPortName, "send-port-name");

			message.Verify(m => m.SetProperty(BtsProperties.AckRequired, true));
			message.Verify(m => m.SetProperty(BtsProperties.ActualRetryCount, 10));
			message.Verify(m => m.SetProperty(BtsProperties.SendPortName, "send-port-name"));
		}

		[Test]
		public void SetPropertyExtensionMethodCanVerifyVerifiableContextSetPropertyExtensionMethodSetup()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.Context.SetProperty(BtsProperties.ActualRetryCount, 10)).Verifiable();
			message.Setup(m => m.Context.SetProperty(BtsProperties.AckRequired, true)).Verifiable();
			message.Setup(m => m.Context.SetProperty(BtsProperties.SendPortName, "send-port-name")).Verifiable();

			message.Object.SetProperty(BtsProperties.ActualRetryCount, 10);
			message.Object.SetProperty(BtsProperties.AckRequired, true);
			message.Object.SetProperty(BtsProperties.SendPortName, "send-port-name");

			message.Verify();
		}

		[Test]
		public void SetPropertyExtensionMethodSetupCanBeVerifiedByContextSetPropertyExtensionMethod()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.SetProperty(BtsProperties.AckRequired, true));
			message.Setup(m => m.SetProperty(BtsProperties.ActualRetryCount, 10));
			message.Setup(m => m.SetProperty(BtsProperties.SendPortName, "send-port-name"));

			message.Object.Context.SetProperty(BtsProperties.AckRequired, true);
			message.Object.Context.SetProperty(BtsProperties.ActualRetryCount, 10);
			message.Object.Context.SetProperty(BtsProperties.SendPortName, "send-port-name");

			message.Verify(m => m.Context.SetProperty(BtsProperties.AckRequired, true));
			message.Verify(m => m.Context.SetProperty(BtsProperties.ActualRetryCount, 10));
			message.Verify(m => m.Context.SetProperty(BtsProperties.SendPortName, "send-port-name"));
		}

		[Test]
		public void SetPropertyExtensionMethodVerifiableSetupCanBeVerifiedByContextSetPropertyExtensionMethod()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.SetProperty(BtsProperties.ActualRetryCount, 10)).Verifiable();
			message.Setup(m => m.SetProperty(BtsProperties.AckRequired, true)).Verifiable();
			message.Setup(m => m.SetProperty(BtsProperties.SendPortName, "send-port-name")).Verifiable();

			message.Object.Context.SetProperty(BtsProperties.ActualRetryCount, 10);
			message.Object.Context.SetProperty(BtsProperties.AckRequired, true);
			message.Object.Context.SetProperty(BtsProperties.SendPortName, "send-port-name");

			message.Verify();
		}

		[Test]
		[Ignore("Fails.")]
		public void VerifyAll()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message.Setup(m => m.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			Assert.That(message.Object.GetProperty(BtsProperties.SendPortName), Is.EqualTo("send-port-name"));

			message.VerifyAll();
		}
	}
}
