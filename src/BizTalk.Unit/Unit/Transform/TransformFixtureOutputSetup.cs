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
using System.Xml.Schema;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	internal class TransformFixtureOutputSetup : IOutputTransformFixtureSetup, IXmlOutputTransformFixtureSetup
	{
		public TransformFixtureOutputSetup(TransformFixtureInputSetup inputs)
		{
			if (inputs == null) throw new ArgumentNullException("inputs");
			_inputs = inputs;
			ContentProcessing = XmlSchemaContentProcessing.Strict;
			Schemas = new List<XmlSchema>();
		}

		#region IOutputTransformFixtureSetup Members

		public IFirstXmlOutputTransformFixtureSetup OutputsXml()
		{
			return this;
		}

		public ISystemUnderTestSetup<ITransformFixtureTextResult> OutputsText()
		{
			return new TransformWithTextOutputFixtureSetup(_inputs);
		}

		#endregion

		#region IXmlOutputTransformFixtureSetup Members

		public IXmlOutputTransformFixtureSetup ConformingTo<T>() where T : SchemaBase, new()
		{
			Schemas.Add(new T().CreateResolvedSchema());
			return this;
		}

		public ISystemUnderTestSetup<ClosedTransformFixtureXmlResult> WithConformanceLevel(XmlSchemaContentProcessing xmlSchemaContentProcessing)
		{
			ContentProcessing = xmlSchemaContentProcessing;
			return new TransformWithXmlOutputFixtureSetup(_inputs, this);
		}

		public ISystemUnderTestSetup<ClosedTransformFixtureXmlResult> WithNoConformanceLevel()
		{
			return WithConformanceLevel(XmlSchemaContentProcessing.Skip);
		}

		public ISystemUnderTestSetup<ClosedTransformFixtureXmlResult> WithLaxConformanceLevel()
		{
			return WithConformanceLevel(XmlSchemaContentProcessing.Lax);
		}

		public ISystemUnderTestSetup<ClosedTransformFixtureXmlResult> WithStrictConformanceLevel()
		{
			return WithConformanceLevel(XmlSchemaContentProcessing.Strict);
		}

		#endregion

		internal XmlSchemaContentProcessing ContentProcessing { get; private set; }

		internal List<XmlSchema> Schemas { get; private set; }

		private readonly TransformFixtureInputSetup _inputs;
	}
}
