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
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Adapter.Sftp;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;
using Microsoft.Win32;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class AdapterBase : IAdapter, IAdapterBindingSerializerFactory
	{
		protected static ProtocolType GetProtocolTypeFromConfigurationClassId(Guid configurationClassId)
		{
			// [HKCR\Wow6432Node\CLSID\<configurationClassId>\BizTalk]
			using (var classes32 = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry32))
			using (var btsKey = classes32.SafeOpenSubKey(string.Format(@"CLSID\{0:B}\BizTalk", configurationClassId)))
			{
				var capabilities = (int) btsKey.GetValue("Constraints");
				var name = (string) btsKey.GetValue("TransportType");
				return new ProtocolType {
					Capabilities = capabilities,
					ConfigurationClsid = configurationClassId.ToString(),
					Name = name
				};
			}
		}

		protected AdapterBase(ProtocolType protocolType)
		{
			if (protocolType == null) throw new ArgumentNullException("protocolType");
			_protocolType = protocolType;
		}

		#region IAdapter Members

		string IAdapter.Address
		{
			get { return GetAddress(); }
		}

		ProtocolType IAdapter.ProtocolType
		{
			get { return _protocolType; }
		}

		string IAdapter.PublicAddress
		{
			get { return GetPublicAddress(); }
		}

		void IAdapter.Load(IPropertyBag propertyBag)
		{
			throw new NotSupportedException();
		}

		void IAdapter.Save(IPropertyBag propertyBag)
		{
			Save(propertyBag);
		}

		void ISupportEnvironmentOverride.ApplyEnvironmentOverrides(string environment)
		{
			if (!environment.IsNullOrEmpty()) ApplyEnvironmentOverrides(environment);
		}

		void ISupportValidation.Validate()
		{
			Validate();
		}

		#endregion

		#region IAdapterBindingSerializerFactory Members

		IDslSerializer IAdapterBindingSerializerFactory.GetAdapterBindingSerializer()
		{
			return new AdapterBindingSerializer(this);
		}

		#endregion

		protected abstract string GetAddress();

		protected virtual string GetPublicAddress()
		{
			return null;
		}

		protected virtual void ApplyEnvironmentOverrides(string environment) { }

		protected abstract void Save(IPropertyBag propertyBag);

		protected abstract void Validate();

		[SuppressMessage("ReSharper", "RedundantCaseLabel")]
		protected TimeSpan BuildTimeSpan(int quantity, PollingIntervalUnit unit)
		{
			switch (unit)
			{
				case PollingIntervalUnit.Seconds:
					return TimeSpan.FromSeconds(quantity);
				case PollingIntervalUnit.Minutes:
					return TimeSpan.FromMinutes(quantity);
				case PollingIntervalUnit.Hours:
					return TimeSpan.FromHours(quantity);
				case PollingIntervalUnit.Days:
				default:
					return TimeSpan.FromDays(quantity);
			}
		}

		protected void UnbuildTimeSpan(TimeSpan interval, Action<int, PollingIntervalUnit> quantityAndUnitSetter)
		{
			if (interval.Seconds != 0 || interval == TimeSpan.Zero)
			{
				quantityAndUnitSetter((int) interval.TotalSeconds, PollingIntervalUnit.Seconds);
			}
			else if (interval.Minutes != 0)
			{
				quantityAndUnitSetter((int) interval.TotalMinutes, PollingIntervalUnit.Minutes);
			}
			else if (interval.Hours != 0)
			{
				quantityAndUnitSetter((int) interval.TotalHours, PollingIntervalUnit.Hours);
			}
			else // if (interval.Days != 0)
			{
				quantityAndUnitSetter((int) interval.TotalDays, PollingIntervalUnit.Days);
			}
		}

		private readonly ProtocolType _protocolType;
	}
}
