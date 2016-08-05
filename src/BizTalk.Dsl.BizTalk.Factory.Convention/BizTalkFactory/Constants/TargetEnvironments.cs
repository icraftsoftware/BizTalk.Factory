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

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention.BizTalkFactory.Constants
{
	public static class TargetEnvironment
	{
		public static bool IsDevelopment(this string environment)
		{
			return Equals(environment, DEVELOPMENT);
		}

		public static bool IsBuild(this string environment)
		{
			return Equals(environment, BUILD);
		}

		public static bool IsAcceptance(this string environment)
		{
			return Equals(environment, ACCEPTANCE);
		}

		public static bool IsProduction(this string environment)
		{
			return Equals(environment, PRODUCTION);
		}

		public const string ACCEPTANCE = "ACC";
		public const string BUILD = "BLD";
		public const string DEVELOPMENT = "DEV";
		public const string PRODUCTION = "PRD";
	}
}
