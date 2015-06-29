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
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.RuleEngine;

namespace Be.Stateless.BizTalk.Dsl.RuleEngine
{
	internal class ConsequentCollection : List<Consequent>
	{
		public static implicit operator ActionCollection(ConsequentCollection fromConsequentCollection)
		{
			var actions = new ActionCollection();
			foreach (var consequent in fromConsequentCollection)
			{
				actions.Add(consequent);
			}
			return actions;
		}
	}

	internal class Consequent
	{
		public static implicit operator Function(Consequent fromConsequent)
		{
			return RuleTranslator.Translate(fromConsequent.Statement);
		}

		public Consequent(Expression<Action> body)
		{
			Statement = body;
		}

		private Expression<Action> Statement { get; set; }
	}
}