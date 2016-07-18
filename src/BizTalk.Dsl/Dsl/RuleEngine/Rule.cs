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
using System.Linq;
using System.Linq.Expressions;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.RuleEngine
{
	public interface IRule : IFluentInterface
	{
		IRule If(Expression<Func<bool>> antecedent);

		IRule Then(params Expression<Action>[] consequents);
	}

	public class Rule : IRule
	{
		#region Operators

		public static implicit operator Microsoft.RuleEngine.Rule(Rule fromRule)
		{
			var toRule = new Microsoft.RuleEngine.Rule(fromRule.Name, fromRule.Antecedent, fromRule.Consequent);
			return toRule;
		}

		#endregion

		private Rule()
		{
			Antecedent = new Antecedent();
			Consequent = new ConsequentCollection();
		}

		public Rule(string name) : this()
		{
			if (name.IsNullOrEmpty()) throw new ArgumentNullException("name");
			Name = name;
		}

		#region IRule Members

		public IRule If(Expression<Func<bool>> antecedent)
		{
			Antecedent.Expression = antecedent;
			return this;
		}

		public IRule Then(params Expression<Action>[] consequents)
		{
			Consequent.AddRange(consequents.Select(c => new Consequent(c)));
			return this;
		}

		#endregion

		public string Name { set; get; }

		private Antecedent Antecedent { get; set; }

		private ConsequentCollection Consequent { get; set; }
	}
}
