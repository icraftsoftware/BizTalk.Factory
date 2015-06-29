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
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.BizTalk.Streaming.Extensions;
using Be.Stateless.BizTalk.Transforms.ToXml;
using Be.Stateless.Extensions;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

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
		public EnvelopeBuilderComponent()
		{
			Map = typeof(BatchContentToAnyEnvelope);
		}

		#region IBaseComponent members

		/// <summary>
		/// Description of the pipeline component.
		/// </summary>
		[Browsable(false)]
		[Description("Description of the pipeline component.")]
		public override string Description
		{
			get { return string.Format("Transforms a {0} message with all its parts into its affiliated envelope.", typeof(Batch.Content).Name); }
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

		#endregion

		#region Base Class Member Overrides

		protected internal override IBaseMessage ExecuteCore(IPipelineContext pipelineContext, IBaseMessage message)
		{
			var markableForwardOnlyEventingReadStream = message.BodyPart.WrapOriginalDataStream(
				originalStream => originalStream.AsMarkable(),
				pipelineContext.ResourceTracker);

			var batchDescriptor = ((IProbeBatchContentStream) markableForwardOnlyEventingReadStream.Probe()).BatchDescriptor;
			if (batchDescriptor == null || batchDescriptor.EnvelopeSpecName.IsNullOrEmpty())
				throw new InvalidOperationException(
					string.Format(
						"No EnvelopeSpecName has been found in {0} message and no envelope can be applied.",
						typeof(Batch.Content).Name));
			var envelopeSchema = Type.GetType(batchDescriptor.EnvelopeSpecName, true);
			message.Promote(BizTalkFactoryProperties.EnvelopePartition, batchDescriptor.Partition);

			// ReSharper disable once PossibleNullReferenceException
			if (_logger.IsInfoEnabled) _logger.DebugFormat("Applying '{0}' envelope to message.", envelopeSchema.AssemblyQualifiedName);
			var envelope = MessageFactory.CreateEnvelope(envelopeSchema);
			// can't use StringStream, which is Unicode/UTF-16, over envelope.OuterXml as CompositeStream assumes UTF-8
			var envelopeStream = new MemoryStream(Encoding.UTF8.GetBytes(envelope.OuterXml));
			message.BodyPart.WrapOriginalDataStream(
				originalStream => new CompositeStream(new[] { envelopeStream, originalStream }),
				pipelineContext.ResourceTracker);

			markableForwardOnlyEventingReadStream.StopMarking();

			envelopeSchema.GetMetadata().Annotations.EnvelopingMap.IfNotNull(m => Map = m);
			return base.ExecuteCore(pipelineContext, message);
		}

		#endregion

		private const string CLASS_ID = "9eb66da9-e3d6-4f34-9288-b49ac5ed6f76";
		private static readonly ILog _logger = LogManager.GetLogger(typeof(EnvelopeBuilderComponent));
	}
}
