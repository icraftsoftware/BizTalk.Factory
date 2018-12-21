#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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
using System.Runtime.InteropServices;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Tracking.Messaging;
using Be.Stateless.Extensions;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Component.Extensions
{
	public static class PipelineContextExtensions
	{
		#region Mock's Factory Hook Point

		internal static Func<IPipelineContext, IActivityFactory> ActivityFactoryFactory
		{
			get { return _activityFactoryFactory; }
			set { _activityFactoryFactory = value; }
		}

		#endregion

		/// <summary>
		/// Tracking-activity factory for messaging-only activities.
		/// </summary>
		/// <param name="pipelineContext">
		/// The pipeline context from which messaging-only activities can be created.
		/// </param>
		/// <returns>
		/// The activity factory.
		/// </returns>
		/// <remarks>
		/// The purpose of this factory is to make <see cref="IPipelineContext"/> extension methods amenable to mocking,
		/// <see href="http://blogs.clariusconsulting.net/kzu/how-to-mock-extension-methods/"/>.
		/// </remarks>
		/// <seealso href="http://blogs.clariusconsulting.net/kzu/how-extension-methods-ruined-unit-testing-and-oop-and-a-way-forward/"/>
		/// <seealso href="http://blogs.clariusconsulting.net/kzu/making-extension-methods-amenable-to-mocking/"/>
		public static IActivityFactory ActivityFactory(this IPipelineContext pipelineContext)
		{
			return _activityFactoryFactory(pipelineContext);
		}

		/// <summary>
		/// Returns the <see cref="ISchemaMetadata"/> associated to the XML schema of messages of a given <see
		/// cref="DocumentSpec"/> type.
		/// </summary>
		/// <param name="pipelineContext">
		/// The pipeline context from which the <see cref="DocumentSpec"/> can be queried.
		/// </param>
		/// <param name="docType">
		/// The <see cref="DocumentSpec"/> type of the messages for which the <see cref="ISchemaMetadata"/> are to be
		/// returned.
		/// </param>
		/// <returns>
		/// The <see cref="ISchemaMetadata"/> associated to the XML Schema.
		/// </returns>
		public static ISchemaMetadata GetSchemaMetadataByType(this IPipelineContext pipelineContext, string docType)
		{
			var docSpec = pipelineContext.GetDocumentSpecByType(docType);
			var schemaType = Type.GetType(docSpec.DocSpecStrongName, true);
			return schemaType.GetMetadata();
		}

		/// <summary>
		/// Returns the <see cref="ISchemaMetadata"/> associated to the XML schema of messages of a given <see
		/// cref="DocumentSpec"/> type.
		/// </summary>
		/// <param name="pipelineContext">
		/// The pipeline context from which the <see cref="DocumentSpec"/> can be queried.
		/// </param>
		/// <param name="docType">
		/// The <see cref="DocumentSpec"/> type of the messages for which the <see cref="ISchemaMetadata"/> are to be
		/// returned.
		/// </param>
		/// <param name="throwOnError">
		/// <c>false</c> to swallow <see cref="COMException"/> and return a <see cref="SchemaMetadata.Unknown"/> should
		/// the document specification not to be found; it will however be logged as a warning. <c>true</c> to let any
		/// exception through.
		/// </param>
		/// <returns>
		/// The <see cref="ISchemaMetadata"/> associated to the XML Schema.
		/// </returns>
		public static ISchemaMetadata GetSchemaMetadataByType(this IPipelineContext pipelineContext, string docType, bool throwOnError)
		{
			if (throwOnError) return pipelineContext.GetSchemaMetadataByType(docType);

			var schemaType = docType
				.IfNotNullOrEmpty(dt => pipelineContext.SafeGetDocumentSpecByType(dt))
				.IfNotNull(docSpec => Type.GetType(docSpec.DocSpecStrongName, false));
			return schemaType.IsSchema() ? schemaType.GetMetadata() : SchemaMetadata.Unknown;
		}

		private static IDocumentSpec SafeGetDocumentSpecByType(this IPipelineContext pipelineContext, string docType)
		{
			try
			{
				return pipelineContext.GetDocumentSpecByType(docType);
			}
			catch (COMException exception)
			{
				// test HResult for Finding the document specification by message type "..." failed. Verify the schema deployed properly.
				if (exception.HResult == E_SCHEMA_NOT_FOUND) return null;
				if (_logger.IsWarnEnabled) _logger.Warn(string.Format("SafeGetDocumentSpecByType({0}) has failed.", docType), exception);
				throw;
			}
		}

		internal static IKernelTransaction GetKernelTransaction(this IPipelineContext pipelineContext)
		{
			return (IKernelTransaction) ((IPipelineContextEx) pipelineContext).GetTransaction();
		}

		internal const int E_SCHEMA_NOT_FOUND = unchecked((int) 0xC0C01300);

		private static Func<IPipelineContext, IActivityFactory> _activityFactoryFactory = pipelineContext => new ActivityFactory(pipelineContext);
		private static readonly ILog _logger = LogManager.GetLogger(typeof(PipelineContextExtensions));
	}
}
