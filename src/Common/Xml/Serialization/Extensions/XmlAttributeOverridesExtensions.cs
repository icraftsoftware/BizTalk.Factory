#region Copyright & License

// Copyright © 2012 - 2014 François Chabot, Yves Dierick
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
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace Be.Stateless.Xml.Serialization.Extensions
{
	/// <summary>
	/// <see cref="XmlAttributeOverrides"/>'s typed-extension methods.
	/// </summary>
	/// <seealso href="http://stackoverflow.com/questions/9377414/excluding-some-properties-during-serialization-without-changing-the-original-cla"/>
	public static class XmlAttributeOverridesExtensions
	{
		public static void Add<T>(this XmlAttributeOverrides overrides, XmlAttributes attributes)
		{
			overrides.Add(typeof(T), attributes);
		}

		public static void Add<T>(this XmlAttributeOverrides overrides, Expression<Func<T, object>> propertySelector, XmlAttributes attributes)
		{
			overrides.Add(typeof(T), propertySelector.GetPropertyName(), attributes);
		}

		public static void Ignore<T>(this XmlAttributeOverrides overrides, Expression<Func<T, object>> propertySelector)
		{
			overrides.Add(typeof(T), propertySelector.GetPropertyName(), _ignore);
		}

		private static string GetPropertyName(this Expression propertySelector)
		{
			switch (propertySelector.NodeType)
			{
				case ExpressionType.Lambda:
					var lambdaExpression = (LambdaExpression) propertySelector;
					return GetPropertyName(lambdaExpression.Body);

				case ExpressionType.Convert:
				case ExpressionType.Quote:
					var unaryExpression = (UnaryExpression) propertySelector;
					return GetPropertyName(unaryExpression.Operand);

				case ExpressionType.MemberAccess:
					var memberExpression = (MemberExpression) propertySelector;
					var propertyInfo = memberExpression.Member;
					if (memberExpression.Expression is ParameterExpression) return propertyInfo.Name;
					// we've got a nested property (e.g. MyType.SomeProperty.SomeNestedProperty)
					return GetPropertyName(memberExpression.Expression) + "." + propertyInfo.Name;

				default:
					throw new ArgumentException(string.Format("Cannot translate expression because '{0}' is not a member expression.", propertySelector));
			}
		}

		private static readonly XmlAttributes _ignore = new XmlAttributes { XmlIgnore = true };
	}
}
