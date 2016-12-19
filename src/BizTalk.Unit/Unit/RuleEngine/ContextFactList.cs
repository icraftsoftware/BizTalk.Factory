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

using Microsoft.BizTalk.Message.Interop;
using Moq;

namespace Be.Stateless.BizTalk.Unit.RuleEngine
{
	public class ContextFactList
	{
		internal ContextFactList(TestRuleEngine engine)
		{
			_engine = engine;
		}

		public IBaseMessageContext Context
		{
			get { return _context.Object; }
		}

		public void Clear()
		{
			_engine.Facts = new ContextFactList(_engine);
		}

		public ContextFactList Assert<T>(ContextFact<T> fact)
		{
			fact.Assert(_context);
			return this;
		}

		public ContextFactList Verify<T>(ContextFact<T> fact)
		{
			fact.Verify(_context);
			return this;
		}

		private readonly Mock<IBaseMessageContext> _context = new Mock<IBaseMessageContext>();
		private readonly TestRuleEngine _engine;
	}
}
