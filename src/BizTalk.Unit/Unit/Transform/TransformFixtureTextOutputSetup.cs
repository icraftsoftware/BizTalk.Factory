﻿#region Copyright & License

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

namespace Be.Stateless.BizTalk.Unit.Transform
{
	internal class TransformFixtureTextOutputSetup : ISystemUnderTestSetup<ITransformFixtureTextResult>
	{
		internal TransformFixtureTextOutputSetup(Stream xsltOutputStream)
		{
			if (xsltOutputStream == null) throw new ArgumentNullException("xsltOutputStream");
			XsltOutputStream = xsltOutputStream;
		}

		#region ISystemUnderTestSetup<ITransformFixtureTextResult> Members

		public ITransformFixtureTextResult Validate()
		{
			using (XsltOutputStream)
			{
				return new TransformFixtureTextResult(XsltOutputStream);
			}
		}

		#endregion

		private Stream XsltOutputStream { get; set; }
	}
}