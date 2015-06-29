#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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

using Be.Stateless.BizTalk.Message.Extensions;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Dsl.RuleEngine
{
	public abstract class RuleSetBase
	{
		#region Nested Type: Schema<T>

		/// <summary>
		/// Allows to write rules in terms of BizTalk SchemaBase artifacts.
		/// </summary>
		/// <remarks>
		/// This class is just syntactic sugar to support the fluent rule DSL. It is only used at install time when
		/// populating the rule store (i.e. when translating a DSL rule into a BRE rule).
		/// </remarks>
		protected internal static class Schema<T> where T : Microsoft.XLANGs.BaseTypes.SchemaBase
		{
			public static string AssemblyQualifiedName
			{
				get { return typeof(T).AssemblyQualifiedName; }
			}

			public static string MessageType
			{
				get { return typeof(T).GetMessageType(); }
			}
		}

		#endregion

		#region Nested Type: Transform<T>

		/// <summary>
		/// Allows to write rules in terms of BizTalk TransformBase artifacts.
		/// </summary>
		/// <remarks>
		/// This class is just syntactic sugar to support the fluent rule DSL. It is only used at install time when
		/// populating the rule store (i.e. when translating a DSL rule into a BRE rule).
		/// </remarks>
		protected internal static class Transform<T> where T : TransformBase
		{
			public static string MapTypeName
			{
				get { return typeof(T).AssemblyQualifiedName; }
			}
		}

		#endregion
	}
}
