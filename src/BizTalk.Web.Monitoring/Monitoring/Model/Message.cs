#region Copyright & License

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
using System.IO;
using System.Linq;
using Be.Stateless.BizTalk.Web.Monitoring.Configuration;
using Be.Stateless.Extensions;
using Be.Stateless.IO.Extensions;

namespace Be.Stateless.BizTalk.Web.Monitoring.Model
{
	public partial class Message
	{
		public string Body
		{
			get
			{
				using (var stream = new StreamReader(Stream))
				{
					if (HasLargeBody)
					{
						var length = stream.Read(_buffer, 0, _buffer.Length);
						return new string(_buffer, 0, length);
					}
					return stream.ReadToEnd();
				}
			}
		}

		public bool HasContent
		{
			get
			{
				return HasLargeBody
					? !MonitoringConfigurationSection.Current.LargeMessageTrackingDirectory.IsNullOrEmpty()
					: !EncodedBody.IsNullOrEmpty();
			}
		}

		public bool HasLargeBody
		{
			get { return EncodedBodyType.IfNotNull(value => value.Equals("BigBody", StringComparison.OrdinalIgnoreCase)); }
		}

		public string MimeType
		{
			get
			{
				if (HasContent)
				{
					using (var stream = Stream)
					{
						return stream.GetMimeType();
					}
				}
				return null;
			}
		}

		public string ReceivedFileName
		{
			get
			{
				var receivedFileName = MessagingStep.Context.Properties
					.SingleOrDefault(p => p.Name.Equals("ReceivedFileName") && p.Namespace.Equals(BIZTALK_FILE_PROPERTIES_NAMESPACE));
				return receivedFileName.IfNotNull(f => Path.GetFileName(f.Value))
					?? (MimeType.StartsWith("application/", StringComparison.OrdinalIgnoreCase)
						? "replay_message.bin"
						: MimeType.Equals("text/html", StringComparison.OrdinalIgnoreCase)
							? "replay_message.html"
							: MimeType.StartsWith("text/", StringComparison.OrdinalIgnoreCase)
								? (Body[0] == '<' ? "replay_message.xml" : "replay_message.txt")
								: "replay_message.unknown");
			}
		}

		public Stream Stream
		{
			get
			{
				return !HasContent
					? new MemoryStream()
					: HasLargeBody
						? File.OpenRead(Path.Combine(MonitoringConfigurationSection.Current.LargeMessageTrackingDirectory, EncodedBody))
						: EncodedBody.DecompressFromBase64String();
			}
		}

		private const string BIZTALK_FILE_PROPERTIES_NAMESPACE = "http://schemas.microsoft.com/BizTalk/2003/file-properties";
		// buffer used to read the first preview characters of large message bodies
		private static readonly char[] _buffer = new char[1024];
	}
}