<?xml version="1.0" encoding="utf-8"?>
<!--
  Copyright © 2012 - 2013 François Chabot, Yves Dierick

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
                xmlns:usp="http://schemas.microsoft.com/Sql/2008/05/TypedProcedures/dbo"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:ctxt="urn:extensions.stateless.be:biztalk:message:context:2012:12"
                xmlns:bf="urn:schemas.stateless.be:biztalk:properties:system:2012:04"
                xmlns:tp="urn:schemas.stateless.be:biztalk:properties:tracking:2012:04"
                exclude-result-prefixes="msxsl ctxt bf tp">
  <xsl:output omit-xml-declaration="yes" method="xml" version="1.0" />
  <xsl:strip-space elements="*" />

  <xsl:template match="/">
    <usp:usp_batch_AddPart>
      <usp:envelopeSpecName>
        <xsl:value-of select="ctxt:Read('bf:EnvelopeSpecName')" />
      </usp:envelopeSpecName>
      <xsl:variable name="partition" select="ctxt:Read('bf:EnvelopePartition')" />
      <xsl:if test="$partition">
        <usp:partition>
          <xsl:value-of select="$partition" />
        </usp:partition>
      </xsl:if>
      <xsl:variable name="messagingStepActivityId" select="ctxt:Read('tp:MessagingStepActivityId')" />
      <xsl:if test="$messagingStepActivityId">
        <usp:messagingStepActivityId>
          <xsl:value-of select="$messagingStepActivityId" />
        </usp:messagingStepActivityId>
      </xsl:if>
      <usp:data>
        <xsl:text disable-output-escaping="yes">&lt;![CDATA[</xsl:text>
        <xsl:apply-templates />
        <xsl:text disable-output-escaping="yes">]]&gt;</xsl:text>
      </usp:data>
    </usp:usp_batch_AddPart>
  </xsl:template>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>