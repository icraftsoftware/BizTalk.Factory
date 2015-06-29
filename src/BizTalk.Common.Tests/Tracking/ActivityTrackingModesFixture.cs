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
using Be.Stateless.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Tracking
{
	/// <summary>
	/// Unit testing <see cref="ActivityTrackingModes"/> because it was originally a regular enum, i.e. not
	/// qualified as a <see cref="FlagsAttribute"/>. This is only to avoid any regression when being string
	/// de-/serialized through a <see cref="PipelineComponent"/>'s <see cref="IPropertyBag"/>.
	/// </summary>
	[TestFixture]
	public class ActivityTrackingModesFixture
	{
		[Test]
		public void ArchiveAndBodyHaveToBeCombined()
		{
			const ActivityTrackingModes sut = ActivityTrackingModes.Archive | ActivityTrackingModes.Body;

			Assert.That(sut.RequiresBodyArchiving());
			Assert.That(sut.RequiresBodyTracking());

			Assert.That(Convert.ToString(sut), Is.EqualTo("Body, Archive"));

			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Archive, Body"), Is.EqualTo(sut));
			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Body, Archive"), Is.EqualTo(sut));
			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Body"), Is.Not.EqualTo(sut));
			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Archive"), Is.Not.EqualTo(sut));
		}

		[Test]
		public void ArchiveAndClaimHaveToBeCombined()
		{
			const ActivityTrackingModes sut = ActivityTrackingModes.Archive | ActivityTrackingModes.Claim;

			Assert.That(sut.RequiresBodyArchiving());
			Assert.That(sut.RequiresBodyClaimChecking());

			Assert.That(Convert.ToString(sut), Is.EqualTo("Claim, Archive"));

			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Archive, Claim"), Is.EqualTo(sut));
			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Claim, Archive"), Is.EqualTo(sut));
			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Claim"), Is.Not.EqualTo(sut));
			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Archive"), Is.Not.EqualTo(sut));
		}

		[Test]
		public void ArchiveDoesNotRequireClaim()
		{
			const ActivityTrackingModes sut = ActivityTrackingModes.Archive;

			Assert.That(sut.RequiresBodyArchiving());
			Assert.That(sut.RequiresBodyClaimChecking(), Is.False);

			Assert.That(Convert.ToString(sut), Is.EqualTo("Archive"));

			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Archive"), Is.EqualTo(sut));
			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Archive, Claim"), Is.Not.EqualTo(sut));
		}

		[Test]
		public void ArchiveDoesNotRequiresBody()
		{
			const ActivityTrackingModes sut = ActivityTrackingModes.Archive;

			Assert.That(sut.RequiresBodyArchiving());
			Assert.That(sut.RequiresBodyTracking(), Is.False);

			Assert.That(Convert.ToString(sut), Is.EqualTo("Archive"));

			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Archive"), Is.EqualTo(sut));
			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Archive, Body"), Is.Not.EqualTo(sut));
		}

		[Test]
		public void BodyRequiresContext()
		{
			const ActivityTrackingModes sut = ActivityTrackingModes.Body;

			Assert.That(sut, Is.EqualTo(ActivityTrackingModes.Body | ActivityTrackingModes.Context));
			Assert.That(sut.RequiresBodyTracking());
			Assert.That(sut.RequiresContextTracking());
			Assert.That(sut.RequiresBodyArchiving(), Is.False);
			Assert.That(sut.RequiresBodyClaimChecking(), Is.False);

			Assert.That(Convert.ToString(sut), Is.EqualTo("Body"));

			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Body"), Is.EqualTo(sut));
			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Body, Context"), Is.EqualTo(sut));
			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Context"), Is.Not.EqualTo(sut));
		}

		[Test]
		public void ClaimRequiresBody()
		{
			const ActivityTrackingModes sut = ActivityTrackingModes.Claim;

			Assert.That(sut, Is.EqualTo(ActivityTrackingModes.Claim | ActivityTrackingModes.Body));
			Assert.That(sut.RequiresBodyClaimChecking());
			Assert.That(sut.RequiresBodyTracking());
			Assert.That(sut.RequiresBodyArchiving(), Is.False);

			Assert.That(Convert.ToString(sut), Is.EqualTo("Claim"));

			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Claim"), Is.EqualTo(sut));
			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Claim, Body"), Is.EqualTo(sut));
			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Body"), Is.Not.EqualTo(sut));
		}

		[Test]
		public void ContextRequiresStep()
		{
			const ActivityTrackingModes sut = ActivityTrackingModes.Context;

			Assert.That(sut, Is.EqualTo(ActivityTrackingModes.Context | ActivityTrackingModes.Step));
			Assert.That(sut.RequiresContextTracking());
			Assert.That(sut.RequiresStepTracking());

			Assert.That(Convert.ToString(sut), Is.EqualTo("Context"));

			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Context"), Is.EqualTo(sut));
			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Context, Step"), Is.EqualTo(sut));
			Assert.That(Enum.Parse(typeof(ActivityTrackingModes), "Step"), Is.Not.EqualTo(sut));
		}

		[Test]
		public void DiscardBodyClaimChecking()
		{
			var sut = (ActivityTrackingModes.Archive | ActivityTrackingModes.Claim).DiscardBodyClaimChecking();

			Assert.That(sut.RequiresBodyArchiving());
			Assert.That(sut.RequiresBodyClaimChecking(), Is.False);
			Assert.That(sut.RequiresBodyTracking());

			sut = (ActivityTrackingModes.Claim).DiscardBodyClaimChecking();

			Assert.That(sut.RequiresBodyArchiving(), Is.False);
			Assert.That(sut.RequiresBodyClaimChecking(), Is.False);
			Assert.That(sut.RequiresBodyTracking());
		}

		[Test]
		public void StepOnly()
		{
			const ActivityTrackingModes sut = ActivityTrackingModes.Step;

			Assert.That(sut.RequiresStepTracking());
			Assert.That(sut.RequiresContextTracking(), Is.False);
			Assert.That(sut.RequiresBodyTracking(), Is.False);
			Assert.That(sut.RequiresBodyClaimChecking(), Is.False);
			Assert.That(sut.RequiresBodyArchiving(), Is.False);

			Assert.That(Convert.ToString(sut), Is.EqualTo("Step"));
		}
	}
}
