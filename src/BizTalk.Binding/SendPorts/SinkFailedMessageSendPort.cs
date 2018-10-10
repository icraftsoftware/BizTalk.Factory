#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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

using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextBuilders.Send;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.Simple;
using Be.Stateless.BizTalk.Dsl.Binding.Subscription;
using Be.Stateless.BizTalk.EnvironmentSettings;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.MicroPipelines;
using RetryPolicy = Be.Stateless.BizTalk.Dsl.Binding.Convention.RetryPolicy;

namespace Be.Stateless.BizTalk
{
	public class SinkFailedMessageSendPort : SendPort<NamingConvention>
	{
		public SinkFailedMessageSendPort()
		{
			Name = SendPortName.Towards("Sink").About("FailedMessage").FormattedAs.None;
			State = ServiceState.Started;
			SendPipeline = new SendPipeline<PassThruTransmit>(
				pipeline => {
					pipeline.PreAssembler<MicroPipelineComponent>(
						pc => {
							pc.Components = new IMicroPipelineComponent[] {
								new ContextBuilder { BuilderType = typeof(FailedProcessResolver) },
								new ActivityTracker(),
								new MessageConsumer()
							};
						});
				});
			Transport.Adapter = new FileAdapter.Outbound(
				a => {
					a.DestinationFolder = @"C:\Files\Drops\BizTalk.Factory\Out";
					a.FileName = "Failed_%MessageID%.xml";
				});
			Transport.Host = CommonSettings.TransmitHost;
			Transport.RetryPolicy = RetryPolicy.RealTime;
			Filter = new Filter(() => ErrorReportProperties.ErrorType == "FailedMessage");
		}
	}
}
