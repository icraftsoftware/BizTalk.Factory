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
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.Unit.Component;
using Microsoft.BizTalk.Message.Interop;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Component
{
	[TestFixture]
	public class ContextBuilderComponentFixture : PipelineComponentFixture<ContextBuilderComponent>
	{
		static ContextBuilderComponentFixture()
		{
			// PipelineComponentFixture<PluginFactoryComponent> assumes and needs the following converters
			TypeDescriptor.AddAttributes(
				typeof(Type),
				new TypeConverterAttribute(typeof(TypeNameConverter)));
		}

		protected override object GetValueForProperty(string name)
		{
			switch (name)
			{
				case "Builder":
					return typeof(DummyBuilder);
				default:
					return base.GetValueForProperty(name);
			}
		}

		private class DummyBuilder : IContextBuilder
		{
			#region IContextBuilder Members

			public void Execute(IBaseMessageContext context)
			{
				throw new NotSupportedException();
			}

			#endregion
		}
	}
}
