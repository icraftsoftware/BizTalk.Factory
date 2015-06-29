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
using System.Linq.Expressions;
using Microsoft.RuleEngine;

namespace Be.Stateless.BizTalk.RuleEngine.Dsl
{
	internal class Antecedent
	{
		public static implicit operator LogicalExpression(Antecedent fromAntecedent)
		{
			return RuleTranslator.Translate(fromAntecedent.Expression);
		}

		public Expression<Func<bool>> Expression { get; set; }
	}
}