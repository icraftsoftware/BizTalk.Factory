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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.Linq;

namespace Be.Stateless.BizTalk.Streaming
{
	[XmlRoot(ElementName = "XmlTranslations", Namespace = NAMESPACE)]
	public class XmlTranslationSet : IEquatable<XmlTranslationSet>
	{
		public static bool operator ==(XmlTranslationSet left, XmlTranslationSet right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(XmlTranslationSet left, XmlTranslationSet right)
		{
			return !Equals(left, right);
		}

		#region IEquatable<XmlTranslationSet> Members

		public bool Equals(XmlTranslationSet other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Override.Equals(other.Override) && Items.SequenceEqual(other.Items);
		}

		#endregion

		#region Base Class Member Overrides

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((XmlTranslationSet) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				// TODO ensure Items.GetHashCode() works as expected
				return (Override.GetHashCode() * 397) ^ (Items != null ? Items.GetHashCode() : 0);
			}
		}

		#endregion

		[XmlAttribute("override")]
		public bool Override { get; set; }

		[XmlElement("NamespaceTranslation")]
		public XmlNamespaceTranslation[] Items
		{
			get { return _items; }
			set
			{
				CheckItemsUnivoqueness(value);
				_items = value;
			}
		}

		public XmlTranslationSet Union(XmlTranslationSet second)
		{
			return Override
				? this
				: new XmlTranslationSet { Items = Items.Union(second.Items).ToArray() };
		}

		#region Helpers

		[Conditional("DEBUG")]
		internal static void CheckItemsUnivoqueness(IEnumerable<XmlNamespaceTranslation> items)
		{
			var conflictingReplacements = items
				// find MatchingPatterns declared multiple times
				.GroupBy(i => i.MatchingPatternString)
				// keep only those that have conflicting ReplacementPatterns, i.e. several disctinct ones
				.Where(g => g.Distinct(new LambdaComparer<XmlNamespaceTranslation>((lns, rns) => lns.ReplacementPattern == rns.ReplacementPattern)).Count() > 1)
				.ToArray();

			if (conflictingReplacements.Any())
				throw new ArgumentException(
					string.Format(
						"[{0}] matchingPatterns have respectively the following conflicting replacementPatterns: [{1}].",
						string.Join("], [", conflictingReplacements.Select(p => p.Key).ToArray()),
						string.Join("], [", conflictingReplacements.Select(g => string.Join(", ", g.Select(nr => nr.ReplacementPattern).ToArray())).ToArray())));
		}

		#endregion

		public const string NAMESPACE = "urn:schemas.stateless.be:biztalk:translations:2013:07";

		public static readonly XmlTranslationSet Empty = new XmlTranslationSet { Items = new XmlNamespaceTranslation[] { } };
		private XmlNamespaceTranslation[] _items;
	}
}
