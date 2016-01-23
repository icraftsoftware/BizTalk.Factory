#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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
	public static class BaseMessageContext
	{
		/// <summary>
		/// Returns the <see cref="TrackingContext"/> associated to the message <paramref name="context"/>.
		/// </summary>
		/// <param name="context">
		/// Message context whose associated <see cref="TrackingContext"/> will be returned.
		/// </param>
		/// <returns>
		/// The <see cref="TrackingContext"/> associated to the message <paramref name="context"/>.
		/// </returns>
		public static TrackingContext GetTrackingContext(this IBaseMessageContext context)
		{
			if (context == null) throw new ArgumentNullException("context");
			return new TrackingContext {
				ProcessActivityId = context.GetProperty(TrackingProperties.ProcessActivityId),
				ProcessingStepActivityId = context.GetProperty(TrackingProperties.ProcessingStepActivityId),
				MessagingStepActivityId = context.GetProperty(TrackingProperties.MessagingStepActivityId)
			};
		}

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
								((XNamespace) nsCache.Add(ns).Value) + "p",
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
	}
}
