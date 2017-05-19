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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using Be.Stateless.BizTalk.Message.Extensions;
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
	/// <remarks>
	/// The implementation is directly inspired by <see
	/// href="http://blogs.msdn.com/b/paolos/archive/2010/01/29/how-to-boost-message-transformations-using-the-xslcompiledtransform-class.aspx">How
	/// To Boost Message Transformations Using the XslCompiledTransform class</see> and <see
	/// href="http://blogs.msdn.com/b/paolos/archive/2010/04/08/how-to-boost-message-transformations-using-the-xslcompiledtransform-class-extended.aspx">How
	/// To Boost Message Transformations Using the XslCompiledTransform class Extended</see>.
	/// </remarks>
	[SuppressMessage("ReSharper", "LocalizableElement")]
	public static class XlangTransformHelper
	{
		/// <summary>
		/// Applies the XSL transformation specified by <paramref name="map"/> to the specified <paramref name="message"/>
		/// and propagates the <paramref name="trackingContext"/> to the resulting message.
		/// </summary>
		/// <remarks>
		/// This method assumes only the first part of a multi-part <paramref name="message"/> message has to be
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
			return Transform(new XlangMessageCollection { message }, map, trackingContext);
		}

		/// <summary>
		/// Applies the XSL transformation specified by <paramref name="map"/> to the specified <paramref name="message"/>
		/// and propagates the <paramref name="trackingContext"/> to the resulting message.
		/// </summary>
		/// <remarks>
		/// This method assumes only the first part of a multi-part <paramref name="message"/> message has to be
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
		/// <param name="splatteredXsltArguments">
		/// The arguments to pass to the XSL transformation. The array must contain a number of items that is a multiple
		/// of 3. Each series of 3 items contains, in that order, an XSL parameter name, the namespace URI of the
		/// parameter, and its value.
		/// </param>
		/// <returns>
		/// The transformed message with the result in the first part (at index 0).
		/// </returns>
		public static XLANGMessage Transform(XLANGMessage message, Type map, TrackingContext trackingContext, params object[] splatteredXsltArguments)
		{
			if (message == null) throw new ArgumentNullException("message");
			return Transform(new XlangMessageCollection { message }, map, trackingContext, splatteredXsltArguments);
		}

		/// <summary>
		/// Applies the XSL transformation specified by <paramref name="map"/> to the specified <paramref
		/// name="messages"/> collection and propagates the <paramref name="trackingContext"/> to the resulting message.
		/// </summary>
		/// <remarks>
		/// This method assumes only the first part of any multi-part <paramref name="messages"/> message has to be
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

		/// <summary>
		/// Applies the XSL transformation specified by <paramref name="map"/> to the specified <paramref
		/// name="messages"/> collection and propagates the <paramref name="trackingContext"/> to the resulting message.
		/// </summary>
		/// <param name="messages">
		/// The <see cref="XlangMessageCollection"/> to be transformed.
		/// </param>
		/// <param name="map">
		/// The type of the BizTalk map class containing the transform to apply.
		/// </param>
		/// <param name="trackingContext">
		/// The <see cref="TrackingContext"/> to be copied in the context of the output message.
		/// </param>
		/// <param name="splatteredXsltArguments">
		/// The arguments to pass to the XSL transformation. The array must contain a number of items that is a multiple
		/// of 3. Each series of 3 items contains, in that order, an XSL parameter name, the namespace URI of the
		/// parameter, and its value.
		/// </param>
		/// <returns>
		/// The transformed message with the result in the first part (at index 0).
		/// </returns>
		public static XLANGMessage Transform(XlangMessageCollection messages, Type map, TrackingContext trackingContext, params object[] splatteredXsltArguments)
		{
			if (messages == null) throw new ArgumentNullException("messages");
			if (messages.Count == 0) throw new ArgumentException("XLangMessageCollection is empty.", "messages");
			if (map == null) throw new ArgumentNullException("map");
			if (splatteredXsltArguments == null) throw new ArgumentNullException("splatteredXsltArguments");
			if (splatteredXsltArguments.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", "splatteredXsltArguments");
			if (splatteredXsltArguments.Length % 3 != 0) throw new ArgumentException("Value must be a collection containing a number of items that is multiple of 3.", "splatteredXsltArguments");
			using (messages)
			{
				var resultContent = Transform(messages, map, splatteredXsltArguments);
				var resultMessage = XlangMessage.Create(Service.RootService.XlangStore.OwningContext, resultContent);
				trackingContext.Apply(resultMessage);
				return resultMessage;
			}
		}

		/// <summary>
		/// Applies the XSL transformation specified by <paramref name="map"/> to the specified <paramref
		/// name="sourceMessage"/> and adds the result as a part named <paramref name="destinationPartName"/> to the
		/// <paramref name="destinationMessage"/> message.
		/// </summary>
		/// <param name="sourceMessage">
		/// The <see cref="XLANGMessage"/> to be transformed.
		/// </param>
		/// <param name="map">
		/// The type of the BizTalk map class containing the transform to apply.
		/// </param>
		/// <param name="destinationMessage">
		/// Message to hold the transformation results.
		/// </param>
		/// <param name="destinationPartName">
		/// Name of the message part that will be added to <paramref name="destinationMessage"/> and that contains the
		/// results of the transformation.
		/// </param>
		public static void TransformAndAddPart(XLANGMessage sourceMessage, Type map, XLANGMessage destinationMessage, string destinationPartName)
		{
			if (sourceMessage == null) throw new ArgumentNullException("sourceMessage");
			if (map == null) throw new ArgumentNullException("map");
			if (destinationMessage == null) throw new ArgumentNullException("destinationMessage");
			try
			{
				using (var xmlReader = new XmlTextReader(sourceMessage.AsStream()))
				{
					var response = Transform(xmlReader, map, null);
					destinationMessage.AddPart(string.Empty, destinationPartName);
					destinationMessage[destinationPartName].LoadFrom(response);
				}
			}
			finally
			{
				sourceMessage.Dispose();
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
