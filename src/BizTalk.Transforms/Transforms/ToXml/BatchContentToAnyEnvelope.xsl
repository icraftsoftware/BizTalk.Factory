<?xml version="1.0" encoding="utf-8"?>
<!--
  Copyright © 2012 François Chabot, Yves Dierick

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
                xmlns:agg="http://schemas.microsoft.com/BizTalk/2003/aggschema"
                xmlns:bat="urn:schemas.stateless.be:biztalk:batch:2012:12"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="agg bat msxsl">
  <xsl:output omit-xml-declaration="yes" method="xml" version="1.0" />

  <xsl:template match="/">
    <xsl:apply-templates select="/*/agg:InputMessagePart_0/*" />
  </xsl:template>

  <xsl:template match="bat:parts-here">
    <xsl:apply-templates select="/*/agg:InputMessagePart_1/bat:BatchContent/bat:Parts/*" />
  </xsl:template>

  <!-- this is somehow the identity transform but swallows unwanted xml namespaces -->
  <xsl:template match="*">
    <xsl:element name="{name(.)}" namespace="{namespace-uri(.)}">
      <xsl:copy-of select="namespace::*[. != 'urn:schemas.stateless.be:biztalk:batch:2012:12' and . != 'http://schemas.microsoft.com/BizTalk/2003/aggschema']" />
      <xsl:apply-templates select="@* | node()" />
    </xsl:element>
  </xsl:template>

  <xsl:template match="@* | comment() | processing-instruction() | text()">
    <xsl:copy />
  </xsl:template>

</xsl:stylesheet>