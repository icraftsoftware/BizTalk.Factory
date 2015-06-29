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
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Component.Interop
{
	/// <summary>
	/// <see cref="IPropertyBag"/> extension methods to read and write individual properties in a type-safe way.
	/// </summary>
	public static class PropertyBag
	{
		/// <summary>
		/// Read and assign an <see cref="Enum"/> property value.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="Enum"/> type of the property.
		/// </typeparam>
		/// <param name="propertyBag">
		/// The <see cref="IPropertyBag"/> bag instance.
		/// </param>
		/// <param name="name">
		/// The name of the property to read from the <see cref="IPropertyBag"/> bag.
		/// </param>
		/// <param name="setter">
		/// The property setter to assign the value read from the <see cref="IPropertyBag"/> bag if not null.
		/// </param>
		public static void ReadProperty<T>(this IPropertyBag propertyBag, string name, Action<T> setter) where T : struct
		{
			PropertyHelper.ReadPropertyBag(propertyBag, name).IfNotNull(value => setter((T) Enum.Parse(typeof(T), (string) value)));
		}

		/// <summary>
		/// Read a <see cref="bool"/> property value.
		/// </summary>
		/// <param name="propertyBag">
		/// The <see cref="IPropertyBag"/> bag instance.
		/// </param>
		/// <param name="name">
		/// The name of the property to read from the <see cref="IPropertyBag"/> bag.
		/// </param>
		/// <param name="setter">
		/// The property setter to assign the value read from the <see cref="IPropertyBag"/> bag if not null.
		/// </param>
		public static void ReadProperty(this IPropertyBag propertyBag, string name, Action<bool> setter)
		{
			PropertyHelper.ReadPropertyBag(propertyBag, name).IfNotNull(value => setter((bool) value));
		}

		/// <summary>
		/// Read a <see cref="int"/> property value.
		/// </summary>
		/// <param name="propertyBag">
		/// The <see cref="IPropertyBag"/> bag instance.
		/// </param>
		/// <param name="name">
		/// The name of the property to read from the <see cref="IPropertyBag"/> bag.
		/// </param>
		/// <param name="setter">
		/// The property setter to assign the value read from the <see cref="IPropertyBag"/> bag if not null.
		/// </param>
		public static void ReadProperty(this IPropertyBag propertyBag, string name, Action<int> setter)
		{
			PropertyHelper.ReadPropertyBag(propertyBag, name).IfNotNull(value => setter((int) value));
		}

		/// <summary>
		/// Read a <see cref="string"/> property value.
		/// </summary>
		/// <param name="propertyBag">
		/// The <see cref="IPropertyBag"/> bag instance.
		/// </param>
		/// <param name="name">
		/// The name of the property to read from the <see cref="IPropertyBag"/> bag.
		/// </param>
		/// <param name="setter">
		/// The property setter to assign the value read from the <see cref="IPropertyBag"/> bag if not null nor empty.
		/// </param>
		public static void ReadProperty(this IPropertyBag propertyBag, string name, Action<string> setter)
		{
			var value = PropertyHelper.ReadPropertyBag(propertyBag, name).IfNotNull(@string => @string as string);
			if (!value.IsNullOrEmpty()) setter(value);
		}

		/// <summary>
		/// Write an <see cref="Enum"/> property value to the <see cref="IPropertyBag"/> bag.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="Enum"/> type of the property.
		/// </typeparam>
		/// <param name="propertyBag">
		/// The <see cref="IPropertyBag"/> bag instance.
		/// </param>
		/// <param name="name">
		/// The name of the property to write to the <see cref="IPropertyBag"/> bag.
		/// </param>
		/// <param name="value">
		/// The <see cref="Enum"/> property value to write to the <see cref="IPropertyBag"/> bag.
		/// </param>
		public static void WriteProperty<T>(this IPropertyBag propertyBag, string name, T value) where T : struct
		{
			PropertyHelper.WritePropertyBag(propertyBag, name, Convert.ToString(value));
		}

		/// <summary>
		/// Write a property value to the <see cref="IPropertyBag"/> bag.
		/// </summary>
		/// <param name="propertyBag">
		/// The <see cref="IPropertyBag"/> bag instance.
		/// </param>
		/// <param name="name">
		/// The name of the property to write to the <see cref="IPropertyBag"/> bag.
		/// </param>
		/// <param name="value">
		/// The property value to write to the <see cref="IPropertyBag"/> bag.
		/// </param>
		public static void WriteProperty(this IPropertyBag propertyBag, string name, bool value)
		{
			PropertyHelper.WritePropertyBag(propertyBag, name, value);
		}

		/// <summary>
		/// Write a property value to the <see cref="IPropertyBag"/> bag.
		/// </summary>
		/// <param name="propertyBag">
		/// The <see cref="IPropertyBag"/> bag instance.
		/// </param>
		/// <param name="name">
		/// The name of the property to write to the <see cref="IPropertyBag"/> bag.
		/// </param>
		/// <param name="value">
		/// The property value to write to the <see cref="IPropertyBag"/> bag.
		/// </param>
		public static void WriteProperty(this IPropertyBag propertyBag, string name, int value)
		{
			PropertyHelper.WritePropertyBag(propertyBag, name, value);
		}

		/// <summary>
		/// Write a property value to the <see cref="IPropertyBag"/> bag.
		/// </summary>
		/// <param name="propertyBag">
		/// The <see cref="IPropertyBag"/> bag instance.
		/// </param>
		/// <param name="name">
		/// The name of the property to write to the <see cref="IPropertyBag"/> bag.
		/// </param>
		/// <param name="value">
		/// The property value to write to the <see cref="IPropertyBag"/> bag.
		/// </param>
		public static void WriteProperty(this IPropertyBag propertyBag, string name, string value)
		{
			// writing string.Empty is compliant with native pipeline component and moreover ensures Pipeline DSL will
			// pickup all properties upon generation of BTP PipelineDocument
			PropertyHelper.WritePropertyBag(propertyBag, name, value.IsNullOrEmpty() ? string.Empty : value);
		}
	}
}
