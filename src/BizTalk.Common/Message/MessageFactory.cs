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
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Message
{
	public static class MessageFactory
	{
		/// <summary>
		/// Creates a <see cref="Claim.Check"/> message with a given <paramref name="url"/> claim.
		/// </summary>
		/// <param name="url">
		/// The URL to some payload that will later on be checked out.
		/// </param>
		/// <returns>
		/// The <see cref="Claim.Check"/> message as an <see cref="XmlDocument"/>.
		/// </returns>
		public static XmlDocument CreateClaimCheck(string url)
		{
			if (url.IsNullOrEmpty()) throw new ArgumentNullException("url");
			var message = CreateMessage<Claim.Check>(
				string.Format(
					"<clm:Check xmlns:clm='{0}'><clm:Url>{1}</clm:Url></clm:Check>",
					_claimSchemaTargetNamespace,
					url));
			return message;
		}

		/// <summary>
		/// Creates a <see cref="Claim.Check"/> message with a given <paramref name="messageType"/> and <paramref
		/// name="url"/> claim.
		/// </summary>
		/// <param name="messageType">
		/// The actual message type of the payload being claimed.
		/// </param>
		/// <param name="url">
		/// The URL to some payload that will later on be checked out.
		/// </param>
		/// <returns>
		/// The <see cref="Claim.Check"/> message as an <see cref="XmlDocument"/>.
		/// </returns>
		public static XmlDocument CreateClaimCheck(string messageType, string url)
		{
			if (url.IsNullOrEmpty()) throw new ArgumentNullException("url");
			if (messageType.IsNullOrEmpty()) return CreateClaimCheck(url);
			var message = CreateMessage<Claim.Check>(
				string.Format(
					"<clm:Check xmlns:clm='{0}'><clm:MessageType>{1}</clm:MessageType><clm:Url>{2}</clm:Url></clm:Check>",
					_claimSchemaTargetNamespace,
					messageType,
					url));
			return message;
		}

		/// <summary>
		/// Creates a <see cref="Claim.CheckIn"/> message with a given <paramref name="url"/> claim.
		/// </summary>
		/// <param name="url">
		/// The URL to some payload that will later on be checked out.
		/// </param>
		/// <returns>
		/// The <see cref="Claim.CheckIn"/> message as an <see cref="XmlDocument"/>.
		/// </returns>
		public static XmlDocument CreateClaimCheckIn(string url)
		{
			if (url.IsNullOrEmpty()) throw new ArgumentNullException("url");
			var message = CreateMessage<Claim.CheckIn>(
				string.Format(
					"<clm:CheckIn xmlns:clm='{0}'><clm:Url>{1}</clm:Url></clm:CheckIn>",
					_claimSchemaTargetNamespace,
					url));
			return message;
		}

		/// <summary>
		/// Creates a <see cref="Claim.CheckIn"/> message with a given <paramref name="messageType"/> and <paramref
		/// name="url"/> claim.
		/// </summary>
		/// <param name="messageType">
		/// The actual message type of the payload being claimed.
		/// </param>
		/// <param name="url">
		/// The URL to some payload that will later on be checked out.
		/// </param>
		/// <returns>
		/// The <see cref="Claim.CheckIn"/> message as an <see cref="XmlDocument"/>.
		/// </returns>
		public static XmlDocument CreateClaimCheckIn(string messageType, string url)
		{
			if (url.IsNullOrEmpty()) throw new ArgumentNullException("url");
			if (messageType.IsNullOrEmpty()) return CreateClaimCheckIn(url);
			var message = CreateMessage<Claim.CheckIn>(
				string.Format(
					"<clm:CheckIn xmlns:clm='{0}'><clm:MessageType>{1}</clm:MessageType><clm:Url>{2}</clm:Url></clm:CheckIn>",
					_claimSchemaTargetNamespace,
					messageType,
					url));
			return message;
		}

		/// <summary>
		/// Creates a <see cref="Claim.CheckOut"/> message with a given <paramref name="url"/> claim.
		/// </summary>
		/// <param name="url">
		/// The URL to some payload that is to be checked out.
		/// </param>
		/// <returns>
		/// The <see cref="Claim.CheckOut"/> message as an <see cref="XmlDocument"/>.
		/// </returns>
		public static XmlDocument CreateClaimCheckOut(string url)
		{
			if (url.IsNullOrEmpty()) throw new ArgumentNullException("url");
			var message = CreateMessage<Claim.CheckOut>(
				string.Format(
					"<clm:CheckOut xmlns:clm='{0}'><clm:Url>{1}</clm:Url></clm:CheckOut>",
					_claimSchemaTargetNamespace,
					url));
			return message;
		}

		/// <summary>
		/// Creates a dummy envelope document for a given <see cref="SchemaBase"/>-derived envelope schema type
		/// <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="SchemaBase"/>-derived envelope schema type.
		/// </typeparam>
		/// <returns>
		/// The dummy envelope document as an <see cref="XmlDocument"/>.
		/// </returns>
		/// <remarks>
		/// Notice the that the <c>&lt;ns:parts-here xmlns:ns="urn:schemas.stateless.be:biztalk:batch:2012:12"
		/// /&gt;</c> XML placeholder element will be inserted where the content of the envelope's body must be located;
		/// this provide an easy way for XSLT transforms to latter override the body's content.
		/// </remarks>
		public static XmlDocument CreateEnvelope<T>() where T : SchemaBase, new()
		{
			return CreateEnvelope(typeof(T));
		}

		/// <summary>
		/// Creates a envelope document with content for a given <see cref="SchemaBase"/>-derived envelope schema type
		/// <typeparamref name="TE"/> and <see cref="SchemaBase"/>-derived content schema type <typeparamref name="TC"/> .
		/// </summary>
		/// <typeparam name="TE">
		/// The <see cref="SchemaBase"/>-derived envelope schema type.
		/// </typeparam>
		/// <typeparam name="TC">
		/// The <see cref="SchemaBase"/>-derived content schema type.
		/// </typeparam>
		/// <returns>
		/// The envelope document with its content as an <see cref="XmlDocument"/>.
		/// </returns>
		public static XmlDocument CreateEnvelope<TE, TC>(string content)
			where TE : SchemaBase, new()
			where TC : SchemaBase, new()
		{
			var message = new XmlDocument();
			using (var reader = new StringReader(content))
			{
				message.Load(ValidatingXmlReader.Create<TE, TC>(reader));
				return message;
			}
		}

		/// <summary>
		/// Creates a dummy envelope document for a given <see cref="SchemaBase"/>-derived envelope schema type
		/// </summary>
		/// <param name="schema">
		/// The <see cref="SchemaBase"/>-derived envelope schema type.
		/// </param>
		/// <returns>
		/// The dummy envelope document as an <see cref="XmlDocument"/>.
		/// </returns>
		/// <remarks>
		/// Notice the that the <c>&lt;ns:parts-here xmlns:ns="urn:schemas.stateless.be:biztalk:batch:2012:12" /&gt;</c>
		/// XML placeholder element will be inserted where the content of the envelope's body must be located; this
		/// provide an easy way for XSLT transforms to latter override the body's content.
		/// </remarks>
		public static XmlDocument CreateEnvelope(Type schema)
		{
			if (!schema.GetMetadata().IsEnvelopeSchema)
				throw new ArgumentException(
					string.Format(
						"Either {0} is not an envelope schema or does not derive from {1}.",
						schema.FullName,
						typeof(SchemaBase).FullName),
					"schema");

			var envelope = CreateMessage(schema);
			var xpath = schema.GetMetadata().BodyXPath;
			var body = envelope.SelectSingleNode(xpath);
			if (body == null) throw new InvalidOperationException(string.Format("Body element cannot be found for envelope schema '{0}'.", schema.FullName));
			// overwrite the whole body's dummy/default content with the parts' placeholder
			body.InnerXml = string.Format("<ns:parts-here xmlns:ns=\"{0}\" />", typeof(Batch.Content).GetMetadata().TargetNamespace);
			return envelope;
		}

		/// <summary>
		/// Creates a dummy instance document of a given <see cref="SchemaBase"/>-derived schema type <typeparamref
		/// name="T"/>.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="SchemaBase"/>-derived schema type.
		/// </typeparam>
		/// <returns>
		/// The dummy instance document as an <see cref="XmlDocument"/>.
		/// </returns>
		public static XmlDocument CreateMessage<T>() where T : SchemaBase, new()
		{
			return CreateMessage(typeof(T).GetMetadata().DocumentSpec);
		}

		/// <summary>
		/// Creates an <see cref="XmlDocument"/> from a given <paramref name="content"/> and ensures its validity against
		/// a given <see cref="SchemaBase"/>-derived schema type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="SchemaBase"/>-derived schema type.
		/// </typeparam>
		/// <param name="content">
		/// The instance document content.
		/// </param>
		/// <returns>
		/// The valid instance document as an <see cref="XmlDocument"/>.
		/// </returns>
		public static XmlDocument CreateMessage<T>(string content) where T : SchemaBase, new()
		{
			var message = new XmlDocument();
			using (var reader = new StringReader(content))
			{
				message.Load(ValidatingXmlReader.Create<T>(reader));
				return message;
			}
		}

		/// <summary>
		/// Creates a dummy instance document of a given <see cref="SchemaBase"/>-derived schema type <paramref
		/// name="schema"/>.
		/// </summary>
		/// <param name="schema">
		/// The <see cref="SchemaBase"/>-derived schema type.
		/// </param>
		/// <returns>
		/// The dummy instance document as an <see cref="XmlDocument"/>.
		/// </returns>
		public static XmlDocument CreateMessage(Type schema)
		{
			if (!schema.IsSchema())
				throw new ArgumentException(
					string.Format(
						"{0} does not derive from {1}.",
						schema.FullName,
						typeof(SchemaBase).FullName),
					"schema");
			return CreateMessage(schema.GetMetadata().DocumentSpec);
		}

		// TODO CreateMessage with TrackingContext (http://msdn.microsoft.com/en-us/library/aa995576.aspx)
		//public static XmlDocument/XLANGMessage CreateMessage(Type schema, TrackingContext trackingContext)
		//{
		//   var document = CreateMessage(schema.FullName, schema.Assembly.FullName);
		//   XLANGMessage sampleOutput = (XLANGMessage) document;
		//   TrackingHelper.ApplyTrackingProperties((XLANGMessage) document, trackingContext);
		//   return document;
		//}

		/// <summary>
		/// Creates a dummy instance document for a given schema's <see cref="DocumentSpec"/>.
		/// </summary>
		/// <param name="documentSpec">
		/// The schema's <see cref="DocumentSpec"/>.
		/// </param>
		/// <returns>
		/// The dummy instance document.
		/// </returns>
		/// <seealso href="http://biztalkmessages.vansplunteren.net/2008/06/19/create-message-instance-from-multiroot-xsd-using-documentspec/"/>
		private static XmlDocument CreateMessage(DocumentSpec documentSpec)
		{
			using (var writer = new StringWriter())
			{
				var document = new XmlDocument();
				document.Load(documentSpec.CreateXmlInstance(writer));
				return document;
			}
		}

		private static readonly string _claimSchemaTargetNamespace = typeof(Claim.Check).GetMetadata().TargetNamespace;
	}
}
