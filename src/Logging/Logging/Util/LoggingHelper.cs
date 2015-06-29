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

namespace Be.Stateless.Logging.Util
{
	internal static class LoggingHelper
	{
		public static byte[] Compute16BitsHashCode(byte[] array)
		{
			var hash = OFFSET_BASIS;

			foreach (var b in array)
			{
				hash = hash ^ b;
				hash = hash * FNV_PRIME;
			}

			hash = (hash >> 16) ^ (hash & MASK16);
			var hash16 = new byte[2];
			hash16[0] = BitConverter.GetBytes(hash)[0];
			hash16[1] = BitConverter.GetBytes(hash)[1];

			return hash16;
		}

		private const uint FNV_PRIME = 16777619;
		private const uint OFFSET_BASIS = 2166136261;
		private const uint MASK16 = ((uint) 1 << 16) - 1;
	}
}