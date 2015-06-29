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

using Be.Stateless.BizTalk.Xml;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Streaming
{
	[TestFixture]
	public class XmlTranslationSetFixture
	{
		[Test]
#if !DEBUG
		[Ignore("Only to be run in DEBUG configuration.")]
#endif
		public void CheckItemsUnivoquenessThrowsWhenConflictingReplacementPatterns()
		{
			Assert.That(
				() => new XmlTranslationSet {
					Items = new[] {
						new XmlNamespaceTranslation("sourceUrnA", "targetUrnA1"),
						new XmlNamespaceTranslation("sourceUrnA", "targetUrnA2"),
						new XmlNamespaceTranslation("sourceUrnB", "targetUrnB1"),
						new XmlNamespaceTranslation("sourceUrnB", "targetUrnB2")
					}
				},
				Throws.ArgumentException.With.Message.EqualTo(
					"[sourceUrnA], [sourceUrnB] matchingPatterns have respectively the following conflicting replacementPatterns: " +
						"[targetUrnA1, targetUrnA2], [targetUrnB1, targetUrnB2]."));
		}

		[Test]
		public void UnionWithOverride()
		{
			var contextReplacementSet = new XmlTranslationSet {
				Override = true,
				Items = new[] { new XmlNamespaceTranslation("contextSourceUrn", "contextTargetUrn") }
			};
			var pipelineReplacementSet = new XmlTranslationSet {
				Items = new[] { new XmlNamespaceTranslation("pipelineSourceUrn", "pipelineTargetUrn") }
			};

			Assert.That(
				contextReplacementSet.Union(pipelineReplacementSet),
				Is.EqualTo(
					new XmlTranslationSet {
						Override = true,
						Items = new[] {
							new XmlNamespaceTranslation("contextSourceUrn", "contextTargetUrn")
						}
					}));
		}

		[Test]
		public void UnionWithoutOverride()
		{
			var contextReplacementSet = new XmlTranslationSet {
				Override = false,
				Items = new[] {
					new XmlNamespaceTranslation("contextSourceUrn", "contextTargetUrn"),
					new XmlNamespaceTranslation("commonSourceUrn", "commonTargetUrn")
				}
			};
			var pipelineReplacementSet = new XmlTranslationSet {
				Items = new[] {
					new XmlNamespaceTranslation("pipelineSourceUrn", "pipelineTargetUrn"),
					new XmlNamespaceTranslation("commonSourceUrn", "commonTargetUrn")
				}
			};

			Assert.That(
				contextReplacementSet.Union(pipelineReplacementSet),
				Is.EqualTo(
					new XmlTranslationSet {
						Override = false,
						Items = new[] {
							new XmlNamespaceTranslation("contextSourceUrn", "contextTargetUrn"),
							new XmlNamespaceTranslation("commonSourceUrn", "commonTargetUrn"),
							new XmlNamespaceTranslation("pipelineSourceUrn", "pipelineTargetUrn")
						}
					}));
		}
	}
}
