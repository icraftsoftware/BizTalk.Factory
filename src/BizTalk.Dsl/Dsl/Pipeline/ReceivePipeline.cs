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
	public abstract class ReceivePipeline : Pipeline<IReceivePipelineStageList>
	{
		protected internal ReceivePipeline() : base(new ReceivePipelineStageList()) { }

		#region Decoder Configurators

		/// <summary>
		/// Components of the 1st stage of a <see cref="ReceivePipeline"/>, i.e. the <see
		/// cref="IReceivePipelineStageList.Decode"/> stage.
		/// </summary>
		public IComponentList Decoders
		{
			get { return Stages.Decode.Components; }
		}

		public T Decoder<T>() where T : IBaseComponent, IPersistPropertyBag
		{
			return Stages.Decode.Component<T>();
		}

		public IConfigurableComponent<T, ReceivePipeline> DecoderAt<T>(int index) where T : IBaseComponent, IPersistPropertyBag
		{
			return new ConfigurableComponent<T, ReceivePipeline>((ConfigurableComponent<T, IStage>) Stages.Decode.ComponentAt<T>(index), this);
		}

		public ReceivePipeline Decoder<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Decode.Component(componentConfigurator);
			return this;
		}

		public ReceivePipeline FirstDecoder<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Decode.FirstComponent(componentConfigurator);
			return this;
		}

		public ReceivePipeline SecondDecoder<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Decode.SecondComponent(componentConfigurator);
			return this;
		}

		public ReceivePipeline ThirdDecoder<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Decode.ThirdComponent(componentConfigurator);
			return this;
		}

		public ReceivePipeline FourthDecoder<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Decode.FourthComponent(componentConfigurator);
			return this;
		}

		public ReceivePipeline FifthDecoder<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Decode.FifthComponent(componentConfigurator);
			return this;
		}

		#endregion

		#region Disassembler Configurators

		/// <summary>
		/// Components of the 2nd stage of a <see cref="ReceivePipeline"/>, i.e. the <see
		/// cref="IReceivePipelineStageList.Disassemble"/> stage.
		/// </summary>
		public IComponentList Disassemblers
		{
			get { return Stages.Disassemble.Components; }
		}

		public T Disassembler<T>() where T : IBaseComponent, IPersistPropertyBag
		{
			return Stages.Disassemble.Component<T>();
		}

		public IConfigurableComponent<T, ReceivePipeline> DisassemblerAt<T>(int index) where T : IBaseComponent, IPersistPropertyBag
		{
			return new ConfigurableComponent<T, ReceivePipeline>((ConfigurableComponent<T, IStage>) Stages.Disassemble.ComponentAt<T>(index), this);
		}

		public ReceivePipeline Disassembler<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Disassemble.Component(componentConfigurator);
			return this;
		}

		public ReceivePipeline FirstDisassembler<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Disassemble.FirstComponent(componentConfigurator);
			return this;
		}

		public ReceivePipeline SecondDisassembler<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Disassemble.SecondComponent(componentConfigurator);
			return this;
		}

		public ReceivePipeline ThirdDisassembler<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Disassemble.ThirdComponent(componentConfigurator);
			return this;
		}

		public ReceivePipeline FourthDisassembler<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Disassemble.FourthComponent(componentConfigurator);
			return this;
		}

		public ReceivePipeline FifthDisassembler<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Disassemble.FifthComponent(componentConfigurator);
			return this;
		}

		#endregion

		#region Validator Configurators

		/// <summary>
		/// Components of the 3rd stage of a <see cref="ReceivePipeline"/>, i.e. the <see
		/// cref="IReceivePipelineStageList.Validate"/> stage.
		/// </summary>
		public IComponentList Validators
		{
			get { return Stages.Validate.Components; }
		}

		public T Validator<T>() where T : IBaseComponent, IPersistPropertyBag
		{
			return Stages.Validate.Component<T>();
		}

		public IConfigurableComponent<T, ReceivePipeline> ValidatorAt<T>(int index) where T : IBaseComponent, IPersistPropertyBag
		{
			return new ConfigurableComponent<T, ReceivePipeline>((ConfigurableComponent<T, IStage>) Stages.Validate.ComponentAt<T>(index), this);
		}

		public ReceivePipeline Validator<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Validate.Component(componentConfigurator);
			return this;
		}

		public ReceivePipeline FirstValidator<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Validate.FirstComponent(componentConfigurator);
			return this;
		}

		public ReceivePipeline SecondValidator<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Validate.SecondComponent(componentConfigurator);
			return this;
		}

		public ReceivePipeline ThirdValidator<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Validate.ThirdComponent(componentConfigurator);
			return this;
		}

		public ReceivePipeline FourthValidator<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Validate.FourthComponent(componentConfigurator);
			return this;
		}

		public ReceivePipeline FifthValidator<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.Validate.FifthComponent(componentConfigurator);
			return this;
		}

		#endregion

		#region PartyResolver Configurators

		/// <summary>
		/// Components of the 4th stage of a <see cref="ReceivePipeline"/>, i.e. the <see
		/// cref="IReceivePipelineStageList.ResolveParty"/> stage.
		/// </summary>
		public IComponentList PartyResolvers
		{
			get { return Stages.ResolveParty.Components; }
		}

		public T PartyResolver<T>() where T : IBaseComponent, IPersistPropertyBag
		{
			return Stages.ResolveParty.Component<T>();
		}

		public IConfigurableComponent<T, ReceivePipeline> PartyResolverAt<T>(int index) where T : IBaseComponent, IPersistPropertyBag
		{
			return new ConfigurableComponent<T, ReceivePipeline>((ConfigurableComponent<T, IStage>) Stages.ResolveParty.ComponentAt<T>(index), this);
		}

		public ReceivePipeline PartyResolver<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.ResolveParty.Component(componentConfigurator);
			return this;
		}

		public ReceivePipeline FirstPartyResolver<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.ResolveParty.FirstComponent(componentConfigurator);
			return this;
		}

		public ReceivePipeline SecondPartyResolver<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.ResolveParty.SecondComponent(componentConfigurator);
			return this;
		}

		public ReceivePipeline ThirdPartyResolver<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.ResolveParty.ThirdComponent(componentConfigurator);
			return this;
		}

		public ReceivePipeline FourthPartyResolver<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.ResolveParty.FourthComponent(componentConfigurator);
			return this;
		}

		public ReceivePipeline FifthPartyResolver<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Stages.ResolveParty.FifthComponent(componentConfigurator);
			return this;
		}

		#endregion
	}
}
