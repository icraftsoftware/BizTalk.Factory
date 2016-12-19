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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace Be.Stateless.Logging.Appender
{
	[TestFixture]
	public class EventLogAppenderFixture
	{
		#region Setup/Teardown

		[OneTimeSetUp]
		public void TestFixtureSetUp()
		{
			if (!EventLog.SourceExists(TEST_SOURCE))
				throw new Exception(
					string.Format(
						"The '{0}' event log source does not exist and needs to be created. Either create it manually or run the unit tests at least once with elevated privileges.",
						TEST_SOURCE));
		}

		#endregion

		[Test]
		[Explicit("Elevated privileges are required to run this test.")]
		[Category("Elevated")]
		public void ActivatingAppenderOptionsWillChangeEventSourceIfNecessary()
		{
			const string source = "Be.Stateless.Logging.EventSourceFromOtherEventLog";
			try
			{
				if (EventLog.SourceExists(source)) EventLog.DeleteEventSource(source, MACHINE_NAME);

				EventLog.CreateEventSource(new EventSourceCreationData(source, "System"));
				var logName = EventLog.LogNameFromSourceName(source, MACHINE_NAME);
				Assert.That(logName, Is.EqualTo("System"));

				var appender = new EventLogAppender { ApplicationName = source };
				appender.ActivateOptions();

				logName = EventLog.LogNameFromSourceName(source, MACHINE_NAME);
				Assert.That(logName, Is.EqualTo(APPLICATION_LOG_NAME));
			}
			finally
			{
				EventLog.DeleteEventSource(source, MACHINE_NAME);
			}
		}

		[Test]
		[Explicit("Elevated privileges are required to run this test.")]
		[Category("Elevated")]
		public void ActivatingAppenderOptionsWillCreateEventSourceIfNecessary()
		{
			const string source = "Be.Stateless.Logging.InexistingEventSource";
			try
			{
				if (EventLog.SourceExists(source)) EventLog.DeleteEventSource(source, MACHINE_NAME);

				var appender = new EventLogAppender { ApplicationName = source };
				appender.ActivateOptions();

				var logName = EventLog.LogNameFromSourceName(source, MACHINE_NAME);
				Assert.That(logName, Is.EqualTo(APPLICATION_LOG_NAME));
			}
			finally
			{
				EventLog.DeleteEventSource(source, MACHINE_NAME);
			}
		}

		[Test]
		public void LogCriticalEvent()
		{
			var message = Guid.NewGuid().ToString();

			var exception = new Exception();
			try
			{
				throw exception;
			}
			catch (Exception ex)
			{
				_logger.Critical(message, ex);
			}

			var le = ApplicationEventLog.First(message);

			Assert.That(le.CategoryNumber, Is.EqualTo((short) LevelCategory.Critical));
			Assert.That(le.InstanceId, Is.EqualTo(EventLogAppender.TypeHashAlgorithm(exception.GetType())));
			Assert.That(le.Source, Is.EqualTo(TEST_SOURCE));
		}

		[Test]
		public void LogDebugEvent()
		{
			var message = Guid.NewGuid().ToString();

			_logger.Debug(message);

			var le = ApplicationEventLog.First(message);

			Assert.That(le.CategoryNumber, Is.EqualTo((short) LevelCategory.Debug));
			Assert.That(le.InstanceId, Is.EqualTo(0));
			Assert.That(le.Source, Is.EqualTo(TEST_SOURCE));
		}

		[Test]
		public void LogErrorEvent()
		{
			var message = Guid.NewGuid().ToString();

			var exception = new ArgumentNullException();
			try
			{
				throw exception;
			}
			catch (Exception ex)
			{
				_logger.Error(message, ex);
			}

			var le = ApplicationEventLog.First(message);

			Assert.That(le.CategoryNumber, Is.EqualTo((short) LevelCategory.Error));
			Assert.That(le.InstanceId, Is.EqualTo(EventLogAppender.TypeHashAlgorithm(exception.GetType())));
			Assert.That(le.Source, Is.EqualTo(TEST_SOURCE));
		}

		[Test]
		public void LogFinestEvent()
		{
			var message = Guid.NewGuid().ToString();

			_logger.Finest(message);

			var le = ApplicationEventLog.First(message);

			Assert.That(le.CategoryNumber, Is.EqualTo((short) LevelCategory.Finest));
			Assert.That(le.InstanceId, Is.EqualTo(0));
			Assert.That(le.Source, Is.EqualTo(TEST_SOURCE));
		}

		[Test]
		public void LogInfoEvent()
		{
			var message = Guid.NewGuid().ToString();

			_logger.Info(message);

			var le = ApplicationEventLog.First(message);

			Assert.That(le.CategoryNumber, Is.EqualTo((short) LevelCategory.Info));
			Assert.That(le.InstanceId, Is.EqualTo(0));
			Assert.That(le.Source, Is.EqualTo(TEST_SOURCE));
		}

		[Test]
		public void LogWarningEvent()
		{
			var message = Guid.NewGuid().ToString();

			_logger.Warn(message);

			var le = ApplicationEventLog.First(message);

			Assert.That(le.CategoryNumber, Is.EqualTo((short) LevelCategory.Warn));
			Assert.That(le.InstanceId, Is.EqualTo(0));
			Assert.That(le.Source, Is.EqualTo(TEST_SOURCE));
		}

		[Test]
		[Ignore("Must be run individually because inner hash function has intrinsic side-effects on other similar tests.")]
		[Category("Hash")]
		public void StringHashHasBestDistributionWithTypeAssemblyQualifiedName()
		{
			var hashes = ExceptionTypes
				.Select(EventLogAppender.TypeHashAlgorithm)
				.ToArray();
			var computedHashesCount = hashes.Length;
			var uniqueHashesCount = hashes.Distinct().Count();
			var hashCollisionsCount = computedHashesCount - uniqueHashesCount;
			var collisionPercentage = decimal.Divide(hashCollisionsCount, computedHashesCount) * 100;

			Assert.That(collisionPercentage, Is.LessThan(1));
		}

		[Test]
		[Ignore("Must be run individually because inner hash function has intrinsic side-effects on other similar tests.")]
		[Category("Hash")]
		public void StringHashHasGoodDistributionWithTypeFullName()
		{
			EventLogAppender.TypeNameSelector = t => t.FullName;

			var hashes = ExceptionTypes
				.Select(EventLogAppender.TypeHashAlgorithm)
				.ToArray();
			var computedHashesCount = hashes.Length;
			var uniqueHashesCount = hashes.Distinct().Count();
			var hashCollisionsCount = computedHashesCount - uniqueHashesCount;
			var collisionPercentage = decimal.Divide(hashCollisionsCount, computedHashesCount) * 100;

			Assert.That(collisionPercentage, Is.LessThan(15));
		}

		[Test]
		[Ignore("Must be run individually because inner hash function has intrinsic side-effects on other similar tests.")]
		[Category("Hash")]
		public void StringHashHasGoodHashDistributionWithTypeName()
		{
			EventLogAppender.TypeNameSelector = t => t.Name;

			var hashes = ExceptionTypes
				.Select(EventLogAppender.TypeHashAlgorithm)
				.ToArray();
			var computedHashesCount = hashes.Length;
			var uniqueHashesCount = hashes.Distinct().Count();
			var hashCollisionsCount = computedHashesCount - uniqueHashesCount;
			var collisionPercentage = decimal.Divide(hashCollisionsCount, computedHashesCount) * 100;

			Assert.That(collisionPercentage, Is.LessThan(17));
		}

		private IEnumerable<Type> ExceptionTypes
		{
			get
			{
				return GlobalAssemblyCache
					.GetAssemblySimpleNames()
					.Select(n => GlobalAssemblyCache.ResolvePartialName(n))
					.Select(i => Assembly.ReflectionOnlyLoadFrom(i.Location))
					.Select(
						a => {
							try
							{
								return a.GetTypes();
							}
							catch
							{
								return new Type[0];
							}
						})
					.Where(t => t.Length > 0)
					.SelectMany(t => t)
					.Where(t => typeof(Exception).IsAssignableFrom(t));
			}
		}

		private static class ApplicationEventLog
		{
			public static EventLogEntry First(string tag)
			{
				var eventLog = new EventLog(APPLICATION_LOG_NAME, MACHINE_NAME, TEST_SOURCE);
				return eventLog.Entries.Cast<EventLogEntry>().FirstOrDefault(e => e.Message.StartsWith(tag));
			}
		}

		private const string TEST_SOURCE = "Be.Stateless.Logging.EventSource";
		private const string MACHINE_NAME = ".";
		private const string APPLICATION_LOG_NAME = "Application";

		private static readonly ILog _logger = LogManager.GetLogger(typeof(EventLogAppenderFixture));
	}
}
