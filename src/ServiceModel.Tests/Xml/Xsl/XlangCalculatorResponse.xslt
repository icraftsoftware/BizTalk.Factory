<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='1.0'>
  <xsl:output omit-xml-declaration='yes' method='xml' version='1.0' />

  <xsl:template match='/'>
    <CalculatorResponse xmlns="urn:services.stateless.be:unit:calculator">
      <xsl:copy>
        <xsl:apply-templates select='@*|node()'/>
      </xsl:copy>
    </CalculatorResponse>
  </xsl:template>

  <xsl:template match='@*|node()'>
    <xsl:copy>
      <xsl:apply-templates select='@*|node()'/>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>