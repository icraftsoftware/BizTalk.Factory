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
using System.IO;
using log4net.Core;
using log4net.Layout.Pattern;
using log4net.Util;

namespace Be.Stateless.Logging.Util
{
	/// <summary>
	/// Class that provides the formatting functionality for exceptions in a Log4net convertion pattern. 
	/// <remarks>
	/// This converter define one option: <i>message</i>. This option convert the exception in a concatenation 
	/// from the deepest inner exception message to the top exception message.
	/// The converter without option returns the string representation of the current exception.
	/// </remarks>
	/// </summary>
	public class ExceptionPatternConverter : PatternLayoutConverter
	{
		protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
		{
			if (loggingEvent.ExceptionObject == null) return;

			if (string.IsNullOrEmpty(Option))
			{
				var exceptionString = loggingEvent.GetExceptionString();
				if (!string.IsNullOrEmpty(exceptionString))
				{
					writer.Write(exceptionString);
				}
			}
			else if (Option.Equals("message", StringComparison.OrdinalIgnoreCase))
			{
				writer.Write(GetFullMessage(loggingEvent.ExceptionObject));
			}
			else
			{
				LogLog.Debug(typeof(ExceptionPatternConverter), string.Format("Invalid Option Converter Value: [{0}]", Option));
			}
		}

		private static string GetFullMessage(Exception ex)
		{
			var message = string.Concat(ex.GetType().FullName, MESSAGE_SEPARATOR, ex.Message);
			if (ex.InnerException != null) message = string.Concat(message, EXCEPTION_MESSAGE_SEPARATOR, GetFullMessage(ex.InnerException));
			return message;
		}

		private const string MESSAGE_SEPARATOR = ": ";
		private const string EXCEPTION_MESSAGE_SEPARATOR = " ---> ";
	}
}