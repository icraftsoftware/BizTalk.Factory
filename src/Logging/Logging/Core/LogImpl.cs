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

using System;
using System.Globalization;
using log4net.Core;
using log4net.Repository;
using log4net.Util;

namespace Be.Stateless.Logging.Core
{
	/// <summary>
	/// Implementation of <see cref="ILog"/> and <see cref="ILogEx"/> wrapper interfaces.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This implementation of the <see cref="ILog"/> and <see cref="ILogEx"/> interfaces forwards to the <see
	/// cref="ILogger"/> held by the base class.
	/// </para>
	/// <para>
	/// This logger has methods to allow the caller to log at the following levels:
	/// </para>
	/// <list type="definition">
	///   <item>
	///     <term>EMERGENCY (new)</term>
	///     <description>
	///     The <see cref="Emergency(object)"/> and <see cref="EmergencyFormat(string, object[])"/> methods log messages
	///     at the <c>EMERGENCY</c> level. That is the level with that name defined in the repositories <see
	///     cref="ILoggerRepository.LevelMap"/>. The default value for this level is <see cref="Level.Emergency"/>. The
	///     <see cref="IsEmergencyEnabled"/> property tests if this level is enabled for logging.
	///     </description>
	///   </item>
	///   <item>
	///     <term>FATAL</term>
	///     <description>
	///     The <see cref="ILogEx.Fatal(object)"/> and <see cref="ILogEx.FatalFormat(string, object[])"/> methods log
	///     messages at the <c>FATAL</c> level. That is the level with that name defined in the repositories <see
	///     cref="ILoggerRepository.LevelMap"/>. The default value for this level is <see cref="Level.Fatal"/>. The <see
	///     cref="ILog.IsFatalEnabled"/> property tests if this level is enabled for logging.
	///     </description>
	///   </item>
	///   <item>
	///     <term>ALERT (new)</term>
	///     <description>
	///     The <see cref="Alert(object)"/> and <see cref="AlertFormat(string, object[])"/> methods log messages at the
	///     <c>ALERT</c> level. That is the level with that name defined in the repositories <see
	///     cref="ILoggerRepository.LevelMap"/>. The default value for this level is <see cref="Level.Alert"/>. The <see
	///     cref="IsAlertEnabled"/> property tests if this level is enabled for logging.
	///     </description>
	///   </item>
	///   <item>
	///     <term>CRITICAL (new)</term>
	///     <description>
	///     The <see cref="Critical(object)"/> and <see cref="CriticalFormat(string, object[])"/> methods log messages at
	///     the <c>CRITICAL</c> level. That is the level with that name defined in the repositories <see
	///     cref="ILoggerRepository.LevelMap"/>. The default value for this level is <see cref="Level.Critical"/>. The
	///     <see cref="IsCriticalEnabled"/> property tests if this level is enabled for logging.
	///     </description>
	///   </item>
	///   <item>
	///     <term>SEVERE (new)</term>
	///     <description>
	///     The <see cref="Severe(object)"/> and <see cref="SevereFormat(string, object[])"/> methods log messages at the
	///     <c>SEVERE</c> level. That is the level with that name defined in the repositories <see
	///     cref="ILoggerRepository.LevelMap"/>. The default value for this level is <see cref="Level.Severe"/>. The <see
	///     cref="IsSevereEnabled"/> property tests if this level is enabled for logging.
	///     </description>
	///   </item>
	///   <item>
	///     <term>ERROR</term>
	///     <description>
	///     The <see cref="ILogEx.Error(object)"/> and <see cref="ILogEx.ErrorFormat(string, object[])"/> methods log
	///     messages at the <c>ERROR</c> level. That is the level with that name defined in the repositories <see
	///     cref="ILoggerRepository.LevelMap"/>. The default value for this level is <see cref="Level.Error"/>. The <see
	///     cref="ILog.IsErrorEnabled"/> property tests if this level is enabled for logging.
	///     </description>
	///   </item>
	///   <item>
	///     <term>WARN</term>
	///     <description>
	///     The <see cref="ILog.Warn(object)"/> and <see cref="ILog.WarnFormat(string, object[])"/> methods log messages
	///     at the <c>WARN</c> level. That is the level with that name defined in the repositories <see
	///     cref="ILoggerRepository.LevelMap"/>. The default value for this level is <see cref="Level.Warn"/>. The <see
	///     cref="ILog.IsWarnEnabled"/> property tests if this level is enabled for logging.
	///     </description>
	///   </item>
	///   <item>
	///     <term>NOTICE (new)</term>
	///     <description>
	///     The <see cref="Notice(object)"/> and <see cref="NoticeFormat(string, object[])"/> methods log messages at the
	///     <c>NOTICE</c> level. That is the level with that name defined in the repositories <see
	///     cref="ILoggerRepository.LevelMap"/>. The default value for this level is <see cref="Level.Notice"/>. The <see
	///     cref="IsNoticeEnabled"/> property tests if this level is enabled for logging.
	///     </description>
	///   </item>
	///   <item>
	///     <term>Info</term>
	///     <description>
	///     The <see cref="ILog.Info(object)"/> and <see cref="ILog.InfoFormat(string, object[])"/> methods log messages
	///     at the <c>INFO</c> level. That is the level with that name defined in the repositories <see
	///     cref="ILoggerRepository.LevelMap"/>. The default value for this level is <see cref="Level.Info"/>. The <see
	///     cref="ILog.IsInfoEnabled"/> property tests if this level is enabled for logging.
	///     </description>
	///   </item>
	///   <item>
	///     <term>DEBUG</term>
	///     <description>
	///     The <see cref="ILog.Debug(object)"/> and <see cref="ILog.DebugFormat(string, object[])"/> methods log
	///     messages at the <c>DEBUG</c> level. That is the level with that name defined in the repositories <see
	///     cref="ILoggerRepository.LevelMap"/>. The default value for this level is <see cref="Level.Debug"/>. The <see
	///     cref="ILog.IsDebugEnabled"/> property tests if this level is enabled for logging.
	///     </description>
	///   </item>
	///   <item>
	///     <term>FINE (new)</term>
	///     <description>
	///     The <see cref="Fine(object)"/> and <see cref="FineFormat(string, object[])"/> methods log messages at the
	///     <c>FINE</c> level. That is the level with that name defined in the repositories <see
	///     cref="ILoggerRepository.LevelMap"/>. The default value for this level is <see cref="Level.Fine"/>. The <see
	///     cref="IsFineEnabled"/> property tests if this level is enabled for logging.
	///     </description>
	///   </item>
	///   <item>
	///     <term>TRACE (new)</term>
	///     <description>
	///     The <see cref="Trace(object)"/> and <see cref="TraceFormat(string, object[])"/> methods log messages at the
	///     <c>TRACE</c> level. That is the level with that name defined in the repositories <see
	///     cref="ILoggerRepository.LevelMap"/>. The default value for this level is <see cref="Level.Trace"/>. The <see
	///     cref="IsTraceEnabled"/> property tests if this level is enabled for logging.
	///     </description>
	///   </item>
	///   <item>
	///     <term>FINER (new)</term>
	///     <description>
	///     The <see cref="Finer(object)"/> and <see cref="FinerFormat(string, object[])"/> methods log messages at the
	///     <c>FINER</c> level. That is the level with that name defined in the repositories <see
	///     cref="ILoggerRepository.LevelMap"/>. The default value for this level is <see cref="Level.Finer"/>. The <see
	///     cref="IsFinerEnabled"/> property tests if this level is enabled for logging.
	///     </description>
	///   </item>
	///   <item>
	///     <term>VERBOSE (new)</term>
	///     <description>
	///     The <see cref="Verbose(object)"/> and <see cref="VerboseFormat(string, object[])"/> methods log messages at
	///     the <c>VERBOSE</c> level. That is the level with that name defined in the repositories <see
	///     cref="ILoggerRepository.LevelMap"/>. The default value for this level is <see cref="Level.Verbose"/>. The
	///     <see cref="IsVerboseEnabled"/> property tests if this level is enabled for logging.
	///     </description>
	///   </item>
	///   <item>
	///     <term>FINEST (new)</term>
	///     <description>
	///     The <see cref="Finest(object)"/> and <see cref="FinestFormat(string, object[])"/> methods log messages at the
	///     <c>FINEST</c> level. That is the level with that name defined in the repositories <see
	///     cref="ILoggerRepository.LevelMap"/>. The default value for this level is <see cref="Level.Finest"/>. The <see
	///     cref="IsFinestEnabled"/> property tests if this level is enabled for logging.
	///     </description>
	///   </item>
	/// </list>
	/// <para>
	/// The values for these levels and their semantic meanings can be changed by configuring the <see
	/// cref="ILoggerRepository.LevelMap"/> for the repository.
	/// </para>
	/// </remarks>
	/// 
	/// <author>François Chabot</author>
	public class LogImpl : log4net.Core.LogImpl, ILogEx
	{
		public LogImpl(ILogger logger) : base(logger) { }

		#region ILogEx Members

		/// <summary>
		/// Logs a message object with the <c>EMERGENCY</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>EMERGENCY</c> enabled by comparing the level of this logger with
		/// the <c>EMERGENCY</c> level. If this logger is <c>EMERGENCY</c> enabled, then it converts the message object
		/// (passed as parameter) to a string by invoking the appropriate <see
		/// cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then proceeds to call all the registered appenders in this
		/// logger and also higher in the hierarchy depending on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Note that passing an <see cref="Exception"/> to this method will print the name of the <see
		/// cref="Exception"/> but no stack trace. To print a stack trace use the <see
		/// cref="Emergency(object,Exception)"/> overload instead.
		/// </para>
		/// </remarks>
		public void Emergency(object message)
		{
			Logger.Log(_thisDeclaringType, _levelEmergency, message, null);
		}

		/// <summary>
		/// Logs a message object with the <c>EMERGENCY</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <c>EMERGENCY</c> level including the stack trace of the <see cref="Exception"/>
		/// <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Emergency(object)"/> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="Emergency(object)"/>
		public void Emergency(object message, Exception exception)
		{
			Logger.Log(_thisDeclaringType, _levelEmergency, message, exception);
		}

		/// <summary>
		/// Logs a formatted message string with the <c>EMERGENCY</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="EmergencyFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Emergency(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void EmergencyFormat(string format, object arg0)
		{
			if (IsEmergencyEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelEmergency, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>EMERGENCY</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="EmergencyFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Emergency(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void EmergencyFormat(string format, object arg0, object arg1)
		{
			if (IsEmergencyEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelEmergency, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>EMERGENCY</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <param name="arg2">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="EmergencyFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Emergency(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void EmergencyFormat(string format, object arg0, object arg1, object arg2)
		{
			if (IsEmergencyEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelEmergency, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1, arg2), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>EMERGENCY</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="EmergencyFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Emergency(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void EmergencyFormat(string format, params object[] args)
		{
			if (IsEmergencyEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelEmergency, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>EMERGENCY</c> level.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting
		/// information</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="EmergencyFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Emergency(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void EmergencyFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (IsEmergencyEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelEmergency, new SystemStringFormat(provider, format, args), null);
			}
		}

		/// <summary>
		/// Checks if this logger is enabled for the <c>EMERGENCY</c> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <c>EMERGENCY</c> events, <c>false</c> otherwise.
		/// </value>
		public bool IsEmergencyEnabled
		{
			get { return Logger.IsEnabledFor(_levelEmergency); }
		}

		/// <summary>
		/// Logs a message object with the <c>ALERT</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>ALERT</c> enabled by comparing the level of this logger with the
		/// <c>ALERT</c> level. If this logger is <c>ALERT</c> enabled, then it converts the message object (passed as
		/// parameter) to a string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It
		/// then proceeds to call all the registered appenders in this logger and also higher in the hierarchy depending
		/// on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Note that passing an <see cref="Exception"/> to this method will print the name of the <see
		/// cref="Exception"/> but no stack trace. To print a stack trace use the <see cref="Alert(object,Exception)"/>
		/// overload instead.
		/// </para>
		/// </remarks>
		public void Alert(object message)
		{
			Logger.Log(_thisDeclaringType, _levelAlert, message, null);
		}

		/// <summary>
		/// Logs a message object with the <c>ALERT</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <c>ALERT</c> level including the stack trace of the <see cref="Exception"/>
		/// <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Alert(object)"/> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="Alert(object)"/>
		public void Alert(object message, Exception exception)
		{
			Logger.Log(_thisDeclaringType, _levelAlert, message, exception);
		}

		/// <summary>
		/// Logs a formatted message string with the <c>ALERT</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="AlertFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Alert(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void AlertFormat(string format, object arg0)
		{
			if (IsAlertEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelAlert, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>ALERT</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="AlertFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Alert(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void AlertFormat(string format, object arg0, object arg1)
		{
			if (IsAlertEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelAlert, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>ALERT</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <param name="arg2">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="AlertFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Alert(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void AlertFormat(string format, object arg0, object arg1, object arg2)
		{
			if (IsAlertEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelAlert, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1, arg2), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>ALERT</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="AlertFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Alert(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void AlertFormat(string format, params object[] args)
		{
			if (IsAlertEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelAlert, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>ALERT</c> level.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting
		/// information</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="AlertFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Alert(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void AlertFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (IsAlertEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelAlert, new SystemStringFormat(provider, format, args), null);
			}
		}

		/// <summary>
		/// Checks if this logger is enabled for the <c>ALERT</c> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <c>ALERT</c> events, <c>false</c> otherwise.
		/// </value>
		public bool IsAlertEnabled
		{
			get { return Logger.IsEnabledFor(_levelAlert); }
		}

		/// <summary>
		/// Logs a message object with the <c>CRITICAL</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>CRITICAL</c> enabled by comparing the level of this logger with
		/// the <c>CRITICAL</c> level. If this logger is <c>CRITICAL</c> enabled, then it converts the message object
		/// (passed as parameter) to a string by invoking the appropriate <see
		/// cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then proceeds to call all the registered appenders in this
		/// logger and also higher in the hierarchy depending on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Note that passing an <see cref="Exception"/> to this method will print the name of the <see
		/// cref="Exception"/> but no stack trace. To print a stack trace use the <see cref="Critical(object,Exception)"/>
		/// overload instead.
		/// </para>
		/// </remarks>
		public void Critical(object message)
		{
			Logger.Log(_thisDeclaringType, _levelCritical, message, null);
		}

		/// <summary>
		/// Logs a message object with the <c>CRITICAL</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <c>CRITICAL</c> level including the stack trace of the <see cref="Exception"/>
		/// <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Critical(object)"/> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="Critical(object)"/>
		public void Critical(object message, Exception exception)
		{
			Logger.Log(_thisDeclaringType, _levelCritical, message, exception);
		}

		/// <summary>
		/// Logs a formatted message string with the <c>CRITICAL</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="CriticalFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Critical(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void CriticalFormat(string format, object arg0)
		{
			if (IsCriticalEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelCritical, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>CRITICAL</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="CriticalFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Critical(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void CriticalFormat(string format, object arg0, object arg1)
		{
			if (IsCriticalEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelCritical, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>CRITICAL</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <param name="arg2">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="CriticalFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Critical(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void CriticalFormat(string format, object arg0, object arg1, object arg2)
		{
			if (IsCriticalEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelCritical, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1, arg2), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>CRITICAL</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="CriticalFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Critical(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void CriticalFormat(string format, params object[] args)
		{
			if (IsCriticalEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelCritical, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>CRITICAL</c> level.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting
		/// information</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="CriticalFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Critical(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void CriticalFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (IsCriticalEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelCritical, new SystemStringFormat(provider, format, args), null);
			}
		}

		/// <summary>
		/// Checks if this logger is enabled for the <c>CRITICAL</c> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <c>CRITICAL</c> events, <c>false</c> otherwise.
		/// </value>
		public bool IsCriticalEnabled
		{
			get { return Logger.IsEnabledFor(_levelCritical); }
		}

		/// <summary>
		/// Logs a message object with the <c>SEVERE</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>SEVERE</c> enabled by comparing the level of this logger with
		/// the <c>SEVERE</c> level. If this logger is <c>SEVERE</c> enabled, then it converts the message object (passed
		/// as parameter) to a string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It
		/// then proceeds to call all the registered appenders in this logger and also higher in the hierarchy depending
		/// on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Note that passing an <see cref="Exception"/> to this method will print the name of the <see
		/// cref="Exception"/> but no stack trace. To print a stack trace use the <see cref="Severe(object,Exception)"/>
		/// overload instead.
		/// </para>
		/// </remarks>
		public void Severe(object message)
		{
			Logger.Log(_thisDeclaringType, _levelSevere, message, null);
		}

		/// <summary>
		/// Logs a message object with the <c>SEVERE</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <c>SEVERE</c> level including the stack trace of the <see cref="Exception"/>
		/// <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Severe(object)"/> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="Severe(object)"/>
		public void Severe(object message, Exception exception)
		{
			Logger.Log(_thisDeclaringType, _levelSevere, message, exception);
		}

		/// <summary>
		/// Logs a formatted message string with the <c>SEVERE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="SevereFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Severe(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void SevereFormat(string format, object arg0)
		{
			if (IsSevereEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelSevere, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>SEVERE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="SevereFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Severe(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void SevereFormat(string format, object arg0, object arg1)
		{
			if (IsSevereEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelSevere, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>SEVERE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <param name="arg2">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="SevereFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Severe(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void SevereFormat(string format, object arg0, object arg1, object arg2)
		{
			if (IsSevereEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelSevere, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1, arg2), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>SEVERE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="SevereFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Severe(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void SevereFormat(string format, params object[] args)
		{
			if (IsSevereEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelSevere, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>SEVERE</c> level.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting
		/// information</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="SevereFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Severe(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void SevereFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (IsSevereEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelSevere, new SystemStringFormat(provider, format, args), null);
			}
		}

		/// <summary>
		/// Checks if this logger is enabled for the <c>SEVERE</c> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <c>SEVERE</c> events, <c>false</c> otherwise.
		/// </value>
		public bool IsSevereEnabled
		{
			get { return Logger.IsEnabledFor(_levelSevere); }
		}

		/// <summary>
		/// Logs a message object with the <c>FINE</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>FINE</c> enabled by comparing the level of this logger with the
		/// <c>FINE</c> level. If this logger is <c>FINE</c> enabled, then it converts the message object (passed as
		/// parameter) to a string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It
		/// then proceeds to call all the registered appenders in this logger and also higher in the hierarchy depending
		/// on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Note that passing an <see cref="Exception"/> to this method will print the name of the <see
		/// cref="Exception"/> but no stack trace. To print a stack trace use the <see cref="Fine(object,Exception)"/>
		/// overload instead.
		/// </para>
		/// </remarks>
		public void Fine(object message)
		{
			Logger.Log(_thisDeclaringType, _levelFine, message, null);
		}

		/// <summary>
		/// Logs a message object with the <c>FINE</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <c>FINE</c> level including the stack trace of the <see cref="Exception"/>
		/// <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Fine(object)"/> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="Fine(object)"/>
		public void Fine(object message, Exception exception)
		{
			Logger.Log(_thisDeclaringType, _levelFine, message, exception);
		}

		/// <summary>
		/// Logs a formatted message string with the <c>FINE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="FineFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Fine(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void FineFormat(string format, object arg0)
		{
			if (IsFineEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelFine, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>FINE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="FineFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Fine(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void FineFormat(string format, object arg0, object arg1)
		{
			if (IsFineEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelFine, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>FINE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <param name="arg2">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="FineFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Fine(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void FineFormat(string format, object arg0, object arg1, object arg2)
		{
			if (IsFineEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelFine, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1, arg2), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>FINE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="FineFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Fine(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void FineFormat(string format, params object[] args)
		{
			if (IsFineEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelFine, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>FINE</c> level.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting
		/// information</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="FineFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Fine(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void FineFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (IsFineEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelFine, new SystemStringFormat(provider, format, args), null);
			}
		}

		/// <summary>
		/// Checks if this logger is enabled for the <c>FINE</c> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <c>FINE</c> events, <c>false</c> otherwise.
		/// </value>
		public bool IsFineEnabled
		{
			get { return Logger.IsEnabledFor(_levelFine); }
		}

		/// <summary>
		/// Logs a message object with the <c>TRACE</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>TRACE</c> enabled by comparing the level of this logger with the
		/// <c>TRACE</c> level. If this logger is <c>TRACE</c> enabled, then it converts the message object (passed as
		/// parameter) to a string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It
		/// then proceeds to call all the registered appenders in this logger and also higher in the hierarchy depending
		/// on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Note that passing an <see cref="Exception"/> to this method will print the name of the <see
		/// cref="Exception"/> but no stack trace. To print a stack trace use the <see cref="Trace(object,Exception)"/>
		/// overload instead.
		/// </para>
		/// </remarks>
		public void Trace(object message)
		{
			Logger.Log(_thisDeclaringType, _levelTrace, message, null);
		}

		/// <summary>
		/// Logs a message object with the <c>TRACE</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <c>TRACE</c> level including the stack trace of the <see cref="Exception"/>
		/// <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Trace(object)"/> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="Trace(object)"/>
		public void Trace(object message, Exception exception)
		{
			Logger.Log(_thisDeclaringType, _levelTrace, message, exception);
		}

		/// <summary>
		/// Logs a formatted message string with the <c>TRACE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="TraceFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Trace(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void TraceFormat(string format, object arg0)
		{
			if (IsTraceEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelTrace, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>TRACE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="TraceFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Trace(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void TraceFormat(string format, object arg0, object arg1)
		{
			if (IsTraceEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelTrace, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>TRACE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <param name="arg2">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="TraceFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Trace(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void TraceFormat(string format, object arg0, object arg1, object arg2)
		{
			if (IsTraceEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelTrace, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1, arg2), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>TRACE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="TraceFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Trace(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void TraceFormat(string format, params object[] args)
		{
			if (IsTraceEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelTrace, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>TRACE</c> level.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting
		/// information</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="TraceFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Trace(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void TraceFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (IsTraceEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelTrace, new SystemStringFormat(provider, format, args), null);
			}
		}

		/// <summary>
		/// Checks if this logger is enabled for the <c>TRACE</c> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <c>TRACE</c> events, <c>false</c> otherwise.
		/// </value>
		public bool IsTraceEnabled
		{
			get { return Logger.IsEnabledFor(_levelTrace); }
		}

		/// <summary>
		/// Logs a message object with the <c>FINER</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>FINER</c> enabled by comparing the level of this logger with the
		/// <c>FINER</c> level. If this logger is <c>FINER</c> enabled, then it converts the message object (passed as
		/// parameter) to a string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It
		/// then proceeds to call all the registered appenders in this logger and also higher in the hierarchy depending
		/// on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Note that passing an <see cref="Exception"/> to this method will print the name of the <see
		/// cref="Exception"/> but no stack trace. To print a stack trace use the <see cref="Finer(object,Exception)"/>
		/// overload instead.
		/// </para>
		/// </remarks>
		public void Finer(object message)
		{
			Logger.Log(_thisDeclaringType, _levelFiner, message, null);
		}

		/// <summary>
		/// Logs a message object with the <c>FINER</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <c>FINER</c> level including the stack trace of the <see cref="Exception"/>
		/// <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Finer(object)"/> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="Finer(object)"/>
		public void Finer(object message, Exception exception)
		{
			Logger.Log(_thisDeclaringType, _levelFiner, message, exception);
		}

		/// <summary>
		/// Logs a formatted message string with the <c>FINER</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="FinerFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Finer(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void FinerFormat(string format, object arg0)
		{
			if (IsFinerEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelFiner, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>FINER</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="FinerFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Finer(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void FinerFormat(string format, object arg0, object arg1)
		{
			if (IsFinerEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelFiner, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>FINER</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <param name="arg2">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="FinerFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Finer(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void FinerFormat(string format, object arg0, object arg1, object arg2)
		{
			if (IsFinerEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelFiner, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1, arg2), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>FINER</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="FinerFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Finer(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void FinerFormat(string format, params object[] args)
		{
			if (IsFinerEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelFiner, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>FINER</c> level.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting
		/// information</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="FinerFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Finer(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void FinerFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (IsFinerEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelFiner, new SystemStringFormat(provider, format, args), null);
			}
		}

		/// <summary>
		/// Checks if this logger is enabled for the <c>FINER</c> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <c>FINER</c> events, <c>false</c> otherwise.
		/// </value>
		public bool IsFinerEnabled
		{
			get { return Logger.IsEnabledFor(_levelFiner); }
		}

		/// <summary>
		/// Logs a message object with the <c>VERBOSE</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>VERBOSE</c> enabled by comparing the level of this logger with
		/// the <c>VERBOSE</c> level. If this logger is <c>VERBOSE</c> enabled, then it converts the message object
		/// (passed as parameter) to a string by invoking the appropriate <see
		/// cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then proceeds to call all the registered appenders in this
		/// logger and also higher in the hierarchy depending on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Note that passing an <see cref="Exception"/> to this method will print the name of the <see
		/// cref="Exception"/> but no stack trace. To print a stack trace use the <see cref="Verbose(object,Exception)"/>
		/// overload instead.
		/// </para>
		/// </remarks>
		public void Verbose(object message)
		{
			Logger.Log(_thisDeclaringType, _levelVerbose, message, null);
		}

		/// <summary>
		/// Logs a message object with the <c>VERBOSE</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <c>VERBOSE</c> level including the stack trace of the <see cref="Exception"/>
		/// <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Verbose(object)"/> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="Verbose(object)"/>
		public void Verbose(object message, Exception exception)
		{
			Logger.Log(_thisDeclaringType, _levelVerbose, message, exception);
		}

		/// <summary>
		/// Logs a formatted message string with the <c>VERBOSE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="VerboseFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Verbose(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void VerboseFormat(string format, object arg0)
		{
			if (IsVerboseEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelVerbose, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>VERBOSE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="VerboseFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Verbose(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void VerboseFormat(string format, object arg0, object arg1)
		{
			if (IsVerboseEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelVerbose, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>VERBOSE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <param name="arg2">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="VerboseFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Verbose(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void VerboseFormat(string format, object arg0, object arg1, object arg2)
		{
			if (IsVerboseEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelVerbose, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1, arg2), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>VERBOSE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="VerboseFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Verbose(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void VerboseFormat(string format, params object[] args)
		{
			if (IsVerboseEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelVerbose, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>VERBOSE</c> level.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting
		/// information</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="VerboseFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Verbose(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void VerboseFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (IsVerboseEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelVerbose, new SystemStringFormat(provider, format, args), null);
			}
		}

		/// <summary>
		/// Checks if this logger is enabled for the <c>VERBOSE</c> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <c>VERBOSE</c> events, <c>false</c> otherwise.
		/// </value>
		public bool IsVerboseEnabled
		{
			get { return Logger.IsEnabledFor(_levelVerbose); }
		}

		/// <summary>
		/// Logs a message object with the <c>FINEST</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>FINEST</c> enabled by comparing the level of this logger with
		/// the <c>FINEST</c> level. If this logger is <c>FINEST</c> enabled, then it converts the message object (passed
		/// as parameter) to a string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It
		/// then proceeds to call all the registered appenders in this logger and also higher in the hierarchy depending
		/// on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Note that passing an <see cref="Exception"/> to this method will print the name of the <see
		/// cref="Exception"/> but no stack trace. To print a stack trace use the <see cref="Finest(object,Exception)"/>
		/// overload instead.
		/// </para>
		/// </remarks>
		public void Finest(object message)
		{
			Logger.Log(_thisDeclaringType, _levelFinest, message, null);
		}

		/// <summary>
		/// Logs a message object with the <c>FINEST</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <c>FINEST</c> level including the stack trace of the <see cref="Exception"/>
		/// <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Finest(object)"/> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="Finest(object)"/>
		public void Finest(object message, Exception exception)
		{
			Logger.Log(_thisDeclaringType, _levelFinest, message, exception);
		}

		/// <summary>
		/// Logs a formatted message string with the <c>FINEST</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="FinestFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Finest(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void FinestFormat(string format, object arg0)
		{
			if (IsFinestEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelFinest, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>FINEST</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="FinestFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Finest(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void FinestFormat(string format, object arg0, object arg1)
		{
			if (IsFinestEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelFinest, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>FINEST</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <param name="arg2">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="FinestFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Finest(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void FinestFormat(string format, object arg0, object arg1, object arg2)
		{
			if (IsFinestEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelFinest, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1, arg2), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>FINEST</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="FinestFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Finest(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void FinestFormat(string format, params object[] args)
		{
			if (IsFinestEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelFinest, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>FINEST</c> level.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting
		/// information</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="FinestFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Finest(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void FinestFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (IsFinestEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelFinest, new SystemStringFormat(provider, format, args), null);
			}
		}

		/// <summary>
		/// Checks if this logger is enabled for the <c>FINEST</c> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <c>FINEST</c> events, <c>false</c> otherwise.
		/// </value>
		public bool IsFinestEnabled
		{
			get { return Logger.IsEnabledFor(_levelFinest); }
		}

		#endregion

		#region Base Class Member Overrides

		/// <summary>
		/// Lookup the current value of the TRACE level
		/// </summary>
		protected override void ReloadLevels(ILoggerRepository repository)
		{
			base.ReloadLevels(repository);

			_levelEmergency = repository.LevelMap.LookupWithDefault(Level.Emergency);
			_levelAlert = repository.LevelMap.LookupWithDefault(Level.Alert);
			_levelCritical = repository.LevelMap.LookupWithDefault(Level.Critical);
			_levelSevere = repository.LevelMap.LookupWithDefault(Level.Severe);
			_levelNotice = repository.LevelMap.LookupWithDefault(Level.Notice);
			_levelFine = repository.LevelMap.LookupWithDefault(Level.Fine);
			_levelTrace = repository.LevelMap.LookupWithDefault(Level.Trace);
			_levelFiner = repository.LevelMap.LookupWithDefault(Level.Finer);
			_levelVerbose = repository.LevelMap.LookupWithDefault(Level.Verbose);
			_levelFinest = repository.LevelMap.LookupWithDefault(Level.Finest);
		}

		#endregion

		/// <summary>
		/// Checks if this logger is enabled for the <c>NOTICE</c> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <c>NOTICE</c> events, <c>false</c> otherwise.
		/// </value>
		public bool IsNoticeEnabled
		{
			get { return Logger.IsEnabledFor(_levelNotice); }
		}

		/// <summary>
		/// Logs a message object with the <c>NOTICE</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>NOTICE</c> enabled by comparing the level of this logger with
		/// the <c>NOTICE</c> level. If this logger is <c>NOTICE</c> enabled, then it converts the message object (passed
		/// as parameter) to a string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It
		/// then proceeds to call all the registered appenders in this logger and also higher in the hierarchy depending
		/// on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Note that passing an <see cref="Exception"/> to this method will print the name of the <see
		/// cref="Exception"/> but no stack trace. To print a stack trace use the <see cref="Notice(object,Exception)"/>
		/// overload instead.
		/// </para>
		/// </remarks>
		public void Notice(object message)
		{
			Logger.Log(_thisDeclaringType, _levelNotice, message, null);
		}

		/// <summary>
		/// Logs a message object with the <c>NOTICE</c> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <c>NOTICE</c> level including the stack trace of the <see cref="Exception"/>
		/// <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Notice(object)"/> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="Notice(object)"/>
		public void Notice(object message, Exception exception)
		{
			Logger.Log(_thisDeclaringType, _levelNotice, message, exception);
		}

		/// <summary>
		/// Logs a formatted message string with the <c>NOTICE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="NoticeFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Notice(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void NoticeFormat(string format, object arg0)
		{
			if (IsNoticeEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelNotice, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>NOTICE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="NoticeFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Notice(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void NoticeFormat(string format, object arg0, object arg1)
		{
			if (IsNoticeEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelNotice, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>NOTICE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <param name="arg2">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="NoticeFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Notice(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void NoticeFormat(string format, object arg0, object arg1, object arg2)
		{
			if (IsNoticeEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelNotice, new SystemStringFormat(CultureInfo.InvariantCulture, format, arg0, arg1, arg2), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>NOTICE</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="NoticeFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Notice(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void NoticeFormat(string format, params object[] args)
		{
			if (IsNoticeEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelNotice, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>NOTICE</c> level.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting
		/// information</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To specify a
		/// localized provider use the <see cref="NoticeFormat(IFormatProvider, string, object[])"/> method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an <see
		/// cref="Exception"/> use the <see cref="Notice(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		public void NoticeFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (IsNoticeEnabled)
			{
				Logger.Log(_thisDeclaringType, _levelNotice, new SystemStringFormat(provider, format, args), null);
			}
		}

		#region Private Static Instance Fields

		/// <summary>
		/// The fully qualified name of this declaring type not the type of any subclass.
		/// </summary>
		private static readonly Type _thisDeclaringType = typeof(LogImpl);

		#endregion

		#region Private Fields

		/// <summary>
		/// The current value for the ALERT level
		/// </summary>
		private Level _levelAlert;

		/// <summary>
		/// The current value for the CRITICAL level
		/// </summary>
		private Level _levelCritical;

		/// <summary>
		/// The current value for the EMERGENCY level
		/// </summary>
		private Level _levelEmergency;

		/// <summary>
		/// The current value for the FINE level
		/// </summary>
		private Level _levelFine;

		/// <summary>
		/// The current value for the FINER level
		/// </summary>
		private Level _levelFiner;

		/// <summary>
		/// The current value for the FINEST level
		/// </summary>
		private Level _levelFinest;

		/// <summary>
		/// The current value for the NOTICE level
		/// </summary>
		private Level _levelNotice;

		/// <summary>
		/// The current value for the SEVERE level
		/// </summary>
		private Level _levelSevere;

		/// <summary>
		/// The current value for the TRACE level
		/// </summary>
		private Level _levelTrace;

		/// <summary>
		/// The current value for the VERBOSE level
		/// </summary>
		private Level _levelVerbose;

		#endregion
	}
}
