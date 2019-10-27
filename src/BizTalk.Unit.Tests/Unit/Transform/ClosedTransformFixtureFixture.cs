﻿#region Copyright & License

// Copyright © 2012 - 2019 François Chabot
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

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Be.Stateless.BizTalk.Message;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Unit.Resources;
using BTF2Schemas;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	[TestFixture]
	public class ClosedTransformFixtureFixture : ClosedTransformFixture<IdentityTransform>
	{
		[Test]
		public void InvalidTransformResultThrows()
		{
			using (var stream = new MemoryStream(Encoding.Default.GetBytes(MessageFactory.CreateMessage<btf2_services_header>().OuterXml)))
			{
				var setup = Given.Message(stream).Transform.OutputsXml().ConformingTo<btf2_services_header>().WithStrictConformanceLevel();
				Assert.That(() => setup.Execute(), Throws.InstanceOf<XmlSchemaValidationException>());
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public void ScalarAssertion()
		{
			using (var stream = new MemoryStream(Encoding.Default.GetBytes(_document.OuterXml)))
			{
				var setup = Given.Message(stream).Transform.OutputsXml().ConformingTo<btf2_services_header>().WithStrictConformanceLevel();
				var result = setup.Execute();
				result.XmlNamespaceManager.AddNamespace("tns", typeof(btf2_services_header).GetMetadata().TargetNamespace);
				Assert.That(result.SelectSingleNode("//*[1]/tns:sendBy/text()").Value, Is.EqualTo("2012-04-12T12:13:14"));
			}
		}

		[Test]
		public void StringJoinAssertion()
		{
			using (var stream = new MemoryStream(Encoding.Default.GetBytes(_document.OuterXml)))
			{
				var setup = Given.Message(stream).Transform.OutputsXml().ConformingTo<btf2_services_header>().WithStrictConformanceLevel();
				var result = setup.Execute();
				result.XmlNamespaceManager.AddNamespace("tns", typeof(btf2_services_header).GetMetadata().TargetNamespace);
				Assert.That(result.StringJoin("//tns:sendBy"), Is.EqualTo("2012-04-12T12:13:14#2012-04-12T23:22:21"));
			}
		}

		[Test]
		public void ValidTransformResultDoesNotThrow()
		{
			using (var stream = new MemoryStream(Encoding.Default.GetBytes(_document.OuterXml)))
			{
				var setup = Given.Message(stream).Transform.OutputsXml().ConformingTo<btf2_services_header>().WithStrictConformanceLevel();
				Assert.That(() => setup.Execute(), Throws.Nothing);
			}
		}

		[Test]
		public void XPathAssertion()
		{
			using (var stream = new MemoryStream(Encoding.Default.GetBytes(_document.OuterXml)))
			{
				var setup = Given.Message(stream).Transform.OutputsXml().ConformingTo<btf2_services_header>().WithStrictConformanceLevel();
				var result = setup.Execute();
				result.XmlNamespaceManager.AddNamespace("tns", typeof(btf2_services_header).GetMetadata().TargetNamespace);
				Assert.That(result.Select("//tns:sendBy").Count, Is.EqualTo(2));
			}
		}

		private readonly XmlDocument _document = MessageFactory.CreateMessage<btf2_services_header>(ResourceManager.LoadString("Data.Message.xml"));
	}
}
