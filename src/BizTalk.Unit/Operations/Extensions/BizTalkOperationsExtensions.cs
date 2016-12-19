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

using System.Diagnostics;
using System.Linq;
using Be.Stateless.Linq.Extensions;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Operations;

namespace Be.Stateless.BizTalk.Operations.Extensions
{
	public static class BizTalkOperationsExtensions
	{
		public static MessageBoxServiceInstance[] GetRunningOrSuspendedServiceInstances(this BizTalkOperations bizTalkOperations)
		{
			return bizTalkOperations
				.GetServiceInstances().OfType<MessageBoxServiceInstance>()
				.Where(i => (i.InstanceStatus & (InstanceStatus.RunningAll | InstanceStatus.SuspendedAll)) != InstanceStatus.None)
				.ToArray();
		}

		public static void TerminateUncompletedBizTalkServiceInstances(this BizTalkOperations bizTalkOperations)
		{
			bizTalkOperations
				.GetRunningOrSuspendedServiceInstances()
				.Select(i => new { ServiceInstance = i, CompletionStatus = bizTalkOperations.TerminateInstance(i.ID) })
				.Where(sd => sd.CompletionStatus != CompletionStatus.Succeeded)
				.Each(
					(idx, sd) => {
						Trace.TraceWarning("Could not terminate the BizTalk service instance with ID {0}", sd.ServiceInstance.ID);
						_logger.WarnFormat(
							"[{0,2}] Could not terminate the BizTalk service instance class: {1}\r\n     ServiceType: {2}\r\n     Creation Time: {3}\r\n     Status: {4}\r\n     Error: {5}\r\n",
							idx,
							sd.ServiceInstance.Class,
							sd.ServiceInstance.ServiceType,
							sd.ServiceInstance.CreationTime,
							sd.ServiceInstance.InstanceStatus,
							sd.ServiceInstance.ErrorDescription);
					}
				);
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(BizTalkOperationsExtensions));
	}
}
