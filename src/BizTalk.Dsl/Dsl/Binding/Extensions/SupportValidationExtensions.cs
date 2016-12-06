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
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Extensions
{
	internal static class SupportValidationExtensions
	{
		internal static void Validate(this ISupportValidation validating, string subject)
		{
			try
			{
				validating.Validate();
			}
			catch (Exception exception)
			{
				if (exception.IsFatal()) throw;
				throw new BindingException(string.Format("{0} is not valid: {1}.", subject, exception.Message), exception);
			}
		}
	}
}
