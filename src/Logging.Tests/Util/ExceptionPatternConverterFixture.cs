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
using System.Text;
using NUnit.Framework;
using log4net.Core;

namespace Be.Stateless.Logging.Util
{
	[TestFixture]
	public class ExceptionPatternConverterFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			var ex = CreateException();
			_loggingEvent = new LoggingEvent(null, null, typeof(ExceptionPatternConverter).ToString(), Level.Error, "", ex);
		}

		#endregion

		[Test]
		public void MessageOptionTest()
		{
			var converter = new ExceptionPatternConverterTester();
			converter.Option = "message";

			TextWriter writer = new StringWriter(new StringBuilder());
			converter.ConvertTester(writer, _loggingEvent);

			Assert.IsTrue(_loggingEvent.ExceptionObject.ToString().IndexOf(writer.ToString(), StringComparison.Ordinal) > -1);
		}

		[Test]
		public void UnknownOptionTest()
		{
			var converter = new ExceptionPatternConverterTester();
			converter.Option = "???";

			TextWriter writer = new StringWriter(new StringBuilder());
			converter.ConvertTester(writer, _loggingEvent);

			Assert.AreEqual(0, writer.ToString().Length);
		}

		[Test]
		public void WithoutExceptionTest()
		{
			var converter = new ExceptionPatternConverterTester();

			using (TextWriter writer = new StringWriter(new StringBuilder()))
			{
				converter.ConvertTester(
					writer,
					new LoggingEvent(null, null, typeof(ExceptionPatternConverter).ToString(), Level.Error, "", null));
				Assert.AreEqual(0, writer.ToString().Length);
			}
		}

		[Test]
		public void WithoutOptionTest()
		{
			var converter = new ExceptionPatternConverterTester();
			converter.Option = null;

			using (TextWriter writer = new StringWriter(new StringBuilder()))
			{
				converter.ConvertTester(writer, _loggingEvent);
				Assert.AreEqual(writer.ToString(), _loggingEvent.GetExceptionString());
			}
		}

		private static Exception CreateException()
		{
			try
			{
				throw new Exception("ExceptionLevel0");
			}
			catch (Exception ex)
			{
				try
				{
					throw new Exception("ExceptionLevel1", ex);
				}
				catch (Exception ex0)
				{
					try
					{
						throw new Exception("ExceptionLevel2", ex0);
					}
					catch (Exception ex1)
					{
						return ex1;
					}
				}
			}
		}

		private LoggingEvent _loggingEvent;
	}

	public class ExceptionPatternConverterTester : ExceptionPatternConverter
	{
		public void ConvertTester(TextWriter writer, LoggingEvent loggingEvent)
		{
			Convert(writer, loggingEvent);
		}
	}
}