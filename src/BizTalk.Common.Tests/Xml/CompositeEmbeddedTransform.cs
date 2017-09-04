﻿#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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

namespace Be.Stateless.BizTalk.Xml
{
	[SchemaReference(@"Be.Stateless.BizTalk.Schemas.Xml.Any", typeof(Schemas.Xml.Any))]
	public class CompositeEmbeddedTransform : TransformBase
	{
		static CompositeEmbeddedTransform()
		{
			_xmlContent = ResourceManager.LoadString("Data.CompositeEmbeddedTransform.xsl");
		}

		#region Base Class Member Overrides

		public override string[] SourceSchemas
		{
			get { return new[] { @"Be.Stateless.BizTalk.Schemas.Xml.Any" }; }
		}

		public override string[] TargetSchemas
		{
			get { return new[] { @"Be.Stateless.BizTalk.Schemas.Xml.Any" }; }
		}

		public override string XmlContent
		{
			get { return _xmlContent; }
		}

		public override string XsltArgumentListContent
		{
			get { return @"<ExtensionObjects />"; }
		}

		#endregion

		private static readonly string _xmlContent;
	}
}
