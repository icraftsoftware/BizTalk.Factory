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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.BizTalk.Bam.EventObservation;

namespace Be.Stateless.BizTalk.Tracking.Messaging
{
	[GeneratedCode("BamActivityModel", "1.1.0.0")]
	[Serializable]
	public partial class Process
	{
		public const string ActivityName = "Process";
		internal const string ContinuationPrefix = "CONT_";

		private readonly string _activityId;
		private readonly Dictionary<string, object> _activityItems = new Dictionary<string, object>();
		private readonly EventStream _eventStream;

		public Process(string activityId, EventStream eventStream)
		{
			if (string.IsNullOrEmpty(activityId)) throw new ArgumentNullException("activityId", "activityId is required.");
			if (eventStream == null) throw new ArgumentNullException("eventStream", "eventStream is required.");
			_activityId = activityId;
			_eventStream = eventStream;
		}

		public string ActivityId
		{
			get { return _activityId; }
		}

		internal const string BeginTimeFieldName = "BeginTime";
		public DateTime? BeginTime
		{
			get { return (DateTime?) _activityItems[BeginTimeFieldName]; }
			set { if (value.HasValue) _activityItems[BeginTimeFieldName] = value.Value; }
		}

		internal const string EndTimeFieldName = "EndTime";
		public DateTime? EndTime
		{
			get { return (DateTime?) _activityItems[EndTimeFieldName]; }
			set { if (value.HasValue) _activityItems[EndTimeFieldName] = value.Value; }
		}

		internal const string InterchangeIDFieldName = "InterchangeID";
		public string InterchangeID
		{
			get { return (string) _activityItems[InterchangeIDFieldName]; }
			set { if (value != null) _activityItems[InterchangeIDFieldName] = value; }
		}

		internal const string ProcessNameFieldName = "ProcessName";
		public string ProcessName
		{
			get { return (string) _activityItems[ProcessNameFieldName]; }
			set { if (value != null) _activityItems[ProcessNameFieldName] = value; }
		}

		internal const string StatusFieldName = "Status";
		public string Status
		{
			get { return (string) _activityItems[StatusFieldName]; }
			set { if (value != null) _activityItems[StatusFieldName] = value; }
		}

		internal const string Value1FieldName = "Value1";
		public string Value1
		{
			get { return (string) _activityItems[Value1FieldName]; }
			set { if (value != null) _activityItems[Value1FieldName] = value; }
		}

		internal const string Value2FieldName = "Value2";
		public string Value2
		{
			get { return (string) _activityItems[Value2FieldName]; }
			set { if (value != null) _activityItems[Value2FieldName] = value; }
		}

		internal const string Value3FieldName = "Value3";
		public string Value3
		{
			get { return (string) _activityItems[Value3FieldName]; }
			set { if (value != null) _activityItems[Value3FieldName] = value; }
		}

		/// <summary>
		/// Begins the BAM activity.
		/// </summary>
		public void BeginProcessActivity()
		{
			// Begin the Activity using the passed identifier
			EventStream es = _eventStream;
			es.BeginActivity(ActivityName, _activityId);
		}

		/// <summary>
		/// Write any data changes to the BAM activity. This must be called after any property changes.
		/// </summary>
		public void CommitProcessActivity()
		{
			// We need to provide the key/value pairs to the BAM API
			var al = new List<object>();
			foreach (var kvp in _activityItems)
			{
				al.Add(kvp.Key);
				al.Add(kvp.Value);
			}

			// Update the BAM Activity with all of the data
			EventStream es = _eventStream;
			es.UpdateActivity(ActivityName, _activityId, al.ToArray());
			es.Flush();
		}

		/// <summary>
		/// End the BAM activity. No more changes will be permitted to this activity except by continuation.
		/// </summary>
		public void EndProcessActivity()
		{
			// End this activity, no more data can be added.
			EventStream es = _eventStream;
			es.EndActivity(ActivityName, _activityId);
		}

		/// <summary>
		/// Add a reference from this activity to another activity.
		/// </summary>
		/// <param name="otherActivityName">The related activity name. Reference names are limited to 128 characters.</param>
		/// <param name="otherActivityId">The related activity Id. Limited to 1024 characters of data.</param>
		public void AddReferenceToAnotherActivity(string otherActivityName, string otherActivityId)
		{
			AddCustomReference("Activity", otherActivityName, otherActivityId);
		}

		/// <summary>
		/// Add a custom reference to this activity, this enables 'data' to be attached to an activity, such as a message body.
		/// </summary>
		/// <param name="referenceType">The related item type. Reference type identifiers are limited to 128 characters.</param>
		/// <param name="referenceName">The related item name. Reference names are limited to 128 characters.</param>
		/// <param name="referenceData">The related item data. Limited to 1024 characters of data.</param>
		/// <remarks>See http://msdn.microsoft.com/en-us/library/aa956648(BTS.10).aspx</remarks>
		public void AddCustomReference(string referenceType, string referenceName, string referenceData)
		{
			// Add a reference to another activity
			EventStream es = _eventStream;
			es.AddReference(ActivityName, _activityId, referenceType, referenceName, referenceData);
		}

		/// <summary>
		/// Add a custom reference to this activity, this enables 'data' to be attached to an activity, such as a message body.
		/// </summary>
		/// <param name="referenceType">The related item type. Reference type identifiers are limited to 128 characters.</param>
		/// <param name="referenceName">The related item name. Reference names are limited to 128 characters.</param>
		/// <param name="referenceData">The related item data. Limited to 1024 characters of data.</param>
		/// <param name="longReferenceData">The related item data containing up to 512 KB of Unicode characters of data.</param>
		/// <remarks>See http://msdn.microsoft.com/en-us/library/aa956648(BTS.10).aspx</remarks>
		public void AddCustomReference(string referenceType, string referenceName, string referenceData, string longReferenceData)
		{
			// Add a reference to another activity
			EventStream es = _eventStream;
			es.AddReference(ActivityName, _activityId, referenceType, referenceName, referenceData, longReferenceData);
		}

		/// <summary>
		/// Activate continuation for this activity. While in the context that is enabling continuation, this activity can
		/// still be updated and MUST be ended with a call to EndProcessActivity().
		/// </summary>
		public string EnableContinuation()
		{
			string continuationId = ContinuationPrefix + _activityId;
			EventStream es = _eventStream;
			es.EnableContinuation(ActivityName, _activityId, continuationId);
			return continuationId;
		}

		/// <summary>
		/// Flush any buffered events.
		/// </summary>
		public void Flush()
		{
			EventStream es = _eventStream;
			es.Flush();
		}
	}

	[GeneratedCode("BamActivityModel", "1.1.0.0")]
	[Serializable]
	public partial class ProcessingStep
	{
		public const string ActivityName = "ProcessingStep";
		internal const string ContinuationPrefix = "CONT_";

		private readonly string _activityId;
		private readonly Dictionary<string, object> _activityItems = new Dictionary<string, object>();
		private readonly EventStream _eventStream;

		public ProcessingStep(string activityId, EventStream eventStream)
		{
			if (string.IsNullOrEmpty(activityId)) throw new ArgumentNullException("activityId", "activityId is required.");
			if (eventStream == null) throw new ArgumentNullException("eventStream", "eventStream is required.");
			_activityId = activityId;
			_eventStream = eventStream;
		}

		public string ActivityId
		{
			get { return _activityId; }
		}

		internal const string BeginTimeFieldName = "BeginTime";
		public DateTime? BeginTime
		{
			get { return (DateTime?) _activityItems[BeginTimeFieldName]; }
			set { if (value.HasValue) _activityItems[BeginTimeFieldName] = value.Value; }
		}

		internal const string EndTimeFieldName = "EndTime";
		public DateTime? EndTime
		{
			get { return (DateTime?) _activityItems[EndTimeFieldName]; }
			set { if (value.HasValue) _activityItems[EndTimeFieldName] = value.Value; }
		}

		internal const string ErrorDescriptionFieldName = "ErrorDescription";
		public string ErrorDescription
		{
			get { return (string) _activityItems[ErrorDescriptionFieldName]; }
			set { if (value != null) _activityItems[ErrorDescriptionFieldName] = value; }
		}

		internal const string MachineNameFieldName = "MachineName";
		public string MachineName
		{
			get { return (string) _activityItems[MachineNameFieldName]; }
			set { if (value != null) _activityItems[MachineNameFieldName] = value; }
		}

		internal const string ProcessActivityIDFieldName = "ProcessActivityID";
		public string ProcessActivityID
		{
			get { return (string) _activityItems[ProcessActivityIDFieldName]; }
			set { if (value != null) _activityItems[ProcessActivityIDFieldName] = value; }
		}

		internal const string StatusFieldName = "Status";
		public string Status
		{
			get { return (string) _activityItems[StatusFieldName]; }
			set { if (value != null) _activityItems[StatusFieldName] = value; }
		}

		internal const string StepNameFieldName = "StepName";
		public string StepName
		{
			get { return (string) _activityItems[StepNameFieldName]; }
			set { if (value != null) _activityItems[StepNameFieldName] = value; }
		}

		/// <summary>
		/// Begins the BAM activity.
		/// </summary>
		public void BeginProcessingStepActivity()
		{
			// Begin the Activity using the passed identifier
			EventStream es = _eventStream;
			es.BeginActivity(ActivityName, _activityId);
		}

		/// <summary>
		/// Write any data changes to the BAM activity. This must be called after any property changes.
		/// </summary>
		public void CommitProcessingStepActivity()
		{
			// We need to provide the key/value pairs to the BAM API
			var al = new List<object>();
			foreach (var kvp in _activityItems)
			{
				al.Add(kvp.Key);
				al.Add(kvp.Value);
			}

			// Update the BAM Activity with all of the data
			EventStream es = _eventStream;
			es.UpdateActivity(ActivityName, _activityId, al.ToArray());
			es.Flush();
		}

		/// <summary>
		/// End the BAM activity. No more changes will be permitted to this activity except by continuation.
		/// </summary>
		public void EndProcessingStepActivity()
		{
			// End this activity, no more data can be added.
			EventStream es = _eventStream;
			es.EndActivity(ActivityName, _activityId);
		}

		/// <summary>
		/// Add a reference from this activity to another activity.
		/// </summary>
		/// <param name="otherActivityName">The related activity name. Reference names are limited to 128 characters.</param>
		/// <param name="otherActivityId">The related activity Id. Limited to 1024 characters of data.</param>
		public void AddReferenceToAnotherActivity(string otherActivityName, string otherActivityId)
		{
			AddCustomReference("Activity", otherActivityName, otherActivityId);
		}

		/// <summary>
		/// Add a custom reference to this activity, this enables 'data' to be attached to an activity, such as a message body.
		/// </summary>
		/// <param name="referenceType">The related item type. Reference type identifiers are limited to 128 characters.</param>
		/// <param name="referenceName">The related item name. Reference names are limited to 128 characters.</param>
		/// <param name="referenceData">The related item data. Limited to 1024 characters of data.</param>
		/// <remarks>See http://msdn.microsoft.com/en-us/library/aa956648(BTS.10).aspx</remarks>
		public void AddCustomReference(string referenceType, string referenceName, string referenceData)
		{
			// Add a reference to another activity
			EventStream es = _eventStream;
			es.AddReference(ActivityName, _activityId, referenceType, referenceName, referenceData);
		}

		/// <summary>
		/// Add a custom reference to this activity, this enables 'data' to be attached to an activity, such as a message body.
		/// </summary>
		/// <param name="referenceType">The related item type. Reference type identifiers are limited to 128 characters.</param>
		/// <param name="referenceName">The related item name. Reference names are limited to 128 characters.</param>
		/// <param name="referenceData">The related item data. Limited to 1024 characters of data.</param>
		/// <param name="longReferenceData">The related item data containing up to 512 KB of Unicode characters of data.</param>
		/// <remarks>See http://msdn.microsoft.com/en-us/library/aa956648(BTS.10).aspx</remarks>
		public void AddCustomReference(string referenceType, string referenceName, string referenceData, string longReferenceData)
		{
			// Add a reference to another activity
			EventStream es = _eventStream;
			es.AddReference(ActivityName, _activityId, referenceType, referenceName, referenceData, longReferenceData);
		}

		/// <summary>
		/// Activate continuation for this activity. While in the context that is enabling continuation, this activity can
		/// still be updated and MUST be ended with a call to EndProcessingStepActivity().
		/// </summary>
		public string EnableContinuation()
		{
			string continuationId = ContinuationPrefix + _activityId;
			EventStream es = _eventStream;
			es.EnableContinuation(ActivityName, _activityId, continuationId);
			return continuationId;
		}

		/// <summary>
		/// Flush any buffered events.
		/// </summary>
		public void Flush()
		{
			EventStream es = _eventStream;
			es.Flush();
		}
	}

	[GeneratedCode("BamActivityModel", "1.1.0.0")]
	[Serializable]
	public partial class ProcessMessagingStep
	{
		public const string ActivityName = "ProcessMessagingStep";
		internal const string ContinuationPrefix = "CONT_";

		private readonly string _activityId;
		private readonly Dictionary<string, object> _activityItems = new Dictionary<string, object>();
		private readonly EventStream _eventStream;

		public ProcessMessagingStep(string activityId, EventStream eventStream)
		{
			if (string.IsNullOrEmpty(activityId)) throw new ArgumentNullException("activityId", "activityId is required.");
			if (eventStream == null) throw new ArgumentNullException("eventStream", "eventStream is required.");
			_activityId = activityId;
			_eventStream = eventStream;
		}

		public string ActivityId
		{
			get { return _activityId; }
		}

		internal const string MessagingStepActivityIDFieldName = "MessagingStepActivityID";
		public string MessagingStepActivityID
		{
			get { return (string) _activityItems[MessagingStepActivityIDFieldName]; }
			set { if (value != null) _activityItems[MessagingStepActivityIDFieldName] = value; }
		}

		internal const string MessagingStepStatusFieldName = "MessagingStepStatus";
		public string MessagingStepStatus
		{
			get { return (string) _activityItems[MessagingStepStatusFieldName]; }
			set { if (value != null) _activityItems[MessagingStepStatusFieldName] = value; }
		}

		internal const string ProcessActivityIDFieldName = "ProcessActivityID";
		public string ProcessActivityID
		{
			get { return (string) _activityItems[ProcessActivityIDFieldName]; }
			set { if (value != null) _activityItems[ProcessActivityIDFieldName] = value; }
		}

		/// <summary>
		/// Begins the BAM activity.
		/// </summary>
		public void BeginProcessMessagingStepActivity()
		{
			// Begin the Activity using the passed identifier
			EventStream es = _eventStream;
			es.BeginActivity(ActivityName, _activityId);
		}

		/// <summary>
		/// Write any data changes to the BAM activity. This must be called after any property changes.
		/// </summary>
		public void CommitProcessMessagingStepActivity()
		{
			// We need to provide the key/value pairs to the BAM API
			var al = new List<object>();
			foreach (var kvp in _activityItems)
			{
				al.Add(kvp.Key);
				al.Add(kvp.Value);
			}

			// Update the BAM Activity with all of the data
			EventStream es = _eventStream;
			es.UpdateActivity(ActivityName, _activityId, al.ToArray());
			es.Flush();
		}

		/// <summary>
		/// End the BAM activity. No more changes will be permitted to this activity except by continuation.
		/// </summary>
		public void EndProcessMessagingStepActivity()
		{
			// End this activity, no more data can be added.
			EventStream es = _eventStream;
			es.EndActivity(ActivityName, _activityId);
		}

		/// <summary>
		/// Add a reference from this activity to another activity.
		/// </summary>
		/// <param name="otherActivityName">The related activity name. Reference names are limited to 128 characters.</param>
		/// <param name="otherActivityId">The related activity Id. Limited to 1024 characters of data.</param>
		public void AddReferenceToAnotherActivity(string otherActivityName, string otherActivityId)
		{
			AddCustomReference("Activity", otherActivityName, otherActivityId);
		}

		/// <summary>
		/// Add a custom reference to this activity, this enables 'data' to be attached to an activity, such as a message body.
		/// </summary>
		/// <param name="referenceType">The related item type. Reference type identifiers are limited to 128 characters.</param>
		/// <param name="referenceName">The related item name. Reference names are limited to 128 characters.</param>
		/// <param name="referenceData">The related item data. Limited to 1024 characters of data.</param>
		/// <remarks>See http://msdn.microsoft.com/en-us/library/aa956648(BTS.10).aspx</remarks>
		public void AddCustomReference(string referenceType, string referenceName, string referenceData)
		{
			// Add a reference to another activity
			EventStream es = _eventStream;
			es.AddReference(ActivityName, _activityId, referenceType, referenceName, referenceData);
		}

		/// <summary>
		/// Add a custom reference to this activity, this enables 'data' to be attached to an activity, such as a message body.
		/// </summary>
		/// <param name="referenceType">The related item type. Reference type identifiers are limited to 128 characters.</param>
		/// <param name="referenceName">The related item name. Reference names are limited to 128 characters.</param>
		/// <param name="referenceData">The related item data. Limited to 1024 characters of data.</param>
		/// <param name="longReferenceData">The related item data containing up to 512 KB of Unicode characters of data.</param>
		/// <remarks>See http://msdn.microsoft.com/en-us/library/aa956648(BTS.10).aspx</remarks>
		public void AddCustomReference(string referenceType, string referenceName, string referenceData, string longReferenceData)
		{
			// Add a reference to another activity
			EventStream es = _eventStream;
			es.AddReference(ActivityName, _activityId, referenceType, referenceName, referenceData, longReferenceData);
		}

		/// <summary>
		/// Activate continuation for this activity. While in the context that is enabling continuation, this activity can
		/// still be updated and MUST be ended with a call to EndProcessMessagingStepActivity().
		/// </summary>
		public string EnableContinuation()
		{
			string continuationId = ContinuationPrefix + _activityId;
			EventStream es = _eventStream;
			es.EnableContinuation(ActivityName, _activityId, continuationId);
			return continuationId;
		}

		/// <summary>
		/// Flush any buffered events.
		/// </summary>
		public void Flush()
		{
			EventStream es = _eventStream;
			es.Flush();
		}
	}

	[GeneratedCode("BamActivityModel", "1.1.0.0")]
	[Serializable]
	public partial class MessagingStep
	{
		public const string ActivityName = "MessagingStep";
		internal const string ContinuationPrefix = "CONT_";

		private readonly string _activityId;
		private readonly Dictionary<string, object> _activityItems = new Dictionary<string, object>();
		private readonly EventStream _eventStream;

		public MessagingStep(string activityId, EventStream eventStream)
		{
			if (string.IsNullOrEmpty(activityId)) throw new ArgumentNullException("activityId", "activityId is required.");
			if (eventStream == null) throw new ArgumentNullException("eventStream", "eventStream is required.");
			_activityId = activityId;
			_eventStream = eventStream;
		}

		public string ActivityId
		{
			get { return _activityId; }
		}

		internal const string ErrorCodeFieldName = "ErrorCode";
		public string ErrorCode
		{
			get { return (string) _activityItems[ErrorCodeFieldName]; }
			set { if (value != null) _activityItems[ErrorCodeFieldName] = value; }
		}

		internal const string ErrorDescriptionFieldName = "ErrorDescription";
		public string ErrorDescription
		{
			get { return (string) _activityItems[ErrorDescriptionFieldName]; }
			set { if (value != null) _activityItems[ErrorDescriptionFieldName] = value; }
		}

		internal const string InterchangeIDFieldName = "InterchangeID";
		public string InterchangeID
		{
			get { return (string) _activityItems[InterchangeIDFieldName]; }
			set { if (value != null) _activityItems[InterchangeIDFieldName] = value; }
		}

		internal const string MachineNameFieldName = "MachineName";
		public string MachineName
		{
			get { return (string) _activityItems[MachineNameFieldName]; }
			set { if (value != null) _activityItems[MachineNameFieldName] = value; }
		}

		internal const string MessageIDFieldName = "MessageID";
		public string MessageID
		{
			get { return (string) _activityItems[MessageIDFieldName]; }
			set { if (value != null) _activityItems[MessageIDFieldName] = value; }
		}

		internal const string MessageSizeFieldName = "MessageSize";
		public int? MessageSize
		{
			get { return (int?) _activityItems[MessageSizeFieldName]; }
			set { if (value.HasValue) _activityItems[MessageSizeFieldName] = value.Value; }
		}

		internal const string MessageTypeFieldName = "MessageType";
		public string MessageType
		{
			get { return (string) _activityItems[MessageTypeFieldName]; }
			set { if (value != null) _activityItems[MessageTypeFieldName] = value; }
		}

		internal const string PortNameFieldName = "PortName";
		public string PortName
		{
			get { return (string) _activityItems[PortNameFieldName]; }
			set { if (value != null) _activityItems[PortNameFieldName] = value; }
		}

		internal const string RetryCountFieldName = "RetryCount";
		public int? RetryCount
		{
			get { return (int?) _activityItems[RetryCountFieldName]; }
			set { if (value.HasValue) _activityItems[RetryCountFieldName] = value.Value; }
		}

		internal const string StatusFieldName = "Status";
		public string Status
		{
			get { return (string) _activityItems[StatusFieldName]; }
			set { if (value != null) _activityItems[StatusFieldName] = value; }
		}

		internal const string TimeFieldName = "Time";
		public DateTime? Time
		{
			get { return (DateTime?) _activityItems[TimeFieldName]; }
			set { if (value.HasValue) _activityItems[TimeFieldName] = value.Value; }
		}

		internal const string TransportLocationFieldName = "TransportLocation";
		public string TransportLocation
		{
			get { return (string) _activityItems[TransportLocationFieldName]; }
			set { if (value != null) _activityItems[TransportLocationFieldName] = value; }
		}

		internal const string TransportTypeFieldName = "TransportType";
		public string TransportType
		{
			get { return (string) _activityItems[TransportTypeFieldName]; }
			set { if (value != null) _activityItems[TransportTypeFieldName] = value; }
		}

		internal const string Value1FieldName = "Value1";
		public string Value1
		{
			get { return (string) _activityItems[Value1FieldName]; }
			set { if (value != null) _activityItems[Value1FieldName] = value; }
		}

		internal const string Value2FieldName = "Value2";
		public string Value2
		{
			get { return (string) _activityItems[Value2FieldName]; }
			set { if (value != null) _activityItems[Value2FieldName] = value; }
		}

		internal const string Value3FieldName = "Value3";
		public string Value3
		{
			get { return (string) _activityItems[Value3FieldName]; }
			set { if (value != null) _activityItems[Value3FieldName] = value; }
		}

		/// <summary>
		/// Begins the BAM activity.
		/// </summary>
		public void BeginMessagingStepActivity()
		{
			// Begin the Activity using the passed identifier
			EventStream es = _eventStream;
			es.BeginActivity(ActivityName, _activityId);
		}

		/// <summary>
		/// Write any data changes to the BAM activity. This must be called after any property changes.
		/// </summary>
		public void CommitMessagingStepActivity()
		{
			// We need to provide the key/value pairs to the BAM API
			var al = new List<object>();
			foreach (var kvp in _activityItems)
			{
				al.Add(kvp.Key);
				al.Add(kvp.Value);
			}

			// Update the BAM Activity with all of the data
			EventStream es = _eventStream;
			es.UpdateActivity(ActivityName, _activityId, al.ToArray());
			es.Flush();
		}

		/// <summary>
		/// End the BAM activity. No more changes will be permitted to this activity except by continuation.
		/// </summary>
		public void EndMessagingStepActivity()
		{
			// End this activity, no more data can be added.
			EventStream es = _eventStream;
			es.EndActivity(ActivityName, _activityId);
		}

		/// <summary>
		/// Add a reference from this activity to another activity.
		/// </summary>
		/// <param name="otherActivityName">The related activity name. Reference names are limited to 128 characters.</param>
		/// <param name="otherActivityId">The related activity Id. Limited to 1024 characters of data.</param>
		public void AddReferenceToAnotherActivity(string otherActivityName, string otherActivityId)
		{
			AddCustomReference("Activity", otherActivityName, otherActivityId);
		}

		/// <summary>
		/// Add a custom reference to this activity, this enables 'data' to be attached to an activity, such as a message body.
		/// </summary>
		/// <param name="referenceType">The related item type. Reference type identifiers are limited to 128 characters.</param>
		/// <param name="referenceName">The related item name. Reference names are limited to 128 characters.</param>
		/// <param name="referenceData">The related item data. Limited to 1024 characters of data.</param>
		/// <remarks>See http://msdn.microsoft.com/en-us/library/aa956648(BTS.10).aspx</remarks>
		public void AddCustomReference(string referenceType, string referenceName, string referenceData)
		{
			// Add a reference to another activity
			EventStream es = _eventStream;
			es.AddReference(ActivityName, _activityId, referenceType, referenceName, referenceData);
		}

		/// <summary>
		/// Add a custom reference to this activity, this enables 'data' to be attached to an activity, such as a message body.
		/// </summary>
		/// <param name="referenceType">The related item type. Reference type identifiers are limited to 128 characters.</param>
		/// <param name="referenceName">The related item name. Reference names are limited to 128 characters.</param>
		/// <param name="referenceData">The related item data. Limited to 1024 characters of data.</param>
		/// <param name="longReferenceData">The related item data containing up to 512 KB of Unicode characters of data.</param>
		/// <remarks>See http://msdn.microsoft.com/en-us/library/aa956648(BTS.10).aspx</remarks>
		public void AddCustomReference(string referenceType, string referenceName, string referenceData, string longReferenceData)
		{
			// Add a reference to another activity
			EventStream es = _eventStream;
			es.AddReference(ActivityName, _activityId, referenceType, referenceName, referenceData, longReferenceData);
		}

		/// <summary>
		/// Activate continuation for this activity. While in the context that is enabling continuation, this activity can
		/// still be updated and MUST be ended with a call to EndMessagingStepActivity().
		/// </summary>
		public string EnableContinuation()
		{
			string continuationId = ContinuationPrefix + _activityId;
			EventStream es = _eventStream;
			es.EnableContinuation(ActivityName, _activityId, continuationId);
			return continuationId;
		}

		/// <summary>
		/// Flush any buffered events.
		/// </summary>
		public void Flush()
		{
			EventStream es = _eventStream;
			es.Flush();
		}
	}

}