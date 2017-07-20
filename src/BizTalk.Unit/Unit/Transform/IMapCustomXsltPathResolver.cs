#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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

namespace Be.Stateless.BizTalk.Unit.Transform
{
	/// <summary>
	/// Resolve the path to BizTalk maps' custom XSLT.
	/// </summary>
	public interface IMapCustomXsltPathResolver
	{
		/// <summary>
		/// Resolve the path to a BizTalk map's custom XSLT.
		/// </summary>
		/// <param name="path">
		/// The resolved path to the custom XSLT of the BizTalk map passed to the constructor.
		/// </param>
		/// <returns>
		/// <c>true</c> if the path to the source of the custom XSLT could be resolved; <c>false</c> otherwise.
		/// </returns>
		bool TryResolveXsltPath(out string path);
	}
}
