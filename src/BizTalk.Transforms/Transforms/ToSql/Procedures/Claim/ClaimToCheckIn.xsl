<?xml version="1.0" encoding="utf-8"?>
<!--
  Copyright © 2012 - 2017 François Chabot, Yves Dierick

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
                xmlns:clm="urn:schemas.stateless.be:biztalk:claim:2017:04"
                xmlns:usp="http://schemas.microsoft.com/Sql/2008/05/TypedProcedures/dbo"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:ctxt="urn:extensions.stateless.be:biztalk:message:context:2012:12"
                xmlns:bf="urn:schemas.stateless.be:biztalk:properties:system:2012:04"
                xmlns:tp="urn:schemas.stateless.be:biztalk:properties:tracking:2012:04"
                exclude-result-prefixes="msxsl clm ctxt bf tp">
  <xsl:output omit-xml-declaration="yes" method="xml" version="1.0" />
  <xsl:strip-space elements="*"/>

  <xsl:template match="clm:CheckIn">
    <usp:usp_claim_CheckIn>
      <!-- for the following properties, take the value that is either embedded in the token or in the context, but give precedence to the embedded value -->
      <xsl:apply-templates select="." mode="correlationToken" />
      <xsl:apply-templates select="." mode="environmentTag" />
      <xsl:apply-templates select="." mode="messageType" />
      <xsl:apply-templates select="." mode="outboundTransportLocation" />
      <xsl:apply-templates select="." mode="processActivityId" />
      <xsl:apply-templates select="." mode="receiverName" />
      <xsl:apply-templates select="." mode="senderName" />
      <usp:url>
        <xsl:value-of select="clm:Url/text()" />
      </usp:url>
      <xsl:apply-templates select="clm:Url/following-sibling::*[1]" />
    </usp:usp_claim_CheckIn>
  </xsl:template>

  <!-- correlationToken -->
  <xsl:template match="clm:CheckIn" mode="correlationToken">
    <xsl:if test="ctxt:Read('bf:CorrelationToken')">
      <usp:correlationToken>
        <xsl:value-of select="ctxt:Read('bf:CorrelationToken')" />
      </usp:correlationToken>
    </xsl:if>
  </xsl:template>
  <xsl:template match="clm:CheckIn[clm:CorrelationToken/text()]" mode="correlationToken">
    <usp:correlationToken>
      <xsl:value-of select="clm:CorrelationToken/text()" />
    </usp:correlationToken>
  </xsl:template>

  <!-- environmentTag -->
  <xsl:template match="clm:CheckIn" mode="environmentTag">
    <xsl:if test="ctxt:Read('bf:EnvironmentTag')">
      <usp:environmentTag>
        <xsl:value-of select="ctxt:Read('bf:EnvironmentTag')" />
      </usp:environmentTag>
    </xsl:if>
  </xsl:template>
  <xsl:template match="clm:CheckIn[clm:EnvironmentTag/text()]" mode="environmentTag">
    <usp:environmentTag>
      <xsl:value-of select="clm:EnvironmentTag/text()" />
    </usp:environmentTag>
  </xsl:template>

  <!-- messageType -->
  <xsl:template match="clm:CheckIn" mode="messageType">
    <xsl:if test="ctxt:Read('bf:ClaimedMessageType')">
      <usp:messageType>
        <xsl:value-of select="ctxt:Read('bf:ClaimedMessageType')" />
      </usp:messageType>
    </xsl:if>
  </xsl:template>
  <xsl:template match="clm:CheckIn[clm:MessageType/text()]" mode="messageType">
    <usp:messageType>
      <xsl:value-of select="clm:MessageType/text()" />
    </usp:messageType>
  </xsl:template>

  <!-- outboundTransportLocation -->
  <xsl:template match="clm:CheckIn" mode="outboundTransportLocation">
    <xsl:if test="ctxt:Read('bf:OutboundTransportLocation')">
      <usp:outboundTransportLocation>
        <xsl:value-of select="ctxt:Read('bf:OutboundTransportLocation')" />
      </usp:outboundTransportLocation>
    </xsl:if>
  </xsl:template>
  <xsl:template match="clm:CheckIn[clm:OutboundTransportLocation/text()]" mode="outboundTransportLocation">
    <usp:outboundTransportLocation>
      <xsl:value-of select="clm:OutboundTransportLocation/text()" />
    </usp:outboundTransportLocation>
  </xsl:template>

  <!-- processActivityId -->
  <xsl:template match="clm:CheckIn" mode="processActivityId">
    <xsl:if test="ctxt:Read('tp:ProcessActivityId')">
      <usp:processActivityId>
        <xsl:value-of select="ctxt:Read('tp:ProcessActivityId')" />
      </usp:processActivityId>
    </xsl:if>
  </xsl:template>
  <xsl:template match="clm:CheckIn[clm:ProcessActivityId/text()]" mode="processActivityId">
    <usp:processActivityId>
      <xsl:value-of select="clm:ProcessActivityId/text()" />
    </usp:processActivityId>
  </xsl:template>

  <!-- receiverName -->
  <xsl:template match="clm:CheckIn" mode="receiverName">
    <xsl:if test="ctxt:Read('bf:ReceiverName')">
      <usp:receiverName>
        <xsl:value-of select="ctxt:Read('bf:ReceiverName')" />
      </usp:receiverName>
    </xsl:if>
  </xsl:template>
  <xsl:template match="clm:CheckIn[clm:ReceiverName/text()]" mode="receiverName">
    <usp:receiverName>
      <xsl:value-of select="clm:ReceiverName/text()" />
    </usp:receiverName>
  </xsl:template>

  <!-- senderName -->
  <xsl:template match="clm:CheckIn" mode="senderName">
    <xsl:if test="ctxt:Read('bf:SenderName')">
      <usp:senderName>
        <xsl:value-of select="ctxt:Read('bf:SenderName')" />
      </usp:senderName>
    </xsl:if>
  </xsl:template>
  <xsl:template match="clm:CheckIn[clm:SenderName/text()]" mode="senderName">
    <usp:senderName>
      <xsl:value-of select="clm:SenderName/text()" />
    </usp:senderName>
  </xsl:template>

  <!-- any -->
  <xsl:template match="*">
    <usp:any>
      <xsl:text disable-output-escaping="yes">&lt;![CDATA[</xsl:text>
      <xsl:apply-templates select="self::* | following-sibling::*" mode="any" />
      <xsl:text disable-output-escaping="yes">]]&gt;</xsl:text>
    </usp:any>
  </xsl:template>

  <!-- this is somehow the identity transform but swallows unwanted xml namespaces -->
  <xsl:template match="*" mode="any">
    <xsl:element name="{name(.)}" namespace="{namespace-uri(.)}">
      <xsl:copy-of select="namespace::*[. != 'urn:schemas.stateless.be:biztalk:claim:2017:04']" />
      <xsl:apply-templates select="@* | node()" mode="any" />
    </xsl:element>
  </xsl:template>

  <xsl:template match="@* | comment() | processing-instruction() | text()" mode="any">
    <xsl:copy />
  </xsl:template>

</xsl:stylesheet>