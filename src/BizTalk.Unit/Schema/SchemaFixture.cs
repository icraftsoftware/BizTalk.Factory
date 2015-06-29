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

using System.IO;
using System.Xml;
using System.Xml.Schema;
using Be.Stateless.BizTalk.Xml;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Unit.Schema
{
	/// <summary>
	/// This base class provides utility methods to validate XML instance documents against a BizTalk Server <see
	/// cref="SchemaBase"/>-derived schema.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the BizTalk Server schema to vaidate against.
	/// </typeparam>
	public abstract class SchemaFixture<T> where T : SchemaBase, new()
	{
		/// <summary>
		/// Loads and validates an XML file against the <typeparamref name="T"/> BizTalk Server schema.
		/// </summary>
		/// <param name="filepath">
		/// The path of the XML file to load and validate.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the XML file.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument(string filepath)
		{
			using (var reader = XmlReader.Create(filepath))
			{
				return ValidateInstanceDocument(reader);
			}
		}

		/// <summary>
		/// Loads and validates an XML file against the <typeparamref name="T"/> BizTalk Server schema.
		/// </summary>
		/// <param name="filepath">
		/// The path of the XML file to load and validate.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the XML file.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument(string filepath, XmlSchemaContentProcessing contentProcessing)
		{
			using (var reader = XmlReader.Create(filepath))
			{
				return ValidateInstanceDocument(reader, contentProcessing);
			}
		}

		/// <summary>
		/// Loads and validates an XML file against the <typeparamref name="T"/> and <typeparamref name="T2"/> BizTalk
		/// Server schemas.
		/// </summary>
		/// <typeparam name="T2">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <param name="filepath">
		/// The path of the XML file to load and validate.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the XML file.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument<T2>(string filepath)
			where T2 : SchemaBase, new()
		{
			using (var reader = XmlReader.Create(filepath))
			{
				return ValidateInstanceDocument<T2>(reader);
			}
		}

		/// <summary>
		/// Loads and validates an XML file against the <typeparamref name="T"/> and <typeparamref name="T2"/> BizTalk
		/// Server schemas.
		/// </summary>
		/// <typeparam name="T2">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <param name="filepath">
		/// The path of the XML file to load and validate.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the XML file.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument<T2>(string filepath, XmlSchemaContentProcessing contentProcessing)
			where T2 : SchemaBase, new()
		{
			using (var reader = XmlReader.Create(filepath))
			{
				return ValidateInstanceDocument<T2>(reader, contentProcessing);
			}
		}

		/// <summary>
		/// Loads and validates an XML file against the <typeparamref name="T"/>, <typeparamref name="T2"/> and
		/// <typeparamref name="T3"/> BizTalk Server schemas.
		/// </summary>
		/// <typeparam name="T2">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <typeparam name="T3">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <param name="filepath">
		/// The path of the XML file to load and validate.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the XML file.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument<T2, T3>(string filepath)
			where T2 : SchemaBase, new()
			where T3 : SchemaBase, new()
		{
			using (var reader = XmlReader.Create(filepath))
			{
				return ValidateInstanceDocument<T2, T3>(reader);
			}
		}

		/// <summary>
		/// Loads and validates an XML file against the <typeparamref name="T"/>, <typeparamref name="T2"/> and
		/// <typeparamref name="T3"/> BizTalk Server schemas.
		/// </summary>
		/// <typeparam name="T2">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <typeparam name="T3">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <param name="filepath">
		/// The path of the XML file to load and validate.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the XML file.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument<T2, T3>(string filepath, XmlSchemaContentProcessing contentProcessing)
			where T2 : SchemaBase, new()
			where T3 : SchemaBase, new()
		{
			using (var reader = XmlReader.Create(filepath))
			{
				return ValidateInstanceDocument<T2, T3>(reader, contentProcessing);
			}
		}

		/// <summary>
		/// Validates an <see cref="XmlDocument"/> against the <typeparamref name="T"/> BizTalk Server schema.
		/// </summary>
		/// <param name="document">
		/// The <see cref="XmlDocument"/> to validate.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the input <see cref="XmlDocument"/>.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument(XmlDocument document)
		{
			using (var reader = new XmlNodeReader(document))
			{
				return ValidateInstanceDocument(reader);
			}
		}

		/// <summary>
		/// Validates an <see cref="XmlDocument"/> against the <typeparamref name="T"/> BizTalk Server schema.
		/// </summary>
		/// <param name="document">
		/// The <see cref="XmlDocument"/> to validate.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the input <see cref="XmlDocument"/>.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument(XmlDocument document, XmlSchemaContentProcessing contentProcessing)
		{
			using (var reader = new XmlNodeReader(document))
			{
				return ValidateInstanceDocument(reader, contentProcessing);
			}
		}

		/// <summary>
		/// Validates an <see cref="XmlDocument"/> against the <typeparamref name="T"/> and <typeparamref name="T2"/>
		/// BizTalk Server schemas.
		/// </summary>
		/// <typeparam name="T2">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <param name="document">
		/// The <see cref="XmlDocument"/> to validate.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the input <see cref="XmlDocument"/>.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument<T2>(XmlDocument document)
			where T2 : SchemaBase, new()
		{
			using (var reader = new XmlNodeReader(document))
			{
				return ValidateInstanceDocument<T2>(reader);
			}
		}

		/// <summary>
		/// Validates an <see cref="XmlDocument"/> against the <typeparamref name="T"/> and <typeparamref name="T2"/>
		/// BizTalk Server schemas.
		/// </summary>
		/// <typeparam name="T2">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <param name="document">
		/// The <see cref="XmlDocument"/> to validate.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the input <see cref="XmlDocument"/>.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument<T2>(XmlDocument document, XmlSchemaContentProcessing contentProcessing)
			where T2 : SchemaBase, new()
		{
			using (var reader = new XmlNodeReader(document))
			{
				return ValidateInstanceDocument<T2>(reader, contentProcessing);
			}
		}

		/// <summary>
		/// Validates an <see cref="XmlDocument"/> against the <typeparamref name="T"/>, <typeparamref name="T2"/> and
		/// <typeparamref name="T3"/> BizTalk Server schemas.
		/// </summary>
		/// <typeparam name="T2">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <typeparam name="T3">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <param name="document">
		/// The <see cref="XmlDocument"/> to validate.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the input <see cref="XmlDocument"/>.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument<T2, T3>(XmlDocument document)
			where T2 : SchemaBase, new()
			where T3 : SchemaBase, new()
		{
			using (var reader = new XmlNodeReader(document))
			{
				return ValidateInstanceDocument<T2, T3>(reader);
			}
		}

		/// <summary>
		/// Validates an <see cref="XmlDocument"/> against the <typeparamref name="T"/>, <typeparamref name="T2"/> and
		/// <typeparamref name="T3"/> BizTalk Server schemas.
		/// </summary>
		/// <typeparam name="T2">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <typeparam name="T3">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <param name="document">
		/// The <see cref="XmlDocument"/> to validate.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the input <see cref="XmlDocument"/>.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument<T2, T3>(XmlDocument document, XmlSchemaContentProcessing contentProcessing)
			where T2 : SchemaBase, new()
			where T3 : SchemaBase, new()
		{
			using (var reader = new XmlNodeReader(document))
			{
				return ValidateInstanceDocument<T2, T3>(reader, contentProcessing);
			}
		}

		/// <summary>
		/// Validates an <see cref="Stream"/> against the <typeparamref name="T"/> BizTalk Server schema.
		/// </summary>
		/// <param name="stream">
		/// The <see cref="Stream"/> to validate.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the XML <see cref="Stream"/>.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument(Stream stream)
		{
			using (var reader = XmlReader.Create(stream))
			{
				return ValidateInstanceDocument(reader);
			}
		}

		/// <summary>
		/// Validates an <see cref="Stream"/> against the <typeparamref name="T"/> BizTalk Server schema.
		/// </summary>
		/// <param name="stream">
		/// The <see cref="Stream"/> to validate.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the XML <see cref="Stream"/>.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument(Stream stream, XmlSchemaContentProcessing contentProcessing)
		{
			using (var reader = XmlReader.Create(stream))
			{
				return ValidateInstanceDocument(reader, contentProcessing);
			}
		}

		/// <summary>
		/// Validates an <see cref="Stream"/> against the <typeparamref name="T"/> and <typeparamref name="T2"/>
		/// BizTalk Server schemas.
		/// </summary>
		/// <typeparam name="T2">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <param name="stream">
		/// The <see cref="Stream"/> to validate.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the XML <see cref="Stream"/>.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument<T2>(Stream stream)
			where T2 : SchemaBase, new()
		{
			using (var reader = XmlReader.Create(stream))
			{
				return ValidateInstanceDocument<T2>(reader);
			}
		}

		/// <summary>
		/// Validates an <see cref="Stream"/> against the <typeparamref name="T"/> and <typeparamref name="T2"/>
		/// BizTalk Server schemas.
		/// </summary>
		/// <typeparam name="T2">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <param name="stream">
		/// The <see cref="Stream"/> to validate.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the XML <see cref="Stream"/>.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument<T2>(Stream stream, XmlSchemaContentProcessing contentProcessing)
			where T2 : SchemaBase, new()
		{
			using (var reader = XmlReader.Create(stream))
			{
				return ValidateInstanceDocument<T2>(reader, contentProcessing);
			}
		}

		/// <summary>
		/// Validates an <see cref="Stream"/> against the <typeparamref name="T"/>, <typeparamref name="T2"/> and
		/// <typeparamref name="T3"/> BizTalk Server schemas.
		/// </summary>
		/// <typeparam name="T2">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <typeparam name="T3">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <param name="stream">
		/// The <see cref="Stream"/> to validate.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the XML <see cref="Stream"/>.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument<T2, T3>(Stream stream)
			where T2 : SchemaBase, new()
			where T3 : SchemaBase, new()
		{
			using (var reader = XmlReader.Create(stream))
			{
				return ValidateInstanceDocument<T2, T3>(reader);
			}
		}

		/// <summary>
		/// Validates an <see cref="Stream"/> against the <typeparamref name="T"/>, <typeparamref name="T2"/> and
		/// <typeparamref name="T3"/> BizTalk Server schemas.
		/// </summary>
		/// <typeparam name="T2">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <typeparam name="T3">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <param name="stream">
		/// The <see cref="Stream"/> to validate.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the XML <see cref="Stream"/>.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument<T2, T3>(Stream stream, XmlSchemaContentProcessing contentProcessing)
			where T2 : SchemaBase, new()
			where T3 : SchemaBase, new()
		{
			using (var reader = XmlReader.Create(stream))
			{
				return ValidateInstanceDocument<T2, T3>(reader, contentProcessing);
			}
		}

		/// <summary>
		/// Validates an <see cref="XmlReader"/> against the <typeparamref name="T"/> BizTalk Server schema.
		/// </summary>
		/// <param name="reader">
		/// The <see cref="XmlReader"/> to validate.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the <see cref="XmlReader"/>.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument(XmlReader reader)
		{
			using (var validatingReader = ValidatingXmlReader.Create<T>(reader))
			{
				var xdoc = new XmlDocument();
				xdoc.Load(validatingReader);
				return xdoc;
			}
		}

		/// <summary>
		/// Validates an <see cref="XmlReader"/> against the <typeparamref name="T"/> BizTalk Server schema.
		/// </summary>
		/// <param name="reader">
		/// The <see cref="XmlReader"/> to validate.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the <see cref="XmlReader"/>.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument(XmlReader reader, XmlSchemaContentProcessing contentProcessing)
		{
			using (var validatingReader = ValidatingXmlReader.Create<T>(reader, contentProcessing))
			{
				var xdoc = new XmlDocument();
				xdoc.Load(validatingReader);
				return xdoc;
			}
		}

		/// <summary>
		/// Validates an <see cref="XmlReader"/> against the <typeparamref name="T"/> and <typeparamref name="T2"/>
		/// BizTalk Server schemas.
		/// </summary>
		/// <typeparam name="T2">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <param name="reader">
		/// The <see cref="XmlReader"/> to validate.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the <see cref="XmlReader"/>.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument<T2>(XmlReader reader)
			where T2 : SchemaBase, new()
		{
			using (var validatingReader = ValidatingXmlReader.Create<T, T2>(reader))
			{
				var xdoc = new XmlDocument();
				xdoc.Load(validatingReader);
				return xdoc;
			}
		}

		/// <summary>
		/// Validates an <see cref="XmlReader"/> against the <typeparamref name="T"/> and <typeparamref name="T2"/>
		/// BizTalk Server schemas.
		/// </summary>
		/// <typeparam name="T2">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <param name="reader">
		/// The <see cref="XmlReader"/> to validate.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the <see cref="XmlReader"/>.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument<T2>(XmlReader reader, XmlSchemaContentProcessing contentProcessing)
			where T2 : SchemaBase, new()
		{
			using (var validatingReader = ValidatingXmlReader.Create<T, T2>(reader, contentProcessing))
			{
				var xdoc = new XmlDocument();
				xdoc.Load(validatingReader);
				return xdoc;
			}
		}

		/// <summary>
		/// Validates an <see cref="XmlReader"/> against the <typeparamref name="T"/>, <typeparamref name="T2"/> and
		/// <typeparamref name="T3"/> BizTalk Server schemas.
		/// </summary>
		/// <typeparam name="T2">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <typeparam name="T3">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <param name="reader">
		/// The <see cref="XmlReader"/> to validate.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the <see cref="XmlReader"/>.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument<T2, T3>(XmlReader reader)
			where T2 : SchemaBase, new()
			where T3 : SchemaBase, new()
		{
			using (var validatingReader = ValidatingXmlReader.Create<T, T2, T3>(reader))
			{
				var xdoc = new XmlDocument();
				xdoc.Load(validatingReader);
				return xdoc;
			}
		}

		/// <summary>
		/// Validates an <see cref="XmlReader"/> against the <typeparamref name="T"/>, <typeparamref name="T2"/> and
		/// <typeparamref name="T3"/> BizTalk Server schemas.
		/// </summary>
		/// <typeparam name="T2">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <typeparam name="T3">
		/// A supplementary type of the BizTalk Server schema to vaidate against.
		/// </typeparam>
		/// <param name="reader">
		/// The <see cref="XmlReader"/> to validate.
		/// </param>
		/// <param name="contentProcessing">
		/// Validation mode.
		/// </param>
		/// <returns>
		/// An <see cref="XmlDocument"/> that has loaded and validated the content of the <see cref="XmlReader"/>.
		/// </returns>
		protected XmlDocument ValidateInstanceDocument<T2, T3>(XmlReader reader, XmlSchemaContentProcessing contentProcessing)
			where T2 : SchemaBase, new()
			where T3 : SchemaBase, new()
		{
			using (var validatingReader = ValidatingXmlReader.Create<T, T2, T3>(reader, contentProcessing))
			{
				var xdoc = new XmlDocument();
				xdoc.Load(validatingReader);
				return xdoc;
			}
		}
	}
}
