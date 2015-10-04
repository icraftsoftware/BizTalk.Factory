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
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.RuleEngine;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Unit.Component
{
	/// <summary>
	/// <see cref="PipelineComponentFixture{T}"/> companion class to support it when unit testing the back-and-forth
	/// persistence of <see cref="PipelineComponent"/> properties to a <see cref="IPropertyBag"/>.
	/// </summary>
	internal class BaggableProperty
	{
		internal BaggableProperty(PropertyInfo property, Func<string, object> nonPrimitiveValueProvider)
		{
			_property = property;
			_valueProvider = nonPrimitiveValueProvider;
			_converter = TypeDescriptor.GetConverter(_property.PropertyType);
		}

		internal object ActualValue { get; private set; }

		internal object BaggedValue { get; private set; }

		internal void EnsureConvertToStringAndBack(object seedValue)
		{
			if (_property.PropertyType.IsPrimitive || _property.PropertyType.IsEnum) return;

			GenerateValue(seedValue);
			var deserializedValue = ConvertFromString();

			var actualEnumerable = ActualValue as IEnumerable;
			var deserializedEnumerable = deserializedValue as IEnumerable;
			if (actualEnumerable != null && deserializedEnumerable != null && SequenceEqual(actualEnumerable, deserializedEnumerable)) return;

			if (!ActualValue.Equals(deserializedValue)) // TODO check if ActualValue is IEquatable<T>
				throw new NotSupportedException(
					string.Format(
						"{0} TypeConverter could not perform a successful roundtrip conversion of {1} {2} for {3}. " +
							"The failure to validate a successful roundtrip conversion could also be due to the fact that {1} does not implement IEquatable<T>.",
						_converter.GetType().Name,
						_property.PropertyType.Name,
						_property.Name,
						ActualValue));
		}

		internal BaggableProperty GenerateValue(object seedValue)
		{
			ActualValue = GetDistinguishedValue(seedValue);
			// for primitive types, the property bag stores the actual typed value (not a string representation), whereas
			// for complex types, the property bag stores a string representation of the actual typed value
			BaggedValue = _property.PropertyType.IsPrimitive ? ActualValue : ConvertToString();
			return this;
		}

		private object ConvertFromString()
		{
			if (!_converter.CanConvertFrom(typeof(string)))
				throw new NotSupportedException(
					string.Format(
						"{0} TypeConverter cannot convert a string to a {1}. {1} must have a dedicated TypeConverter that supports back-and-forth string conversions.",
						_converter.GetType().Name,
						_property.PropertyType.Name));
			var deserializedValue = _converter.ConvertFrom(BaggedValue);
			if (deserializedValue == null)
				throw new NotSupportedException(
					string.Format(
						"The conversion from string made by {0} of the {1} {2} property returned null for {3}.",
						_converter.GetType().Name,
						_property.PropertyType.Name,
						_property.Name,
						BaggedValue));
			return deserializedValue;
		}

		private string ConvertToString()
		{
			if (!_converter.CanConvertTo(typeof(string)))
				throw new NotSupportedException(
					string.Format(
						"{0} TypeConverter cannot convert a {1} to a string. {1} must have a dedicated TypeConverter that supports back-and-forth string conversions.",
						_converter.GetType().Name,
						_property.PropertyType.Name));
			var serializedValue = (string) _converter.ConvertTo(ActualValue, typeof(string));
			if (serializedValue.IsNullOrEmpty())
				throw new NotSupportedException(
					string.Format(
						"The conversion to string made by {0} of the {1} {2} property returned null for {3}.",
						_converter.GetType().Name,
						_property.PropertyType.Name,
						_property.Name,
						ActualValue));
			return serializedValue;
		}

		private object GetDistinguishedValue(object seedValue)
		{
			var value = _property.PropertyType.IsPrimitive
				? GetDistinguishedPrimitiveValue(seedValue)
				: (_property.PropertyType.IsEnum
					? GetDistinguishedEnumValue(seedValue)
					: GetDistinguishedComplexValue());
			if (!_property.PropertyType.IsInstanceOfType(value))
				throw new NotSupportedException(
					string.Format(
						"{0} property value is of type {1} and not {2} as expected.",
						_property.Name,
						value.GetType().Name,
						_property.PropertyType.Name));
			return value;
		}

		private object GetDistinguishedEnumValue(object seedValue)
		{
			if (!Attribute.IsDefined(_property.PropertyType, typeof(FlagsAttribute)))
			{
				// pick first Enum value that is different from default/current one
				var currentValueName = Enum.GetName(_property.PropertyType, seedValue);
				var otherValueName = Enum.GetNames(_property.PropertyType).First(name => name != currentValueName);
				return Enum.Parse(_property.PropertyType, otherValueName);
			}

			// pick a combination of flags that is different from default/current one
			var flags = Convert.ToString(seedValue).Split(',').Select(s => s.Trim()).ToArray();
			if (flags.Length > 2)
			{
				flags = flags.Skip(1).ToArray();
				return Enum.Parse(_property.PropertyType, string.Join(", ", flags));
			}

			var modifiedClosure = flags;
			var otherFlags = Enum.GetNames(_property.PropertyType)
				.Where(name => modifiedClosure.Any(f => f != name))
				.Take(2);
			flags = flags.Union(otherFlags).ToArray();
			return Enum.Parse(_property.PropertyType, string.Join(", ", flags));
		}

		private object GetDistinguishedPrimitiveValue(object seedValue)
		{
			if (_property.PropertyType == typeof(bool))
			{
				// negating a default Boolean property's value ensures that this property is actually
				// being set accordingly and any assertion on top of it is not lured by its default value
				return !(bool) seedValue;
			}
			if (_property.PropertyType == typeof(string))
			{
				// pick a unique value to ensure load/save roundtrip
				return Guid.NewGuid().ToString();
			}
			if (_property.PropertyType == typeof(int))
			{
				// pick a unique value to ensure load/save roundtrip
				return new Random().Next();
			}
			throw new NotSupportedException(string.Format("Support for {0} primitive type has yet to be provided.", _property.PropertyType.Name));
		}

		private object GetDistinguishedComplexValue()
		{
			var complexValue = _property.PropertyType == typeof(PolicyName)
				// pick a unique value to ensure load/save roundtrip
				? new PolicyName(Guid.NewGuid().ToString("N"), 9, 6)
				: _valueProvider(_property.Name);
			if (complexValue == null)
				throw new NotSupportedException(
					string.Format(
						"A non null value must be provided for {0} {1} property.",
						_property.PropertyType.Name,
						_property.Name));
			return complexValue;
		}

		private bool SequenceEqual(IEnumerable actualEnumerable, IEnumerable deserializedEnumerable)
		{
			var e1 = actualEnumerable.GetEnumerator();
			var e2 = deserializedEnumerable.GetEnumerator();
			while (e1.MoveNext())
			{
				if (!(e2.MoveNext() && e1.Current.Equals(e2.Current))) return false;
			}
			if (e2.MoveNext()) return false;
			return true;
		}

		private readonly TypeConverter _converter;
		private readonly PropertyInfo _property;
		private readonly Func<string, object> _valueProvider;
	}
}
