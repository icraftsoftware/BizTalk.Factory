#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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
using Microsoft.BizTalk.Diagnostics;
using Microsoft.BizTalk.Tracing;
using log4net.Appender;
using log4net.Core;

namespace Be.Stateless.Logging.Appender
{
	/// <summary>
	/// This appender internally uses a <see cref="TraceProvider"/> to forward the logging events.
	/// The <see cref="TraceProvider"/> is instantiated using the custom component
	/// GUID (6A223DEA-F806-4523-BAD0-312DCC4F63F9). In order to capture the logging events using Event
	/// Tracing for Windows (ETW), some configuration (beyond the classical log4net configuration) is needed.
	/// These configuration tasks are executed using the Starttrace.cmd and StopTrace.cmd scripts that can be found
	/// in utils\Tracing.
	/// There is a mapping between the <see cref="log4net.Core.Level"/> of the logging event and the <see cref="TraceLevel"/> used
	/// to output the message:
	/// <list type="">
	/// <item><see cref="Level.Error"/> or above => <see cref="TraceLevel.Error"/></item>
	/// <item><see cref="Level.Warn"/> or above => <see cref="TraceLevel.Warning"/></item>
	/// <item><see cref="Level.Info"/> or above => <see cref="TraceLevel.Info"/></item>
	/// <item><see cref="Level.Debug"/> or above => <see cref="TraceLevel.Messages"/></item>
	/// <item>All levels lower than <see cref="Level.Debug"/> => <see cref="TraceLevel.Tracking"/></item>
	/// </list>
	/// </summary>
	public class BizTalkTracingAppender : AppenderSkeleton
	{
		protected override void Append(LoggingEvent loggingEvent)
		{
			if (!_traceProvider.IsEnabled) return;

			Level level = loggingEvent.Level;
			if (level >= Level.Error)
			{
				if ((_traceProvider.Flags & TraceLevel.Error) != 0) TraceMessage(TraceLevel.Error, loggingEvent);
			}
			else if (level >= Level.Warn)
			{
				if ((_traceProvider.Flags & TraceLevel.Warning) != 0) TraceMessage(TraceLevel.Warning, loggingEvent);
			}
			else if (level >= Level.Info)
			{
				if ((_traceProvider.Flags & TraceLevel.Info) != 0) TraceMessage(TraceLevel.Info, loggingEvent);
			}
			else if (level >= Level.Debug)
			{
				if ((_traceProvider.Flags & TraceLevel.Messages) != 0) TraceMessage(TraceLevel.Messages, loggingEvent);
			}
			else
			{
				if ((_traceProvider.Flags & TraceLevel.Tracking) != 0) TraceMessage(TraceLevel.Tracking, loggingEvent);
			}
		}

		private void TraceMessage(uint flags, LoggingEvent loggingEvent)
		{
			_traceProvider.TraceMessage(
				flags, Layout == null ? loggingEvent.MessageObject : RenderLoggingEvent(loggingEvent));
		}

		public override void ActivateOptions()
		{
			base.ActivateOptions();
			_traceProvider = new TraceProvider(Name, CustomTraceComponentGuid);
		}

		public static readonly Guid CustomTraceComponentGuid = new Guid("6A223DEA-F806-4523-BAD0-312DCC4F63F9");
		private TraceProvider _traceProvider;
	}
}