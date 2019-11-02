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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.BizTalk.Xml.XPath.Extensions;
using Be.Stateless.Linq.Extensions;
using Be.Stateless.Xml.XPath.Extensions;
using Microsoft.XLANGs.BaseTypes;
using ValidatingXmlReaderSettings = Be.Stateless.Xml.ValidatingXmlReaderSettings;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	internal class TransformFixtureXmlOutputSetup : TransformFixtureOutputSetup, ITransformFixtureXmlOutputSetup, ISystemUnderTestSetup<TransformFixtureXmlResult>
	{
		public TransformFixtureXmlOutputSetup(TransformFixtureInputSetup inputs) : base(inputs)
		{
			ContentProcessing = XmlSchemaContentProcessing.Strict;
			Schemas = new List<XmlSchema>();
		}

		#region ISystemUnderTestSetup<TransformFixtureXmlResult> Members

		public TransformFixtureXmlResult Validate()
		{
			using (var xsltResultStream = CreateXsltResultStream())
			using (var xsltResultReader = CreateValidationAwareReader(xsltResultStream))
			{
				var navigator = new XPathDocument(xsltResultReader).CreateNavigator();
				var xmlNamespaceManager = navigator.GetNamespaceManager();
				_inputs.XsltNamespaces.Each(ns => xmlNamespaceManager.AddNamespace(ns.Key, ns.Value));
				// assert that there is no empty element, i.e. element without child or without text, and no non-valued attribute
				navigator.CheckValuedness(ValuednessValidationCallback);
				return new TransformFixtureXmlResult(navigator, xmlNamespaceManager);
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

		internal XmlSchemaContentProcessing ContentProcessing { get; private set; }

		internal List<XmlSchema> Schemas { get; private set; }

		internal ValuednessValidationCallback ValuednessValidationCallback { get; private set; }

		private XmlReader CreateValidationAwareReader(Stream transformStream)
		{
			// TODO validation exception should return output/transformed xml content in exception

			return Schemas.Any()
				? XmlReader.Create(transformStream, ValidatingXmlReaderSettings.Create(ContentProcessing, Schemas.ToArray()))
				: XmlReader.Create(transformStream);
		}
	}
}
