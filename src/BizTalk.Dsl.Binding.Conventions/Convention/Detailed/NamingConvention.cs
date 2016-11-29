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

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention.Detailed
{
	public class NamingConvention<TParty, TMessageName> : NamingConventionBase<NamingConvention<TParty, TMessageName>, TParty, TMessageName>,
		INamingConvention<NamingConvention<TParty, TMessageName>>
	{
		#region Operators

		public static implicit operator string(NamingConvention<TParty, TMessageName> convention)
		{
			throw new NotSupportedException("In order to support Be.Stateless.BizTalk.Dsl.Binding.Subscription.FilterTranslator.");
		}

		#endregion

		#region INamingConvention<NamingConvention<TParty,TMessageName>> Members

		string INamingConvention<NamingConvention<TParty, TMessageName>>.ComputeApplicationName(IApplicationBinding<NamingConvention<TParty, TMessageName>> application)
		{
			return base.ComputeApplicationName(application);
		}

		string INamingConvention<NamingConvention<TParty, TMessageName>>.ComputeReceivePortName(IReceivePort<NamingConvention<TParty, TMessageName>> receivePort)
		{
			return base.ComputeReceivePortName(receivePort);
		}

		string INamingConvention<NamingConvention<TParty, TMessageName>>.ComputeReceiveLocationName(IReceiveLocation<NamingConvention<TParty, TMessageName>> receiveLocation)
		{
			return base.ComputeReceiveLocationName(receiveLocation);
		}

		string INamingConvention<NamingConvention<TParty, TMessageName>>.ComputeSendPortName(ISendPort<NamingConvention<TParty, TMessageName>> sendPort)
		{
			return base.ComputeSendPortName(sendPort);
		}

		#endregion
	}
}
