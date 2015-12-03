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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Be.Stateless.BizTalk.Dsl.RuleEngine;
using Be.Stateless.Linq.Extensions;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Operations;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.Process
{
	public abstract class ProcessFixture
	{
		#region Setup/Teardown

		[TestFixtureSetUp]
		protected void BizTalkFactoryProcessFixtureTestFixtureSetUp()
		{
			AllFolders.Each(
				d => {
					if (!Directory.Exists(d))
					{
						_logger.FineFormat("Creating folder '{0}'.", d);
						Directory.CreateDirectory(d);
					}
				});
		}

		[SetUp]
		protected void BizTalkFactoryProcessFixtureSetup()
		{
			_logger.InfoFormat("Running test '{0}'.", TestContext.CurrentContext.Test.FullName);
			_logger.Debug("Emptying output folders.");
			CleanFolders(AllOutputFolders);

			StartTime = DateTime.UtcNow;
		}

		[TearDown]
		protected void BizTalkFactoryProcessFixtureTearDown()
		{
			_logger.Debug("Terminating uncompleted BizTalk service instances.");
			TerminateUncompletedBizTalkServiceInstances();

			_logger.Debug("Emptying input folders.");
			CleanFolders(AllInputFolders);
		}

		#endregion

		protected ProcessFixture()
		{
			// compute values of ProcessNameAttribute-qualified members so that one can use them in unit test assertions

			// only assemblies referencing the target assembly must be taken into account
			var targetReferencedAssemblyName = typeof(ProcessNameAttribute).Assembly.GetName().FullName;

			// eagerly compute process names, whose values will be written back to members (that's the wanted side-effect)
			GetType().Assembly.GetReferencedAssemblies()
				.Where(an => an.Name.Contains("Policies"))
				.Select(a => Assembly.Load(a.FullName))
				.Where(a => a.GetReferencedAssemblies().Any(an => an.FullName == targetReferencedAssemblyName))
				.Select(a => a.GetTypes())
				.Select(ProcessNameAttribute.GetProcessNames)
				// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
				.ToArray();
		}

		// ReSharper disable once MemberCanBePrivate.Global
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		protected DateTime StartTime { get; private set; }

		protected MessageBoxServiceInstance[] BizTalkServiceInstances
		{
			get { return GetRunningOrSuspendedServiceInstances(new BizTalkOperations()).ToArray(); }
		}

		/// <summary>
		/// Input system (e.g. Claim Check) folders where input files are dropped.
		/// </summary>
		/// <remarks>
		/// <para>
		/// All folders, whether <see cref="SystemInputFolders"/>, <see cref="SystemOutputFolders"/>, <see
		/// cref="InputFolders"/>, or <see cref="OutputFolders"/> will always be created if necessary. Besides all output
		/// folders, that is <see cref="SystemOutputFolders"/> and <see cref="OutputFolders"/>, will be emptied
		/// immediately before each test is run, while all input folders, that is <see cref="SystemInputFolders"/> and
		/// <see cref="InputFolders"/>, will be emptied immediately after each test has run.
		/// </para>
		/// <para>
		/// Any override must call its base class overridden <see cref="SystemInputFolders"/> member so that not any
		/// system folder is ever missing in the list.
		/// </para>
		/// </remarks>
		protected internal virtual IEnumerable<string> SystemInputFolders
		{
			get { return Enumerable.Empty<string>(); }
		}

		/// <summary>
		/// Output system (e.g. Claim Check) folders where output files are dropped.
		/// </summary>
		/// <remarks>
		/// <para>
		/// All folders, whether <see cref="SystemInputFolders"/>, <see cref="SystemOutputFolders"/>, <see
		/// cref="InputFolders"/>, or <see cref="OutputFolders"/> will always be created if necessary. Besides all output
		/// folders, that is <see cref="SystemOutputFolders"/> and <see cref="OutputFolders"/>, will be emptied
		/// immediately before each test is run, while all input folders, that is <see cref="SystemInputFolders"/> and
		/// <see cref="InputFolders"/>, will be emptied immediately after each test has run.
		/// </para>
		/// <para>
		/// Any override must call its base class overridden <see cref="SystemOutputFolders"/> member so that not any
		/// system folder is ever missing in the list.
		/// </para>
		/// </remarks>
		protected internal virtual IEnumerable<string> SystemOutputFolders
		{
			get { return Enumerable.Empty<string>(); }
		}

		/// <summary>
		/// Input folders where input files are dropped.
		/// </summary>
		/// <remarks>
		/// <para>
		/// All folders, whether <see cref="SystemInputFolders"/>, <see cref="SystemOutputFolders"/>, <see
		/// cref="InputFolders"/>, or <see cref="OutputFolders"/> will always be created if necessary. Besides all output
		/// folders, that is <see cref="SystemOutputFolders"/> and <see cref="OutputFolders"/>, will be emptied
		/// immediately before each test is run, while all input folders, that is <see cref="SystemInputFolders"/> and
		/// <see cref="InputFolders"/>, will be emptied immediately after each test has run.
		/// </para>
		/// <para>
		/// It is not required that an override call its base class overridden <see cref="InputFolders"/> member; that is
		/// why the <see cref="SystemInputFolders"/> has been made available in the first place: to limit the burden on
		/// the unit test developers.
		/// </para>
		/// </remarks>
		protected virtual IEnumerable<string> InputFolders
		{
			get { return Enumerable.Empty<string>(); }
		}

		/// <summary>
		/// Output folders where output files are dropped.
		/// </summary>
		/// <remarks>
		/// <para>
		/// All folders, whether <see cref="SystemInputFolders"/>, <see cref="SystemOutputFolders"/>, <see
		/// cref="InputFolders"/>, or <see cref="OutputFolders"/> will always be created if necessary. Besides all output
		/// folders, that is <see cref="SystemOutputFolders"/> and <see cref="OutputFolders"/>, will be emptied
		/// immediately before each test is run, while all input folders, that is <see cref="SystemInputFolders"/> and
		/// <see cref="InputFolders"/>, will be emptied immediately after each test has run.
		/// </para>
		/// <para>
		/// It is not required that an override call its base class overridden <see cref="OutputFolders"/> member; that is
		/// why the <see cref="SystemOutputFolders"/> has been made available in the first place: to limit the burden on
		/// the unit test developers.
		/// </para>
		/// </remarks>
		protected virtual IEnumerable<string> OutputFolders
		{
			get { return Enumerable.Empty<string>(); }
		}

		private IEnumerable<string> AllFolders
		{
			get { return AllInputFolders.Concat(AllOutputFolders); }
		}

		private IEnumerable<string> AllInputFolders
		{
			get { return SystemInputFolders.Concat(InputFolders); }
		}

		private IEnumerable<string> AllOutputFolders
		{
			get { return SystemOutputFolders.Concat(OutputFolders); }
		}

		/// <summary>
		/// Terminate all BizTalk service instances that are still running or that have been suspended.
		/// </summary>
		protected void TerminateUncompletedBizTalkServiceInstances()
		{
			var bizTalkOperations = new BizTalkOperations();
			GetRunningOrSuspendedServiceInstances(bizTalkOperations)
				.Select(i => new { ServiceInstance = i, CompletionStatus = bizTalkOperations.TerminateInstance(i.ID) })
				.Where(sd => sd.CompletionStatus != CompletionStatus.Succeeded)
				.Each(
					(idx, sd) => {
						Trace.TraceWarning("Could not terminate the BizTalk service instance with ID {0}", sd.ServiceInstance.ID);
						_logger.WarnFormat(
							"[{0,2}] Could not terminate the BizTalk service instance class: {1}\r\n     ServiceType: {2}\r\n     Creation Time: {3}\r\n     Status: {4}\r\n     Error: {5}\r\n",
							idx,
							sd.ServiceInstance.Class,
							sd.ServiceInstance.ServiceType,
							sd.ServiceInstance.CreationTime,
							sd.ServiceInstance.InstanceStatus,
							sd.ServiceInstance.ErrorDescription);
					}
				);
		}

		private void CleanFolders(IEnumerable<string> folders)
		{
			folders.Each(
				d => {
					_logger.DebugFormat("Emptying folder '{0}'.", d);
					Directory.GetFiles(d).Each(
						f => {
							var attempts = 0;
							while (File.Exists(f))
							{
								try
								{
									attempts++;
									_logger.DebugFormat("Attempting to delete file '{0}'.", f);
									File.Delete(f);
								}
								catch (Exception exception)
								{
									_logger.DebugFormat("Exception encountered while attempting to delete file '{0}'. {1}", f, exception);
									if (attempts == 5)
										throw;
									Thread.Sleep(TimeSpan.FromSeconds(1));
								}
							}
						});
					CleanFolders(Directory.GetDirectories(d));
				});
		}

		private IEnumerable<MessageBoxServiceInstance> GetRunningOrSuspendedServiceInstances(BizTalkOperations bizTalkOperations)
		{
			return bizTalkOperations
				.GetServiceInstances().OfType<MessageBoxServiceInstance>()
				.Where(i => (i.InstanceStatus & (InstanceStatus.RunningAll | InstanceStatus.SuspendedAll)) != InstanceStatus.None);
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(ProcessFixture));
	}
}
