#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.Extensions;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Monitoring.Model
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
	public class MessageContext
	{
		#region Nested Type: Property

		public class Property<T>
		{
			#region Operators

			public static implicit operator Property<T>(Property<object> property)
			{
				return property == null
					? null
					: new Property<T> {
						IsPromoted = property.IsPromoted,
						Name = property.Name,
						Namespace = property.Namespace,
						Value = (T) Convert.ChangeType(property.Value, typeof(T))
					};
			}

			#endregion

			public bool IsPromoted { get; set; }

			public string Name { get; set; }

			public string Namespace { get; set; }

			public T Value { get; set; }
		}

		#endregion

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
		public string EncodedContext { get; set; }

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
		public MessagingStep MessagingStep { get; set; }

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
		public string MessagingStepActivityID { get; set; }

		// public for Be.Stateless.BizTalk.Web.Monitoring.Site
		public IEnumerable<Property<object>> Properties
		{
			get
			{
				if (_properties != null) return _properties;
				_properties = EncodedContext.IsNullOrEmpty()
					? Enumerable.Empty<Property<object>>()
					: XDocument.Load(new StringReader(EncodedContext)).Elements("context")
						.Elements()
						.Select(
							p => new Property<object> {
								IsPromoted = p.Attribute("promoted").IfNotNull(v => bool.Parse(v.Value)),
								Name = p.Attribute("n").IfNotNull(v => v.Value) ?? string.Empty,
								Namespace = p.Name.Namespace.NamespaceName,
								Value = p.Value
							})
						.ToList();
				return _properties;
			}
		}

		public Property<string> GetProperty<T>(MessageContextProperty<T, string> property)
			where T : MessageContextPropertyBase, new()
		{
			return Properties.SingleOrDefault(p => p.Name == property.Name && p.Namespace == property.Namespace);
		}

		public Property<TR> GetProperty<T, TR>(MessageContextProperty<T, TR> property)
			where T : MessageContextPropertyBase, new()
			where TR : struct
		{
			return Properties.SingleOrDefault(p => p.Name == property.Name && p.Namespace == property.Namespace);
		}

		private IEnumerable<Property<object>> _properties;
	}
}
