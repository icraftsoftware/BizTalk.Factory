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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Be.Stateless.Extensions;
using Be.Stateless.Linq.Extensions;

#if NET35

// ReSharper disable CheckNamespace

namespace System.Runtime.Caching
{
	/// <summary>
	/// A simple substitute for the MemoryCache class from .NET 4.0, offering the same interface in .NET 3.5.
	/// </summary>
	internal class MemoryCache
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryCache"/> class.
		/// </summary>
		/// <param name="name">
		/// The name of this cache. Not actually used in this implementation.
		/// </param>
		public MemoryCache(string name)
		{
			if (name.IsNullOrEmpty()) throw new ArgumentNullException("name");
			_name = name;
		}

		/// <summary>
		/// The name of this cache instance.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets a value in the cache by using the default indexer property.
		/// </summary>
		/// <param name="key">
		/// A unique identifier for the cache value to get.
		/// </param>
		/// <returns>
		/// The value in the cache instance for the specified key, if the entry exists; otherwise, <c>null</c>.
		/// </returns>
		public object this[string key]
		{
			get
			{
				if (key.IsNullOrEmpty()) throw new ArgumentNullException("key");

				_lock.EnterReadLock();

				CacheItem item;
				var itemIsFound = _cache.TryGetValue(key, out item);
				if (itemIsFound && !item.Policy.HasExpired) item.Policy.Renew();

				_lock.ExitReadLock();
				ScheduleItemExpirationCheck();

				return itemIsFound && !item.Policy.HasExpired ? item.Value : null;
			}
		}

		/// <summary>
		/// Determines whether a cache entry exists in the cache.
		/// </summary>
		/// <param name="key">
		/// A unique identifier for the cache entry to search for.
		/// </param>
		/// <returns>
		/// <c>true</c> if the cache contains a cache entry whose key matches <paramref name="key"/>; otherwise,
		/// <c>false</c>.
		/// </returns>
		public bool Contains(string key)
		{
			if (key == null) throw new ArgumentNullException("key");

			_lock.EnterReadLock();

			CacheItem item;
			var itemIsFound = _cache.TryGetValue(key, out item);
			var result = itemIsFound && !item.Policy.HasExpired;

			_lock.ExitReadLock();
			ScheduleItemExpirationCheck();

			return result;
		}

		/// <summary>
		/// Tries to insert a cache entry into the cache as a <see cref="CacheItem"/> instance, and adds details about how
		/// the entry should be evicted.
		/// </summary>
		/// <param name="item">
		/// The object to add.
		/// </param>
		/// <param name="policy">
		/// An object that contains eviction details for the cache entry.
		/// </param>
		/// <returns>
		/// <c>true</c> if insertion succeeded, or <c>false</c> if there is an already an entry in the cache that has the
		/// same key as <paramref name="item"/>.
		/// </returns>
		public bool Add(CacheItem item, CacheItemPolicy policy)
		{
			if (item == null) throw new ArgumentNullException("item");
			if (policy == null) throw new ArgumentNullException("policy");

			_lock.EnterWriteLock();

			CacheItem existingItem;
			var itemIsAlreadyInCache = _cache.TryGetValue(item.Key, out existingItem);
			if (itemIsAlreadyInCache && existingItem.Policy.HasExpired)
			{
				_cache.Remove(item.Key);
				itemIsAlreadyInCache = false;
			}
			if (!itemIsAlreadyInCache)
			{
				item.Policy = policy;
				policy.Renew();
				_cache.Add(item.Key, item);
			}

			_lock.ExitWriteLock();
			ScheduleItemExpirationCheck();

			return !itemIsAlreadyInCache;
		}

		/// <summary>
		/// Removes the cache entry from the cache.
		/// </summary>
		/// <param name="key">
		/// A unique identifier for the cache entry.
		/// </param>
		/// <returns>
		/// An object that represents the value of the removed cache entry that was specified by the key, or null if the
		/// specified entry was not found.
		/// </returns>
		public object Remove(string key)
		{
			if (key == null) throw new ArgumentNullException("key");

			_lock.EnterWriteLock();

			CacheItem item;
			var itemIsFound = _cache.TryGetValue(key, out item);
			if (itemIsFound) _cache.Remove(key);

			_lock.ExitWriteLock();
			ScheduleItemExpirationCheck();

			return itemIsFound ? item.Value : null;
		}

		private void ScheduleItemExpirationCheck()
		{
			// check for expired items every minute
			if (_lastExpiredItemCheckTime.AddMinutes(1.0) < DateTime.UtcNow) ThreadPool.QueueUserWorkItem(RemoveExpiredItems);
		}

		private void RemoveExpiredItems(object state /*ignored*/)
		{
			// check if there is any expired item 
			_lock.EnterReadLock();
			var atLeastOneExpiredItem = _cache.Any(keyValuePair => keyValuePair.Value.Policy.HasExpired);
			_lock.ExitReadLock();

			if (atLeastOneExpiredItem)
			{
				// snapshot enumerator so as not to modify it while walking it
				_lock.EnterReadLock();
				var expiredItems = _cache
					.Where(keyValuePair => keyValuePair.Value.Policy.HasExpired)
					.ToArray();
				_lock.ExitReadLock();

				// do the actual removal of expired items
				_lock.EnterWriteLock();
				expiredItems.Each(keyValuePair => _cache.Remove(keyValuePair.Key));
				_lock.ExitWriteLock();
			}

			_lastExpiredItemCheckTime = DateTime.UtcNow;
		}

		private readonly Dictionary<string, CacheItem> _cache = new Dictionary<string, CacheItem>(32);
		private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
		private readonly string _name;
		internal DateTime _lastExpiredItemCheckTime = DateTime.UtcNow;
	}

	internal class CacheItem
	{
		internal CacheItem(string key, object value)
		{
			if (key.IsNullOrEmpty()) throw new ArgumentNullException("key");
			_key = key;
			_value = value;
		}

		internal object Value
		{
			get { return _value; }
		}

		internal string Key
		{
			get { return _key; }
		}

		internal CacheItemPolicy Policy { get; set; }

		private readonly string _key;
		private readonly object _value;
	}

	internal class CacheItemPolicy
	{
		public CacheItemPolicy()
		{
			SlidingExpiration = TimeSpan.Zero;
			AbsoluteExpiration = DateTimeOffset.MaxValue;
		}

		public TimeSpan SlidingExpiration { get; set; }

		public DateTimeOffset AbsoluteExpiration { get; set; }

		internal bool HasExpired
		{
			get { return AbsoluteExpiration < DateTimeOffset.Now; }
		}

		private bool HasSlidingExpiration
		{
			get { return SlidingExpiration != TimeSpan.Zero; }
		}

		internal void Renew()
		{
			if (HasSlidingExpiration) AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow + SlidingExpiration);
		}
	}
}

#endif
