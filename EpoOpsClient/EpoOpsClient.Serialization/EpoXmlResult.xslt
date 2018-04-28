<?xml version="1.0" encoding="utf-8"?>
<!--
Copyright(c) 2018 by Keith Bielaczyc. All Right Reserved.

Unless required by applicable law or agreed to in writing,
software distributed under the License is distributed on an
"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, either express or implied.See the License for the
specific language governing permissions and limitations
under the License.
-->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:ops="http://ops.epo.org"
    xmlns:reg="http://www.epo.org/register"
    xmlns:xlink="http://www.w3.org/1999/xlink"
    xmlns:cpc="http://www.epo.org/cpcexport"
    xmlns:cpcdef="http://www.epo.org/cpcdefinition"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl ops reg xlink cpc cpcdef">
<xsl:output encoding="utf-8" method="text" indent="no"/>
<xsl:variable name="TAB"><xsl:text>&#x09;</xsl:text></xsl:variable>
<xsl:template match="//reg:register-documents/reg:register-document">
<!--
Application Date	
-->
<xsl:text>"</xsl:text>
<xsl:call-template name="formatDate">
	<xsl:with-param name="input" select="reg:bibliographic-data/reg:application-reference/reg:document-id[reg:country[text() = 'EP']]/reg:date"></xsl:with-param>
</xsl:call-template>
<xsl:text>"</xsl:text>
<xsl:value-of select="$TAB"/>
<!--
Application Number
-->
<xsl:text>"</xsl:text><xsl:value-of select="reg:bibliographic-data/reg:application-reference/reg:document-id[reg:country[text() = 'EP']]/reg:country"/><xsl:value-of select="reg:bibliographic-data/reg:application-reference/reg:document-id[reg:country[text() = 'EP']]/reg:doc-number"/><xsl:text>"</xsl:text>
<xsl:value-of select="$TAB"/>
<!--
Current Owner
-->
<xsl:text>"</xsl:text>
<xsl:value-of select="reg:bibliographic-data[@country='EP']/reg:parties//reg:applicant//reg:name"/>
<xsl:text>"</xsl:text>
<xsl:value-of select="$TAB"/>
<!--
EPO ID
-->
<xsl:text>"</xsl:text>
<xsl:value-of select="reg:bibliographic-data[@country='EP']/@id"/>
<xsl:text>"</xsl:text>
<xsl:value-of select="$TAB"/>
<!--
Grant Date	
-->
<xsl:text>"</xsl:text>
<xsl:variable name="patent-grant-date" select="reg:bibliographic-data/reg:publication-reference/reg:document-id[reg:country[text() = 'EP'] and reg:kind[text() = 'B1']]/reg:date"/>
<xsl:choose>
<xsl:when test="$patent-grant-date">
<xsl:call-template name="formatDate">
<xsl:with-param name="input" select="$patent-grant-date"></xsl:with-param>
</xsl:call-template>
</xsl:when>
</xsl:choose>
<xsl:text>"</xsl:text>
<xsl:value-of select="$TAB"/>
<!--
Patent Number	
-->
<xsl:text>"</xsl:text>
<xsl:value-of select="reg:bibliographic-data/reg:publication-reference/reg:document-id[reg:country[text() = 'EP'] and reg:kind[text() = 'B1']]/reg:country"/>
<xsl:value-of select="reg:bibliographic-data/reg:publication-reference/reg:document-id[reg:country[text() = 'EP'] and reg:kind[text() = 'B1']]/reg:doc-number"/>
<xsl:text>"</xsl:text>
<xsl:value-of select="$TAB"/>
<!--
Renewal Year	
Renewal Fee Paid Date	
-->
<xsl:variable name="renewalFee">
	<xsl:for-each select="reg:procedural-data">
		<xsl:for-each select="reg:procedural-step[reg:procedural-step-code[text() = 'RFEE']]">
			<xsl:sort select="reg:procedural-step-text[@step-text-type = 'YEAR']" data-type="number" order="descending"/>
			<xsl:if test="position() = 1">
				<xsl:text>"</xsl:text><xsl:value-of select="reg:procedural-step-text[@step-text-type = 'YEAR']"/><xsl:text>"</xsl:text>
        <xsl:value-of select="$TAB"/>
        <xsl:text>"</xsl:text><xsl:call-template name="formatDate"><xsl:with-param name="input" select="reg:procedural-step-date[@step-date-type = 'DATE_OF_PAYMENT']/reg:date"></xsl:with-param></xsl:call-template><xsl:text>"</xsl:text>
			</xsl:if>
		</xsl:for-each>
	</xsl:for-each>
</xsl:variable>
<xsl:if test="string-length($renewalFee)">
	<xsl:value-of select="$renewalFee"/>
</xsl:if>
<xsl:if test="string-length($renewalFee)=0">
  <xsl:text>""</xsl:text><xsl:value-of select="$TAB"/><xsl:text>""</xsl:text>
</xsl:if>
<xsl:value-of select="$TAB"/>
<!--
Status
-->
<xsl:text>"</xsl:text><xsl:value-of select="reg:bibliographic-data/@status"/><xsl:text>"</xsl:text>
</xsl:template>
<xsl:template name="formatDate">
  <xsl:param name="input"/>
  <xsl:if test="0 &lt; string-length($input)">
    <xsl:variable name="year" select="substring($input,1,4)"/>
    <xsl:variable name="month" select="substring($input,5,2)"/>
    <xsl:variable name="day" select="substring($input,7,2)"/>
    <xsl:value-of select="concat($month,'/',$day,'/',$year)"/>
  </xsl:if>
</xsl:template>
<xsl:template match="@* | node()">
  <xsl:apply-templates select="@* | node()"/>
</xsl:template>
</xsl:stylesheet>
