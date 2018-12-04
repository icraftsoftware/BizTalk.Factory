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

using System.Linq.Expressions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Subscription
{
	public class FilterNormalizer : ExpressionVisitor
	{
		public static Expression Normalize(Expression expression)
		{
			var normalizer = new FilterNormalizer();
			return normalizer.Visit(expression);
		}

		#region Base Class Member Overrides

		protected override Expression VisitBinary(BinaryExpression node)
		{
			return node.NodeType == ExpressionType.AndAlso
				? DistributeConjunctionOverDisjunction(node.Left, node.Right)
				: base.VisitBinary(node);
		}

		#endregion

		private Expression DistributeConjunctionOverDisjunction(Expression left, Expression right)
		{
			var binary = left as BinaryExpression;
			if (binary != null && binary.NodeType == ExpressionType.OrElse)
			{
				return Expression.MakeBinary(
					ExpressionType.OrElse,
					DistributeConjunctionOverDisjunction(Normalize(binary.Left), Normalize(right)),
					DistributeConjunctionOverDisjunction(Normalize(binary.Right), Normalize(right)));
			}

			binary = right as BinaryExpression;
			if (binary != null && binary.NodeType == ExpressionType.OrElse)
			{
				return Expression.MakeBinary(
					ExpressionType.OrElse,
					DistributeConjunctionOverDisjunction(Normalize(left), Normalize(binary.Left)),
					DistributeConjunctionOverDisjunction(Normalize(left), Normalize(binary.Right)));
			}

			return Expression.MakeBinary(ExpressionType.AndAlso, Normalize(left), Normalize(right));
		}
	}
}
