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

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.Extensions;
using Microsoft.RuleEngine;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Dsl.RuleEngine
{
	/// <summary>
	/// </summary>
	/// <remarks>
	/// To effectively work, invocation of static members of a class must be enabled for the rule engine. See
	/// http://msdn.microsoft.com/en-us/library/aa950269.aspx.
	/// </remarks>
	public class RuleSet : IHideObjectMembers
	{
		#region Nested Type: Context

		/// <summary>
		/// Allows to write rules in terms of properties of the message context.
		/// </summary>
		/// <remarks>
		/// This class is just syntactic sugar to support the fluent rule DSL. It is never used at runtime.
		/// </remarks>
		// TODO translate this class into plugin that can handle its own translation so that RuleTranslator can delegate to it
		// TODO enforce value of the right type when promoting/reading/writing a value for ResolvedProperties.ProcessName
		protected internal static class Context
		{
			public static void Promote<TP>(MessageContextProperty<TP, string> property, string value)
				where TP : MessageContextPropertyBase, new() { }

			public static void Promote<TP, TV>(MessageContextProperty<TP, TV> property, TV value)
				where TP : MessageContextPropertyBase, new()
				where TV : struct { }

			// TODO enforce value of the right type when promoting/reading/writing a Transform<T> for ResolvedProperties.MapTypeName
			//public static void Promote(MessageContextProperty<MapTypeName, string> property, Type value)
			//{
			//}

			// TODO enforce value of the right type when promoting/reading/writing a Schema<T> for BtsProperties.MessageType
			//public static void Promote(MessageContextProperty<MessageType, string> property, Type value)
			//{
			//}

			public static string Read<TP>(MessageContextProperty<TP, string> property)
				where TP : MessageContextPropertyBase, new()
			{
				return null;
			}

			public static TV Read<TP, TV>(MessageContextProperty<TP, TV> property)
				where TP : MessageContextPropertyBase, new()
				where TV : struct
			{
				return default(TV);
			}

			// TODO enforce value of the right type when promoting/reading/writing a Transform<T> for ResolvedProperties.MapTypeName
			// TODO ?? rename MapTypeName to MapType ??
			//public static Type Read(MessageContextProperty<MapTypeName, string> property)
			//{
			//   return null;
			//}

			// TODO enforce value of the right type when promoting/reading/writing a Schema<T> for BtsProperties.MessageType
			//public static Type Read(MessageContextProperty<MessageType, string> property)
			//{
			//   return null;
			//}

			public static void Write<TP>(MessageContextProperty<TP, string> property, string value)
				where TP : MessageContextPropertyBase, new() { }

			public static void Write<TP, TV>(MessageContextProperty<TP, TV> property, TV value)
				where TP : MessageContextPropertyBase, new()
				where TV : struct { }

			// TODO enforce value of the right type when promoting/reading/writing a Transform<T> for ResolvedProperties.MapTypeName
			//public static void Write(MessageContextProperty<MapTypeName, string> property, Type value)
			//{
			//}

			// TODO enforce value of the right type when promoting/reading/writing a Schema<T> for BtsProperties.MessageType
			//public static void Write(MessageContextProperty<MessageType, string> property, Type value)
			//{
			//}
		}

		#endregion

		#region Operators

		public static implicit operator Microsoft.RuleEngine.RuleSet(RuleSet fromRuleSet)
		{
			var toRuleSet = new Microsoft.RuleEngine.RuleSet(fromRuleSet.Name, fromRuleSet.VersionInfo) {
				ExecutionConfiguration = { MaxExecutionLoopDepth = 1 }
			};

			foreach (var rule in (RuleList) fromRuleSet.Rules)
			{
				toRuleSet.Rules.Add(rule);
			}

			return toRuleSet;
		}

		#endregion

		protected RuleSet()
		{
			_rules = new RuleList();
			VersionInfo = new VersionInfo(string.Empty, DateTime.Now, Environment.UserName, 1, 0);
		}

		protected RuleSet(string name) : this()
		{
			if (name.IsNullOrEmpty()) throw new ArgumentNullException("name");
			_name = name;
		}

		protected RuleSet(string name, int majorRevision, int minorRevision)
			: this(name)
		{
			VersionInfo.MajorRevision = majorRevision;
			VersionInfo.MinorRevision = minorRevision;
		}

		#region IHideObjectMembers Members

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("ReSharper", "BaseObjectEqualsIsObjectEquals")]
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
		[SuppressMessage("ReSharper", "BaseObjectGetHashCodeCallInGetHashCode")]
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string ToString()
		{
			return base.ToString();
		}

		#endregion

		public string Name
		{
			get { return _name.IsNullOrEmpty() ? GetType().FullName : _name; }
			set { _name = value; }
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public RuleSetInfo RuleSetInfo
		{
			get
			{
				return new RuleSetInfo(
					Name,
					VersionInfo.MajorRevision,
					VersionInfo.MinorRevision,
					VersionInfo.Description,
					VersionInfo.ModifiedTime,
					VersionInfo.ModifiedBy,
					false);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public VersionInfo VersionInfo { get; set; }

		protected IRuleList Rules
		{
			get { return _rules; }
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public string ToBrl()
		{
			var ms = new MemoryStream();
			new BusinessRulesLanguageConverter().Save(ms, default(VocabularyDictionary), new RuleSetDictionary { this });
			var brl = Encoding.Default.GetString(ms.GetBuffer());
			return brl;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public string ToXml()
		{
			return ToBrl();
		}

		protected IRule Rule(string name)
		{
			return new Rule(name);
		}

		private readonly RuleList _rules;
		private string _name;
	}
}
