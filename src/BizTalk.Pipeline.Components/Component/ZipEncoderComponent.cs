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
using Be.Stateless.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// Wraps the message's original stream in an zip-compressing stream.
	/// </summary>
	/// <remarks>
	/// Zip-compress outbound message's body part stream.
	/// </remarks>
	/// <seealso cref="ZipOutputStream"/>
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[Guid(CLASS_ID)]
	[ComponentCategory(CategoryTypes.CATID_Encoder)]
	public class ZipEncoderComponent : PipelineComponent
	{
		public ZipEncoderComponent()
		{
			_microComponent = new ZipEncoder();
		}

		#region Base Class Member Overrides

		[Browsable(false)]
		public override string Description
		{
			get { return "Wraps the message's original stream in an zip-compressing stream."; }
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

		private const string CLASS_ID = "0895f316-1e7b-46c4-ba19-7d357d5ac116";
		private static readonly ILog _logger = LogManager.GetLogger(typeof(ZipEncoderComponent));
		private readonly ZipEncoder _microComponent;
	}
}
