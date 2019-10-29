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
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.MicroComponent.Extensions;
using Be.Stateless.BizTalk.Streaming.Extensions;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.BizTalk.Xml.Xsl.Extensions;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// Pipeline component that applies an XSL Transformation on messages along their way in the pipeline.
	/// </summary>
	/// <remarks>
	/// Contrary to maps statically-configured at the receive or send port level, <see cref="XsltRunner"/> allows for:
	/// <list type="bullet">
	/// <item>the transform to be dynamically chosen;</item>
	/// <item>the transform to be executed anywhere in the pipeline. This is particularly interesting, as we can choose
	/// to place it before or after an XML assembler/disassembler;</item>
	/// <item>the execution to be conditional;</item>
	/// <item>arguments to be dynamically supplied to the transform.</item>
	/// </list>
	/// </remarks>
	public class XsltRunner : IMicroPipelineComponent
	{
		public XsltRunner()
		{
			Encoding = new UTF8Encoding();
		}

		#region IMicroPipelineComponent Members

		public virtual IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			var map = message.ResolvePluginType(BizTalkFactoryProperties.MapTypeName, MapType)
				.OfPluginType<TransformBase>();
			if (map != null)
			{
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Applying '{0}' XSL Transform to message.", map.AssemblyQualifiedName);
				message.BodyPart.WrapOriginalDataStream(
					originalStream => originalStream
						.Transform()
						.ExtendWith(message.Context)
						.Apply(map, Encoding),
					pipelineContext.ResourceTracker);

				if (map.GetOutputSettings().OutputMethod == XmlOutputMethod.Xml)
				{
					if (_logger.IsDebugEnabled) _logger.DebugFormat("Probing output of '{0}' XSL Transform and promoting new message type.", map.AssemblyQualifiedName);
					message.ProbeAndPromoteMessageType(pipelineContext);
				}
				else if (_logger.IsDebugEnabled)
				{
					_logger.DebugFormat("Skip probing of '{0}' XSL Transform output for a message type as its OutputMethod is not XML.", map.AssemblyQualifiedName);
				}
			}
			else
			{
				if (_logger.IsDebugEnabled) _logger.Debug("No XSL Transform was found in message context or configured to apply to message.");
			}
			return message;
		}

		#endregion

		/// <summary>
		/// Encoding to use for output and, if Unicode, whether to emit a byte order mark.
		/// </summary>
		/// <remarks>
		/// It defaults to <see cref="UTF8Encoding"/> with a BOM preamble.
		/// </remarks>
		[XmlElement(typeof(EncodingXmlSerializer))]
		public Encoding Encoding { get; set; }

		/// <summary>
		/// The type name of the BizTalk Map to apply to the message.
		/// </summary>
		[XmlElement("Map", typeof(RuntimeTypeXmlSerializer))]
		public Type MapType { get; set; }

		private static readonly ILog _logger = LogManager.GetLogger(typeof(XsltRunner));
	}
}
