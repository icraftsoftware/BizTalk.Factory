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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Web.Monitoring.Model
{
	public partial class Context
	{
		#region Nested type: Property

		public class Property
		{
			public bool IsPromoted { get; set; }

			public string Name { get; set; }

			public string Namespace { get; set; }

			public string Value { get; set; }
		}

		#endregion

		public IEnumerable<Property> Properties
		{
			get
			{
				return EncodedContext.IsNullOrEmpty()
					? Enumerable.Empty<Property>()
					: XDocument.Load(new StringReader(EncodedContext)).Elements("context")
						.Elements()
						.Select(
							p => new Property {
								IsPromoted = bool.Parse(p.Attribute("promoted").IfNotNull(v => v.Value) ?? bool.FalseString),
								Name = p.Attribute("n").IfNotNull(v => v.Value) ?? string.Empty,
								Namespace = p.Name.Namespace.NamespaceName,
								Value = p.Value
							});
			}
		}
	}
}