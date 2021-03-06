﻿#region Copyright & License

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

using Be.Stateless.BizTalk.Unit.Resources;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Schema
{
	[BodyXPath(@"/*[local-name()='Annotated' and namespace-uri()='urn:schemas.stateless.be:biztalk:tests:annotated:2013:01']")]
	[Schema(@"urn:schemas.stateless.be:biztalk:tests:annotated:2013:01", @"Annotated")]
	[SchemaRoots(new[] { @"Annotated" })]
	internal class AnnotatedSchema : SchemaBase
	{
		static AnnotatedSchema()
		{
			_xmlContent = ResourceManager.LoadString("AnnotatedSchema.xsd");
		}

		#region Base Class Member Overrides

		protected override object RawSchema { get; set; }

		public override string XmlContent
		{
			get { return _xmlContent; }
		}

		#endregion

		private static readonly string _xmlContent;
	}
}
