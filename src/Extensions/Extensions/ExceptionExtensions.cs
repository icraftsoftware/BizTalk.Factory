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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Be.Stateless.Extensions
{
	public static class ExceptionExtensions
	{
		/// <summary>
		/// Determines whether an exception is fatal, that is to say <b>unrecoverable</b>, or not.
		/// </summary>
		/// <param name="exception">
		/// The exception to assess.
		/// </param>
		/// <returns>
		/// <c>true</c> if <paramref name="exception"/> is fatal; <c>false</c> otherwise.
		/// </returns>
		/// <seealso href="http://vasters.com/clemensv/2012/09/06/Are+You+Catching+Falling+Knives.aspx"/>
		public static bool IsFatal(this Exception exception)
		{
			while (exception != null)
			{
				if ((exception is OutOfMemoryException && !(exception is InsufficientMemoryException))
					|| exception is BadImageFormatException
					|| exception is ThreadAbortException
					|| exception is AccessViolationException
					|| exception is SEHException
					|| exception is StackOverflowException)
				{
					return true;
				}
				if (!(exception is TypeInitializationException) && !(exception is TargetInvocationException))
				{
					break;
				}
				exception = exception.InnerException;
			}
			return false;
		}
	}
}
