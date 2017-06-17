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

namespace Be.Stateless.Quartz
{
	/// <summary>
	/// This class allows storing the fully qualified name of Quartz job classes in the configuration database. This is
	/// necessary to allow Quartz to load those classes when they are in an assembly deployed in the GAC.
	/// </summary>
	public class SqlServerDelegate : global::Quartz.Impl.AdoJobStore.SqlServerDelegate
	{
		#region Base Class Member Overrides

		[SuppressMessage("ReSharper", "IdentifierTypo")]
		protected override string GetStorableJobTypeName(Type jobType)
		{
			return jobType.AssemblyQualifiedName;
		}

		#endregion
	}
}
