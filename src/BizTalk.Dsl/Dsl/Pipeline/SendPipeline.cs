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
	public abstract class SendPipeline : Pipeline<ISendPipelineStageList>
	{
		protected internal SendPipeline() : base(new SendPipelineStageList()) { }

		#region PreAssembler Configurators

		/// <summary>
		/// Components of the 1st stage of a <see cref="SendPipeline"/>, i.e. the <see
		/// cref="ISendPipelineStageList.PreAssemble"/> stage.
		/// </summary>
		public IComponentList PreAssemblers
		{
			get { return Stages.PreAssemble.Components; }
		}

		public T PreAssembler<T>() where T : IBaseComponent, IPersistPropertyBag
		{
			return Stages.PreAssemble.Component<T>();
		}

		public IConfigurableComponent<T, SendPipeline> PreAssemblerAt<T>(int index) where T : IBaseComponent, IPersistPropertyBag
		{
			return new ConfigurableComponent<T, SendPipeline>((ConfigurableComponent<T, IStage>) Stages.PreAssemble.ComponentAt<T>(index), this);
		}

		public SendPipeline PreAssembler<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.PreAssemble.Component(componentConfigurator);
			return this;
		}

		public SendPipeline FirstPreAssembler<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.PreAssemble.FirstComponent(componentConfigurator);
			return this;
		}

		public SendPipeline SecondPreAssembler<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.PreAssemble.SecondComponent(componentConfigurator);
			return this;
		}

		public SendPipeline ThirdPreAssembler<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.PreAssemble.ThirdComponent(componentConfigurator);
			return this;
		}

		public SendPipeline FourthPreAssembler<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.PreAssemble.FourthComponent(componentConfigurator);
			return this;
		}

		public SendPipeline FifthPreAssembler<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.PreAssemble.FifthComponent(componentConfigurator);
			return this;
		}

		#endregion

		#region Assembler Configurators

		/// <summary>
		/// Components of the 2nd stage of a <see cref="SendPipeline"/>, i.e. the <see
		/// cref="ISendPipelineStageList.Assemble"/> stage.
		/// </summary>
		public IComponentList Assemblers
		{
			get { return Stages.Assemble.Components; }
		}

		public T Assembler<T>() where T : IBaseComponent, IPersistPropertyBag
		{
			return Stages.Assemble.Component<T>();
		}

		public IConfigurableComponent<T, SendPipeline> AssemblerAt<T>(int index) where T : IBaseComponent, IPersistPropertyBag
		{
			return new ConfigurableComponent<T, SendPipeline>((ConfigurableComponent<T, IStage>) Stages.Assemble.ComponentAt<T>(index), this);
		}

		public SendPipeline Assembler<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Assemble.Component(componentConfigurator);
			return this;
		}

		public SendPipeline FirstAssembler<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Assemble.FirstComponent(componentConfigurator);
			return this;
		}

		public SendPipeline SecondAssembler<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Assemble.SecondComponent(componentConfigurator);
			return this;
		}

		public SendPipeline ThirdAssembler<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Assemble.ThirdComponent(componentConfigurator);
			return this;
		}

		public SendPipeline FourthAssembler<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Assemble.FourthComponent(componentConfigurator);
			return this;
		}

		public SendPipeline FifthAssembler<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Assemble.FifthComponent(componentConfigurator);
			return this;
		}

		#endregion

		#region Encoder Configurators

		/// <summary>
		/// Components of the 3rd stage of a <see cref="SendPipeline"/>, i.e. the <see
		/// cref="ISendPipelineStageList.Encode"/> stage.
		/// </summary>
		public IComponentList Encoders
		{
			get { return Stages.Encode.Components; }
		}

		public T Encoder<T>() where T : IBaseComponent, IPersistPropertyBag
		{
			return Stages.Encode.Component<T>();
		}

		public IConfigurableComponent<T, SendPipeline> EncoderAt<T>(int index) where T : IBaseComponent, IPersistPropertyBag
		{
			return new ConfigurableComponent<T, SendPipeline>((ConfigurableComponent<T, IStage>) Stages.Encode.ComponentAt<T>(index), this);
		}

		public SendPipeline Encoder<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Encode.Component(componentConfigurator);
			return this;
		}

		public SendPipeline FirstEncoder<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Encode.FirstComponent(componentConfigurator);
			return this;
		}

		public SendPipeline SecondEncoder<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Encode.SecondComponent(componentConfigurator);
			return this;
		}

		public SendPipeline ThirdEncoder<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Encode.ThirdComponent(componentConfigurator);
			return this;
		}

		public SendPipeline FourthEncoder<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Encode.FourthComponent(componentConfigurator);
			return this;
		}

		public SendPipeline FifthEncoder<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Encode.FifthComponent(componentConfigurator);
			return this;
		}

		#endregion
	}
}
