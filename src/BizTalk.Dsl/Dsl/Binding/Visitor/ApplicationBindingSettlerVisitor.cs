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

using Be.Stateless.BizTalk.Install;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	/// <summary>
	/// Base <see cref="IApplicationBindingVisitor"/> implementation that ensures that environment overrides are applied
	/// and validated before visit.
	/// </summary>
	public class ApplicationBindingSettlerVisitor : IApplicationBindingVisitor
	{
		#region IApplicationBindingVisitor Members

		public void VisitApplicationBinding<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding) where TNamingConvention : class
		{
			((ISupportEnvironmentOverride) applicationBinding).ApplyEnvironmentOverrides(Environment);
			((ISupportValidation) applicationBinding).Validate();
		}

		public void VisitOrchestration(IOrchestrationBinding orchestrationBinding)
		{
			((ISupportEnvironmentOverride) orchestrationBinding).ApplyEnvironmentOverrides(Environment);
			((ISupportValidation) orchestrationBinding).Validate();
		}

		public void VisitReceivePort<TNamingConvention>(IReceivePort<TNamingConvention> receivePort) where TNamingConvention : class
		{
			((ISupportEnvironmentOverride) receivePort).ApplyEnvironmentOverrides(Environment);
			((ISupportValidation) receivePort).Validate();
		}

		public void VisitReceiveLocation<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation) where TNamingConvention : class
		{
			((ISupportEnvironmentOverride) receiveLocation).ApplyEnvironmentOverrides(Environment);
			((ISupportValidation) receiveLocation).Validate();
		}

		public void VisitSendPort<TNamingConvention>(ISendPort<TNamingConvention> sendPort) where TNamingConvention : class
		{
			((ISupportEnvironmentOverride) sendPort).ApplyEnvironmentOverrides(Environment);
			((ISupportValidation) sendPort).Validate();
		}

		#endregion

		private string Environment
		{
			get { return BindingGenerationContext.TargetEnvironment; }
		}
	}
}
