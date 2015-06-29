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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Be.Stateless.BizTalk.Monitoring.Model;
using Be.Stateless.BizTalk.Unit.Constraints;
using NUnit.Framework;

namespace Be.Stateless.BizTalk
{
	public static class TrackingRepository
	{
		static TrackingRepository()
		{
			// ReSharper disable once UnusedVariable, see http://pinter.org/?p=2374; https://entityframework.codeplex.com/workitem/1590
			var ensureReferenceToEntityFrameworkSqlServer = typeof(System.Data.Entity.SqlServer.SqlProviderServices);

			_context = new ActivityContext();
			Timeout = TimeSpan.FromSeconds(30);
			PollingInterval = TimeSpan.FromSeconds(1);
		}

		public static IEnumerable<MessagingStep> MessagingSteps
		{
			get { return _context.MessagingSteps; }
		}

		public static IEnumerable<Process> Processes
		{
			get { return _context.Processes; }
		}

		public static TimeSpan PollingInterval { get; set; }

		public static TimeSpan Timeout { get; set; }

		public static Process SingleProcess(Func<Process, bool> predicate)
		{
			return SingleProcess(predicate, Timeout);
		}

		public static Process SingleProcess(Func<Process, bool> predicate, TimeSpan timeout)
		{
			Assert.That(
				() => {
					using (var searchContext = new ActivityContext())
					{
						return searchContext.Processes.Where(predicate).SingleOrDefault();
					}
				},
				Is.Not.Null.After(timeout, PollingInterval));
			return Processes.Where(predicate).Single();
		}

		internal static void Refresh<TEntity>(TEntity entity) where TEntity : class
		{
			_context.Entry(entity).Reload();
		}

		internal static void Refresh<TEntity, TElement>(TEntity entity, Expression<Func<TEntity, ICollection<TElement>>> navigationProperty)
			where TEntity : class
			where TElement : class
		{
			// http://www.codemag.com/Article/0907071, 8 Entity Framework Gotchas by Julia Lerman, ObjectContext.Refresh and Entity Graphs
			// http://msdn.microsoft.com/en-us/data/jj574232.aspx, Loading Related Entities, Explicitly Loading

			_context.Entry(entity).Collection(navigationProperty).Load();
		}

		private static readonly ActivityContext _context;
	}

	public static class ProcessExtensions
	{
		public static MessagingStep SingleMessagingStep(this Process process, Func<MessagingStep, bool> predicate)
		{
			return process.SingleMessagingStep(predicate, TrackingRepository.Timeout);
		}

		public static MessagingStep SingleMessagingStep(this Process process, Func<MessagingStep, bool> predicate, TimeSpan timeout)
		{
			Assert.That(
				() => {
					using (var searchContext = new ActivityContext())
					{
						return searchContext.Processes.Single(p => p.ActivityID == process.ActivityID).MessagingSteps.Where(predicate).SingleOrDefault();
					}
				},
				Is.Not.Null.After(timeout, TrackingRepository.PollingInterval));

			TrackingRepository.Refresh(process, p => p.MessagingSteps);
			return process.MessagingSteps.Where(predicate).Single();
		}
	}
}
