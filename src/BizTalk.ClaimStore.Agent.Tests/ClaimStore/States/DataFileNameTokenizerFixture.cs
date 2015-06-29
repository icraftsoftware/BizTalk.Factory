#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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
using NUnit.Framework;

namespace Be.Stateless.BizTalk.ClaimStore.States
{
	[TestFixture]
	public class DataFileNameTokenizerFixture
	{
		[Test]
		public void CaptureDate()
		{
			Assert.That(
				"201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150626160941.gathered".Tokenize().CaptureDate,
				Is.EqualTo("20130615"));
		}

		[Test]
		public void Id()
		{
			Assert.That(
				"201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150626160941.gathered".Tokenize().Id,
				Is.EqualTo("8F341A2D6FD7416B87073A0132DD51AE"));
		}

		[Test]
		[TestCaseSource(typeof(DataFileNameTokenizerFixture), "InvalidDataFilePaths")]
		public void IsNotValidDataFilePath(string filePath)
		{
			Assert.That(filePath.IsValidDataFilePath(), Is.False);
		}

		[Test]
		[TestCaseSource(typeof(DataFileNameTokenizerFixture), "ValidDataFilePaths")]
		public void IsValidDataFilePath(string filePath)
		{
			Assert.That(filePath.IsValidDataFilePath(), Is.True);
		}

		[Test]
		public void LockTime()
		{
			Assert.That(
				"201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150626160941.gathered".Tokenize().LockTime,
				Is.EqualTo(new DateTime(2015, 6, 26, 16, 09, 41, DateTimeKind.Utc).ToUniversalTime()));

			Assert.That(
				"201306158F341A2D6FD7416B87073A0132DD51AE.chk".Tokenize().LockTime,
				Is.EqualTo(null));
		}

		[Test]
		public void State()
		{
			Assert.That(
				"201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150626160941.gathered".Tokenize().State,
				Is.EqualTo(GatheredDataFile.STATE_TOKEN));
		}

		[Test]
		public void TokenizeThrowsWhenDataFileNameIsInvalid()
		{
			Assert.That(
				() => "201306158F341A2D6FD7416B87073A0132DD51AE.txt".Tokenize().CaptureDate,
				Throws.ArgumentException
					.With.Message.StartsWith("Claim Store Agent does not recognized the message body's data file path: '201306158F341A2D6FD7416B87073A0132DD51AE.txt'."));
		}

		[Test]
		public void TrackingMode()
		{
			Assert.That(
				"201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150626160941.gathered".Tokenize().TrackingMode,
				Is.EqualTo("chk"));
		}

		[Test]
		public void UnlockedFilePath()
		{
			Assert.That(
				"201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150626160941.gathered".Tokenize().UnlockedFilePath,
				Is.EqualTo("201306158F341A2D6FD7416B87073A0132DD51AE.chk"));
		}

		private IEnumerable<string> ValidDataFilePaths
		{
			get
			{
				yield return @"201306158F341A2D6FD7416B87073A0132DD51AE.trk";
				yield return @"c:\temp\201306158F341A2D6FD7416B87073A0132DD51AE.trk";
				yield return @"201306158F341A2D6FD7416B87073A0132DD51AE.chk";
				yield return @"c:\temp\201306158F341A2D6FD7416B87073A0132DD51AE.chk";
				yield return @"201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150626160941.gathered";
				yield return @"c:\temp\201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150626160941.gathered";
				yield return @"201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150626160941.released";
				yield return @"c:\temp\201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150626160941.released";
				yield return @"201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150626160941.locked";
				yield return @"c:\temp\201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150626160941.locked";
			}
		}

		private IEnumerable<string> InvalidDataFilePaths
		{
			get
			{
				yield return @"201306158F341A2D6FD7416B87073A0132DD51AE.TRK";
				yield return @"201306158F341A2D6FD7416B87073A0132DD51AE.txt";
				yield return @"201306158F341A2D6FD7416B87073A0132DD51AE.CHK.20150626160941.GATHERED";
				yield return @"201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150626160941.unlocked";
				yield return @"c:\temp\201306158F341A2D6FD7416B87073A0132DD51AE.chk.gathered";
				yield return @"201306158F341A2D6FD7416B87073A0132DD51AE.txt.20150626160941.released";
			}
		}
	}
}
