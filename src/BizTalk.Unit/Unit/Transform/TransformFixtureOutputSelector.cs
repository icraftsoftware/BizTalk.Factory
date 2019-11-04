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
using System.IO;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	internal class TransformFixtureOutputSelector<TTransform> : ITransformFixtureOutputSelector
		where TTransform : TransformBase, new()
	{
		internal TransformFixtureOutputSelector(Stream xsltOutputStream)
		{
			if (xsltOutputStream == null) throw new ArgumentNullException("xsltOutputStream");
			XsltOutputStream = xsltOutputStream;
		}

		#region ITransformFixtureOutputSelector Members

		public ISystemUnderTestSetup<TransformFixtureXmlResult> OutputsXml(Action<ITransformFixtureXmlOutputSetup> xmlOutputSetupConfigurator)
		{
			return new TransformFixtureXmlOutputSetup<TTransform>(XsltOutputStream, xmlOutputSetupConfigurator);
		}

		public ISystemUnderTestSetup<ITransformFixtureTextResult> OutputsText()
		{
			return new TransformFixtureTextOutputSetup<TTransform>(XsltOutputStream);
		}

		#endregion

		private Stream XsltOutputStream { get; set; }
	}
}
