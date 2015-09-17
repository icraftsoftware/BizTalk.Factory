#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.XPath;

namespace Be.Stateless.BizTalk.XPath
{
	public enum ExtractionMode
	{
		Clear = 1,
		Demote,
		Ignore,
		Promote,
		Write = 0
	}

	public enum ExtractorPrecedence
	{
		Schema,
		SchemaOnly,
		Pipeline,
		PipelineOnly
	}

	/// <summary>
	/// Denotes a context property whose value will be extracted from an <see cref="IBaseMessagePart"/>'s payload while
	/// being processed by the <c>Be.Stateless.BizTalk.Component.ContextPropertyExtractorComponent</c> pipeline
	/// component.
	/// </summary>
	public class XPathExtractor : IEquatable<XPathExtractor>
	{
		#region Operators

		public static bool operator ==(XPathExtractor left, XPathExtractor right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(XPathExtractor left, XPathExtractor right)
		{
			return !Equals(left, right);
		}

		#endregion

		public XPathExtractor(XmlQualifiedName propertyName, string xpathExpression, ExtractionMode extractionMode)
		{
			PropertyName = propertyName;
			XPathExpression = new XPathExpression(xpathExpression);
			ExtractionMode = extractionMode;
		}

		#region IEquatable<XPathExtractor> Members

		public bool Equals(XPathExtractor other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return ExtractionMode == other.ExtractionMode
				&& PropertyName.Equals(other.PropertyName)
				&& XPathExpression.XPath.Equals(other.XPathExpression.XPath);
		}

		#endregion

		#region Base Class Member Overrides

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((XPathExtractor) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (int) ExtractionMode;
				hashCode = (hashCode * 397) ^ PropertyName.GetHashCode();
				hashCode = (hashCode * 397) ^ XPathExpression.XPath.GetHashCode();
				return hashCode;
			}
		}

		#endregion

		public ExtractionMode ExtractionMode { get; private set; }

		public XmlQualifiedName PropertyName { get; private set; }

		public XPathExpression XPathExpression { get; private set; }
	}
}
