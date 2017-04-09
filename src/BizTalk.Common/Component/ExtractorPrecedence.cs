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

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// Determines how to merge two <see cref="PropertyExtractorCollection"/> instances.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="PropertyExtractorCollection"/> can only be declared at 2 places, via XML schema annotations or via
	/// pipeline configuration. By default, precedence will be given to the <see cref="PropertyExtractorCollection"/>
	/// being declared closest to the runtime execution point; hence the default <see cref="ExtractorPrecedence"/>: <see
	/// cref="Pipeline"/>.
	/// </para>
	/// <para>
	/// Should conflicting <see cref="ExtractorPrecedence"/>s be declared by XML schema annotations and pipeline
	/// configuration, the <see cref="ExtractorPrecedence"/> declared by the pipeline configuration will always have the
	/// precedence over the one declared by XML schema annotations. It would consequently make sense to <b>only declare
	/// the <see cref="ExtractorPrecedence"/> at the pipeline level.</b> Declaring an <see cref="ExtractorPrecedence"/>
	/// via XML schema annotations is possible but pointless.
	/// </para>
	/// </remarks>
#pragma warning disable 1584,1711,1572,1581,1580
	/// <seealso cref="Be.Stateless.BizTalk.Component.ContextPropertyExtractorComponent"/>
#pragma warning restore 1584,1711,1572,1581,1580
	public enum ExtractorPrecedence
	{
		/// <summary>
		/// <see cref="PropertyExtractorCollection"/> declared by pipeline configuration have precedence over the one
		/// declared by XML schema annotations.
		/// </summary>
		Pipeline = 0,

		/// <summary>
		/// <see cref="PropertyExtractorCollection"/> declared by XML schema annotations have precedence over the one
		/// declared by pipeline configuration.
		/// </summary>
		Schema,

		/// <summary>
		/// Only the <see cref="PropertyExtractorCollection"/> declared by pipeline configuration is taken into account
		/// and the one declared by XML schema annotations is ignored.
		/// </summary>
		PipelineOnly,

		/// <summary>
		/// Only <see cref="PropertyExtractorCollection"/> declared by XML schema annotations is taken into account and
		/// the one declared by pipeline configuration is ignored.
		/// </summary>
		SchemaOnly
	}
}
