#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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

using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class FileAdapterInboundFixture
	{
		[Test]
		public void CredentialsAreCompatibleWithNetworkFolder()
		{
			var ifa = new FileAdapter.Inbound(
				a => {
					a.ReceiveFolder = @"\\server\folder";
					a.NetworkCredentials.UserName = "user";
					a.NetworkCredentials.Password = "pwd";
				});
			Assert.That(() => ((ISupportValidation) ifa).Validate(), Throws.Nothing);
		}

		[Test]
		public void CredentialsAreNotCompatibleWithLocalFolder()
		{
			var ifa = new FileAdapter.Inbound(
				a => {
					a.ReceiveFolder = @"c:\files\drops";
					a.NetworkCredentials.UserName = "user";
					a.NetworkCredentials.Password = "pwd";
				});
			Assert.That(
				() => ((ISupportValidation) ifa).Validate(),
				Throws.TypeOf<BindingException>()
					.With.Message.EqualTo("Alternate credentials to access the file folder cannot be supplied while accessing local drive or a mapped network drive."));
		}

		[Test]
		public void FileNameIsRequired()
		{
			var ifa = new FileAdapter.Inbound(
				a => {
					a.ReceiveFolder = @"\\server";
					a.FileMask = string.Empty;
				});
			Assert.That(
				() => ((ISupportValidation) ifa).Validate(),
				Throws.TypeOf<BindingException>().With.Message.EqualTo("Inbound file adapter has no source file mask."));
		}

		[Test]
		public void ReceiveFolderIsRequired()
		{
			var ifa = new FileAdapter.Inbound(a => { });
			Assert.That(
				() => ((ISupportValidation) ifa).Validate(),
				Throws.TypeOf<BindingException>().With.Message.EqualTo("Inbound file adapter has no source folder."));
		}

		[Test]
		public void SerializeToXml()
		{
			var ifa = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\files\drops"; });
			var xml = ((IAdapterBindingSerializerFactory) ifa).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<BatchSize vt=\"19\">20</BatchSize>" +
						"<BatchSizeInBytes vt=\"19\">102400</BatchSizeInBytes>" +
						"<FileMask vt=\"8\">*.xml</FileMask>" +
						"<FileNetFailRetryCount vt=\"19\">5</FileNetFailRetryCount>" +
						"<FileNetFailRetryInt vt=\"19\">5</FileNetFailRetryInt>" +
						"<PollingInterval vt=\"19\">60000</PollingInterval>" +
						"<RemoveReceivedFileDelay vt=\"19\">10</RemoveReceivedFileDelay>" +
						"<RemoveReceivedFileMaxInterval vt=\"19\">300000</RemoveReceivedFileMaxInterval>" +
						"<RemoveReceivedFileRetryCount vt=\"19\">5</RemoveReceivedFileRetryCount>" +
						"<RenameReceivedFiles vt=\"11\">-1</RenameReceivedFiles>" +
						"</CustomProps>"));
		}
	}
}
