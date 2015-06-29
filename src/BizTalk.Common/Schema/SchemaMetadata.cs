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
using System.Linq;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.XLANGs.RuntimeTypes;

namespace Be.Stateless.BizTalk.Schema
{
	/// <summary>
	/// Metadata associated to any <see cref="SchemaBase"/>-derived <see cref="Type"/>, ranging from information such as
	/// <see cref="BodyXPath"/>, <see cref="MessageType"/>, <see cref="TargetNamespace"/>, to annotations embedded in the
	/// XML schema definition.
	/// </summary>
	public class SchemaMetadata : ISchemaMetadata
	{
		#region Nested type: UnknownSchemaMetadata

		internal class UnknownSchemaMetadata : ISchemaMetadata
		{
			#region ISchemaMetadata Members

			public ISchemaAnnotations Annotations
			{
				get { return SchemaAnnotations.Empty; }
			}

			public string BodyXPath
			{
				get { return string.Empty; }
			}

			public DocumentSpec DocumentSpec
			{
				get { return null; }
			}

			public bool IsEnvelopeSchema
			{
				get { return false; }
			}

			public string MessageType
			{
				get { return string.Empty; }
			}

			public string RootElementName
			{
				get { return string.Empty; }
			}

			public string TargetNamespace
			{
				get { return string.Empty; }
			}

			public Type Type
			{
				get { return null; }
			}

			#endregion
		}

		#endregion

		internal SchemaMetadata(Type type)
		{
			if (!type.IsSchema()) throw new ArgumentException("Type is not a SchemaBase derived Type instance.", "type");
			_type = type;

			// ?? use SchemaMetadata.For(type).BodyXPath
			BodyXPath = Attribute.GetCustomAttributes(type, typeof(BodyXPathAttribute))
				.Cast<BodyXPathAttribute>()
				.SingleOrDefault()
				.IfNotNull(xpa => xpa.BodyXPath);

			var sa = Attribute.GetCustomAttributes(type, typeof(SchemaAttribute))
				.Cast<SchemaAttribute>()
				.Single();

			// ?? use SchemaMetadata.For(type).SchemaName
			MessageType = PartTypeMetadata.ComposeMessageType(sa.TargetNamespace, sa.RootElement);

			// ?? use SchemaMetadata.For(type).RootElementName
			RootElementName = sa.RootElement;

			// ?? use SchemaMetadata.For(type).TargetNamespace
			TargetNamespace = sa.TargetNamespace;
		}

		#region ISchemaMetadata Members

		public ISchemaAnnotations Annotations
		{
			get
			{
				if (_annotations != null) return _annotations;
				lock (this)
				{
					return _annotations ?? (_annotations = SchemaAnnotations.Create(this));
				}
			}
		}

		public string BodyXPath { get; private set; }

		public DocumentSpec DocumentSpec
		{
			get { return new DocumentSpec(_type.FullName, _type.Assembly.FullName); }
		}

		public bool IsEnvelopeSchema
		{
			get { return !BodyXPath.IsNullOrEmpty(); }
		}

		public string MessageType { get; private set; }

		public string RootElementName { get; private set; }

		public string TargetNamespace { get; private set; }

		public Type Type
		{
			get { return _type; }
		}

		#endregion

		public static readonly ISchemaMetadata Unknown = new UnknownSchemaMetadata();

		private readonly Type _type;
		private ISchemaAnnotations _annotations;
	}
}
