<?xml version="1.0" encoding="utf-8"?>
<!--

  Copyright © 2013 François Chabot, Yves Dierick
  
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
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="msxsl xsd">
  <xsl:output method="xml" indent="yes" cdata-section-elements="passphrase Password password"/>
  <xsl:strip-space elements="*" />

  <!--
      *
      * phased processing:
      *   1. clean up bindings, e.g. discard empty stages, default values, enclose passwords/passphrases in CDATA, etc...
      *   2. clear empty pipelines, i.e. pipelines without any component at any stage
      *
      -->

  <!-- bootstrap -->
  <xsl:template match="/">
    <xsl:apply-templates mode="clean-binding" select="/" />
  </xsl:template>

  <!-- to be called by importing XSLT -->
  <xsl:template mode="clean-binding" match="/">
    <xsl:variable name="cleaned">
      <xsl:apply-templates mode="clean-binding-phase-one" select="/" />
    </xsl:variable>
    <xsl:apply-templates mode="clean-binding-phase-two" select="msxsl:node-set($cleaned)" />
  </xsl:template>

  <!--
      *
      * 1st processing phase - clean up bindings
      *
      -->

  <xsl:template mode="clean-binding-phase-one" match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@*" mode="clean-binding-phase-one" >
        <!-- uncomment the following line should you want to sort the attributes -->
        <!--<xsl:sort select="name(.)" />-->
      </xsl:apply-templates>
      <xsl:apply-templates select="node()" mode="clean-binding-phase-one" />
    </xsl:copy>
  </xsl:template>

  <!-- sort Services, SendPorts, ReceivePorts, ReceiveLocations -->
  <xsl:template mode="clean-binding-phase-one" match="ModuleRefCollection | Services | SendPortCollection | ReceivePortCollection | ReceiveLocations">
    <xsl:copy>
      <xsl:apply-templates select="ModuleRef | Service | SendPort | ReceivePort | ReceiveLocation" mode="clean-binding-phase-one">
        <xsl:sort select="@Name" />
      </xsl:apply-templates>
    </xsl:copy>
  </xsl:template>


  <!-- sort Pipeline Components' Properties but don't take the chance to reorganize them when there could be xml preprocess comments -->
  <xsl:template mode="clean-binding-phase-one" match="Component/Properties[not(comment())]">
    <xsl:copy>
      <xsl:apply-templates select="*" mode="clean-binding-phase-one">
        <xsl:sort select="local-name(.)" />
      </xsl:apply-templates>
    </xsl:copy>
  </xsl:template>

  <!-- sort CustomProps elements but don't take the chance to reorganize them when there could be xml preprocess comments -->
  <xsl:template mode="clean-binding-phase-one" match="CustomProps[not(comment())] | Config[not(comment())]">
    <xsl:copy>
      <xsl:apply-templates select="*" mode="clean-binding-phase-one">
        <xsl:sort select="local-name(.)" />
      </xsl:apply-templates>
    </xsl:copy>
  </xsl:template>

  <!-- discard pipeline's stages for which all of their components have their default values (i.e. no value has been set) -->
  <xsl:template mode="clean-binding-phase-one" match="Stage[not(.//Component/Properties/*/text() or .//Component/Properties/*/*)]" />

  <!-- ensure absolutely no tracking at all is ever performed by DTA/TDDS -->
  <xsl:template mode="clean-binding-phase-one" match="Tracking" />
  <xsl:template mode="clean-binding-phase-one" match="@TrackingOption">
    <xsl:attribute name="{name(.)}">None</xsl:attribute>
  </xsl:template>

  <!-- swallow unmodified (default values), unset, useless, or sometimes offending nodes and attributes -->
  <xsl:template mode="clean-binding-phase-one" match="@*[name(.) != 'matchingPattern' and name(.) != 'replacementPattern' and string-length(.) = 0]">
    <xsl:message>
      <xsl:text>bindings-cleaner.xslt has removed empty attribute: </xsl:text>
      <xsl:value-of select="name(.)"/>
    </xsl:message>
  </xsl:template>
  <xsl:template mode="clean-binding-phase-one" match="Authentication[text() = '0']" />
  <xsl:template mode="clean-binding-phase-one" match="Description[@xsi:nil = 'true']" />
  <xsl:template mode="clean-binding-phase-one" match="@Description[not(text())]" />
  <xsl:template mode="clean-binding-phase-one" match="DeliveryNotification[text() = '1']" />
  <xsl:template mode="clean-binding-phase-one" match="InboundTransforms[not(text())]" />
  <xsl:template mode="clean-binding-phase-one" match="OrderedDelivery[text() = 'false']" />
  <xsl:template mode="clean-binding-phase-one" match="Priority[text() = '5']" />
  <xsl:template mode="clean-binding-phase-one" match="RouteFailedMessage" />
  <xsl:template mode="clean-binding-phase-one" match="SecondaryTransport[not(Address/text())]" />
  <xsl:template mode="clean-binding-phase-one" match="StopSendingOnFailure[text() = 'false']" />
  <xsl:template mode="clean-binding-phase-one" match="Transforms[not(text())]" />
  <xsl:template mode="clean-binding-phase-one" match="TrackedSchemas/node() | TrackedSchemas/@*" />
  <xsl:template mode="clean-binding-phase-one" match="Host/@NTGroupName" />

  <!-- swallow unset service window elements -->
  <xsl:template mode="clean-binding-phase-one" match="ServiceWindowEnabled[text() = 'false']" />
  <xsl:template mode="clean-binding-phase-one" match="FromTime[preceding-sibling::ServiceWindowEnabled/text() = 'false']" />
  <xsl:template mode="clean-binding-phase-one" match="ToTime[preceding-sibling::ServiceWindowEnabled/text() = 'false']" />
  <xsl:template mode="clean-binding-phase-one" match="ReceiveLocationServiceWindowEnabled[text() = 'false']" />
  <xsl:template mode="clean-binding-phase-one" match="ReceiveLocationFromTime[preceding-sibling::ReceiveLocationServiceWindowEnabled/text() = 'false']" />
  <xsl:template mode="clean-binding-phase-one" match="ReceiveLocationToTime[preceding-sibling::ReceiveLocationServiceWindowEnabled/text() = 'false']" />
  <xsl:template mode="clean-binding-phase-one" match="ReceiveLocationStartDateEnabled[text() = 'false']" />
  <xsl:template mode="clean-binding-phase-one" match="ReceiveLocationStartDate[preceding-sibling::ReceiveLocationStartDateEnabled/text() = 'false']" />
  <xsl:template mode="clean-binding-phase-one" match="ReceiveLocationEndDateEnabled[text() = 'false']" />
  <xsl:template mode="clean-binding-phase-one" match="ReceiveLocationEndDate[preceding-sibling::ReceiveLocationEndDateEnabled/text() = 'false']" />

  <!--
      *
      * 2nd processing phase - clear empty pipelines
      *
      -->

  <xsl:template mode="clean-binding-phase-two" match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" mode="clean-binding-phase-two" />
    </xsl:copy>
  </xsl:template>

  <xsl:template mode="clean-binding-phase-two" match="ReceivePipelineData[not(*/Stages/*)]">
    <ReceivePipelineData xsi:nil="true" />
  </xsl:template>

  <xsl:template mode="clean-binding-phase-two" match="SendPipelineData[not(*/Stages/*)]">
    <SendPipelineData xsi:nil="true" />
  </xsl:template>

</xsl:stylesheet>
