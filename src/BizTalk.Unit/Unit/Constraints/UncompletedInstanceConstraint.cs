#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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
using Microsoft.BizTalk.Operations;
using NUnit.Framework.Constraints;

namespace Be.Stateless.BizTalk.Unit.Constraints
{
	public class UncompletedInstanceConstraint : Constraint
	{
		#region Nested Type: MessageBoxServiceInstanceWrapper

		private class MessageBoxServiceInstanceWrapper
		{
			public MessageBoxServiceInstanceWrapper(MessageBoxServiceInstance messageBoxServiceInstance)
			{
				if (messageBoxServiceInstance == null) throw new ArgumentNullException("messageBoxServiceInstance");
				_messageBoxServiceInstance = messageBoxServiceInstance;
			}

			#region Base Class Member Overrides

			public override string ToString()
			{
				return string.Format(
					"Class: {0}\r\n     ServiceType: {1}\r\n     Creation Time: {2}\r\n     Status: {3}\r\n     Error: {4}\r\n",
					_messageBoxServiceInstance.Class,
					_messageBoxServiceInstance.ServiceType,
					_messageBoxServiceInstance.CreationTime,
					_messageBoxServiceInstance.InstanceStatus,
					_messageBoxServiceInstance.ErrorDescription);
			}

			#endregion

			private readonly MessageBoxServiceInstance _messageBoxServiceInstance;
		}

		#endregion

		#region Base Class Member Overrides

		public override ConstraintResult ApplyTo<TActual>(TActual actual)
		{
			var enumerable = actual as IEnumerable<MessageBoxServiceInstance>;
			if (enumerable == null) return new ConstraintResult(this, actual, false);

			var actualCollection = enumerable.Select(mbsi => new MessageBoxServiceInstanceWrapper(mbsi));
			return new ConstraintResult(this, actualCollection, actualCollection.Any());
		}

		public override string Description
		{
			get { return "any uncompleted BizTalk service instance."; }
		}

		#endregion
	}
}
