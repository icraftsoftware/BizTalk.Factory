﻿#region Copyright & License

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
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public abstract class TransportBase<T> : ISupportEnvironmentOverride, ISupportValidation
		where T : class, IAdapter, ISupportEnvironmentOverride, ISupportValidation
	{
		#region Nested Type: UnknownAdapter

		protected internal abstract class UnknownAdapter : IAdapter
		{
			#region IAdapter Members

			public string Address
			{
				get { throw new NotSupportedException(); }
			}

			public ProtocolType ProtocolType
			{
				get { throw new NotSupportedException(); }
			}

			public string PublicAddress
			{
				get { throw new NotSupportedException(); }
			}

			public void ApplyEnvironmentOverrides(string environment) { }

			public void Load(IPropertyBag propertyBag)
			{
				throw new NotSupportedException();
			}

			public void Save(IPropertyBag propertyBag)
			{
				throw new NotSupportedException();
			}

			public void Validate()
			{
				throw new NotSupportedException(
					string.Format(
						"{0} is not a valid transport adapter.",
						GetType().Name));
			}

			#endregion
		}

		#endregion

		#region ISupportEnvironmentOverride Members

		void ISupportEnvironmentOverride.ApplyEnvironmentOverrides(string environment)
		{
			if (environment.IsNullOrEmpty()) return;
			ApplyEnvironmentOverrides(environment);
			Adapter.IfNotNull(a => a.ApplyEnvironmentOverrides(environment));
		}

		#endregion

		#region ISupportValidation Members

		void ISupportValidation.Validate()
		{
			if (Host.IsNullOrEmpty()) throw new BindingException("Transport's Host is not defined.");
			if (Adapter == null || Adapter is UnknownAdapter) throw new BindingException("Transport's Adapter is not defined.");
			Adapter.Validate();
		}

		#endregion

		public T Adapter { get; set; }

		public string Host { get; set; }

		protected abstract void ApplyEnvironmentOverrides(string environment);
	}
}
