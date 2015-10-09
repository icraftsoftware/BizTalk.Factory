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
using Be.Stateless.BizTalk.Dsl.Pipeline;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
	public class ReceivePipeline<T> : ReceivePipeline, ISupportEnvironmentOverride, ITypeDescriptor
		where T : Microsoft.BizTalk.PipelineOM.ReceivePipeline, new()
	{
		public static ReceivePipeline<T> Configure(Action<ReceivePipeline<T>> receivePipelineConfigurator)
		{
			return new ReceivePipeline<T>(receivePipelineConfigurator);
		}

		public ReceivePipeline()
		{
			PipelineObjectModel<T>.CloneStageDefinitions(Stages);
		}

		public ReceivePipeline(Action<ReceivePipeline<T>> receivePipelineConfigurator) : this()
		{
			receivePipelineConfigurator(this);
		}

		#region ISupportEnvironmentOverride Members

		void ISupportEnvironmentOverride.ApplyEnvironmentOverrides(string environment)
		{
			ApplyEnvironmentOverrides(environment);
		}

		#endregion

		#region ITypeDescriptor Members

		string ITypeDescriptor.FullName
		{
			get { return typeof(T).FullName; }
		}

		string ITypeDescriptor.AssemblyQualifiedName
		{
			get { return typeof(T).AssemblyQualifiedName; }
		}

		#endregion

		protected virtual void ApplyEnvironmentOverrides(string environment) { }
	}
}
