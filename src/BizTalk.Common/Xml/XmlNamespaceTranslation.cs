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
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.Runtime.Caching;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Xml
{
	[XmlRoot(ElementName = "NamespaceTranslation", Namespace = XmlTranslationSet.NAMESPACE)]
	public class XmlNamespaceTranslation : IEquatable<XmlNamespaceTranslation>
	{
		public static bool operator ==(XmlNamespaceTranslation left, XmlNamespaceTranslation right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(XmlNamespaceTranslation left, XmlNamespaceTranslation right)
		{
			return !Equals(left, right);
		}

		public XmlNamespaceTranslation() { }

		public XmlNamespaceTranslation(string matchingPattern, string replacementPattern)
		{
			MatchingPatternString = matchingPattern;
			ReplacementPattern = replacementPattern;
		}

		#region IEquatable<XmlNamespaceTranslation> Members

		public bool Equals(XmlNamespaceTranslation other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(MatchingPatternString, other.MatchingPatternString) && string.Equals(ReplacementPattern, other.ReplacementPattern);
		}

		#endregion

		#region Base Class Member Overrides

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((XmlNamespaceTranslation) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((MatchingPatternString != null ? MatchingPatternString.GetHashCode() : 0) * 397) ^ (ReplacementPattern != null ? ReplacementPattern.GetHashCode() : 0);
			}
		}

		#endregion

		[XmlIgnore]
		public Regex MatchingPattern
		{
			get { return MatchingPatternString.IsNullOrEmpty() ? RegexCache.Instance["^$"] : RegexCache.Instance[MatchingPatternString]; }
		}

		[XmlAttribute("matchingPattern")]
		public string MatchingPatternString { get; set; }

		[XmlAttribute("replacementPattern")]
		public string ReplacementPattern { get; set; }
	}
}
