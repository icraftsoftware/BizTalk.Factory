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
	public interface IStage : IHideObjectMembers
	{
		StageCategory Category { get; }

		IComponentList Components { get; }

		IStage AddComponent<T>(T component) where T : IBaseComponent, IPersistPropertyBag;

		IStage AddComponent<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag, new();

		T Component<T>() where T : IBaseComponent, IPersistPropertyBag;

		IConfigurableComponent<T, IStage> ComponentAt<T>(int index) where T : IBaseComponent, IPersistPropertyBag;

		IStage Component<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag;

		IStage FirstComponent<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag;

		IStage SecondComponent<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag;

		IStage ThirdComponent<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag;

		IStage FourthComponent<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag;

		IStage FifthComponent<T>(Action<T> componentConfigurator) where T : IBaseComponent, IPersistPropertyBag;
	}
}
