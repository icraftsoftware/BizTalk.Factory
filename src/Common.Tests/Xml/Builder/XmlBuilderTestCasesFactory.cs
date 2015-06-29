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

using System.Collections;
using NUnit.Framework;

namespace Be.Stateless.Xml.Builder
{
	internal static class XmlBuilderTestCasesFactory
	{
		public static IEnumerable XmlElementBuilders
		{
			get
			{
				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root",
						Nodes = new[] { new XmlElementBuilder { LocalName = "child" } }
					},
					"<root><child /></root>")
					.SetName("RootHasChildThatIsEmpty");

				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root",
						Attributes = new[] {
							new XmlAttributeBuilder { LocalName = "a1", Value = "one" },
							new XmlAttributeBuilder { LocalName = "a2", Value = "two" }
						},
						Nodes = new[] {
							new XmlElementBuilder {
								LocalName = "child",
								Attributes = new[] {
									new XmlAttributeBuilder { LocalName = "a1", Value = "one" },
									new XmlAttributeBuilder { LocalName = "a2", Value = "two" }
								},
							}
						}
					},
					"<root a1=\"one\" a2=\"two\"><child a1=\"one\" a2=\"two\" /></root>")
					.SetName("RootHasChildThatIsEmptyWithAttributes");

				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root",
						Nodes = new[] {
							new XmlElementBuilder {
								LocalName = "child",
								Nodes = new[] { new XmlTextBuilder() }
							}
						}
					},
					"<root><child></child></root>")
					.SetName("RootHasChildThatIsNotEmpty");

				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root",
						Nodes = new[] {
							new XmlElementBuilder {
								LocalName = "child",
								Nodes = new[] { new XmlTextBuilder { Value = "content" } }
							}
						}
					},
					"<root><child>content</child></root>")
					.SetName("RootHasChildWithText");

				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root",
						NamespaceUri = "urn:ns-one",
						Attributes = new[] { new XmlAttributeBuilder { LocalName = "xmlns", Value = "urn:ns-one" } },
						Nodes = new[] {
							new XmlElementBuilder {
								LocalName = "child",
								NamespaceUri = "urn:ns-one",
								Nodes = new[] { new XmlTextBuilder { Value = "content" } }
							}
						}
					},
					"<root xmlns=\"urn:ns-one\"><child>content</child></root>")
					.SetName("RootHasChildWithTextAndDefaultXmlNamespace");

				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root",
						NamespaceUri = "urn:ns-one",
						Prefix = "s1",
						Attributes = new[] { new XmlAttributeBuilder { LocalName = "s1", Prefix = "xmlns", Value = "urn:ns-one" } },
						Nodes = new[] {
							new XmlElementBuilder {
								LocalName = "child",
								NamespaceUri = "urn:ns-one",
								Prefix = "s1",
								Nodes = new[] { new XmlTextBuilder { Value = "content" } }
							}
						}
					},
					"<s1:root xmlns:s1=\"urn:ns-one\"><s1:child>content</s1:child></s1:root>")
					.SetName("RootHasChildWithTextAndXmlNamespace");

				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root",
						Nodes = new[] {
							new XmlElementBuilder { LocalName = "child-one" },
							new XmlElementBuilder { LocalName = "child-two" }
						}
					},
					"<root><child-one /><child-two /></root>")
					.SetName("RootHasChildrenThatAreEmpty");

				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root",
						Nodes = new[] {
							new XmlElementBuilder {
								LocalName = "child-one",
								Nodes = new[] { new XmlTextBuilder() }
							},
							new XmlElementBuilder {
								LocalName = "child-two",
								Nodes = new[] { new XmlTextBuilder() }
							}
						}
					},
					"<root><child-one></child-one><child-two></child-two></root>")
					.SetName("RootHasChildrenThatAreNotEmpty");

				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root"
					},
					"<root />")
					.SetName("RootIsEmpty");

				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root",
						Attributes = new[] {
							new XmlAttributeBuilder { LocalName = "a1", Value = "one" },
							new XmlAttributeBuilder { LocalName = "a2", Value = "two" }
						}
					},
					"<root a1=\"one\" a2=\"two\" />")
					.SetName("RootIsEmptyWithAttributes");

				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root",
						NamespaceUri = "urn:ns-one",
						Attributes = new[] { new XmlAttributeBuilder { LocalName = "xmlns", Value = "urn:ns-one" } }
					},
					"<root xmlns=\"urn:ns-one\" />")
					.SetName("RootIsEmptyWithDefaultXmlNamespace");

				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root",
						NamespaceUri = "urn:ns-one",
						Prefix = "s1",
						Attributes = new[] { new XmlAttributeBuilder { LocalName = "s1", Prefix = "xmlns", Value = "urn:ns-one" } }
					},
					"<s1:root xmlns:s1=\"urn:ns-one\" />")
					.SetName("RootIsEmptyWithXmlNamespace");

				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root",
						Nodes = new[] { new XmlTextBuilder() }
					},
					"<root></root>")
					.SetName("RootIsNotEmpty");

				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root",
						Attributes = new[] {
							new XmlAttributeBuilder { LocalName = "a1", Value = "one" },
							new XmlAttributeBuilder { LocalName = "a2", Value = "two" }
						},
						Nodes = new[] { new XmlTextBuilder() }
					},
					"<root a1=\"one\" a2=\"two\"></root>")
					.SetName("RootIsNotEmptyWithAttributes");

				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root",
						NamespaceUri = "urn:ns-one",
						Prefix = "s1",
						Attributes = new[] { new XmlAttributeBuilder { LocalName = "s1", Prefix = "xmlns", Value = "urn:ns-one" } },
						Nodes = new[] { new XmlTextBuilder() }
					},
					"<s1:root xmlns:s1=\"urn:ns-one\"></s1:root>")
					.SetName("RootIsNotEmptyWithXmlNamespace");

				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root",
						Attributes = new[] {
							new XmlAttributeBuilder { LocalName = "a1", Value = "one" },
							new XmlAttributeBuilder { LocalName = "a2", Value = "two" }
						},
						Nodes = new[] { new XmlTextBuilder { Value = "content" } }
					},
					"<root a1=\"one\" a2=\"two\">content</root>")
					.SetName("RootWithAttributesAndText");

				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root",
						Nodes = new[] { new XmlTextBuilder { Value = "content" } }
					},
					"<root>content</root>")
					.SetName("RootWithText");

				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root",
						Prefix = "s1",
						NamespaceUri = "urn:ns-one",
						Attributes = new[] {
							new XmlAttributeBuilder { LocalName = "a1", Value = "one" },
							new XmlAttributeBuilder { LocalName = "a2", Prefix = "s2", NamespaceUri = "urn:ns-two", Value = "two" },
							new XmlAttributeBuilder { LocalName = "s1", Prefix = "xmlns", Value = "urn:ns-one" },
							new XmlAttributeBuilder { LocalName = "s2", Prefix = "xmlns", Value = "urn:ns-two" }
						},
						Nodes = new IXmlNodeBuilder[] {
							new XmlElementBuilder {
								LocalName = "child-one",
								Prefix = "s1",
								NamespaceUri = "urn:ns-one",
								Nodes = new IXmlNodeBuilder[] {
									new XmlElementBuilder {
										LocalName = "grand-child-one",
										Nodes = new IXmlNodeBuilder[] { new XmlTextBuilder { Value = "grand-content" } }
									},
									new XmlTextBuilder { Value = "content" },
									new XmlElementBuilder {
										LocalName = "grand-child-two",
										Attributes = new[] {
											new XmlAttributeBuilder { LocalName = "a1", Value = "one" },
											new XmlAttributeBuilder { LocalName = "a2", Prefix = "s2", NamespaceUri = "urn:ns-two", Value = "two" }
										},
										Nodes = new IXmlNodeBuilder[] { new XmlTextBuilder { Value = "grand-content" } }
									}
								}
							},
							new XmlTextBuilder { Value = "intermezzo" },
							new XmlElementBuilder {
								LocalName = "child-two",
								Prefix = "s2",
								NamespaceUri = "urn:ns-two",
								Nodes = new[] { new XmlTextBuilder { Value = "content" } }
							}
						}
					},
					"<s1:root a1=\"one\" s2:a2=\"two\" xmlns:s1=\"urn:ns-one\" xmlns:s2=\"urn:ns-two\">" +
						"<s1:child-one>" +
						"<grand-child-one>grand-content</grand-child-one>" +
						"content" +
						"<grand-child-two a1=\"one\" s2:a2=\"two\">grand-content</grand-child-two>" +
						"</s1:child-one>" +
						"intermezzo" +
						"<s2:child-two>content</s2:child-two>" +
						"</s1:root>")
					.SetName("XmlContentIsComplex");

				yield return new TestCaseData(
					new XmlElementBuilder {
						LocalName = "root",
						NamespaceUri = "urn:ns-one",
						Attributes = new[] {
							new XmlAttributeBuilder { LocalName = "a1", Value = "one" },
							new XmlAttributeBuilder { LocalName = "a2", Prefix = "s2", NamespaceUri = "urn:ns-two", Value = "two" },
							new XmlAttributeBuilder { LocalName = "xmlns", Value = "urn:ns-one" },
							new XmlAttributeBuilder { LocalName = "s2", Prefix = "xmlns", Value = "urn:ns-two" }
						},
						Nodes = new IXmlNodeBuilder[] {
							new XmlElementBuilder {
								LocalName = "child-one",
								NamespaceUri = "urn:ns-one",
								Nodes = new IXmlNodeBuilder[] {
									new XmlElementBuilder {
										LocalName = "grand-child-one",
										NamespaceUri = "urn:ns-one",
										Nodes = new IXmlNodeBuilder[] { new XmlTextBuilder { Value = "grand-content" } }
									},
									new XmlTextBuilder { Value = "content" },
									new XmlElementBuilder {
										LocalName = "grand-child-two",
										NamespaceUri = "urn:ns-one",
										Attributes = new[] {
											new XmlAttributeBuilder { LocalName = "a1", Value = "one" },
											new XmlAttributeBuilder { LocalName = "a2", Prefix = "s2", NamespaceUri = "urn:ns-two", Value = "two" }
										},
										Nodes = new IXmlNodeBuilder[] { new XmlTextBuilder { Value = "grand-content" } }
									}
								}
							},
							new XmlTextBuilder { Value = "intermezzo" },
							new XmlElementBuilder {
								LocalName = "child-two",
								Prefix = "s2",
								NamespaceUri = "urn:ns-two",
								Nodes = new[] { new XmlTextBuilder { Value = "content" } }
							}
						}
					},
					"<root a1=\"one\" s2:a2=\"two\" xmlns=\"urn:ns-one\" xmlns:s2=\"urn:ns-two\">" +
						"<child-one>" +
						"<grand-child-one>grand-content</grand-child-one>" +
						"content" +
						"<grand-child-two a1=\"one\" s2:a2=\"two\">grand-content</grand-child-two>" +
						"</child-one>" +
						"intermezzo" +
						"<s2:child-two>content</s2:child-two>" +
						"</root>")
					.SetName("XmlContentIsComplexToo");

				yield return new TestCaseData(
					null,
					string.Empty)
					.SetName("XmlIsEmpty");
			}
		}
	}
}
