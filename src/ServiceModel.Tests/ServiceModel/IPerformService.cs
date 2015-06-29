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
using Be.Stateless.ServiceModel.Channels;

namespace Be.Stateless.ServiceModel
{
	[ServiceContract(Namespace = "urn:services.stateless.be:unit:calculator", Name = "PerformService")]
	[XmlSerializerFormat]
	public interface IPerformService
	{
		[OperationContract(Action = "Perform", ReplyAction = "Perform/Response")]
		void Perform([MessageParameter(Name = "CalculatorRequest")] XlangCalculatorRequest request);

		[OperationContract(AsyncPattern = true, Action = "Process", ReplyAction = "Process/Response")]
		IAsyncResult BeginProcess(
			[MessageParameter(Name = "CalculatorRequest")] XlangCalculatorRequest request,
			AsyncCallback asyncCallback,
			object asyncState);

		void EndProcess(IAsyncResult asyncResult);
	}

	/// <summary>
	/// Client-side version of the <see cref="IPerformService"/> contract that allow for easier setup of the <see
	/// cref="StubService"/> as all asynchronous operations have been translated into their analogous synchronous
	/// counter-part, not differently from what svcutil.exe would do when generating a service proxy/client code.
	/// </summary>
	[ServiceContract(Namespace = "urn:services.stateless.be:unit:calculator", Name = "PerformService")]
	[XmlSerializerFormat]
	public interface IPerformServiceSync
	{
		[OperationContract(Action = "Process", ReplyAction = "Process/Response")]
		void Process([MessageParameter(Name = "CalculatorRequest")] XlangCalculatorRequest request);
	}
}
