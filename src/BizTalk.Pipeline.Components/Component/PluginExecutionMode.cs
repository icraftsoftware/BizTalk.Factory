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

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// The moment at which a pipeline component's plugin will be executed. A plugin can take various forms ranging, for
	/// instance, from the Business Rule Policy that is configured for a <see cref="PolicyRunnerComponent"/> to the <see
	/// cref="IContextBuilder"/> that is configured for a <see cref="ContextBuilderComponent"/>.
	/// </summary>
	/// <remarks>
	/// The execution mode can either be <see cref="Immediate"/>, in which case the plugin will be executed as soon as
	/// its hosting pipeline component &#8212;e.g. the <see cref="ContextBuilderComponent"/> or the <see
	/// cref="PolicyRunnerComponent"/>&#8212; starts being executed, or <see cref="Deferred"/>, in which case the
	/// plugin's hosting component will wait for the message stream to have been exhausted to execute it.
	/// </remarks>
	public enum PluginExecutionMode
	{
		/// <summary>
		/// Executes the plugin as soon as its hosting pipeline component starts being executed.
		/// </summary>
		Immediate,

		/// <summary>
		/// Executes the plugin only after the message stream has been exhausted.
		/// </summary>
		Deferred
	}
}
