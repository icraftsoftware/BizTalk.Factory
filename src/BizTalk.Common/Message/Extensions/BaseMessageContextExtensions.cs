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
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Tracking;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	public static class BaseMessageContextExtensions
	{
		#region message context xml serialization

		public static string ToXml(this IBaseMessageContext context)
		{
			// cache xmlns while constructing xml infoset...
			var nsCache = new XmlDictionary();
			var xdoc = new XElement(
				"context",
				Enumerable.Range(0, (int) context.CountProperties).Select(
					i => {
						string name, ns;
						var value = context.ReadAt(i, out name, out ns);
						// give each property element a name of 'p' and store its actual name inside the 'n' attribute, which
						// avoids the cost of the name.IsValidQName() check for each of them as the name could be an xpath
						// expression in the case of a distinguished property
						return name.IndexOf("password", StringComparison.OrdinalIgnoreCase) > -1
							? null
							: new XElement(
								(XNamespace) nsCache.Add(ns).Value + "p",
								new XAttribute("n", name),
								context.IsPromoted(name, ns) ? new XAttribute("promoted", true) : null,
								value);
					}));

			// ... and declare/alias all of them at the root element level to minimize xml string size
			XmlDictionaryString xds;
			for (var i = 0; nsCache.TryLookup(i, out xds); i++)
			{
				xdoc.Add(new XAttribute(XNamespace.Xmlns + "s" + xds.Key.ToString(CultureInfo.InvariantCulture), xds.Value));
			}

			return xdoc.ToString(SaveOptions.DisableFormatting);
		}

		#endregion

		#region tracking context helpers

		/// <summary>
		/// Clear the <see cref="TrackingContext"/> stored into the <paramref name="messageContext"/>.
		/// </summary>
		/// <param name="messageContext">
		/// Message context to clear the <see cref="TrackingContext"/> from.
		/// </param>
		public static void ClearTrackingContext(this IBaseMessageContext messageContext)
		{
			if (messageContext == null) throw new ArgumentNullException("messageContext");
			messageContext.DeleteProperty(TrackingProperties.ProcessActivityId);
			messageContext.DeleteProperty(TrackingProperties.ProcessingStepActivityId);
			messageContext.DeleteProperty(TrackingProperties.MessagingStepActivityId);
		}

		/// <summary>
		/// Returns the <see cref="TrackingContext"/> stored into the <paramref name="messageContext"/>.
		/// </summary>
		/// <param name="messageContext">
		/// Message context that stores the <see cref="TrackingContext"/> to be returned.
		/// </param>
		/// <returns>
		/// The <see cref="TrackingContext"/> stored into the <paramref name="messageContext"/>.
		/// </returns>
		public static TrackingContext GetTrackingContext(this IBaseMessageContext messageContext)
		{
			if (messageContext == null) throw new ArgumentNullException("messageContext");
			return new TrackingContext {
				ProcessActivityId = messageContext.GetProperty(TrackingProperties.ProcessActivityId),
				ProcessingStepActivityId = messageContext.GetProperty(TrackingProperties.ProcessingStepActivityId),
				MessagingStepActivityId = messageContext.GetProperty(TrackingProperties.MessagingStepActivityId)
			};
		}

		/// <summary>
		/// Store the given <see cref="TrackingContext"/> into the <paramref name="messageContext"/>.
		/// </summary>
		/// <param name="messageContext">
		/// Message context to store the <paramref name="trackingContext"/> into.
		/// </param>
		/// <param name="trackingContext">
		/// The <see cref="TrackingContext"/> to be stored into the <paramref name="messageContext"/>.
		/// </param>
		public static TrackingContext SetTrackingContext(this IBaseMessageContext messageContext, TrackingContext trackingContext)
		{
			if (messageContext == null) throw new ArgumentNullException("messageContext");
			messageContext.SetProperty(TrackingProperties.ProcessActivityId, trackingContext.ProcessActivityId);
			messageContext.SetProperty(TrackingProperties.ProcessingStepActivityId, trackingContext.ProcessingStepActivityId);
			messageContext.SetProperty(TrackingProperties.MessagingStepActivityId, trackingContext.MessagingStepActivityId);
			return trackingContext;
		}

		#endregion
	}
}
