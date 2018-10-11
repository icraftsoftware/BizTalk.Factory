#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using Be.Stateless.BizTalk.Management;
using Be.Stateless.BizTalk.Operations.Extensions;
using Be.Stateless.Linq.Extensions;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Operations;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.Process
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public abstract class ProcessFixture
	{
		protected internal virtual IEnumerable<Type> AllDependantOrchestrationTypes
		{
			get { return DependantOrchestrationTypes; }
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

		protected IEnumerable<MessageBoxServiceInstance> BizTalkServiceInstances
		{
			get { return BizTalkOperationsExtensions.GetRunningOrSuspendedServiceInstances(); }
		}

		/// <summary>
		/// Ordered list of orchestration types that this process depends upon.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Dependant orchestrations will be started before any test method of this process is run; notice that they will
		/// be started in the order in which they were given. Likewise, dependant orchestrations will be unenlisted after
		/// all of the test methods of this process has run; notice that they will be unenlisted in the reverse order in
		/// which they were given.
		/// </para>
		/// <para>
		/// Any service instance of any one of the dependant orchestrations will be terminated after each test method of
		/// this process has run should the service instance be suspended or still be running.
		/// </para>
		/// </remarks>
		protected virtual IEnumerable<Type> DependantOrchestrationTypes
		{
			get { return Enumerable.Empty<Type>(); }
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

		protected DateTime StartTime { get; private set; }

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
			BizTalkOperationsExtensions.TerminateUncompletedBizTalkServiceInstances();
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
									if (attempts == 5) throw;
									Thread.Sleep(TimeSpan.FromSeconds(1));
								}
							}
						});
					CleanFolders(Directory.GetDirectories(d));
				});
		}

		[OneTimeSetUp]
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
			AllDependantOrchestrationTypes.Each(ot => { new Orchestration(ot).EnsureStarted(); });
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

		[OneTimeTearDown]
		public void BizTalkFactoryOrchestrationFixtureOneTimeTearDown()
		{
			// reverse the list to stop the dependant orchestrations
			AllDependantOrchestrationTypes.Reverse().Each(ot => { new Orchestration(ot).EnsureUnenlisted(); });
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(ProcessFixture));
	}
}
