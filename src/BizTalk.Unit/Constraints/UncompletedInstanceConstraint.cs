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

using System.Collections.Generic;
using System.Linq;
using Be.Stateless.Linq.Extensions;
using Microsoft.BizTalk.Operations;
using NUnit.Framework.Constraints;

namespace Be.Stateless.BizTalk.Unit.Constraints
{
	public class UncompletedInstanceConstraint : Constraint
	{
		#region Base Class Member Overrides

		public override bool Matches(object actualObject)
		{
			actual = actualObject;
			var enumerable = actualObject as IEnumerable<MessageBoxServiceInstance>;
			return enumerable != null && enumerable.Any();
		}

		public override void WriteActualValueTo(MessageWriter writer)
		{
			var enumerable = actual as IEnumerable<MessageBoxServiceInstance>;
			if (enumerable == null)
			{
				base.WriteActualValueTo(writer);
			}
			else
			{
				writer.Write(writer.NewLine);
				enumerable.Each(
					(i, si) => writer.Write(
						"[{0,2}] Class: {1}\r\n     ServiceType: {2}\r\n     Creation Time: {3}\r\n     Status: {4}\r\n     Error: {5}\r\n",
						i,
						si.Class,
						si.ServiceType,
						si.CreationTime,
						si.InstanceStatus,
						si.ErrorDescription));
			}
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("any uncompleted BizTalk service instance.");
		}

		#endregion
	}
}
