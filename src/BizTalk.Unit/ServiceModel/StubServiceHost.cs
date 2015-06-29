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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub.Language;

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	public class StubServiceHost : ServiceHost
	{
		static StubServiceHost()
		{
			AppDomain.CurrentDomain.DomainUnload += (sender, args) => {
				if (_defaultInstance.State == CommunicationState.Opened)
				{
					try
					{
						_defaultInstance.Close();
					}
					catch
					{
						_defaultInstance.Abort();
						throw;
					}
				}
			};
		}

		public static Binding DefaultBinding
		{
			// ReSharper disable PossibleNullReferenceException
			get { return _defaultInstance.Description.Endpoints.Find(typeof(IStubService)).Binding; }
			// ReSharper restore PossibleNullReferenceException
		}

		public static EndpointAddress DefaultEndpointAddress
		{
			// ReSharper disable PossibleNullReferenceException
			get { return _defaultInstance.Description.Endpoints.Find(typeof(IStubService)).Address; }
			// ReSharper restore PossibleNullReferenceException
		}

		public static StubServiceHost DefaultInstance
		{
			get { return _defaultInstance; }
		}

		public static ISetupOperation<ISolicitResponse> DefaultService
		{
			get { return DefaultInstance.Service; }
		}

		public static ISetupOperation<TContract> FindDefaultService<TContract>() where TContract : class
		{
			return DefaultInstance.FindService<TContract>();
		}

		private StubServiceHost() : base(new StubService(), new Uri("http://localhost:8000/"))
		{
			var debugBehavior = Description.Behaviors.Find<ServiceDebugBehavior>();
			if (debugBehavior != null)
			{
				debugBehavior.IncludeExceptionDetailInFaults = true;
			}
			else
			{
				Description.Behaviors.Add(new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });
			}
			AddServiceEndpoint(typeof(IStubService), new BasicHttpBinding(), "stubservice");
		}

		public ISetupOperation<ISolicitResponse> Service
		{
			get { return ((StubService) SingletonInstance).FindSetupRecorder<ISolicitResponse>(); }
		}

		public ISetupOperation<TContract> FindService<TContract>() where TContract : class
		{
			return ((StubService) SingletonInstance).FindSetupRecorder<TContract>();
		}

		public void Recycle()
		{
			try
			{
				Close();
			}
			catch
			{
				Abort();
				throw;
			}
			finally
			{
				_defaultInstance = new StubServiceHost();
			}
		}

		private static StubServiceHost _defaultInstance = new StubServiceHost();
	}
}
