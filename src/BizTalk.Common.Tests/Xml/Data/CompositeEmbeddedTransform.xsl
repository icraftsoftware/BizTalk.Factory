<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:ns="urn:schemas.stateless.be:biztalk:batch:2012:12">

  <xsl:import href="map://type/Be.Stateless.BizTalk.Unit.Transform.IdentityTransform, Be.Stateless.BizTalk.Unit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=3707daa0b119fc14" />
  <xsl:import href="map://resource/Imported.xsl" />
  <xsl:include href="map://resource/Be.Stateless.BizTalk.Xml.Data.Included.xsl" />

  <xsl:template match="ns:ReleaseBatch[3]">dropped by EmbeddedTransform.xsl</xsl:template>

</xsl:stylesheet>