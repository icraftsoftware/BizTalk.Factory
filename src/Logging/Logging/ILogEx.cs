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
using System.Reflection;
using log4net.Core;

namespace Be.Stateless.Logging
{
	/// <summary>
	/// The ILogEx interface is use by applications to log messages into the log4net framework.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Use the <see cref="LogManager"/> to obtain logger instances that implement this interface. The <see
	/// cref="LogManager.GetLogger(Assembly, Type)"/> static method is used to get logger instances.
	/// </para>
	/// <para>
	/// This class contains methods for logging at different levels and also has properties for determining if
	/// those logging levels are enabled in the current configuration.
	/// </para>
	/// <para>
	/// This interface can be implemented in different ways. This documentation specifies reasonable behavior that
	/// a caller can expect from the actual implementation, however different implementations reserve the right to
	/// do things differently.
	/// </para>
	/// </remarks>
	/// <example>Simple example of logging messages
	/// <code lang="C#">
	/// <![CDATA[
	/// ILogEx log = (ILogEx) LogManager.GetLogger(typeof(MyClass));
	/// log.Emergency("Computer Failure");
	/// if (log.IsEmergencyEnabled)
	/// {
	///   log.Emergency("This is another emergency message");
	/// }
	/// ]]>
	/// </code>
	/// </example>
	/// <seealso cref="ILog"/>
	/// <seealso cref="LogManager"/>
	/// <seealso cref="LogManager.GetLogger(Assembly, Type)"/>
	/// 
	/// <author>François Chabot</author>
	public interface ILogEx : ILog
	{
		#region Level(120 000, "EMERGENCY")

		/// <summary>
		/// Checks if this logger is enabled for the <see cref="Level.Emergency"/> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <see cref="Level.Emergency"/> events,
		/// <c>false</c> otherwise.
		/// </value>
		/// <seealso cref="Emergency(object)"/>
		/// <seealso cref="Emergency(object,Exception)"/>
		/// <seealso cref="EmergencyFormat(string,object[])"/>
		/// <seealso cref="EmergencyFormat(IFormatProvider,string,object[])"/>
		bool IsEmergencyEnabled { get; }

		/// <summary>
		/// Logs a message object with the <see cref="Level.Emergency"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <see cref="Level.Emergency"/> enabled by comparing the
		/// level of this logger with the <see cref="Level.Emergency"/> level. If this logger is
		/// <see cref="Level.Emergency"/> enabled, then it converts the message object (passed as parameter) to a
		/// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then
		/// proceeds to call all the registered appenders in this logger and also higher in the hierarchy
		/// depending on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Notice that passing an <see cref="Exception"/> to this method will output the name of
		/// the <see cref="Exception"/> but no stack trace. To output a stack trace use the
		/// <see cref="Emergency(object, Exception)"/> overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsEmergencyEnabled"/>
		/// <seealso cref="Emergency(object, Exception)"/>
		void Emergency(object message);

		/// <summary>
		/// Logs a message object with the <see cref="Level.Emergency"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <see cref="Level.Emergency"/> level including the stack trace of the
		/// <see cref="Exception"/> <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Emergency(object)"/> overload for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsEmergencyEnabled"/>
		/// <seealso cref="Emergency(object)"/>
		void Emergency(object message, Exception exception);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Emergency"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="EmergencyFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Emergency(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsEmergencyEnabled"/>
		/// <seealso cref="Emergency(object, Exception)"/>
		/// <seealso cref="EmergencyFormat(IFormatProvider, string, object[])"/>
		void EmergencyFormat(string format, object arg0);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Emergency"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <param name="arg1">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="EmergencyFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Emergency(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsEmergencyEnabled"/>
		/// <seealso cref="Emergency(object, Exception)"/>
		/// <seealso cref="EmergencyFormat(IFormatProvider, string, object[])"/>
		void EmergencyFormat(string format, object arg0, object arg1);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Emergency"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <param name="arg1">An object to format</param>
		/// <param name="arg2">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="EmergencyFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Emergency(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsEmergencyEnabled"/>
		/// <seealso cref="Emergency(object, Exception)"/>
		/// <seealso cref="EmergencyFormat(IFormatProvider, string, object[])"/>
		void EmergencyFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Emergency"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="EmergencyFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Emergency(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsEmergencyEnabled"/>
		/// <seealso cref="Emergency(object, Exception)"/>
		/// <seealso cref="EmergencyFormat(IFormatProvider, string, object[])"/>
		void EmergencyFormat(string format, params object[] args);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Emergency"/> level.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting
		/// information</param>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/>
		/// method. See <c>String.Format</c> for details of the syntax of the format string and the behavior of
		/// the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Emergency(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsEmergencyEnabled"/>
		/// <seealso cref="Emergency(object, Exception)"/>
		void EmergencyFormat(IFormatProvider provider, string format, params object[] args);

		#endregion Level(120 000, "EMERGENCY")

		#region Level(110 000, "FATAL") - Part 2

		/// <summary>
		/// Logs a message object with the <see cref="Level.Fatal"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <see cref="Level.Fatal"/> enabled by comparing the
		/// level of this logger with the <see cref="Level.Fatal"/> level. If this logger is
		/// <see cref="Level.Fatal"/> enabled, then it converts the message object (passed as parameter) to a
		/// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then
		/// proceeds to call all the registered appenders in this logger and also higher in the hierarchy
		/// depending on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Notice that passing an <see cref="Exception"/> to this method will output the name of
		/// the <see cref="Exception"/> but no stack trace. To output a stack trace use the
		/// <see cref="ILog.Fatal(object, Exception)"/> overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsFatalEnabled"/>
		/// <seealso cref="ILog.Fatal(object, Exception)"/>
		void Fatal(object message);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Fatal"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="FatalFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Fatal(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsFatalEnabled"/>
		/// <seealso cref="ILog.Fatal(object, Exception)"/>
		/// <seealso cref="FatalFormat(IFormatProvider, string, object[])"/>
		void FatalFormat(string format, object arg0);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Fatal"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <param name="arg1">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="FatalFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Fatal(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsFatalEnabled"/>
		/// <seealso cref="ILog.Fatal(object, Exception)"/>
		/// <seealso cref="FatalFormat(IFormatProvider, string, object[])"/>
		void FatalFormat(string format, object arg0, object arg1);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Fatal"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <param name="arg1">An object to format</param>
		/// <param name="arg2">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="FatalFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Fatal(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsFatalEnabled"/>
		/// <seealso cref="ILog.Fatal(object, Exception)"/>
		/// <seealso cref="FatalFormat(IFormatProvider, string, object[])"/>
		void FatalFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Fatal"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="FatalFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Fatal(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsFatalEnabled"/>
		/// <seealso cref="ILog.Fatal(object, Exception)"/>
		/// <seealso cref="FatalFormat(IFormatProvider, string, object[])"/>
		void FatalFormat(string format, params object[] args);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Fatal"/> level.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting
		/// information</param>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/>
		/// method. See <c>String.Format</c> for details of the syntax of the format string and the behavior of
		/// the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Fatal(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsFatalEnabled"/>
		/// <seealso cref="ILog.Fatal(object, Exception)"/>
		void FatalFormat(IFormatProvider provider, string format, params object[] args);

		#endregion Level(110 000, "FATAL") - Part 2

		#region Level(100 000, "ALERT")

		/// <summary>
		/// Checks if this logger is enabled for the <see cref="Level.Alert"/> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <see cref="Level.Alert"/> events,
		/// <c>false</c> otherwise.
		/// </value>
		/// <seealso cref="Alert(object)"/>
		/// <seealso cref="Alert(object,Exception)"/>
		/// <seealso cref="AlertFormat(string,object[])"/>
		/// <seealso cref="AlertFormat(IFormatProvider,string,object[])"/>
		bool IsAlertEnabled { get; }

		/// <summary>
		/// Logs a message object with the <see cref="Level.Alert"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <see cref="Level.Alert"/> enabled by comparing the
		/// level of this logger with the <see cref="Level.Alert"/> level. If this logger is
		/// <see cref="Level.Alert"/> enabled, then it converts the message object (passed as parameter) to a
		/// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then
		/// proceeds to call all the registered appenders in this logger and also higher in the hierarchy
		/// depending on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Notice that passing an <see cref="Exception"/> to this method will output the name of
		/// the <see cref="Exception"/> but no stack trace. To output a stack trace use the
		/// <see cref="Alert(object, Exception)"/> overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsAlertEnabled"/>
		/// <seealso cref="Alert(object, Exception)"/>
		void Alert(object message);

		/// <summary>
		/// Logs a message object with the <see cref="Level.Alert"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <see cref="Level.Alert"/> level including the stack trace of the
		/// <see cref="Exception"/> <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Alert(object)"/> overload for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsAlertEnabled"/>
		/// <seealso cref="Alert(object)"/>
		void Alert(object message, Exception exception);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Alert"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="AlertFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Alert(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsAlertEnabled"/>
		/// <seealso cref="Alert(object, Exception)"/>
		/// <seealso cref="AlertFormat(IFormatProvider, string, object[])"/>
		void AlertFormat(string format, object arg0);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Alert"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <param name="arg1">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="AlertFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Alert(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsAlertEnabled"/>
		/// <seealso cref="Alert(object, Exception)"/>
		/// <seealso cref="AlertFormat(IFormatProvider, string, object[])"/>
		void AlertFormat(string format, object arg0, object arg1);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Alert"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <param name="arg1">An object to format</param>
		/// <param name="arg2">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="AlertFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Alert(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsAlertEnabled"/>
		/// <seealso cref="Alert(object, Exception)"/>
		/// <seealso cref="AlertFormat(IFormatProvider, string, object[])"/>
		void AlertFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Alert"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="AlertFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Alert(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsAlertEnabled"/>
		/// <seealso cref="Alert(object, Exception)"/>
		/// <seealso cref="AlertFormat(IFormatProvider, string, object[])"/>
		void AlertFormat(string format, params object[] args);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Alert"/> level.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting
		/// information</param>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/>
		/// method. See <c>String.Format</c> for details of the syntax of the format string and the behavior of
		/// the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Alert(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsAlertEnabled"/>
		/// <seealso cref="Alert(object, Exception)"/>
		void AlertFormat(IFormatProvider provider, string format, params object[] args);

		#endregion Level(100 000, "ALERT")

		#region Level(90 000, "CRITICAL") - Part 2

		/// <summary>
		/// Logs a message object with the <see cref="Level.Critical"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <see cref="Level.Critical"/> enabled by comparing the
		/// level of this logger with the <see cref="Level.Critical"/> level. If this logger is
		/// <see cref="Level.Critical"/> enabled, then it converts the message object (passed as parameter) to a
		/// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then
		/// proceeds to call all the registered appenders in this logger and also higher in the hierarchy
		/// depending on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Notice that passing an <see cref="Exception"/> to this method will output the name of
		/// the <see cref="Exception"/> but no stack trace. To output a stack trace use the
		/// <see cref="ILog.Critical(object, Exception)"/> overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsCriticalEnabled"/>
		/// <seealso cref="ILog.Critical(object, Exception)"/>
		void Critical(object message);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Critical"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="CriticalFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Critical(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsCriticalEnabled"/>
		/// <seealso cref="ILog.Critical(object, Exception)"/>
		/// <seealso cref="CriticalFormat(IFormatProvider, string, object[])"/>
		void CriticalFormat(string format, object arg0);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Critical"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <param name="arg1">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="CriticalFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Critical(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsCriticalEnabled"/>
		/// <seealso cref="ILog.Critical(object, Exception)"/>
		/// <seealso cref="CriticalFormat(IFormatProvider, string, object[])"/>
		void CriticalFormat(string format, object arg0, object arg1);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Critical"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <param name="arg1">An object to format</param>
		/// <param name="arg2">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="CriticalFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Critical(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsCriticalEnabled"/>
		/// <seealso cref="ILog.Critical(object, Exception)"/>
		/// <seealso cref="CriticalFormat(IFormatProvider, string, object[])"/>
		void CriticalFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Critical"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="CriticalFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Critical(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsCriticalEnabled"/>
		/// <seealso cref="ILog.Critical(object, Exception)"/>
		/// <seealso cref="CriticalFormat(IFormatProvider, string, object[])"/>
		void CriticalFormat(string format, params object[] args);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Critical"/> level.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting
		/// information</param>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/>
		/// method. See <c>String.Format</c> for details of the syntax of the format string and the behavior of
		/// the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Critical(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsCriticalEnabled"/>
		/// <seealso cref="ILog.Critical(object, Exception)"/>
		void CriticalFormat(IFormatProvider provider, string format, params object[] args);

		#endregion Level(90 000, "CRITICAL") - Part 2

		#region Level(80 000, "SEVERE") - Part 2

		/// <summary>
		/// Logs a message object with the <see cref="Level.Severe"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <see cref="Level.Severe"/> enabled by comparing the
		/// level of this logger with the <see cref="Level.Severe"/> level. If this logger is
		/// <see cref="Level.Severe"/> enabled, then it converts the message object (passed as parameter) to a
		/// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then
		/// proceeds to call all the registered appenders in this logger and also higher in the hierarchy
		/// depending on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Notice that passing an <see cref="Exception"/> to this method will output the name of
		/// the <see cref="Exception"/> but no stack trace. To output a stack trace use the
		/// <see cref="ILog.Severe(object, Exception)"/> overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsSevereEnabled"/>
		/// <seealso cref="ILog.Severe(object, Exception)"/>
		void Severe(object message);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Severe"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="SevereFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Severe(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsSevereEnabled"/>
		/// <seealso cref="ILog.Severe(object, Exception)"/>
		/// <seealso cref="SevereFormat(IFormatProvider, string, object[])"/>
		void SevereFormat(string format, object arg0);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Severe"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <param name="arg1">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="SevereFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Severe(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsSevereEnabled"/>
		/// <seealso cref="ILog.Severe(object, Exception)"/>
		/// <seealso cref="SevereFormat(IFormatProvider, string, object[])"/>
		void SevereFormat(string format, object arg0, object arg1);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Severe"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <param name="arg1">An object to format</param>
		/// <param name="arg2">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="SevereFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Severe(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsSevereEnabled"/>
		/// <seealso cref="ILog.Severe(object, Exception)"/>
		/// <seealso cref="SevereFormat(IFormatProvider, string, object[])"/>
		void SevereFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Severe"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="SevereFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Severe(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsSevereEnabled"/>
		/// <seealso cref="ILog.Severe(object, Exception)"/>
		/// <seealso cref="SevereFormat(IFormatProvider, string, object[])"/>
		void SevereFormat(string format, params object[] args);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Severe"/> level.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting
		/// information</param>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/>
		/// method. See <c>String.Format</c> for details of the syntax of the format string and the behavior of
		/// the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Severe(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsSevereEnabled"/>
		/// <seealso cref="ILog.Severe(object, Exception)"/>
		void SevereFormat(IFormatProvider provider, string format, params object[] args);

		#endregion Level(80 000, "SEVERE") - Part 2

		#region Level(70 000, "ERROR") - Part 2

		/// <summary>
		/// Logs a message object with the <see cref="Level.Error"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <see cref="Level.Error"/> enabled by comparing the
		/// level of this logger with the <see cref="Level.Error"/> level. If this logger is
		/// <see cref="Level.Error"/> enabled, then it converts the message object (passed as parameter) to a
		/// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then
		/// proceeds to call all the registered appenders in this logger and also higher in the hierarchy
		/// depending on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Notice that passing an <see cref="Exception"/> to this method will output the name of
		/// the <see cref="Exception"/> but no stack trace. To output a stack trace use the
		/// <see cref="ILog.Error(object, Exception)"/> overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsErrorEnabled"/>
		/// <seealso cref="ILog.Error(object, Exception)"/>
		void Error(object message);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Error"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="ErrorFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Error(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsErrorEnabled"/>
		/// <seealso cref="ILog.Error(object, Exception)"/>
		/// <seealso cref="ErrorFormat(IFormatProvider, string, object[])"/>
		void ErrorFormat(string format, object arg0);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Error"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <param name="arg1">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="ErrorFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Error(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsErrorEnabled"/>
		/// <seealso cref="ILog.Error(object, Exception)"/>
		/// <seealso cref="ErrorFormat(IFormatProvider, string, object[])"/>
		void ErrorFormat(string format, object arg0, object arg1);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Error"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <param name="arg1">An object to format</param>
		/// <param name="arg2">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="ErrorFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Error(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsErrorEnabled"/>
		/// <seealso cref="ILog.Error(object, Exception)"/>
		/// <seealso cref="ErrorFormat(IFormatProvider, string, object[])"/>
		void ErrorFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Error"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="ErrorFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Error(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsErrorEnabled"/>
		/// <seealso cref="ILog.Error(object, Exception)"/>
		/// <seealso cref="ErrorFormat(IFormatProvider, string, object[])"/>
		void ErrorFormat(string format, params object[] args);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Error"/> level.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting
		/// information</param>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/>
		/// method. See <c>String.Format</c> for details of the syntax of the format string and the behavior of
		/// the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="ILog.Error(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="ILog.IsErrorEnabled"/>
		/// <seealso cref="ILog.Error(object, Exception)"/>
		void ErrorFormat(IFormatProvider provider, string format, params object[] args);

		#endregion Level(70 000, "ERROR") - Part 2

		#region Level(20 000, "TRACE")

		/// <summary>
		/// Checks if this logger is enabled for the <see cref="Level.Trace"/> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <see cref="Level.Trace"/> events,
		/// <c>false</c> otherwise.
		/// </value>
		/// <seealso cref="Trace(object)"/>
		/// <seealso cref="Trace(object,Exception)"/>
		/// <seealso cref="TraceFormat(string,object[])"/>
		/// <seealso cref="TraceFormat(IFormatProvider,string,object[])"/>
		bool IsTraceEnabled { get; }

		/// <summary>
		/// Logs a message object with the <see cref="Level.Trace"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <see cref="Level.Trace"/> enabled by comparing the
		/// level of this logger with the <see cref="Level.Trace"/> level. If this logger is
		/// <see cref="Level.Trace"/> enabled, then it converts the message object (passed as parameter) to a
		/// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then
		/// proceeds to call all the registered appenders in this logger and also higher in the hierarchy
		/// depending on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Notice that passing an <see cref="Exception"/> to this method will output the name of
		/// the <see cref="Exception"/> but no stack trace. To output a stack trace use the
		/// <see cref="Trace(object, Exception)"/> overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsTraceEnabled"/>
		/// <seealso cref="Trace(object, Exception)"/>
		void Trace(object message);

		/// <summary>
		/// Logs a message object with the <see cref="Level.Trace"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <see cref="Level.Trace"/> level including the stack trace of the
		/// <see cref="Exception"/> <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Trace(object)"/> overload for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsTraceEnabled"/>
		/// <seealso cref="Trace(object)"/>
		void Trace(object message, Exception exception);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Trace"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="TraceFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Trace(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsTraceEnabled"/>
		/// <seealso cref="Trace(object, Exception)"/>
		/// <seealso cref="TraceFormat(IFormatProvider, string, object[])"/>
		void TraceFormat(string format, object arg0);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Trace"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <param name="arg1">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="TraceFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Trace(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsTraceEnabled"/>
		/// <seealso cref="Trace(object, Exception)"/>
		/// <seealso cref="TraceFormat(IFormatProvider, string, object[])"/>
		void TraceFormat(string format, object arg0, object arg1);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Trace"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <param name="arg1">An object to format</param>
		/// <param name="arg2">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="TraceFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Trace(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsTraceEnabled"/>
		/// <seealso cref="Trace(object, Exception)"/>
		/// <seealso cref="TraceFormat(IFormatProvider, string, object[])"/>
		void TraceFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Trace"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="TraceFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Trace(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsTraceEnabled"/>
		/// <seealso cref="Trace(object, Exception)"/>
		/// <seealso cref="TraceFormat(IFormatProvider, string, object[])"/>
		void TraceFormat(string format, params object[] args);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Trace"/> level.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting
		/// information</param>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/>
		/// method. See <c>String.Format</c> for details of the syntax of the format string and the behavior of
		/// the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Trace(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsTraceEnabled"/>
		/// <seealso cref="Trace(object, Exception)"/>
		void TraceFormat(IFormatProvider provider, string format, params object[] args);

		#endregion Level(20 000, "TRACE")

		#region Level(20 000, "FINER")

		/// <summary>
		/// Checks if this logger is enabled for the <see cref="Level.Finer"/> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <see cref="Level.Finer"/> events,
		/// <c>false</c> otherwise.
		/// </value>
		/// <seealso cref="Finer(object)"/>
		/// <seealso cref="Finer(object,Exception)"/>
		/// <seealso cref="FinerFormat(string,object[])"/>
		/// <seealso cref="FinerFormat(IFormatProvider,string,object[])"/>
		bool IsFinerEnabled { get; }

		/// <summary>
		/// Logs a message object with the <see cref="Level.Finer"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <see cref="Level.Finer"/> enabled by comparing the
		/// level of this logger with the <see cref="Level.Finer"/> level. If this logger is
		/// <see cref="Level.Finer"/> enabled, then it converts the message object (passed as parameter) to a
		/// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then
		/// proceeds to call all the registered appenders in this logger and also higher in the hierarchy
		/// depending on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Notice that passing an <see cref="Exception"/> to this method will output the name of
		/// the <see cref="Exception"/> but no stack trace. To output a stack trace use the
		/// <see cref="Finer(object, Exception)"/> overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFinerEnabled"/>
		/// <seealso cref="Finer(object, Exception)"/>
		void Finer(object message);

		/// <summary>
		/// Logs a message object with the <see cref="Level.Finer"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <see cref="Level.Finer"/> level including the stack trace of the
		/// <see cref="Exception"/> <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Finer(object)"/> overload for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFinerEnabled"/>
		/// <seealso cref="Finer(object)"/>
		void Finer(object message, Exception exception);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Finer"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="FinerFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Finer(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFinerEnabled"/>
		/// <seealso cref="Finer(object, Exception)"/>
		/// <seealso cref="FinerFormat(IFormatProvider, string, object[])"/>
		void FinerFormat(string format, object arg0);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Finer"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <param name="arg1">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="FinerFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Finer(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFinerEnabled"/>
		/// <seealso cref="Finer(object, Exception)"/>
		/// <seealso cref="FinerFormat(IFormatProvider, string, object[])"/>
		void FinerFormat(string format, object arg0, object arg1);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Finer"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="arg0">An object to format</param>
		/// <param name="arg1">An object to format</param>
		/// <param name="arg2">An object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="FinerFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Finer(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFinerEnabled"/>
		/// <seealso cref="Finer(object, Exception)"/>
		/// <seealso cref="FinerFormat(IFormatProvider, string, object[])"/>
		void FinerFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Finer"/> level.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(string, object[])"/> method. See
		/// <c>String.Format</c> for details of the syntax of the format string and the behavior of the
		/// formatting.
		/// </para>
		/// <para>
		/// The string is formatted using the <see cref="CultureInfo.InvariantCulture"/> format provider. To
		/// specify a localized provider use the <see cref="FinerFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Finer(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFinerEnabled"/>
		/// <seealso cref="Finer(object, Exception)"/>
		/// <seealso cref="FinerFormat(IFormatProvider, string, object[])"/>
		void FinerFormat(string format, params object[] args);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Finer"/> level.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting
		/// information</param>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <see cref="String.Format(IFormatProvider, string, object[])"/>
		/// method. See <c>String.Format</c> for details of the syntax of the format string and the behavior of
		/// the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Finer(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFinerEnabled"/>
		/// <seealso cref="Finer(object, Exception)"/>
		void FinerFormat(IFormatProvider provider, string format, params object[] args);

		#endregion Level(20 000, "FINER")
	}
}