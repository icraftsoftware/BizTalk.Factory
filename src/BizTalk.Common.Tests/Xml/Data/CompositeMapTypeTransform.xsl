<?xml version="1.0" encoding="UTF-16"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:import href="map://type/Be.Stateless.BizTalk.Xml.CompoundMapTypeTransform1, Be.Stateless.BizTalk.Common.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=3707daa0b119fc14" />
  <xsl:include href="map://type/Be.Stateless.BizTalk.Unit.Transform.IdentityTransform, Be.Stateless.BizTalk.Unit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=3707daa0b119fc14" />
  <xsl:variable name="messageType" select="'MESSAGE_TYPE'" />
  <xsl:variable name="attachment-fragment">
    <Attachment></Attachment>
  </xsl:variable>
</xsl:stylesheet>