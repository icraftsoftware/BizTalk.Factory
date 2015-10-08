#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Diagnostics;
using Be.Stateless.BizTalk.Dsl.Binding.Subscription;
using Be.Stateless.BizTalk.Dsl.Pipeline;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	// see also SendPort, http://msdn.microsoft.com/en-us/library/aa560374.aspx
	public abstract class SendPortBase<TNamingConvention>
		: ISendPort<TNamingConvention>,
			ISupportEnvironmentSensitivity,
			ISupportNamingConvention,
			ISupportValidation,
			IProvideSourceFileInformation,
			IVisitable<IApplicationBindingVisitor>
		where TNamingConvention : class
	{
		protected internal SendPortBase()
		{
			_sourceFileInformationProvider = new SourceFileInformationProvider();
			_sourceFileInformationProvider.Capture();
			Priority = Priority.Normal;
			Transport = new SendPortTransport();
		}

		protected internal SendPortBase(Action<ISendPort<TNamingConvention>> sendPortConfigurator) : this()
		{
			sendPortConfigurator(this);
			((ISupportValidation) this).Validate();
		}

		#region IProvideSourceFileInformation Members

		string IProvideSourceFileInformation.Name
		{
			get { return _sourceFileInformationProvider.Name; }
		}

		int IProvideSourceFileInformation.Column
		{
			get { return _sourceFileInformationProvider.Column; }
		}

		int IProvideSourceFileInformation.Line
		{
			get { return _sourceFileInformationProvider.Line; }
		}

		#endregion

		#region ISendPort<TNamingConvention> Members

		public IApplicationBinding<TNamingConvention> ApplicationBinding { get; internal set; }

		public SendPortTransport BackupTransport
		{
			get { return _backupTransport ?? (_backupTransport = new SendPortTransport(this)); }
		}

		public string Description { get; set; }

		public Filter Filter { get; set; }

		public bool IsTwoWay
		{
			get { return ReceivePipeline != null; }
		}

		public bool OrderedDelivery { get; set; }

		public Priority Priority { get; set; }

		public TNamingConvention Name { get; set; }

		public ReceivePipeline ReceivePipeline { get; set; }

		public SendPipeline SendPipeline { get; set; }

		public bool StopSendingOnOrderedDeliveryFailure { get; set; }

		public SendPortTransport Transport { get; private set; }

		#endregion

		#region ISupportEnvironmentSensitivity Members

		void ISupportEnvironmentOverride.ApplyEnvironmentOverrides(string environment)
		{
			if (!environment.IsNullOrEmpty() && ((ISupportEnvironmentDeploymentPredicate) this).IsDeployableForEnvironment(environment))
			{
				ApplyEnvironmentOverrides(environment);
				// ReSharper disable once SuspiciousTypeConversion.Global
				(Filter as ISupportEnvironmentOverride).IfNotNull(f => f.ApplyEnvironmentOverrides(environment));
				((ISupportEnvironmentOverride) ReceivePipeline).IfNotNull(rp => rp.ApplyEnvironmentOverrides(environment));
				((ISupportEnvironmentOverride) SendPipeline).ApplyEnvironmentOverrides(environment);
				((ISupportEnvironmentOverride) Transport).ApplyEnvironmentOverrides(environment);
				((ISupportEnvironmentOverride) _backupTransport).IfNotNull(bt => bt.ApplyEnvironmentOverrides(environment));
			}
		}

		bool ISupportEnvironmentDeploymentPredicate.IsDeployableForEnvironment(string environment)
		{
			return environment.IsNullOrEmpty() || IsDeployableForEnvironment(environment);
		}

		#endregion

		#region ISupportNamingConvention Members

		string ISupportNamingConvention.Name
		{
			get { return NamingConventionThunk.ComputeSendPortName(this); }
		}

		#endregion

		#region ISupportValidation Members

		void ISupportValidation.Validate()
		{
			if (Name == null) throw new BindingException("Send Port's Name is not defined.", this);
			if (SendPipeline == null) throw new BindingException("Send Port's Send Pipeline is not defined.", this);
			((ISupportValidation) Transport).Validate();
			_backupTransport.IfNotNull(bt => ((ISupportValidation) bt).Validate());
		}

		#endregion

		#region IVisitable<IApplicationBindingVisitor> Members

		public void Accept(IApplicationBindingVisitor visitor)
		{
			visitor.VisitSendPort(this);
		}

		#endregion

		protected virtual void ApplyEnvironmentOverrides(string environment) { }

		protected virtual bool IsDeployableForEnvironment(string environment)
		{
			return true;
		}

		private readonly SourceFileInformationProvider _sourceFileInformationProvider;
		private SendPortTransport _backupTransport;
	}
}
