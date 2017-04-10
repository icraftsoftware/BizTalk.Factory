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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Unit.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Component
{
	[TestFixture]
	public class MicroPipelineComponentFixture : PipelineComponentFixture<MicroPipelineComponent>
	{
		[Test]
		public void ExecuteMicroComponents()
		{
			var pipelineContextMock = new Mock<IPipelineContext>();
			var messageMock1 = new Unit.Message.Mock<IBaseMessage>();
			var messageMock2 = new Unit.Message.Mock<IBaseMessage>();
			var messageMock3 = new Unit.Message.Mock<IBaseMessage>();

			var microComponentMockOne = new Mock<IMicroPipelineComponent>();
			microComponentMockOne
				.Setup(mc => mc.Execute(pipelineContextMock.Object, messageMock1.Object)).Returns(messageMock2.Object)
				.Verifiable();
			var microComponentMockTwo = new Mock<IMicroPipelineComponent>();
			microComponentMockTwo
				.Setup(mc => mc.Execute(pipelineContextMock.Object, messageMock2.Object)).Returns(messageMock3.Object)
				.Verifiable();

			var sut = new MicroPipelineComponent {
				Components = new[] {
					microComponentMockOne.Object,
					microComponentMockTwo.Object
				}
			};

			Assert.That(sut.Execute(pipelineContextMock.Object, messageMock1.Object), Is.SameAs(messageMock3.Object));

			microComponentMockOne.VerifyAll();
			microComponentMockTwo.VerifyAll();
		}

		[Test]
		public void ExecuteNoMicroComponents()
		{
			var messageMock = new Unit.Message.Mock<IBaseMessage>();

			var sut = new MicroPipelineComponent();

			Assert.That(sut.Execute(new Mock<IPipelineContext>().Object, messageMock.Object), Is.SameAs(messageMock.Object));
		}

		[Test]
		public void LoadConfiguration()
		{
			var microPipelineComponents = new IMicroPipelineComponent[] {
				new MicroPipelineComponentDummyOne(),
				new MicroPipelineComponentDummyTwo()
			};

			var propertyBag = new PropertyBag();
			propertyBag.Add("Enabled", true);
			propertyBag.Add("Components", MicroPipelineComponentEnumerableConverter.Serialize(microPipelineComponents));

			var sut = new MicroPipelineComponent();
			sut.Load(propertyBag, 0);

			Assert.That(sut.Components, Is.EqualTo(microPipelineComponents));
		}

		[Test]
		public void SaveConfiguration()
		{
			var microPipelineComponents = new IMicroPipelineComponent[] {
				new MicroPipelineComponentDummyOne(),
				new MicroPipelineComponentDummyTwo()
			};

			var propertyBag = new PropertyBag();

			var sut = new MicroPipelineComponent { Components = microPipelineComponents };
			sut.Save(propertyBag, true, true);

			Assert.That(propertyBag.Read("Components"), Is.EqualTo(MicroPipelineComponentEnumerableConverter.Serialize(microPipelineComponents)));
		}

		static MicroPipelineComponentFixture()
		{
			// PipelineComponentFixture<MicroPipelineComponent> assumes and needs the following converter
			TypeDescriptor.AddAttributes(typeof(IEnumerable<IMicroPipelineComponent>), new TypeConverterAttribute(typeof(MicroPipelineComponentEnumerableConverter)));
		}

		protected override object GetValueForProperty(string name)
		{
			switch (name)
			{
				case "Components":
					return new IMicroPipelineComponent[] { new MicroPipelineComponentDummyOne() };
				default:
					return base.GetValueForProperty(name);
			}
		}

		public class MicroPipelineComponentDummyOne : IMicroPipelineComponent, IEquatable<MicroPipelineComponentDummyOne>
		{
			#region IEquatable<MicroPipelineComponentDummyOne> Members

			[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
			public bool Equals(MicroPipelineComponentDummyOne other)
			{
				return GetType() == other.GetType();
			}

			#endregion

			#region IMicroPipelineComponent Members

			public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
			{
				throw new NotSupportedException();
			}

			#endregion

			#region Base Class Member Overrides

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != GetType()) return false;
				return Equals((MicroPipelineComponentDummyOne) obj);
			}

			public override int GetHashCode()
			{
				return GetType().GetHashCode();
			}

			#endregion
		}

		public class MicroPipelineComponentDummyTwo : IMicroPipelineComponent, IEquatable<MicroPipelineComponentDummyTwo>
		{
			#region IEquatable<MicroPipelineComponentDummyTwo> Members

			[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
			public bool Equals(MicroPipelineComponentDummyTwo other)
			{
				return GetType() == other.GetType();
			}

			#endregion

			#region IMicroPipelineComponent Members

			public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
			{
				throw new NotSupportedException();
			}

			#endregion

			#region Base Class Member Overrides

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != GetType()) return false;
				return Equals((MicroPipelineComponentDummyTwo) obj);
			}

			public override int GetHashCode()
			{
				return GetType().GetHashCode();
			}

			#endregion
		}
	}
}
