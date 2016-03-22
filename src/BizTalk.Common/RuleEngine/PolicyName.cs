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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Be.Stateless.Extensions;
using Microsoft.RuleEngine;

namespace Be.Stateless.BizTalk.RuleEngine
{
	/// <summary>
	/// A <see cref="PolicyName"/> provides a structured identifier to a <see cref="Policy"/>, much like a <see
	/// cref="RuleSetInfo"/> does but with back-and-forth <see cref="string"/> conversion.
	/// </summary>
	[TypeConverter(typeof(PolicyNameConverter))]
	public class PolicyName : RuleSetInfo, IEquatable<RuleSetInfo>
	{
		/// <summary>
		/// Converts the given <see cref="string"/> to a <see cref="PolicyName"/> instance.
		/// </summary>
		/// <param name="displayName">
		/// The <see cref="string"/> to convert to a <see cref="PolicyName"/> instance.
		/// </param>
		/// <returns>
		/// A <see cref="PolicyName"/> instance that represents the converted <paramref name="displayName"/>.
		/// </returns>
		/// <exception cref="NotSupportedException">
		/// The conversion cannot be performed.
		/// </exception>
		/// <remarks>
		/// This <see cref="PolicyName"/> can be converted back to a <see cref="string"/> via the <see cref="ToString"/>
		/// methods.
		/// </remarks>
		/// <seealso cref="ToString"/>
		public static PolicyName Parse(string displayName)
		{
			if (displayName.IsNullOrEmpty()) throw new NotSupportedException(string.Format("Cannot parse a null or empty string into a {0}.", typeof(PolicyName).Name));

			var match = _regex.Match(displayName);
			if (!match.Success) throw new NotSupportedException(string.Format("'{0}' format is invalid and cannot be parsed into a {1}.", displayName, typeof(PolicyName).Name));

			return new PolicyName(match.Groups["Name"].Value, int.Parse(match.Groups["Major"].Value), int.Parse(match.Groups["Minor"].Value));
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Required by XML serialization but not supposed to be called.")]
		public PolicyName() : base(string.Empty, 0, 0)
		{
			throw new NotSupportedException();
		}

		public PolicyName(RuleSetInfo ruleSetInfo) : base(ruleSetInfo.Name, ruleSetInfo.MajorRevision, ruleSetInfo.MinorRevision) { }

		public PolicyName(string name, int major, int minor) : base(name, major, minor) { }

		#region Base Class Member Overrides

		/// <summary>
		/// Converts a <see cref="PolicyName"/> instance to its <see cref="string"/> representation.
		/// </summary>
		/// <returns>
		/// A <see cref="string"/> that represents the <see cref="PolicyName"/>.
		/// </returns>
		/// <remarks>
		/// This <see cref="string"/> can be converted back to a <see cref="PolicyName"/> via the <see cref="Parse"/>
		/// methods.
		/// </remarks>
		/// <seealso cref="Parse"/>
		public override string ToString()
		{
			return string.Format("{0}, Version={1}.{2}", Name, MajorRevision, MinorRevision);
		}

		#endregion

		private static readonly Regex _regex = new Regex(@"^(?<Name>\w[\w\.]+?)\,\sVersion=(?<Major>\d+)\.(?<Minor>\d)$", RegexOptions.Singleline | RegexOptions.Compiled);

		#region Equality members

		public static bool operator ==(PolicyName left, PolicyName right)
		{
			return Equals(left, right);
		}

		public static bool operator ==(PolicyName left, RuleSetInfo right)
		{
			return Equals(left, right);
		}

		public static bool operator ==(RuleSetInfo left, PolicyName right)
		{
			return Equals(right, left);
		}

		public static bool operator !=(PolicyName left, PolicyName right)
		{
			return !Equals(left, right);
		}

		public static bool operator !=(PolicyName left, RuleSetInfo right)
		{
			return !Equals(left, right);
		}

		public static bool operator !=(RuleSetInfo left, PolicyName right)
		{
			return !Equals(right, left);
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">
		/// An object to compare with this object.
		/// </param>
		/// <returns>
		/// <c>true</c> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.
		/// </returns>
		public bool Equals(RuleSetInfo other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name) && MajorRevision == other.MajorRevision && MinorRevision == other.MinorRevision;
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see
		/// cref="T:System.Object"/>.
		/// </summary>
		/// <param name="obj">
		/// The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.
		/// </param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="T:System.Object"/> is equal to the current <see
		/// cref="T:System.Object"/>; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			var other = obj as RuleSetInfo;
			return other != null && Equals(other);
		}

		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Name != null ? Name.GetHashCode() : 0;
				hashCode = (hashCode * 397) ^ MajorRevision;
				hashCode = (hashCode * 397) ^ MinorRevision;
				return hashCode;
			}
		}

		#endregion
	}
}
