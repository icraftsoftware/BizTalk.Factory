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

namespace Be.Stateless.BizTalk
{
	internal static class DropFolders
	{
		private const string ROOT_FOLDER = @"C:\Files\Drops\BizTalk.Factory";
		internal const string INPUT_FOLDER = ROOT_FOLDER + @"\In";
		internal const string OUTPUT_FOLDER = ROOT_FOLDER + @"\Out";
		internal const string TRACE_FOLDER = ROOT_FOLDER + @"\Trace";
	}
}
