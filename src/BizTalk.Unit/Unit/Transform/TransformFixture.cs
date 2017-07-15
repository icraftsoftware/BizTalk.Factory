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

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using Be.Stateless.BizTalk.Streaming.Extensions;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.Linq.Extensions;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	/// <summary>
	/// This base class provides utility methods to test a BizTalk transform.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the BizTalk transform to test.
	/// </typeparam>
	public abstract class TransformFixture<T> where T : TransformBase, new()
	{
		#region Nested Type: TransformResult

		/// <summary>
		/// Allows to build up assertions on the result of a BizTalk transform returned by one of <see
		/// cref="TransformFixture{T}"/>'s <c>Transform</c> overloads.
		/// </summary>
		protected class TransformResult
		{
			internal TransformResult(XmlReader result, IXmlNamespaceResolver namespaceResolver)
			{
				_navigator = new XPathDocument(result).CreateNavigator();
				_namespaceResolver = namespaceResolver;
			}

			/// <summary>
			/// The whole result of the transform as an XML string.
			/// </summary>
			public string XmlContent
			{
				get
				{
					var builder = new StringBuilder();
					var navigator = _navigator;
					using (var writer = new StringWriter(builder))
					using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
					{
						navigator.WriteSubtree(xmlWriter);
						xmlWriter.Close();
					}
					return builder.ToString();
				}
			}

			/// <summary>
			/// Evaluates the specified XPath expression against the result of the transform and returns the typed result.
			/// </summary>
			/// <param name="xpathExpression">
			/// A <see cref="string"/> representing an XPath expression that can be evaluated.
			/// </param>
			/// <returns>
			/// The result of the expression (Boolean, number, string, or node set). This maps to <see cref="bool"/>, <see
			/// cref="double"/>, <see cref="string"/>, or <see cref="XPathNodeIterator"/> objects respectively.
			/// </returns>
			/// <remarks>
			/// <para>
			/// <see cref="Evaluate"/> automatically benefits of an <see cref="IXmlNamespaceResolver"/> resolver that
			/// redeclares all of the namespaces that the generic XSLT transform of <see cref="TransformFixture{T}"/> have
			/// declared.
			/// </para>
			/// <para>
			/// It is possible to extend this <see cref="IXmlNamespaceResolver"/> resolver via the <see
			/// cref="TransformFixture{T}.AddNamespace"/> method. Notice that the namespaces added this way will scope the
			/// whole <see cref="TransformFixture{T}"/> and not a particular test method.
			/// </para>
			/// </remarks>
			public object Evaluate(string xpathExpression)
			{
				return _navigator.Evaluate(xpathExpression, _namespaceResolver);
			}

			/// <summary>
			/// Selects a node set using the specified XPath expression.
			/// </summary>
			/// <param name="xpathExpression">
			/// A <see cref="string"/> representing an XPath expression.
			/// </param>
			/// <returns>
			/// An <see cref="XPathNodeIterator"/> that points to the selected node set.
			/// </returns>
			/// <remarks>
			/// <para>
			/// <see cref="Select"/> automatically benefits of an <see cref="IXmlNamespaceResolver"/> resolver that
			/// redeclares all of the namespaces that the generic XSLT transform of <see cref="TransformFixture{T}"/> have
			/// declared.
			/// </para>
			/// <para>
			/// It is possible to extend this <see cref="IXmlNamespaceResolver"/> resolver via the <see
			/// cref="TransformFixture{T}.AddNamespace"/> method. Notice that the namespaces added this way will scope the
			/// whole <see cref="TransformFixture{T}"/> and not a particular test method.
			/// </para>
			/// </remarks>
			public XPathNodeIterator Select(string xpathExpression)
			{
				return _navigator.Select(xpathExpression, _namespaceResolver);
			}

			/// <summary>
			/// Selects a single node using the specified XPath expression.
			/// </summary>
			/// <param name="xpathExpression">
			/// A <see cref="string"/> representing an XPath expression.
			/// </param>
			/// <returns>
			/// An <see cref="XPathNodeIterator"/> that contains the single node matching the XPath expression specified.
			/// </returns>
			/// <exception cref="InvalidOperationException">
			/// Either there is no node or more than one node matching the XPath expression.
			/// </exception>
			/// <remarks>
			/// <para>
			/// <see cref="Single"/> automatically benefits of an <see cref="IXmlNamespaceResolver"/> resolver that
			/// redeclares all of the namespaces that the generic XSLT transform of <see cref="TransformFixture{T}"/> have
			/// declared.
			/// </para>
			/// <para>
			/// It is possible to extend this <see cref="IXmlNamespaceResolver"/> resolver via the <see
			/// cref="TransformFixture{T}.AddNamespace"/> method. Notice that the namespaces added this way will scope the
			/// whole <see cref="TransformFixture{T}"/> and not a particular test method.
			/// </para>
			/// </remarks>
			public XPathNavigator Single(string xpathExpression)
			{
				return Select(xpathExpression)
					.OfType<XPathNavigator>()
					.Single();
			}

			/// <summary>
			/// Concatenates all the text values of the nodes selected using the specified XPath expression and uses the
			/// default separator ('#') between each value.
			/// </summary>
			/// <param name="xpathExpression">
			/// A <see cref="string"/> representing an XPath expression.
			/// </param>
			/// <returns>
			/// A string that consists of the selected nodes' values delimited by the separator <see cref="char"/>.
			/// </returns>
			/// <remarks>
			/// <para>
			/// <see cref="StringJoin(string)"/> automatically benefits of an <see cref="IXmlNamespaceResolver"/> resolver
			/// that redeclares all of the namespaces that the generic XSLT transform of <see cref="TransformFixture{T}"/>
			/// have declared.
			/// </para>
			/// <para>
			/// It is possible to extend this <see cref="IXmlNamespaceResolver"/> resolver via the <see
			/// cref="TransformFixture{T}.AddNamespace"/> method. Notice that the namespaces added this way will scope the
			/// whole <see cref="TransformFixture{T}"/> and not a particular test method.
			/// </para>
			/// </remarks>
			/// <seealso href="http://www.w3.org/TR/2002/WD-xquery-operators-20021115/#func-string-join"/>
			public string StringJoin(string xpathExpression)
			{
				return StringJoin(xpathExpression, '#');
			}

			/// <summary>
			/// Concatenates all the text values of the nodes selected using the specified XPath expression and uses the
			/// specified separator between each value.
			/// </summary>
			/// <param name="xpathExpression">
			/// A <see cref="string"/> representing an XPath expression.
			/// </param>
			/// <param name="separator">
			/// The <see cref="char"/> to use as a separator.
			/// </param>
			/// <returns>
			/// A string that consists of the selected nodes' values delimited by the separator <see cref="char"/>.
			/// </returns>
			/// <remarks>
			/// <para>
			/// <see cref="StringJoin(string,char)"/> automatically benefits of an <see cref="IXmlNamespaceResolver"/>
			/// resolver that redeclares all of the namespaces that the generic XSLT transform of <see
			/// cref="TransformFixture{T}"/> have declared.
			/// </para>
			/// <para>
			/// It is possible to extend this <see cref="IXmlNamespaceResolver"/> resolver via the <see
			/// cref="TransformFixture{T}.AddNamespace"/> method. Notice that the namespaces added this way will scope the
			/// whole <see cref="TransformFixture{T}"/> and not a particular test method.
			/// </para>
			/// </remarks>
			/// <seealso href="http://www.w3.org/TR/2002/WD-xquery-operators-20021115/#func-string-join"/>
			public string StringJoin(string xpathExpression, char separator)
			{
				return Select(xpathExpression)
					.OfType<XPathNavigator>()
					.Aggregate(string.Empty, (str, node) => str + node.Value + separator)
					.TrimEnd(separator);
			}

			private readonly IXmlNamespaceResolver _namespaceResolver;
			private readonly XPathNavigator _navigator;
		}

		#endregion

		static TransformFixture()
		{
			if (Debugger.IsAttached)
			{
				// inject map/transform extensions around XML streams that support XSLT debugging
				StreamExtensions.StreamTransformerFactory = streams => new DebuggerSupportingTransformer(streams);
			}
		}

		protected TransformFixture()
		{
			// redeclares each of the namespaces declared in the XSLT with the same prefixes
			using (var sr = new StringReader(new T().XmlContent))
			{
				var navigator = new XPathDocument(sr).CreateNavigator();
				navigator.MoveToFollowing(XPathNodeType.Element);
				Debug.Assert(navigator.NameTable != null, "_namespaceResolver.NameTable != null");
				_namespaceManager = new XmlNamespaceManager(navigator.NameTable);
				navigator.GetNamespacesInScope(XmlNamespaceScope.All).Each(n => _namespaceManager.AddNamespace(n.Key, n.Value));
			}
		}

		/// <summary>
		/// Adds the given namespace to the collection of namespaces that have been automatically redeclared after the
		/// ones declared in the <typeparamref name="T"/> XSLT transform.
		/// </summary>
		/// <param name="prefix">
		/// The prefix to associate with the namespace being added.
		/// </param>
		/// <param name="uri">
		/// The namespace to add.
		/// </param>
		protected void AddNamespace(string prefix, string uri)
		{
			_namespaceManager.AddNamespace(prefix, uri);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the document at <paramref name="inputPath"/> and
		/// validates the resulting document against the <typeparamref name="TSchema"/>-<see cref="SchemaBase"/>-derived
		/// XML schema.
		/// </summary>
		/// <param name="inputPath">
		/// The full path to the file being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema>(string inputPath)
			where TSchema : SchemaBase, new()
		{
			return Transform<TSchema>(XmlSchemaContentProcessing.Strict, inputPath);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStream"/> stream and
		/// validates the resulting document against the <typeparamref name="TSchema"/>-<see cref="SchemaBase"/>-derived
		/// XML schema.
		/// </summary>
		/// <param name="inputStream">
		/// The XML stream to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema>(Stream inputStream)
			where TSchema : SchemaBase, new()
		{
			return Transform<TSchema>(XmlSchemaContentProcessing.Strict, inputStream);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the documents at <paramref name="inputPaths"/> and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema"/>.
		/// </summary>
		/// <param name="inputPaths">
		/// The full paths to the files being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema>(params string[] inputPaths)
			where TSchema : SchemaBase, new()
		{
			return Transform<TSchema>(XmlSchemaContentProcessing.Strict, inputPaths);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStreams"/> streams and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema"/>.
		/// </summary>
		/// <param name="inputStreams">
		/// The XML streams to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema>(params Stream[] inputStreams)
			where TSchema : SchemaBase, new()
		{
			return Transform<TSchema>(XmlSchemaContentProcessing.Strict, inputStreams);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the document at <paramref name="inputPath"/> and
		/// validates the resulting document against the <typeparamref name="TSchema"/>-<see cref="SchemaBase"/>-derived
		/// XML schema.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="inputPath">
		/// The full path to the file being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema>(IBaseMessageContext context, string inputPath)
			where TSchema : SchemaBase, new()
		{
			return Transform<TSchema>(context, XmlSchemaContentProcessing.Strict, inputPath);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStream"/> stream and
		/// validates the resulting document against the <typeparamref name="TSchema"/>-<see cref="SchemaBase"/>-derived
		/// XML schema.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="inputStream">
		/// The XML stream to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema>(IBaseMessageContext context, Stream inputStream)
			where TSchema : SchemaBase, new()
		{
			return Transform<TSchema>(context, XmlSchemaContentProcessing.Strict, inputStream);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the documents at <paramref name="inputPaths"/> and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="inputPaths">
		/// The full paths to the files being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema>(IBaseMessageContext context, params string[] inputPaths)
			where TSchema : SchemaBase, new()
		{
			return Transform<TSchema>(context, XmlSchemaContentProcessing.Strict, inputPaths);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStreams"/> streams and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="inputStreams">
		/// The XML streams to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema>(IBaseMessageContext context, params Stream[] inputStreams)
			where TSchema : SchemaBase, new()
		{
			return Transform<TSchema>(context, XmlSchemaContentProcessing.Strict, inputStreams);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the document at <paramref name="inputPath"/> and
		/// validates the resulting document against the <typeparamref name="TSchema"/>-<see cref="SchemaBase"/>-derived
		/// XML schema.
		/// </summary>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputPath">
		/// The full path to the file being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema>(XmlSchemaContentProcessing contentProcessing, string inputPath)
			where TSchema : SchemaBase, new()
		{
			using (var inputStream = File.OpenRead(inputPath))
			{
				return Transform<TSchema>(contentProcessing, inputStream);
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStream"/> stream and
		/// validates the resulting document against the <typeparamref name="TSchema"/>-<see cref="SchemaBase"/>-derived
		/// XML schema.
		/// </summary>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputStream">
		/// The XML stream to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema>(XmlSchemaContentProcessing contentProcessing, Stream inputStream)
			where TSchema : SchemaBase, new()
		{
			using (var resultStream = inputStream.Transform().Apply(typeof(T)))
			using (var reader = ValidatingXmlReader.Create<TSchema>(resultStream, contentProcessing))
			{
				return new TransformResult(reader, _namespaceManager);
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the documents at <paramref name="inputPaths"/> and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema"/>.
		/// </summary>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputPaths">
		/// The full paths to the files being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema>(XmlSchemaContentProcessing contentProcessing, params string[] inputPaths)
			where TSchema : SchemaBase, new()
		{
			var inputStreams = Enumerable.Empty<Stream>().ToArray();
			try
			{
				inputStreams = inputPaths.Select(File.OpenRead).Cast<Stream>().ToArray();
				return Transform<TSchema>(contentProcessing, inputStreams);
			}
			finally
			{
				inputStreams.Where(s => s != null).Each(s => s.Dispose());
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStreams"/> streams and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema"/>.
		/// </summary>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputStreams">
		/// The XML streams to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema>(XmlSchemaContentProcessing contentProcessing, params Stream[] inputStreams)
			where TSchema : SchemaBase, new()
		{
			using (var resultStream = inputStreams.Transform().Apply(typeof(T)))
			using (var reader = ValidatingXmlReader.Create<TSchema>(resultStream, contentProcessing))
			{
				return new TransformResult(reader, _namespaceManager);
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the document at <paramref name="inputPath"/> and
		/// validates the resulting document against the <typeparamref name="TSchema"/>-<see cref="SchemaBase"/>-derived
		/// XML schema.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputPath">
		/// The full path to the file being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema>(IBaseMessageContext context, XmlSchemaContentProcessing contentProcessing, string inputPath)
			where TSchema : SchemaBase, new()
		{
			using (var inputStream = File.OpenRead(inputPath))
			{
				return Transform<TSchema>(context, contentProcessing, inputStream);
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStream"/> stream and
		/// validates the resulting document against the <typeparamref name="TSchema"/>-<see cref="SchemaBase"/>-derived
		/// XML schema.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputStream">
		/// The XML stream to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema>(IBaseMessageContext context, XmlSchemaContentProcessing contentProcessing, Stream inputStream)
			where TSchema : SchemaBase, new()
		{
			using (var resultStream = inputStream.Transform().ExtendWith(context).Apply(typeof(T)))
			using (var reader = ValidatingXmlReader.Create<TSchema>(resultStream, contentProcessing))
			{
				return new TransformResult(reader, _namespaceManager);
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the documents at <paramref name="inputPaths"/> and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputPaths">
		/// The full paths to the files being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema>(IBaseMessageContext context, XmlSchemaContentProcessing contentProcessing, params string[] inputPaths)
			where TSchema : SchemaBase, new()
		{
			var inputStreams = Enumerable.Empty<Stream>().ToArray();
			try
			{
				inputStreams = inputPaths.Select(File.OpenRead).Cast<Stream>().ToArray();
				return Transform<TSchema>(context, contentProcessing, inputStreams);
			}
			finally
			{
				inputStreams.Where(s => s != null).Each(s => s.Dispose());
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStreams"/> streams and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputStreams">
		/// The XML streams to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema>(IBaseMessageContext context, XmlSchemaContentProcessing contentProcessing, params Stream[] inputStreams)
			where TSchema : SchemaBase, new()
		{
			using (var resultStream = inputStreams.Transform().ExtendWith(context).Apply(typeof(T)))
			using (var reader = ValidatingXmlReader.Create<TSchema>(resultStream, contentProcessing))
			{
				return new TransformResult(reader, _namespaceManager);
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the document at <paramref name="inputPath"/> and
		/// validates the resulting document against the schemas identified by the type parameter <typeparamref
		/// name="TSchema1"/> and <typeparamref name="TSchema2"/>.
		/// </summary>
		/// <param name="inputPath">
		/// The full path to the file being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2>(string inputPath)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
		{
			return Transform<TSchema1, TSchema2>(XmlSchemaContentProcessing.Strict, inputPath);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStream"/> stream and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/> and <typeparamref name="TSchema2"/>.
		/// </summary>
		/// <param name="inputStream">
		/// The XML stream to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2>(Stream inputStream)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
		{
			return Transform<TSchema1, TSchema2>(XmlSchemaContentProcessing.Strict, inputStream);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the documents at <paramref name="inputPaths"/> and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/> and <typeparamref name="TSchema2"/>.
		/// </summary>
		/// <param name="inputPaths">
		/// The full paths to the files being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2>(params string[] inputPaths)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
		{
			return Transform<TSchema1, TSchema2>(XmlSchemaContentProcessing.Strict, inputPaths);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStreams"/> streams and
		/// validates the resulting document against the schemas identified by the type parameter <typeparamref
		/// name="TSchema1"/> and <typeparamref name="TSchema2"/>.
		/// </summary>
		/// <param name="inputStreams">
		/// The XML streams to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2>(params Stream[] inputStreams)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
		{
			return Transform<TSchema1, TSchema2>(XmlSchemaContentProcessing.Strict, inputStreams);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the document at <paramref name="inputPath"/> and
		/// validates the resulting document against the schemas identified by the type parameter <typeparamref
		/// name="TSchema1"/> and <typeparamref name="TSchema2"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="inputPath">
		/// The full path to the file being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2>(IBaseMessageContext context, string inputPath)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
		{
			return Transform<TSchema1, TSchema2>(context, XmlSchemaContentProcessing.Strict, inputPath);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStream"/> stream and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/> and <typeparamref name="TSchema2"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="inputStream">
		/// The XML stream to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2>(IBaseMessageContext context, Stream inputStream)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
		{
			return Transform<TSchema1, TSchema2>(context, XmlSchemaContentProcessing.Strict, inputStream);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the documents at <paramref name="inputPaths"/> and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/> and <typeparamref name="TSchema2"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="inputPaths">
		/// The full paths to the files being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2>(IBaseMessageContext context, params string[] inputPaths)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
		{
			return Transform<TSchema1, TSchema2>(context, XmlSchemaContentProcessing.Strict, inputPaths);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStreams"/> streams and
		/// validates the resulting document against the schemas identified by the type parameter <typeparamref
		/// name="TSchema1"/> and <typeparamref name="TSchema2"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="inputStreams">
		/// The XML streams to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2>(IBaseMessageContext context, params Stream[] inputStreams)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
		{
			return Transform<TSchema1, TSchema2>(context, XmlSchemaContentProcessing.Strict, inputStreams);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the document at <paramref name="inputPath"/> and
		/// validates the resulting document against the schemas identified by the type parameter <typeparamref
		/// name="TSchema1"/> and <typeparamref name="TSchema2"/>.
		/// </summary>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputPath">
		/// The full path to the file being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2>(XmlSchemaContentProcessing contentProcessing, string inputPath)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
		{
			using (var inputStream = File.OpenRead(inputPath))
			{
				return Transform<TSchema1, TSchema2>(contentProcessing, inputStream);
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStream"/> stream and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/> and <typeparamref name="TSchema2"/>.
		/// </summary>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputStream">
		/// The XML stream to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2>(XmlSchemaContentProcessing contentProcessing, Stream inputStream)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
		{
			using (var resultStream = inputStream.Transform().Apply(typeof(T)))
			using (var reader = ValidatingXmlReader.Create<TSchema1, TSchema2>(resultStream, contentProcessing))
			{
				return new TransformResult(reader, _namespaceManager);
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the documents at <paramref name="inputPaths"/> and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/> and <typeparamref name="TSchema2"/>.
		/// </summary>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputPaths">
		/// The full paths to the files being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2>(XmlSchemaContentProcessing contentProcessing, params string[] inputPaths)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
		{
			var inputStreams = Enumerable.Empty<Stream>().ToArray();
			try
			{
				inputStreams = inputPaths.Select(File.OpenRead).Cast<Stream>().ToArray();
				return Transform<TSchema1, TSchema2>(contentProcessing, inputStreams);
			}
			finally
			{
				inputStreams.Where(s => s != null).Each(s => s.Dispose());
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStreams"/> streams and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/> and <typeparamref name="TSchema2"/>.
		/// </summary>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputStreams">
		/// The XML streams to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2>(XmlSchemaContentProcessing contentProcessing, params Stream[] inputStreams)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
		{
			using (var resultStream = inputStreams.Transform().Apply(typeof(T)))
			using (var reader = ValidatingXmlReader.Create<TSchema1, TSchema2>(resultStream, contentProcessing))
			{
				return new TransformResult(reader, _namespaceManager);
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the document at <paramref name="inputPath"/> and
		/// validates the resulting document against the schemas identified by the type parameter <typeparamref
		/// name="TSchema1"/> and <typeparamref name="TSchema2"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputPath">
		/// The full path to the file being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2>(IBaseMessageContext context, XmlSchemaContentProcessing contentProcessing, string inputPath)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
		{
			using (var inputStream = File.OpenRead(inputPath))
			{
				return Transform<TSchema1, TSchema2>(context, contentProcessing, inputStream);
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStream"/> stream and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/> and <typeparamref name="TSchema2"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputStream">
		/// The XML stream to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2>(IBaseMessageContext context, XmlSchemaContentProcessing contentProcessing, Stream inputStream)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
		{
			using (var resultStream = inputStream.Transform().ExtendWith(context).Apply(typeof(T)))
			using (var reader = ValidatingXmlReader.Create<TSchema1, TSchema2>(resultStream, contentProcessing))
			{
				return new TransformResult(reader, _namespaceManager);
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the documents at <paramref name="inputPaths"/> and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/> and <typeparamref name="TSchema2"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputPaths">
		/// The full paths to the files being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2>(IBaseMessageContext context, XmlSchemaContentProcessing contentProcessing, params string[] inputPaths)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
		{
			var inputStreams = Enumerable.Empty<Stream>().ToArray();
			try
			{
				inputStreams = inputPaths.Select(File.OpenRead).Cast<Stream>().ToArray();
				return Transform<TSchema1, TSchema2>(context, contentProcessing, inputStreams);
			}
			finally
			{
				inputStreams.Where(s => s != null).Each(s => s.Dispose());
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStreams"/> streams and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/> and <typeparamref name="TSchema2"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputStreams">
		/// The XML streams to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2>(IBaseMessageContext context, XmlSchemaContentProcessing contentProcessing, params Stream[] inputStreams)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
		{
			using (var resultStream = inputStreams.Transform().ExtendWith(context).Apply(typeof(T)))
			using (var reader = ValidatingXmlReader.Create<TSchema1, TSchema2>(resultStream, contentProcessing))
			{
				return new TransformResult(reader, _namespaceManager);
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the document at <paramref name="inputPath"/> and
		/// validates the resulting document against the schemas identified by the type parameter <typeparamref
		/// name="TSchema1"/>, <typeparamref name="TSchema2"/> and <typeparamref name="TSchema3"/>.
		/// </summary>
		/// <param name="inputPath">
		/// The full path to the file being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2, TSchema3>(string inputPath)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
			where TSchema3 : SchemaBase, new()
		{
			return Transform<TSchema1, TSchema2, TSchema3>(XmlSchemaContentProcessing.Strict, inputPath);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStream"/> stream and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/>, <typeparamref name="TSchema2"/> and <typeparamref name="TSchema3"/>.
		/// </summary>
		/// <param name="inputStream">
		/// The XML stream to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2, TSchema3>(Stream inputStream)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
			where TSchema3 : SchemaBase, new()
		{
			return Transform<TSchema1, TSchema2, TSchema3>(XmlSchemaContentProcessing.Strict, inputStream);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the documents at <paramref name="inputPaths"/> and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/>, <typeparamref name="TSchema2"/> and <typeparamref name="TSchema3"/>.
		/// </summary>
		/// <param name="inputPaths">
		/// The full paths to the files being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2, TSchema3>(params string[] inputPaths)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
			where TSchema3 : SchemaBase, new()
		{
			return Transform<TSchema1, TSchema2, TSchema3>(XmlSchemaContentProcessing.Strict, inputPaths);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStreams"/> streams and
		/// validates the resulting document against the schemas identified by the type parameter <typeparamref
		/// name="TSchema1"/>, <typeparamref name="TSchema2"/> and <typeparamref name="TSchema3"/>.
		/// </summary>
		/// <param name="inputStreams">
		/// The XML streams to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2, TSchema3>(params Stream[] inputStreams)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
			where TSchema3 : SchemaBase, new()
		{
			return Transform<TSchema1, TSchema2, TSchema3>(XmlSchemaContentProcessing.Strict, inputStreams);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the document at <paramref name="inputPath"/> and
		/// validates the resulting document against the schemas identified by the type parameter <typeparamref
		/// name="TSchema1"/>, <typeparamref name="TSchema2"/> and <typeparamref name="TSchema3"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="inputPath">
		/// The full path to the file being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2, TSchema3>(IBaseMessageContext context, string inputPath)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
			where TSchema3 : SchemaBase, new()
		{
			return Transform<TSchema1, TSchema2, TSchema3>(context, XmlSchemaContentProcessing.Strict, inputPath);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStream"/> stream and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/>, <typeparamref name="TSchema2"/> and <typeparamref name="TSchema3"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="inputStream">
		/// The XML stream to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2, TSchema3>(IBaseMessageContext context, Stream inputStream)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
			where TSchema3 : SchemaBase, new()
		{
			return Transform<TSchema1, TSchema2, TSchema3>(context, XmlSchemaContentProcessing.Strict, inputStream);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the documents at <paramref name="inputPaths"/> and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/>, <typeparamref name="TSchema2"/> and <typeparamref name="TSchema3"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="inputPaths">
		/// The full paths to the files being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2, TSchema3>(IBaseMessageContext context, params string[] inputPaths)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
			where TSchema3 : SchemaBase, new()
		{
			return Transform<TSchema1, TSchema2, TSchema3>(context, XmlSchemaContentProcessing.Strict, inputPaths);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStreams"/> streams and
		/// validates the resulting document against the schemas identified by the type parameter <typeparamref
		/// name="TSchema1"/>, <typeparamref name="TSchema2"/> and <typeparamref name="TSchema3"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="inputStreams">
		/// The XML streams to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2, TSchema3>(IBaseMessageContext context, params Stream[] inputStreams)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
			where TSchema3 : SchemaBase, new()
		{
			return Transform<TSchema1, TSchema2, TSchema3>(context, XmlSchemaContentProcessing.Strict, inputStreams);
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the document at <paramref name="inputPath"/> and
		/// validates the resulting document against the schemas identified by the type parameter <typeparamref
		/// name="TSchema1"/>, <typeparamref name="TSchema2"/> and <typeparamref name="TSchema3"/>.
		/// </summary>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputPath">
		/// The full path to the file being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2, TSchema3>(XmlSchemaContentProcessing contentProcessing, string inputPath)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
			where TSchema3 : SchemaBase, new()
		{
			using (var inputStream = File.OpenRead(inputPath))
			{
				return Transform<TSchema1, TSchema2, TSchema3>(contentProcessing, inputStream);
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStream"/> stream and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/>, <typeparamref name="TSchema2"/> and <typeparamref name="TSchema3"/>.
		/// </summary>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputStream">
		/// The XML stream to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2, TSchema3>(XmlSchemaContentProcessing contentProcessing, Stream inputStream)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
			where TSchema3 : SchemaBase, new()
		{
			using (var resultStream = inputStream.Transform().Apply(typeof(T)))
			using (var reader = ValidatingXmlReader.Create<TSchema1, TSchema2, TSchema3>(resultStream, contentProcessing))
			{
				return new TransformResult(reader, _namespaceManager);
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the documents at <paramref name="inputPaths"/> and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/>, <typeparamref name="TSchema2"/> and <typeparamref name="TSchema3"/>.
		/// </summary>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputPaths">
		/// The full paths to the files being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2, TSchema3>(XmlSchemaContentProcessing contentProcessing, params string[] inputPaths)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
			where TSchema3 : SchemaBase, new()
		{
			var inputStreams = Enumerable.Empty<Stream>().ToArray();
			try
			{
				inputStreams = inputPaths.Select(File.OpenRead).Cast<Stream>().ToArray();
				return Transform<TSchema1, TSchema2, TSchema3>(contentProcessing, inputStreams);
			}
			finally
			{
				inputStreams.Where(s => s != null).Each(s => s.Dispose());
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStreams"/> streams and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/>, <typeparamref name="TSchema2"/> and <typeparamref name="TSchema3"/>.
		/// </summary>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputStreams">
		/// The XML streams to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2, TSchema3>(XmlSchemaContentProcessing contentProcessing, params Stream[] inputStreams)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
			where TSchema3 : SchemaBase, new()
		{
			using (var resultStream = inputStreams.Transform().Apply(typeof(T)))
			using (var reader = ValidatingXmlReader.Create<TSchema1, TSchema2, TSchema3>(resultStream, contentProcessing))
			{
				return new TransformResult(reader, _namespaceManager);
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the document at <paramref name="inputPath"/> and
		/// validates the resulting document against the schemas identified by the type parameter <typeparamref
		/// name="TSchema1"/>, <typeparamref name="TSchema2"/> and <typeparamref name="TSchema3"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputPath">
		/// The full path to the file being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2, TSchema3>(IBaseMessageContext context, XmlSchemaContentProcessing contentProcessing, string inputPath)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
			where TSchema3 : SchemaBase, new()
		{
			using (var inputStream = File.OpenRead(inputPath))
			{
				return Transform<TSchema1, TSchema2, TSchema3>(context, contentProcessing, inputStream);
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStream"/> stream and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/>, <typeparamref name="TSchema2"/> and <typeparamref name="TSchema3"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputStream">
		/// The XML stream to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2, TSchema3>(IBaseMessageContext context, XmlSchemaContentProcessing contentProcessing, Stream inputStream)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
			where TSchema3 : SchemaBase, new()
		{
			using (var resultStream = inputStream.Transform().ExtendWith(context).Apply(typeof(T)))
			using (var reader = ValidatingXmlReader.Create<TSchema1, TSchema2, TSchema3>(resultStream, contentProcessing))
			{
				return new TransformResult(reader, _namespaceManager);
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the documents at <paramref name="inputPaths"/> and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/>, <typeparamref name="TSchema2"/> and <typeparamref name="TSchema3"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputPaths">
		/// The full paths to the files being transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2, TSchema3>(IBaseMessageContext context, XmlSchemaContentProcessing contentProcessing, params string[] inputPaths)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
			where TSchema3 : SchemaBase, new()
		{
			var inputStreams = Enumerable.Empty<Stream>().ToArray();
			try
			{
				inputStreams = inputPaths.Select(File.OpenRead).Cast<Stream>().ToArray();
				return Transform<TSchema1, TSchema2, TSchema3>(context, contentProcessing, inputStreams);
			}
			finally
			{
				inputStreams.Where(s => s != null).Each(s => s.Dispose());
			}
		}

		/// <summary>
		/// Executes the given BizTalk map <typeparamref name="T"/> on the <paramref name="inputStreams"/> streams and
		/// validates the resulting document against the schema identified by the type parameter <typeparamref
		/// name="TSchema1"/>, <typeparamref name="TSchema2"/> and <typeparamref name="TSchema3"/>.
		/// </summary>
		/// <param name="context">
		/// The <see cref="IBaseMessageContext"/> that the <typeparamref name="T"/> transform requires to be applied.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <param name="inputStreams">
		/// The XML streams to be transformed.
		/// </param>
		/// <returns>
		/// A <see cref="TransformResult"/> containing the transform results. The contents of this document can be further
		/// validated by the caller using for instance either the <see cref="TransformResult.Select"/> or the <see
		/// cref="TransformResult.StringJoin(string,char)"/> methods if necessary.
		/// </returns>
		/// <remarks>
		/// The executed transform is the <see cref="XslCompiledTransform"/> equivalent of the <see
		/// cref="TransformBase"/>-derived BizTalk map.
		/// </remarks>
		protected TransformResult Transform<TSchema1, TSchema2, TSchema3>(
			IBaseMessageContext context,
			XmlSchemaContentProcessing contentProcessing,
			params Stream[] inputStreams)
			where TSchema1 : SchemaBase, new()
			where TSchema2 : SchemaBase, new()
			where TSchema3 : SchemaBase, new()
		{
			using (var resultStream = inputStreams.Transform().ExtendWith(context).Apply(typeof(T)))
			using (var reader = ValidatingXmlReader.Create<TSchema1, TSchema2, TSchema3>(resultStream, contentProcessing))
			{
				return new TransformResult(reader, _namespaceManager);
			}
		}

		private readonly XmlNamespaceManager _namespaceManager;
	}
}
