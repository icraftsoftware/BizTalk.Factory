#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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

using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Transforms.ToSql.Procedures.Batch;
using Be.Stateless.BizTalk.Transforms.ToXml;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Xml.Xsl
{
	[TestFixture]
	public class XslCompiledTransformDescriptorFixture
	{
		[Test]
		public void ImplicitlyReliesOnEmbeddedXmlResolver()
		{
			Assert.That(() => new XslCompiledTransformDescriptor(typeof(CompositeTransform)), Throws.Nothing);
		}

		[Test]
		public void MessageContextRequirementDetection()
		{
			var td1 = new XslCompiledTransformDescriptor(typeof(AnyToAddPart));
			Assert.That(td1.ExtensionRequirements, Is.EqualTo(ExtensionRequirements.MessageContext));
			Assert.That(td1.NamespaceResolver.LookupNamespace("bf"), Is.EqualTo(BizTalkFactoryProperties.EnvelopeSpecName.Namespace));
			Assert.That(td1.NamespaceResolver.LookupNamespace("tp"), Is.EqualTo(TrackingProperties.MessagingStepActivityId.Namespace));

			var td2 = new XslCompiledTransformDescriptor(typeof(BatchContentToAnyEnvelope));
			Assert.That(td2.ExtensionRequirements, Is.EqualTo(ExtensionRequirements.None));
			Assert.That(td2.NamespaceResolver, Is.Null);
		}
	}
}
