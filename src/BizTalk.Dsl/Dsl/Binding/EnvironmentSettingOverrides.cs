#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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
using System.Xml;
using Be.Stateless.Extensions;
using Be.Stateless.Xml.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	internal class EnvironmentSettingOverrides : IEnvironmentSettingOverrides
	{
		internal EnvironmentSettingOverrides(string filePath)
		{
			if (filePath == null) throw new ArgumentNullException("filePath");
			var xmlDocument = new XmlDocument();
			xmlDocument.Load(filePath);
			_nsm = xmlDocument.GetNamespaceManager();
			_nsm.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");
			_data = xmlDocument.SelectSingleNode("/ss:Workbook/ss:Worksheet[@ss:Name='Settings']/ss:Table", _nsm);
		}

		#region IEnvironmentSettingOverrides Members

		T[] IEnvironmentSettingOverrides.ValuesForProperty<T>(string propertyName, T[] defaultValues)
		{
			var values = ValuesForProperty(propertyName)
				.Select(v => (T) Convert.ChangeType(v, typeof(T)))
				.ToArray();
			return values.Any() ? values : defaultValues;
		}

		T?[] IEnvironmentSettingOverrides.ValuesForProperty<T>(string propertyName, T?[] defaultValues)
		{
			var values = ValuesForProperty(propertyName)
				.Select(v => (T?) v.IfNotNull(v2 => Convert.ChangeType(v2, typeof(T))))
				.ToArray();
			return values.Any() ? values : defaultValues;
		}

		#endregion

		private IEnumerable<string> ValuesForProperty(string propertyName)
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			var values = _data
				.SelectNodes("ss:Row[ss:Cell[1]/ss:Data[@ss:Type='String']/text()='" + propertyName + "']/ss:Cell[position() > 1]", _nsm)
				.Cast<XmlNode>()
				.Select(cell => cell.SelectSingleNode("ss:Data/text()", _nsm).IfNotNull(data => data.Value))
				.ToArray();

			return values;
		}

		private readonly XmlNode _data;
		private readonly XmlNamespaceManager _nsm;
	}
}
