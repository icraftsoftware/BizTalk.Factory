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
	public class FileAdapterOutboundFixture
	{
		[Test]
		public void CredentialsAreCompatibleWithNetworkFolder()
		{
			var ofa = new FileAdapter.Outbound(
				a => {
					a.DestinationFolder = @"\\server\folder";
					a.NetworkCredentials.UserName = "user";
					a.NetworkCredentials.Password = "pwd";
				});
			Assert.That(() => ((ISupportValidation) ofa).Validate(), Throws.Nothing);
		}

		[Test]
		public void CredentialsAreNotCompatibleWithLocalFolder()
		{
			var ofa = new FileAdapter.Outbound(
				a => {
					a.DestinationFolder = @"c:\files\drops";
					a.NetworkCredentials.UserName = "user";
					a.NetworkCredentials.Password = "pwd";
				});
			Assert.That(
				() => ((ISupportValidation) ofa).Validate(),
				Throws.TypeOf<BindingException>()
					.With.Message.EqualTo("Alternate credentials to access the file folder cannot be supplied while accessing local drive or a mapped network drive."));
		}

		[Test]
		public void DestinationFolderIsRequired()
		{
			var ofa = new FileAdapter.Outbound(a => { });
			Assert.That(
				() => ((ISupportValidation) ofa).Validate(),
				Throws.TypeOf<BindingException>().With.Message.EqualTo("Outbond file adapter has no destination folder."));
		}

		[Test]
		public void FileNameIsRequired()
		{
			var ofa = new FileAdapter.Outbound(
				a => {
					a.DestinationFolder = @"\\server";
					a.FileName = string.Empty;
				});
			Assert.That(
				() => ((ISupportValidation) ofa).Validate(),
				Throws.TypeOf<BindingException>().With.Message.EqualTo("Outbond file adapter has no destination file name."));
		}

		[Test]
		public void SerializeToXml()
		{
			var ofa = new FileAdapter.Outbound(a => { a.DestinationFolder = @"c:\files\drops"; });
			var xml = ((IAdapterBindingSerializerFactory) ofa).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<AllowCacheOnWrite vt=\"11\">0</AllowCacheOnWrite>" +
						"<CopyMode vt=\"19\">1</CopyMode>" +
						"<FileName vt=\"8\">%MessageID%.xml</FileName>" +
						"<UseTempFileOnWrite vt=\"11\">-1</UseTempFileOnWrite>" +
						"</CustomProps>"));
		}

		[Test]
		public void UseTempFileOnWriteAndAppendFileAreNotCompatible()
		{
			var ofa = new FileAdapter.Outbound(
				a => {
					a.DestinationFolder = @"\\server";
					a.Mode = FileAdapter.CopyMode.Append;
					a.UseTempFileOnWrite = false;
				});
			Assert.That(() => ((ISupportValidation) ofa).Validate(), Throws.Nothing);
		}

		[Test]
		public void UseTempFileOnWriteAndCreateNewFileAreCompatible()
		{
			var ofa = new FileAdapter.Outbound(
				a => {
					a.DestinationFolder = @"\\server";
					a.Mode = FileAdapter.CopyMode.CreateNew;
					a.UseTempFileOnWrite = true;
				});
			Assert.That(() => ((ISupportValidation) ofa).Validate(), Throws.Nothing);
		}

		[Test]
		public void UseTempFileOnWriteAndOverwriteFileAreNotCompatible()
		{
			var ofa = new FileAdapter.Outbound(
				a => {
					a.DestinationFolder = @"\\server";
					a.Mode = FileAdapter.CopyMode.Overwrite;
					a.UseTempFileOnWrite = true;
				});
			Assert.That(
				() => ((ISupportValidation) ofa).Validate(),
				Throws.TypeOf<BindingException>()
					.With.Message.EqualTo("Outbond file adapter cannot use a temporary file when it is meant to append or overwrite an existing file."));
		}
	}
}
