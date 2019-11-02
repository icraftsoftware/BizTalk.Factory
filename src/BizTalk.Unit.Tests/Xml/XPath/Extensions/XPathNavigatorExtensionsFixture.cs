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
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Xml.XPath.Extensions
{
	[TestFixture]
	public class XPathNavigatorExtensionsFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUop()
		{
			_defaultValuednessValidatorFactory = XPathNavigatorExtensions.ValuednessValidatorFactory;
		}

		[TearDown]
		public void TearDown()
		{
			XPathNavigatorExtensions.ValuednessValidatorFactory = _defaultValuednessValidatorFactory;
		}

		#endregion

		[Test]
		public void CheckValuednessDelegatesToValuednessValidator()
		{
			using (var reader = XmlReader.Create(new StringReader(XML_CONTENT)))
			{
				var navigator = new XPathDocument(reader).CreateNavigator();

				var mock = new Moq.Mock<ValuednessValidator>(navigator, null);
				XPathNavigatorExtensions.ValuednessValidatorFactory = (n, c) => mock.Object;
				mock.Setup(m => m.Validate()).Returns(false);

				Assert.That(navigator.CheckValuedness(null), Is.False);

				mock.VerifyAll();
			}
		}

		[Test]
		public void SelectEmptyAttributes()
		{
			using (var reader = XmlReader.Create(new StringReader(XML_CONTENT)))
			{
				var navigator = new XPathDocument(reader).CreateNavigator();
				Assert.That(navigator.SelectEmptyAttributes().Select(n => n.Name), Is.EqualTo(new[] { "no-language" }));
			}
		}

		[Test]
		public void SelectEmptyElements()
		{
			using (var reader = XmlReader.Create(new StringReader(XML_CONTENT)))
			{
				var navigator = new XPathDocument(reader).CreateNavigator();
				Assert.That(
					navigator.SelectEmptyElements().Select(n => n.Name),
					Is.EqualTo(new[] { "empty-parent", "empty-child", "non-nil-child", "firstname" }));
			}
		}

		private Func<XPathNavigator, ValuednessValidationCallback, ValuednessValidator> _defaultValuednessValidatorFactory;

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
  </parent>
</root>";
	}
}
