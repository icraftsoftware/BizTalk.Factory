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
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Interop
{
	/// <summary>
	/// Provides an object with a property bag in which the object can save its properties persistently.
	/// </summary>
	/// <seealso href="Microsoft.BizTalk.Internal.BTMPropertyBag" />
	public class PropertyBag : IPropertyBag, Microsoft.BizTalk.ExplorerOM.IPropertyBag, IXmlSerializable
	{
		internal PropertyBag()
		{
			_properties = new Dictionary<string, object>();
		}

		#region IPropertyBag Members

		/// <summary>
		/// Reads a named property from the property bag.
		/// </summary>
		/// <param name="name">
		/// The name of the property to read.
		/// </param>
		/// <param name="value">
		/// The value of the property that is read.
		/// </param>
		/// <param name="unused"></param>
		public void Read(string name, out object value, int unused)
		{
			if (name.IsNullOrEmpty()) throw new ArgumentNullException("name");
			value = _properties[name];
		}

		/// <summary>
		/// Saves a named property into the property bag.
		/// </summary>
		/// <param name="name">
		/// The name of the property to write.
		/// </param>
		/// <param name="value">
		/// The value of the property that is written.
		/// </param>
		public void Write(string name, ref object value)
		{
			if (name.IsNullOrEmpty()) throw new ArgumentNullException("name");
			if (_properties.ContainsKey(name)) _properties.Remove(name);
			if (value != null) _properties.Add(name, value);
		}

		#endregion

		#region IXmlSerializable Members

		public XmlSchema GetSchema()
		{
			throw new NotSupportedException();
		}

		public void ReadXml(XmlReader reader)
		{
			throw new NotSupportedException();
		}

		public void WriteXml(XmlWriter writer)
		{
			foreach (var property in Properties)
			{
				writer.WriteStartElement(property.Key);
				writer.WriteAttributeString("vt", ToVariantTypeCode(property.Value.GetType()).ToString("D"));
				writer.WriteValue(ToVariantValue(property.Value));
				writer.WriteEndElement();
			}
		}

		#endregion

		#region Base Class Member Overrides

		public override string ToString()
		{
			var xdoc = new XElement(
				"Properties",
				Properties.Select(
					p => new XElement(
						p.Key,
						new XAttribute("vt", ToVariantTypeCode(p.Value.GetType()).ToString("D")),
						ToVariantValue(p.Value))));
			return xdoc.ToString(SaveOptions.None);
		}

		#endregion

		public int Count
		{
			get { return Properties.Count(); }
		}

		protected IDictionary<string, object> Bag
		{
			get { return _properties; }
		}

		protected virtual IEnumerable<KeyValuePair<string, object>> Properties
		{
			get { return _properties; }
		}

		private readonly IDictionary<string, object> _properties;

		#region Variant Helper

		private static VarEnum ToVariantTypeCode(Type type)
		{
			// see Microsoft.BizTalk.Adapter.Framework.VariantHelper
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Empty:
					return VarEnum.VT_EMPTY;
				case TypeCode.Object:
					return VarEnum.VT_UNKNOWN;
				case TypeCode.DBNull:
					return VarEnum.VT_NULL;
				case TypeCode.Boolean:
					return VarEnum.VT_BOOL;
				case TypeCode.Char:
					return VarEnum.VT_UI2;
				case TypeCode.SByte:
					return VarEnum.VT_I1;
				case TypeCode.Byte:
					return VarEnum.VT_UI1;
				case TypeCode.Int16:
					return VarEnum.VT_I2;
				case TypeCode.UInt16:
					return VarEnum.VT_UI2;
				case TypeCode.Int32:
					return VarEnum.VT_I4;
				case TypeCode.UInt32:
					return VarEnum.VT_UI4;
				case TypeCode.Int64:
					return VarEnum.VT_I8;
				case TypeCode.UInt64:
					return VarEnum.VT_UI8;
				case TypeCode.Single:
					return VarEnum.VT_R4;
				case TypeCode.Double:
					return VarEnum.VT_R8;
				case TypeCode.Decimal:
					return VarEnum.VT_DECIMAL;
				case TypeCode.DateTime:
					return VarEnum.VT_DATE;
				case TypeCode.String:
					return VarEnum.VT_BSTR;
				default:
					throw new ArgumentException("Type not supported.", "type");
			}
		}

		private static string ToVariantValue(object obj)
		{
			// see Microsoft.BizTalk.Adapter.Framework.VariantHelper
			return obj is bool
				? (bool) obj ? "-1" : "0"
				: Convert.ToString(obj, CultureInfo.InvariantCulture);
		}

		#endregion
	}
}
