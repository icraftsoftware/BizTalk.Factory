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
using System.Globalization;
using System.Text;
using Be.Stateless.Logging;
using Microsoft.RuleEngine;

namespace Be.Stateless.BizTalk.RuleEngine
{
	public class RuleSetTrackingLogger : IRuleSetTrackingInterceptor
	{
		public static IRuleSetTrackingInterceptor Create(RuleSetInfo ruleSetInfo)
		{
			var logger = LogManager.GetLogger(ruleSetInfo.Name);
			return logger.IsDebugEnabled ? new RuleSetTrackingLogger(logger) : null;
		}

		internal RuleSetTrackingLogger(ILog logger)
		{
			_logger = logger;
		}

		#region IRuleSetTrackingInterceptor Members

		public void SetTrackingConfig(TrackingConfiguration trackingConfig)
		{
			// TODO IRuleSetTrackingInterceptor.SetTrackingConfig
		}

		public void TrackRuleSetEngineAssociation(RuleSetInfo ruleSetInfo, Guid ruleEngineGuid)
		{
			_ruleSetName = string.Format("Ruleset Name: {0}.{1}.{2}", ruleSetInfo.Name, ruleSetInfo.MajorRevision, ruleSetInfo.MinorRevision);
			_ruleEngineInstanceId = "Rule Engine Instance Identifier: " + ruleEngineGuid;
			if (_logger.IsDebugEnabled) LogMessage("RULESET ENGINE ASSOCIATION");
		}

		public void TrackFactActivity(FactActivityType activityType, string classType, int classInstanceId)
		{
			if (!_logger.IsDebugEnabled) return;
			_builder.AppendFormat("\n\tOperation: {0}", activityType);
			_builder.AppendFormat("\n\tObject Type: {0}", classType);
			_builder.AppendFormat("\n\tObject Instance Identifer: {0}", classInstanceId);
			LogMessage("FACT ACTIVITY");
		}

		public void TrackConditionEvaluation(
			string testExpression,
			string leftClassType,
			int leftClassInstanceId,
			object leftValue,
			string rightClassType,
			int rightClassInstanceId,
			object rightValue,
			bool result)
		{
			if (!_logger.IsDebugEnabled) return;
			_builder.AppendFormat("\n\tTest Expression: {0}", testExpression);
			_builder.AppendFormat("\n\tLeft Operand Type, InstanceId, Value: ({0}, {1}, {2})", leftClassType, leftClassInstanceId, leftValue);
			_builder.AppendFormat("\n\tRight Operand Type, InstanceId, Value: ({0}, {1}, {2})", rightClassType, rightClassInstanceId, rightValue);
			_builder.AppendFormat("\n\tTest Result: {0}", result);
			LogMessage("CONDITION EVALUATION TEST (MATCH)");
		}

		public void TrackAgendaUpdate(bool isAddition, string ruleName, object conflictResolutionCriteria)
		{
			if (!_logger.IsDebugEnabled) return;
			_builder.AppendFormat("\n\tOperation: {0}", (isAddition ? "Add" : "Remove"));
			_builder.AppendFormat("\n\tRule Name: {0}", ruleName);
			_builder.AppendFormat("\n\tConflict Resolution Criteria: {0}", conflictResolutionCriteria);
			LogMessage("AGENDA UPDATE");
		}

		public void TrackRuleFiring(string ruleName, object conflictResolutionCriteria)
		{
			if (!_logger.IsDebugEnabled) return;
			_builder.AppendFormat("\n\tRule Name: {0}", ruleName);
			_builder.AppendFormat("\n\tConflict Resolution Criteria: {0}", conflictResolutionCriteria);
			LogMessage("RULE FIRED");
		}

		#endregion

		private void LogMessage(string operationType)
		{
			_builder.Insert(
				0,
				string.Format(
					"RuleSetTrackingLogger\n\t{0} {1}\n\t{2}\n\t{3}",
					operationType,
					DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
					_ruleEngineInstanceId,
					_ruleSetName));
			_builder.AppendLine();
			_logger.Debug(_builder.ToString());
			// clear the buffer (see http://bobondevelopment.com/2007/06/11/three-ways-to-clear-a-stringbuilder/)
			_builder.Length = 0;
		}

		private readonly StringBuilder _builder = new StringBuilder(512);
		private readonly ILog _logger;
		private string _ruleEngineInstanceId;
		private string _ruleSetName;
	}
}
