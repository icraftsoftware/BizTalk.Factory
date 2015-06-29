#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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

using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	[SchemaReference(@"BTF2Schemas.btf2_services_header", typeof(BTF2Schemas.btf2_services_header))]
	[SchemaReference(@"BTF2Schemas.btf2_services_header", typeof(BTF2Schemas.btf2_services_header))]
	public sealed class IdentityTransform : TransformBase
	{
		public override string XmlContent
		{
			get { return XML_CONTENT; }
		}

		public override string XsltArgumentListContent
		{
			get { return @"<ExtensionObjects />"; }
		}

		public override string[] SourceSchemas
		{
			get { return new[] { @"BTF2Schemas.btf2_services_header" }; }
		}

		public override string[] TargetSchemas
		{
			get { return new[] { @"BTF2Schemas.btf2_services_header" }; }
		}

		private const string XML_CONTENT = @"<?xml version='1.0' encoding='UTF-8'?>
<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='1.0'>
  <xsl:output omit-xml-declaration='yes' method='xml' version='1.0' />
  <xsl:template match='@*|node()'>
    <xsl:copy>
      <xsl:apply-templates select='@*|node()'/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>";
	}
}