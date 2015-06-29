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
                xmlns:bat="urn:schemas.stateless.be:biztalk:batch:2012:12"
                xmlns:usp="http://schemas.microsoft.com/Sql/2008/05/TypedProcedures/dbo"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:ctxt="urn:extensions.stateless.be:biztalk:message:context:2012:12"
                xmlns:tp="urn:schemas.stateless.be:biztalk:properties:tracking:2012:04"
                exclude-result-prefixes="msxsl bat ctxt tp">
  <xsl:output omit-xml-declaration="yes" method="xml" version="1.0" />

  <xsl:template match="text()" />

  <xsl:template match="bat:ReleaseBatch">
    <usp:usp_batch_QueueControlledRelease>
      <usp:envelopeSpecName>
        <xsl:value-of select="bat:EnvelopeSpecName/text()" />
      </usp:envelopeSpecName>
      <xsl:if test="bat:Partition/text()">
        <usp:partition>
          <xsl:value-of select="bat:Partition/text()" />
        </usp:partition>
      </xsl:if>
      <xsl:variable name="processActivityId" select="ctxt:Read('tp:ProcessActivityId')" />
      <xsl:if test="$processActivityId">
        <usp:processActivityId>
          <xsl:value-of select="$processActivityId" />
        </usp:processActivityId>
      </xsl:if>
    </usp:usp_batch_QueueControlledRelease>
  </xsl:template>
</xsl:stylesheet>