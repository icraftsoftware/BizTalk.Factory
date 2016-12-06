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

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	internal static class NamingConventionThunk
	{
		internal static string ComputeApplicationName<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding)
			where TNamingConvention : class
		{
			return ComputeName(
				applicationBinding,
				"Application",
				convention => convention.ComputeApplicationName(applicationBinding));
		}

		internal static string ComputeReceivePortName<TNamingConvention>(IReceivePort<TNamingConvention> receivePort)
			where TNamingConvention : class
		{
			return ComputeName(
				receivePort,
				"Receive Port",
				convention => convention.ComputeReceivePortName(receivePort));
		}

		internal static string ComputeReceiveLocationName<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation)
			where TNamingConvention : class
		{
			return ComputeName(
				receiveLocation,
				"Receive Location",
				convention => convention.ComputeReceiveLocationName(receiveLocation));
		}

		internal static string ComputeSendPortName<TNamingConvention>(ISendPort<TNamingConvention> sendPort)
			where TNamingConvention : class
		{
			return ComputeName(
				sendPort,
				"Send Port",
				convention => convention.ComputeSendPortName(sendPort));
		}

		private static string ComputeName<TNamingConvention>(
			IObjectBinding<TNamingConvention> bindingObject,
			string bindingObjectType,
			Func<INamingConvention<TNamingConvention>, string> computingDelegate)
			where TNamingConvention : class
		{
			try
			{
				var convention = bindingObject.Name as INamingConvention<TNamingConvention>;
				return convention != null ? computingDelegate(convention) : (string) (object) bindingObject.Name;
			}
			catch (Exception exception)
			{
				throw new NamingConventionException(string.Format("{0}'s naming convention is invalid.", bindingObjectType), exception);
			}
		}
	}
}
