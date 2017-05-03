#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Tracking.Messaging
{
	public class ActivityFactory : IActivityFactory, IBatchProcessActivityFactory
	{
		public ActivityFactory(IPipelineContext pipelineContext)
		{
			if (pipelineContext == null) throw new ArgumentNullException("pipelineContext");
			_pipelineContext = pipelineContext;
		}

		#region IActivityFactory Members

		public Process CreateProcess(IBaseMessage message, string name)
		{
			return new Process(_pipelineContext, message, name);
		}

		public Process FindProcess(TrackingContext trackingContext)
		{
			return new ProcessReference(trackingContext.ProcessActivityId, _pipelineContext.GetEventStream());
		}

		public MessagingStep CreateMessagingStep(IBaseMessage message)
		{
			return new MessagingStep(_pipelineContext, message);
		}

		public MessagingStep FindMessagingStep(TrackingContext trackingContext)
		{
			return new MessagingStepReference(trackingContext.MessagingStepActivityId, _pipelineContext.GetEventStream());
		}

		#endregion

		#region IBatchProcessActivityFactory Members

		BatchReleaseProcess IBatchProcessActivityFactory.CreateProcess(IBaseMessage message, string name)
		{
			return new BatchReleaseProcess(_pipelineContext, message, name);
		}

		BatchReleaseProcess IBatchProcessActivityFactory.FindProcess(string processActivityId)
		{
			return new BatchReleaseProcessReference(processActivityId, _pipelineContext.GetEventStream());
		}

		#endregion

		private readonly IPipelineContext _pipelineContext;
	}
}
