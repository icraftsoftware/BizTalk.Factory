#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	public class SendPortNamingConvention<TNamingConvention> :
		ISendPortNamingConvention<TNamingConvention>,
		ILocationMessageName<TNamingConvention>,
		ILocationMessageFormat<TNamingConvention>
		where TNamingConvention : new()
	{
		public SendPortNamingConvention()
		{
			_convention = new TNamingConvention();
		}

		#region ILocationMessageFormat<TNamingConvention> Members

		MessageFormat<TNamingConvention> ILocationMessageFormat<TNamingConvention>.FormattedAs
		{
			get { return new MessageFormat<TNamingConvention>(_convention); }
		}

		#endregion

		#region ILocationMessageName<TNamingConvention> Members

		ILocationMessageFormat<TNamingConvention> ILocationMessageName<TNamingConvention>.About<T>(T messageName)
		{
			((IMessageNameMemento<T>) _convention).MessageName = messageName;
			return this;
		}

		#endregion

		#region ISendPortNamingConvention<TNamingConvention> Members

		public ILocationMessageName<TNamingConvention> Towards<T>(T party)
		{
			((IPartyMemento<T>) _convention).Party = party;
			return this;
		}

		#endregion

		private readonly TNamingConvention _convention;
	}
}
