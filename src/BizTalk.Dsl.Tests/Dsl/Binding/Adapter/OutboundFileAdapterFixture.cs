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

using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class OutboundFileAdapterFixture
	{
		[Test]
		public void CredentialsAreCompatibleWithNetworkFolder()
		{
			var oft = new OutboundFileAdapter(
				t => {
					t.DestinationFolder = @"\\server\folder";
					t.NetworkCredentials.Username = "user";
					t.NetworkCredentials.Password = "pwd";
				});
			Assert.That(() => ((ISupportValidation) oft).Validate(), Throws.Nothing);
		}

		[Test]
		public void CredentialsAreNotCompatibleWithLocalFolder()
		{
			var oft = new OutboundFileAdapter(
				t => {
					t.DestinationFolder = @"c:\files\drops";
					t.NetworkCredentials.Username = "user";
					t.NetworkCredentials.Password = "pwd";
				});
			Assert.That(
				() => ((ISupportValidation) oft).Validate(),
				Throws.TypeOf<BindingException>()
					.With.Message.EqualTo("Alternate credentials to access the file folder cannot be supplied while accessing local drive or a mapped network drive."));
		}

		[Test]
		public void DestinationFolderIsRequired()
		{
			var oft = new OutboundFileAdapter(t => { });
			Assert.That(
				() => ((ISupportValidation) oft).Validate(),
				Throws.TypeOf<BindingException>().With.Message.EqualTo("Outbond file adapter has no destination folder."));
		}

		[Test]
		public void FileNameIsRequired()
		{
			var oft = new OutboundFileAdapter(
				t => {
					t.DestinationFolder = @"\\server";
					t.FileName = string.Empty;
				});
			Assert.That(
				() => ((ISupportValidation) oft).Validate(),
				Throws.TypeOf<BindingException>().With.Message.EqualTo("Outbond file adapter has no destination file name."));
		}

		[Test]
		public void SerializeToXml()
		{
			var oft = new OutboundFileAdapter(t => { t.DestinationFolder = @"c:\files\drops"; });
			var xml = ((IAdapterBindingSerializerFactory) oft).GetAdapterBindingSerializer().Serialize();
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
			var oft = new OutboundFileAdapter(
				t => {
					t.DestinationFolder = @"\\server";
					t.CopyMode = CopyMode.Append;
					t.UseTempFileOnWrite = false;
				});
			Assert.That(() => ((ISupportValidation) oft).Validate(), Throws.Nothing);
		}

		[Test]
		public void UseTempFileOnWriteAndCreateNewFileAreCompatible()
		{
			var oft = new OutboundFileAdapter(
				t => {
					t.DestinationFolder = @"\\server";
					t.CopyMode = CopyMode.CreateNew;
					t.UseTempFileOnWrite = true;
				});
			Assert.That(() => ((ISupportValidation) oft).Validate(), Throws.Nothing);
		}

		[Test]
		public void UseTempFileOnWriteAndOverwriteFileAreNotCompatible()
		{
			var oft = new OutboundFileAdapter(
				t => {
					t.DestinationFolder = @"\\server";
					t.CopyMode = CopyMode.Overwrite;
					t.UseTempFileOnWrite = true;
				});
			Assert.That(
				() => ((ISupportValidation) oft).Validate(),
				Throws.TypeOf<BindingException>()
					.With.Message.EqualTo("Outbond file adapter cannot use a temporary file when it is meant to append or overwrite an existing file."));
		}
	}
}
