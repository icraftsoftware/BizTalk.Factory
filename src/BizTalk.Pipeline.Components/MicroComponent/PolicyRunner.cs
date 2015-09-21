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

using System.Xml.Serialization;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.RuleEngine;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// Executes a BizTalk Business Rule Policy against facts asserted in the message context.
	/// </summary>
	public class PolicyRunner : IMicroPipelineComponent
	{
		#region IMicroPipelineComponent Members

		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			if (Policy == null) return message;

			if (ExecutionMode == PluginExecutionMode.Deferred)
			{
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Scheduling policy '{0}' for deferred execution.", Policy.ToString());
				message.BodyPart.WrapOriginalDataStream(
					originalStream => {
						var substitutionStream = new EventingReadStream(originalStream);
						substitutionStream.AfterLastReadEvent += (src, args) => {
							if (_logger.IsDebugEnabled) _logger.DebugFormat("Executing policy '{0}' that was scheduled for deferred execution.", Policy.ToString());
							RuleEngine.Policy.Execute(Policy, new Context(message.Context));
						};
						return substitutionStream;
					},
					pipelineContext.ResourceTracker);
			}
			else
			{
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Executing policy '{0}' that is scheduled for immediate execution.", Policy.ToString());
				RuleEngine.Policy.Execute(Policy, new Context(message.Context));
			}
			return message;
		}

		#endregion

		/// <summary>
		/// The Business Rule Policy execution mode, either <see cref="PluginExecutionMode.Immediate"/> or <see
		/// cref="PluginExecutionMode.Deferred"/>.
		/// </summary>
		public PluginExecutionMode ExecutionMode { get; set; }

		/// <summary>
		/// The Business Rule Policy to be executed.
		/// </summary>
		[XmlElement("Builder", typeof(PolicyNameXmlSerializer))]
		public PolicyName Policy { get; set; }

		private static readonly ILog _logger = LogManager.GetLogger(typeof(PolicyRunner));
	}
}
