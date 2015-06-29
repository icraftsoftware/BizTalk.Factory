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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.Extensions;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Monitoring.Model
{
	public partial class Context
	{
		#region Nested type: Property

		public class Property<T>
		{
			public static implicit operator Property<T>(Property<string> src)
			{
				return new Property<T> {
					IsPromoted = src.IsPromoted,
					Name = src.Name,
					Namespace = src.Namespace,
					Value = (T) (object) src.Value
				};
			}

			public bool IsPromoted { get; set; }
			public string Name { get; set; }
			public string Namespace { get; set; }
			public T Value { get; set; }
		}

		public class Property : Property<string> { }

		#endregion

		public IEnumerable<Property> Properties
		{
			get
			{
				if (_properties != null) return _properties;
				_properties = EncodedContext.IsNullOrEmpty()
					? Enumerable.Empty<Property>()
					: XDocument.Load(new StringReader(EncodedContext)).Elements("context")
						.Elements()
						.Select(
							p => new Property {
								IsPromoted = bool.Parse(p.Attribute("promoted").IfNotNull(v => v.Value) ?? bool.FalseString),
								Name = p.Attribute("n").IfNotNull(v => v.Value) ?? string.Empty,
								Namespace = p.Name.Namespace.NamespaceName,
								Value = p.Value
							})
						.ToList();
				return _properties;
			}
		}

		public Property GetProperty<T>(MessageContextProperty<T, string> property)
			where T : MessageContextPropertyBase, new()
		{
			return Properties.SingleOrDefault(p => p.Name == property.Name && p.Namespace == property.Namespace);
		}

		public Property<TR?> GetProperty<T, TR>(MessageContextProperty<T, TR> property)
			where T : MessageContextPropertyBase, new()
			where TR : struct
		{
			return Properties.SingleOrDefault(p => p.Name == property.Name && p.Namespace == property.Namespace);
		}

		private IEnumerable<Property> _properties;
	}
}
