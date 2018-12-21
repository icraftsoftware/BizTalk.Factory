#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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

using Be.Stateless.BizTalk.Schema;
using Microsoft.BizTalk.Component.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Component.Extensions
{
	[TestFixture]
	public class PipelineContextExtensionsFixture
	{
		[Test]
		public void GetSchemaMetadataByType()
		{
			// this is an actual odd edge case that has been occurring on a production BTS server but not reproducible
			// indeed pipelineContext.GetDocumentSpecByType("string") should throw instead of returning this DocSpecStrongName
			var documentSpecMock = new Mock<IDocumentSpec>();
			documentSpecMock
				.Setup(m => m.DocSpecStrongName)
				.Returns(typeof(string).AssemblyQualifiedName);

			var pipelineContextMock = new Mock<IPipelineContext> { DefaultValue = DefaultValue.Mock };
			pipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType("string"))
				.Returns(documentSpecMock.Object);

			var metadata = pipelineContextMock.Object.GetSchemaMetadataByType("string", false);

			Assert.That(metadata, Is.SameAs(SchemaMetadata.Unknown));
		}
	}
}
