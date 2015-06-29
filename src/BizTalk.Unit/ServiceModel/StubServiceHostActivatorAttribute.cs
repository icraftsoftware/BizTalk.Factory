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
using System.ServiceModel;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	/// <summary>
	/// Activates the <see cref="SimpleServiceHost{StubService, IStubService}"/> for the <see cref="StubService"/>
	/// service, i.e. the in-process host for the <see cref="IStubService"/> service meant to facilitate the NUnit-based
	/// unit testing of WCF service contracts.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="StubServiceHostActivatorAttribute"/> ensures that the <see
	/// cref="SimpleServiceHost{StubService,IStubService}"/> for the <see cref="StubService"/> service's endpoint is
	/// listening and started before any NUnit-test that depends on it runs. Making use of this attribute therefore
	/// assumes and requires that unit tests are written on the basis of the NUnit testing framework.
	/// </para>
	/// <para>
	/// By default, the in-process <see cref="SimpleServiceHost{StubService, IStubService}"/> for the <see
	/// cref="StubService"/> service's endpoint will be exposed over the <see cref="BasicHttpBinding"/> binding at the
	/// <c>http://localhost:8000/services</c> address.
	/// </para>
	/// </remarks>
	/// <seealso cref="SimpleServiceHost{TService,TChannel}"/>
	/// <seealso cref="ITestAction"/>
	/// <seealso cref="TestActionAttribute"/>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
	public class StubServiceHostActivatorAttribute : TestActionAttribute
	{
		#region Base Class Member Overrides

		public override ActionTargets Targets
		{
			get { return ActionTargets.Suite | ActionTargets.Test; }
		}

		public override void AfterTest(TestDetails testDetails)
		{
			if (testDetails.IsSuite)
			{
				StubServiceHost.DefaultInstance.Recycle();
			}
		}

		public override void BeforeTest(TestDetails testDetails)
		{
			if (testDetails.IsSuite)
			{
				StubServiceHost.DefaultInstance.Open();
			}
			else
			{
				StubServiceHost.DefaultService.ClearSetups();
			}
		}

		#endregion
	}
}
