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
using Be.Stateless.BizTalk.Unit.ServiceModel;
using Be.Stateless.BizTalk.Unit.Transform;
using Be.Stateless.ServiceModel.Channels;
using Be.Stateless.Xml.Xsl;

namespace Be.Stateless.ServiceModel
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class CalculatorService : ServiceRelay, ICalculatorService, IPerformService, ITranslatingCalculatorService, IValidatingCalculatorService
	{
		public CalculatorService() : base(StubServiceHost.DefaultBinding, StubServiceHost.DefaultEndpointAddress) { }

		#region ICalculatorService Members

		public XmlCalculatorResponse Add(XmlCalculatorRequest request)
		{
			return RelayRequest<XmlCalculatorRequest, XmlCalculatorResponse>(request);
		}

		public XmlCalculatorResponse Subtract(XmlCalculatorRequest request)
		{
			return RelayRequest<XmlCalculatorRequest, XmlCalculatorResponse>(request, TimeSpan.FromMilliseconds(100));
		}

		public IAsyncResult BeginMultiply(XmlCalculatorRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return BeginRelayRequest(request, asyncCallback, asyncState);
		}

		public XmlCalculatorResponse EndMultiply(IAsyncResult asyncResult)
		{
			return EndRelayRequest<XmlCalculatorResponse>(asyncResult);
		}

		public IAsyncResult BeginDivide(XmlCalculatorRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return BeginRelayRequest(request, TimeSpan.FromMilliseconds(100), asyncCallback, asyncState);
		}

		public XmlCalculatorResponse EndDivide(IAsyncResult asyncResult)
		{
			return EndRelayRequest<XmlCalculatorResponse>(asyncResult);
		}

		#endregion

		#region IPerformService Members

		public void Perform(XlangCalculatorRequest request)
		{
			RelayRequest<XlangCalculatorRequest, EmptyXmlMessage>(request, new XlangTranslator<IdentityTransform, IdentityTransform>());
		}

		public IAsyncResult BeginProcess(XlangCalculatorRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return BeginRelayRequest(request, new XlangTranslator<IdentityTransform, IdentityTransform>(), asyncCallback, asyncState);
		}

		public void EndProcess(IAsyncResult asyncResult)
		{
			EndRelayRequest<EmptyXmlMessage>(asyncResult);
		}

		#endregion

		#region ITranslatingCalculatorService Members

		public XlangCalculatorResponse Subtract(XlangCalculatorRequest request)
		{
			return RelayRequest<XlangCalculatorRequest, XlangCalculatorResponse>(request, new XlangTranslator<IdentityTransform, IdentityTransform>());
		}

		public IAsyncResult BeginDivide(XlangCalculatorRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return BeginRelayRequest(request, new XlangTranslator<IdentityTransform, IdentityTransform>(), asyncCallback, asyncState);
		}

		XlangCalculatorResponse ITranslatingCalculatorService.EndDivide(IAsyncResult asyncResult)
		{
			return EndRelayRequest<XlangCalculatorResponse>(asyncResult);
		}

		#endregion

		#region IValidatingCalculatorService Members

		public XlangCalculatorResponse Add(XlangCalculatorRequest request)
		{
			return RelayRequest<XlangCalculatorRequest, XlangCalculatorResponse>(request);
		}

		public IAsyncResult BeginMultiply(XlangCalculatorRequest request, AsyncCallback asyncCallback, object asyncState)
		{
			return BeginRelayRequest(request, asyncCallback, asyncState);
		}

		XlangCalculatorResponse IValidatingCalculatorService.EndMultiply(IAsyncResult asyncResult)
		{
			return EndRelayRequest<XlangCalculatorResponse>(asyncResult);
		}

		#endregion

		public const string URI = "http://localhost:8001/calculator";
	}
}
