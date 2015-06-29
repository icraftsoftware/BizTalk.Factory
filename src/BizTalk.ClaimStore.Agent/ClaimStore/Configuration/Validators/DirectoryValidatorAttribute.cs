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
using System.Configuration;

namespace Be.Stateless.BizTalk.ClaimStore.Configuration.Validators
{
	/// <summary>
	/// Declaratively instructs the .NET Framework to perform a directory existence validation check on a configuration
	/// property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class DirectoryValidatorAttribute : ConfigurationValidatorAttribute
	{
		#region Base Class Member Overrides

		/// <summary>
		/// Gets an instance of the <see cref="DirectoryValidator" /> class.
		/// </summary>
		/// <returns>
		/// The <see cref="DirectoryValidator" /> validator instance.
		/// </returns>
		public override ConfigurationValidatorBase ValidatorInstance
		{
			get { return new DirectoryValidator(); }
		}

		#endregion
	}
}
