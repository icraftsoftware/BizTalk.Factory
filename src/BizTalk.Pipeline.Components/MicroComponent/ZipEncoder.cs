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

using System;
using System.IO;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.Extensions;
using log4net;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// Wraps the message's original stream in an zip-compressing stream.
	/// </summary>
	/// <remarks>
	/// Zip-compress outbound message's body part stream.
	/// </remarks>
	/// <seealso cref="ZipOutputStream"/>
	public class ZipEncoder : IMicroPipelineComponent
	{
		#region IMicroPipelineComponent Members

		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			var location = message.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation);
			if (location.IsNullOrEmpty()) throw new InvalidOperationException("BizTalkFactoryProperties.OutboundTransportLocation has to be set in context in order to determine zip entry name.");

			// TODO instead of SharpZipLib's ZipOutputStream use BCL's System.IO.Compression.ZipArchive or System.IO.Compression.GZipStream or System.IO.Compression.DeflateStream as only one zip entry is supported
			message.BodyPart.WrapOriginalDataStream(
				originalStream => {
					if (_logger.IsDebugEnabled) _logger.Debug("Wrapping message stream in a zip-compressing stream.");
					var zipEntryName = Path.GetFileName(location);
					return new ZipOutputStream(originalStream, zipEntryName);
				},
				pipelineContext.ResourceTracker);

			// ReSharper disable once AssignNullToNotNullAttribute
			var zipLocation = Path.Combine(Path.GetDirectoryName(location), Path.GetFileNameWithoutExtension(location) + ".zip");
			message.SetProperty(BizTalkFactoryProperties.OutboundTransportLocation, zipLocation);

			return message;
		}

		#endregion

		private static readonly ILog _logger = LogManager.GetLogger(typeof(ZipEncoder));
	}
}
