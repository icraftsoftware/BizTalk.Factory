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

using System.Linq;
using System.Text.RegularExpressions;

namespace Be.Stateless.BizTalk.Monitoring.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Returns a process friendly, or short, name made from the concatenation of the Project/Area/Process tokens in a
		/// comprehensive process name.
		/// </summary>
		/// <param name="processName">
		/// The comprehensive process name.
		/// </param>
		/// <returns>
		/// The concatenated Project/Area/Process tokens.
		/// </returns>
		/// <remarks>
		/// Tokens are isolated via a regular expression. Any Area named <c>Default</c> is skipped or equivalent to a
		/// <c>null</c>, non-existing, area.
		/// </remarks>
		/// <example>
		/// Given the following comprehensive process names
		/// <list type="bullet">
		/// <item><c>Be.Stateless.BizTalk.Factory.Areas.Default.Failed</c></item>
		/// <item><c>Be.Stateless.BizTalk.Factory.Areas.Default.Unidentified</c></item>
		/// <item><c>Be.Stateless.BizTalk.Factory.Areas.Batch.Aggregate</c></item>
		/// <item><c>Be.Stateless.BizTalk.Factory.Areas.Batch.Release</c></item>
		/// <item><c>Be.Stateless.BizTalk.Factory.Areas.Claim.Check</c></item>
		/// <item><c>Be.Stateless.Accounting.Orchestrations.Invoicing.UpdateLedger</c></item>
		/// <item><c>Be.Stateless.Accounting.Orchestrations.UpdateMasterData</c></item>
		/// </list>
		/// the friendly names are respectively
		/// <list type="bullet">
		/// <item>
		/// <c>Factory/Failed</c> where
		/// <para>
		/// <list type="definition">
		/// <item>
		/// <term>Project</term>
		/// <description><c>Factory</c></description>
		/// </item>
		/// <item>
		/// <term>Area</term>
		/// <description>null</description>
		/// </item>
		/// <item>
		/// <term>Process</term>
		/// <description><c>Failed</c></description>
		/// </item>
		/// </list>
		/// </para>
		/// </item>
		/// <item>
		/// <c>Factory/Unidentified</c> where
		/// <para>
		/// <list type="definition">
		/// <item>
		/// <term>Project</term>
		/// <description><c>Factory</c></description>
		/// </item>
		/// <item>
		/// <term>Area</term>
		/// <description>null</description>
		/// </item>
		/// <item>
		/// <term>Process</term>
		/// <description><c>Unidentified</c></description>
		/// </item>
		/// </list>
		/// </para>
		/// </item>
		/// <item>
		/// <c>Factory/Batch/Aggregate</c> where
		/// <para>
		/// <list type="definition">
		/// <item>
		/// <term>Project</term>
		/// <description><c>Factory</c></description>
		/// </item>
		/// <item>
		/// <term>Area</term>
		/// <description><c>Batch</c></description>
		/// </item>
		/// <item>
		/// <term>Process</term>
		/// <description><c>Aggregate</c></description>
		/// </item>
		/// </list>
		/// </para>
		/// </item>
		/// <item>
		/// <c>Factory/Batch/Release</c> where
		/// <para>
		/// <list type="definition">
		/// <item>
		/// <term>Project</term>
		/// <description><c>Factory</c></description>
		/// </item>
		/// <item>
		/// <term>Area</term>
		/// <description><c>Batch</c></description>
		/// </item>
		/// <item>
		/// <term>Process</term>
		/// <description><c>Release</c></description>
		/// </item>
		/// </list>
		/// </para>
		/// </item>
		/// <item>
		/// <c>Factory/Claim/Check</c> where
		/// <para>
		/// <list type="definition">
		/// <item>
		/// <term>Project</term>
		/// <description><c>Factory</c></description>
		/// </item>
		/// <item>
		/// <term>Area</term>
		/// <description><c>Claim</c></description>
		/// </item>
		/// <item>
		/// <term>Process</term>
		/// <description><c>Check</c></description>
		/// </item>
		/// </list>
		/// </para>
		/// </item>
		/// <item>
		/// <c>Accounting/Invoicing/UpdateLedger</c> where
		/// <para>
		/// <list type="definition">
		/// <item>
		/// <term>Project</term>
		/// <description><c>Accounting</c></description>
		/// </item>
		/// <item>
		/// <term>Area</term>
		/// <description><c>Invoicing</c></description>
		/// </item>
		/// <item>
		/// <term>Process</term>
		/// <description><c>UpdateLedger</c></description>
		/// </item>
		/// </list>
		/// </para>
		/// </item>
		/// <item>
		/// <c>Accounting/UpdateMasterData</c> where
		/// <para>
		/// <list type="definition">
		/// <item>
		/// <term>Project</term>
		/// <description><c>Accounting</c></description>
		/// </item>
		/// <item>
		/// <term>Area</term>
		/// <description>null</description>
		/// </item>
		/// <item>
		/// <term>Process</term>
		/// <description><c>UpdateMasterData</c></description>
		/// </item>
		/// </list>
		/// </para>
		/// </item>
		/// </list>
		/// </example>
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
