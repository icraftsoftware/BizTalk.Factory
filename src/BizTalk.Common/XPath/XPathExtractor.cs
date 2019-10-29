#region Copyright & License

// Copyright © 2012 - 2019 François Chabot
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
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.Extensions;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.XPath;

namespace Be.Stateless.BizTalk.XPath
{
	/// <summary>
	/// Denotes a context property whose value will be extracted from an <see cref="IBaseMessagePart"/>'s payload while
	/// being processed by the <c>Be.Stateless.BizTalk.Component.ContextPropertyExtractorComponent</c> pipeline
	/// component.
	/// </summary>
	[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Necessary for mocking purposes.")]
	public class XPathExtractor : PropertyExtractor, IEquatable<XPathExtractor>
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

		public XPathExtractor(XmlQualifiedName propertyName, string xpathExpression, ExtractionMode extractionMode = ExtractionMode.Write)
			: base(propertyName, extractionMode)
		{
			XPathExpression = new XPathExpression(xpathExpression);
		}

		public XPathExtractor(IMessageContextProperty property, string xpathExpression, ExtractionMode extractionMode = ExtractionMode.Write)
			: this(property.QName, xpathExpression, extractionMode) { }

		#region IEquatable<XPathExtractor> Members

		public bool Equals(XPathExtractor other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other) && Equals(XPathExpression.XPath, other.XPathExpression.XPath);
		}

		#endregion

		#region Base Class Member Overrides

		public override bool Equals(object other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			if (other.GetType() != GetType()) return false;
			return Equals((XPathExtractor) other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (base.GetHashCode() * 397) ^ XPathExpression.XPath.GetHashCode();
			}
		}

		public override string ToString()
		{
			return string.Format("{0}[XPath:{1}]", base.ToString(), XPathExpression.XPath);
		}

		protected internal override void WriteXmlCore(XmlWriter writer)
		{
			base.WriteXmlCore(writer);
			writer.WriteAttributeString("xpath", XPathExpression.XPath);
		}

		#endregion

		public XPathExpression XPathExpression { get; private set; }

		public virtual void Execute(IBaseMessageContext messageContext, string originalValue, ref string newValue)
		{
			if (ExtractionMode == ExtractionMode.Write)
			{
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Writing property {0} with value {1} to context.", PropertyName, originalValue);
				messageContext.Write(PropertyName.Name, PropertyName.Namespace, originalValue);
			}
			else if (ExtractionMode == ExtractionMode.Promote)
			{
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Promoting property {0} with value {1} to context.", PropertyName, originalValue);
				messageContext.Promote(PropertyName.Name, PropertyName.Namespace, originalValue);
			}
			else if (ExtractionMode == ExtractionMode.Demote)
			{
				var @object = messageContext.Read(PropertyName.Name, PropertyName.Namespace);
				var value = @object.IfNotNull(o => o.ToString());
				if (!value.IsNullOrEmpty())
				{
					newValue = value;
					if (_logger.IsDebugEnabled) _logger.DebugFormat("Demoting property {0} with value {1} from context.", PropertyName, newValue);
				}
			}
			else
			{
				base.Execute(messageContext);
			}
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(XPathExtractor));
	}
}
