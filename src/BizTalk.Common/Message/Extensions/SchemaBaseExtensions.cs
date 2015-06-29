#region Copyright & License

// Copyright © 2013 François Chabot, Yves Dierick
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
using Be.Stateless.BizTalk.Runtime.Caching;
using Be.Stateless.BizTalk.Schema;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	public static class SchemaBaseExtensions
	{
		/// <summary>
		/// Returns whether the <paramref name="type"/> is a <see cref="SchemaBase"/>-derived schema type.
		/// </summary>
		/// <param name="type">
		/// The <see cref="Type"/> to inspect.
		/// </param>
		/// <returns>
		/// <c>true</c> if <paramref name="type"/> is a <see cref="SchemaBase"/>-derived <see cref="Type"/>s.
		/// </returns>
		public static bool IsSchema(this Type type)
		{
			return (type != null && type.BaseType == typeof(SchemaBase));
		}

		/// <summary>
		/// Metadata for <see cref="SchemaBase"/>-derived <see cref="Type"/>s.
		/// </summary>
		/// <param name="type">
		/// The <see cref="SchemaBase"/>-derived <see cref="Type"/>.
		/// </param>
		/// <returns>
		/// The <see cref="ISchemaMetadata"/> pieces of information related to the <see cref="SchemaBase"/>-derived <see
		/// cref="Type"/>.
		/// </returns>
		/// <remarks>
		/// The purpose of this extension is to make <see cref="SchemaBase"/>-derived <see cref="Type"/>'s extension
		/// methods amenable to mocking, <see href="http://blogs.clariusconsulting.net/kzu/how-to-mock-extension-methods/"/>.
		/// </remarks>
		/// <seealso href="http://blogs.clariusconsulting.net/kzu/how-extension-methods-ruined-unit-testing-and-oop-and-a-way-forward/"/>
		/// <seealso href="http://blogs.clariusconsulting.net/kzu/making-extension-methods-amenable-to-mocking/"/>
		public static ISchemaMetadata GetMetadata(this Type type)
		{
			if (!type.IsSchema()) throw new ArgumentException("Type is not a SchemaBase derived Type instance.", "type");
			return _schemaMetadataFactory(type);
		}

		#region Mock's Factory Hook Point

		internal static Func<Type, ISchemaMetadata> SchemaMetadataFactory
		{
			get { return _schemaMetadataFactory; }
			set { _schemaMetadataFactory = value; }
		}

		#endregion

		private static Func<Type, ISchemaMetadata> _schemaMetadataFactory = type => SchemaMetadataCache.Instance[type];
	}
}
