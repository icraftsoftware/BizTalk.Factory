#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
using Microsoft.RuleEngine;

namespace Be.Stateless.BizTalk.RuleEngine
{
	/// <summary>
	/// Allows to execute a <see cref="Microsoft.RuleEngine.Policy"/> form code on a set of given facts.
	/// </summary>
	public class Policy : IPolicy
	{
		/// <summary>
		/// Allows to execute a <see cref="Microsoft.RuleEngine.Policy"/> form code on a given set of facts.
		/// </summary>
		/// <param name="ruleSetInfo">
		/// The <see cref="RuleSetInfo"/> identifying the <see cref="Microsoft.RuleEngine.Policy"/> to execute.
		/// </param>
		/// <param name="facts">
		/// A collection of facts to execute the <see cref="Microsoft.RuleEngine.Policy"/> against.
		/// </param>
		public static void Execute(RuleSetInfo ruleSetInfo, params object[] facts)
		{
			_factory(ruleSetInfo).Execute(facts);
		}

		#region Mock's Factory Hook Point

		internal static Func<RuleSetInfo, IPolicy> Factory
		{
			get { return _factory; }
			set { _factory = value; }
		}

		#endregion

		private Policy(RuleSetInfo ruleSetInfo)
		{
			_ruleSetInfo = ruleSetInfo;
		}

		#region Implementation of IPolicy

		void IPolicy.Execute(params object[] facts)
		{
			using (var policy = new Microsoft.RuleEngine.Policy(_ruleSetInfo.Name, _ruleSetInfo.MajorRevision, _ruleSetInfo.MinorRevision))
			{
				var interceptor = RuleSetTrackingLogger.Create(policy.RuleSetInfo);
				if (interceptor == null) policy.Execute(facts);
				else policy.Execute(facts, interceptor);
			}
		}

		#endregion

		private static Func<RuleSetInfo, IPolicy> _factory = ruleSetInfo => new Policy(ruleSetInfo);

		private readonly RuleSetInfo _ruleSetInfo;
	}
}
