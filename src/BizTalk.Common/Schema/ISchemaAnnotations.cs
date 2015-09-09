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
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.XPath;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Schema
{
	public interface ISchemaAnnotations
	{
		/// <summary>
		/// The <see cref="Type"/> of the <see cref="TransformBase"/>-derived XSLT transform thSat has to be applied when
		/// transforming from a <see cref="Batch.Content"/> message to the <see cref="SchemaBase"/>-derived envelope
		/// schema which embeds this annotation.
		/// </summary>
		Type EnvelopingMap { get; }

		/// <summary>
		/// Collection of <see cref="XPathExtractor"/>s to be used to extract values of context properties from an <see
		/// cref="IBaseMessagePart"/>'s payload while being processed through the pipelines.
		/// </summary>
		IEnumerable<XPathExtractor> Extractors { get; }
	}
}
