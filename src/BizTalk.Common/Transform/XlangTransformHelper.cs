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
		/// Applies an XSL transformation to the specified <paramref name="sourceMessage"/>. The XSL transformation to be
		/// applied is specified in <paramref name="mapType"/>. The supplied <paramref name="trackingContext"/> is
		/// propagated to the resulting message.
		/// </summary>
		/// <remarks>
		/// This method assumes only the first part of a multipart <paramref name="sourceMessage"/> message has to be
		/// transformed and creates an output message with a single part named "Main".
		/// </remarks>
		/// <param name="sourceMessage">
		/// The <see cref="XLANGMessage"/> to be transformed.
		/// </param>
		/// <param name="mapType">
		/// The type of the BizTalk map class containing the transform to apply.
		/// </param>
		/// <param name="trackingContext">
		/// The <see cref="TrackingContext"/> to be copied in the context of the output message.
		/// </param>
		/// <returns>
		/// The transformed message with the result in the first part (at index 0).
		/// </returns>
		public static XLANGMessage Transform(XLANGMessage sourceMessage, Type mapType, TrackingContext trackingContext)
		{
			if (sourceMessage == null) throw new ArgumentNullException("sourceMessage");
			if (mapType == null) throw new ArgumentNullException("mapType");
			return Transform(new XlangMessageCollection { sourceMessage }, mapType, trackingContext);
		}

		/// <summary>
		/// Applies an XSL transformation to the specified <paramref name="sourceMessages"/>. The XSL transformation to be
		/// applied is specified in <paramref name="mapType"/>. The supplied <paramref name="trackingContext"/> is
		/// propagated to the resulting message.
		/// </summary>
		/// <remarks>
		/// This method assumes only the first part of any multipart <paramref name="sourceMessages"/> message has to be
		/// transformed and creates an output message with a single part named "Main".
		/// </remarks>
		/// <param name="sourceMessages">
		/// The <see cref="XlangMessageCollection"/> to be transformed.
		/// </param>
		/// <param name="mapType">
		/// The type of the BizTalk map class containing the transform to apply.
		/// </param>
		/// <param name="trackingContext">
		/// The <see cref="TrackingContext"/> to be copied in the context of the output message.
		/// </param>
		/// <returns>
		/// The transformed message with the result in the first part (at index 0).
		/// </returns>
		public static XLANGMessage Transform(XlangMessageCollection sourceMessages, Type mapType, TrackingContext trackingContext)
		{
			if (sourceMessages == null) throw new ArgumentNullException("sourceMessages");
			if (sourceMessages.Count == 0) throw new ArgumentException("XLangMessageCollection is empty.", "sourceMessages");
			if (mapType == null) throw new ArgumentNullException("mapType");
			using (sourceMessages)
			{
				var outputStream = Transform(sourceMessages.ToXmlReader(), mapType, null);
				var btxMessage = new CustomBtxMessage(Service.RootService.XlangStore.OwningContext, outputStream);
				XLANGMessage resultMessage = btxMessage.GetMessageWrapperForUserCode();
				trackingContext.Apply(resultMessage);
				return resultMessage;
			}
		}

		private static Stream Transform(XmlReader xmlReader, Type mapType, object[] splatteredTransformArguments)
		{
			if (_logger.IsDebugEnabled) _logger.DebugFormat("About to execute transform '{0}'.", mapType.AssemblyQualifiedName);
			var transformDescriptor = XsltCache.Instance[mapType];
			using (xmlReader)
			{
				var outputStream = new VirtualStream(DEFAULT_BUFFER_SIZE, DEFAULT_THRESHOLD_SIZE);
				using (var writer = XmlWriter.Create(outputStream, transformDescriptor.XslCompiledTransform.OutputSettings.Override(s => s.Encoding = Encoding.UTF8)))
				{
					if (_logger.IsFineEnabled) _logger.FineFormat("Executing transform '{0}'.", mapType.AssemblyQualifiedName);
					transformDescriptor.XslCompiledTransform.Transform(xmlReader, transformDescriptor.Arguments.Concat(splatteredTransformArguments), writer);
				}
				outputStream.Seek(0, SeekOrigin.Begin);
				return outputStream;
			}
		}

		/// <summary>
		/// This method will add custom transform arguments to the base transform arguments. For BizTalk maps, the base
		/// transform arguments essentially contain the necessary references to the extension objects used if any.
		/// </summary>
		/// <param name="argumentListTemplate"></param>
		/// <param name="splatteredTransformArguments"></param>
		/// <returns></returns>
		private static XsltArgumentList Concat(this Stateless.Xml.Xsl.XsltArgumentList argumentListTemplate, object[] splatteredTransformArguments)
		{
			var argumentList = argumentListTemplate.Clone();
			if (splatteredTransformArguments == null) return argumentList;

			for (var i = 0; i < splatteredTransformArguments.Length; i += 3)
			{
				argumentList.AddParam((string) splatteredTransformArguments[i], (string) splatteredTransformArguments[i + 1], splatteredTransformArguments[i + 2]);
			}
			return argumentList;
		}

		private static XmlWriterSettings Override(this XmlWriterSettings outputSettingsTemplate, Action<XmlWriterSettings> overrider)
		{
			// get a modifiable copy of the settings
			var outputSettings = outputSettingsTemplate.Clone();
			overrider(outputSettings);
			// ensure the underlying stream stay open
			outputSettings.CloseOutput = false;
			return outputSettings;
		}

		private const int DEFAULT_BUFFER_SIZE = 10 * 1024; //10 KB
		private const int DEFAULT_THRESHOLD_SIZE = 1024 * 1024; //1 MB

		private static readonly ILog _logger = LogManager.GetLogger(typeof(XlangTransformHelper));
	}
}
