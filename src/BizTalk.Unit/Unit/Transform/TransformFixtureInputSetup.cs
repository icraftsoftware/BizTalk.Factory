#region Copyright & License

// Copyright © 2012 - 2019 François Chabot
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
using System.IO;
using System.Xml.Schema;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.Xml.Xsl;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	internal class TransformFixtureInputSetup : IInputTransformFixtureSetup
	{
		internal TransformFixtureInputSetup(Type transformType, Dictionary<string, string> xsltNamespaces)
		{
			if (transformType == null) throw new ArgumentNullException("transformType");
			if (xsltNamespaces == null) throw new ArgumentNullException("xsltNamespaces");
			TransformType = transformType;
			XsltNamespaces = xsltNamespaces;
			Messages = new List<Stream>();
		}

		#region IInputTransformFixtureSetup Members

		public IOutputTransformFixtureSetup Transform
		{
			get { return new TransformFixtureOutputSetup(this); }
		}

		public IInputTransformFixtureSetup Arguments(XsltArgumentList arguments)
		{
			if (arguments == null) throw new ArgumentNullException("arguments");
			XsltArguments = arguments;
			return this;
		}

		public IInputTransformFixtureSetup Context(IBaseMessageContext context)
		{
			if (context == null) throw new ArgumentNullException("context");
			MessageContext = context;
			return this;
		}

		public IInputTransformFixtureSetup Message<T>(Stream message) where T : SchemaBase, new()
		{
			if (message == null) throw new ArgumentNullException("message");
			var reader = ValidatingXmlReader.Create<T>(message, XmlSchemaContentProcessing.Strict);
			var validatingStream = new XmlTranslatorStream(reader);
			Messages.Add(validatingStream);
			return this;
		}

		public IInputTransformFixtureSetup Message(Stream message)
		{
			if (message == null) throw new ArgumentNullException("message");
			Messages.Add(message);
			return this;
		}

		#endregion

		internal IBaseMessageContext MessageContext { get; private set; }

		internal List<Stream> Messages { get; private set; }

		internal Type TransformType { get; private set; }

		internal XsltArgumentList XsltArguments { get; private set; }

		internal Dictionary<string, string> XsltNamespaces { get; private set; }
	}
}
