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
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Runtime.Caching
{
	/// <summary>
	/// Runtime memory cache for the <see cref="TrackingContext"/> associated to <see cref="IBaseMessage"/>-derived
	/// types.
	/// </summary>
	// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
	public class TrackingContextCache
	{
		/// <summary>
		/// Singleton <see cref="TrackingContextCache"/> instance.
		/// </summary>
		public static TrackingContextCache Instance
		{
			get { return _instance; }
			internal set { _instance = value; }
		}

		protected TrackingContextCache()
		{
			_cache = new MemoryCache(typeof(TrackingContextCache).Name);
		}

		/// <summary>
		/// Adds a new <see cref="TrackingContext"/> in cache with an absolute expiration of <paramref name="duration"/>.
		/// </summary>
		/// <param name="key">
		/// The cache entry identifier, or key, at which to add the new <paramref name="trackingContext"/>.
		/// </param>
		/// <param name="trackingContext">
		/// The <see cref="TrackingContext"/> to cache.
		/// </param>
		/// <param name="duration">
		/// The duration, in seconds, after which the <paramref name="trackingContext"/> entry will be removed from the
		/// cache.
		/// </param>
		public virtual void Add(string key, TrackingContext trackingContext, int duration)
		{
			if (key.IsNullOrEmpty()) throw new ArgumentNullException("key");
			if (trackingContext.IsEmpty()) throw new ArgumentNullException("trackingContext");
			if (duration < 1) throw new ArgumentException("Expiration duration must be strictly positive.", "duration");

			var cacheItem = new CacheItem(key, trackingContext);
			var policy = new CacheItemPolicy { AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow.AddSeconds(duration)) };
			if (!_cache.Add(cacheItem, policy))
				throw new InvalidOperationException(
					string.Format("{0} already contains an entry for '{1}'.", GetType().Name, key));
		}

		/// <summary>
		/// Removes and returns a previously cached <see cref="TrackingContext"/>.
		/// </summary>
		/// <param name="key">
		/// The cache entry identifier, or key, of the <see cref="TrackingContext"/> to retrieve and erase.
		/// </param>
		/// <returns>
		/// The <see cref="TrackingContext"/> that had been previously added to the cache.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// The key is null or empty.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// No entry has been found in cache for the given <paramref name="key"/>.
		/// </exception>
		public virtual TrackingContext Remove(string key)
		{
			if (key.IsNullOrEmpty()) throw new ArgumentNullException("key");

			var cachedData = _cache.Remove(key);
			if (cachedData == null) throw new InvalidOperationException("TrackingContext could not be found in cache.");
			var trackingContext = (TrackingContext) cachedData;
			if (trackingContext.IsEmpty()) throw new InvalidOperationException("Invalid TrackingContext: None of its individual activity Ids is set.");
			return trackingContext;
		}

		/// <summary>
		/// Returns a previously cached <see cref="TrackingContext"/>.
		/// </summary>
		/// <param name="key">
		/// The cache entry identifier, or key, of the <see cref="TrackingContext"/> to retrieve.
		/// </param>
		/// <returns>
		/// The <see cref="TrackingContext"/> that had been previously added to the cache.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// The key is null or empty.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// No entry has been found in cache for the given <paramref name="key"/>, 
		/// or the <see cref="TrackingContext"/> is invalid.
		/// </exception>
		public virtual TrackingContext Get(string key)
		{
			if (key.IsNullOrEmpty()) throw new ArgumentNullException("key");

			var cachedData = _cache.Get(key);
			if (cachedData == null) throw new InvalidOperationException("TrackingContext could not be found in cache.");
			var trackingContext = (TrackingContext) cachedData;
			if (trackingContext.IsEmpty()) throw new InvalidOperationException("Invalid TrackingContext: None of its individual activity Ids is set.");
			return trackingContext;
		}

		private static TrackingContextCache _instance = new TrackingContextCache();
		private readonly MemoryCache _cache;
	}
}
