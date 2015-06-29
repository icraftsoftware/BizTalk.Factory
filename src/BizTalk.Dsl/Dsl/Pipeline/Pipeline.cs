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
using System.ComponentModel;
using Be.Stateless.BizTalk.Dsl.Binding;

namespace Be.Stateless.BizTalk.Dsl.Pipeline
{
	public abstract class Pipeline<T> : IHideObjectMembers, IVisitable<IPipelineVisitor>, IPipelineSerializerFactory where T : IPipelineStageList
	{
		static Pipeline()
		{
			if (!typeof(IReceivePipelineStageList).IsAssignableFrom(typeof(T)) && !typeof(ISendPipelineStageList).IsAssignableFrom(typeof(T)))
				throw new ArgumentException(
					string.Format(
						"A pipeline does not support {0} as a stage container because it does not derive from either IReceivePipelineStageList or ISendPipelineStageList.",
						typeof(T).Name));
		}

		protected Pipeline(T stages)
		{
			Stages = stages;
			Version = new Version(1, 0);
		}

		#region IHideObjectMembers Members

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object obj)
		{
			// ReSharper disable BaseObjectEqualsIsObjectEquals
			return base.Equals(obj);
			// ReSharper restore BaseObjectEqualsIsObjectEquals
		}

		// ReSharper disable NonReadonlyFieldInGetHashCode
		[EditorBrowsable(EditorBrowsableState.Never)]
		// ReSharper restore NonReadonlyFieldInGetHashCode
		public override int GetHashCode()
		{
			// ReSharper disable BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
			// ReSharper restore BaseObjectGetHashCodeCallInGetHashCode
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string ToString()
		{
			return base.ToString();
		}

		#endregion

		#region IPipelineSerializerFactory Members

		IDslSerializer IPipelineSerializerFactory.GetPipelineBindingSerializer()
		{
			return new PipelineBindingSerializer(this);
		}

		IDslSerializer IPipelineSerializerFactory.GetPipelineCompiler()
		{
			return new PipelineCompiler(this);
		}

		IDslSerializer IPipelineSerializerFactory.GetPipelineDesignerDocumentSerializer()
		{
			return new PipelineDesignerDocumentSerializer(this);
		}

		#endregion

		#region IVisitable<IPipelineVisitor> Members

		void IVisitable<IPipelineVisitor>.Accept(IPipelineVisitor visitor)
		{
			visitor.VisitPipeline(this);
			((IVisitable<IPipelineVisitor>) Stages).Accept(visitor);
		}

		#endregion

		public string Description { get; protected set; }

		public T Stages { get; private set; }

		public Version Version { get; protected set; }

		/// <summary>
		/// Only needed if the pipeline DSL definition is meant to be interpreted at runtime by <see
		/// cref="Be.Stateless.BizTalk.Dsl.Pipeline.Interpreters.ReceivePipelineInterpreter{T}"/> and <see
		/// cref="Be.Stateless.BizTalk.Dsl.Pipeline.Interpreters.SendPipelineInterpreter{T}"/>.
		/// </summary>
		public Guid VersionDependentGuid { get; protected set; }
	}
}
