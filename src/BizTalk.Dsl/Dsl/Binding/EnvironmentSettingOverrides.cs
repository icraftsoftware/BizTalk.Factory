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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Be.Stateless.BizTalk.Install;
using Be.Stateless.Extensions;
using Be.Stateless.Xml.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	internal class EnvironmentSettingOverrides
	{
		internal EnvironmentSettingOverrides(string filePath)
		{
			if (filePath == null) throw new ArgumentNullException("filePath");
			_filePath = filePath;
			var xmlDocument = new XmlDocument();
			xmlDocument.Load(filePath);
			_nsm = xmlDocument.GetNamespaceManager();
			_nsm.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");
			_data = xmlDocument.SelectSingleNode("/ss:Workbook/ss:Worksheet[@ss:Name='Settings']/ss:Table", _nsm);
		}

		internal T ReferenceTypeValueForTargetEnvironment<T>(string propertyName, int targetEnvironmentIndex) where T : class
		{
			var values = ValuesForProperty(propertyName)
				.Select(v => (T) Convert.ChangeType(v, typeof(T)))
				.ToArray();
			var value = values[targetEnvironmentIndex] ?? values[0];
			if (value == null)
				throw new InvalidOperationException(
					string.Format(
						"Setting overrides '{0}' does not have a defined value neither for '{1}' or default target environment.",
						propertyName,
						BindingGenerationContext.Instance.TargetEnvironment));
			return value;
		}

		internal T ValueTypeValueForTargetEnvironment<T>(string propertyName, int targetEnvironmentIndex) where T : struct
		{
			var values = ValuesForProperty(propertyName)
				.Select(v => (T?) v.IfNotNull(v2 => Convert.ChangeType(v2, typeof(T))))
				.ToArray();
			var value = values[targetEnvironmentIndex] ?? values[0];
			if (value == null)
				throw new InvalidOperationException(
					string.Format(
						"Setting overrides '{0}' does not have a defined value neither for '{1}' or default target environment.",
						propertyName,
						BindingGenerationContext.Instance.TargetEnvironment));
			return value.Value;
		}

		private IEnumerable<string> ValuesForProperty(string propertyName)
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			var values = _data
				.SelectNodes("ss:Row[ss:Cell[1]/ss:Data[@ss:Type='String']/text()='" + propertyName + "']/ss:Cell[position() > 1]", _nsm)
				.Cast<XmlNode>()
				.Select(cell => cell.SelectSingleNode("ss:Data/text()", _nsm).IfNotNull(data => data.Value))
				.ToArray();
			if (!values.Any())
				throw new InvalidOperationException(
					string.Format(
						"Environment setting file '{0}' does not define the setting '{1}'.",
						_filePath,
						propertyName));
			return values;
		}

		private readonly XmlNode _data;
		private readonly string _filePath;
		private readonly XmlNamespaceManager _nsm;
	}
}
