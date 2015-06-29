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
using System.Text;
using NUnit.Framework;

namespace Be.Stateless.Text
{
	[TestFixture]
	public class EncodingConverterFixture
	{
		[Test]
		public void CanConvertFrom()
		{
			var sut = new EncodingConverter();
			Assert.That(sut.CanConvertFrom(typeof(string)));
		}

		[Test]
		public void CanConvertTo()
		{
			var sut = new EncodingConverter();
			Assert.That(sut.CanConvertTo(typeof(string)));
		}

		[Test]
		public void ConvertFromThrowsWhenEncodingIsUnknown()
		{
			const string value = "utf-48";
			var sut = new EncodingConverter();
			Assert.That(
				() => sut.ConvertFrom(value),
				Throws.TypeOf<ArgumentException>().With.Message.StartsWith("'utf-48' is not a supported encoding name."));
		}

		[Test]
		public void ConvertFromThrowsWhenInvalidFormat()
		{
			const string value = "utf-8, bam";
			var sut = new EncodingConverter();
			Assert.That(
				() => sut.ConvertFrom(value),
				Throws.TypeOf<NotSupportedException>().With.Message.EqualTo(string.Format("'{0}' format is invalid and cannot be parsed into a {1}.", value, typeof(Encoding).Name)));
		}

		[Test]
		public void ConvertFromThrowsWhenNullOrEmptyString()
		{
			var sut = new EncodingConverter();
			Assert.That(
				() => sut.ConvertFrom(""),
				Throws.TypeOf<NotSupportedException>().With.Message.EqualTo(string.Format("Cannot parse a null or empty string into a {0}.", typeof(Encoding).Name)));
		}

		[Test]
		public void ConvertFromWithBom()
		{
			var encoding = new UTF8Encoding(true);
			var displayName = string.Format("{0} {1}", encoding.WebName, EncodingConverter.ENCODING_SIGNATURE_MODIFIER);

			var sut = new EncodingConverter();
			var convertedObject = sut.ConvertFrom(displayName);

			Assert.That(convertedObject, Is.InstanceOf<UTF8Encoding>());
			var convertedEncoding = convertedObject as UTF8Encoding;
			// ReSharper disable PossibleNullReferenceException
			Assert.That(convertedEncoding.WebName, Is.EqualTo(encoding.WebName));
			Assert.That(convertedEncoding.GetPreamble().Length, Is.GreaterThan(0));
			// ReSharper restore PossibleNullReferenceException
		}

		[Test]
		public void ConvertFromWithBomIsCaseInsensitive()
		{
			var encoding = new UTF8Encoding(true);
			var displayName = string.Format("{0} {1}", encoding.WebName, EncodingConverter.ENCODING_SIGNATURE_MODIFIER.ToUpper());

			var sut = new EncodingConverter();
			var convertedObject = sut.ConvertFrom(displayName);

			Assert.That(convertedObject, Is.EqualTo(encoding));

			Assert.That(convertedObject, Is.InstanceOf<UTF8Encoding>());
			var convertedEncoding = convertedObject as UTF8Encoding;
			// ReSharper disable PossibleNullReferenceException
			Assert.That(convertedEncoding.WebName, Is.EqualTo(encoding.WebName));
			Assert.That(convertedEncoding.GetPreamble().Length, Is.GreaterThan(0));
			// ReSharper restore PossibleNullReferenceException
		}

		[Test]
		public void ConvertFromWithoutBom()
		{
			var encoding = new UTF8Encoding();
			var displayName = string.Format("{0}", encoding.WebName);

			var sut = new EncodingConverter();
			var convertedObject = sut.ConvertFrom(displayName);

			Assert.That(convertedObject, Is.InstanceOf<UTF8Encoding>());
			var convertedEncoding = convertedObject as UTF8Encoding;
			// ReSharper disable PossibleNullReferenceException
			Assert.That(convertedEncoding.WebName, Is.EqualTo(encoding.WebName));
			Assert.That(convertedEncoding.GetPreamble().Length, Is.EqualTo(0));
			// ReSharper restore PossibleNullReferenceException
		}

		[Test]
		public void ConvertToWithBom()
		{
			var encoding = new UTF8Encoding(true);

			var sut = new EncodingConverter();
			var displayName = sut.ConvertTo(encoding, typeof(string));

			Assert.That(displayName, Is.EqualTo(string.Format("{0} {1}", encoding.WebName, EncodingConverter.ENCODING_SIGNATURE_MODIFIER)));
		}

		[Test]
		public void ConvertToWithoutBom()
		{
			var encoding = new UTF8Encoding();

			var sut = new EncodingConverter();
			var displayName = sut.ConvertTo(encoding, typeof(string));

			Assert.That(displayName, Is.EqualTo(string.Format("{0}", encoding.WebName)));
		}
	}
}
