#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using Be.Stateless.BizTalk.Runtime.Caching;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Streaming;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.XLANGs.Core;

namespace Be.Stateless.BizTalk.Transform
{
#pragma warning disable 612,618
	/// <summary>
	/// This class contains methods to use an <see cref="XslCompiledTransform"/> from BizTalk, instead of an <see
	/// cref="XslTransform"/>.
	/// </summary>
	/// <remarks>
	/// It is a slightly modified version of the solution described at <see
	/// href="http://blogs.msdn.com/b/paolos/archive/2010/04/08/how-to-boost-message-transformations-using-the-xslcompiledtransform-class-extended.aspx"/>.
	/// It contains, among other, instrumentation code and <see cref="TrackingContext"/> propagation. See also <see
	/// href="http://blogs.msdn.com/b/paolos/archive/2010/01/29/how-to-boost-message-transformations-using-the-xslcompiledtransform-class.aspx"/>.
	/// </remarks>
#pragma warning restore 612,618
	// TODO to be deprecated
	public static class XslCompiledTransformHelper
	{
		#region XLANGMessage based tranforms

		/// <summary>
		/// Applies an XSL transformation to the specified <paramref name="sourceMessage"/>. The XSL transformation to be
		/// applied is specified in <paramref name="mapType"/>. The supplied <paramref name="trackingContext"/> is
		/// propagated to the resulting message. This method does by default "the right thing" with multipart messages: it
		/// takes the first part of the <paramref name="sourceMessage"/> and creates an output message with a single part
		/// named "Main".
		/// </summary>
		/// <param name="sourceMessage">The <see cref="XLANGMessage"/> to be transformed.</param>
		/// <param name="mapType">The type of the BizTalk map class containing the transform to apply.</param>
		/// <param name="trackingContext">The <see cref="TrackingContext"/> to be copied in the context of the output message.</param>
		/// <returns>The transformed message, with the result in the first part (at index 0).</returns>
		public static XLANGMessage Transform(XLANGMessage sourceMessage, Type mapType, TrackingContext trackingContext)
		{
			if (sourceMessage == null) throw new ArgumentNullException("sourceMessage");
			if (mapType == null) throw new ArgumentNullException("mapType");
			return Transform(trackingContext, mapType, sourceMessage, null);
		}

		/// <summary>
		/// Applies an XSL transformation to the specified <paramref name="sourceMessage"/>. The XSL transformation to be
		/// applied is specified in <paramref name="mapType"/>. The supplied <paramref name="trackingContext"/> is
		/// propagated to the resulting message. This method does by default "the right thing" with multipart messages: it
		/// takes the first part of the <paramref name="sourceMessage"/> and creates an output message with a single part
		/// named "Main".
		/// </summary>
		/// <param name="trackingContext">The <see cref="TrackingContext"/> to be copied in the context of the output message.</param>
		/// <param name="mapType">The type of the BizTalk map class containing the transform to apply.</param>
		/// <param name="sourceMessage">The <see cref="XLANGMessage"/> to be transformed.</param>
		/// <param name="transformArguments">The arguments to pass to the XSL transformation. The array must contain a
		/// number of items that is a multiple of 3. Each series of 3 items contains, in that order, an XSL parameter
		/// name, the namespace URI of the parameter, and its value.</param>
		/// <returns>The transformed message, with the result in the first part (at index 0).</returns>
		public static XLANGMessage Transform(TrackingContext trackingContext, Type mapType, XLANGMessage sourceMessage, params object[] transformArguments)
		{
			if (mapType == null) throw new ArgumentNullException("mapType");
			if (sourceMessage == null) throw new ArgumentNullException("sourceMessage");
			if (transformArguments != null)
			{
				if (transformArguments.Length % 3 != 0)
					throw new ArgumentException(
						"Arguments of the XSL transform were not specified correctly. Please specify for each argument its name, namespace URI and value.",
						"transformArguments");
			}

			return Transform(
				sourceMessage,
				0,
				mapType,
				DEFAULT_MESSAGE_NAME,
				DEFAULT_PART_NAME,
				DEFAULT_BUFFER_SIZE,
				DEFAULT_THRESHOLD_SIZE,
				trackingContext,
				transformArguments);
		}

		public static XLANGMessage Transform(
			XLANGMessage sourceMessage1,
			XLANGMessage sourceMessage2,
			Type mapType,
			TrackingContext trackingContext,
			params object[] transformArguments)
		{
			if (sourceMessage1 == null)
			{
				throw new ArgumentNullException("sourceMessage1");
			}
			if (sourceMessage2 == null)
			{
				throw new ArgumentNullException("sourceMessage2");
			}
			if (mapType == null) throw new ArgumentNullException("mapType");

			var sourceMessages = new[] { sourceMessage1, sourceMessage2 };

			return Transform(
				sourceMessages,
				Enumerable.Repeat(0, sourceMessages.Length).ToArray(),
				mapType,
				DEFAULT_MESSAGE_NAME,
				DEFAULT_PART_NAME,
				DEFAULT_BUFFER_SIZE,
				DEFAULT_THRESHOLD_SIZE,
				trackingContext,
				transformArguments);
		}

		public static XLANGMessage Transform(
			XLANGMessage sourceMessage1,
			XLANGMessage sourceMessage2,
			XLANGMessage sourceMessage3,
			Type mapType,
			TrackingContext trackingContext,
			params object[] transformArguments)
		{
			if (sourceMessage1 == null)
			{
				throw new ArgumentNullException("sourceMessage1");
			}
			if (sourceMessage2 == null)
			{
				throw new ArgumentNullException("sourceMessage2");
			}
			if (sourceMessage3 == null)
			{
				throw new ArgumentNullException("sourceMessage3");
			}
			if (mapType == null) throw new ArgumentNullException("mapType");

			var sourceMessages = new[] { sourceMessage1, sourceMessage2, sourceMessage3 };

			return Transform(
				sourceMessages,
				Enumerable.Repeat(0, sourceMessages.Length).ToArray(),
				mapType,
				DEFAULT_MESSAGE_NAME,
				DEFAULT_PART_NAME,
				DEFAULT_BUFFER_SIZE,
				DEFAULT_THRESHOLD_SIZE,
				trackingContext,
				transformArguments);
		}

		public static XLANGMessage Transform(
			XLANGMessage sourceMessage1,
			XLANGMessage sourceMessage2,
			XLANGMessage sourceMessage3,
			XLANGMessage sourceMessage4,
			Type mapType,
			TrackingContext trackingContext,
			params object[] transformArguments)
		{
			if (sourceMessage1 == null)
			{
				throw new ArgumentNullException("sourceMessage1");
			}
			if (sourceMessage2 == null)
			{
				throw new ArgumentNullException("sourceMessage2");
			}
			if (sourceMessage3 == null)
			{
				throw new ArgumentNullException("sourceMessage3");
			}
			if (sourceMessage4 == null) throw new ArgumentNullException("sourceMessage4");
			if (mapType == null) throw new ArgumentNullException("mapType");

			var sourceMessages = new[] { sourceMessage1, sourceMessage2, sourceMessage3, sourceMessage4 };

			return Transform(
				sourceMessages,
				Enumerable.Repeat(0, sourceMessages.Length).ToArray(),
				mapType,
				DEFAULT_MESSAGE_NAME,
				DEFAULT_PART_NAME,
				DEFAULT_BUFFER_SIZE,
				DEFAULT_THRESHOLD_SIZE,
				trackingContext,
				transformArguments);
		}

		/// <summary>
		/// Applies the transformation in the BizTalk map <paramref name="mapType"/> to the first part of <paramref
		/// name="sourceMessage"/> and adds the result as a part named <paramref name="destinationPartName"/> to the
		/// <paramref name="destinationMessage"/>.
		/// </summary>
		/// <param name="sourceMessage">Message to transform, in part at index 0.</param>
		/// <param name="mapType">Map type to execute the transformation.</param>
		/// <param name="destinationMessage">Message to hold the transformation results.</param>
		/// <param name="destinationPartName">Name of the message part that will be added to <paramref name="destinationMessage"/> 
		/// and that contains the results of the transformation.</param>
		public static void TransformAndAddPart(XLANGMessage sourceMessage, Type mapType, XLANGMessage destinationMessage, string destinationPartName)
		{
			if (sourceMessage == null) throw new ArgumentNullException("sourceMessage");
			if (mapType == null) throw new ArgumentNullException("mapType");
			if (destinationMessage == null) throw new ArgumentNullException("destinationMessage");

			if (_logger.IsDebugEnabled) _logger.DebugFormat("About to execute transform {0} on an XLANG message", mapType.AssemblyQualifiedName);
			try
			{
				using (var stream = (Stream) sourceMessage[0].RetrieveAs(typeof(Stream)))
				using (var xmlReader = new XmlTextReader(stream))
				{
					Stream response = Transform(xmlReader, mapType, Encoding.UTF8, DEFAULT_BUFFER_SIZE, DEFAULT_THRESHOLD_SIZE, null);
					destinationMessage.AddPart(string.Empty, destinationPartName);
					destinationMessage[destinationPartName].LoadFrom(response);
				}
			}
			finally
			{
				sourceMessage.Dispose();
			}
		}

		/// <summary>
		/// Applies an XSL transformation to the specified <paramref name="sourceMessage"/>. The XSL transformation to be
		/// applied is specified in <paramref name="mapType"/>. The supplied <paramref name="trackingContext"/> is
		/// propagated to the resulting message. This method does by default "the right thing" with multipart messages: it
		/// takes the first part of the <paramref name="sourceMessage"/> and creates an output message with a single part
		/// named "Main".
		/// </summary>
		/// <param name="sourceMessage">
		/// The <see cref="XLANGMessage"/> to be transformed.
		/// </param>
		/// <param name="sourcePartIndex">
		/// The index of the <see cref="XLANGPart"/> to be transformed in the input message.
		/// </param>
		/// <param name="mapType">
		/// The assembly qualified name of the BizTalk map class containing the transform to apply.
		/// </param>
		/// <param name="messageName">
		/// The <see cref="XLANGMessage.Name"/> of the output message.</param>
		/// <param name="partName">
		/// The name of <see cref="XLANGPart"/> in the output message. This part is always at index 0.
		/// </param>
		/// <param name="bufferSize">
		/// Size of the buffer used for the underlying <see cref="VirtualStream"/> used during transform.
		/// </param>
		/// <param name="thresholdSize">
		/// Threshold before overflowing to disk for the underlying <see cref="VirtualStream"/> used during transform.
		/// </param>
		/// <param name="trackingContext">The <see cref="TrackingContext"/> to be copied in the context of the output message.</param>
		/// <param name="transformArguments">
		/// See <see
		/// cref="Transform(Be.Stateless.BizTalk.Tracking.TrackingContext,System.Type,Microsoft.XLANGs.BaseTypes.XLANGMessage,object[])"/>
		/// </param>
		/// <returns>
		/// The transformed message, with the result in the first message part.
		/// </returns>
		private static XLANGMessage Transform(
			XLANGMessage sourceMessage,
			int sourcePartIndex,
			Type mapType,
			string messageName,
			string partName,
			int bufferSize,
			int thresholdSize,
			TrackingContext trackingContext,
			object[] transformArguments)
		{
			if (sourceMessage == null) throw new ArgumentNullException("sourceMessage");

			if (_logger.IsDebugEnabled) _logger.DebugFormat("About to execute transform {0} on an XLANG message", mapType);
			try
			{
				using (var stream = (Stream) sourceMessage[sourcePartIndex].RetrieveAs(typeof(Stream)))
				using (var xmlReader = new XmlTextReader(stream))
				{
					var response = Transform(xmlReader, mapType, Encoding.UTF8, bufferSize, thresholdSize, transformArguments);
					var customBtxMessage = new CustomBtxMessage(messageName, Service.RootService.XlangStore.OwningContext);
					customBtxMessage.AddPart(string.Empty, partName);
					customBtxMessage[0].LoadFrom(response);
					XLANGMessage result = customBtxMessage.GetMessageWrapperForUserCode();
					trackingContext.Apply(result);
					return result;
				}
			}
			finally
			{
				sourceMessage.Dispose();
			}
		}

		private static XLANGMessage Transform(
			XLANGMessage[] sourceMessages,
			int[] partIndexArray,
			Type mapType,
			string messageName,
			string partName,
			int bufferSize,
			int thresholdSize,
			TrackingContext trackingContext,
			object[] transformArguments)
		{
			try
			{
				if (sourceMessages != null && sourceMessages.Length > 0)
				{
					var streamArray = new Stream[sourceMessages.Length];
					for (var i = 0; i < sourceMessages.Length; i++)
					{
						streamArray[i] = sourceMessages[i][partIndexArray[i]].RetrieveAs(typeof(Stream)) as Stream;
					}
					using (var reader = CompositeXmlReader.Create(streamArray, new XmlReaderSettings { CloseInput = true }))
					{
						var response = Transform(reader, mapType, Encoding.UTF8, bufferSize, thresholdSize, transformArguments);
						var customBtxMessage = new CustomBtxMessage(messageName, Service.RootService.XlangStore.OwningContext);
						customBtxMessage.AddPart(string.Empty, partName);
						customBtxMessage[0].LoadFrom(response);
						XLANGMessage result = customBtxMessage.GetMessageWrapperForUserCode();
						trackingContext.Apply(result);
						return result;
					}
				}
			}
			finally
			{
				if (sourceMessages != null && sourceMessages.Length > 0)
				{
					foreach (var t in sourceMessages.Where(t => t != null))
					{
						t.Dispose();
					}
				}
			}
			return null;
		}

		#endregion

		private static Stream Transform(
			XmlReader xmlReader,
			Type mapType,
			Encoding outputEncoding,
			int bufferSize,
			int thresholdSize,
			object[] transformArguments)
		{
			if (_logger.IsDebugEnabled) _logger.DebugFormat("About to execute transform {0} on an input stream.", mapType.AssemblyQualifiedName);
			var transformDescriptor = XsltCache.Instance[mapType];
			using (xmlReader)
			{
				var outputStream = new VirtualStream(bufferSize, thresholdSize);
				using (var writer = CreateXmlWriter(outputStream, outputEncoding, transformDescriptor.XslCompiledTransform.OutputSettings))
				{
					transformDescriptor.XslCompiledTransform.Transform(xmlReader, BuildTransformArguments(transformDescriptor.Arguments.Clone(), transformArguments), writer);
				}
				outputStream.Seek(0, SeekOrigin.Begin);
				return outputStream;
			}
		}

		/// <summary>
		/// This method will add custom transform arguments to the base transform arguments. For BizTalk maps, the base
		/// transform arguments essentially contain the necessary references to the extension objects used if any.
		/// </summary>
		/// <param name="arguments"></param>
		/// <param name="transformArguments"></param>
		/// <returns></returns>
		private static XsltArgumentList BuildTransformArguments(XsltArgumentList arguments, object[] transformArguments)
		{
			if (transformArguments != null && transformArguments.Length != 0)
			{
				for (var i = 0; i < transformArguments.Length; i += 3)
				{
					arguments.AddParam((string) transformArguments[i], (string) transformArguments[i + 1], transformArguments[i + 2]);
				}
			}
			return arguments;
		}

		private static XmlWriter CreateXmlWriter(Stream outputStream, Encoding outputEncoding, XmlWriterSettings outputSettings)
		{
			// ensure we have a modifiable copy of the settings from the transform (ReadOnly = false)
			var settings = outputSettings.Clone();
			settings.Encoding = outputEncoding;
			// so we are sure the underlying stream will stay open!!
			settings.CloseOutput = false;
			return XmlWriter.Create(outputStream, settings);
		}

		private const string DEFAULT_MESSAGE_NAME = "transformedMessage";
		private const string DEFAULT_PART_NAME = "Main";
		private const int DEFAULT_BUFFER_SIZE = 10240; //10 KB
		private const int DEFAULT_THRESHOLD_SIZE = 1048576; //1 MB

		private static readonly ILog _logger = LogManager.GetLogger(typeof(XslCompiledTransformHelper));
	}
}
