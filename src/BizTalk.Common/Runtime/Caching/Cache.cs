#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
using System.Runtime.Caching;

namespace Be.Stateless.BizTalk.Runtime.Caching
{
	/// <summary>
	/// Simple runtime memory cache base class.
	/// </summary>
	/// <typeparam name="TKey">
	/// The type of the objects to be used as key.
	/// </typeparam>
	/// <typeparam name="TItem">
	/// The type of the objects to be cached.
	/// </typeparam>
	/// <remarks>
	/// For each derived class, <see cref="Cache{TKey,TItem}"/> creates behind the scene a named memory cache instance,
	/// either a real <see cref="System.Runtime.Caching.MemoryCache"/> for .NET 4.0 (from System.runtime.Caching
	/// assembly) and onwards or a simple substitute for it otherwise (from this assembly).
	/// </remarks>
	public abstract class Cache<TKey, TItem>
	{
		/// <summary>
		/// Create the <see cref="Cache{TKey,TItem}"/>-derived instance with a default sliding expiration of 30 minutes.
		/// </summary>
		/// <remarks>
		/// The <see cref="Cache{TKey,TItem}"/> creates behind the scene a <see cref="MemoryCache"/> named after the
		/// derived class name.
		/// </remarks>
		protected Cache() : this(TimeSpan.FromMinutes(30)) { }

		/// <summary>
		/// Create the <see cref="Cache{TKey,TItem}"/>-derived instance and overrides the default sliding expiration.
		/// </summary>
		/// <param name="slidingExpiration">
		/// The <see cref="TimeSpan"/> denoting the sliding expiration to apply to newly inserted items in cache.
		/// </param>
		/// <remarks>
		/// The <see cref="Cache{TKey,TItem}"/> creates behind the scene a <see cref="MemoryCache"/> named after the
		/// derived class name.
		/// </remarks>
		protected Cache(TimeSpan slidingExpiration)
		{
			if (slidingExpiration.TotalMinutes <= 0) throw new ArgumentException("Sliding expiration time span must be greater than 0 minutes", "slidingExpiration");
			_slidingExpiration = slidingExpiration;
			_cache = new MemoryCache(GetType().Name);
		}

		/// <summary>
		/// Gets, and sets if not already in cache, the <typeparamref name="TItem"/> instance associated to the
		/// <typeparamref name="TKey"/> instance.
		/// </summary>
		/// <param name="key">
		/// The <typeparamref name="TKey"/> key object instance to get or set in cache.
		/// </param>
		/// <returns>
		/// The <typeparamref name="TKey"/> object instance associated to the <typeparamref name="TKey"/> instance.
		/// </returns>
		/// <remarks>
		/// When the cache does not already contains the <typeparamref name="TItem"/> instance, it is inserted into the
		/// cache with a default 30 minutes sliding expiration, unless overridden by <see
		/// cref="Cache{TKey,TItem}(TimeSpan)"/>.
		/// </remarks>
		public TItem this[TKey key]
		{
			get
			{
				var keyString = ConvertKeyToString(key);

				if (_cache.Contains(keyString)) return (TItem) _cache[keyString];

				lock (_cache)
				{
					if (!_cache.Contains(keyString))
					{
						var cacheItem = new CacheItem(keyString, CreateItem(key));
						if (!_cache.Add(cacheItem, new CacheItemPolicy { SlidingExpiration = _slidingExpiration }))
							throw new InvalidOperationException(
								string.Format(
									"{0} already contains an entry for '{1}'.", GetType().Name, keyString));
						return (TItem) cacheItem.Value;
					}
				}
				return (TItem) _cache[keyString];
			}
		}

		/// <summary>
		/// Determines whether a cache entry exists in the cache for the <typeparamref name="TKey"/> instance.
		/// </summary>
		/// <param name="key">
		/// The <typeparamref name="TKey"/> instance to search for in cache.
		/// </param>
		/// <returns>
		/// <c>true</c> if the cache contains an entry for the <typeparamref name="TKey"/> instance; otherwise,
		/// <c>false</c>.
		/// </returns>
		public bool Contains(TKey key)
		{
			var keyString = ConvertKeyToString(key);
			return _cache.Contains(keyString);
		}

		protected abstract TItem CreateItem(TKey key);

		protected abstract string ConvertKeyToString(TKey key);

		private readonly MemoryCache _cache;
		private readonly TimeSpan _slidingExpiration;
	}
}
