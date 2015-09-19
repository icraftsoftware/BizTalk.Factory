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
using System.ComponentModel;
using System.Runtime.InteropServices;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.Streaming;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// Wraps the message's original stream in a <see cref="ZipInputStream"/>.
	/// </summary>
	/// <remarks>
	/// Decompress inbound zipped messages.
	/// </remarks>
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[Guid(CLASS_ID)]
	[ComponentCategory(CategoryTypes.CATID_Decoder)]
	public class ZipDecoderComponent : PipelineComponent
	{
		public ZipDecoderComponent()
		{
			_microComponent = new ZipDecoder();
		}

		#region Base Class Member Overrides

		[Browsable(false)]
		public override string Description
		{
			get { return "Wraps the message's original stream in a zip-decompressing stream."; }
		}

		protected internal override IBaseMessage ExecuteCore(IPipelineContext pipelineContext, IBaseMessage message)
		{
			return _microComponent.Execute(pipelineContext, message);
		}

		public override void GetClassID(out Guid classId)
		{
			classId = new Guid(CLASS_ID);
		}

		protected override void Load(IPropertyBag propertyBag) { }

		protected override void Save(IPropertyBag propertyBag) { }

		#endregion

		private const string CLASS_ID = "0a05a00f-e797-4b00-8af2-08c4263e7d39";
		private readonly ZipDecoder _microComponent;
	}
}
