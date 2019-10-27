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
using Be.Stateless.BizTalk.Streaming.Extensions;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	internal abstract class TransformOutputFixtureSetup
	{
		protected TransformOutputFixtureSetup(TransformFixtureInputSetup inputs)
		{
			if (inputs == null) throw new ArgumentNullException("inputs");
			_inputs = inputs;
		}

		protected Stream CreateXsltResultStream()
		{
			var inputStream = _inputs.Messages.ToArray().Transform();
			_inputs.MessageContext.IfNotNull(messageContext => inputStream.ExtendWith(messageContext));
			return _inputs.XsltArguments != null
				? inputStream.Apply(_inputs.TransformType, _inputs.XsltArguments)
				: inputStream.Apply(_inputs.TransformType);
		}

		protected readonly TransformFixtureInputSetup _inputs;
	}
}
