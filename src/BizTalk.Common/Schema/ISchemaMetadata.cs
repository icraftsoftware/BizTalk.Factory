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
using Microsoft.BizTalk.Component.Interop;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Schema
{
	public interface ISchemaMetadata
	{
		/// <summary>
		/// Returns the annotations associated, and embedded, with a <see cref="SchemaBase"/>-derived envelope schema <see
		/// cref="Type"/>.
		/// </summary>
		/// <returns>
		/// The annotations associated, and embedded, with a <see cref="SchemaBase"/>-derived envelope schema <see
		/// cref="Type"/>.
		/// </returns>
		ISchemaAnnotations Annotations { get; }

		/// <summary>
		/// Returns the XPath expression to the node being the body of a <see cref="SchemaBase"/>-derived envelope schema
		/// <see cref="Type"/>.
		/// </summary>
		/// <returns>
		/// The XPath expression to the node being the body of a <see cref="SchemaBase"/>-derived envelope schema <see
		/// cref="Type"/>.
		/// </returns>
		string BodyXPath { get; }

		/// <summary>
		/// Returns the <see cref="DocumentSpec"/> of a given <see cref="SchemaBase"/>-derived envelope schema <see
		/// cref="Type"/>.
		/// </summary>
		/// <returns>
		/// The <see cref="DocumentSpec"/> of the <see cref="SchemaBase"/>-derived <see cref="Type"/>.
		/// </returns>
		DocumentSpec DocumentSpec { get; }

		/// <summary>
		/// Returns whether the <see cref="SchemaBase"/>-derived <see cref="Type"/> is an envelope schema.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the <see cref="SchemaBase"/>-derived <see cref="Type"/> is an envelope schema; <c>false</c>
		/// otherwise.
		/// </returns>
		bool IsEnvelopeSchema { get; }

		/// <summary>
		/// Returns the <see cref="BTS.MessageType"/> that any message being an instance of the <see
		/// cref="SchemaBase"/>-derived <see cref="Type"/> will have.
		/// </summary>
		/// <returns>
		/// The <see cref="BTS.MessageType"/> that any message being an instance of the <see cref="SchemaBase"/>-derived
		/// <see cref="Type"/> will have.
		/// </returns>
		string MessageType { get; }

		/// <summary>
		/// Returns the name of the root element of the <see cref="SchemaBase"/>-derived schema <see cref="Type"/>.
		/// </summary>
		/// <returns>
		/// The name of the root element of the <see cref="SchemaBase"/>-derived schema <see cref="Type"/>.
		/// </returns>
		string RootElementName { get; }

		/// <summary>
		/// Returns the target xml namespace of a <see cref="SchemaBase"/>-derived schema <see cref="Type"/>.
		/// </summary>
		/// <returns>
		/// The target xml namespace of a <see cref="SchemaBase"/>-derived schema <see cref="Type"/>.
		/// </returns>
		string TargetNamespace { get; }

		/// <summary>
		/// Returns the <see cref="SchemaBase"/>-derived <see cref="Type"/> to which this <see cref="ISchemaMetadata"/> is
		/// associated.
		/// </summary>
		/// <returns>
		/// The <see cref="SchemaBase"/>-derived <see cref="Type"/> to which this <see cref="ISchemaMetadata"/> is
		/// associated.
		/// </returns>
		Type Type { get; }
	}
}
