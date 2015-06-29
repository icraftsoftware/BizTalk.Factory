#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Be.Stateless.BizTalk.Message;
using Be.Stateless.Extensions;
using Be.Stateless.IO;
using Be.Stateless.Xml.Extensions;

namespace Be.Stateless.BizTalk.Processes.Batch
{
	internal static class BatchAdapter
	{
		#region Nested type: Envelope

		internal class Envelope
		{
			// ReSharper disable UnusedAutoPropertyAccessor.Global
			public int Id { get; set; }
			public string SpecName { get; set; }
			// ReSharper restore UnusedAutoPropertyAccessor.Global
		}

		#endregion

		#region Nested type: Part

		internal class Part
		{
			// ReSharper disable UnusedAutoPropertyAccessor.Global
			public int EnvelopeId { get; set; }
			public string MessagingStepActivityId { get; set; }
			public string Partition { get; set; }
			// ReSharper restore UnusedAutoPropertyAccessor.Global
		}

		#endregion

		#region Nested type: QueuedControlledRelease

		internal class QueuedControlledRelease
		{
			// ReSharper disable UnusedAutoPropertyAccessor.Global
			public int EnvelopeId { get; set; }
			public string Partition { get; set; }
			public DateTime Timestamp { get; set; }
			// ReSharper restore UnusedAutoPropertyAccessor.Global
		}

		#endregion

		#region Nested type: ReleasePolicy

		internal class ReleasePolicy
		{
			// ReSharper disable UnusedAutoPropertyAccessor.Global
			public int EnvelopeId { get; set; }
			public string Partition { get; set; }
			public bool Enabled { get; set; }
			public int ReleaseOnItemCount { get; set; }
			// ReSharper restore UnusedAutoPropertyAccessor.Global
		}

		#endregion

		internal static Stream CreatePartMessage(string envelopeSpecName, string partition)
		{
			var builder = new StringBuilder();
			builder.Append("<ns0:Any xmlns:ns0='urn:schemas.stateless.be:biztalk:any:2012:12'>");
			builder.AppendFormat("<EnvelopeSpecName>{0}</EnvelopeSpecName>", envelopeSpecName);
			if (!partition.IsNullOrEmpty()) builder.AppendFormat("<EnvelopePartition>{0}</EnvelopePartition>", partition);
			builder.AppendFormat(
				@"<extra-content><record><field1>{0}</field1><field2>{1}</field2><field3>{2}</field3></record></extra-content>",
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid());
			builder.Append("</ns0:Any>");
			return new StringStream(builder.ToString());
		}

		internal static Stream CreateReleaseMessage(string envelopeSpecName, string partition)
		{
			const string t1 = "<ns:ReleaseBatch xmlns:ns=\"urn:schemas.stateless.be:biztalk:batch:2012:12\">" +
				"<ns:EnvelopeSpecName>{0}</ns:EnvelopeSpecName></ns:ReleaseBatch>";
			const string t2 = "<ns:ReleaseBatch xmlns:ns=\"urn:schemas.stateless.be:biztalk:batch:2012:12\">" +
				"<ns:EnvelopeSpecName>{0}</ns:EnvelopeSpecName><ns:Partition>{1}</ns:Partition></ns:ReleaseBatch>";
			var message = partition.IsNullOrEmpty()
				? MessageFactory.CreateMessage<Schemas.Xml.Batch.Release>(string.Format(t1, envelopeSpecName))
				: MessageFactory.CreateMessage<Schemas.Xml.Batch.Release>(string.Format(t2, envelopeSpecName, partition));
			return message.AsStream();
		}

		private static SqlConnection Connection
		{
			get { return new SqlConnection(ConfigurationManager.ConnectionStrings["TransientStateDb"].ConnectionString); }
		}

		internal static IEnumerable<Envelope> Envelopes
		{
			get
			{
				using (var cnx = Connection)
				using (var cmd = new SqlCommand("SELECT Id, EnvelopeSpecName FROM batch_Envelopes", cnx))
				{
					cnx.Open();
					return cmd.ExecuteReader().Cast<IDataRecord>()
						.Select(
							r => new Envelope {
								Id = (int) r["Id"],
								SpecName = (string) r["EnvelopeSpecName"]
							})
						.ToArray();
				}
			}
		}

		internal static IEnumerable<Part> Parts
		{
			get
			{
				using (var cnx = Connection)
				using (var cmd = new SqlCommand("SELECT EnvelopeId, MessagingStepActivityId, Partition FROM batch_Parts", cnx))
				{
					cnx.Open();
					return cmd.ExecuteReader().Cast<IDataRecord>()
						.Select(
							r => new Part {
								EnvelopeId = (int) r["EnvelopeId"],
								MessagingStepActivityId = (string) r["MessagingStepActivityId"],
								Partition = (string) r["Partition"]
							})
						.ToArray();
				}
			}
		}

		internal static IEnumerable<QueuedControlledRelease> QueuedControlledReleases
		{
			get
			{
				using (var cnx = Connection)
				using (var cmd = new SqlCommand("SELECT EnvelopeId, Partition, Timestamp FROM batch_QueuedControlledReleases", cnx))
				{
					cnx.Open();
					return cmd.ExecuteReader().Cast<IDataRecord>()
						.Select(
							r => new QueuedControlledRelease {
								EnvelopeId = (int) r["EnvelopeId"],
								Partition = (string) r["Partition"],
								Timestamp = (DateTime) r["Timestamp"]
							})
						.ToArray();
				}
			}
		}

		internal static IEnumerable<ReleasePolicy> ReleasePolicies
		{
			get
			{
				using (var cnx = Connection)
				using (var cmd = new SqlCommand("SELECT EnvelopeId, Partition, Enabled, ReleaseOnItemCount FROM vw_batch_ReleasePolicies", cnx))
				{
					cnx.Open();
					return cmd.ExecuteReader().Cast<IDataRecord>()
						.Select(
							r => new ReleasePolicy {
								EnvelopeId = (int) r["EnvelopeId"],
								Partition = (string) r["Partition"],
								Enabled = (bool) r["Enabled"],
								ReleaseOnItemCount = (int) r["ReleaseOnItemCount"]
							})
						.ToArray();
				}
			}
		}

		internal static void AddPart(string envelopeSpecName, string partition, string messagingStepActivityId, string data)
		{
			using (var cnx = Connection)
			using (var cmd = new SqlCommand("usp_batch_AddPart", cnx))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@envelopeSpecName", envelopeSpecName);
				cmd.Parameters.AddWithValue("@partition", partition);
				cmd.Parameters.AddWithValue("@messagingStepActivityId", messagingStepActivityId);
				cmd.Parameters.AddWithValue("@data", data);
				cnx.Open();
				cmd.ExecuteNonQuery();
			}
		}

		internal static void ClearParts()
		{
			using (var cnx = Connection)
			using (var cmd = new SqlCommand("DELETE FROM batch_Parts", cnx))
			{
				cnx.Open();
				cmd.ExecuteNonQuery();
			}
		}

		internal static void ClearQueuedReleases()
		{
			using (var cnx = Connection)
			using (var cmd = new SqlCommand("DELETE FROM batch_QueuedControlledReleases", cnx))
			{
				cnx.Open();
				cmd.ExecuteNonQuery();
			}
		}

		internal static void RegisterBatch(string envelopeSpecName, string partition, bool enabled, int releaseOnItemCount)
		{
			RegisterBatch(envelopeSpecName, partition, enabled, releaseOnItemCount, false);
		}

		internal static void RegisterBatch(string envelopeSpecName, string partition, bool enabled, int releaseOnItemCount, bool enforceItemCountLimitOnRelease)
		{
			using (var cnx = Connection)
			using (var cmd = new SqlCommand("usp_batch_Register", cnx))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@envelopeSpecName", envelopeSpecName);
				cmd.Parameters.AddWithValue("@partition", partition);
				cmd.Parameters.AddWithValue("@enabled", enabled);
				cmd.Parameters.AddWithValue("@releaseOnItemCount", releaseOnItemCount);
				cmd.Parameters.AddWithValue("@enforceItemCountLimitOnRelease", enforceItemCountLimitOnRelease);
				cnx.Open();
				cmd.ExecuteNonQuery();
			}
		}

		internal static void UnregisterBatch(string envelopeSpecName, string partition)
		{
			using (var cnx = Connection)
			using (var cmd = new SqlCommand("usp_batch_Unregister", cnx))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@envelopeSpecName", envelopeSpecName);
				cmd.Parameters.AddWithValue("@partition", partition);
				cnx.Open();
				cmd.ExecuteNonQuery();
			}
		}
	}
}
