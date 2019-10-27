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
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using Be.Stateless.Linq.Extensions;
using Be.Stateless.Xml;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	internal class TransformWithXmlOutputFixtureSetup : TransformOutputFixtureSetup, ISystemUnderTestSetup<ClosedTransformFixtureXmlResult>
	{
		public TransformWithXmlOutputFixtureSetup(TransformFixtureInputSetup inputs, TransformFixtureOutputSetup outputs) : base(inputs)
		{
			if (outputs == null) throw new ArgumentNullException("outputs");
			_outputs = outputs;
		}

		#region ISystemUnderTestSetup<ClosedTransformFixtureXmlResult> Members

		public ClosedTransformFixtureXmlResult Execute()
		{
			using (var xsltResultStream = CreateXsltResultStream())
			using (var xsltResultReader = CreateValidationAwareReader(xsltResultStream))
			{
				var navigator = new XPathDocument(xsltResultReader).CreateNavigator();
				var xmlNamespaceManager = new XmlNamespaceManager(navigator.NameTable);
				_inputs.XsltNamespaces.Each(ns => xmlNamespaceManager.AddNamespace(ns.Key, ns.Value));
				return new ClosedTransformFixtureXmlResult(navigator, xmlNamespaceManager);
			}
		}

		#endregion

		private XmlReader CreateValidationAwareReader(Stream transformStream)
		{
			// TODO validation exception should return output/transformed xml content in exception

			return _outputs.Schemas.Any()
				? XmlReader.Create(transformStream, ValidatingXmlReaderSettings.Create(_outputs.ContentProcessing, _outputs.Schemas.ToArray()))
				: XmlReader.Create(transformStream);
		}

		private readonly TransformFixtureOutputSetup _outputs;
	}
}
