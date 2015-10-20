#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Be.Stateless.Extensions;
using Be.Stateless.IO;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class FileAdapter
	{
		#region Nested Type: Outbound

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API.")]
		public class Outbound : FileAdapter, IOutboundAdapter
		{
			private Outbound()
			{
				FileName = "%MessageID%.xml";
				Mode = CopyMode.CreateNew;
				UseTempFileOnWrite = true;
			}

			public Outbound(Action<Outbound> adapterConfigurator) : this()
			{
				adapterConfigurator(this);
			}

			#region Base Class Member Overrides

			protected override string GetAddress()
			{
				return System.IO.Path.Combine(DestinationFolder, FileName);
			}

			protected override void Validate()
			{
				if (DestinationFolder.IsNullOrEmpty()) throw new BindingException("Outbond file adapter has no destination folder.");
				if (FileName.IsNullOrEmpty()) throw new BindingException("Outbond file adapter has no destination file name.");
				if (UseTempFileOnWrite && Mode != CopyMode.CreateNew) throw new BindingException("Outbond file adapter cannot use a temporary file when it is meant to append or overwrite an existing file.");
				if (!Path.IsNetworkPath(DestinationFolder) && !NetworkCredentials.Username.IsNullOrEmpty()) throw new BindingException("Alternate credentials to access the file folder cannot be supplied while accessing local drive or a mapped network drive.");
			}

			#endregion

			#region Base Class Member Overrides

			protected override void Save(IPropertyBag propertyBag)
			{
				base.Save(propertyBag);
				propertyBag.WriteAdapterCustomProperty("AllowCacheOnWrite", AllowCacheOnWrite);
				propertyBag.WriteAdapterCustomProperty("CopyMode", Convert.ToUInt32(Mode));
				propertyBag.WriteAdapterCustomProperty("FileName", FileName);
				propertyBag.WriteAdapterCustomProperty("UseTempFileOnWrite", UseTempFileOnWrite);
			}

			#endregion

			/// <summary>
			/// Allow cache on write.
			/// </summary>
			public bool AllowCacheOnWrite { get; set; }

			/// <summary>
			/// Destination folder.
			/// </summary>
			public string DestinationFolder { get; set; }

			/// <summary>
			/// Destination file name.
			/// </summary>
			/// <remarks>
			/// <list type="bullet">
			/// <item>
			/// <description>
			/// <see href="https://msdn.microsoft.com/en-us/library/aa578688.aspx">Restrictions on the File Mask and File Name Properties.</see>
			/// </description>
			/// </item>
			/// </list>
			/// </remarks>
			public string FileName { get; set; }

			/// <summary>
			/// File content writing mode.
			/// </summary>
			public CopyMode Mode { get; set; }

			/// <summary>
			/// Use temporary file while writing.
			/// </summary>
			/// <remarks>
			/// Can be set to <c>true</c> only when the <see cref="Mode"/> is set to <see
			/// cref="FileAdapter.CopyMode.CreateNew"/>.
			/// </remarks>
			public bool UseTempFileOnWrite { get; set; }
		}

		#endregion
	}
}
