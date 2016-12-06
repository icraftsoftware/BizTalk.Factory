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
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	/// <summary>
	/// Base <see cref="IApplicationBindingVisitor"/> implementation that ensures that environment overrides are applied
	/// and validated before visit.
	/// </summary>
	public class ApplicationBindingSettlerVisitor : IApplicationBindingVisitor
	{
		public ApplicationBindingSettlerVisitor(string targetEnvironment)
		{
			if (targetEnvironment.IsNullOrEmpty()) throw new ArgumentNullException("targetEnvironment");
			Environment = targetEnvironment;
		}

		#region IApplicationBindingVisitor Members

		void IApplicationBindingVisitor.VisitApplicationBinding<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding)
		{
			((ISupportEnvironmentOverride) applicationBinding).ApplyEnvironmentOverrides(Environment);
			((ISupportValidation) applicationBinding).Validate();
			VisitApplication(applicationBinding);
		}

		void IApplicationBindingVisitor.VisitOrchestration(IOrchestrationBinding orchestrationBinding)
		{
			((ISupportEnvironmentOverride) orchestrationBinding).ApplyEnvironmentOverrides(Environment);
			((ISupportValidation) orchestrationBinding).Validate();
			VisitOrchestration(orchestrationBinding);
		}

		void IApplicationBindingVisitor.VisitReceivePort<TNamingConvention>(IReceivePort<TNamingConvention> receivePort)
		{
			((ISupportEnvironmentOverride) receivePort).ApplyEnvironmentOverrides(Environment);
			((ISupportValidation) receivePort).Validate();
			VisitReceivePort(receivePort);
		}

		void IApplicationBindingVisitor.VisitReceiveLocation<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation)
		{
			((ISupportEnvironmentOverride) receiveLocation).ApplyEnvironmentOverrides(Environment);
			((ISupportValidation) receiveLocation).Validate();
			VisitReceiveLocation(receiveLocation);
		}

		void IApplicationBindingVisitor.VisitSendPort<TNamingConvention>(ISendPort<TNamingConvention> sendPort)
		{
			((ISupportEnvironmentOverride) sendPort).ApplyEnvironmentOverrides(Environment);
			((ISupportValidation) sendPort).Validate();
			VisitSendPort(sendPort);
		}

		#endregion

		protected string Environment { get; private set; }

		protected internal virtual void VisitApplication<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding)
			where TNamingConvention : class { }

		protected internal virtual void VisitOrchestration(IOrchestrationBinding orchestrationBinding) { }

		protected internal virtual void VisitReceivePort<TNamingConvention>(IReceivePort<TNamingConvention> receivePort)
			where TNamingConvention : class { }

		protected internal virtual void VisitReceiveLocation<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation)
			where TNamingConvention : class { }

		protected internal virtual void VisitSendPort<TNamingConvention>(ISendPort<TNamingConvention> sendPort)
			where TNamingConvention : class { }
	}
}
