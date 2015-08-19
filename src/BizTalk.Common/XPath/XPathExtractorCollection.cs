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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Be.Stateless.Linq;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.XPath
{
	public enum ExtractorPrecedence
	{
		Schema,
		SchemaOnly,
		Pipeline,
		PipelineOnly
	}

	/// <summary>
	/// Collection of <see cref="XPathExtractor"/>s to be used to extract values of context properties from an <see
	/// cref="IBaseMessagePart"/>'s payload while being processed through the pipelines.
	/// </summary>
	/// <remarks>
	/// <see cref="XPathExtractor"/> instances must not be stored in a <see cref="Hashtable"/> or any container that
	/// relies on its <see cref="XPathExtractor.GetHashCode"/> as its hash value is dependent on the collection state.
	/// </remarks>
	public class XPathExtractorCollection : List<XPathExtractor>, IEquatable<XPathExtractorCollection>
	{
		#region Operators

		public static bool operator ==(XPathExtractorCollection left, XPathExtractorCollection right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(XPathExtractorCollection left, XPathExtractorCollection right)
		{
			return !Equals(left, right);
		}

		#endregion

		public XPathExtractorCollection() { }

		public XPathExtractorCollection(IEnumerable<XPathExtractor> collection) : base(collection) { }

		#region IEquatable<XPathExtractorCollection> Members

		public bool Equals(XPathExtractorCollection other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return this.SequenceEqual(other);
		}

		#endregion

		#region Base Class Member Overrides

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((XPathExtractorCollection) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return this.Aggregate(0, (hashCode, xpe) => (hashCode * 397) ^ xpe.GetHashCode());
			}
		}

		#endregion

		/// <summary>
		/// Produces the set union of two <see cref="XPathExtractorCollection"/> sequences by giving precedence to <see
		/// cref="XPathExtractor"/> elements from <c>this</c> sequence.
		/// </summary>
		/// <param name="second">
		/// An <see cref="XPathExtractorCollection"/> whose distinct elements form the second set for the union, and whose
		/// elements are given lower precedence.
		/// </param>
		/// <returns>
		/// An <see cref="IEnumerable{T}"/> of <see cref="XPathExtractor"/> that contains the elements from both input
		/// sequences, excluding any duplicate elements in general, and elements from the <paramref name="second"/>
		/// sequence that also exists in <c>this</c> sequence.
		/// </returns>
		public IEnumerable<XPathExtractor> Union(IEnumerable<XPathExtractor> second)
		{
			// merge configured and schema-annotated extractors so as to give precedence to schema annotations.
			// Union enumerates first and second in that order and yields each element that has not already been
			// yielded, see http://msdn.microsoft.com/en-us/library/bb358407(v=VS.90)
			return this.Union(
				second,
				new LambdaComparer<XPathExtractor>((lxpe, rxpe) => lxpe.PropertyName == rxpe.PropertyName));
		}

		public static readonly XPathExtractorCollection Empty = new XPathExtractorCollection();
	}
}
