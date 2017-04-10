#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Extensions
{
	/// <summary>
	/// Allows to skip <see cref="ICommunicationObject"/> casts when calling either <see
	/// cref="ICommunicationObject.Abort()"/> or <see cref="ICommunicationObject.Close()"/> on a <see
	/// cref="SimpleServiceClient{TService,TChannel}"/> instance.
	/// </summary>
	[SuppressMessage("ReSharper", "LocalizableElement")]
	public static class SimpleServiceClientExtensions
	{
		public static void Abort<TChannel>(this TChannel client) where TChannel : class
		{
			var communicationObject = client as ICommunicationObject;
			if (communicationObject == null) throw new ArgumentException("client does support ICommunicationObject.", "client");
			communicationObject.Abort();
		}

		public static void Close<TChannel>(this TChannel client) where TChannel : class
		{
			var communicationObject = client as ICommunicationObject;
			if (communicationObject == null) throw new ArgumentException("client does support ICommunicationObject.", "client");
			communicationObject.Close();
		}
	}
}
