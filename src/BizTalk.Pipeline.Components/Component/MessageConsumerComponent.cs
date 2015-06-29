#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.IO.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Component
{
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[ComponentCategory(CategoryTypes.CATID_Any)]
	[Guid(CLASS_ID)]
	// TODO derive from MessageTrackerComponent
	public class MessageConsumerComponent : PipelineComponent
	{
		#region IBaseComponent members

		/// <summary>
		/// Description of the component
		/// </summary>
		[Browsable(false)]
		public override string Description
		{
			get { return "Drain and consume the pipeline message."; }
		}

		#endregion

		#region IPersistPropertyBag members

		/// <summary>
		/// Gets class ID of component for usage from unmanaged code.
		/// </summary>
		/// <param name="classId">
		/// Class ID of the component
		/// </param>
		public override void GetClassID(out Guid classId)
		{
			classId = new Guid(CLASS_ID);
		}

		protected override void Load(IPropertyBag propertyBag) { }

		protected override void Save(IPropertyBag propertyBag) { }

		#endregion

		#region Base Class Member Overrides

		protected internal override IBaseMessage ExecuteCore(IPipelineContext pipelineContext, IBaseMessage message)
		{
			// drain the message
			message.BodyPart.GetOriginalDataStream().Drain();
			// because of absorption, ensure no ack is generated should one be required
			message.SetProperty(BtsProperties.AckRequired, false);
			// absorb the message
			return null;
		}

		#endregion

		private const string CLASS_ID = "bf843dd6-b68f-444f-890d-a0f648d788db";
	}
}
