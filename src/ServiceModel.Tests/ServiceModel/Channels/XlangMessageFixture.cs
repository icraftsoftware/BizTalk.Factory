#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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

using System.IO;
using System.Xml;
using System.Xml.Schema;
using Be.Stateless.Xml.Schema;
using NUnit.Framework;

namespace Be.Stateless.ServiceModel.Channels
{
	[TestFixture]
	public class XlangMessageFixture
	{
		[Test]
		public void LaxValidationFails()
		{
			using (var xmlReader = GetXmlReader(INVALID_LAX_ARGUMENTS_XML))
			{
				var arguments = new XlangMessage<Calculator.LaxArguments>(XmlSchemaContentProcessing.Lax);
				// ReSharper disable AccessToDisposedClosure
				Assert.That(() => arguments.ReadXml(xmlReader), Throws.TypeOf<XmlSchemaValidationException>());
				// ReSharper restore AccessToDisposedClosure
			}
		}

		[Test]
		public void LaxValidationSucceeds()
		{
			using (var xmlReader = GetXmlReader(UNKNOWN_ARGUMENTS_XML))
			{
				var arguments = new XlangMessage<Calculator.LaxArguments>(XmlSchemaContentProcessing.Lax);
				// ReSharper disable AccessToDisposedClosure
				Assert.That(() => arguments.ReadXml(xmlReader), Throws.Nothing);
				// ReSharper restore AccessToDisposedClosure
			}
			using (var xmlReader = GetXmlReader(LAX_ARGUMENTS_XML))
			{
				var arguments = new XlangMessage<Calculator.LaxArguments>(XmlSchemaContentProcessing.Lax);
				// ReSharper disable AccessToDisposedClosure
				Assert.That(() => arguments.ReadXml(xmlReader), Throws.Nothing);
				// ReSharper restore AccessToDisposedClosure
			}
			using (var xmlReader = GetXmlReader(CALCULATOR_REQUEST_XML))
			{
				var arguments = new XlangMessage<Calculator.LaxArguments>(XmlSchemaContentProcessing.Lax);
				// ReSharper disable AccessToDisposedClosure
				Assert.That(() => arguments.ReadXml(xmlReader), Throws.Nothing);
				// ReSharper restore AccessToDisposedClosure
			}
		}

		[Test]
		public void StrictValidationFails()
		{
			using (var xmlReader = GetXmlReader(INVALID_LAX_ARGUMENTS_XML))
			{
				var arguments = new XlangMessage<Calculator.LaxArguments>(XmlSchemaContentProcessing.Strict);
				// ReSharper disable AccessToDisposedClosure
				Assert.That(() => arguments.ReadXml(xmlReader), Throws.TypeOf<XmlSchemaValidationException>());
				// ReSharper restore AccessToDisposedClosure
			}
			using (var xmlReader = GetXmlReader(UNKNOWN_ARGUMENTS_XML))
			{
				var arguments = new XlangMessage<Calculator.LaxArguments>(XmlSchemaContentProcessing.Strict);
				// ReSharper disable AccessToDisposedClosure
				Assert.That(() => arguments.ReadXml(xmlReader), Throws.TypeOf<XmlSchemaValidationException>());
				// ReSharper restore AccessToDisposedClosure
			}
		}

		[Test]
		public void StrictValidationSucceeds()
		{
			using (var xmlReader = GetXmlReader(LAX_ARGUMENTS_XML))
			{
				var arguments = new XlangMessage<Calculator.LaxArguments>(XmlSchemaContentProcessing.Strict);
				// ReSharper disable AccessToDisposedClosure
				Assert.That(() => arguments.ReadXml(xmlReader), Throws.Nothing);
				// ReSharper restore AccessToDisposedClosure
			}
			using (var xmlReader = GetXmlReader(CALCULATOR_REQUEST_XML))
			{
				var arguments = new XlangMessage<Calculator.LaxArguments>(XmlSchemaContentProcessing.Strict);
				// ReSharper disable AccessToDisposedClosure
				Assert.That(() => arguments.ReadXml(xmlReader), Throws.Nothing);
				// ReSharper restore AccessToDisposedClosure
			}
		}

		private static XmlReader GetXmlReader(string xml)
		{
			var xmlReader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { CloseInput = true });
			xmlReader.MoveToContent();
			return xmlReader;
		}

		private const string CALCULATOR_REQUEST_XML = "<s0:CalculatorRequest xmlns:s0=\"urn:services.stateless.be:unit:calculator\">" +
			"<s0:Arguments xmlns:s0=\"urn:services.stateless.be:unit:calculator\">" +
			"<s0:Term>1</s0:Term>" +
			"<s0:Term>2</s0:Term>" +
			"</s0:Arguments>" +
			"</s0:CalculatorRequest>";

		private const string LAX_ARGUMENTS_XML = "<s0:LaxArguments xmlns:s0=\"urn:services.stateless.be:unit:calculator\">" +
			"<s0:Term>1</s0:Term>" +
			"<s0:Term>2</s0:Term>" +
			"</s0:LaxArguments>";

		private const string INVALID_LAX_ARGUMENTS_XML = "<s0:LaxArguments xmlns:s0=\"urn:services.stateless.be:unit:calculator\">" +
			"<s0:Operand>1</s0:Operand>" +
			"<s0:Operand>2</s0:Operand>" +
			"</s0:LaxArguments>";

		private const string UNKNOWN_ARGUMENTS_XML = "<s0:Arguments xmlns:s0=\"urn:servcies.stateless.be:unit:computer\">" +
			"<s0:Argument>1</s0:Argument>" +
			"<s0:Argument>2</s0:Argument>" +
			"</s0:Arguments>";
	}
}
