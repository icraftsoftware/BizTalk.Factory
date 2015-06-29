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
using System.Xml.Xsl;
using Be.Stateless.BizTalk.Transform;

namespace Be.Stateless.BizTalk.Xml.Xsl
{
	/// <summary>
	/// Describes the requirements of a <see cref="XslCompiledTransform"/> in terms of extension objects.
	/// </summary>
	[Flags]
	public enum ExtensionRequirements
	{
		/// <summary>
		/// The <see cref="XslCompiledTransform"/> has no requirement.
		/// </summary>
		None,

		/// <summary>
		/// The <see cref="XslCompiledTransform"/> needs the <see cref="BaseMessageContextFunctions"/> extension object.
		/// </summary>
		/// <remarks>
		/// If will automatically be set if the stylesheet markup declares the namespace of the <see
		/// cref="BaseMessageContextFunctions"/> (see <see cref="BaseMessageContextFunctions.TARGET_NAMESPACE"/>).
		/// </remarks>>
		/// <seealso cref="BaseMessageContextFunctions.TARGET_NAMESPACE"/>
		MessageContext
	}
}
