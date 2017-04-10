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
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Be.Stateless.Extensions;
using Be.Stateless.IO;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class FileAdapter
	{
		#region Nested Type: Inbound

		public class Inbound : FileAdapter, IInboundAdapter
		{
			private Inbound()
			{
				FileMask = "*.xml";
				BatchMessagesCount = 20;
				BatchSize = 102400;
				RenameReceivedFiles = true;
				PollingInterval = TimeSpan.FromMinutes(1);
				FileRemovingSettings = new FileRemovingSettings();
				RetryCountOnNetworkFailure = 5;
				RetryIntervalOnNetworkFailure = TimeSpan.FromMinutes(5);
			}

			public Inbound(Action<Inbound> adapterConfigurator) : this()
			{
				adapterConfigurator(this);
			}

			#region Base Class Member Overrides

			protected override string GetAddress()
			{
				return System.IO.Path.Combine(ReceiveFolder, FileMask);
			}

			protected override void Validate()
			{
				if (ReceiveFolder.IsNullOrEmpty()) throw new BindingException("Inbound file adapter has no source folder.");
				if (FileMask.IsNullOrEmpty()) throw new BindingException("Inbound file adapter has no source file mask.");
				if (!Path.IsNetworkPath(ReceiveFolder) && !NetworkCredentials.UserName.IsNullOrEmpty()) throw new BindingException("Alternate credentials to access the file folder cannot be supplied while accessing local drive or a mapped network drive.");
			}

			#endregion

			#region Base Class Member Overrides

			protected override void Save(IPropertyBag propertyBag)
			{
				base.Save(propertyBag);
				propertyBag.WriteAdapterCustomProperty("BatchSize", Convert.ToUInt32(BatchMessagesCount));
				propertyBag.WriteAdapterCustomProperty("BatchSizeInBytes", BatchSize);
				propertyBag.WriteAdapterCustomProperty("FileMask", FileMask);
				propertyBag.WriteAdapterCustomProperty("FileNetFailRetryCount", RetryCountOnNetworkFailure);
				propertyBag.WriteAdapterCustomProperty("FileNetFailRetryInt", Convert.ToUInt32(RetryIntervalOnNetworkFailure.TotalMinutes));
				propertyBag.WriteAdapterCustomProperty("PollingInterval", Convert.ToUInt32(PollingInterval.TotalMilliseconds));
				propertyBag.WriteAdapterCustomProperty("RemoveReceivedFileDelay", Convert.ToUInt32(FileRemovingSettings.RetryInterval.TotalMilliseconds));
				propertyBag.WriteAdapterCustomProperty("RemoveReceivedFileMaxInterval", Convert.ToUInt32(FileRemovingSettings.MaxRetryInterval.TotalMilliseconds));
				propertyBag.WriteAdapterCustomProperty("RemoveReceivedFileRetryCount", FileRemovingSettings.RetryCount);
				propertyBag.WriteAdapterCustomProperty("RenameReceivedFiles", RenameReceivedFiles);
			}

			#endregion

			/// <summary>
			/// Source file mask.
			/// </summary>
			/// <remarks>
			/// <list type="bullet">
			/// <item>
			/// <description>Restrictions on the File Mask and File Name Properties, see https://msdn.microsoft.com/en-us/library/aa578688.aspx.</description>
			/// <description>Restrictions on Using Macros in File Names, https://msdn.microsoft.com/en-us/library/aa578022.aspx.</description>
			/// </item>
			/// </list>
			/// </remarks>
			public string FileMask { get; set; }

			/// <summary>
			/// Source folder.
			/// </summary>
			/// <remarks>
			/// <list type="bullet">
			/// <item>
			/// <description>Restrictions on the Receive Folder and Destination Location Properties, https://msdn.microsoft.com/en-us/library/aa561308.aspx</description>
			/// </item>
			/// </list>
			/// </remarks>
			public string ReceiveFolder { get; set; }

			#region Advanced Settings

			/// <summary>
			/// Renames files while reading.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Specify whether to rename files before picking them up for processing
			/// </para>
			/// <para>
			/// See also File Transport Properties Dialog Box, Receive, Advanced Settings Dialog Box,
			/// https://msdn.microsoft.com/en-us/library/aa559438.aspx.
			/// </para>
			/// </remarks>
			public bool RenameReceivedFiles { get; set; }

			/// <summary>
			/// Receive location polling interval.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Specify the interval frequency that the file adapter will use to poll the specified location for new files.
			/// </para>
			/// <para>
			/// See also File Transport Properties Dialog Box, Receive, Advanced Settings Dialog Box,
			/// https://msdn.microsoft.com/en-us/library/aa559438.aspx.
			/// </para>
			/// </remarks>
			public TimeSpan PollingInterval { get; set; }

			public FileRemovingSettings FileRemovingSettings { get; set; }

			#endregion

			#region Batching

			/// <summary>
			/// Specify the maximum number of messages to be submitted in a batch.
			/// </summary>
			public byte BatchMessagesCount { get; set; }

			/// <summary>
			/// Specify the maximum total bytes for a batch.
			/// </summary>
			public uint BatchSize { get; set; }

			#endregion

			#region Network Failure

			/// <summary>
			/// Specify the number of attempts to access the receive location on a network share if it is temporarily
			/// unavailable.
			/// </summary>
			public uint RetryCountOnNetworkFailure { get; set; }

			/// <summary>
			/// Specify the retry interval time (in minutes) between attempts to access the receive location on the network
			/// share if it is temporarily unavailable.
			/// </summary>
			public TimeSpan RetryIntervalOnNetworkFailure { get; set; }

			#endregion
		}

		#endregion
	}
}
