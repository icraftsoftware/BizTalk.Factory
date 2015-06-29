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
	/// Validates that a <see cref="ConfigurationElementCollection"/>-derived collection object is not empty.
	/// </summary>
	public class CollectionValidator : ConfigurationValidatorBase
	{
		#region Base Class Member Overrides

		/// <summary>
		/// Determines whether the type of the object can be validated.
		/// </summary>
		/// <param name="type">
		/// The type of object.
		/// </param>
		/// <returns>
		/// <c>true</c> if the <paramref name="type" /> parameter is a <see cref="string"/>; otherwise, <c>false</c>. 
		/// </returns>
		public override bool CanValidate(Type type)
		{
			return typeof(ConfigurationElementCollection).IsAssignableFrom(type);
		}

		/// <summary>
		/// Determines whether the value of an object is valid.
		/// </summary>
		/// <param name="value">
		/// The value of an object.
		/// </param>
		public override void Validate(object value)
		{
			var collection = (ConfigurationElementCollection) value;
			if (collection.Count == 0)
				throw new ConfigurationErrorsException(
					string.Format("{0} collection contains no items.", value.GetType().Name),
					collection.ElementInformation.Source,
					collection.ElementInformation.LineNumber);
		}

		#endregion
	}
}
