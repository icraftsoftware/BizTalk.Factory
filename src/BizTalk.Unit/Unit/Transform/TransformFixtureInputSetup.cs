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
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Be.Stateless.BizTalk.Streaming.Extensions;
using Be.Stateless.BizTalk.Xml.Extensions;
using Be.Stateless.IO.Extensions;
using Be.Stateless.Xml.Xsl;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	internal class TransformFixtureInputSetup<TTransform> : ITransformFixtureInputSetup, ITransformFixtureSetup
		where TTransform : TransformBase, new()

	{
		internal TransformFixtureInputSetup(Action<ITransformFixtureInputSetup> inputSetupConfigurator)
		{
			if (inputSetupConfigurator == null) throw new ArgumentNullException("inputSetupConfigurator");
			Messages = new List<Stream>();

			inputSetupConfigurator(this);
			ValidateSetup();
		}

		#region ITransformFixtureInputSetup Members

		public ITransformFixtureInputSetup Arguments(XsltArgumentList arguments)
		{
			if (arguments == null) throw new ArgumentNullException("arguments");
			XsltArguments = arguments;
			return this;
		}

		public ITransformFixtureInputSetup Context(IBaseMessageContext context)
		{
			if (context == null) throw new ArgumentNullException("context");
			MessageContext = context;
			return this;
		}

		public ITransformFixtureInputSetup Message(Stream message)
		{
			if (message == null) throw new ArgumentNullException("message");
			Messages.Add(message);
			return this;
		}

		public ITransformFixtureInputSetup Message<TSchema>(Stream message)
			where TSchema : SchemaBase, new()
		{
			return Message<TSchema>(message, XmlSchemaContentProcessing.Strict);
		}

		public ITransformFixtureInputSetup Message<TSchema>(Stream message, XmlSchemaContentProcessing contentProcessing)
			where TSchema : SchemaBase, new()
		{
			if (message == null) throw new ArgumentNullException("message");
			var partCount = Messages.Count;
			var validatingXmlReader = XmlReader.Create(
				message,
				Be.Stateless.Xml.ValidatingXmlReaderSettings.Create(
					contentProcessing,
					(sender, args) => {
						throw new XmlSchemaValidationException(
							string.Format(
								"Transform's input message #{0} failed '{1}' schema validation for the following reason:{2}{3}: {4}{2}{2}The message's content is:{2}{5}{2}",
								partCount + 1,
								typeof(TSchema).Name,
								Environment.NewLine,
								args.Severity,
								args.Message,
								message.ReadToEnd()),
							args.Exception);
					},
					new TSchema().CreateResolvedSchema()));
			Messages.Add(validatingXmlReader.AsStream());
			return this;
		}

		#endregion

		#region ITransformFixtureSetup Members

		public ITransformFixtureOutputSelector Transform
		{
			get
			{
				var inputStream = Messages.ToArray().Transform();
				if (MessageContext != null) inputStream.ExtendWith(MessageContext);
				var outputStream = XsltArguments != null
					? inputStream.Apply(typeof(TTransform), XsltArguments)
					: inputStream.Apply(typeof(TTransform));
				return new TransformFixtureOutputSelector<TTransform>(outputStream);
			}
		}

		#endregion

		private IBaseMessageContext MessageContext { get; set; }

		private List<Stream> Messages { get; set; }

		private XsltArgumentList XsltArguments { get; set; }

		private void ValidateSetup()
		{
			if (!Messages.Any()) throw new InvalidOperationException("At least one input message must be setup.");
		}
	}
}
