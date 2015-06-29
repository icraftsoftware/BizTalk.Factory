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

namespace Be.Stateless.BizTalk.Message.Extensions
{
	public static class PortTypeExtensions
	{
		public static bool IsOneWay(this PortType portType)
		{
			return portType == PortType.OneWayReceivePort || portType == PortType.OneWaySendPort;
		}

		public static bool IsTwoWay(this PortType portType)
		{
			return portType == PortType.RequestResponseReceivePort || portType == PortType.SolicitResponseSendPort;
		}

		public static bool IsSolicitResponse(this PortType portType)
		{
			return portType == PortType.SolicitResponseSendPort;
		}

		public static bool IsRequestResponse(this PortType portType)
		{
			return portType == PortType.RequestResponseReceivePort;
		}
	}
}