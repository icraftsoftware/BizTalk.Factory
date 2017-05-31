#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	/// <summary>
	/// Provides in-process self-hosting for a single endpoint of some <typeparamref name="TService"/> service class
	/// exposing a <typeparamref name="TChannel"/> service contract.
	/// </summary>
	/// <typeparam name="TService">
	/// The service class that implements the <typeparamref name="TChannel"/> service contract.
	/// </typeparam>
	/// <typeparam name="TChannel">
	/// The shape of the channel to be used to connect to the service's endpoint.
	/// </typeparam>
	/// <remarks>
	/// <para>
	/// The only purpose of the <see cref="SimpleServiceHost{TService,TChannel}"/> is to support WCF service hosting for
	/// the sake of unit testing. Even though one can always mock a service contract, sometimes nothing compares to the
	/// real thing when one has to do WCF plumbing.
	/// </para>
	/// <para>
	/// Notice that <see cref="SimpleServiceHost{TService,TChannel}"/> will systematically try to register a metadata
	/// exchange endpoint, in accordance to the address' scheme, at the relative <c>mex</c> address. <see
	/// cref="SimpleServiceHost{TService,TChannel}"/> might consequently fail to start if there is no <see
	/// cref="Binding"/> that supports the exchange of metadata over the <see cref="Endpoint"/>'s address scheme; see
	/// also <see cref="MetadataExchangeBindings"/>.
	/// </para>
	/// </remarks>
	/// <seealso cref="SimpleServiceHostActivatorAttribute"/>
	/// <seealso cref="MetadataExchangeBindings"/>
	public class SimpleServiceHost<TService, TChannel> : ISimpleServiceHost
		where TService : TChannel
		where TChannel : class
	{
		public static ServiceEndpoint Endpoint
		{
			get
			{
				if (_host == null) throw new InvalidOperationException(string.Format("{0} service host has not started yet.", typeof(SimpleServiceHost<TService, TChannel>).Name));
				// ReSharper disable once PossibleNullReferenceException
				return _host.Description.Endpoints.Find(typeof(TChannel));
			}
		}

		#region ISimpleServiceHost Members

		public void Dispose()
		{
			Close();
		}

		public void Open(System.ServiceModel.Channels.Binding binding, Uri address)
		{
			if (binding == null) throw new ArgumentNullException("binding");
			if (address == null) throw new ArgumentNullException("address");
			if (_host != null) throw new InvalidOperationException(string.Format("{0} service host has already started.", typeof(SimpleServiceHost<TService, TChannel>).Name));

			_host = new ServiceHost(typeof(TService), address);
			// ReSharper disable once PossibleNullReferenceException
			var debugBehavior = _host.Description.Behaviors.Find<ServiceDebugBehavior>();
			debugBehavior.IncludeExceptionDetailInFaults = true;
			_host.AddServiceEndpoint(typeof(TChannel), binding, string.Empty);
			AddMexEndpoint(address.Scheme);
			_host.Open();
		}

		public void Close()
		{
			if (_host != null) _host.Close();
			_host = null;
		}

		#endregion

		#region Mex Endpoint Helpers

		private static void AddMexEndpoint(string scheme)
		{
			if (scheme == null) throw new ArgumentNullException("scheme");
			// ReSharper disable once PossibleNullReferenceException
			var debugBehavior = _host.Description.Behaviors.Find<ServiceDebugBehavior>();
			var metaDataBehavior = _host.Description.Behaviors.Find<ServiceMetadataBehavior>();
			if (metaDataBehavior == null)
			{
				metaDataBehavior = new ServiceMetadataBehavior();
				_host.Description.Behaviors.Add(metaDataBehavior);
			}
			debugBehavior.HttpHelpPageEnabled = metaDataBehavior.HttpGetEnabled = scheme.IndexOf("http", StringComparison.OrdinalIgnoreCase) >= 0;
			_host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, CreateMexBindingForScheme(scheme), "mex");
		}

		private static System.ServiceModel.Channels.Binding CreateMexBindingForScheme(string scheme)
		{
			if (scheme == null) throw new ArgumentNullException("scheme");
			if (string.Compare(scheme, "http", StringComparison.OrdinalIgnoreCase) == 0) return MetadataExchangeBindings.CreateMexHttpBinding();
			if (string.Compare(scheme, "https", StringComparison.OrdinalIgnoreCase) == 0) return MetadataExchangeBindings.CreateMexHttpsBinding();
			if (string.Compare(scheme, "net.pipe", StringComparison.OrdinalIgnoreCase) == 0) return MetadataExchangeBindings.CreateMexNamedPipeBinding();
			if (string.Compare(scheme, "net.tcp", StringComparison.OrdinalIgnoreCase) == 0) return MetadataExchangeBindings.CreateMexTcpBinding();
			throw new ArgumentException(string.Format("MEX for scheme {0} is not supported.", scheme));
		}

		#endregion

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static ServiceHost _host;
	}
}
