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

namespace Be.Stateless.BizTalk.Unit.Transform
{
	internal class TransformFixtureOutputSelector : ITransformFixtureOutputSelector
	{
		public TransformFixtureOutputSelector(TransformFixtureInputSetup inputSetup)
		{
			if (inputSetup == null) throw new ArgumentNullException("inputSetup");
			_inputSetup = inputSetup;
		}

		#region ITransformFixtureOutputSelector Members

		public ISystemUnderTestSetup<TransformFixtureXmlResult> OutputsXml(Action<ITransformFixtureXmlOutputSetup> xmlOutputSetupConfigurator)
		{
			var outputSetup = new TransformFixtureXmlOutputSetup(_inputSetup);
			xmlOutputSetupConfigurator(outputSetup);
			return outputSetup;
		}

		public ISystemUnderTestSetup<ITransformFixtureTextResult> OutputsText()
		{
			throw new NotImplementedException();
			return new TransformFixtureTextOutputSetup(_inputSetup);
		}

		#endregion

		private readonly TransformFixtureInputSetup _inputSetup;
	}
}
