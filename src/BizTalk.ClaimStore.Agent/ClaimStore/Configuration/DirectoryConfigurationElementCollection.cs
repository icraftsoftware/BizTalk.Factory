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
using System.Configuration;

namespace Be.Stateless.BizTalk.ClaimStore.Configuration
{
	/// <summary>
	/// The collection of folders to collect claimed and tracked message bodies from.
	/// </summary>
	[ConfigurationCollection(typeof(DirectoryConfigurationElement), AddItemName = DIRECTORY_COLLECTION_ITEM_NAME)]
	public sealed class DirectoryConfigurationElementCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DirectoryConfigurationElementCollection"/> class.
		/// </summary>
		public DirectoryConfigurationElementCollection() : base(StringComparer.OrdinalIgnoreCase)
		{
			AddElementName = DIRECTORY_COLLECTION_ITEM_NAME;
		}

		#region Base Class Member Overrides

		/// <summary>
		/// Creates a new <see cref="DirectoryConfigurationElement"/>.
		/// </summary>
		/// <returns>
		/// A new <see cref="DirectoryConfigurationElement"/>.
		/// </returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new DirectoryConfigurationElement();
		}

		/// <summary>
		/// Gets the element key for a specified configuration element when overridden in a derived class.
		/// </summary>
		/// <param name="element">
		/// The <see cref="ConfigurationElement"/>-derived <see cref="DirectoryConfigurationElement"/> to return the key for.
		/// </param>
		/// <returns>
		/// </returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((DirectoryConfigurationElement) element).Path;
		}

		#endregion

		private const string DIRECTORY_COLLECTION_ITEM_NAME = "directory";
	}
}
