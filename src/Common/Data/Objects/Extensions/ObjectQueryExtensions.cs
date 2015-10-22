#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq.Expressions;

namespace Be.Stateless.Data.Objects.Extensions
{
	/// <summary>
	/// Entity Framework Include() with a Func%lt;&gt;.
	/// </summary>
	/// <remarks>
	/// <code>context.Include("Products.Order_Details")</code> can now be written as <![CDATA[
	/// context.Categories.Include(ca => ca.Products.Include(p => p.Order_Details).Include(od => od.Orders))
	/// ]]>
	/// </remarks>
	/// <seealso href="http://msmvps.com/blogs/matthieu/archive/2008/06/06/entity-framework-include-with-func-next.aspx" />
	public static class ObjectQueryExtensions
	{
		public static ObjectQuery<T1> Include<T1, T2>(this ObjectQuery<T1> query, Expression<Func<T1, T2>> selector)
			where T1 : EntityObject, IEntityWithRelationships
			where T2 : class
		{
			return query.Include(SelectorToPath(selector.Body));
		}

		public static T2 Include<T1, T2>(this EntityCollection<T1> query, Expression<Func<T1, T2>> selector)
			where T1 : EntityObject, IEntityWithRelationships
			where T2 : class
		{
			// only there to provide intellisense and intended to be translated
			throw new NotSupportedException();
		}

		private static string SelectorToPath(Expression selector)
		{
			switch (selector.NodeType)
			{
				case ExpressionType.MemberAccess:
					return ((MemberExpression) selector).Member.Name;
				case ExpressionType.Call:
					var method = (MethodCallExpression) selector;
					return SelectorToPath(method.Arguments[0]) + "." + SelectorToPath(method.Arguments[1]);
				case ExpressionType.Quote:
					return SelectorToPath(((LambdaExpression) ((UnaryExpression) selector).Operand).Body);
				default:
					throw new InvalidOperationException();
			}
		}
	}
}