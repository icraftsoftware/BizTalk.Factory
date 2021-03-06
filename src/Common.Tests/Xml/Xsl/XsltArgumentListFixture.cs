﻿#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;
using Be.Stateless.Linq.Extensions;
using NUnit.Framework;

namespace Be.Stateless.Xml.Xsl
{
	[TestFixture]
	public class XsltArgumentListFixture
	{
		#region Setup/Teardown

		[OneTimeSetUp]
		public void TestFixtureSetUp()
		{
			_extensions = new Dictionary<string, object> {
				{ "urn:extensions:one", new object() },
				{ "urn:extensions:two", new object() },
				{ "urn:extensions:ten", new object() }
			};
			_params = new Dictionary<XmlQualifiedName, object> {
				{ new XmlQualifiedName("p1", "urn:parameters"), new object() },
				{ new XmlQualifiedName("p2", "urn:parameters"), new object() },
				{ new XmlQualifiedName("p3", "urn:parameters"), new object() }
			};
		}

		#endregion

		[Test]
		public void Clone()
		{
			var arguments = new XsltArgumentList();
			_extensions.Each(e => arguments.AddExtensionObject(e.Key, e.Value));
			_params.Each(p => arguments.AddParam(p.Key.Name, p.Key.Namespace, p.Value));

			var copy = arguments.Clone();
			_extensions.Each(e => Assert.That(copy.GetExtensionObject(e.Key), Is.SameAs(e.Value)));
			_params.Each(p => Assert.That(copy.GetParam(p.Key.Name, p.Key.Namespace), Is.SameAs(p.Value)));
		}

		[Test]
		public void CopyConstructor()
		{
			var arguments = new System.Xml.Xsl.XsltArgumentList();
			_extensions.Each(e => arguments.AddExtensionObject(e.Key, e.Value));
			_params.Each(p => arguments.AddParam(p.Key.Name, p.Key.Namespace, p.Value));

			var copy = new XsltArgumentList(arguments);
			_extensions.Each(e => Assert.That(copy.GetExtensionObject(e.Key), Is.SameAs(e.Value)));
			_params.Each(p => Assert.That(copy.GetParam(p.Key.Name, p.Key.Namespace), Is.SameAs(p.Value)));
		}

		[Test]
		public void Initializer()
		{
			var arguments = new XsltArgumentList { new XsltArgument("param", "value") };
			Assert.That(arguments.GetParam("param", string.Empty), Is.Not.Null);
			Assert.That(arguments.GetParam("param", string.Empty), Is.EqualTo("value"));
		}

		[Test]
		public void Union()
		{
			var arguments = new XsltArgumentList();
			_extensions.Each(e => arguments.AddExtensionObject(e.Key, e.Value));
			_params.Each(p => arguments.AddParam(p.Key.Name, p.Key.Namespace, p.Value));

			_extensions.Add("urn:extensions:six", new object());
			_params.Add(new XmlQualifiedName("p4", "urn:parameters"), new object());

			var newArguments = new System.Xml.Xsl.XsltArgumentList();
			newArguments.AddExtensionObject(_extensions.Last().Key, _extensions.Last().Value);
			newArguments.AddParam(_params.Last().Key.Name, _params.Last().Key.Namespace, _params.Last().Value);

			var union = arguments.Union(newArguments);

			_extensions.Each(e => Assert.That(union.GetExtensionObject(e.Key), Is.SameAs(e.Value)));
			_params.Each(p => Assert.That(union.GetParam(p.Key.Name, p.Key.Namespace), Is.SameAs(p.Value)));
		}

		[Test]
		[SuppressMessage("ReSharper", "ImplicitlyCapturedClosure")]
		public void UnionThrowsWhenDuplicate()
		{
			var arguments = new XsltArgumentList();
			_extensions.Each(e => arguments.AddExtensionObject(e.Key, e.Value));
			_params.Each(p => arguments.AddParam(p.Key.Name, p.Key.Namespace, p.Value));

			var union1 = new System.Xml.Xsl.XsltArgumentList();
			union1.AddExtensionObject(_extensions.First().Key, _extensions.First().Value);
			Assert.That(
				() => arguments.Union(union1),
				Throws.ArgumentException);

			var union2 = new System.Xml.Xsl.XsltArgumentList();
			union2.AddParam(_params.First().Key.Name, _params.First().Key.Namespace, _params.First().Value);
			Assert.That(
				() => arguments.Union(union2),
				Throws.ArgumentException);
		}

		[Test]
		public void UnionWithNull()
		{
			var lhs = new XsltArgumentList();
			lhs.AddExtensionObject("urn:extensions:one", new object());
			lhs.AddParam("p1", "urn:parameters", new object());

			var union = lhs.Union((System.Xml.Xsl.XsltArgumentList) null);

			Assert.That(union, Is.Not.SameAs(lhs));
			Assert.That(union.GetExtensionObject("urn:extensions:one"), Is.SameAs(lhs.GetExtensionObject("urn:extensions:one")));
			Assert.That(union.GetParam("p1", "urn:parameters"), Is.SameAs(lhs.GetParam("p1", "urn:parameters")));
		}

		[Test]
		public void UnionYieldsNewInstance()
		{
			var lhs = new XsltArgumentList();
			lhs.AddExtensionObject("urn:extensions:one", new object());
			lhs.AddParam("p1", "urn:parameters", new object());

			var rhs = new XsltArgumentList();
			rhs.AddExtensionObject("urn:extensions:two", new object());
			rhs.AddParam("p2", "urn:parameters", new object());

			var union = lhs.Union(rhs);

			Assert.That(union, Is.Not.SameAs(lhs));
			Assert.That(union, Is.Not.SameAs(rhs));
		}

		private Dictionary<string, object> _extensions;
		private Dictionary<XmlQualifiedName, object> _params;
	}
}
