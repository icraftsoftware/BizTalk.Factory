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

using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Be.Stateless.BizTalk.Orchestrations.Dummy;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.Linq.Extensions;
using Microsoft.CSharp;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.CodeDom
{
	[TestFixture]
	public class BtxServiceTypeExtensionsFixture
	{
		[Test]
		public void CompileToDynamicAssembly()
		{
			var btxServiceType = Assembly.GetExecutingAssembly().GetReferencedAssemblies()
				.Distinct((a1, a2) => a1.FullName == a2.FullName).Select(Assembly.Load)
				.Where(a => a.IsBizTalkAssembly())
				.SelectMany(a => a.GetOrchestrations())
				.First();

			var assembly = btxServiceType
				.CompileToDynamicAssembly();

			var orchestrationBinding = assembly.CreateInstance(btxServiceType.FullName + BtxServiceTypeExtensions.ORCHESTRATIONBINDING_SUFFIX);
			Assert.That(orchestrationBinding, Is.Not.Null);
			Assert.That(orchestrationBinding, Is.InstanceOf<OrchestrationBindingBase<Process>>());
		}

		[Test]
		public void ConvertToOrchestrationBindingCodeCompileUnit()
		{
			var btxServiceType = Assembly.GetExecutingAssembly().GetReferencedAssemblies()
				.Distinct((a1, a2) => a1.FullName == a2.FullName).Select(Assembly.Load)
				.Where(a => a.IsBizTalkAssembly())
				.SelectMany(a => a.GetOrchestrations())
				.First();

			var builder = new StringBuilder();
			using (var provider = new CSharpCodeProvider())
			using (var writer = new StringWriter(builder))
			{
				provider.GenerateCodeFromCompileUnit(
					btxServiceType.ConvertToOrchestrationBindingCodeCompileUnit(),
					writer,
					new CodeGeneratorOptions { BracingStyle = "C", IndentString = "\t", VerbatimOrder = true });
			}

			// Notice that ProcessOrchestrationBinding.Designer.cs is indeed included twice, in both Compile and
			// EmbeddedResource ItemGroups, both linking to Be.Stateless.Binding project's item. However Visual Studio's
			// Solution Explorer does only display the first occurrence in the .csproj file of the included item.
			Assert.That(builder.ToString(), Is.EqualTo(ResourceManager.LoadString("Data.ProcessOrchestrationBinding.Designer.cs")));
		}
	}
}
