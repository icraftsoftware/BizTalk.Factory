#region Copyright & License

// Copyright © 2012 - 2014 François Chabot, Yves Dierick
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
using System.Diagnostics;
using log4net.Core;

namespace Be.Stateless.Logging.Appender
{
#pragma warning disable 618
	/// <summary>
	/// Augment the log4net's <see cref="log4net.Appender.EventLogAppender"/> by making sure that both the <see
	/// cref="EventLogEntry.Category"/> and <see cref="EventLogEntry.EventID"/> properties of the written <see
	/// cref="EventLogEntry"/> are provided with meaningful values.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Contrary to the <see cref="EventLogEntry"/>.<see cref="EventLogEntry.EntryType"/> property, the value of the <see
	/// cref="EventLogEntry"/>.<see cref="EventLogEntry.Category"/> property will accurately reflect the actual <see
	/// cref="LoggingEvent.Level"/> of the log4net <see cref="LoggingEvent"/>, as each of the possible <see
	/// cref="LoggingEvent.Level"/> values will be uniquely mapped to one member of the <see cref="short"/>-based <see
	/// cref = "Be.Stateless.Logging.Appender.LevelCategory"/> enumeration.
	/// </para>
	/// <para>
	/// The value of the <see cref="EventLogEntry"/>.<see cref="EventLogEntry.EventID"/> property will store a hash of
	/// the <see cref="Type.AssemblyQualifiedName"/> of the <see cref="LoggingEvent"/>.<see
	/// cref="LoggingEvent.ExceptionObject"/>'s <see cref="Type"/>, see <see cref="string.GetHashCode"/>. It will store a
	/// <c>0</c> (zero) should <see cref="LoggingEvent"/>.<see cref="LoggingEvent.ExceptionObject"/> be <c>null</c>.
	/// </para>
	/// </remarks>
#pragma warning restore 618
	public sealed class EventLogAppender : log4net.Appender.EventLogAppender
	{
		/// <summary>
		/// Alias the base appender <see cref="log4net.Appender.EventLogAppender.ApplicationName"/> property to better
		/// reflect the actual <see cref="EventLogEntry"/>'s data model, where it is called <see
		/// cref="EventLogEntry.Source"/>.
		/// </summary>
		public string Source
		{
			get { return ApplicationName; }
			set { ApplicationName = value; }
		}

		protected override void Append(LoggingEvent loggingEvent)
		{
			loggingEvent.Properties["Category"] = GetCategory(loggingEvent);
			loggingEvent.Properties["EventID"] = GetEventID(loggingEvent);
			base.Append(loggingEvent);
		}

		private ushort GetCategory(LoggingEvent loggingEvent)
		{
			var category = (short) Enum.Parse(typeof(LevelCategory), loggingEvent.Level.Name, true);
			var result = BitConverter.ToUInt16(BitConverter.GetBytes(category), 0);
			return result;
		}

		private ushort GetEventID(LoggingEvent loggingEvent)
		{
			var exception = loggingEvent.ExceptionObject;
			var instanceId = exception == null ? (ushort) 0 : TypeHashAlgorithm(exception.GetType());
			return instanceId;
		}

		#region Unit Test Hook Points

		internal static Func<Type, ushort> TypeHashAlgorithm
		{
			get
			{
				return t => {
					var hash = _typeNameSelector(t).GetHashCode();
					var result = BitConverter.ToUInt16(BitConverter.GetBytes(hash), 0);
					return result;
				};
			}
		}

		internal static Func<Type, string> TypeNameSelector
		{
			get { return _typeNameSelector; }
			set { _typeNameSelector = value; }
		}

		#endregion

		private static Func<Type, string> _typeNameSelector = t => t.AssemblyQualifiedName;
	}
}
