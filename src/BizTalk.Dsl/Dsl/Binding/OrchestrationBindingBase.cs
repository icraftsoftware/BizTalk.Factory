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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Services.Description;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Diagnostics;
using Be.Stateless.Extensions;
using Be.Stateless.Reflection;
using Microsoft.BizTalk.XLANGs.BTXEngine;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.XLANGs.Core;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	// TODO ISupportEnvironmentSensitivity should be a ModuleRef level and not OrchestrationBindingBase one
	public abstract class OrchestrationBindingBase<T>
		: IOrchestrationBinding,
			IProvideSourceFileInformation,
			ISupportEnvironmentSensitivity,
			ISupportNamingConvention,
			ISupportValidation,
			IVisitable<IApplicationBindingVisitor>
		where T : BTXService
	{
		#region Nested Type: PortBindingInfo

		private class PortBindingInfo : IOrchestrationPortBinding
		{
			internal PortBindingInfo(OrchestrationBindingBase<T> orchestrationBinding, PortInfo logicalPort)
			{
				if (orchestrationBinding == null) throw new ArgumentNullException("orchestrationBinding");
				if (logicalPort == null) throw new ArgumentNullException("logicalPort");
				_orchestrationBinding = orchestrationBinding;
				_logicalPort = logicalPort;
			}

			#region IOrchestrationPortBinding Members

			public string ActualPortName
			{
				// unbound ports, (i.e. Reflector.GetProperty() returning null) have been caught by OrchestrationBindingBase<T>'s ISupportValidation.Validate()
				get { return ((ISupportNamingConvention) Reflector.GetProperty(_orchestrationBinding, _logicalPort.Name)).Name; }
			}

			public bool IsInbound
			{
				get { return _logicalPort.Polarity == Polarity.implements; }
			}

			public string LogicalPortName
			{
				get { return _logicalPort.Name; }
			}

			#endregion

			private readonly PortInfo _logicalPort;
			private readonly OrchestrationBindingBase<T> _orchestrationBinding;
		}

		#endregion

		protected internal OrchestrationBindingBase()
		{
			_sourceFileInformationProvider = new SourceFileInformationProvider();
			_sourceFileInformationProvider.Capture();
		}

		#region IOrchestrationBinding Members

		// TODO ?? fetch <om:Property Name='AnalystComments' from <om:Element Type='ServiceDeclaration' ??
		public string Description { get; set; }

		public string Host { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public Type Type
		{
			get { return typeof(T); }
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public IOrchestrationPortBinding[] PortBindings
		{
			get { return LogicalPorts.Select(lp => new PortBindingInfo(this, lp)).Cast<IOrchestrationPortBinding>().ToArray(); }
		}

		public ServiceState State { get; set; }

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

		#region ISupportEnvironmentSensitivity Members

		bool ISupportEnvironmentDeploymentPredicate.IsDeployableForEnvironment(string environment)
		{
			return environment.IsNullOrEmpty() || IsDeployableForEnvironment(environment);
		}

		void ISupportEnvironmentOverride.ApplyEnvironmentOverrides(string environment)
		{
			if (!environment.IsNullOrEmpty() && ((ISupportEnvironmentDeploymentPredicate) this).IsDeployableForEnvironment(environment))
			{
				ApplyEnvironmentOverrides(environment);
			}
		}

		#endregion

		#region ISupportNamingConvention Members

		string ISupportNamingConvention.Name
		{
			get { return typeof(T).FullName; }
		}

		#endregion

		#region ISupportValidation Members

		void ISupportValidation.Validate()
		{
			if (Host.IsNullOrEmpty()) throw new BindingException("Orchestration's Host is not defined.", this);

			// validate that all logical ports are bound
			var unboundPorts = LogicalPorts
				.Where(p => Reflector.GetProperty(this, p.Name) == null)
				.ToArray();
			if (unboundPorts.Any())
				throw new BindingException(
					string.Format(
						"The '{0}' orchestration has unbound logical ports: '{1}'.",
						typeof(T).FullName,
						unboundPorts.Aggregate(string.Empty, (k, p) => k + "', '" + p.Name, s => s.Substring(4))));

			// Microsoft.XLANGs.BaseTypes.Polarity (i.e. implements/receive or uses/send) validation is statically enforced
			// by IReceivePort and ISendPort interfaces

			// validate that the operation flow (i.e. one or two way) of each individual logical port matches the one of
			// its associated actual port
			foreach (var logicalPort in LogicalPorts)
			{
				var operationFlow = logicalPort.Operations.First().OperationFlow;
				if (operationFlow != OperationFlow.OneWay && operationFlow != OperationFlow.RequestResponse) throw new NotSupportedException("Unexpected OperationFlow enumeration value.");
				var isLogicalPortTwoWay = operationFlow != OperationFlow.OneWay;

				var actualPort = Reflector.GetProperty(this, logicalPort.Name);
				var isActualPortTwoWay = logicalPort.Polarity == Polarity.implements
					? ((IReceivePort) actualPort).IsTwoWay
					: ((ISendPort) actualPort).IsTwoWay;

				if (isLogicalPortTwoWay && !isActualPortTwoWay)
					throw new BindingException(
						string.Format(
							"Orchestration's two-way logical port '{0}' is bound to one-way port '{1}'.",
							logicalPort.Name,
							((ISupportNamingConvention) actualPort).Name),
						this);
				if (!isLogicalPortTwoWay && isActualPortTwoWay)
					throw new BindingException(
						string.Format(
							"Orchestration's one-way logical port '{0}' is bound to two-way port '{1}'.",
							logicalPort.Name,
							((ISupportNamingConvention) actualPort).Name),
						this);
			}
		}

		#endregion

		#region IVisitable<IApplicationBindingVisitor> Members

		void IVisitable<IApplicationBindingVisitor>.Accept(IApplicationBindingVisitor visitor)
		{
			visitor.VisitOrchestration(this);
		}

		#endregion

		private IEnumerable<PortInfo> LogicalPorts
		{
			get
			{
				// see also https://msdn.microsoft.com/en-us/library/System.Web.Services.Description.OperationFlow.aspx
				return ((PortInfo[]) Reflector.GetField(typeof(T), "_portInfo"))
					// filter out direct ports
					// TODO ensure we get private types as well
					.Where(p => p.FindAttribute(typeof(DirectBindingAttribute)) == null);
			}
		}

		protected virtual bool IsDeployableForEnvironment(string environment)
		{
			return true;
		}

		protected virtual void ApplyEnvironmentOverrides(string environment) { }

		private readonly SourceFileInformationProvider _sourceFileInformationProvider;
	}
}
