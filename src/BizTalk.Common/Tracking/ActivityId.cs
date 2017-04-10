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

using System;

namespace Be.Stateless.BizTalk.Tracking
{
	/// <summary>
	/// Helper class that offers a central and unique place to generate homogenous and normalized BAM activity ids.
	/// </summary>
	public static class ActivityId
	{
		/// <summary>
		/// Generate a new tracking activity id as a normalized string activity id.
		/// </summary>
		/// <returns>
		/// A new activity id as a normalized string activity id.
		/// </returns>
		public static string NewActivityId()
		{
			return Guid.NewGuid().ToString("N");
		}

		/// <summary>
		/// Convert <see cref="Guid"/> to a normalized string activity id.
		/// </summary>
		/// <param name="guid">
		/// The <see cref="Guid"/> to convert into a normalized string activity id.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/> as a normalized string activity id.
		/// </returns>
		public static string AsNormalizedActivityId(this Guid guid)
		{
			return guid.ToString("N");
		}

		/// <summary>
		/// Normalize the string activity id to have a homogenous <see cref="Guid"/> format across all BAM tracking
		/// activities.
		/// </summary>
		/// <param name="id">The string <see cref="Guid"/> to normalize.</param>
		/// <returns>The normalized string activity id.</returns>
		public static string AsNormalizedActivityId(this string id)
		{
			return new Guid(id).ToString("N");
		}
	}
}
