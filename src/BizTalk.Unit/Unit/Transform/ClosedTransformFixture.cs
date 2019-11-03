#region Copyright & License

// Copyright © 2012 - 2019 François Chabot
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
using System.Diagnostics;
using Be.Stateless.BizTalk.Streaming.Extensions;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	public abstract class ClosedTransformFixture<TTransform> : IMapCustomXsltPathResolver
		where TTransform : TransformBase, new()
	{
		protected ClosedTransformFixture()
		{
			if (Debugger.IsAttached)
			{
				// inject map/transform extensions around XML streams that support XSLT debugging
				StreamExtensions.StreamTransformerFactory = streams => new DebuggerSupportingTransformer(streams, this);
			}
		}

		#region IMapCustomXsltPathResolver Members

		public virtual bool TryResolveXsltPath(out string path)
		{
			return typeof(TTransform).TryResolveCustomXsltPath(out path);
		}

		#endregion

		protected ITransformFixtureSetup Given(Action<ITransformFixtureInputSetup> inputSetupConfigurator)
		{
			return new TransformFixtureInputSetup<TTransform>(inputSetupConfigurator);
		}
	}
}
