﻿#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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
using System.ServiceModel.Configuration;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class WcfOneWayAdapterBase<TAddress, TBinding, TConfig>
		: WcfAdapterBase<TAddress, TBinding, TConfig>,
			IAdapterConfigTimeouts
		where TBinding : StandardBindingElement, new()
		where TConfig : AdapterConfig,
			IAdapterConfigAddress,
			Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigIdentity,
			IAdapterConfigTimeouts,
			new()
	{
		protected WcfOneWayAdapterBase(ProtocolType protocolType) : base(protocolType) { }

		#region IAdapterConfigTimeouts Members

		/// <summary>
		/// Gets or sets the interval of time after which the close method, invoked by a communication object, times out.
		/// </summary>
		/// <remarks>
		/// The interval of time provided for a connection to close before the transport raises an exception. The default
		/// value is 1 minute.
		/// </remarks>
		/// <returns>
		/// The <see cref="T:Timespan"/> that specifies the interval of time to wait for the close method to time out.
		/// </returns>
		/// <exception cref="T:ArgumentOutOfRangeException">
		/// The value is less than zero or too large.
		/// </exception>
		public TimeSpan CloseTimeout
		{
			get { return _adapterConfig.CloseTimeout; }
			set { _adapterConfig.CloseTimeout = value; }
		}

		/// <summary>
		/// Gets or sets the interval of time after which the open method, invoked by a communication object, times out.
		/// </summary>
		/// <remarks>
		/// The interval of time provided for a connection to open before the transport raises an exception. The default
		/// value is 1 minute.
		/// </remarks>
		/// <returns>
		/// The <see cref="T:Timespan"/> that specifies the interval of time to wait for the open method to time out.
		/// </returns>
		/// <exception cref="T:ArgumentOutOfRangeException">
		/// The value is less than zero or too large.
		/// </exception>
		public TimeSpan OpenTimeout
		{
			get { return _adapterConfig.OpenTimeout; }
			set { _adapterConfig.OpenTimeout = value; }
		}

		/// <summary>
		/// Gets or sets the interval of time after which the send method, invoked by a communication object, times out.
		/// </summary>
		/// <remarks>
		/// The interval of time provided for a write operation to complete before the transport raises an exception. The
		/// default value is 1 minute.
		/// </remarks>
		/// <returns>
		/// The <see cref="T:Timespan"/> that specifies the interval of time to wait for the send method to time out.
		/// </returns>
		/// <exception cref="T:ArgumentOutOfRangeException">
		/// The value is less than zero or too large.
		/// </exception>
		public TimeSpan SendTimeout
		{
			get { return _adapterConfig.SendTimeout; }
			set { _adapterConfig.SendTimeout = value; }
		}

		#endregion
	}
}
