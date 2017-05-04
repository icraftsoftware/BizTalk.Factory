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

using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Streaming.Extensions;
using Be.Stateless.BizTalk.Tracking.Messaging;
using Be.Stateless.Extensions;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// <see cref="ActivityTracker"/> that specifically tracks the messaging activities involved in the release process
	/// of a batch message.
	/// </summary>
	public class BatchTracker : ActivityTracker
	{
		#region Base Class Member Overrides

		public override IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			message = base.Execute(pipelineContext, message);

			var markableForwardOnlyEventingReadStream = message.BodyPart.WrapOriginalDataStream(
				originalStream => originalStream.AsMarkable(),
				pipelineContext.ResourceTracker);
			var probe = (IProbeBatchContentStream) markableForwardOnlyEventingReadStream.Probe();

			var batchDescriptor = probe.BatchDescriptor;
			if (batchDescriptor != null && !batchDescriptor.EnvelopeSpecName.IsNullOrEmpty())
			{
				if (_logger.IsInfoEnabled)
					_logger.DebugFormat(
						"Tracking batch release process for envelope '{0}' and partition '{1}'.",
						batchDescriptor.EnvelopeSpecName,
						batchDescriptor.Partition ?? "[null]");
				var batchTrackingContext = probe.BatchTrackingContext;
				BatchReleaseProcessActivityTracker.Create(pipelineContext, message).TrackActivity(batchTrackingContext);

				message.SetProperty(TrackingProperties.Value1, batchDescriptor.EnvelopeSpecName);
				message.SetProperty(TrackingProperties.Value2, batchDescriptor.EnvironmentTag);
				message.SetProperty(TrackingProperties.Value3, batchDescriptor.Partition);
			}
			else
			{
				if (_logger.IsInfoEnabled) _logger.Debug("Tracking of batch release process is skipped for non batch message.");
			}

			markableForwardOnlyEventingReadStream.StopMarking();

			return message;
		}

		#endregion

		private static readonly ILog _logger = LogManager.GetLogger(typeof(BatchTracker));
	}
}
