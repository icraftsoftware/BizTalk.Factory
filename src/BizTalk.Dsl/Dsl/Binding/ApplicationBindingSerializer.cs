#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.Dsl.Binding.Visitor;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public class ApplicationBindingSerializer : IDslSerializer
	{
		public ApplicationBindingSerializer(IVisitable<IApplicationBindingVisitor> applicationBinding, string environment)
		{
			if (applicationBinding == null) throw new ArgumentNullException("applicationBinding");
			if (environment.IsNullOrEmpty()) throw new ArgumentNullException("environment");
			_applicationBinding = applicationBinding;
			_environment = environment;
		}

		#region IDslSerializer Members

		public string Serialize()
		{
			var bindingInfo = GetBindingInfo();
			using (var writer = new StringWriter())
			{
				var serializer = new XmlSerializer(typeof(BindingInfo));
				serializer.Serialize(writer, bindingInfo);
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
			var bindingInfo = GetBindingInfo();
			using (var xmlTextWriter = new XmlTextWriter(stream, Encoding.UTF8))
			{
				xmlTextWriter.Formatting = Formatting.Indented;
				var xmlSerializer = new XmlSerializer(typeof(BindingInfo));
				xmlSerializer.Serialize(xmlTextWriter, bindingInfo);
			}
		}

		#endregion

		private BindingInfo GetBindingInfo()
		{
			var visitor = BindingInfoBuilderVisitor.Create(_environment);
			_applicationBinding.Accept(visitor);
			return visitor.BindingInfo;
		}

		private readonly IVisitable<IApplicationBindingVisitor> _applicationBinding;
		private readonly string _environment;
	}
}
