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
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.Extensions;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// Wraps the message's original stream in an zip-compressing stream.
	/// </summary>
	/// <remarks>
	/// Zip-compress outbound message's body part stream.
	/// </remarks>
	/// <seealso cref="ZipOutputStream"/>
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[Guid(CLASS_ID)]
	[ComponentCategory(CategoryTypes.CATID_Encoder)]
	public class ZipEncoderComponent : PipelineComponent
	{
		#region Base Class Member Overrides

		[Browsable(false)]
		public override string Description
		{
			get { return "Wraps the message's original stream in an zip-compressing stream."; }
		}

		protected internal override IBaseMessage ExecuteCore(IPipelineContext pipelineContext, IBaseMessage message)
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

		public override void GetClassID(out Guid classId)
		{
			classId = new Guid(CLASS_ID);
		}

		protected override void Load(IPropertyBag propertyBag) { }

		protected override void Save(IPropertyBag propertyBag) { }

		#endregion

		private const string CLASS_ID = "0895f316-1e7b-46c4-ba19-7d357d5ac116";
		private static readonly ILog _logger = LogManager.GetLogger(typeof(ZipEncoderComponent));
	}
}
