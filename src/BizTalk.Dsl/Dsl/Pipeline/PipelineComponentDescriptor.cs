#region Copyright & License

// Copyright © 2012 - 2014 François Chabot, Yves Dierick
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

namespace Be.Stateless.BizTalk.Dsl.Pipeline
{
	/// <summary>
	/// BizTalk Server pipeline component wrapper to be used in conjunction with <see
	/// cref="IPipelineComponentDescriptor"/>.
	/// </summary>
	/// <typeparam name="T">
	/// The pipeline component <see cref="Type"/> to wrap.
	/// </typeparam>
	/// <remarks>
	/// This class is not meant to be used explicitly but only fulfils a Pipeline DSL scaffolding role.
	/// </remarks>
	internal class PipelineComponentDescriptor<T> : IPipelineComponentDescriptor
		where T : IBaseComponent, IPersistPropertyBag
	{
		public static implicit operator T(PipelineComponentDescriptor<T> pipelineComponentDescriptor)
		{
			return pipelineComponentDescriptor._pipelineComponent;
		}

		public PipelineComponentDescriptor(T pipelineComponent)
		{
			_pipelineComponent = pipelineComponent;
		}

		#region IBaseComponent Delegation

		public string Name
		{
			get { return _pipelineComponent.Name; }
		}

		public string Description
		{
			get { return _pipelineComponent.Description; }
		}

		public string Version
		{
			get { return _pipelineComponent.Version; }
		}

		#endregion

		#region IPersistPropertyBag Delegation

		public void GetClassID(out Guid classID)
		{
			_pipelineComponent.GetClassID(out classID);
		}

		public void InitNew()
		{
			_pipelineComponent.InitNew();
		}

		public void Load(IPropertyBag propertyBag, int errorLog)
		{
			_pipelineComponent.Load(propertyBag, errorLog);
		}

		public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
		{
			_pipelineComponent.Save(propertyBag, clearDirty, saveAllProperties);
		}

		#endregion

		#region IPipelineComponentDescriptor Members

		public string AssemblyQualifiedName
		{
			get { return typeof(T).AssemblyQualifiedName; }
		}

		public string FullName
		{
			get { return typeof(T).FullName; }
		}

		IPropertyBag IPipelineComponentDescriptor.Properties { get; set; }

		void IVisitable<IPipelineVisitor>.Accept(IPipelineVisitor visitor)
		{
			visitor.VisitComponent(this);
		}

		#endregion

		private readonly T _pipelineComponent;
	}
}
