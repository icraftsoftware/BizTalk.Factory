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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.BizTalk.Xml.Extensions;
using Be.Stateless.BizTalk.Xml.XPath.Extensions;
using Be.Stateless.Xml.XPath.Extensions;
using Microsoft.XLANGs.BaseTypes;
using ValidatingXmlReaderSettings = Be.Stateless.Xml.ValidatingXmlReaderSettings;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	internal class TransformFixtureXmlOutputSetup<TTransform> : ITransformFixtureXmlOutputSetup, ISystemUnderTestSetup<TransformFixtureXmlResult>
		where TTransform : TransformBase, new()
	{
		internal TransformFixtureXmlOutputSetup(Stream xsltOutputStream, Action<ITransformFixtureXmlOutputSetup> xmlOutputSetupConfigurator)
		{
			if (xsltOutputStream == null) throw new ArgumentNullException("xsltOutputStream");
			if (xmlOutputSetupConfigurator == null) throw new ArgumentNullException("xmlOutputSetupConfigurator");
			XsltOutputStream = xsltOutputStream;
			ContentProcessing = XmlSchemaContentProcessing.Strict;
			Schemas = new List<XmlSchema>();

			xmlOutputSetupConfigurator(this);
			ValidateSetup();
		}

		#region ISystemUnderTestSetup<TransformFixtureXmlResult> Members

		public TransformFixtureXmlResult Validate()
		{
			using (XsltOutputStream)
			using (var xsltResultReader = CreateValidationAwareReader(XsltOutputStream))
			{
				var navigator = new XPathDocument(xsltResultReader).CreateNavigator();
				var xmlResult = new TransformFixtureXmlResult(navigator, navigator.GetNamespaceManager().AddNamespaces<TTransform>());
				xmlResult.CheckValuedness(ValuednessValidationCallback);
				return xmlResult;
			}
		}

		#endregion

		#region ITransformFixtureXmlOutputSetup Members

		public ITransformFixtureXmlOutputSetup WithValuednessValidationCallback(ValuednessValidationCallback valuednessValidationCallback)
		{
			ValuednessValidationCallback = valuednessValidationCallback;
			return this;
		}

		public ITransformFixtureXmlOutputSetup ConformingTo<T>() where T : SchemaBase, new()
		{
			Schemas.Add(new T().CreateResolvedSchema());
			return this;
		}

		public ISystemUnderTestSetup<TransformFixtureXmlResult> WithConformanceLevel(XmlSchemaContentProcessing xmlSchemaContentProcessing)
		{
			ContentProcessing = xmlSchemaContentProcessing;
			return this;
		}

		public ISystemUnderTestSetup<TransformFixtureXmlResult> WithNoConformanceLevel()
		{
			return WithConformanceLevel(XmlSchemaContentProcessing.Skip);
		}

		public ISystemUnderTestSetup<TransformFixtureXmlResult> WithLaxConformanceLevel()
		{
			return WithConformanceLevel(XmlSchemaContentProcessing.Lax);
		}

		public ISystemUnderTestSetup<TransformFixtureXmlResult> WithStrictConformanceLevel()
		{
			return WithConformanceLevel(XmlSchemaContentProcessing.Strict);
		}

		#endregion

		private XmlSchemaContentProcessing ContentProcessing { get; set; }

		private List<XmlSchema> Schemas { get; set; }

		private ValuednessValidationCallback ValuednessValidationCallback { get; set; }

		private Stream XsltOutputStream { get; set; }

		private void ValidateSetup()
		{
			switch (ContentProcessing)
			{
				case XmlSchemaContentProcessing.None:
				case XmlSchemaContentProcessing.Skip:
					break;
				case XmlSchemaContentProcessing.Lax:
				case XmlSchemaContentProcessing.Strict:
					if (!Schemas.Any()) throw new InvalidOperationException("At least one XML Schema to which the output must conform to must be setup.");
					break;
			}
		}

		private XmlReader CreateValidationAwareReader(Stream transformStream)
		{
			// TODO validation exception should return xml content in exception and be specific about this TTransform transform's output stream
			return XmlReader.Create(transformStream, ValidatingXmlReaderSettings.Create(ContentProcessing, Schemas.ToArray()));
			// TODO ?? check if necessary ??
			//return Schemas.Any()
			//	? XmlReader.Create(transformStream, ValidatingXmlReaderSettings.Create(ContentProcessing, Schemas.ToArray()))
			//	: XmlReader.Create(transformStream);
		}
	}
}
