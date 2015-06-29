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
using System.ComponentModel;
using System.Configuration;
using System.IO;
using Be.Stateless.BizTalk.Unit.Component;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Component
{
	[TestFixture]
	public class MessageFactoryComponentFixture : PipelineComponentFixture<MessageFactoryComponent>
	{
		[Test]
		public void MessageFactoryPluginIsExecuted()
		{
			var sut = CreatePipelineComponent();
			sut.Factory = typeof(MessageFactoryMockWrapper);

			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageFactoryMockWrapper.MessageFactoryMock.Verify(m => m.CreateMessage(It.IsAny<IBaseMessage>()), Times.Once());
		}

		private class MessageFactoryMockWrapper : IMessageFactory
		{
			static MessageFactoryMockWrapper()
			{
				MessageFactoryMock = new Mock<IMessageFactory>();
				MessageFactoryMock
					.Setup(m => m.CreateMessage(It.IsAny<IBaseMessage>()))
					.Returns(new MemoryStream());
			}

			#region IMessageFactory Members

			public Stream CreateMessage(IBaseMessage message)
			{
				return MessageFactoryMock.Object.CreateMessage(message);
			}

			#endregion

			public static readonly Mock<IMessageFactory> MessageFactoryMock;
		}

		protected override object GetValueForProperty(string name)
		{
			switch (name)
			{
				case "Factory":
					// return dummy type
					return typeof(MessageFactoryMockWrapper);
				default:
					return base.GetValueForProperty(name);
			}
		}

		static MessageFactoryComponentFixture()
		{
			// PipelineComponentFixture<CreateMessageComponent> assumes and needs the following converters
			TypeDescriptor.AddAttributes(typeof(Type), new TypeConverterAttribute(typeof(TypeNameConverter)));
		}
	}
}
