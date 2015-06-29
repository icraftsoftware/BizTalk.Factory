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

namespace Be.Stateless.BizTalk.Dsl.RuleEngine
{
	// ReSharper disable UnusedAutoPropertyAccessor.Global
	// ReSharper disable UnassignedField.Global
	// ReSharper disable InconsistentNaming
	// ReSharper disable FieldCanBeMadeReadOnly.Global
	// ReSharper disable ConvertToConstant.Global
	public static class TestProcesses
	{
		[ProcessName]
		public static string One { get; set; }

		public const string Three = "Be.Stateless.BizTalk.RuleEngine.Dsl.TestProcesses.Three";

		[ProcessName]
		public static string Two;

		public static string Four = "Be.Stateless.BizTalk.RuleEngine.Dsl.TestProcesses.Four";
	}
}
