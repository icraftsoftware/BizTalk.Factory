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

using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Explorer;
using Be.Stateless.Logging;
using OrchestrationStatus = Microsoft.BizTalk.ExplorerOM.OrchestrationStatus;
using PortStatus = Microsoft.BizTalk.ExplorerOM.PortStatus;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	/// <summary>
	/// <see cref="IApplicationBindingVisitor"/> implementation that manage BizTalk Server services' state.
	/// </summary>
	/// <remarks>
	/// <see cref="IApplicationBindingVisitor"/> implementation that either
	/// <list type="bullet">
	/// <item>
	/// Enable or disable a receive location according to its DSL-based binding;
	/// </item>
	/// <item>
	/// Start, stop, enlist or unenlist a send port according to its DSL-based binding;
	/// </item>
	/// <item>
	/// Start, stop, enlist or unenlist an orchestration according to its DSL-based binding.
	/// </item>
	/// </list>
	/// </remarks>
	public class BizTalkServiceConfiguratorVisitor : IApplicationBindingVisitor
	{
		public static BizTalkServiceConfiguratorVisitor Create()
		{
			return new BizTalkServiceConfiguratorVisitor();
		}

		private BizTalkServiceConfiguratorVisitor()
		{
			_applicationBindingEnvironmentSettlerVisitor = new ApplicationBindingEnvironmentSettlerVisitor();
		}

		#region IApplicationBindingVisitor Members

		public void VisitReferencedApplicationBinding(IVisitable<IApplicationBindingVisitor> applicationBinding)
		{
			// do not configure BizTalk Service for referenced applications
		}

		public void VisitApplicationBinding<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding)
			where TNamingConvention : class
		{
			_applicationBindingEnvironmentSettlerVisitor.VisitApplicationBinding(applicationBinding);
			var name = ((ISupportNamingConvention) applicationBinding).Name;
			_application = BizTalkServerGroup.Applications[name];
		}

		public void VisitOrchestration(IOrchestrationBinding orchestrationBinding)
		{
			_applicationBindingEnvironmentSettlerVisitor.VisitOrchestration(orchestrationBinding);
			var name = orchestrationBinding.Type.FullName;
			var orchestration = _application.Orchestrations[name];
			if (_logger.IsDebugEnabled)
			{
				if (orchestrationBinding.State == ServiceState.Unenlisted) _logger.DebugFormat("Unenlisting orchestration '{0}'", name);
				else if (orchestrationBinding.State == ServiceState.Enlisted) _logger.DebugFormat("Enlisting or stopping orchestration '{0}'", name);
				else _logger.DebugFormat("Starting orchestration '{0}'", name);
			}
			orchestration.Status = (OrchestrationStatus) orchestrationBinding.State;
		}

		public void VisitReceivePort<TNamingConvention>(IReceivePort<TNamingConvention> receivePort)
			where TNamingConvention : class
		{
			_applicationBindingEnvironmentSettlerVisitor.VisitReceivePort(receivePort);
			var name = ((ISupportNamingConvention) receivePort).Name;
			_receivePort = _application.ReceivePorts[name];
		}

		public void VisitReceiveLocation<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation)
			where TNamingConvention : class
		{
			_applicationBindingEnvironmentSettlerVisitor.VisitReceiveLocation(receiveLocation);
			var name = ((ISupportNamingConvention) receiveLocation).Name;
			var rl = _receivePort.ReceiveLocations[name];
			if (_logger.IsDebugEnabled) _logger.DebugFormat(receiveLocation.Enabled ? "Enabling receive location '{0}'" : "Disabling receive location '{0}'", name);
			rl.Enabled = receiveLocation.Enabled;
		}

		public void VisitSendPort<TNamingConvention>(ISendPort<TNamingConvention> sendPort)
			where TNamingConvention : class
		{
			_applicationBindingEnvironmentSettlerVisitor.VisitSendPort(sendPort);
			var name = ((ISupportNamingConvention) sendPort).Name;
			var sp = _application.SendPorts[name];
			if (_logger.IsDebugEnabled)
			{
				if (sendPort.State == ServiceState.Indefinite) _logger.DebugFormat("Leaving send port '{0}''s state as it is.", name);
				else if (sendPort.State == ServiceState.Unenlisted) _logger.DebugFormat("Unenlisting send port '{0}'.", name);
				else if (sendPort.State == ServiceState.Enlisted) _logger.DebugFormat("Enlisting or stopping send port '{0}'.", name);
				else _logger.DebugFormat("Starting send port '{0}'.", name);
			}
			if (sendPort.State != ServiceState.Indefinite) sp.Status = (PortStatus) sendPort.State;
		}

		#endregion

		public void Commit()
		{
			_application.ApplyChanges();
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(BizTalkServiceConfiguratorVisitor));
		private readonly ApplicationBindingEnvironmentSettlerVisitor _applicationBindingEnvironmentSettlerVisitor;
		private Application _application;
		private Explorer.ReceivePort _receivePort;
	}
}
