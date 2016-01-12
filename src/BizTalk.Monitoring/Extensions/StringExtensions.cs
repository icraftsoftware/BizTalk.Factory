#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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

using System.Linq;
using System.Text.RegularExpressions;

namespace Be.Stateless.BizTalk.Monitoring.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Returns a process friendly, or short, name assembled by the concatenation of the Project/Area/Process parts of
		/// a process name.
		/// </summary>
		/// <param name="processName">The comprehensive process name.</param>
		/// <returns>The concatenated Project/Area/Process parts.</returns>
		/// <remarks>
		/// Regex isolation of the following tokens:
		/// <list type="ul">
		///    <item>the Project: e.g Factory</item>
		///    <item>an Area: e.g. Switching</item>
		///    <item>the Process Name: e.g. UpdateMasterData, ReceiveSupplierChangeAcknowledgments, or Failed</item>
		/// </list>
		/// given the following inputs:
		/// <list type="ul">
		///    <item>Be.Stateless.Factory.Orchestrations.Switching.UpdateMasterData</item>
		///    <item>Be.Stateless.Factory.Areas.Switching.UpdateMasterData</item>
		///    <item>Be.Stateless.Factory.Areas.Default.ReceiveSupplierChangeAcknowledgments</item>
		///    <item>Be.Stateless.Factory.Areas.Unknown.Failed</item>
		/// </list>
		/// </remarks>
		public static string ToFriendlyProcessName(this string processName)
		{
			var match = Regex.Match(processName, _friendlyNameRegex, RegexOptions.Compiled);
			var name = _friendlyNameTokens.Skip(1)
				.Select(t => match.Groups[t])
				.Where(g => g.Success)
				.Aggregate(match.Groups[_friendlyNameTokens[0]].Value, (acc, g) => acc + "/" + g.Value);
			return name;
		}

		private static readonly string[] _friendlyNameTokens = { "Project", "Area", "Process" };

		private static readonly string _friendlyNameRegex = string.Format(
			@".+\.(?<{0}>\w+)\.(?:Areas(?:\.Default)?|Orchestrations)(?:\.(?<{1}>\w+))?\.(?<{2}>\w*)",
			// ReSharper disable once CoVariantArrayConversion
			_friendlyNameTokens);
	}
}
