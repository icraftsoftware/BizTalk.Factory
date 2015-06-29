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

using System;
using System.ComponentModel;

namespace Be.Stateless.BizTalk.Dsl
{
	/// <summary>
	/// Helper interface to provide a better intellisense experience by hiding the base <see cref="T:System.Object" />
	/// members from the fluent API.
	/// </summary>
	/// <remarks>
	/// The credit goes to Moq.
	/// </remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IHideObjectMembers
	{
		/// <summary />
		[EditorBrowsable(EditorBrowsableState.Never)]
		bool Equals(object other);

		/// <summary />
		[EditorBrowsable(EditorBrowsableState.Never)]
		int GetHashCode();

		/// <summary />
		[EditorBrowsable(EditorBrowsableState.Never)]
		Type GetType();

		/// <summary />
		[EditorBrowsable(EditorBrowsableState.Never)]
		string ToString();
	}
}
