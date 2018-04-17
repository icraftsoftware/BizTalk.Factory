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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml.Xsl;
using Be.Stateless.BizTalk.Xml.Xsl;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Streaming.Extensions
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public interface ITransformStream
	{
		/// <summary>
		/// Applies the <see cref="Stream"/>-derived transform on the current <see cref="Stream"/>, or <see
		/// cref="Stream"/>s, and returns the results as another <see cref="Stream"/>.
		/// </summary>
		/// <param name="transform">
		/// The <see cref="Stream"/>-derived type of the transform to apply.
		/// </param>
		/// <returns>
		/// The output <see cref="TransformBase"/> being the results of the transform.
		/// </returns>
		Stream Apply(Type transform);

		/// <summary>
		/// Applies the <see cref="TransformBase"/>-derived transform on the current <see cref="Stream"/>, or <see
		/// cref="Stream"/>s, and returns the results as another <see cref="Stream"/>.
		/// </summary>
		/// <param name="transform">
		/// The <see cref="TransformBase"/>-derived type of the transform to apply.
		/// </param>
		/// <param name="encoding">
		/// The encoding to use for the output <see cref="Stream"/>.
		/// </param>
		/// <returns>
		/// The output <see cref="Stream"/> being the results of the transform.
		/// </returns>
		Stream Apply(Type transform, Encoding encoding);

		/// <summary>
		/// Applies the <see cref="TransformBase"/>-derived transform on the current <see cref="Stream"/>, or <see
		/// cref="Stream"/>s, and returns the results as another <see cref="Stream"/>.
		/// </summary>
		/// <param name="transform">
		/// The <see cref="TransformBase"/>-derived type of the transform to apply.
		/// </param>
		/// <param name="arguments">
		/// An <see cref="XsltArgumentList"/> containing the namespace-qualified arguments used as input to the transform.
		/// </param>
		/// <returns>
		/// The output <see cref="Stream"/> being the results of the transform.
		/// </returns>
		Stream Apply(Type transform, XsltArgumentList arguments);

		/// <summary>
		/// Applies the <see cref="TransformBase"/>-derived transform on the current <see cref="Stream"/>, or <see
		/// cref="Stream"/>s, and returns the results as another <see cref="Stream"/>.
		/// </summary>
		/// <param name="transform">
		/// The <see cref="TransformBase"/>-derived type of the transform to apply.
		/// </param>
		/// <param name="arguments">
		/// An <see cref="XsltArgumentList"/> containing the namespace-qualified arguments used as input to the transform.
		/// </param>
		/// <param name="encoding">
		/// The encoding to use for the output <see cref="Stream"/>.
		/// </param>
		/// <returns>
		/// The output <see cref="Stream"/> being the results of the transform.
		/// </returns>
		Stream Apply(Type transform, XsltArgumentList arguments, Encoding encoding);

		/// <summary>
		/// Provides extension objects that the <see cref="TransformBase"/>-derived transform might need.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that will be later on passed as an extension object to the <see
		/// cref="TransformBase"/>-derived transform should it require it, see <see
		/// cref="XslCompiledTransformDescriptor.ExtensionRequirements"/>.
		/// </param>
		/// <returns>
		/// The <see cref="ITransformStream"/> instance that will apply the transform on the current <see cref="Stream"/>.
		/// </returns>
		ITransformStream ExtendWith(IBaseMessageContext context);
	}
}
