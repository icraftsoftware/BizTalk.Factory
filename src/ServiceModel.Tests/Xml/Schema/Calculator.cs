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

using System;
using Be.Stateless.BizTalk.Unit.Resources;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.Xml.Schema
{
	[SchemaType(SchemaTypeEnum.Document)]
	[SchemaRoots(new[] { @"CalculatorRequest", @"CalculatorResponse", @"LaxArguments", @"LaxCalculatorRequest", @"LaxCalculatorResponse", @"LaxResult" })]
	[Serializable]
	public sealed class Calculator : SchemaBase
	{
		#region Nested type: LaxArguments

		[Schema(@"urn:services.stateless.be:unit:calculator", @"LaxArguments")]
		[SchemaRoots(new[] { @"LaxArguments" })]
		[Serializable]
		public sealed class LaxArguments : SchemaBase
		{
			#region Base Class Member Overrides

			protected override object RawSchema
			{
				get { return null; }
				set { }
			}

			public override string[] RootNodes
			{
				get { return new[] { "LaxArguments" }; }
			}

			public override string XmlContent
			{
				get { return _xmlContent; }
			}

			#endregion
		}

		#endregion

		#region Nested type: Request

		[Schema(@"urn:services.stateless.be:unit:calculator", @"CalculatorRequest")]
		[SchemaRoots(new[] { @"CalculatorRequest" })]
		[Serializable]
		public sealed class Request : SchemaBase
		{
			#region Base Class Member Overrides

			protected override object RawSchema
			{
				get { return null; }
				set { }
			}

			public override string[] RootNodes
			{
				get { return new[] { "CalculatorRequest" }; }
			}

			public override string XmlContent
			{
				get { return _xmlContent; }
			}

			#endregion
		}

		#endregion

		#region Nested type: Response

		[Schema(@"urn:services.stateless.be:unit:calculator", @"CalculatorResponse")]
		[SchemaRoots(new[] { @"CalculatorResponse" })]
		[Serializable]
		public sealed class Response : SchemaBase
		{
			#region Base Class Member Overrides

			protected override object RawSchema
			{
				get { return null; }
				set { }
			}

			public override string[] RootNodes
			{
				get { return new[] { "CalculatorResponse" }; }
			}

			public override string XmlContent
			{
				get { return _xmlContent; }
			}

			#endregion
		}

		#endregion

		static Calculator()
		{
			_xmlContent = ResourceManager.LoadString("Calculator.xsd");
		}

		#region Base Class Member Overrides

		protected override object RawSchema
		{
			get { return null; }
			set { }
		}

		public override string[] RootNodes
		{
			get { return new[] { @"CalculatorRequest", @"CalculatorResponse", @"LaxArguments", @"LaxCalculatorRequest", @"LaxCalculatorResponse", @"LaxResult" }; }
		}

		public override string XmlContent
		{
			get { return _xmlContent; }
		}

		#endregion

		[NonSerialized]
		private static readonly string _xmlContent;
	}
}
