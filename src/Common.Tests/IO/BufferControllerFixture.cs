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

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Be.Stateless.IO
{
	[TestFixture]
	public class BufferControllerFixture
	{
		[Test]
		public void AppendBufferListLessThanAvailable()
		{
			var buffer = new byte[10];
			var controller = new BufferController(buffer, 0, buffer.Length);
			var buffers = new[] {
				new byte[] { 1, 2, 3 },
				new byte[] { 4, 5, 6, 7 },
				new byte[] { 8, 9 }
			};

			Assert.That(controller.Append(buffers), Is.Empty);
			Assert.That(buffer, Is.EqualTo(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }));
			Assert.That(controller.Availability, Is.EqualTo(1));
			Assert.That(controller.Count, Is.EqualTo(9));
		}

		[Test]
		public void AppendBufferListMoreThanAvailable()
		{
			var buffer = new byte[9];
			var controller = new BufferController(buffer, 0, buffer.Length);
			var buffers = new[] {
				new byte[] { 1, 2, 3 },
				new byte[] { 4, 5, 6, 7 },
				new byte[] { 8, 9, 8 },
				new byte[] { 7, 6, 5 }
			};

			Assert.That(
				controller.Append(buffers),
				Is.EqualTo(
					new[] {
						new byte[] { 8 },
						new byte[] { 7, 6, 5 }
					}));
			Assert.That(buffer, Is.EqualTo(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
			Assert.That(controller.Availability, Is.EqualTo(0));
			Assert.That(controller.Count, Is.EqualTo(9));
		}

		[Test]
		public void AppendBufferListThatIsEmpty()
		{
			var controller = new BufferController(new byte[3], 0, 3);
			Assert.That(controller.Append(Enumerable.Empty<byte[]>()), Is.EqualTo(Enumerable.Empty<byte[]>()));
			Assert.That(controller.Availability, Is.EqualTo(3));
			Assert.That(controller.Count, Is.EqualTo(0));
		}

		[Test]
		public void AppendBufferListThrows()
		{
			var controller = new BufferController(new byte[3], 0, 3);
			Assert.That(
				() => controller.Append((IEnumerable<byte[]>) null),
				Throws.InstanceOf<ArgumentNullException>()
					.With.Message.EqualTo("Value cannot be null.\r\nParameter name: buffers"));
		}

		[Test]
		public void AppendBufferListWhenNoAvailability()
		{
			var controller = new BufferController(new byte[3], 3, 0);
			var buffers = new[] {
				new byte[] { 1, 2, 3 },
				new byte[] { 4, 5, 6, 7 },
				new byte[] { 8, 9, 8 }
			};
			Assert.That(controller.Append(buffers), Is.EqualTo(buffers));
			Assert.That(controller.Availability, Is.EqualTo(0));
			Assert.That(controller.Count, Is.EqualTo(0));
		}

		[Test]
		public void AppendEmptyArray()
		{
			var controller = new BufferController(new byte[3], 0, 3);
			Assert.That(controller.Append(new byte[] { }), Is.EqualTo(new byte[] { }));
			Assert.That(controller.Availability, Is.EqualTo(3));
			Assert.That(controller.Count, Is.EqualTo(0));
			Assert.That(controller.Availability, Is.EqualTo(3));
			Assert.That(controller.Count, Is.EqualTo(0));
		}

		[Test]
		public void AppendFromArrayLessThanAvailable()
		{
			var buffer = new byte[3];
			var controller = new BufferController(buffer, 0, buffer.Length);
			Assert.That(controller.Append(new byte[] { 1, 2, 3 }, 1, 2), Is.Empty);
			Assert.That(buffer, Is.EqualTo(new byte[] { 2, 3, 0 }));
			Assert.That(controller.Availability, Is.EqualTo(1));
			Assert.That(controller.Count, Is.EqualTo(2));
		}

		[Test]
		public void AppendFromArrayMoreThanAvailable()
		{
			var buffer = new byte[3];
			var controller = new BufferController(buffer, 0, buffer.Length);
			Assert.That(controller.Append(new byte[] { 1, 2, 3, 4, 5, 6, 7 }, 2, 5), Is.EqualTo(new byte[] { 6, 7 }));
			Assert.That(buffer, Is.EqualTo(new byte[] { 3, 4, 5 }));
			Assert.That(controller.Availability, Is.EqualTo(0));
			Assert.That(controller.Count, Is.EqualTo(3));
		}

		[Test]
		public void AppendFromArrayThrows()
		{
			var controller = new BufferController(new byte[3], 0, 3);

			Assert.That(
				() => controller.Append(null, 0, 0),
				Throws.InstanceOf<ArgumentNullException>()
					.With.Message.EqualTo("Value cannot be null.\r\nParameter name: bytes"));

			Assert.That(
				() => controller.Append(new byte[0], -1, 0),
				Throws.InstanceOf<ArgumentException>()
					.With.Message.EqualTo("Cannot be negative.\r\nParameter name: offset"));
			Assert.That(
				() => controller.Append(new byte[0], 1, -1),
				Throws.InstanceOf<ArgumentException>()
					.With.Message.EqualTo("Cannot be negative.\r\nParameter name: count"));

			Assert.That(
				() => controller.Append(new byte[0], 1, 0),
				Throws.InstanceOf<ArgumentException>()
					.With.Message.EqualTo("The sum of offset and count is greater than the byte array length."));
			Assert.That(
				() => controller.Append(new byte[2], 0, 3),
				Throws.InstanceOf<ArgumentException>()
					.With.Message.EqualTo("The sum of offset and count is greater than the byte array length."));
			Assert.That(
				() => controller.Append(new byte[0], 0, 0),
				Throws.Nothing);
		}

		[Test]
		public void AppendFromArrayWhenCountIsZero()
		{
			var controller = new BufferController(new byte[3], 0, 3);
			Assert.That(controller.Append(new byte[0], 0, 0), Is.Null);
			Assert.That(controller.Availability, Is.EqualTo(3));
			Assert.That(controller.Count, Is.EqualTo(0));
		}

		[Test]
		public void AppendFromArrayWhenNoAvailability()
		{
			var controller = new BufferController(new byte[3], 3, 0);
			Assert.That(controller.Append(new byte[] { 1, 2, 3, 4, 5, 6, 7 }, 2, 5), Is.EqualTo(new byte[] { 3, 4, 5, 6, 7 }));
			Assert.That(controller.Availability, Is.EqualTo(0));
			Assert.That(controller.Count, Is.EqualTo(0));
		}

		[Test]
		public void AppendLessThanAvailable()
		{
			var controller = new BufferController(new byte[3], 0, 3);
			Assert.That(controller.Append(new byte[] { 1, 2, 3 }), Is.Null);
			Assert.That(controller.Availability, Is.EqualTo(0));
			Assert.That(controller.Count, Is.EqualTo(3));
		}

		[Test]
		public void AppendMoreThanAvailable()
		{
			var controller = new BufferController(new byte[3], 0, 3);
			Assert.That(controller.Append(new byte[] { 1, 2, 3, 4, 5 }), Is.EqualTo(new byte[] { 4, 5 }));
			Assert.That(controller.Availability, Is.EqualTo(0));
			Assert.That(controller.Count, Is.EqualTo(3));
		}

		[Test]
		public void AppendNullArray()
		{
			var controller = new BufferController(new byte[3], 0, 3);
			Assert.That(controller.Append((byte[]) null), Is.Null);
			Assert.That(controller.Availability, Is.EqualTo(3));
			Assert.That(controller.Count, Is.EqualTo(0));
		}

		[Test]
		public void AppendWhenNoAvailability()
		{
			var controller = new BufferController(new byte[] { 1, 2, 3 }, 3, 0);
			Assert.That(controller.Append(new byte[] { 4, 5 }), Is.EqualTo(new byte[] { 4, 5 }));
			Assert.That(controller.Availability, Is.EqualTo(0));
			Assert.That(controller.Count, Is.EqualTo(0));
		}

		[Test]
		public void AppendWithReadDelegate()
		{
			var buffer = new byte[10];
			var controller = new BufferController(buffer, 0, buffer.Length);
			controller.Append(new byte[] { 0, 1 });

			controller.Append(Read);

			Assert.That(buffer, Is.EqualTo(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 0, 0 }));
			Assert.That(controller.Availability, Is.EqualTo(2));
			Assert.That(controller.Count, Is.EqualTo(8));
		}

		[Test]
		public void AppendWithReadDelegateThrowsIfNoAvailability()
		{
			var buffer = new byte[0];
			var controller = new BufferController(buffer, 0, buffer.Length);
			Assert.That(
				() => controller.Append(Read),
				Throws.TypeOf<InvalidOperationException>()
					.With.Message.EqualTo(string.Format("{0} has no more availability to append further bytes to buffer.", typeof(BufferController).Name)));
		}

		private int Read(byte[] buffer, int offset, int count)
		{
			var bytes = new byte[] { 2, 3, 4, 5, 6, 7 };
			count = Math.Min(bytes.Length, count);
			Buffer.BlockCopy(bytes, 0, buffer, offset, count);
			return count;
		}
	}
}
