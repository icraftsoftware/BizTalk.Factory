#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Be.Stateless.Extensions;
using Be.Stateless.Linq.Extensions;
using Be.Stateless.Logging;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.Process
{
	public abstract class ClaimBasedProcessFixture : ProcessFixture
	{
		#region Nested Type: ClaimToken

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
		protected class ClaimToken
		{
			public string Any { get; set; }

			public string CorrelationToken { get; set; }

			public string MessageType { get; set; }

			public string OutboundTransportLocation { get; set; }

			public string ProcessActivityId { get; set; }

			public string ReceiverName { get; set; }

			public string SenderName { get; set; }

			public string Url { get; set; }

			internal string FilePath
			{
				get { return Path.Combine(BizTalkFactorySettings.ClaimStoreCheckInDirectory, Url.Replace("\\", string.Empty) + ".chk"); }
			}
		}

		#endregion

		#region Base Class Member Overrides

		protected internal override IEnumerable<string> SystemOutputFolders
		{
			get { return new[] { CheckInFolder, CheckOutFolder }; }
		}

		#endregion

		protected string CheckInFolder
		{
			get { return BizTalkFactorySettings.ClaimStoreCheckInDirectory; }
		}

		protected string CheckOutFolder
		{
			get { return BizTalkFactorySettings.ClaimStoreCheckOutDirectory; }
		}

		private SqlConnection Connection
		{
			get { return new SqlConnection(ConfigurationManager.ConnectionStrings["TransientStateDb"].ConnectionString); }
		}

		protected void ClearTokens()
		{
			_logger.Debug("Clearing claim tokens.");
			using (var cnx = Connection)
			using (var cmd = new SqlCommand("SELECT Url, MessageType, ProcessActivityId FROM claim_Tokens", cnx))
			{
				cnx.Open();
				cmd.ExecuteReader().Cast<IDataRecord>()
					.Each(
						(idx, r) => _logger.DebugFormat(
							"[{0,2}] Clearing claim token\r\n     Url: {1}\r\n     MessageType: {1}\r\n     ProcessActivityId: {1}",
							idx,
							r["Url"].ToString(),
							r["MessageType"].ToString(),
							r["ProcessActivityId"].ToString()));
			}
			using (var cnx = Connection)
			using (var cmd = new SqlCommand("DELETE FROM claim_Tokens", cnx))
			{
				cnx.Open();
				cmd.ExecuteNonQuery();
			}
		}

		protected ClaimToken[] FindTokens()
		{
			using (var cnx = Connection)
			using (var cmd = new SqlCommand("SELECT * FROM claim_Tokens", cnx))
			{
				cnx.Open();
				return cmd.ExecuteReader().Cast<IDataRecord>()
					.Select(
						r => new ClaimToken {
							Url = r["Url"].ToString(),
							CorrelationToken = r["CorrelationToken"].ToString(),
							MessageType = r["MessageType"].ToString(),
							OutboundTransportLocation = r["OutboundTransportLocation"].ToString(),
							ProcessActivityId = r["ProcessActivityId"].ToString(),
							ReceiverName = r["ReceiverName"].ToString(),
							SenderName = r["SenderName"].ToString(),
							Any = r["Any"].ToString()
						})
					.ToArray();
			}
		}

		internal void InsertToken(string tokenUrl, string correlationToken, string messageType = null, string receiverName = null, string senderName = null, string any = null)
		{
			const string cmdText = "INSERT claim_Tokens (Url, CorrelationToken, MessageType, ReceiverName, SenderName, [Any])"
				+ " VALUES(@url, @correlationToken, @messageType, @receiverName, @senderName, @any)";
			using (var cnx = Connection)
			using (var cmd = new SqlCommand(cmdText, cnx))
			{
				cmd.Parameters.AddWithValue("@url", tokenUrl);
				cmd.Parameters.AddWithValue("@correlationToken", correlationToken.IsNullOrEmpty() ? (object) DBNull.Value : correlationToken);
				cmd.Parameters.AddWithValue("@messageType", messageType.IsNullOrEmpty() ? (object) DBNull.Value : messageType);
				cmd.Parameters.AddWithValue("@receiverName", receiverName.IsNullOrEmpty() ? (object) DBNull.Value : receiverName);
				cmd.Parameters.AddWithValue("@senderName", senderName.IsNullOrEmpty() ? (object) DBNull.Value : senderName);
				// ReSharper disable once PossibleNullReferenceException
				cmd.Parameters.AddWithValue("@any", any.IsNullOrEmpty() ? (object) DBNull.Value : any.Trim());
				cnx.Open();
				cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Moves the claimed file described by the the given <see cref="ClaimToken"/>, and releases the token, as a real
		/// claim store agent would do. 
		/// </summary>
		/// <param name="token"></param>
		protected void ReleaseToken(ClaimToken token)
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(CheckOutFolder, token.Url)));
			File.Move(Path.Combine(CheckInFolder, token.FilePath), Path.Combine(CheckOutFolder, token.Url));
			ReleaseTokensFromDatabase(new[] { token.Url });
		}

		internal void ReleaseTokenFromDatabase(string tokenUrl)
		{
			ReleaseTokensFromDatabase(new[] { tokenUrl });
		}

		internal void ReleaseTokensFromDatabase(string[] tokenUrls)
		{
			using (var cnx = Connection)
			using (var cmd = new SqlCommand("usp_claim_Release", cnx))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@url", string.Empty);
				cnx.Open();
				tokenUrls.Each(
					tu => {
						// ReSharper disable AccessToDisposedClosure
						cmd.Parameters["@url"].Value = tu;
						cmd.ExecuteNonQuery();
						// ReSharper restore AccessToDisposedClosure
					});
			}
		}

		[SetUp]
		public void BizTalkFactoryClaimBasedProcessFixtureSetup()
		{
			ClearTokens();
		}

		[TearDown]
		public void BizTalkFactoryClaimBasedProcessFixtureTearDown()
		{
			ClearTokens();
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(ClaimBasedProcessFixture));
	}
}
