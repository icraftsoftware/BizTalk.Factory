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
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Diagnostics;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public abstract class ApplicationBindingBase<TNamingConvention>
		: IApplicationBinding<TNamingConvention>,
			IBindingSerializerFactory,
			ISupportEnvironmentOverride,
			ISupportNamingConvention,
			ISupportValidation,
			IProvideSourceFileInformation,
			IVisitable<IApplicationBindingVisitor>
		where TNamingConvention : class
	{
		protected internal ApplicationBindingBase()
		{
			_referencedApplications = new ReferencedApplicationBindingCollection();
			_receivePorts = new ReceivePortCollection<TNamingConvention>(this);
			_sendPorts = new SendPortCollection<TNamingConvention>(this);
			_orchestrations = new OrchestrationBindingCollection<TNamingConvention>(this);
			_sourceFileInformationProvider = new SourceFileInformationProvider();
			_sourceFileInformationProvider.Capture();
			Timestamp = DateTime.Now;
		}

		protected ApplicationBindingBase(Action<IApplicationBinding<TNamingConvention>> applicationBindingConfigurator) : this()
		{
			applicationBindingConfigurator(this);
			((ISupportValidation) this).Validate();
		}

		#region IApplicationBinding<TNamingConvention> Members

		public string Description { get; set; }

		public TNamingConvention Name { get; set; }

		public IOrchestrationBindingCollection Orchestrations
		{
			get { return _orchestrations; }
		}

		public IReferencedApplicationBindingCollection ReferencedApplications
		{
			get { return _referencedApplications; }
		}

		public IReceivePortCollection<TNamingConvention> ReceivePorts
		{
			get { return _receivePorts; }
		}

		public ISendPortCollection<TNamingConvention> SendPorts
		{
			get { return _sendPorts; }
		}

		public DateTime Timestamp { get; internal set; }

		#endregion

		#region IBindingSerializerFactory Members

		IDslSerializer IBindingSerializerFactory.GetBindingSerializer(string environment)
		{
			return new ApplicationBindingSerializer(this, environment);
		}

		#endregion

		#region IProvideSourceFileInformation Members

		int IProvideSourceFileInformation.Line
		{
			get { return _sourceFileInformationProvider.Line; }
		}

		int IProvideSourceFileInformation.Column
		{
			get { return _sourceFileInformationProvider.Column; }
		}

		string IProvideSourceFileInformation.Name
		{
			get { return _sourceFileInformationProvider.Name; }
		}

		#endregion

		#region ISupportEnvironmentOverride Members

		void ISupportEnvironmentOverride.ApplyEnvironmentOverrides(string environment)
		{
			if (!environment.IsNullOrEmpty()) ApplyEnvironmentOverrides(environment);
		}

		#endregion

		#region ISupportNamingConvention Members

		string ISupportNamingConvention.Name
		{
			get { return NamingConventionThunk.ComputeApplicationName(this); }
		}

		#endregion

		#region ISupportValidation Members

		void ISupportValidation.Validate()
		{
			if (Name == null) throw new BindingException("Application's Name is not defined.", this);
		}

		#endregion

		#region IVisitable<IApplicationBindingVisitor> Members

		void IVisitable<IApplicationBindingVisitor>.Accept(IApplicationBindingVisitor visitor)
		{
			visitor.VisitApplicationBinding(this);
			((IVisitable<IApplicationBindingVisitor>) _receivePorts).Accept(visitor);
			((IVisitable<IApplicationBindingVisitor>) _sendPorts).Accept(visitor);
			((IVisitable<IApplicationBindingVisitor>) _orchestrations).Accept(visitor);
		}

		#endregion

		protected virtual void ApplyEnvironmentOverrides(string environment) { }

		private readonly OrchestrationBindingCollection<TNamingConvention> _orchestrations;
		private readonly ReceivePortCollection<TNamingConvention> _receivePorts;
		private readonly ReferencedApplicationBindingCollection _referencedApplications;
		private readonly SendPortCollection<TNamingConvention> _sendPorts;
		private readonly SourceFileInformationProvider _sourceFileInformationProvider;
	}
}
