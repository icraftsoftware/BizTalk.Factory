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

using Be.Stateless.BizTalk.Dsl.Binding.Adapter;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	public abstract class FileAdapterFolderVisitorBase : IApplicationBindingVisitor
	{
		#region IApplicationBindingVisitor Members

		public void VisitReferencedApplicationBinding(IVisitable<IApplicationBindingVisitor> applicationBinding) { }

		public void VisitApplicationBinding<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding)
			where TNamingConvention : class { }

		public void VisitOrchestration(IOrchestrationBinding orchestrationBinding) { }

		public void VisitReceivePort<TNamingConvention>(IReceivePort<TNamingConvention> receivePort)
			where TNamingConvention : class { }

		public void VisitReceiveLocation<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation)
			where TNamingConvention : class
		{
			var fileAdapter = receiveLocation.Transport.Adapter as FileAdapter.Inbound;
			if (fileAdapter != null) VisitDirectory(fileAdapter.ReceiveFolder);
		}

		public void VisitSendPort<TNamingConvention>(ISendPort<TNamingConvention> sendPort)
			where TNamingConvention : class
		{
			var fileAdapter = sendPort.Transport.Adapter as FileAdapter.Outbound;
			if (fileAdapter != null) VisitDirectory(fileAdapter.DestinationFolder);
		}

		#endregion

		protected abstract void VisitDirectory(string path);
	}
}
