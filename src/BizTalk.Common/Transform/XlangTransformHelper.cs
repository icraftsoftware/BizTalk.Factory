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

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using Be.Stateless.BizTalk.Runtime.Caching;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.Logging;
using Be.Stateless.Xml.Extensions;
using Microsoft.BizTalk.Streaming;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.XLANGs.Core;

namespace Be.Stateless.BizTalk.Transform
{
	/// <summary>
	/// Helper class that allows for an orchestration to easily use an <see cref="XslCompiledTransform"/>.
	/// </summary>
	public static class XlangTransformHelper
	{
		/// <summary>
		/// Applies an XSL transformation to the specified <paramref name="message"/>. The XSL transformation to be
		/// applied is specified in <paramref name="map"/>. The supplied <paramref name="trackingContext"/> is propagated
		/// to the resulting message.
		/// </summary>
		/// <remarks>
		/// This method assumes only the first part of a multipart <paramref name="message"/> message has to be
		/// transformed and creates an output message with a single part named "Main".
		/// </remarks>
		/// <param name="message">
		/// The <see cref="XLANGMessage"/> to be transformed.
		/// </param>
		/// <param name="map">
		/// The type of the BizTalk map class containing the transform to apply.
		/// </param>
		/// <param name="trackingContext">
		/// The <see cref="TrackingContext"/> to be copied in the context of the output message.
		/// </param>
		/// <returns>
		/// The transformed message with the result in the first part (at index 0).
		/// </returns>
		public static XLANGMessage Transform(XLANGMessage message, Type map, TrackingContext trackingContext)
		{
			if (message == null) throw new ArgumentNullException("message");
			if (map == null) throw new ArgumentNullException("map");
			return Transform(new XlangMessageCollection { message }, map, trackingContext);
		}

		/// <summary>
		/// Applies an XSL transformation to the specified <paramref name="messages"/>. The XSL transformation to be
		/// applied is specified in <paramref name="map"/>. The supplied <paramref name="trackingContext"/> is propagated
		/// to the resulting message.
		/// </summary>
		/// <remarks>
		/// This method assumes only the first part of any multipart <paramref name="messages"/> message has to be
		/// transformed and creates an output message with a single part named "Main".
		/// </remarks>
		/// <param name="messages">
		/// The <see cref="XlangMessageCollection"/> to be transformed.
		/// </param>
		/// <param name="map">
		/// The type of the BizTalk map class containing the transform to apply.
		/// </param>
		/// <param name="trackingContext">
		/// The <see cref="TrackingContext"/> to be copied in the context of the output message.
		/// </param>
		/// <returns>
		/// The transformed message with the result in the first part (at index 0).
		/// </returns>
		public static XLANGMessage Transform(XlangMessageCollection messages, Type map, TrackingContext trackingContext)
		{
			if (messages == null) throw new ArgumentNullException("messages");
			if (messages.Count == 0) throw new ArgumentException("XLangMessageCollection is empty.", "messages");
			if (map == null) throw new ArgumentNullException("map");
			using (messages)
			{
				var resultContent = Transform(messages, map, null);
				var resultMessage = XlangMessage.Create(Service.RootService.XlangStore.OwningContext, resultContent);
				trackingContext.Apply(resultMessage);
				return resultMessage;
			}
		}

		private static Stream Transform(XmlReader reader, Type map, object[] splatteredXsltArguments)
		{
			if (_logger.IsDebugEnabled) _logger.DebugFormat("About to execute transform '{0}'.", map.AssemblyQualifiedName);
			var transformDescriptor = XsltCache.Instance[map];
			using (reader)
			{
				var outputStream = new VirtualStream(DEFAULT_BUFFER_SIZE, DEFAULT_THRESHOLD_SIZE);
				var writerSettings = transformDescriptor.XslCompiledTransform.OutputSettings.Override(
					s => {
						s.CloseOutput = false;
						s.Encoding = Encoding.UTF8;
					});
				using (var writer = XmlWriter.Create(outputStream, writerSettings))
				{
					if (_logger.IsFineEnabled) _logger.FineFormat("Executing transform '{0}'.", map.AssemblyQualifiedName);
					var xsltArguments = transformDescriptor.Arguments.Union(splatteredXsltArguments);
					transformDescriptor.XslCompiledTransform.Transform(reader, xsltArguments, writer);
				}
				outputStream.Seek(0, SeekOrigin.Begin);
				return outputStream;
			}
		}

		private const int DEFAULT_BUFFER_SIZE = 10 * 1024; //10 KB
		private const int DEFAULT_THRESHOLD_SIZE = 1024 * 1024; //1 MB

		private static readonly ILog _logger = LogManager.GetLogger(typeof(XlangTransformHelper));
	}
}
