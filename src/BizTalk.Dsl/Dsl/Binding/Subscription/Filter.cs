#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.BizTalk.B2B.PartnerManagement;

namespace Be.Stateless.BizTalk.Dsl.Binding.Subscription
{
	public class Filter
	{
		#region Operators

		public static Filter operator &(Filter left, Filter right)
		{
			// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/conditional-and-operator
			return new Filter(
				Expression.Lambda<Func<bool>>(
					Expression.MakeBinary(
						ExpressionType.AndAlso,
						left._predicate.Body,
						right._predicate.Body)));
		}

		public static Filter operator |(Filter left, Filter right)
		{
			// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/conditional-or-operator
			return new Filter(
				Expression.Lambda<Func<bool>>(
					Expression.MakeBinary(
						ExpressionType.OrElse,
						left._predicate.Body,
						right._predicate.Body)));
		}

		public static bool operator false(Filter filter)
		{
			// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#user-defined-conditional-logical-operators
			// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/true-operator
			// return opposite of what expected to prevent short-circuit evaluation as actual intent is to return a new expression tree
			return false;
		}

		public static implicit operator string(Filter filter)
		{
			return TranslateFilterGroup(FilterTranslator.Translate(filter._predicate));
		}

		public static bool operator true(Filter filter)
		{
			// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#user-defined-conditional-logical-operators
			// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/false-operator
			// return opposite of what expected to prevent short-circuit evaluation as actual intent is to return a new expression tree
			return false;
		}

		#endregion

		private static string TranslateFilterGroup(FilterPredicate filterPredicate)
		{
			if (!filterPredicate.Groups.Any()) return null;

			using (var writer = new StringWriter())
			using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Encoding = new UTF8Encoding(false), Indent = false, OmitXmlDeclaration = true }))
			{
				var serializer = new XmlSerializer(typeof(FilterPredicate));
				var absorbXsdAndXsiXmlns = new XmlSerializerNamespaces(new[] { new XmlQualifiedName(string.Empty, string.Empty) });
				serializer.Serialize(xmlWriter, filterPredicate, absorbXsdAndXsiXmlns);
				return writer.ToString();
			}
		}

		public Filter(Expression<Func<bool>> predicate)
		{
			if (predicate == null) throw new ArgumentNullException("predicate");
			_predicate = predicate;
		}

		#region Base Class Member Overrides

		public override string ToString()
		{
			return this;
		}

		#endregion

		private readonly Expression<Func<bool>> _predicate;
	}
}
