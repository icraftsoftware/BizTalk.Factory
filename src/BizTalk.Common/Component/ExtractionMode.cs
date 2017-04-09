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

using Be.Stateless.BizTalk.XPath;

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// Denotes how a context property has to be processed by the
	/// <c>Be.Stateless.BizTalk.Component.ContextPropertyExtractorComponent</c> pipeline component.
	/// </summary>
	/// <remarks>
	/// Extraction can either means
	/// <list type="bullet">
	/// <item>
	/// <see cref="ExtractionMode.Clear" /> &#8212; the property is deleted from the context.
	/// </item>
	/// <item>
	/// <see cref="ExtractionMode.Demote" /> &#8212; the property is demoted from the context, that is to say that its
	/// value is written back into the outgoing XML stream according to its <see cref="XPathExtractor"/>'s <see
	/// cref="XPathExtractor.XPathExpression"/>.
	/// </item>
	/// <item>
	/// <see cref="ExtractionMode.Ignore" /> &#8212; the extractor is ignored and no action is performed.
	/// </item>
	/// <item>
	/// <see cref="ExtractionMode.Promote" /> &#8212; the value either given via the <see cref="ConstantExtractor"/>.<see
	/// cref="ConstantExtractor.Value"/> property or extracted according to its <see cref="XPathExtractor"/>.<see
	/// cref="XPathExtractor.XPathExpression"/> is promoted to the context.
	/// </item>
	/// <item>
	/// <see cref="ExtractionMode.Write" /> &#8212; the value either given via the <see cref="ConstantExtractor"/>.<see
	/// cref="ConstantExtractor.Value"/> property or extracted according to its <see cref="XPathExtractor"/>.<see
	/// cref="XPathExtractor.XPathExpression"/> is written to the context.
	/// </item>
	/// </list>
	/// </remarks>
	public enum ExtractionMode
	{
		/// <summary>
		/// The property is deleted from the context.
		/// </summary>
		Clear = 1,

		/// <summary>
		/// The property is demoted from the context, that is to say that its value is written back into the outgoing XML
		/// stream according to its <see cref="XPathExtractor"/>'s <see cref="XPathExtractor.XPathExpression"/>.
		/// </summary>
		/// <remarks>
		/// This <see cref="ExtractionMode"/> is not compatible with <see cref="ConstantExtractor"/> and cannot be used
		/// with it.
		/// </remarks>
		Demote,

		/// <summary>
		/// The extractor is ignored and no action is performed.
		/// </summary>
		Ignore,

		/// <summary>
		/// The value either given via the <see cref="ConstantExtractor"/>.<see cref="ConstantExtractor.Value"/> property
		/// or extracted according to its <see cref="XPathExtractor"/>.<see cref="XPathExtractor.XPathExpression"/> is
		/// promoted to the context.
		/// </summary>
		Promote,

		/// <summary>
		/// The value either given via the <see cref="ConstantExtractor"/>.<see cref="ConstantExtractor.Value"/> property
		/// or extracted according to its <see cref="XPathExtractor"/>.<see cref="XPathExtractor.XPathExpression"/> is
		/// written to the context.
		/// </summary>
		Write = 0
	}
}
