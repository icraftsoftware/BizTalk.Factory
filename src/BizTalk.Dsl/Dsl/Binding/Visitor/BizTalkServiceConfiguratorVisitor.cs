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
using Be.Stateless.BizTalk.Explorer;
using Be.Stateless.Extensions;
using Be.Stateless.Logging;
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
	public class BizTalkServiceConfiguratorVisitor : ApplicationBindingVisitorBase
	{
		public static BizTalkServiceConfiguratorVisitor Create(string targetEnvironment)
		{
			if (targetEnvironment.IsNullOrEmpty()) throw new ArgumentNullException("targetEnvironment");
			return new BizTalkServiceConfiguratorVisitor(targetEnvironment);
		}

		private BizTalkServiceConfiguratorVisitor(string targetEnvironment) : base(targetEnvironment) { }

		#region Base Class Member Overrides

		protected internal override void VisitApplicationCore<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding)
		{
			var name = ((ISupportNamingConvention) applicationBinding).Name;
			_application = BizTalkServerGroup.Applications[name];
		}

		protected internal override void VisitOrchestrationCore(IOrchestrationBinding orchestrationBinding)
		{
			// TODO enforce orchestration status
		}

		protected internal override void VisitReceiveLocationCore<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation)
		{
			var name = ((ISupportNamingConvention) receiveLocation).Name;
			var rl = _receivePort.ReceiveLocations[name];
			if (_logger.IsDebugEnabled) _logger.DebugFormat(receiveLocation.Enabled ? "Enabling receive location '{0}'" : "Disabling receive location '{0}'", name);
			rl.Enabled = receiveLocation.Enabled;
		}

		protected internal override void VisitReceivePortCore<TNamingConvention>(IReceivePort<TNamingConvention> receivePort)
		{
			var name = ((ISupportNamingConvention) receivePort).Name;
			_receivePort = _application.ReceivePorts[name];
		}

		protected internal override void VisitSendPortCore<TNamingConvention>(ISendPort<TNamingConvention> sendPort)
		{
			var name = ((ISupportNamingConvention) sendPort).Name;
			var sp = _application.SendPorts[name];
			if (_logger.IsDebugEnabled)
			{
				if (sendPort.Status == PortStatus.Bound) _logger.DebugFormat("Unenlisting send port '{0}'", name);
				else if (sendPort.Status == PortStatus.Stopped) _logger.DebugFormat("Enlisting send port '{0}'", name);
				else _logger.DebugFormat("Starting send port '{0}'", name);
			}
			sp.Status = sendPort.Status;
		}

		#endregion

		public void Commit()
		{
			_application.ApplyChanges();
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(BizTalkServiceConfiguratorVisitor));
		private Application _application;
		private Explorer.ReceivePort _receivePort;
	}
}
