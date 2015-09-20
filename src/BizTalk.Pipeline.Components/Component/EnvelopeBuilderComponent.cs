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
using Be.Stateless.BizTalk.Schemas.Xml;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// Pipeline component that transforms a <see cref="Batch.Content"/> with all its parts into its affiliated envelope.
	/// </summary>
	/// <remarks>
	/// It is meant to be used in the receive pipeline of the receive location that polls for batches to release.
	/// </remarks>
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[ComponentCategory(CategoryTypes.CATID_Decoder)]
	[Guid(CLASS_ID)]
	public class EnvelopeBuilderComponent : XsltRunnerComponent
	{
		public EnvelopeBuilderComponent() : base(new EnvelopeBuilder()) { }

		#region Base Class Member Overrides

		/// <summary>
		/// Description of the pipeline component.
		/// </summary>
		[Browsable(false)]
		[Description("Description of the pipeline component.")]
		public override string Description
		{
			get { return string.Format("Transforms a {0} message with all its parts into its affiliated envelope.", typeof(Batch.Content).Name); }
		}

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

		#endregion

		private const string CLASS_ID = "9eb66da9-e3d6-4f34-9288-b49ac5ed6f76";
	}
}
