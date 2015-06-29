#region Copyright & License

// Copyright © 2012 - 2014 François Chabot, Yves Dierick
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
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.BizTalk.PipelineEditor.PipelineFile;
using PolicyFileDocument = Microsoft.BizTalk.PipelineEditor.PolicyFile.Document;

namespace Be.Stateless.BizTalk.Dsl.Pipeline
{
	public class PipelineDesignerDocumentSerializer : IDslSerializer
	{
		internal PipelineDesignerDocumentSerializer(IVisitable<IPipelineVisitor> pipeline)
		{
			_pipeline = pipeline;
		}

		#region IPipelineSerializer Members

		public string Serialize()
		{
			var pipelineDocument = GetDesignerDocument();
			using (var writer = new StringWriter())
			{
				var serializer = new XmlSerializer(typeof(Document));
				serializer.Serialize(writer, pipelineDocument);
				return writer.ToString();
			}
		}

		public void Save(string filePath)
		{
			using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				Write(file);
			}
		}

		public void Write(Stream stream)
		{
			var pipelineDocument = GetDesignerDocument();
			using (var xmlTextWriter = new XmlTextWriter(stream, Encoding.Unicode))
			{
				xmlTextWriter.Formatting = Formatting.Indented;
				var xmlSerializer = new XmlSerializer(typeof(Document));
				xmlSerializer.Serialize(xmlTextWriter, pipelineDocument);
			}
		}

		#endregion

		private Document GetDesignerDocument()
		{
			var visitor = new PipelineDesignerDocumentBuilderVisitor();
			_pipeline.Accept(visitor);
			return visitor.Document;
		}

		private readonly IVisitable<IPipelineVisitor> _pipeline;
	}
}
