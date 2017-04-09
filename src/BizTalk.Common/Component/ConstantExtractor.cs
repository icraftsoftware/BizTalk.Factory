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
using System.Xml;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Component
{
	public class ConstantExtractor : PropertyExtractor, IEquatable<ConstantExtractor>
	{
		#region Operators

		public static bool operator ==(ConstantExtractor left, ConstantExtractor right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ConstantExtractor left, ConstantExtractor right)
		{
			return !Equals(left, right);
		}

		#endregion

		public ConstantExtractor(XmlQualifiedName propertyName, string value, ExtractionMode extractionMode = ExtractionMode.Write)
			: base(propertyName, extractionMode)
		{
			if (value.IsNullOrEmpty()) throw new ArgumentNullException("value");
			if (extractionMode == ExtractionMode.Demote)
				throw new ArgumentException(
					string.Format(
						"{0} '{1}' is not supported by {2}.",
						typeof(ExtractionMode).Name,
						ExtractionMode.Demote,
						typeof(ConstantExtractor).Name),
					"extractionMode");
			Value = value;
		}

		public ConstantExtractor(IMessageContextProperty property, string value, ExtractionMode extractionMode = ExtractionMode.Write)
			: this(property.QName, value, extractionMode) { }

		#region IEquatable<ConstantExtractor> Members

		public bool Equals(ConstantExtractor other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other) && string.Equals(Value, other.Value);
		}

		#endregion

		#region Base Class Member Overrides

		public override bool Equals(object other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			if (other.GetType() != GetType()) return false;
			return Equals((ConstantExtractor) other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (base.GetHashCode() * 397) ^ Value.GetHashCode();
			}
		}

		public override string ToString()
		{
			return string.Format("{0}[Value:{1}]", base.ToString(), Value);
		}

		protected internal override void WriteXmlCore(XmlWriter writer)
		{
			base.WriteXmlCore(writer);
			writer.WriteAttributeString("value", Value);
		}

		#endregion

		public string Value { get; private set; }
	}
}
