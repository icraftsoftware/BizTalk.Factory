#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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

using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Pipeline
{
	/// <summary>
	/// Merge BizTalk Server pipeline component's interfaces that are used and required by Pipeline DSL.
	/// </summary>
	/// <remarks>
	/// This interface is not meant to be used explicitly but only fulfills a Pipeline DSL scaffolding role and is meant
	/// to be used in conjunction with <see cref="PipelineComponentDescriptor{T}"/>.
	/// </remarks>
	public interface IPipelineComponentDescriptor : IBaseComponent, IPersistPropertyBag, ITypeDescriptor, IVisitable<IPipelineVisitor>
	{
		IPropertyBag Properties { get; set; }
	}
}
