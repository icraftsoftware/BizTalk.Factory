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
using System.Collections.Generic;
using System.Linq;
using Be.Stateless.Linq.Extensions;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Pipeline
{
	public class ComponentList : List<IPipelineComponentDescriptor>, IComponentList, IVisitable<IPipelineVisitor>
	{
		internal ComponentList(Stage stage)
		{
			Stage = stage;
		}

		#region IComponentList Members

		IComponentList IComponentList.Add<T>(T component)
		{
			component.EnsureIsPipelineComponent();
			component.EnsureIsCompatibleWith(Stage.Category);
			base.Add(new PipelineComponentDescriptor<T>(component));
			return this;
		}

		public T Component<T>() where T : IBaseComponent, IPersistPropertyBag
		{
			return this.OfType<PipelineComponentDescriptor<T>>().Single();
		}

		public IConfigurableComponent<T, IComponentList> ComponentAt<T>(int index) where T : IBaseComponent, IPersistPropertyBag
		{
			return new ConfigurableComponent<T, IComponentList>((PipelineComponentDescriptor<T>) this.ElementAt(index), this);
		}

		public IComponentList Component<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			var component = Component<T>();
			componentConfigurator(component);
			return this;
		}

		public IComponentList FirstComponent<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			ComponentOfTypeAt<T>(0).Configure(componentConfigurator);
			return this;
		}

		public IComponentList SecondComponent<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			ComponentOfTypeAt<T>(1).Configure(componentConfigurator);
			return this;
		}

		public IComponentList ThirdComponent<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			ComponentOfTypeAt<T>(2).Configure(componentConfigurator);
			return this;
		}

		public IComponentList FourthComponent<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			ComponentOfTypeAt<T>(3).Configure(componentConfigurator);
			return this;
		}

		public IComponentList FifthComponent<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag
		{
			ComponentOfTypeAt<T>(4).Configure(componentConfigurator);
			return this;
		}

		#endregion

		#region IVisitable<IPipelineVisitor> Members

		void IVisitable<IPipelineVisitor>.Accept(IPipelineVisitor visitor)
		{
			this.Cast<IVisitable<IPipelineVisitor>>().Each(component => component.Accept(visitor));
		}

		#endregion

		public Stage Stage { get; private set; }

		public IComponentList Add<T>(T component) where T : IBaseComponent, IPersistPropertyBag
		{
			return ((IComponentList) this).Add(component);
		}

		private IConfigurableComponent<T, IComponentList> ComponentOfTypeAt<T>(int index) where T : IBaseComponent, IPersistPropertyBag
		{
			return new ConfigurableComponent<T, IComponentList>(this.OfType<PipelineComponentDescriptor<T>>().ElementAt(index), this);
		}
	}
}
