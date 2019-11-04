#region Copyright & License

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

using System;
using System.Xml;
using System.Xml.Schema;
using Be.Stateless.Linq.Extensions;

namespace Be.Stateless.Xml
{
	/// <summary>
	/// Creates the necessary <see cref="XmlReaderSettings"/> that an <see cref="XmlReader"/> instance needs to validate
	/// its content against one or more XSD schemas while reading it.
	/// </summary>
	public static class ValidatingXmlReaderSettings
	{
		/// <summary>
		/// Specifies a set of <see cref="XmlReaderSettings"/> features to support XSD validation on the <see cref="XmlReader"/> object created by the <see
		/// cref="XmlReader.Create(XmlReader,XmlReaderSettings)"/> method or one of its other overloads.
		/// </summary>
		/// <param name="contentProcessing">
		/// The validation mode of the whole document and not only the any and anyAttribute element replacements.
		/// </param>
		/// <param name="schemas">
		/// The XSD schemas against which to validate the XML content.
		/// </param>
		/// <returns>
		/// The <see cref="XmlReaderSettings"/> needed by an <see cref="XmlReader"/> instance needs to validate its content against the given XSD <paramref
		/// name="schemas"/>.
		/// </returns>
		/// <remarks>
		/// <para>
		/// If <paramref name="contentProcessing"/> is <see cref="XmlSchemaContentProcessing.Lax"/>, the <see cref="XmlReader"/> won't fail the validation if it cannot
		/// obtain a schema for any of the processed XML namespaces.
		/// </para>
		/// <para>
		/// If the <paramref name="contentProcessing"/> is <see cref="XmlSchemaContentProcessing.Strict"/>, the <see cref="XmlReader"/> will fail the validation if it
		/// cannot obtain a schema for any of the processed XML namespaces and, more specifically, any reported validation warning (<see
		/// cref="XmlSchemaValidationFlags.ReportValidationWarnings"/>) will converted into an <see cref="XmlSchemaValidationException"/>.
		/// </para>
		/// </remarks>
		/// <seealso cref="XmlSchemaContentProcessing"/>
		public static XmlReaderSettings Create(XmlSchemaContentProcessing contentProcessing, params XmlSchema[] schemas)
		{
			return Create(
				contentProcessing,
				(o, args) => { throw new XmlSchemaValidationException(string.Format("{0}: {1}", args.Severity, args.Message), args.Exception); },
				schemas);
		}

		/// <summary>
		/// Specifies a set of <see cref="XmlReaderSettings"/> features to support XSD validation on the <see cref="XmlReader"/> object created by the <see
		/// cref="XmlReader.Create(XmlReader,XmlReaderSettings)"/> method or one of its other overloads.
		/// </summary>
		/// <param name="contentProcessing">
		/// The validation mode of the whole document and not only the any and anyAttribute element replacements.
		/// </param>
		/// <param name="validationEventHandler">
		/// Represents the callback method that will handle XML schema validation events and the <see cref="ValidationEventArgs"/>. Notice the callback will only be
		/// invoked if there is an exception thrown by the validating <see cref="XmlReader"/>, via <see cref="ValidationEventArgs"/>.<see
		/// cref="ValidationEventArgs.Exception"/>, and <paramref name="contentProcessing"/> is different than <see cref="XmlSchemaContentProcessing.Skip"/>.
		/// </param>
		/// <param name="schemas">
		/// The XSD schemas against which to validate the XML content.
		/// </param>
		/// <returns>
		/// The <see cref="XmlReaderSettings"/> needed by an <see cref="XmlReader"/> instance needs to validate its content against the given XSD <paramref
		/// name="schemas"/>.
		/// </returns>
		/// <remarks>
		/// <para>
		/// If <paramref name="contentProcessing"/> is <see cref="XmlSchemaContentProcessing.Lax"/>, the <see cref="XmlReader"/> won't fail the validation if it cannot
		/// obtain a schema for any of the processed XML namespaces.
		/// </para>
		/// <para>
		/// If the <paramref name="contentProcessing"/> is <see cref="XmlSchemaContentProcessing.Strict"/>, the <see cref="XmlReader"/> will fail the validation if it
		/// cannot obtain a schema for any of the processed XML namespaces and, more specifically, any reported validation warning (<see
		/// cref="XmlSchemaValidationFlags.ReportValidationWarnings"/>) will converted into an <see cref="XmlSchemaValidationException"/>.
		/// </para>
		/// </remarks>
		/// <seealso cref="XmlSchemaContentProcessing"/>
		public static XmlReaderSettings Create(XmlSchemaContentProcessing contentProcessing, ValidationEventHandler validationEventHandler, params XmlSchema[] schemas)
		{
			if (validationEventHandler == null) throw new ArgumentNullException("validationEventHandler");
			var readerSettings = new XmlReaderSettings();
			if (contentProcessing != XmlSchemaContentProcessing.None)
			{
				schemas.Each(s => readerSettings.Schemas.Add(s));
				readerSettings.ValidationEventHandler +=
					(o, args) => {
						if (args.Exception != null && contentProcessing != XmlSchemaContentProcessing.Skip)
						{
							validationEventHandler(o, args);
						}
					};
				if (contentProcessing == XmlSchemaContentProcessing.Strict) readerSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
				readerSettings.ValidationType = ValidationType.Schema;
			}
			return readerSettings;
		}
	}
}
