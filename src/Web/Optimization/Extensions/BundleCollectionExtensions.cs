#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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

using System.Web.Optimization;

namespace Be.Stateless.Web.Optimization.Extensions
{
	public static class BundleCollectionExtensions
	{
		/// <summary>
		/// </summary>
		/// <param name="bundleCollection"></param>
		/// <seealso href="http://www.jefclaes.be/2012/02/aspnet-mvc4-bundling-in-aspnet-mvc3.html?m=1"/>
		/// <seealso href="http://www.davidhayden.me/blog/asp.net-mvc-4-bundling-and-minification"/>
		public static void EnableDebugAwareDefaultBundles(this BundleCollection bundleCollection)
		{
			BundleTable.Bundles.RegisterTemplateBundles();

			var cssBundle = bundleCollection.GetBundleFor("~/Content/css");
			cssBundle.AddDirectory("~/Content", "*.css");

			var jsBundle = bundleCollection.GetBundleFor("~/Scripts/js");
			jsBundle.AddDirectory("~/Scripts", "*.js");

#if DEBUG
			cssBundle.Transform = new NoTransform("text/css");
			jsBundle.Transform = new NoTransform("text/javascript");
#endif
		}
	}
}