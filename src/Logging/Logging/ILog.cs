#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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

#pragma warning disable 1580

namespace Be.Stateless.Logging
{
	/// <summary>
	/// The ILog interface is use by application to log messages into the log4net framework.
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
	/// ILog log = LogManager.GetLogger(typeof(MyClass));
	/// log.Info("Application Start");
	/// log.Debug("This is a debug message");
	/// if (log.IsDebugEnabled)
	/// {
	///   log.Debug("This is another debug message");
	/// }
	/// ]]>
	/// </code>
	/// </example>
	/// <seealso cref="log4net.ILog"/>
	/// <seealso cref="LogManager"/>
	/// <seealso cref="LogManager.GetLogger(Assembly, Type)"/>
	/// 
	/// <author>François Chabot</author>
	public interface ILog
	{
		#region Level(int.MaxValue, "OFF")

		//provided by base log4net.Core.Level.Off

		#endregion

		#region Level(110 000, "FATAL")

		/// <summary>
		/// Checks if this logger is enabled for the <see cref="Level.Fatal"/> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <see cref="Level.Fatal"/> events,
		/// <c>false</c> otherwise.
		/// </value>
		/// <seealso cref="ILogEx.Fatal(object)"/>
		/// <seealso cref="Fatal(object,Exception)"/>
		/// <seealso cref="ILogEx.FatalFormat(string,object[])"/>
		/// <seealso cref="ILogEx.FatalFormat(IFormatProvider,string,object[])"/>
		bool IsFatalEnabled { get; }

		/// <summary>
		/// Logs a message object with the <see cref="Level.Fatal"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <see cref="Level.Fatal"/> level including the stack trace of the
		/// <see cref="Exception"/> <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="ILogEx.Fatal(object)"/> overload for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFatalEnabled"/>
		/// <seealso cref="ILogEx.Fatal(object)"/>
		void Fatal(object message, Exception exception);

		#endregion Level(110 000, "FATAL")

		#region Level(90 000, "CRITICAL")

		/// <summary>
		/// Checks if this logger is enabled for the <see cref="Level.Critical"/> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <see cref="Level.Critical"/> events,
		/// <c>false</c> otherwise.
		/// </value>
		/// <seealso cref="ILogEx.Critical(object)"/>
		/// <seealso cref="Critical(object,Exception)"/>
		/// <seealso cref="ILogEx.CriticalFormat(string,object[])"/>
		/// <seealso cref="ILogEx.CriticalFormat(IFormatProvider,string,object[])"/>
		bool IsCriticalEnabled { get; }

		/// <summary>
		/// Logs a message object with the <see cref="Level.Critical"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <see cref="Level.Critical"/> level including the stack trace of the
		/// <see cref="Exception"/> <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="ILogEx.Critical(object)"/> overload for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsCriticalEnabled"/>
		/// <seealso cref="ILogEx.Critical(object)"/>
		void Critical(object message, Exception exception);

		#endregion Level(90 000, "CRITICAL")

		#region Level(80 000, "SEVERE")

		/// <summary>
		/// Checks if this logger is enabled for the <see cref="Level.Severe"/> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <see cref="Level.Severe"/> events,
		/// <c>false</c> otherwise.
		/// </value>
		/// <seealso cref="ILogEx.Severe(object)"/>
		/// <seealso cref="Severe(object,Exception)"/>
		/// <seealso cref="ILogEx.SevereFormat(string,object[])"/>
		/// <seealso cref="ILogEx.SevereFormat(IFormatProvider,string,object[])"/>
		bool IsSevereEnabled { get; }

		/// <summary>
		/// Logs a message object with the <see cref="Level.Severe"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <see cref="Level.Severe"/> level including the stack trace of the
		/// <see cref="Exception"/> <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="ILogEx.Severe(object)"/> overload for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsSevereEnabled"/>
		/// <seealso cref="ILogEx.Severe(object)"/>
		void Severe(object message, Exception exception);

		#endregion Level(80 000, "SEVERE")

		#region Level(70 000, "ERROR")

		/// <summary>
		/// Checks if this logger is enabled for the <see cref="Level.Error"/> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <see cref="Level.Error"/> events,
		/// <c>false</c> otherwise.
		/// </value>
		/// <seealso cref="ILogEx.Error(object)"/>
		/// <seealso cref="Error(object,Exception)"/>
		/// <seealso cref="ILogEx.ErrorFormat(string,object[])"/>
		/// <seealso cref="ILogEx.ErrorFormat(IFormatProvider,string,object[])"/>
		bool IsErrorEnabled { get; }

		/// <summary>
		/// Logs a message object with the <see cref="Level.Error"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <see cref="Level.Error"/> level including the stack trace of the
		/// <see cref="Exception"/> <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="ILogEx.Error(object)"/> overload for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsErrorEnabled"/>
		/// <seealso cref="ILogEx.Error(object)"/>
		void Error(object message, Exception exception);

		#endregion Level(70 000, "ERROR")

		#region Level(60 000, "WARN")

		/// <summary>
		/// Checks if this logger is enabled for the <see cref="Level.Warn"/> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <see cref="Level.Warn"/> events,
		/// <c>false</c> otherwise.
		/// </value>
		/// <seealso cref="Warn(object)"/>
		/// <seealso cref="Warn(object,Exception)"/>
		/// <seealso cref="WarnFormat(string,object[])"/>
		/// <seealso cref="WarnFormat(IFormatProvider,string,object[])"/>
		bool IsWarnEnabled { get; }

		/// <summary>
		/// Logs a message object with the <see cref="Level.Warn"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <see cref="Level.Warn"/> enabled by comparing the
		/// level of this logger with the <see cref="Level.Warn"/> level. If this logger is
		/// <see cref="Level.Warn"/> enabled, then it converts the message object (passed as parameter) to a
		/// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then
		/// proceeds to call all the registered appenders in this logger and also higher in the hierarchy
		/// depending on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Notice that passing an <see cref="Exception"/> to this method will output the name of
		/// the <see cref="Exception"/> but no stack trace. To output a stack trace use the
		/// <see cref="Warn(object, Exception)"/> overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsWarnEnabled"/>
		/// <seealso cref="Warn(object, Exception)"/>
		void Warn(object message);

		/// <summary>
		/// Logs a message object with the <see cref="Level.Warn"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <see cref="Level.Warn"/> level including the stack trace of the
		/// <see cref="Exception"/> <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Warn(object)"/> overload for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsWarnEnabled"/>
		/// <seealso cref="Warn(object)"/>
		void Warn(object message, Exception exception);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Warn"/> level.
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
		/// specify a localized provider use the <see cref="WarnFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Warn(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsWarnEnabled"/>
		/// <seealso cref="Warn(object, Exception)"/>
		/// <seealso cref="WarnFormat(IFormatProvider, string, object[])"/>
		void WarnFormat(string format, object arg0);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Warn"/> level.
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
		/// specify a localized provider use the <see cref="WarnFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Warn(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsWarnEnabled"/>
		/// <seealso cref="Warn(object, Exception)"/>
		/// <seealso cref="WarnFormat(IFormatProvider, string, object[])"/>
		void WarnFormat(string format, object arg0, object arg1);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Warn"/> level.
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
		/// specify a localized provider use the <see cref="WarnFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Warn(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsWarnEnabled"/>
		/// <seealso cref="Warn(object, Exception)"/>
		/// <seealso cref="WarnFormat(IFormatProvider, string, object[])"/>
		void WarnFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Warn"/> level.
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
		/// specify a localized provider use the <see cref="WarnFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Warn(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsWarnEnabled"/>
		/// <seealso cref="Warn(object, Exception)"/>
		/// <seealso cref="WarnFormat(IFormatProvider, string, object[])"/>
		void WarnFormat(string format, params object[] args);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Warn"/> level.
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
		/// <see cref="Exception"/> use the <see cref="Warn(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsWarnEnabled"/>
		/// <seealso cref="Warn(object, Exception)"/>
		void WarnFormat(IFormatProvider provider, string format, params object[] args);

		#endregion Level(60 000, "WARN")

		#region Level(40 000, "INFO")

		/// <summary>
		/// Checks if this logger is enabled for the <see cref="Level.Info"/> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <see cref="Level.Info"/> events,
		/// <c>false</c> otherwise.
		/// </value>
		/// <seealso cref="Info(object)"/>
		/// <seealso cref="Info(object,Exception)"/>
		/// <seealso cref="InfoFormat(string,object[])"/>
		/// <seealso cref="InfoFormat(IFormatProvider,string,object[])"/>
		bool IsInfoEnabled { get; }

		/// <summary>
		/// Logs a message object with the <see cref="Level.Info"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <see cref="Level.Info"/> enabled by comparing the
		/// level of this logger with the <see cref="Level.Info"/> level. If this logger is
		/// <see cref="Level.Info"/> enabled, then it converts the message object (passed as parameter) to a
		/// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then
		/// proceeds to call all the registered appenders in this logger and also higher in the hierarchy
		/// depending on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Notice that passing an <see cref="Exception"/> to this method will output the name of
		/// the <see cref="Exception"/> but no stack trace. To output a stack trace use the
		/// <see cref="Info(object, Exception)"/> overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsInfoEnabled"/>
		/// <seealso cref="Info(object, Exception)"/>
		void Info(object message);

		/// <summary>
		/// Logs a message object with the <see cref="Level.Info"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <see cref="Level.Info"/> level including the stack trace of the
		/// <see cref="Exception"/> <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Info(object)"/> overload for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsInfoEnabled"/>
		/// <seealso cref="Info(object)"/>
		void Info(object message, Exception exception);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Info"/> level.
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
		/// specify a localized provider use the <see cref="InfoFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Info(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsInfoEnabled"/>
		/// <seealso cref="Info(object, Exception)"/>
		/// <seealso cref="InfoFormat(IFormatProvider, string, object[])"/>
		void InfoFormat(string format, object arg0);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Info"/> level.
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
		/// specify a localized provider use the <see cref="InfoFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Info(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsInfoEnabled"/>
		/// <seealso cref="Info(object, Exception)"/>
		/// <seealso cref="InfoFormat(IFormatProvider, string, object[])"/>
		void InfoFormat(string format, object arg0, object arg1);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Info"/> level.
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
		/// specify a localized provider use the <see cref="InfoFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Info(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsInfoEnabled"/>
		/// <seealso cref="Info(object, Exception)"/>
		/// <seealso cref="InfoFormat(IFormatProvider, string, object[])"/>
		void InfoFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Info"/> level.
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
		/// specify a localized provider use the <see cref="InfoFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Info(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsInfoEnabled"/>
		/// <seealso cref="Info(object, Exception)"/>
		/// <seealso cref="InfoFormat(IFormatProvider, string, object[])"/>
		void InfoFormat(string format, params object[] args);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Info"/> level.
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
		/// <see cref="Exception"/> use the <see cref="Info(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsInfoEnabled"/>
		/// <seealso cref="Info(object, Exception)"/>
		void InfoFormat(IFormatProvider provider, string format, params object[] args);

		#endregion Level(40 000, "INFO")

		#region Level(30 000, "DEBUG")

		/// <summary>
		/// Checks if this logger is enabled for the <see cref="Level.Debug"/> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <see cref="Level.Debug"/> events,
		/// <c>false</c> otherwise.
		/// </value>
		/// <seealso cref="Debug(object)"/>
		/// <seealso cref="Debug(object,Exception)"/>
		/// <seealso cref="DebugFormat(string,object[])"/>
		/// <seealso cref="DebugFormat(IFormatProvider,string,object[])"/>
		bool IsDebugEnabled { get; }

		/// <summary>
		/// Logs a message object with the <see cref="Level.Debug"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <see cref="Level.Debug"/> enabled by comparing the
		/// level of this logger with the <see cref="Level.Debug"/> level. If this logger is
		/// <see cref="Level.Debug"/> enabled, then it converts the message object (passed as parameter) to a
		/// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then
		/// proceeds to call all the registered appenders in this logger and also higher in the hierarchy
		/// depending on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Notice that passing an <see cref="Exception"/> to this method will output the name of
		/// the <see cref="Exception"/> but no stack trace. To output a stack trace use the
		/// <see cref="Debug(object, Exception)"/> overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsDebugEnabled"/>
		/// <seealso cref="Debug(object, Exception)"/>
		void Debug(object message);

		/// <summary>
		/// Logs a message object with the <see cref="Level.Debug"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <see cref="Level.Debug"/> level including the stack trace of the
		/// <see cref="Exception"/> <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Debug(object)"/> overload for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsDebugEnabled"/>
		/// <seealso cref="Debug(object)"/>
		void Debug(object message, Exception exception);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Debug"/> level.
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
		/// specify a localized provider use the <see cref="DebugFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Debug(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsDebugEnabled"/>
		/// <seealso cref="Debug(object, Exception)"/>
		/// <seealso cref="DebugFormat(IFormatProvider, string, object[])"/>
		void DebugFormat(string format, object arg0);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Debug"/> level.
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
		/// specify a localized provider use the <see cref="DebugFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Debug(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsDebugEnabled"/>
		/// <seealso cref="Debug(object, Exception)"/>
		/// <seealso cref="DebugFormat(IFormatProvider, string, object[])"/>
		void DebugFormat(string format, object arg0, object arg1);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Debug"/> level.
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
		/// specify a localized provider use the <see cref="DebugFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Debug(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsDebugEnabled"/>
		/// <seealso cref="Debug(object, Exception)"/>
		/// <seealso cref="DebugFormat(IFormatProvider, string, object[])"/>
		void DebugFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Debug"/> level.
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
		/// specify a localized provider use the <see cref="DebugFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Debug(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsDebugEnabled"/>
		/// <seealso cref="Debug(object, Exception)"/>
		/// <seealso cref="DebugFormat(IFormatProvider, string, object[])"/>
		void DebugFormat(string format, params object[] args);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Debug"/> level.
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
		/// <see cref="Exception"/> use the <see cref="Debug(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsDebugEnabled"/>
		/// <seealso cref="Debug(object, Exception)"/>
		void DebugFormat(IFormatProvider provider, string format, params object[] args);

		#endregion Level(30 000, "DEBUG")

		#region Level(30 000, "FINE")

		/// <summary>
		/// Checks if this logger is enabled for the <see cref="Level.Fine"/> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <see cref="Level.Fine"/> events,
		/// <c>false</c> otherwise.
		/// </value>
		/// <seealso cref="Fine(object)"/>
		/// <seealso cref="Fine(object,Exception)"/>
		/// <seealso cref="FineFormat(string,object[])"/>
		/// <seealso cref="FineFormat(IFormatProvider,string,object[])"/>
		bool IsFineEnabled { get; }

		/// <summary>
		/// Logs a message object with the <see cref="Level.Fine"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <see cref="Level.Fine"/> enabled by comparing the
		/// level of this logger with the <see cref="Level.Fine"/> level. If this logger is
		/// <see cref="Level.Fine"/> enabled, then it converts the message object (passed as parameter) to a
		/// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then
		/// proceeds to call all the registered appenders in this logger and also higher in the hierarchy
		/// depending on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Notice that passing an <see cref="Exception"/> to this method will output the name of
		/// the <see cref="Exception"/> but no stack trace. To output a stack trace use the
		/// <see cref="Fine(object, Exception)"/> overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFineEnabled"/>
		/// <seealso cref="Fine(object, Exception)"/>
		void Fine(object message);

		/// <summary>
		/// Logs a message object with the <see cref="Level.Fine"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <see cref="Level.Fine"/> level including the stack trace of the
		/// <see cref="Exception"/> <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Fine(object)"/> overload for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFineEnabled"/>
		/// <seealso cref="Fine(object)"/>
		void Fine(object message, Exception exception);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Fine"/> level.
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
		/// specify a localized provider use the <see cref="FineFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Fine(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFineEnabled"/>
		/// <seealso cref="Fine(object, Exception)"/>
		/// <seealso cref="FineFormat(IFormatProvider, string, object[])"/>
		void FineFormat(string format, object arg0);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Fine"/> level.
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
		/// specify a localized provider use the <see cref="FineFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Fine(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFineEnabled"/>
		/// <seealso cref="Fine(object, Exception)"/>
		/// <seealso cref="FineFormat(IFormatProvider, string, object[])"/>
		void FineFormat(string format, object arg0, object arg1);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Fine"/> level.
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
		/// specify a localized provider use the <see cref="FineFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Fine(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFineEnabled"/>
		/// <seealso cref="Fine(object, Exception)"/>
		/// <seealso cref="FineFormat(IFormatProvider, string, object[])"/>
		void FineFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Fine"/> level.
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
		/// specify a localized provider use the <see cref="FineFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Fine(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFineEnabled"/>
		/// <seealso cref="Fine(object, Exception)"/>
		/// <seealso cref="FineFormat(IFormatProvider, string, object[])"/>
		void FineFormat(string format, params object[] args);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Fine"/> level.
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
		/// <see cref="Exception"/> use the <see cref="Fine(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFineEnabled"/>
		/// <seealso cref="Fine(object, Exception)"/>
		void FineFormat(IFormatProvider provider, string format, params object[] args);

		#endregion Level(30 000, "FINE")

		#region Level(10 000, "VERBOSE")

		/// <summary>
		/// Checks if this logger is enabled for the <see cref="Level.Verbose"/> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <see cref="Level.Verbose"/> events,
		/// <c>false</c> otherwise.
		/// </value>
		/// <seealso cref="Verbose(object)"/>
		/// <seealso cref="Verbose(object,Exception)"/>
		/// <seealso cref="VerboseFormat(string,object[])"/>
		/// <seealso cref="VerboseFormat(IFormatProvider,string,object[])"/>
		bool IsVerboseEnabled { get; }

		/// <summary>
		/// Logs a message object with the <see cref="Level.Verbose"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <see cref="Level.Verbose"/> enabled by comparing the
		/// level of this logger with the <see cref="Level.Verbose"/> level. If this logger is
		/// <see cref="Level.Verbose"/> enabled, then it converts the message object (passed as parameter) to a
		/// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then
		/// proceeds to call all the registered appenders in this logger and also higher in the hierarchy
		/// depending on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Notice that passing an <see cref="Exception"/> to this method will output the name of
		/// the <see cref="Exception"/> but no stack trace. To output a stack trace use the
		/// <see cref="Verbose(object, Exception)"/> overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsVerboseEnabled"/>
		/// <seealso cref="Verbose(object, Exception)"/>
		void Verbose(object message);

		/// <summary>
		/// Logs a message object with the <see cref="Level.Verbose"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <see cref="Level.Verbose"/> level including the stack trace of the
		/// <see cref="Exception"/> <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Verbose(object)"/> overload for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsVerboseEnabled"/>
		/// <seealso cref="Verbose(object)"/>
		void Verbose(object message, Exception exception);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Verbose"/> level.
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
		/// specify a localized provider use the <see cref="VerboseFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Verbose(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsVerboseEnabled"/>
		/// <seealso cref="Verbose(object, Exception)"/>
		/// <seealso cref="VerboseFormat(IFormatProvider, string, object[])"/>
		void VerboseFormat(string format, object arg0);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Verbose"/> level.
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
		/// specify a localized provider use the <see cref="VerboseFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Verbose(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsVerboseEnabled"/>
		/// <seealso cref="Verbose(object, Exception)"/>
		/// <seealso cref="VerboseFormat(IFormatProvider, string, object[])"/>
		void VerboseFormat(string format, object arg0, object arg1);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Verbose"/> level.
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
		/// specify a localized provider use the <see cref="VerboseFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Verbose(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsVerboseEnabled"/>
		/// <seealso cref="Verbose(object, Exception)"/>
		/// <seealso cref="VerboseFormat(IFormatProvider, string, object[])"/>
		void VerboseFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Verbose"/> level.
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
		/// specify a localized provider use the <see cref="VerboseFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Verbose(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsVerboseEnabled"/>
		/// <seealso cref="Verbose(object, Exception)"/>
		/// <seealso cref="VerboseFormat(IFormatProvider, string, object[])"/>
		void VerboseFormat(string format, params object[] args);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Verbose"/> level.
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
		/// <see cref="Exception"/> use the <see cref="Verbose(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsVerboseEnabled"/>
		/// <seealso cref="Verbose(object, Exception)"/>
		void VerboseFormat(IFormatProvider provider, string format, params object[] args);

		#endregion Level(10 000, "VERBOSE")

		#region Level(10 000, "FINEST")

		/// <summary>
		/// Checks if this logger is enabled for the <see cref="Level.Finest"/> level.
		/// </summary>
		/// <value>
		/// <c>true</c> if this logger is enabled for <see cref="Level.Finest"/> events,
		/// <c>false</c> otherwise.
		/// </value>
		/// <seealso cref="Finest(object)"/>
		/// <seealso cref="Finest(object,Exception)"/>
		/// <seealso cref="FinestFormat(string,object[])"/>
		/// <seealso cref="FinestFormat(IFormatProvider,string,object[])"/>
		bool IsFinestEnabled { get; }

		/// <summary>
		/// Logs a message object with the <see cref="Level.Finest"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <see cref="Level.Finest"/> enabled by comparing the
		/// level of this logger with the <see cref="Level.Finest"/> level. If this logger is
		/// <see cref="Level.Finest"/> enabled, then it converts the message object (passed as parameter) to a
		/// string by invoking the appropriate <see cref="log4net.ObjectRenderer.IObjectRenderer"/>. It then
		/// proceeds to call all the registered appenders in this logger and also higher in the hierarchy
		/// depending on the value of the additivity flag.
		/// </para>
		/// <para>
		/// <b>WARNING</b> Notice that passing an <see cref="Exception"/> to this method will output the name of
		/// the <see cref="Exception"/> but no stack trace. To output a stack trace use the
		/// <see cref="Finest(object, Exception)"/> overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFinestEnabled"/>
		/// <seealso cref="Finest(object, Exception)"/>
		void Finest(object message);

		/// <summary>
		/// Logs a message object with the <see cref="Level.Finest"/> level.
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// Logs a message object with the <see cref="Level.Finest"/> level including the stack trace of the
		/// <see cref="Exception"/> <paramref name="exception"/> passed as a parameter.
		/// </para>
		/// <para>
		/// See the <see cref="Finest(object)"/> overload for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFinestEnabled"/>
		/// <seealso cref="Finest(object)"/>
		void Finest(object message, Exception exception);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Finest"/> level.
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
		/// specify a localized provider use the <see cref="FinestFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Finest(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFinestEnabled"/>
		/// <seealso cref="Finest(object, Exception)"/>
		/// <seealso cref="FinestFormat(IFormatProvider, string, object[])"/>
		void FinestFormat(string format, object arg0);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Finest"/> level.
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
		/// specify a localized provider use the <see cref="FinestFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Finest(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFinestEnabled"/>
		/// <seealso cref="Finest(object, Exception)"/>
		/// <seealso cref="FinestFormat(IFormatProvider, string, object[])"/>
		void FinestFormat(string format, object arg0, object arg1);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Finest"/> level.
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
		/// specify a localized provider use the <see cref="FinestFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Finest(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFinestEnabled"/>
		/// <seealso cref="Finest(object, Exception)"/>
		/// <seealso cref="FinestFormat(IFormatProvider, string, object[])"/>
		void FinestFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Finest"/> level.
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
		/// specify a localized provider use the <see cref="FinestFormat(IFormatProvider, string, object[])"/>
		/// method overload.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="Exception"/> object to include in the log event. To pass an
		/// <see cref="Exception"/> use the <see cref="Finest(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFinestEnabled"/>
		/// <seealso cref="Finest(object, Exception)"/>
		/// <seealso cref="FinestFormat(IFormatProvider, string, object[])"/>
		void FinestFormat(string format, params object[] args);

		/// <summary>
		/// Logs a formatted message string with the <see cref="Level.Finest"/> level.
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
		/// <see cref="Exception"/> use the <see cref="Finest(object, Exception)"/> method overload instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="IsFinestEnabled"/>
		/// <seealso cref="Finest(object, Exception)"/>
		void FinestFormat(IFormatProvider provider, string format, params object[] args);

		#endregion Level(10 000, "FINEST")

		#region Level(int.MinValue, "ALL")

		//provided by base log4net.Core.Level.All

		#endregion
	}
}

#pragma warning restore 1580
