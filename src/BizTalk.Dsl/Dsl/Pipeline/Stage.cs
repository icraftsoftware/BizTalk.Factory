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
	public class Stage : IStage, IVisitable<IPipelineVisitor>
	{
		internal Stage(Guid categoryId)
		{
			Category = StageCategory.FromKnownCategoryId(categoryId);
			Components = new ComponentList(this);
		}

		#region IStage Members

		public StageCategory Category { get; private set; }

		public IComponentList Components { get; private set; }

		public IStage AddComponent<T>(T component) where T : IBaseComponent, IPersistPropertyBag
		{
			Components.Add(component);
			return this;
		}

		public IStage AddComponent<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag, new()
		{
			var component = new T();
			componentConfigurator(component);
			Components.Add(component);
			return this;
		}

		public T Component<T>() where T : IBaseComponent, IPersistPropertyBag
		{
			return Components.Component<T>();
		}

		public IConfigurableComponent<T, IStage> ComponentAt<T>(int index) where T : IBaseComponent, IPersistPropertyBag
		{
			return new ConfigurableComponent<T, IStage>((ConfigurableComponent<T, IComponentList>) Components.ComponentAt<T>(index), this);
		}

		public IStage Component<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Components.Component(componentConfigurator);
			return this;
		}

		public IStage FirstComponent<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Components.FirstComponent(componentConfigurator);
			return this;
		}

		public IStage SecondComponent<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Components.SecondComponent(componentConfigurator);
			return this;
		}

		public IStage ThirdComponent<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Components.ThirdComponent(componentConfigurator);
			return this;
		}

		public IStage FourthComponent<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Components.FourthComponent(componentConfigurator);
			return this;
		}

		public IStage FifthComponent<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			Components.FifthComponent(componentConfigurator);
			return this;
		}

		#endregion

		#region IVisitable<IPipelineVisitor> Members

		void IVisitable<IPipelineVisitor>.Accept(IPipelineVisitor visitor)
		{
			visitor.VisitStage(this);
			((IVisitable<IPipelineVisitor>) Components).Accept(visitor);
		}

		#endregion
	}
}
