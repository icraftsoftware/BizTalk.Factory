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

using Be.Stateless.BizTalk.Install;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	[TestFixture]
	public class ReferencedApplicationBindingCollectionFixture
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
		public void AcceptsAndPropagatesVisitor()
		{
			var applicationBindingMock = new Mock<ApplicationBindingBase<string>> { CallBase = true };

			var referencedApplicationBindingCollection = new ReferencedApplicationBindingCollection();
			referencedApplicationBindingCollection.Add(applicationBindingMock.Object);

			var visitorMock = new Mock<IApplicationBindingVisitor>();
			((IVisitable<IApplicationBindingVisitor>) referencedApplicationBindingCollection).Accept(visitorMock.Object);

			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>().Verify(m => m.Accept(visitorMock.Object), Times.Once);
		}
	}
}
