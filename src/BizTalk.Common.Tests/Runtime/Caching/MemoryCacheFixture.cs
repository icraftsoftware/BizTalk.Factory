#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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
using System.Threading;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Runtime.Caching
{
	[TestFixture]
	public class MemoryCacheFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			_memoryCache = new MemoryCache("test");
		}

		#endregion

		[Test]
		public void AccessingItemRenewsItWhenUsingSlidingExpiration()
		{
			// Note: the real MemoryCache from System.runtime.caching does not update sliding expiration
			// data for expiration that is below 1 second, therefore we use longer timespans in this test
			var value = new object();

			_memoryCache.Add(new CacheItem("test", value), new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMilliseconds(2000.0) });

			Thread.Sleep(1000);
			var o = _memoryCache["test"]; // should renew for another 2000 ms
			Assert.That(o, Is.Not.Null);
			Thread.Sleep(1500);

			Assert.That(_memoryCache["test"], Is.SameAs(value));
		}

		[Test]
		public void AddReturnsFalseWhenKeyIsAlreadyInCache()
		{
			var value = new object();

			_memoryCache.Add(new CacheItem("test", value), new CacheItemPolicy());

			Assert.That(_memoryCache.Add(new CacheItem("test", new object()), new CacheItemPolicy()), Is.False);
		}

		[Test]
		public void ConstructorThrowsWhenNameIsNull()
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			Assert.That(
				() => new MemoryCache(null),
				Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void ContainsReturnsFalseWithExistingSlidingExpiredKey()
		{
			var value = new object();

			_memoryCache.Add(new CacheItem("test", value), new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMilliseconds(50.0) });
			Thread.Sleep(60);

			Assert.That(_memoryCache.Contains("test"), Is.False);
		}

		[Test]
		public void ContainsReturnsFalseWithUnexistingKey()
		{
			Assert.That(_memoryCache.Contains("stuff"), Is.False);
		}

		[Test]
		public void ContainsReturnsTrueWithExistingUnexpiredKey()
		{
			_memoryCache.Add(new CacheItem("test", new object()), new CacheItemPolicy());

			Assert.That(_memoryCache.Contains("test"), Is.True);
		}

		[Test]
		public void ContainsReturnsTrueWithExistingUnexpiredKeyWhenUsingSlidingExpiration()
		{
			_memoryCache.Add(new CacheItem("test", new object()), new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMilliseconds(50.0) });

			Assert.That(_memoryCache.Contains("test"), Is.True);
		}

		[Test]
		public void ContainsThrowsWhenKeyIsNull()
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			Assert.That(() => _memoryCache.Contains(null), Throws.ArgumentNullException);
		}

		[Test]
		public void IndexerReturnsItemWithExistingUnexpiredKey()
		{
			var value = new object();

			_memoryCache.Add(new CacheItem("test", value), new CacheItemPolicy());

			Assert.That(_memoryCache["test"], Is.SameAs(value));
		}

		[Test]
		public void IndexerReturnsItemWithExistingUnexpiredKeyWhenUsingAbsoluteExpiration()
		{
			var value = new object();

			_memoryCache.Add(new CacheItem("test", value), new CacheItemPolicy { AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow + TimeSpan.FromMilliseconds(50.0)) });

			Assert.That(_memoryCache["test"], Is.SameAs(value));
		}

		[Test]
		public void IndexerReturnsNullWithExistingSlidingExpiredKey()
		{
			var value = new object();

			_memoryCache.Add(new CacheItem("test", value), new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMilliseconds(50.0) });
			Thread.Sleep(60);

			Assert.That(_memoryCache["test"], Is.Null);
		}

		[Test]
		public void ItemIsNotRenewedWhenUsingAbsoluteExpiration()
		{
			var value = new object();

			_memoryCache.Add(new CacheItem("test", value), new CacheItemPolicy { AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow + TimeSpan.FromMilliseconds(50.0)) });
			Thread.Sleep(30);
#pragma warning disable 168
			// ReSharper disable once UnusedVariable
			var o = _memoryCache["test"];
#pragma warning restore 168
			Thread.Sleep(30);

			Assert.That(_memoryCache["test"], Is.Null);
		}

		[Test]
		public void RemoveMakesItemNoLongerAccessible()
		{
			var value = new object();

			_memoryCache.Add(new CacheItem("test", value), new CacheItemPolicy());

			_memoryCache.Remove("test");

			Assert.That(_memoryCache["test"], Is.Null);
		}

		[Test]
		public void RemoveReturnsRemovedItem()
		{
			var value = new object();

			_memoryCache.Add(new CacheItem("test", value), new CacheItemPolicy());

			Assert.That(_memoryCache.Remove("test"), Is.SameAs(value));
		}

		private MemoryCache _memoryCache;
	}
}
