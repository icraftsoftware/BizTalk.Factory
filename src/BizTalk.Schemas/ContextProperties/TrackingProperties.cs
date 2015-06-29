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

using Be.Stateless.BizTalk.Schemas.Tracking;

namespace Be.Stateless.BizTalk.ContextProperties
{
	public static class TrackingProperties
	{
		public static readonly MessageContextProperty<ProcessName, string> ProcessName
			= new MessageContextProperty<ProcessName, string>();

		public static readonly MessageContextProperty<ProcessActivityId, string> ProcessActivityId
			= new MessageContextProperty<ProcessActivityId, string>();

		public static readonly MessageContextProperty<ProcessingStepActivityId, string> ProcessingStepActivityId
			= new MessageContextProperty<ProcessingStepActivityId, string>();

		public static readonly MessageContextProperty<MessagingStepActivityId, string> MessagingStepActivityId
			= new MessageContextProperty<MessagingStepActivityId, string>();

		public static readonly MessageContextProperty<Value1, string> Value1
			= new MessageContextProperty<Value1, string>();

		public static readonly MessageContextProperty<Value2, string> Value2
			= new MessageContextProperty<Value2, string>();

		public static readonly MessageContextProperty<Value3, string> Value3
			= new MessageContextProperty<Value3, string>();
	}
}