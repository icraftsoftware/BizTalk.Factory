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

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	/// <summary>
	/// <see cref="IApplicationBindingVisitor"/> decorator that ensures that the BizTalk Server application being
	/// deployed has been settled for its target environment and that only the BizTalk Services belonging to this
	/// application are being visited by the decorated <see cref="IApplicationBindingVisitor"/> implementation.
	/// </summary>
	public class CurrentApplicationBindingVisitor : IApplicationBindingVisitor
	{
		public static IApplicationBindingVisitor Create(IApplicationBindingVisitor decoratedVisitor)
		{
			return new CurrentApplicationBindingVisitor(decoratedVisitor);
		}

		private CurrentApplicationBindingVisitor(IApplicationBindingVisitor decoratedVisitor)
		{
			if (decoratedVisitor == null) throw new ArgumentNullException("decoratedVisitor");
			_decoratedVisitor = decoratedVisitor;
		}

		#region IApplicationBindingVisitor Members

		public void VisitReferencedApplicationBinding(IVisitable<IApplicationBindingVisitor> applicationBinding)
		{
			// skip ReferencedApplicationBinding
		}

		public void VisitApplicationBinding<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding)
			where TNamingConvention : class
		{
			// ensure referenced application is settled for environment
			var applicationBindingEnvironmentSettlerVisitor = new ApplicationBindingEnvironmentSettlerVisitor();
			((IVisitable<IApplicationBindingVisitor>) applicationBinding).Accept(applicationBindingEnvironmentSettlerVisitor);
			_applicationBinding = applicationBinding;
			_decoratedVisitor.VisitApplicationBinding(applicationBinding);
		}

		public void VisitOrchestration(IOrchestrationBinding orchestrationBinding)
		{
			// skip Orchestration not belonging to this application
			if (!ReferenceEquals(orchestrationBinding.ApplicationBinding, _applicationBinding)) return;
			_decoratedVisitor.VisitOrchestration(orchestrationBinding);
		}

		public void VisitReceivePort<TNamingConvention>(IReceivePort<TNamingConvention> receivePort)
			where TNamingConvention : class
		{
			// skip ReceivePort not belonging to this application
			if (!ReferenceEquals(receivePort.ApplicationBinding, _applicationBinding)) return;
			_decoratedVisitor.VisitReceivePort(receivePort);
		}

		public void VisitReceiveLocation<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation)
			where TNamingConvention : class
		{
			// skip ReceiveLocation not belonging to this application
			if (!ReferenceEquals(receiveLocation.ReceivePort.ApplicationBinding, _applicationBinding)) return;
			_decoratedVisitor.VisitReceiveLocation(receiveLocation);
		}

		public void VisitSendPort<TNamingConvention>(ISendPort<TNamingConvention> sendPort)
			where TNamingConvention : class
		{
			// skip SendPort not belonging to this application
			if (!ReferenceEquals(sendPort.ApplicationBinding, _applicationBinding)) return;
			_decoratedVisitor.VisitSendPort(sendPort);
		}

		#endregion

		private readonly IApplicationBindingVisitor _decoratedVisitor;
		private IApplicationBinding _applicationBinding;
	}
}
