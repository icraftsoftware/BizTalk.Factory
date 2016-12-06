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
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Install;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	[TestFixture]
	public class ApplicationBindingBaseFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			BindingGenerationContext.Instance.TargetEnvironment = "ANYTHING";
		}

		[TearDown]
		public void TearDown()
		{
			BindingGenerationContext.Instance.TargetEnvironment = null;
		}

		#endregion

		[Test]
		public void AcceptsAndPropagatesVisitor()
		{
			var applicationBindingMock = new Mock<ApplicationBindingBase<string>> { CallBase = true };

			var orchestrationBindingCollectionMock = new Mock<OrchestrationBindingCollection<string>>(applicationBindingMock.Object) { CallBase = false };
			var visitableOrchestrationBindingCollectionMock = orchestrationBindingCollectionMock.As<IVisitable<IApplicationBindingVisitor>>();
			Reflection.Reflector.SetField(applicationBindingMock.Object, "_orchestrations", orchestrationBindingCollectionMock.Object);

			var receivePortCollectionMock = new Mock<ReceivePortCollection<string>>(applicationBindingMock.Object) { CallBase = false };
			var visitableReceivePortCollectionMock = receivePortCollectionMock.As<IVisitable<IApplicationBindingVisitor>>();
			Reflection.Reflector.SetField(applicationBindingMock.Object, "_receivePorts", receivePortCollectionMock.Object);

			var sendPortCollectionMock = new Mock<SendPortCollection<string>>(applicationBindingMock.Object) { CallBase = false };
			var visitableSendPortCollectionMock = sendPortCollectionMock.As<IVisitable<IApplicationBindingVisitor>>();
			Reflection.Reflector.SetField(applicationBindingMock.Object, "_sendPorts", sendPortCollectionMock.Object);

			var visitorMock = new Mock<IApplicationBindingVisitor>();

			((IVisitable<IApplicationBindingVisitor>) applicationBindingMock.Object).Accept(visitorMock.Object);

			visitorMock.Verify(m => m.VisitApplicationBinding(applicationBindingMock.Object), Times.Once);
			visitableOrchestrationBindingCollectionMock.Verify(m => m.Accept(visitorMock.Object), Times.Once);
			visitableReceivePortCollectionMock.Verify(m => m.Accept(visitorMock.Object), Times.Once);
			visitableSendPortCollectionMock.Verify(m => m.Accept(visitorMock.Object), Times.Once);
		}

		[Test]
		public void AutomaticallyValidatesOnConfiguratorAction()
		{
			var applicationBindingMock = new Mock<ApplicationBindingBase<string>>((Action<IApplicationBinding<string>>) (ab => { ab.Name = "name"; })) { CallBase = true };
			var validatingApplicationBindingMock = applicationBindingMock.As<ISupportValidation>();

			applicationBindingMock.Object.Description = "Force Moq to call ctor.";

			validatingApplicationBindingMock.Verify(m => m.Validate(), Times.Once);
		}

		[Test]
		public void EnvironmentOverridesAreAppliedForGivenEnvironment()
		{
			var applicationBindingMock = new Mock<ApplicationBindingBase<string>> { CallBase = true };

			((ISupportEnvironmentOverride) applicationBindingMock.Object).ApplyEnvironmentOverrides("ACC");

			applicationBindingMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == "ACC"));
		}

		[Test]
		public void EnvironmentOverridesAreSkippedWhenNoGivenEnvironment()
		{
			var applicationBindingMock = new Mock<ApplicationBindingBase<string>> { CallBase = true };

			((ISupportEnvironmentOverride) applicationBindingMock.Object).ApplyEnvironmentOverrides(string.Empty);

			applicationBindingMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Never(), ItExpr.IsAny<string>());
		}

		[Test]
		public void NameIsMandatory()
		{
			var applicationBindingMock = new Mock<ApplicationBindingBase<string>> { CallBase = true };
			applicationBindingMock.Object.Description = "Force Moq to call ctor.";

			Assert.That(
				() => ((ISupportValidation) applicationBindingMock.Object).Validate(),
				Throws.InstanceOf<BindingException>().With.Message.EqualTo("Application's Name is not defined."));
		}

		[Test]
		public void SupportINamingConvention()
		{
			const string name = "Application Name";

			var conventionMock = new Mock<INamingConvention<object>>();
			conventionMock.Setup(m => m.ComputeApplicationName(It.IsAny<IApplicationBinding<object>>())).Returns(name);

			var applicationBindingMock = new Mock<ApplicationBindingBase<object>> { CallBase = true };
			applicationBindingMock.Object.Name = conventionMock.Object;

			Assert.That(((ISupportNamingConvention) applicationBindingMock.Object).Name, Is.EqualTo(name));
		}

		[Test]
		public void SupportStringNamingConvention()
		{
			const string name = "ApplicationName";

			var applicationBindingMock = new Mock<ApplicationBindingBase<string>> { CallBase = true };
			applicationBindingMock.Object.Name = name;

			Assert.That(((ISupportNamingConvention) applicationBindingMock.Object).Name, Is.EqualTo(name));
		}
	}
}
