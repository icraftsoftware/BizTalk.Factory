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
using Microsoft.BizTalk.Message.Interop;
using NUnit.Framework;
using MessageMock = Be.Stateless.BizTalk.Unit.Message.Mock<Microsoft.BizTalk.Message.Interop.IBaseMessage>;

namespace Be.Stateless.BizTalk.MicroComponent.Extensions
{
	[TestFixture]
	public class PluginResolutionExtensionsFixture
	{
		[Test]
		public void AsPluginReturnsInstance()
		{
			var type = typeof(ContextualMessageFactory);

			var resolvedPlugin = type.AsPlugin<IMessageFactory>();

			Assert.That(resolvedPlugin, Is.Not.Null.And.InstanceOf<IMessageFactory>());
		}

		[Test]
		public void AsPluginReturnsNull()
		{
			var resolvedPluginType = ((Type) null).AsPlugin<IMessageFactory>();

			Assert.That(resolvedPluginType, Is.Null);
		}

		[Test]
		public void AsPluginThrowsNothingWhenExpectedRuntimeType()
		{
			var type = typeof(ContextualMessageFactory);

			Assert.That(() => type.AsPlugin<IMessageFactory>(), Throws.Nothing);
		}

		[Test]
		public void AsPluginThrowsWhenNotExpectedRuntimeType()
		{
			var type = GetType();

			Assert.That(
				() => type.AsPlugin<IMessageFactory>(),
				Throws.InvalidOperationException.With.Message.EqualTo(
					string.Format(
						"The plugin type '{0}' does not support the type '{1}'.",
						GetType().AssemblyQualifiedName,
						typeof(IMessageFactory).AssemblyQualifiedName)));
		}

		[Test]
		public void OfPluginTypeReturnsNull()
		{
			var resolvedPluginType = ((Type) null).OfPluginType<IMessageFactory>();

			Assert.That(resolvedPluginType, Is.Null);
		}

		[Test]
		public void OfPluginTypeReturnsPluginType()
		{
			var type = typeof(ContextualMessageFactory);

			var resolvedPluginType = type.OfPluginType<IMessageFactory>();

			Assert.That(resolvedPluginType, Is.SameAs(type));
		}

		[Test]
		public void OfPluginTypeThrowsNothingWhenExpectedRuntimeType()
		{
			var type = typeof(ContextualMessageFactory);

			Assert.That(() => type.OfPluginType<IMessageFactory>(), Throws.Nothing);
		}

		[Test]
		public void OfPluginTypeThrowsWhenNotExpectedRuntimeType()
		{
			var type = GetType();

			Assert.That(
				() => type.OfPluginType<IMessageFactory>(),
				Throws.InvalidOperationException.With.Message.EqualTo(
					string.Format(
						"The plugin type '{0}' does not support the type '{1}'.",
						GetType().AssemblyQualifiedName,
						typeof(IMessageFactory).AssemblyQualifiedName)));
		}

		[Test]
		public void ResolvePluginTypeReturnsConfiguredPluginType()
		{
			var messageMock = new MessageMock();

			var resolvedPluginType = messageMock.Object.ResolvePluginType(BizTalkFactoryProperties.MessageBodyStreamFactoryTypeName, typeof(ConfiguredMessageFactory));

			Assert.That(resolvedPluginType, Is.EqualTo(typeof(ConfiguredMessageFactory)));
		}

		[Test]
		public void ResolvePluginTypeReturnsContextualPluginType()
		{
			var messageMock = new MessageMock();
			messageMock.Setup(m => m.GetProperty(BizTalkFactoryProperties.MessageBodyStreamFactoryTypeName)).Returns(typeof(ContextualMessageFactory).AssemblyQualifiedName);

			var resolvedPluginType = messageMock.Object.ResolvePluginType(BizTalkFactoryProperties.MessageBodyStreamFactoryTypeName, typeof(ConfiguredMessageFactory));

			Assert.That(resolvedPluginType, Is.EqualTo(typeof(ContextualMessageFactory)));
		}

		[Test]
		public void ResolvePluginTypeReturnsNull()
		{
			var messageMock = new MessageMock();

			var resolvedPluginType = messageMock.Object.ResolvePluginType(BizTalkFactoryProperties.MessageBodyStreamFactoryTypeName, null);

			Assert.That(resolvedPluginType, Is.Null);
		}

		private class ContextualMessageFactory : IMessageFactory
		{
			#region IMessageFactory Members

			public Stream CreateMessage(IBaseMessage message)
			{
				throw new NotSupportedException();
			}

			#endregion
		}

		private class ConfiguredMessageFactory : IMessageFactory
		{
			#region IMessageFactory Members

			public Stream CreateMessage(IBaseMessage message)
			{
				throw new NotSupportedException();
			}

			#endregion
		}
	}
}
