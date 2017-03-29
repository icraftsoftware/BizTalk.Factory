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
using System.Xml;
using System.Xml.Schema;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.Extensions;
using Be.Stateless.Logging;
using Be.Stateless.Xml.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	public static class BaseMessagePart
	{
		public static Stream AsStream(this XLANGPart messagePart)
		{
			return (Stream) messagePart.RetrieveAs(typeof(Stream));
		}

		/// <summary>
		/// Return the content of a claim token message, either <see cref="Claim.Check"/>, <see cref="Claim.CheckIn"/>, or
		/// <see cref="Claim.CheckOut"/>, as a <see cref="MessageBodyCaptureDescriptor"/> filled in according to the
		/// content of the claim token.
		/// </summary>
		/// <param name="messagePart">
		/// The part whose stream contains a claim token.
		/// </param>
		/// <returns>
		/// The <see cref="MessageBodyCaptureDescriptor"/> corresponding to the content of the claim token.
		/// </returns>
		internal static MessageBodyCaptureDescriptor AsMessageBodyCaptureDescriptor(this IBaseMessagePart messagePart)
		{
			// Claim.Check, Claim.CheckIn, and Claim.CheckOut are all in the same XML Schema: any one can be used to
			// reference the XML Schema to use to validate a token message, whatever its specific type
			using (var reader = ValidatingXmlReader.Create<Claim.CheckOut>(messagePart.GetOriginalDataStream(), XmlSchemaContentProcessing.Lax))
			{
				return reader.AsMessageBodyCaptureDescriptor();
			}
		}

		internal static MessageBodyCaptureDescriptor AsMessageBodyCaptureDescriptor(this XmlReader reader)
		{
			var document = new XmlDocument();
			document.Load(reader);
			// Claim.Check, Claim.CheckIn, and Claim.CheckOut are all in the same XML Schema: any one can be used to
			// reference the XML Schema TargetNamespace, whatever its specific type
			var nsmgr = document.GetNamespaceManager();
			nsmgr.AddNamespace("s0", typeof(Claim.CheckOut).GetMetadata().TargetNamespace);
			// extract url from claim token
			var urlNode = document.SelectSingleNode("/*/s0:Url", nsmgr);
			if (urlNode == null) throw new ArgumentException(string.Format("{0} token message has no Url element.", document.DocumentElement.IfNotNull(de => de.Name)));
			return new MessageBodyCaptureDescriptor(urlNode.InnerText, MessageBodyCaptureMode.Claimed);
		}

		/// <summary>
		/// Replaces this <paramref name="messagePart"/>'s original data stream by another <paramref name="stream"/>.
		/// </summary>
		/// <param name="messagePart">
		/// The part whose original data stream is replaced.
		/// </param>
		/// <param name="stream">
		/// The replacement stream.
		/// </param>
		/// <param name="tracker">
		/// Pipeline's resource tracker to which to report the newly created wrapping stream.
		/// </param>
		public static void SetDataStream(this IBaseMessagePart messagePart, Stream stream, IResourceTracker tracker)
		{
			// TODO ?consider providing an overload when there is no IResourceTracker as this is necessary only for unmanaged objects/resources? not really necessary as one could use Data setter, but it'd be consistent/balanced API

			if (messagePart == null) throw new ArgumentNullException("messagePart");
			if (stream == null) throw new ArgumentNullException("stream");
			if (tracker == null) throw new ArgumentNullException("tracker");

			messagePart.Data = stream;
			tracker.AddResource(stream);
		}

		/// <summary>
		/// Wraps this message part's original data stream in another stream returned by the <paramref name="wrapper"/>
		/// delegate.
		/// </summary>
		/// <param name="messagePart">
		/// The part whose original data stream is wrapped.
		/// </param>
		/// <param name="wrapper">
		/// A delegate, or stream factory, that returns the stream wrapping the original one.
		/// </param>
		/// <param name="tracker">
		/// Pipeline's resource tracker to which to report the newly created wrapping stream.
		/// </param>
		/// <returns>
		/// The new wrapping <see cref="Stream"/> if it is not the same instance as the original one. The original <see
		/// cref="Stream"/> otherwise.
		/// </returns>
		public static T WrapOriginalDataStream<T>(this IBaseMessagePart messagePart, Func<Stream, T> wrapper, IResourceTracker tracker) where T : Stream
		{
			// TODO ?consider providing an overload when there is no IResourceTracker as this is necessary only for unmanaged objects/resources?

			if (messagePart == null) throw new ArgumentNullException("messagePart");
			if (wrapper == null) throw new ArgumentNullException("wrapper");
			if (tracker == null) throw new ArgumentNullException("tracker");

			var originalDataStream = messagePart.GetOriginalDataStream();
			if (originalDataStream == null) return null;

			var wrappingStream = wrapper(originalDataStream);
			if (ReferenceEquals(originalDataStream, wrappingStream)) return (T) originalDataStream;

			if (_logger.IsDebugEnabled) _logger.DebugFormat("Wrapping message part's original data stream in a '{0}' stream.", wrappingStream.GetType().FullName);
			messagePart.SetDataStream(wrappingStream, tracker);
			return wrappingStream;
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(BaseMessagePart));
	}
}
