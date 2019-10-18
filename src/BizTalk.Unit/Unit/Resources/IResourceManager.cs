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
using System.IO;
using System.Xml.Xsl;

namespace Be.Stateless.BizTalk.Unit.Resources
{
	public interface IResourceManager
	{
		/// <summary>
		/// Loads a <see cref="Stream"/> embedded in the calling assembly.
		/// </summary>
		/// <param name="name">
		/// The name of the <see cref="Stream"/> resource.
		/// </param>
		/// <returns>
		/// The resource <see cref="Stream"/>.
		/// </returns>
		Stream Load(string name);

		/// <summary>
		/// Loads and deserializes a resource embedded in the calling assembly.
		/// </summary>
		/// <typeparam name="T">
		/// The type of the deserialized resource.
		/// </typeparam>
		/// <param name="name">
		/// The name of the resource.
		/// </param>
		/// <param name="deserializer">
		/// A delegate to a method that can deserialize the resource from its stream.
		/// </param>
		/// <returns>
		/// The deserialized resource.
		/// </returns>
		T Load<T>(string name, Func<Stream, T> deserializer);

		/// <summary>
		/// Loads and deserializes a <see cref="string"/> embedded in the calling assembly.
		/// </summary>
		/// <param name="name">
		/// The name of the <see cref="string"/> resource.
		/// </param>
		/// <returns>
		/// The deserialized <see cref="string"/>.
		/// </returns>
		string LoadString(string name);

		/// <summary>
		/// Loads and deserializes an <see cref="XslCompiledTransform"/> embedded in the calling assembly.
		/// </summary>
		/// <param name="name">
		/// The name of the <see cref="XslCompiledTransform"/> resource.
		/// </param>
		/// <returns>
		/// The deserialized <see cref="XslCompiledTransform"/>.
		/// </returns>
		XslCompiledTransform LoadTransform(string name);

		/// <summary>
		/// Loads and deserializes an XML <see cref="string"/> embedded in the calling assembly.
		/// </summary>
		/// <param name="name">
		/// The name of the XML <see cref="string"/> resource.
		/// </param>
		/// <returns>
		/// The deserialized XML <see cref="string"/>.
		/// </returns>
		/// <remarks>
		/// Contrasting with <see cref="LoadString"/>, this methods strips the XML string from all new-line and indentation formatting characters.
		/// </remarks>
		string LoadXmlString(string name);
	}
}
