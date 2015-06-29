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
using System.IO;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Unit.Transform;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.BaseTypes;
using Moq;
using NUnit.Framework;
using MessageMock = Be.Stateless.BizTalk.Unit.Message.Mock<Microsoft.BizTalk.Message.Interop.IBaseMessage>;

namespace Be.Stateless.BizTalk.Component
{
	[TestFixture]
	public class ExtensiblePipelineComponentFixture
	{
		[Test]
		public void ResolvePluginReturnsInstance()
		{
			var messageMock = new MessageMock();
			messageMock.Setup(m => m.GetProperty(BizTalkFactoryProperties.MessageFactoryTypeName)).Returns(typeof(ContextualMessageFactory).AssemblyQualifiedName);

			var sut = new Mock<DummyMessageFactoryComponent> { CallBase = true };

			Assert.That(sut.Object.ResolvePlugin(messageMock.Object, typeof(ConfiguredMessageFactory)), Is.TypeOf<ContextualMessageFactory>());
		}

		[Test]
		public void ResolvePluginReturnsNull()
		{
			var messageMock = new MessageMock();

			var sut = new Mock<DummyMessageFactoryComponent> { CallBase = true };

			Assert.That(sut.Object.ResolvePlugin(messageMock.Object, null), Is.Null);
		}

		[Test]
		public void ResolvePluginTypeReturnsConfiguredPluginType()
		{
			var messageMock = new MessageMock();

			var sut = new Mock<DummyMessageFactoryComponent> { CallBase = true };

			Assert.That(sut.Object.ResolvePluginType(messageMock.Object, typeof(ConfiguredMessageFactory)), Is.EqualTo(typeof(ConfiguredMessageFactory)));
		}

		[Test]
		public void ResolvePluginTypeReturnsContextualPluginType()
		{
			var messageMock = new MessageMock();
			messageMock.Setup(m => m.GetProperty(BizTalkFactoryProperties.MessageFactoryTypeName)).Returns(typeof(ContextualMessageFactory).AssemblyQualifiedName);

			var sut = new Mock<DummyMessageFactoryComponent> { CallBase = true };

			Assert.That(sut.Object.ResolvePluginType(messageMock.Object, typeof(ConfiguredMessageFactory)), Is.EqualTo(typeof(ContextualMessageFactory)));
		}

		[Test]
		public void ResolvePluginTypeReturnsNull()
		{
			var messageMock = new MessageMock();

			var sut = new Mock<DummyMessageFactoryComponent> { CallBase = true };

			Assert.That(sut.Object.ResolvePluginType(messageMock.Object, null), Is.Null);
		}

		[Test]
		public void ResolvePluginTypeThrowsNothingWhenExpectedRuntimeTypeIsSupported()
		{
			var messageMock = new MessageMock();

			var sut = new Mock<DummyXsltRunnerComponent> { CallBase = true };

			Assert.That(() => sut.Object.ResolvePluginType(messageMock.Object, typeof(IdentityTransform)), Throws.Nothing);
		}

		[Test]
		public void ResolvePluginTypeThrowsWhenExpectedRuntimeTypeIsNotSupported()
		{
			var messageMock = new MessageMock();

			var sut = new Mock<DummyMessageFactoryComponent> { CallBase = true };

			Assert.That(
				() => sut.Object.ResolvePluginType(messageMock.Object, typeof(object)),
				Throws.InvalidOperationException.With.Message.EqualTo(
					string.Format(
						"The plugin type '{0}' does not support the type '{1}'.",
						typeof(object).AssemblyQualifiedName,
						typeof(IMessageFactory).AssemblyQualifiedName)));
		}

		// ReSharper disable once MemberCanBePrivate.Global
		internal abstract class DummyXsltRunnerComponent : ExtensiblePipelineComponent<TransformBase>
		{
			public Type ResolvePluginType(IBaseMessage message, Type configuredType)
			{
				return base.ResolvePluginType(message, BizTalkFactoryProperties.MessageFactoryTypeName, configuredType);
			}
		}

		// ReSharper disable once MemberCanBePrivate.Global
		internal abstract class DummyMessageFactoryComponent : ExtensiblePipelineComponent<IMessageFactory>
		{
			public Type ResolvePluginType(IBaseMessage message, Type configuredType)
			{
				return base.ResolvePluginType(message, BizTalkFactoryProperties.MessageFactoryTypeName, configuredType);
			}

			public IMessageFactory ResolvePlugin(IBaseMessage message, Type configuredType)
			{
				return base.ResolvePlugin(message, BizTalkFactoryProperties.MessageFactoryTypeName, configuredType);
			}
		}

		private class ContextualMessageFactory : IMessageFactory
		{
			#region IMessageFactory Members

			public Stream CreateMessage(IBaseMessage message)
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		private class ConfiguredMessageFactory : IMessageFactory
		{
			#region IMessageFactory Members

			public Stream CreateMessage(IBaseMessage message)
			{
				throw new NotImplementedException();
			}

			#endregion
		}
	}
}
