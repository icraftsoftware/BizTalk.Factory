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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Be.Stateless.BizTalk.Dsl.RuleEngine.Extensions;

namespace Be.Stateless.BizTalk.Dsl.RuleEngine
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class ProcessNameAttribute : Attribute
	{
		public static string[] GetProcessNames(IEnumerable<Type> processDescriptors)
		{
			// ReSharper disable once PossibleMultipleEnumeration
			return processDescriptors
				.Where(t => t.IsProcessDescriptor())
				.Select(pd => pd.GetProcessNames())
				.SelectMany(pn => pn)
				.ToArray();
		}
	}

	namespace Extensions
	{
		public static class MemberInfoExtensions
		{
			public static string GetProcessName(this MemberInfo memberInfo)
			{
				if (memberInfo == null) throw new ArgumentNullException("memberInfo");
				var declaringType = memberInfo.DeclaringType;
				if (declaringType == null) throw new ArgumentException("MemberInfo.DeclaringType is null.", "memberInfo");

				if (!declaringType.IsStatic())
					throw new NotSupportedException(
						string.Format(
							"{0} is not static, {1} can only be applied to members of static classes.",
							declaringType.Name,
							typeof(ProcessNameAttribute).Name));
				if (!memberInfo.IsQualifiedByProcessNameAttribute())
					throw new NotSupportedException(
						string.Format(
							"{0} has not been qualified, process names can only be reflected from members upon which the {1} has been applied.",
							memberInfo.Name,
							typeof(ProcessNameAttribute).Name));
				if (memberInfo.GetValueType() != typeof(string))
					throw new NotSupportedException(
						string.Format(
							"{0} is not of string type, process names can only be reflected from string members.",
							memberInfo.Name));
				var processName = declaringType.FullName + "." + memberInfo.Name;
				// assigning the computed process name back to its declaring member field or property
				// at least so that one can intuitively write unit test assertions using this member accessor
				declaringType.InvokeMember(memberInfo.Name, BindingFlags.SetField | BindingFlags.SetProperty, null, null, new object[] { processName });
				return processName;
			}

			public static bool IsProcessName(this MemberInfo memberInfo)
			{
				if (memberInfo == null) throw new ArgumentNullException("memberInfo");
				return memberInfo.DeclaringType.IsStatic()
					&& memberInfo.IsQualifiedByProcessNameAttribute()
					&& memberInfo.GetValueType() == typeof(string);
			}

			private static Type GetValueType(this MemberInfo memberInfo)
			{
				switch (memberInfo.MemberType)
				{
					case (MemberTypes.Field):
						return ((FieldInfo) memberInfo).FieldType;
					case (MemberTypes.Property):
						return ((PropertyInfo) memberInfo).PropertyType;
					default:
						// can never occur as ProcessNameAttribute's AttributeUsage is Property or Field
						throw new NotSupportedException();
				}
			}

			private static bool IsQualifiedByProcessNameAttribute(this MemberInfo memberInfo)
			{
				return memberInfo.IsDefined(typeof(ProcessNameAttribute), false);
			}

			private static bool IsStatic(this Type declaringType)
			{
				return declaringType.IsAbstract && declaringType.IsSealed;
			}
		}

		public static class TypeExtensions
		{
			public static bool IsProcessDescriptor(this Type type)
			{
				if (type == null) throw new ArgumentNullException("type");

				// being a class and static (i.e. IsAbstract && IsSealed)
				return type.IsClass && type.IsAbstract && type.IsSealed
					// and having any ProcessNameAttribute-qualified member
					&& type.GetMembers().Any(m => m.IsProcessName());
			}

			public static IEnumerable<string> GetProcessNames(this Type processDescriptor)
			{
				if (processDescriptor == null) throw new ArgumentNullException("processDescriptor");
				if (!processDescriptor.IsProcessDescriptor())
					throw new ArgumentException(
						string.Format(
							"{0} does not declare any {1}-qualified property or field.",
							processDescriptor.Name,
							typeof(ProcessNameAttribute).Name),
						"processDescriptor");

				// all ProcessNameAttribute-qualified properties and fields
				var members = processDescriptor.GetProperties()
					.Union(processDescriptor.GetFields().Cast<MemberInfo>());
				var processNames = members
					.Where(mi => mi.IsProcessName())
					.Select(mi => mi.GetProcessName());

				return processNames;
			}
		}
	}
}
