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

using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.Message.Context
{
	[TestFixture]
	public class MockFixture
	{
		[Test]
		public void AnyFunctionCanBeUsedToSetupExtensionMethod()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.GetProperty(BtsProperties.AckRequired)).Returns(() => true);
			context.Setup(c => c.GetProperty(BtsProperties.ActualRetryCount)).Returns(() => 10);
			context.Setup(c => c.GetProperty(BtsProperties.SendPortName)).Returns(() => "send-port-name");

			Assert.That(() => context.Object.GetProperty(BtsProperties.AckRequired), Throws.Nothing);
			Assert.That(context.Object.GetProperty(BtsProperties.AckRequired), Is.True);
			Assert.That(() => context.Object.GetProperty(BtsProperties.ActualRetryCount), Throws.Nothing);
			Assert.That(context.Object.GetProperty(BtsProperties.ActualRetryCount), Is.EqualTo(10));
			Assert.That(() => context.Object.GetProperty(BtsProperties.SendPortName), Throws.Nothing);
			Assert.That(context.Object.GetProperty(BtsProperties.SendPortName), Is.EqualTo("send-port-name"));
		}

		[Test]
		public void AnyPredicateCanBeUsedToSetupPromoteExtensionMethod()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired, It.Is<bool>(b => b)));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount, It.Is<int>(i => i % 2 == 0)));
			context.Setup(c => c.Promote(BtsProperties.SendPortName, It.Is<string>(s => s.IsQName())));

			Assert.That(() => context.Object.Promote(BtsProperties.AckRequired, true), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.ActualRetryCount, 12), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.SendPortName, "ns:name"), Throws.Nothing);
		}

		[Test]
		public void AnyPredicateCanBeUsedToSetupSetPropertyExtensionMethod()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.SetProperty(BtsProperties.AckRequired, It.Is<bool>(b => !b)));
			context.Setup(c => c.SetProperty(BtsProperties.ActualRetryCount, It.Is<int>(i => i % 2 != 0)));
			context.Setup(c => c.SetProperty(BtsProperties.SendPortName, It.Is<string>(s => !s.IsQName())));

			Assert.That(() => context.Object.SetProperty(BtsProperties.AckRequired, false), Throws.Nothing);
			Assert.That(() => context.Object.SetProperty(BtsProperties.ActualRetryCount, 11), Throws.Nothing);
			Assert.That(() => context.Object.SetProperty(BtsProperties.SendPortName, "any name"), Throws.Nothing);
		}

		[Test]
		public void AnyValueCanBeUsedToSetupPromoteExtensionMethod()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired, It.IsAny<bool>()));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount, It.IsAny<int>()));
			context.Setup(c => c.Promote(BtsProperties.SendPortName, It.IsAny<string>()));

			Assert.That(() => context.Object.Promote(BtsProperties.AckRequired, false), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.ActualRetryCount, 11), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.SendPortName, "any-send-port-name"), Throws.Nothing);
		}

		[Test]
		public void AnyValueCanBeUsedToSetupSetPropertyExtensionMethod()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.SetProperty(BtsProperties.AckRequired, It.IsAny<bool>()));
			context.Setup(c => c.SetProperty(BtsProperties.ActualRetryCount, It.IsAny<int>()));
			context.Setup(c => c.SetProperty(BtsProperties.SendPortName, It.IsAny<string>()));

			Assert.That(() => context.Object.SetProperty(BtsProperties.AckRequired, false), Throws.Nothing);
			Assert.That(() => context.Object.SetProperty(BtsProperties.ActualRetryCount, 11), Throws.Nothing);
			Assert.That(() => context.Object.SetProperty(BtsProperties.SendPortName, "any-send-port-name"), Throws.Nothing);
		}

		[Test]
		public void AnyValueOrPredicateCanBeUsedToVerifyPromoteExtensionMethod()
		{
			var context = new Mock<IBaseMessageContext>();

			context.Object.Promote(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10);
			context.Object.Promote(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true);
			context.Object.Promote(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name");

			context.Verify(c => c.Promote(BtsProperties.ActualRetryCount, It.IsAny<int>()));
			context.Verify(c => c.Promote(BtsProperties.ActualRetryCount, It.Is<int>(i => i == 10)));
			context.Verify(c => c.Promote(BtsProperties.ActualRetryCount, It.Is<int>(i => i == 10)), Times.Once);
			context.Verify(c => c.Promote(BtsProperties.AckRequired, It.IsAny<bool>()));
			context.Verify(c => c.Promote(BtsProperties.AckRequired, It.Is<bool>(b => b)));
			context.Verify(c => c.Promote(BtsProperties.AckRequired, It.Is<bool>(b => b)), Times.Once);
			context.Verify(c => c.Promote(BtsProperties.SendPortName, It.IsAny<string>()));
			context.Verify(c => c.Promote(BtsProperties.SendPortName, It.Is<string>(s => s == "send-port-name")));
			context.Verify(c => c.Promote(BtsProperties.SendPortName, It.Is<string>(s => s == "send-port-name")), Times.Once);
		}

		[Test]
		public void AnyValueOrPredicateCanBeUsedToVerifySetPropertyExtensionMethod()
		{
			var context = new Mock<IBaseMessageContext>();

			context.Object.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10);
			context.Object.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true);
			context.Object.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name");

			context.Verify(c => c.SetProperty(BtsProperties.ActualRetryCount, It.IsAny<int>()));
			context.Verify(c => c.SetProperty(BtsProperties.ActualRetryCount, It.Is<int>(i => i == 10)));
			context.Verify(c => c.SetProperty(BtsProperties.ActualRetryCount, It.Is<int>(i => i == 10)), Times.Once);
			context.Verify(c => c.SetProperty(BtsProperties.AckRequired, It.IsAny<bool>()));
			context.Verify(c => c.SetProperty(BtsProperties.AckRequired, It.Is<bool>(b => b)));
			context.Verify(c => c.SetProperty(BtsProperties.AckRequired, It.Is<bool>(b => b)), Times.Once);
			context.Verify(c => c.SetProperty(BtsProperties.SendPortName, It.IsAny<string>()));
			context.Verify(c => c.SetProperty(BtsProperties.SendPortName, It.Is<string>(s => s == "send-port-name")));
			context.Verify(c => c.SetProperty(BtsProperties.SendPortName, It.Is<string>(s => s == "send-port-name")), Times.Once);
		}

		[Test]
		public void ContextPropertyValueDefaultsToNullWithoutSetupIfMockBehaviorIsLoose()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Loose);

			Assert.That(context.Object.GetProperty(BtsProperties.AckRequired), Is.Null);
			Assert.That(context.Object.Read(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace), Is.Null);
			Assert.That(context.Object.GetProperty(BtsProperties.ActualRetryCount), Is.Null);
			Assert.That(context.Object.Read(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace), Is.Null);
			Assert.That(context.Object.GetProperty(BtsProperties.SendPortName), Is.Null);
			Assert.That(context.Object.Read(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace), Is.Null);
		}

		[Test]
		public void DeletePropertyExtensionMethodCanBeSetupByWrite()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, null));
			context.Setup(c => c.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, null));
			context.Setup(c => c.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, null));

			Assert.That(() => context.Object.DeleteProperty(BtsProperties.AckRequired), Throws.Nothing);
			Assert.That(() => context.Object.DeleteProperty(BtsProperties.ActualRetryCount), Throws.Nothing);
			Assert.That(() => context.Object.DeleteProperty(BtsProperties.SendPortName), Throws.Nothing);
		}

		[Test]
		public void DeletePropertyExtensionMethodCanVerifyWriteSetupAndCall()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, null));
			context.Setup(c => c.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, null));
			context.Setup(c => c.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, null));

			Assert.That(() => context.Object.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, null), Throws.Nothing);
			Assert.That(() => context.Object.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, null), Throws.Nothing);
			Assert.That(() => context.Object.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, null), Throws.Nothing);

			context.Verify(c => c.DeleteProperty(BtsProperties.AckRequired));
			context.Verify(c => c.DeleteProperty(BtsProperties.ActualRetryCount));
			context.Verify(c => c.DeleteProperty(BtsProperties.SendPortName));
		}

		[Test]
		public void DeletePropertyExtensionMethodSetupAndCall()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.DeleteProperty(BtsProperties.AckRequired));
			context.Setup(c => c.DeleteProperty(BtsProperties.ActualRetryCount));
			context.Setup(c => c.DeleteProperty(BtsProperties.SendPortName));

			Assert.That(() => context.Object.DeleteProperty(BtsProperties.AckRequired), Throws.Nothing);
			Assert.That(() => context.Object.DeleteProperty(BtsProperties.ActualRetryCount), Throws.Nothing);
			Assert.That(() => context.Object.DeleteProperty(BtsProperties.SendPortName), Throws.Nothing);
		}

		[Test]
		public void DeletePropertyExtensionMethodSetupAndCallThroughWrite()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.DeleteProperty(BtsProperties.AckRequired));
			context.Setup(c => c.DeleteProperty(BtsProperties.ActualRetryCount));
			context.Setup(c => c.DeleteProperty(BtsProperties.SendPortName));

			Assert.That(() => context.Object.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, null), Throws.Nothing);
			Assert.That(() => context.Object.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, null), Throws.Nothing);
			Assert.That(() => context.Object.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, null), Throws.Nothing);
		}

		[Test]
		public void DeletePropertyExtensionMethodSetupAndVerify()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.DeleteProperty(BtsProperties.AckRequired));
			context.Setup(c => c.DeleteProperty(BtsProperties.ActualRetryCount));
			context.Setup(c => c.DeleteProperty(BtsProperties.SendPortName));

			Assert.That(() => context.Object.DeleteProperty(BtsProperties.AckRequired), Throws.Nothing);
			Assert.That(() => context.Object.DeleteProperty(BtsProperties.ActualRetryCount), Throws.Nothing);
			Assert.That(() => context.Object.DeleteProperty(BtsProperties.SendPortName), Throws.Nothing);

			context.Verify(c => c.DeleteProperty(BtsProperties.AckRequired));
			context.Verify(c => c.DeleteProperty(BtsProperties.ActualRetryCount));
			context.Verify(c => c.DeleteProperty(BtsProperties.SendPortName));
		}

		[Test]
		public void DeletePropertyExtensionMethodSetupAndVerifyThroughWrite()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.DeleteProperty(BtsProperties.AckRequired));
			context.Setup(c => c.DeleteProperty(BtsProperties.ActualRetryCount));
			context.Setup(c => c.DeleteProperty(BtsProperties.SendPortName));

			Assert.That(() => context.Object.DeleteProperty(BtsProperties.AckRequired), Throws.Nothing);
			Assert.That(() => context.Object.DeleteProperty(BtsProperties.ActualRetryCount), Throws.Nothing);
			Assert.That(() => context.Object.DeleteProperty(BtsProperties.SendPortName), Throws.Nothing);

			context.Verify(c => c.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, null));
			context.Verify(c => c.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, null));
			context.Verify(c => c.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, null));
		}

		[Test]
		public void DeletePropertyExtensionMethodSetupAndVerifyWhenCalledThroughWrite()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.DeleteProperty(BtsProperties.AckRequired));
			context.Setup(c => c.DeleteProperty(BtsProperties.ActualRetryCount));
			context.Setup(c => c.DeleteProperty(BtsProperties.SendPortName));

			Assert.That(() => context.Object.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, null), Throws.Nothing);
			Assert.That(() => context.Object.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, null), Throws.Nothing);
			Assert.That(() => context.Object.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, null), Throws.Nothing);

			context.Verify(c => c.DeleteProperty(BtsProperties.AckRequired));
			context.Verify(c => c.DeleteProperty(BtsProperties.ActualRetryCount));
			context.Verify(c => c.DeleteProperty(BtsProperties.SendPortName));
		}

		[Test]
		public void GetPropertyExtensionMethodCanBeSetupByRead()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Read(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace)).Returns(true);
			context.Setup(c => c.Read(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace)).Returns(10);
			context.Setup(c => c.Read(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace)).Returns("send-port-name");

			Assert.That(context.Object.GetProperty(BtsProperties.AckRequired), Is.True);
			Assert.That(context.Object.GetProperty(BtsProperties.ActualRetryCount), Is.EqualTo(10));
			Assert.That(context.Object.GetProperty(BtsProperties.SendPortName), Is.EqualTo("send-port-name"));
		}

		[Test]
		public void GetPropertyExtensionMethodCanVerifyReadSetupAndCall()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Read(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace)).Returns(true);
			context.Setup(c => c.Read(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace)).Returns(10);
			context.Setup(c => c.Read(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace)).Returns("send-port-name");

			Assert.That(context.Object.Read(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace), Is.True);
			Assert.That(context.Object.Read(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace), Is.EqualTo(10));
			Assert.That(context.Object.Read(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace), Is.EqualTo("send-port-name"));

			context.Verify(c => c.GetProperty(BtsProperties.AckRequired));
			context.Verify(c => c.GetProperty(BtsProperties.ActualRetryCount));
			context.Verify(c => c.GetProperty(BtsProperties.SendPortName));
		}

		[Test]
		public void GetPropertyExtensionMethodSetupAndCall()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.GetProperty(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.GetProperty(BtsProperties.ActualRetryCount)).Returns(10);
			context.Setup(c => c.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			Assert.That(context.Object.GetProperty(BtsProperties.AckRequired), Is.True);
			Assert.That(context.Object.GetProperty(BtsProperties.ActualRetryCount), Is.EqualTo(10));
			Assert.That(context.Object.GetProperty(BtsProperties.SendPortName), Is.EqualTo("send-port-name"));
		}

		[Test]
		public void GetPropertyExtensionMethodSetupAndCallThroughRead()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.GetProperty(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.GetProperty(BtsProperties.ActualRetryCount)).Returns(10);
			context.Setup(c => c.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			Assert.That(context.Object.Read(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace), Is.True);
			Assert.That(context.Object.Read(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace), Is.EqualTo(10));
			Assert.That(context.Object.Read(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace), Is.EqualTo("send-port-name"));
		}

		[Test]
		public void GetPropertyExtensionMethodSetupAndVerify()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.GetProperty(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.GetProperty(BtsProperties.ActualRetryCount)).Returns(10);
			context.Setup(c => c.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			Assert.That(context.Object.GetProperty(BtsProperties.AckRequired), Is.True);
			Assert.That(context.Object.GetProperty(BtsProperties.ActualRetryCount), Is.EqualTo(10));
			Assert.That(context.Object.GetProperty(BtsProperties.SendPortName), Is.EqualTo("send-port-name"));

			context.Verify(c => c.GetProperty(BtsProperties.AckRequired));
			context.Verify(c => c.GetProperty(BtsProperties.ActualRetryCount));
			context.Verify(c => c.GetProperty(BtsProperties.SendPortName));
		}

		[Test]
		public void GetPropertyExtensionMethodSetupAndVerifyThroughRead()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.GetProperty(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.GetProperty(BtsProperties.ActualRetryCount)).Returns(10);
			context.Setup(c => c.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			Assert.That(context.Object.GetProperty(BtsProperties.AckRequired), Is.True);
			Assert.That(context.Object.GetProperty(BtsProperties.ActualRetryCount), Is.EqualTo(10));
			Assert.That(context.Object.GetProperty(BtsProperties.SendPortName), Is.EqualTo("send-port-name"));

			context.Verify(c => c.Read(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace));
			context.Verify(c => c.Read(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace));
			context.Verify(c => c.Read(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace));
		}

		[Test]
		public void GetPropertyExtensionMethodSetupAndVerifyWhenCalledThroughRead()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.GetProperty(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.GetProperty(BtsProperties.ActualRetryCount)).Returns(10);
			context.Setup(c => c.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");

			Assert.That(context.Object.Read(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace), Is.True);
			Assert.That(context.Object.Read(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace), Is.EqualTo(10));
			Assert.That(context.Object.Read(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace), Is.EqualTo("send-port-name"));

			context.Verify(c => c.GetProperty(BtsProperties.AckRequired));
			context.Verify(c => c.GetProperty(BtsProperties.ActualRetryCount));
			context.Verify(c => c.GetProperty(BtsProperties.SendPortName));
		}

		[Test]
		public void IsPromotedExtensionMethodCallAndVerifyRequiresPromoteExtensionMethodSetup()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired, true));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.Promote(BtsProperties.SendPortName, "send-port-name"));

			// notice IsPromoted() extension method returns false because it also calls GetProperty() to ensure that the
			// property actually has a value as prescribed by BizTalk: a promoted property must have a value
			Assert.That(context.Object.IsPromoted(BtsProperties.AckRequired), Is.True);
			Assert.That(context.Object.IsPromoted(BtsProperties.ActualRetryCount), Is.True);
			Assert.That(context.Object.IsPromoted(BtsProperties.SendPortName), Is.True);

			// IsPromoted() call can be verified when setup via Promote() extension method because it actually sets up two
			// core operations as well: Read() and IsPromoted()
			context.Verify(c => c.IsPromoted(BtsProperties.AckRequired));
			context.Verify(c => c.IsPromoted(BtsProperties.ActualRetryCount));
			context.Verify(c => c.IsPromoted(BtsProperties.SendPortName));
		}

		[Test]
		public void IsPromotedExtensionMethodCallWillFailWhenIsPromotedExtensionMethodOnlyIsSetup()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Loose); // notice behavior is loose
			context.Setup(c => c.IsPromoted(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.ActualRetryCount)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.SendPortName)).Returns(true);

			// notice IsPromoted() extension method returns false because it also calls GetProperty() to ensure that the
			// property actually has a value as prescribed by BizTalk: a promoted property must have a value
			Assert.That(context.Object.IsPromoted(BtsProperties.AckRequired), Is.False);
			Assert.That(context.Object.IsPromoted(BtsProperties.ActualRetryCount), Is.False);
			Assert.That(context.Object.IsPromoted(BtsProperties.SendPortName), Is.False);

			// IsPromoted() call cannot be verified as well because it is actually rewritten as two core operations: Read() and IsPromoted()
			Assert.That(() => context.Verify(c => c.IsPromoted(BtsProperties.AckRequired)), Throws.TypeOf<MockException>());
			Assert.That(() => context.Verify(c => c.IsPromoted(BtsProperties.ActualRetryCount)), Throws.TypeOf<MockException>());
			Assert.That(() => context.Verify(c => c.IsPromoted(BtsProperties.SendPortName)), Throws.TypeOf<MockException>());
		}

		[Test]
		public void IsPromotedExtensionMethodCannotBeSetupByCoreIsPromoted()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Loose); // notice behavior is loose
			context.Setup(c => c.IsPromoted(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace)).Returns(true);

			// notice IsPromoted() extension method returns false because it also calls GetProperty() to ensure that the
			// property actually has a value as prescribed by BizTalk: a promoted property must have a value
			Assert.That(context.Object.IsPromoted(BtsProperties.AckRequired), Is.False);
			Assert.That(context.Object.IsPromoted(BtsProperties.ActualRetryCount), Is.False);
			Assert.That(context.Object.IsPromoted(BtsProperties.SendPortName), Is.False);
		}

		[Test]
		public void IsPromotedExtensionMethodCanVerifyCoreIsPromotedSetupAndCall()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.IsPromoted(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace)).Returns(true);

			Assert.That(context.Object.IsPromoted(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace), Is.True);
			Assert.That(context.Object.IsPromoted(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace), Is.True);
			Assert.That(context.Object.IsPromoted(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace), Is.True);

			context.Verify(c => c.IsPromoted(BtsProperties.AckRequired));
			context.Verify(c => c.IsPromoted(BtsProperties.ActualRetryCount));
			context.Verify(c => c.IsPromoted(BtsProperties.SendPortName));
		}

		[Test]
		public void IsPromotedExtensionMethodSetupAndCallThroughCoreIsPromoted()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.IsPromoted(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.ActualRetryCount)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.SendPortName)).Returns(true);

			Assert.That(context.Object.IsPromoted(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace), Is.True);
			Assert.That(context.Object.IsPromoted(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace), Is.True);
			Assert.That(context.Object.IsPromoted(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace), Is.True);
		}

		[Test]
		public void IsPromotedExtensionMethodSetupAndVerifyThroughCoreIsPromoted()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Loose); // notice behavior is loose
			context.Setup(c => c.IsPromoted(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.ActualRetryCount)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.SendPortName)).Returns(true);

			// notice IsPromoted() extension method returns false because it also calls GetProperty() to ensure that the
			// property actually has a value as prescribed by BizTalk: a promoted property must have a value
			Assert.That(context.Object.IsPromoted(BtsProperties.AckRequired), Is.False);
			Assert.That(context.Object.IsPromoted(BtsProperties.ActualRetryCount), Is.False);
			Assert.That(context.Object.IsPromoted(BtsProperties.SendPortName), Is.False);

			// Core IsPromoted() cannot be verified because IsPromoted() extension method calls core IsPromoted() after
			// having called core Read() method only if the latter returns a property value
			Assert.That(() => context.Verify(c => c.IsPromoted(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace)), Throws.TypeOf<MockException>());
			Assert.That(() => context.Verify(c => c.IsPromoted(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace)), Throws.TypeOf<MockException>());
			Assert.That(() => context.Verify(c => c.IsPromoted(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace)), Throws.TypeOf<MockException>());
		}

		[Test]
		public void IsPromotedExtensionMethodSetupAndVerifyWhenCalledThroughCoreIsPromoted()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.IsPromoted(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.ActualRetryCount)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.SendPortName)).Returns(true);

			Assert.That(context.Object.IsPromoted(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace), Is.True);
			Assert.That(context.Object.IsPromoted(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace), Is.True);
			Assert.That(context.Object.IsPromoted(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace), Is.True);

			context.Verify(c => c.IsPromoted(BtsProperties.AckRequired));
			context.Verify(c => c.IsPromoted(BtsProperties.ActualRetryCount));
			context.Verify(c => c.IsPromoted(BtsProperties.SendPortName));
		}

		[Test]
		public void IsPromotedExtensionMethodVerifyWillFailWhenIsPromotedExtensionMethodOnlyIsSetup()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Loose); // notice behavior is loose
			context.Setup(c => c.IsPromoted(BtsProperties.AckRequired)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.ActualRetryCount)).Returns(true);
			context.Setup(c => c.IsPromoted(BtsProperties.SendPortName)).Returns(true);

			// notice IsPromoted() extension method returns false because it also calls GetProperty() to ensure that the
			// property actually has a value as prescribed by BizTalk: a promoted property must have a value
			Assert.That(context.Object.IsPromoted(BtsProperties.AckRequired), Is.False);
			Assert.That(context.Object.IsPromoted(BtsProperties.ActualRetryCount), Is.False);
			Assert.That(context.Object.IsPromoted(BtsProperties.SendPortName), Is.False);

			// IsPromoted() call cannot be verified as well because it is actually rewritten as two core operations: Read() and IsPromoted()
			Assert.That(() => context.Verify(c => c.IsPromoted(BtsProperties.AckRequired)), Throws.TypeOf<MockException>());
			Assert.That(() => context.Verify(c => c.IsPromoted(BtsProperties.ActualRetryCount)), Throws.TypeOf<MockException>());
			Assert.That(() => context.Verify(c => c.IsPromoted(BtsProperties.SendPortName)), Throws.TypeOf<MockException>());
		}

		[Test]
		public void PromoteExtensionMethodCanBeSetupByCorePromote()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10));
			context.Setup(c => c.Promote(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name"));

			Assert.That(() => context.Object.Promote(BtsProperties.AckRequired, true), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.ActualRetryCount, 10), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.SendPortName, "send-port-name"), Throws.Nothing);
		}

		[Test]
		public void PromoteExtensionMethodCanVerifyCorePromoteSetupAndCall()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10));
			context.Setup(c => c.Promote(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name"));

			Assert.That(() => context.Object.Promote(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name"), Throws.Nothing);

			context.Verify(c => c.Promote(BtsProperties.AckRequired, true));
			context.Verify(c => c.Promote(BtsProperties.ActualRetryCount, 10));
			context.Verify(c => c.Promote(BtsProperties.SendPortName, "send-port-name"));
		}

		[Test]
		public void PromoteExtensionMethodSetupAndCall()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired, true));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.Promote(BtsProperties.SendPortName, "send-port-name"));

			Assert.That(() => context.Object.Promote(BtsProperties.AckRequired, true), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.ActualRetryCount, 10), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.SendPortName, "send-port-name"), Throws.Nothing);
		}

		[Test]
		public void PromoteExtensionMethodSetupAndCallThroughCorePromote()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired, true));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.Promote(BtsProperties.SendPortName, "send-port-name"));

			Assert.That(() => context.Object.Promote(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name"), Throws.Nothing);
		}

		[Test]
		public void PromoteExtensionMethodSetupAndVerify()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired, true));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.Promote(BtsProperties.SendPortName, "send-port-name"));

			Assert.That(() => context.Object.Promote(BtsProperties.AckRequired, true), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.ActualRetryCount, 10), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.SendPortName, "send-port-name"), Throws.Nothing);

			context.Verify(c => c.Promote(BtsProperties.AckRequired, true));
			context.Verify(c => c.Promote(BtsProperties.ActualRetryCount, 10));
			context.Verify(c => c.Promote(BtsProperties.SendPortName, "send-port-name"));
		}

		[Test]
		public void PromoteExtensionMethodSetupAndVerifyThroughCorePromote()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired, true));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.Promote(BtsProperties.SendPortName, "send-port-name"));

			Assert.That(() => context.Object.Promote(BtsProperties.AckRequired, true), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.ActualRetryCount, 10), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.SendPortName, "send-port-name"), Throws.Nothing);

			context.Verify(c => c.Promote(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true));
			context.Verify(c => c.Promote(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10));
			context.Verify(c => c.Promote(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name"));
		}

		[Test]
		public void PromoteExtensionMethodSetupAndVerifyWhenCalledThroughCorePromote()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Promote(BtsProperties.AckRequired, true));
			context.Setup(c => c.Promote(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.Promote(BtsProperties.SendPortName, "send-port-name"));

			Assert.That(() => context.Object.Promote(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10), Throws.Nothing);
			Assert.That(() => context.Object.Promote(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name"), Throws.Nothing);

			context.Verify(c => c.Promote(BtsProperties.AckRequired, true));
			context.Verify(c => c.Promote(BtsProperties.ActualRetryCount, 10));
			context.Verify(c => c.Promote(BtsProperties.SendPortName, "send-port-name"));
		}

		[Test]
		public void SetPropertyExtensionMethodCanBeSetupByWrite()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true));
			context.Setup(c => c.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10));
			context.Setup(c => c.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name"));

			Assert.That(() => context.Object.SetProperty(BtsProperties.AckRequired, true), Throws.Nothing);
			Assert.That(() => context.Object.SetProperty(BtsProperties.ActualRetryCount, 10), Throws.Nothing);
			Assert.That(() => context.Object.SetProperty(BtsProperties.SendPortName, "send-port-name"), Throws.Nothing);
		}

		[Test]
		public void SetPropertyExtensionMethodCanVerifyWriteSetupAndCall()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true));
			context.Setup(c => c.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10));
			context.Setup(c => c.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name"));

			Assert.That(() => context.Object.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true), Throws.Nothing);
			Assert.That(() => context.Object.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10), Throws.Nothing);
			Assert.That(() => context.Object.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name"), Throws.Nothing);

			context.Verify(c => c.SetProperty(BtsProperties.AckRequired, true));
			context.Verify(c => c.SetProperty(BtsProperties.ActualRetryCount, 10));
			context.Verify(c => c.SetProperty(BtsProperties.SendPortName, "send-port-name"));
		}

		[Test]
		public void SetPropertyExtensionMethodSetupAndCall()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.SetProperty(BtsProperties.AckRequired, true));
			context.Setup(c => c.SetProperty(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.SetProperty(BtsProperties.SendPortName, "send-port-name"));

			Assert.That(() => context.Object.SetProperty(BtsProperties.AckRequired, true), Throws.Nothing);
			Assert.That(() => context.Object.SetProperty(BtsProperties.ActualRetryCount, 10), Throws.Nothing);
			Assert.That(() => context.Object.SetProperty(BtsProperties.SendPortName, "send-port-name"), Throws.Nothing);
		}

		[Test]
		public void SetPropertyExtensionMethodSetupAndCallThroughWrite()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.SetProperty(BtsProperties.AckRequired, true));
			context.Setup(c => c.SetProperty(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.SetProperty(BtsProperties.SendPortName, "send-port-name"));

			Assert.That(() => context.Object.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true), Throws.Nothing);
			Assert.That(() => context.Object.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10), Throws.Nothing);
			Assert.That(() => context.Object.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name"), Throws.Nothing);
		}

		[Test]
		public void SetPropertyExtensionMethodSetupAndVerify()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.SetProperty(BtsProperties.AckRequired, true));
			context.Setup(c => c.SetProperty(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.SetProperty(BtsProperties.SendPortName, "send-port-name"));

			Assert.That(() => context.Object.SetProperty(BtsProperties.AckRequired, true), Throws.Nothing);
			Assert.That(() => context.Object.SetProperty(BtsProperties.ActualRetryCount, 10), Throws.Nothing);
			Assert.That(() => context.Object.SetProperty(BtsProperties.SendPortName, "send-port-name"), Throws.Nothing);

			context.Verify(c => c.SetProperty(BtsProperties.AckRequired, true));
			context.Verify(c => c.SetProperty(BtsProperties.ActualRetryCount, 10));
			context.Verify(c => c.SetProperty(BtsProperties.SendPortName, "send-port-name"));
		}

		[Test]
		public void SetPropertyExtensionMethodSetupAndVerifyThroughWrite()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.SetProperty(BtsProperties.AckRequired, true));
			context.Setup(c => c.SetProperty(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.SetProperty(BtsProperties.SendPortName, "send-port-name"));

			Assert.That(() => context.Object.SetProperty(BtsProperties.AckRequired, true), Throws.Nothing);
			Assert.That(() => context.Object.SetProperty(BtsProperties.ActualRetryCount, 10), Throws.Nothing);
			Assert.That(() => context.Object.SetProperty(BtsProperties.SendPortName, "send-port-name"), Throws.Nothing);

			context.Verify(c => c.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true));
			context.Verify(c => c.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10));
			context.Verify(c => c.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name"));
		}

		[Test]
		public void SetPropertyExtensionMethodSetupAndVerifyWhenCalledThroughWrite()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.SetProperty(BtsProperties.AckRequired, true));
			context.Setup(c => c.SetProperty(BtsProperties.ActualRetryCount, 10));
			context.Setup(c => c.SetProperty(BtsProperties.SendPortName, "send-port-name"));

			Assert.That(() => context.Object.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true), Throws.Nothing);
			Assert.That(() => context.Object.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10), Throws.Nothing);
			Assert.That(() => context.Object.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name"), Throws.Nothing);

			context.Verify(c => c.SetProperty(BtsProperties.AckRequired, true));
			context.Verify(c => c.SetProperty(BtsProperties.ActualRetryCount, 10));
			context.Verify(c => c.SetProperty(BtsProperties.SendPortName, "send-port-name"));
		}

		[Test]
		public void VerifyOfVerifiableExtensionMethodSetup()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.SetProperty(BtsProperties.AckRequired, true)).Verifiable();
			context.Setup(c => c.SetProperty(BtsProperties.ActualRetryCount, 10)).Verifiable();
			context.Setup(c => c.SetProperty(BtsProperties.SendPortName, "send-port-name")).Verifiable();

			context.Object.SetProperty(BtsProperties.ActualRetryCount, 10);
			context.Object.SetProperty(BtsProperties.AckRequired, true);
			context.Object.SetProperty(BtsProperties.SendPortName, "send-port-name");

			context.Verify();
		}

		[Test]
		public void VerifyOfVerifiableExtensionMethodSetupThrowsIfUnmatchedExpectation()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context.Setup(c => c.GetProperty(BtsProperties.AckRequired)).Returns(true).Verifiable();
			context.Setup(c => c.GetProperty(BtsProperties.ActualRetryCount)).Returns(It.IsAny<string>()).Verifiable();
			context.Setup(c => c.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name").Verifiable();

			context.Object.GetProperty(BtsProperties.ActualRetryCount);
			context.Object.GetProperty(BtsProperties.AckRequired);

			Assert.That(
				() => context.Verify(),
				Throws.InstanceOf<MockException>().With.Message.EqualTo(
					string.Format(
						"The following setups were not matched:\n" + "IBaseMessageContext ctxt => ctxt.Read(\"{0}\", \"{1}\")\r\n",
						BtsProperties.SendPortName.Name,
						BtsProperties.SendPortName.Namespace)));
		}
	}
}
