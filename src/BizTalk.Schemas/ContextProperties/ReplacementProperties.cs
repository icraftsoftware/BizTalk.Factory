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

using System;
using System.Xml;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.ContextProperties
{
	// TODO find a better way than having to define this class
	[Serializable]
	[PropertyType("namespace-replacements", ReplacementProperties.Namespace, "string", "System.String")]
	[PropertyGuid("ff741ce2-6a52-49e0-80d6-ff11edf4a8f4")]
	public sealed class NamespaceReplacements : MessageContextPropertyBase
	{
		public override XmlQualifiedName Name
		{
			get { return new XmlQualifiedName("namespace-replacements", ReplacementProperties.Namespace); }
		}

		public override Type Type
		{
			get { return typeof(string); }
		}
	}

	public class ReplacementProperties
	{
		internal const string Namespace = "urn:schemas.stateless.be:biztalk:properties:replacement:2012:04";

		/// <summary>
		/// This context property value is consumed by the <see
		/// cref="Be.Stateless.BizTalk.PipelineComponents.ChangeNamespaceComponent"/>.
		/// </summary>
		public static readonly MessageContextProperty<NamespaceReplacements, string> NamespaceReplacements
			= new MessageContextProperty<NamespaceReplacements, string>();
	}
}