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

using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Tracking;
using Microsoft.BizTalk.Streaming;

namespace Be.Stateless.BizTalk.Streaming.Extensions
{
	public interface IProbeStream
	{
		/// <summary>
		/// Probes the current <see cref="MarkableForwardOnlyEventingReadStream"/> for the message type.
		/// </summary>
		/// <returns>
		/// The message type if probing is successful, <c>null</c> otherwise.
		/// </returns>
		string MessageType { get; }
	}

	internal interface IProbeBatchContentStream
	{
		/// <summary>
		/// Probes the current <see cref="MarkableForwardOnlyEventingReadStream"/> for a <see cref="BatchDescriptor"/>.
		/// </summary>
		/// <returns>
		/// The <see cref="BatchDescriptor"/> if probing is successful, <c>null</c> otherwise.
		/// </returns>
		/// <remarks>
		/// Probing will be successful only if the <see cref="MarkableForwardOnlyEventingReadStream"/>'s content is an
		/// instance of the <see cref="Batch.Content"/> schema &#8212; only the <see cref="BTS.MessageType"/> is
		/// verified,&#8212; and its <c>EnvelopeSpecName</c> element is not null nor empty.
		/// </remarks>
		BatchDescriptor BatchDescriptor { get; }

		BatchTrackingContext BatchTrackingContext { get; }
	}
}
