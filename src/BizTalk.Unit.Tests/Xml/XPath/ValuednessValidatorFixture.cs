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
using System.Xml.Schema;
using System.Xml.XPath;
using Be.Stateless.Xml.XPath.Extensions;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Xml.XPath
{
	[TestFixture]
	public class ValuednessValidatorFixture
	{
		[Test]
		public void CheckValuednessWithoutValuednessValidationCallback()
		{
			using (var reader = XmlReader.Create(new StringReader(XML_CONTENT)))
			{
				var navigator = new XPathDocument(reader).CreateNavigator();

				var sut = new ValuednessValidator(navigator, null);

				Assert.That(sut.Validate(), Is.False);
			}
		}

		[Test]
		public void CheckValuednessWithValuednessValidationCallbackReportingErrorSeverity()
		{
			ExpectedEmptyNodes = new string[] { };
			using (var reader = XmlReader.Create(new StringReader(XML_CONTENT)))
			{
				var navigator = new XPathDocument(reader).CreateNavigator();

				var sut = new ValuednessValidator(navigator, ValidationCallback);

				Assert.That(
					() => sut.Validate(),
					Throws.TypeOf<XmlException>()
						.With.Message.EqualTo(
							"The following nodes have either no value nor any child element:" + Environment.NewLine + string.Join(
								Environment.NewLine,
								new[] {
									"/root/empty-parent",
									"/root/parent/empty-child[1]",
									"/root/parent/non-nil-child",
									"/root/parent/child/firstname",
									"/root/parent/empty-child[2]",
									"/root/parent/child/firstname/@no-language"
								})));
			}
		}

		[Test]
		public void CheckValuednessWithValuednessValidationCallbackReportingWarningSeverity()
		{
			ExpectedEmptyNodes = new[] {
				"/*/ns:empty-parent",
				"ns:parent/ns:empty-child",
				"ns:parent/ns:non-nil-child",
				"ns:parent/ns:child/ns:firstname",
				"ns:firstname/@no-language"
			};
			using (var reader = XmlReader.Create(new StringReader(XML_CONTENT)))
			{
				var navigator = new XPathDocument(reader).CreateNavigator();

				var sut = new ValuednessValidator(navigator, ValidationCallback);

				Assert.That(sut.Validate(), Is.False);
			}
		}

		private string[] ExpectedEmptyNodes { get; set; }

		private void ValidationCallback(object sender, ValuednessValidationCallbackArgs args)
		{
			var nsm = args.Navigator.GetNamespaceManager();
			nsm.AddNamespace("ns", "urn:no-schema");
			var matched = ExpectedEmptyNodes.Any(xpath => args.Navigator.Matches(XPathExpression.Compile(xpath, nsm)));
			if (matched) args.Severity = XmlSeverityType.Warning;
		}

		private const string XML_CONTENT = @"<?xml version='1.0' encoding='utf-8'?>
<root xmlns='urn:no-schema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
  <empty-parent />
  <parent>
    <empty-child />
    <nil-child xsi:nil='true' />
    <non-nil-child xsi:nil='false' />
    <child>
      <firstname no-language='' />
      <lastname language='en-US'>chabot</lastname>
    </child>
    <empty-child />
  </parent>
</root>";
	}
}
