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
using System.ServiceModel.Channels;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	/// <summary>
	/// Activates the <see cref="SimpleServiceHost{TService,TChannel}"/>, i.e. the in-process host with a single WCF
	/// service endpoint meant to facilitate the NUnit-based unit testing of WCF service contracts.
	/// </summary>
	/// <remarks>
	/// <see cref="SimpleServiceHostActivatorAttribute"/> ensures that the <see
	/// cref="SimpleServiceHost{TService,TChannel}"/> with a single WCF service endpoint listening and started before any
	/// NUnit-test depending on it runs. Making use of this attribute therefore assumes and requires that unit tests are
	/// written on the basis of the NUnit testing framework.
	/// </remarks>
	/// <seealso cref="SimpleServiceHost{TService,TChannel}"/>
	/// <seealso cref="ITestAction"/>
	/// <seealso cref="TestActionAttribute"/>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
	public class SimpleServiceHostActivatorAttribute : TestActionAttribute
	{
		/// <summary>
		/// Creates a <see cref="SimpleServiceHostActivatorAttribute"/> that will ensure that the <see
		/// cref="SimpleServiceHost{TService,TChannel}"/> with a single endpoint listening at <paramref name="address"/>
		/// over <paramref name="binding"/> is started before any NUnit-test depending on it runs.
		/// </summary>
		/// <param name="service">
		/// The <see cref="Type"/> of the <see cref="SimpleServiceHost{TService,TChannel}"/> host to activate.
		/// </param>
		/// <param name="binding">
		/// The <see cref="Type"/> of the <see cref="Binding"/> over which to expose the single endpoint.
		/// </param>
		/// <param name="address">
		/// The address at which to expose the single endpoint.
		/// </param>
		/// <remarks>
		/// Don't forget to <see href="http://msdn.microsoft.com/en-us/library/ms733768.aspx">configure HTTP</see>:
		/// <code><![CDATA[netsh http add urlacl url=http://+:8001/calculator user=$env:USERDOMAIN\$env:USERNAME]]>!</code>
		/// </remarks>
		/// <seealso href="http://msdn.microsoft.com/en-us/library/ms733768.aspx"/>
		public SimpleServiceHostActivatorAttribute(Type service, Type binding, string address)
		{
			if (service == null) throw new ArgumentNullException("service");
			if (binding == null) throw new ArgumentNullException("binding");
			if (address == null) throw new ArgumentNullException("address");
			_host = (ISimpleServiceHost) Activator.CreateInstance(service);
			_binding = (System.ServiceModel.Channels.Binding) Activator.CreateInstance(binding);
			_uri = new Uri(address);
		}

		/// <summary>
		/// Creates a <see cref="SimpleServiceHostActivatorAttribute"/> that will ensure that the <see
		/// cref="SimpleServiceHost{TService,TChannel}"/> with a single endpoint listening at <paramref name="address"/>
		/// over <paramref name="binding"/> is started before any NUnit-test depending on it runs.
		/// </summary>
		/// <param name="service">
		/// The <see cref="Type"/> of the <see cref="SimpleServiceHost{TService,TChannel}"/> host to activate.
		/// </param>
		/// <param name="binding">
		/// The <see cref="Type"/> of the <see cref="Binding"/> over which to expose the single endpoint.
		/// </param>
		/// <param name="address">
		/// The address at which to expose the single endpoint.
		/// </param>
		/// <param name="sendTimeout">
		/// The interval of time in seconds provided for a write operation to complete before the transport raises an
		/// exception.
		/// </param>
		/// <remarks>
		/// Don't forget to <see href="http://msdn.microsoft.com/en-us/library/ms733768.aspx">configure HTTP</see>:
		/// <code><![CDATA[netsh http add urlacl url=http://+:8001/calculator user=$env:USERDOMAIN\$env:USERNAME]]>!</code>
		/// </remarks>
		/// <seealso href="http://msdn.microsoft.com/en-us/library/ms733768.aspx"/>
		public SimpleServiceHostActivatorAttribute(Type service, Type binding, string address, int sendTimeout) : this(service, binding, address)
		{
			_binding.SendTimeout = TimeSpan.FromSeconds(sendTimeout);
		}

		#region Base Class Member Overrides

		public override void AfterTest(ITest test)
		{
			_host.Close();
		}

		public override void BeforeTest(ITest test)
		{
			_host.Open(_binding, _uri);
		}

		public override ActionTargets Targets
		{
			get { return ActionTargets.Suite; }
		}

		#endregion

		private readonly System.ServiceModel.Channels.Binding _binding;
		private readonly ISimpleServiceHost _host;
		private readonly Uri _uri;
	}
}
