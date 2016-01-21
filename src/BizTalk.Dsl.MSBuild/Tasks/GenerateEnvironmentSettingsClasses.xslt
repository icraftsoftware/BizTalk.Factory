<?xml version="1.0" encoding="utf-8"?>
<!--

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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

-->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
                xmlns:string="urn:extensions.stateless.be:biztalk:environment-settings-class-generation:string:2015:10"
                xmlns:type="urn:extensions.stateless.be:biztalk:environment-settings-class-generation:typifier:2015:10"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt">
  <xsl:output method="text" indent="no"/>

  <xsl:param name="clr-namespace-name" />
  <xsl:param name="clr-class-name" />
  <xsl:param name="settings-file-name" />

  <xsl:template match="text()" />

  <xsl:template match="/ss:Workbook/ss:Worksheet[@ss:Name='Settings']/ss:Table">
    <xsl:text>#region Copyright &amp; License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Install;

namespace </xsl:text>
    <xsl:value-of select="$clr-namespace-name" />
    <xsl:text>
{
	[GeneratedCode("EnvironmentSettings", "1.0.0.0")]
	public class </xsl:text>
    <xsl:value-of select="$clr-class-name" />
    <xsl:text> : Be.Stateless.BizTalk.Dsl.Binding.EnvironmentSettings
	{
		static </xsl:text>
    <xsl:value-of select="$clr-class-name" />
    <xsl:text>()
		{
			_instance = new </xsl:text>
    <xsl:value-of select="$clr-class-name" />
    <xsl:text>();
		}
</xsl:text>
    <xsl:apply-templates />
    <xsl:text>
		protected override string SettingsFileName
		{
			get { return "</xsl:text>
    <xsl:value-of select="$settings-file-name" />
    <xsl:text>"; }
		}

		protected override string[] TargetEnvironments
		{
			get { return _targetEnvironments; }
		}

		private static readonly </xsl:text>
    <xsl:value-of select="$clr-class-name" />
    <xsl:text> _instance;
		private static readonly string[] _targetEnvironments = { </xsl:text>
    <xsl:apply-templates select="ss:Row[ss:Cell[1]/ss:Data/text()='TargetEnvironment']/ss:Cell" mode="values" />
    <xsl:text> };
	}
}
</xsl:text>
  </xsl:template>

  <!-- DateTime property row -->
  <xsl:template match="ss:Row[ss:Cell[position()>2]/ss:Data[@ss:Type='DateTime']]/ss:Cell[position()=1][ss:Data/text()]">
    <!--
    TODO parse and distinguish TimeSpan from DateTime
    <Cell><Data ss:Type="DateTime">2015-12-01T00:00:00.000</Data></Cell>
    <Cell><Data ss:Type="DateTime">1899-12-31T00:00:10.000</Data></Cell>
    -->
    <!--
    <xsl:call-template name="generate-property-getter">
      <xsl:with-param name="type" select="'DateTime'" />
    </xsl:call-template>
    -->
    <xsl:message terminate="yes">Not Implemented.</xsl:message>
  </xsl:template>

  <!-- integer property row -->
  <xsl:template match="ss:Row[ss:Cell[position()>2]/ss:Data[@ss:Type='Number']]/ss:Cell[position()=1][ss:Data/text()]">
    <xsl:call-template name="generate-property-getter">
      <xsl:with-param name="type" select="'int'" />
    </xsl:call-template>
  </xsl:template>

  <!-- double property row : i.e. a number property whose not every data value is an integer -->
  <xsl:template match="ss:Row[ss:Cell[position()>2]/ss:Data[@ss:Type='Number'][not(type:IsInteger(text()))]]/ss:Cell[position()=1][ss:Data/text()]">
    <xsl:call-template name="generate-property-getter">
      <xsl:with-param name="type" select="'double'" />
    </xsl:call-template>
  </xsl:template>

  <!-- string property row -->
  <xsl:template match="ss:Row[ss:Cell[position()>2]/ss:Data[@ss:Type='String']]/ss:Cell[position()=1][ss:Data/text()]">
    <xsl:call-template name="generate-property-getter">
      <xsl:with-param name="type" select="'string'" />
    </xsl:call-template>
  </xsl:template>

  <!-- property row whose not all values are of the same type -->
  <xsl:template match="ss:Row[ss:Cell[position()>2][ss:Data/@ss:Type != following-sibling::ss:Cell/ss:Data/@ss:Type]]">
    <xsl:message terminate="yes">
      <xsl:text>Not all the values are of the same type for property '</xsl:text>
      <xsl:value-of select="ss:Cell[1]/ss:Data/text()"/>
      <xsl:text>'.</xsl:text>
    </xsl:message>
  </xsl:template>

  <!-- property row that has more values than there are target environments -->
  <xsl:template match="ss:Row[count(ss:Cell[position()>2][ss:Data/text()]) > count(preceding-sibling::ss:Row[ss:Cell[1]/ss:Data/text()='TargetEnvironment']/ss:Cell[position()>2])]">
    <xsl:message terminate="yes">
      <xsl:text>Property '</xsl:text>
      <xsl:value-of select="ss:Cell[1]/ss:Data/text()"/>
      <xsl:text>' has more values than they are target environments.</xsl:text>
    </xsl:message>
  </xsl:template>

  <!-- irrelevant or discarded property rows -->
  <xsl:template match="ss:Row[ss:Cell[1]/ss:Data[text()='Environment Name:' or text()='Generate File?' or text()='Settings File Name:' or text()='TargetEnvironment']]" />

  <!-- property row without values -->
  <xsl:template match="ss:Row[not(ss:Cell[position()>1][ss:Data/text()])]" />

  <!-- property getter code template -->
  <xsl:template name="generate-property-getter">
    <xsl:param name="type" />
    <xsl:text>
		public static </xsl:text>
    <xsl:value-of select="$type"/>
    <xsl:text> </xsl:text>
    <xsl:value-of select="translate(ss:Data/text(), '.- ', '')"/>
    <xsl:text> 
		{
			get { return _instance.ValueForTargetEnvironment(new </xsl:text>
    <xsl:value-of select="$type"/>
    <xsl:if test="$type != 'string'">?</xsl:if>
    <xsl:text>[] { </xsl:text>
    <xsl:apply-templates select="following-sibling::ss:Cell" mode="values" />
    <xsl:text> }); }
		}
</xsl:text>
  </xsl:template>

  <!-- value list -->
  <xsl:template match="text()" mode="values" />

  <xsl:template match="ss:Cell[position()>1]" mode="values">
    <xsl:apply-templates select="." mode="literal"/>
    <xsl:text>, </xsl:text>
  </xsl:template>

  <xsl:template match="ss:Cell[last()]" mode="values">
    <xsl:apply-templates select="." mode="literal"/>
  </xsl:template>

  <!-- value literal -->
  <xsl:template match="ss:Cell[ss:Data/@ss:Type='DateTime']" mode="literal">
    <!--
    TODO parse and distinguish TimeSpan from DateTime
    <Cell><Data ss:Type="DateTime">2015-12-01T00:00:00.000</Data></Cell>
    <Cell><Data ss:Type="DateTime">1899-12-31T00:00:10.000</Data></Cell>
    -->
    <!--<xsl:value-of select="ss:Data/text()" />-->
    <xsl:message terminate="yes">Not Implemented.</xsl:message>
  </xsl:template>

  <xsl:template match="ss:Cell[ss:Data/@ss:Type='Number']" mode="literal">
    <xsl:value-of select="ss:Data/text()" />
  </xsl:template>

  <xsl:template match="ss:Cell[ss:Data/@ss:Type='String']" mode="literal">
    <xsl:value-of select="string:Escape(ss:Data/text())" />
  </xsl:template>

  <xsl:template match="ss:Cell[not(ss:Data/text())]" mode="literal">
    <xsl:text>null</xsl:text>
  </xsl:template>

</xsl:stylesheet>
