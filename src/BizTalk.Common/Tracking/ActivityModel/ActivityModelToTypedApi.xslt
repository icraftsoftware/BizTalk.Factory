<?xml version="1.0" encoding="utf-8" ?>
<!--

// Copyright © 2012 François Chabot, Yves Dierick
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
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
                xmlns:bam="http://schemas.microsoft.com/BizTalkServer/2004/10/BAM"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:script="urn:schemas.stateless.be:xslt:scripts:bam-api:2012:04">

  <xsl:output method="text" encoding="utf-8" omit-xml-declaration="yes" />

  <!-- either Buffered, Direct, or Orchestration -->
  <xsl:param name="EventStreamType" />
  <xsl:param name="TargetNamespace" />

  <xsl:template match="bam:BAMDefinition">
    <xsl:if test="not($EventStreamType) or not($TargetNamespace)">
      <xsl:message terminate="yes">One or both of the EventStreamType and TargetNamespace parameters have no value.</xsl:message>
    </xsl:if>
    <xsl:text>#region Copyright &amp; License

// Copyright © 2012 François Chabot, Yves Dierick
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
using System.Collections.Generic;
using Microsoft.BizTalk.Bam.EventObservation;

namespace </xsl:text><xsl:value-of select="$TargetNamespace" /><xsl:text>
{</xsl:text>

    <xsl:apply-templates select="bam:Activity" />

    <xsl:text>
}</xsl:text>
  </xsl:template>

  <xsl:template match="bam:Activity">
    <xsl:variable name="Name" select="@Name" />
    <xsl:variable name="CompressedDisplayName" select="translate(@Name, ' ', '')" />
    <xsl:text>
	[GeneratedCode("BamActivityModel", "1.1.0.0")]
	[Serializable]
	public partial class </xsl:text><xsl:value-of select="$CompressedDisplayName" /><xsl:text>
	{
		public const string ActivityName = "</xsl:text><xsl:value-of select="@Name" /><xsl:text>";
		internal const string ContinuationPrefix = "CONT_";

		private readonly string _activityId;
		private readonly Dictionary&lt;string, object&gt; _activityItems = new Dictionary&lt;string, object&gt;();
</xsl:text>

    <xsl:if test="$EventStreamType = 'Direct'">
      <xsl:text>		private readonly string _connectionString;
</xsl:text>
    </xsl:if>

    <xsl:if test="$EventStreamType != 'Orchestration'">
      <xsl:text>		private readonly EventStream _eventStream;
</xsl:text>
    </xsl:if>

    <xsl:if test="$EventStreamType = 'Buffered'">
      <xsl:text>
		public </xsl:text><xsl:value-of select="$CompressedDisplayName" /><xsl:text>(string activityId, EventStream eventStream)
		{
			if (string.IsNullOrEmpty(activityId)) throw new ArgumentNullException("activityId", "activityId is required.");
			if (eventStream == null) throw new ArgumentNullException("eventStream", "eventStream is required.");
			_activityId = activityId;
			_eventStream = eventStream;
		}
</xsl:text>
    </xsl:if>

    <xsl:if test="$EventStreamType = 'Orchestration'">
      <xsl:text>
		public </xsl:text><xsl:value-of select="$CompressedDisplayName" /><xsl:text>(string activityId)
		{
			if (string.IsNullOrEmpty(activityId)) throw new ArgumentNullException("activityId", "activityId is required.");
			_activityId = activityId;
		}
</xsl:text>
    </xsl:if>

    <xsl:text>
		public string ActivityId
		{
			get { return _activityId; }
		}
</xsl:text>

    <xsl:if test="$EventStreamType = 'Direct'">
      <xsl:text>
		public string ConnectionString
		{
			get { return _connectionString; }
		}
</xsl:text>
    </xsl:if>

    <xsl:apply-templates select="bam:Checkpoint" />

    <xsl:text>
		/// &lt;summary&gt;
		/// Begins the BAM activity.
		/// &lt;/summary&gt;
		public void Begin</xsl:text><xsl:value-of select="$CompressedDisplayName" /><xsl:text>Activity()
		{
			// Begin the Activity using the passed identifier</xsl:text>
    <xsl:if test="$EventStreamType != 'Orchestration'">
      <xsl:text>
			EventStream es = _eventStream;
			es.BeginActivity(ActivityName, _activityId);</xsl:text>
    </xsl:if>
    <xsl:if test="$EventStreamType = 'Orchestration'">
      <xsl:text>
			OrchestrationEventStream.BeginActivity(ActivityName, _activityId);</xsl:text>
    </xsl:if>
    <xsl:text>
		}
</xsl:text>

    <xsl:text>
		/// &lt;summary&gt;
		/// Write any data changes to the BAM activity. This must be called after any property changes.
		/// &lt;/summary&gt;
		public void Commit</xsl:text><xsl:value-of select="$CompressedDisplayName" /><xsl:text>Activity()
		{
			// We need to provide the key/value pairs to the BAM API
			var al = new List&lt;object&gt;();
			foreach (var kvp in _activityItems)
			{
				al.Add(kvp.Key);
				al.Add(kvp.Value);
			}

			// Update the BAM Activity with all of the data</xsl:text>
    <xsl:if test="$EventStreamType != 'Orchestration'">
      <xsl:text>
			EventStream es = _eventStream;
			es.UpdateActivity(ActivityName, _activityId, al.ToArray());
			es.Flush();</xsl:text>
    </xsl:if>
    <xsl:if test="$EventStreamType = 'Orchestration'">
      <xsl:text>
			OrchestrationEventStream.UpdateActivity(ActivityName, _activityId, al.ToArray());</xsl:text>
    </xsl:if><xsl:text>
		}
</xsl:text>

    <xsl:text>
		/// &lt;summary&gt;
		/// End the BAM activity. No more changes will be permitted to this activity except by continuation.
		/// &lt;/summary&gt;
		public void End</xsl:text><xsl:value-of select="$CompressedDisplayName" /><xsl:text>Activity()
		{
			// End this activity, no more data can be added.</xsl:text>
    <xsl:if test="$EventStreamType != 'Orchestration'">
      <xsl:text>
			EventStream es = _eventStream;
			es.EndActivity(ActivityName, _activityId);</xsl:text>
    </xsl:if>
    <xsl:if test="$EventStreamType = 'Orchestration'">
      <xsl:text>
			OrchestrationEventStream.EndActivity(ActivityName, _activityId);</xsl:text>
    </xsl:if>
    <xsl:text>
		}
</xsl:text>

    <xsl:text>
		/// &lt;summary&gt;
		/// Add a reference from this activity to another activity.
		/// &lt;/summary&gt;
		/// &lt;param name="otherActivityName"&gt;The related activity name. Reference names are limited to 128 characters.&lt;/param&gt;
		/// &lt;param name="otherActivityId"&gt;The related activity Id. Limited to 1024 characters of data.&lt;/param&gt;
		public void AddReferenceToAnotherActivity(string otherActivityName, string otherActivityId)
		{
			AddCustomReference("Activity", otherActivityName, otherActivityId);
		}
</xsl:text>

    <xsl:text>
		/// &lt;summary&gt;
		/// Add a custom reference to this activity, this enables 'data' to be attached to an activity, such as a message body.
		/// &lt;/summary&gt;
		/// &lt;param name="referenceType"&gt;The related item type. Reference type identifiers are limited to 128 characters.&lt;/param&gt;
		/// &lt;param name="referenceName"&gt;The related item name. Reference names are limited to 128 characters.&lt;/param&gt;
		/// &lt;param name="referenceData"&gt;The related item data. Limited to 1024 characters of data.&lt;/param&gt;
		/// &lt;remarks&gt;See http://msdn.microsoft.com/en-us/library/aa956648(BTS.10).aspx&lt;/remarks&gt;
		public void AddCustomReference(string referenceType, string referenceName, string referenceData)
		{
			// Add a reference to another activity</xsl:text>
		<xsl:if test="$EventStreamType != 'Orchestration'">
      <xsl:text>
			EventStream es = _eventStream;
			es.AddReference(ActivityName, _activityId, referenceType, referenceName, referenceData);</xsl:text>
    </xsl:if>
    <xsl:if test="$EventStreamType = 'Orchestration'">
      <xsl:text>
			OrchestrationEventStream.AddReference(ActivityName, _activityId, referenceType, referenceName, referenceData);</xsl:text>
    </xsl:if>
    <xsl:text>
		}
</xsl:text>

    <xsl:text>
		/// &lt;summary&gt;
		/// Add a custom reference to this activity, this enables 'data' to be attached to an activity, such as a message body.
		/// &lt;/summary&gt;
		/// &lt;param name="referenceType"&gt;The related item type. Reference type identifiers are limited to 128 characters.&lt;/param&gt;
		/// &lt;param name="referenceName"&gt;The related item name. Reference names are limited to 128 characters.&lt;/param&gt;
		/// &lt;param name="referenceData"&gt;The related item data. Limited to 1024 characters of data.&lt;/param&gt;
		/// &lt;param name="longReferenceData"&gt;The related item data containing up to 512 KB of Unicode characters of data.&lt;/param&gt;
		/// &lt;remarks&gt;See http://msdn.microsoft.com/en-us/library/aa956648(BTS.10).aspx&lt;/remarks&gt;
		public void AddCustomReference(string referenceType, string referenceName, string referenceData, string longReferenceData)
		{
			// Add a reference to another activity</xsl:text>
    <xsl:if test="$EventStreamType != 'Orchestration'">
      <xsl:text>
			EventStream es = _eventStream;
			es.AddReference(ActivityName, _activityId, referenceType, referenceName, referenceData, longReferenceData);</xsl:text>
    </xsl:if>
    <xsl:if test="$EventStreamType = 'Orchestration'">
      <xsl:text>
			OrchestrationEventStream.AddReference(ActivityName, _activityId, referenceType, referenceName, referenceData, longReferenceData);</xsl:text>
    </xsl:if>
    <xsl:text>
		}
</xsl:text>

    <xsl:text>
		/// &lt;summary&gt;
		/// Activate continuation for this activity. While in the context that is enabling continuation, this activity can
		/// still be updated and MUST be ended with a call to End</xsl:text><xsl:value-of select="$CompressedDisplayName" /><xsl:text>Activity().
		/// &lt;/summary&gt;
		public string EnableContinuation()
		{
			string continuationId = ContinuationPrefix + _activityId;</xsl:text>
    <xsl:if test="$EventStreamType != 'Orchestration'">
      <xsl:text>
			EventStream es = _eventStream;
			es.EnableContinuation(ActivityName, _activityId, continuationId);</xsl:text>
		</xsl:if>
		<xsl:if test="$EventStreamType = 'Orchestration'">
      <xsl:text>
			OrchestrationEventStream.EnableContinuation(ActivityName, _activityId, continuationId);</xsl:text>
		</xsl:if>
    <xsl:text>
			return continuationId;
		}
</xsl:text>

    <xsl:if test="$EventStreamType != 'Orchestration'">
      <xsl:text>
		/// &lt;summary&gt;
		/// Flush any buffered events.
		/// &lt;/summary&gt;
		public void Flush()
		{
			EventStream es = _eventStream;
			es.Flush();
		}
</xsl:text>
    </xsl:if>

    <xsl:text>	}
</xsl:text>
  </xsl:template>

  <xsl:template match="bam:Checkpoint[@DataType='DATETIME' or @DataType='FLOAT' or @DataType='INT']">
    <xsl:variable name="Name" select="@Name" />
    <xsl:variable name="CompressedDisplayName" select="translate(@Name, ' ', '')" />
    <xsl:variable name="DataType" select="concat(script:ResolveCLRType(@DataType),'?')" />
    <xsl:text>
		internal const string </xsl:text><xsl:value-of select="$CompressedDisplayName" /><xsl:text>FieldName = "</xsl:text><xsl:value-of select="@Name" /><xsl:text>";
		public </xsl:text><xsl:value-of select="$DataType" /><xsl:text xml:space="preserve"> </xsl:text><xsl:value-of select="$CompressedDisplayName" /><xsl:text>
		{
			get { return (</xsl:text><xsl:value-of select="$DataType" /><xsl:text>) _activityItems[</xsl:text><xsl:value-of select="$CompressedDisplayName" /><xsl:text>FieldName]; }
			set { if (value.HasValue) _activityItems[</xsl:text><xsl:value-of select="$CompressedDisplayName" /><xsl:text>FieldName] = value.Value; }
		}
</xsl:text>
  </xsl:template>

  <xsl:template match="bam:Checkpoint[@DataType='NVARCHAR']">
    <xsl:variable name="Name" select="@Name" />
    <xsl:variable name="CompressedDisplayName" select="translate(@Name, ' ', '')" />
    <xsl:variable name="DataType" select="script:ResolveCLRType(@DataType)" />
    <xsl:text>
		internal const string </xsl:text><xsl:value-of select="$CompressedDisplayName" /><xsl:text>FieldName = "</xsl:text><xsl:value-of select="@Name" /><xsl:text>";
		public </xsl:text><xsl:value-of select="$DataType" /><xsl:text xml:space="preserve"> </xsl:text><xsl:value-of select="$CompressedDisplayName" /><xsl:text>
		{
			get { return (</xsl:text><xsl:value-of select="$DataType" /><xsl:text>) _activityItems[</xsl:text><xsl:value-of select="$CompressedDisplayName" /><xsl:text>FieldName]; }
			set { if (value != null) _activityItems[</xsl:text><xsl:value-of select="$CompressedDisplayName" /><xsl:text>FieldName] = value; }
		}
</xsl:text>
  </xsl:template>

  <msxsl:script language="C#" implements-prefix="script">
    <![CDATA[
    public string ResolveCLRType(string bamDataType)
    {
      switch (bamDataType)
      {
        case "NVARCHAR":
          return "string";
        case "DATETIME":
          return "DateTime";
        case "FLOAT":
          return "decimal"; 
        case "INT":
          return "int";
        default:
          throw new System.ArgumentException("bamDataType");
      }
    }
    ]]>
  </msxsl:script>

</xsl:stylesheet>
