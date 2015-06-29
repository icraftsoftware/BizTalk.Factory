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

using System.IO;
using Be.Stateless.BizTalk.Monitoring.Configuration;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.Extensions;
using Be.Stateless.IO;
using Be.Stateless.IO.Extensions;
using Path = System.IO.Path;

namespace Be.Stateless.BizTalk.Monitoring.Model
{
	public partial class Message
	{
		public string Body
		{
			get
			{
				using (var stream = new StreamReader(Stream))
				{
					if (HasBeenClaimed)
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
				return HasBeenClaimed
					? !MonitoringConfigurationSection.Current.ClaimStoreDirectory.IsNullOrEmpty()
					: !EncodedBody.IsNullOrEmpty();
			}
		}

		public bool HasBeenClaimed
		{
			get { return EncodedBodyType.IfNotNull(value => value == MessageBodyCaptureMode.Claimed.ToString()); }
		}

		public bool ClaimAvailabe
		{
			get { return HasBeenClaimed && File.Exists(Path.Combine(MonitoringConfigurationSection.Current.ClaimStoreDirectory, EncodedBody)); }
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

		public Stream Stream
		{
			get
			{
				return !HasContent
					? new MemoryStream()
					: HasBeenClaimed
						? ClaimedStream
						: EncodedBody.DecompressFromBase64String();
			}
		}

		private Stream ClaimedStream
		{
			get
			{
				return ClaimAvailabe
					? (Stream) File.OpenRead(Path.Combine(MonitoringConfigurationSection.Current.ClaimStoreDirectory, EncodedBody))
					: new StringStream(string.Format("The captured payload entry '{0}' is not yet available in the central store.", EncodedBody));
			}
		}

		// buffer used to read the first preview characters of large message bodies
		private static readonly char[] _buffer = new char[1024];
	}
}
