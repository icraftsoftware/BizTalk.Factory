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

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention.Simple
{
	public class NamingConvention : NamingConventionBase<NamingConvention, string, string>, INamingConvention<NamingConvention>
	{
		#region Operators

		public static implicit operator string(NamingConvention convention)
		{
			throw new NotSupportedException("In order to support Be.Stateless.BizTalk.Dsl.Binding.Subscription.FilterTranslator.");
		}

		#endregion

		#region INamingConvention<NamingConvention> Members

		string INamingConvention<NamingConvention>.ComputeApplicationName(IApplicationBinding<NamingConvention> application)
		{
			return base.ComputeApplicationName(application);
		}

		string INamingConvention<NamingConvention>.ComputeReceivePortName(IReceivePort<NamingConvention> receivePort)
		{
			return base.ComputeReceivePortName(receivePort);
		}

		string INamingConvention<NamingConvention>.ComputeReceiveLocationName(IReceiveLocation<NamingConvention> receiveLocation)
		{
			return base.ComputeReceiveLocationName(receiveLocation);
		}

		string INamingConvention<NamingConvention>.ComputeSendPortName(ISendPort<NamingConvention> sendPort)
		{
			return base.ComputeSendPortName(sendPort);
		}

		#endregion
	}
}
