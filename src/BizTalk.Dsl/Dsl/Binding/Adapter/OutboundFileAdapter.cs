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
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Be.Stateless.Extensions;
using Be.Stateless.IO;
using Microsoft.BizTalk.Component.Interop;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class OutboundFileAdapter : FileAdapter, IOutboundAdapter
	{
		private OutboundFileAdapter()
		{
			FileName = "%MessageID%.xml";
			CopyMode = CopyMode.CreateNew;
			UseTempFileOnWrite = true;
		}

		public OutboundFileAdapter(Action<OutboundFileAdapter> adapterConfigurator) : this()
		{
			adapterConfigurator(this);
		}

		#region Base Class Member Overrides

		protected override string Address
		{
			get { return System.IO.Path.Combine(DestinationFolder, FileName); }
		}

		protected override void Save(IPropertyBag propertyBag)
		{
			base.Save(propertyBag);
			propertyBag.WriteAdapterCustomProperty("AllowCacheOnWrite", AllowCacheOnWrite);
			propertyBag.WriteAdapterCustomProperty("CopyMode", Convert.ToUInt32(CopyMode));
			propertyBag.WriteAdapterCustomProperty("FileName", FileName);
			propertyBag.WriteAdapterCustomProperty("UseTempFileOnWrite", UseTempFileOnWrite);
		}

		protected override void Validate()
		{
			if (DestinationFolder.IsNullOrEmpty()) throw new BindingException("Outbond file adapter has no destination folder.");
			if (FileName.IsNullOrEmpty()) throw new BindingException("Outbond file adapter has no destination file name.");
			if (UseTempFileOnWrite && CopyMode != CopyMode.CreateNew) throw new BindingException("Outbond file adapter cannot use a temporary file when it is meant to append or overwrite an existing file.");
			if (!Path.IsNetworkPath(DestinationFolder) && !NetworkCredentials.Username.IsNullOrEmpty()) throw new BindingException("Alternate credentials to access the file folder cannot be supplied while accessing local drive or a mapped network drive.");
		}

		#endregion

		/// <summary>
		/// Allow cache on write.
		/// </summary>
		public bool AllowCacheOnWrite { get; set; }

		/// <summary>
		/// File content writing mode.
		/// </summary>
		public CopyMode CopyMode { get; set; }

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
		/// <description>Restrictions on the File Mask and File Name Properties, see https://msdn.microsoft.com/en-us/library/aa578688.aspx.</description>
		/// </item>
		/// </list>
		/// </remarks>
		public string FileName { get; set; }

		/// <summary>
		/// Use temporary file while writing.
		/// </summary>
		/// <remarks>
		/// Can be set to <c>true</c> only when the <see cref="CopyMode"/> is set to <see
		/// cref="Adapter.CopyMode.CreateNew"/>.
		/// </remarks>
		public bool UseTempFileOnWrite { get; set; }
	}
}
