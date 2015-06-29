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

using Be.Stateless.BizTalk.Dsl.RuleEngine;
using Be.Stateless.BizTalk.RuleEngine;

namespace Be.Stateless.BizTalk.Dsl
{
	/// <summary>
	/// Allows to write bindings in terms of BizTalk <see cref="RuleSet"/>-derived artifacts.
	/// </summary>
	/// <typeparam name="T">
	/// The type of <see cref="RuleSet"/>-derived policy.
	/// </typeparam>
	public static class Policy<T> where T : RuleSet, new()
	{
		/// <summary>
		/// Returns the <see cref="PolicyName"/> of a <see cref="RuleSet"/>-derived policy <typeparamref name="T"/>.
		/// </summary>
		public static PolicyName Name
		{
			get { return new PolicyName(new T().RuleSetInfo); }
		}
	}
}
