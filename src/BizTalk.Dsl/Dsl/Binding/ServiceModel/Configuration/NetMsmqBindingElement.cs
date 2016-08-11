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
using System.ServiceModel;
using System.ServiceModel.Configuration;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration
{
	public class NetMsmqBindingElement : StandardBindingElement, IBindingElementDecorator, ISupportEnvironmentOverride
	{
		public NetMsmqBindingElement()
		{
			_decorated = new System.ServiceModel.Configuration.NetMsmqBindingElement("netMsmqBinding");
			RetryPolicy = NetMsmqRetryPolicy.Default;
		}

		#region IBindingElementDecorator Members

		public IBindingConfigurationElement DecoratedBindingElement
		{
			get
			{
				// StandardBindingElement properties
				_decorated.CloseTimeout = CloseTimeout;
				_decorated.OpenTimeout = OpenTimeout;
				_decorated.ReceiveTimeout = ReceiveTimeout;
				_decorated.SendTimeout = SendTimeout;
				// retry policy properties
				_decorated.MaxRetryCycles = RetryPolicy.MaxRetryCycles;
				_decorated.ReceiveRetryCount = RetryPolicy.ReceiveRetryCount;
				_decorated.RetryCycleDelay = RetryPolicy.RetryCycleDelay;
				_decorated.TimeToLive = RetryPolicy.TimeToLive;
				return _decorated;
			}
		}

		#endregion

		#region ISupportEnvironmentOverride Members

		public void ApplyEnvironmentOverrides(string environment)
		{
			(RetryPolicy as ISupportEnvironmentOverride).IfNotNull(p => { p.ApplyEnvironmentOverrides(environment); });
		}

		#endregion

		#region Base Class Member Overrides

		protected override Type BindingElementType
		{
			get
			{
				throw new NotSupportedException(
					string.Format(
						"Only the actual {0} binding element supports this operation but not its decorator.",
						_decorated.GetType().FullName));
			}
		}

		protected override void OnApplyConfiguration(System.ServiceModel.Channels.Binding binding)
		{
			throw new NotSupportedException(
				string.Format(
					"Only the actual {0} binding element supports this operation but not its decorator.",
					_decorated.GetType().FullName));
		}

		#endregion

		public Uri CustomDeadLetterQueue
		{
			get { return _decorated.CustomDeadLetterQueue; }
			set { _decorated.CustomDeadLetterQueue = value; }
		}

		public DeadLetterQueue DeadLetterQueue
		{
			get { return _decorated.DeadLetterQueue; }
			set { _decorated.DeadLetterQueue = value; }
		}

		public bool Durable
		{
			get { return _decorated.Durable; }
			set { _decorated.Durable = value; }
		}

		public bool ExactlyOnce
		{
			get { return _decorated.ExactlyOnce; }
			set { _decorated.ExactlyOnce = value; }
		}

		public long MaxBufferPoolSize
		{
			get { return _decorated.MaxBufferPoolSize; }
			set { _decorated.MaxBufferPoolSize = value; }
		}

		public long MaxReceivedMessageSize
		{
			get { return _decorated.MaxReceivedMessageSize; }
			set { _decorated.MaxReceivedMessageSize = value; }
		}

		public QueueTransferProtocol QueueTransferProtocol
		{
			get { return _decorated.QueueTransferProtocol; }
			set { _decorated.QueueTransferProtocol = value; }
		}

		public XmlDictionaryReaderQuotasElement ReaderQuotas
		{
			get { return _decorated.ReaderQuotas; }
		}

		public bool ReceiveContextEnabled
		{
			get { return _decorated.ReceiveContextEnabled; }
			set { _decorated.ReceiveContextEnabled = value; }
		}

		public ReceiveErrorHandling ReceiveErrorHandling
		{
			get { return _decorated.ReceiveErrorHandling; }
			set { _decorated.ReceiveErrorHandling = value; }
		}

		public NetMsmqRetryPolicy RetryPolicy { get; set; }

		public NetMsmqSecurityElement Security
		{
			get { return _decorated.Security; }
		}

		public bool UseActiveDirectory
		{
			get { return _decorated.UseActiveDirectory; }
			set { _decorated.UseActiveDirectory = value; }
		}

		public bool UseMsmqTracing
		{
			get { return _decorated.UseMsmqTracing; }
			set { _decorated.UseMsmqTracing = value; }
		}

		public bool UseSourceJournal
		{
			get { return _decorated.UseSourceJournal; }
			set { _decorated.UseSourceJournal = value; }
		}

		public TimeSpan ValidityDuration
		{
			get { return _decorated.ValidityDuration; }
			set { _decorated.ValidityDuration = value; }
		}

		private readonly System.ServiceModel.Configuration.NetMsmqBindingElement _decorated;
	}
}
