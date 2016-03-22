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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Resources;
using Be.Stateless.BizTalk.Component.Interop;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using IComponent = Microsoft.BizTalk.Component.Interop.IComponent;

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// Base class for BizTalk Server pipeline components.
	/// </summary>
	public abstract class PipelineComponent : IBaseComponent, IComponent, IComponentUI, IPersistPropertyBag
	{
		protected PipelineComponent()
		{
			_enabled = true;
		}

		#region Implementation of IBaseComponent

		/// <summary>
		/// Description of the pipeline component.
		/// </summary>
		[Browsable(false)]
		[Description("Description of the pipeline component.")]
		public abstract string Description { get; }

		/// <summary>
		/// Name of the pipeline component.
		/// </summary>
		[Browsable(false)]
		[Description("Name of the pipeline component.")]
		public virtual string Name
		{
			get { return GetType().Name; }
		}

		/// <summary>
		/// Version of the pipeline component.
		/// </summary>
		[Browsable(false)]
		[Description("Version of the pipeline component.")]
		public virtual string Version
		{
			get { return "1.0"; }
		}

		#endregion

		#region IComponent Members

		/// <summary>
		/// Executes a pipeline component to process the input message and get the resulting message.
		/// </summary>
		/// <param name="pipelineContext">
		/// The <see cref="IPipelineContext" /> that contains the current pipeline context.
		/// </param>
		/// <param name="message">
		/// The <see cref="IBaseMessage" /> that contains the message to process.
		/// </param>
		/// <returns>
		/// The <see cref="IBaseMessage" /> that contains the resulting message.
		/// </returns>
		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			try
			{
				if (message == null) return null;
				if (pipelineContext == null) throw new ArgumentNullException("pipelineContext");

				if (_logger.IsDebugEnabled) _logger.DebugFormat("Entering pipeline component {0}.", GetType().ToString());
				if (!Enabled)
				{
					if (_logger.IsDebugEnabled) _logger.DebugFormat("Pipeline component {0} is disabled, skipping execution.", GetType().ToString());
					return message;
				}

				if (_logger.IsDebugEnabled) _logger.DebugFormat("Pipeline component {0} is enabled, starting execution.", GetType().ToString());
				return ExecuteCore(pipelineContext, message);
			}
			catch (Exception exception)
			{
				if (_logger.IsWarnEnabled) _logger.Warn(string.Format("An exception occurred while executing pipeline component {0}.", GetType()), exception);
				throw;
			}
		}

		#endregion

		#region IComponentUI Members

		/// <summary>
		/// Component icon to use in BizTalk Editor
		/// </summary>
		[Browsable(false)]
		public virtual IntPtr Icon
		{
			get
			{
				var resource = _resourceManager.GetObject("COMPONENTICON", CultureInfo.InvariantCulture);
				return resource != null ? ((Bitmap) resource).GetHicon() : IntPtr.Zero;
			}
		}

		/// <summary>
		/// The Validate method is called by the BizTalk Editor during the build of a BizTalk project.
		/// </summary>
		/// <param name="projectSystem">
		/// An object containing the configuration properties.
		/// </param>
		/// <returns>
		/// The IEnumerator enables the caller to enumerate through a collection of strings containing error messages.
		/// These error messages appear as compiler error messages. To report successful property validation, the method
		/// should return an empty enumerator.
		/// </returns>
		public virtual IEnumerator Validate(object projectSystem)
		{
			return null;
		}

		#endregion

		#region IPersistPropertyBag

		public abstract void GetClassID(out Guid classId);

		/// <summary>
		/// Not implemented
		/// </summary>
		void IPersistPropertyBag.InitNew() { }

		/// <summary>
		/// Loads the pipeline component configuration. This base class implementation must be called by derived classes
		/// if they override it.
		/// </summary>
		/// <param name="propertyBag"></param>
		/// <param name="errorLog"></param>
		public void Load(IPropertyBag propertyBag, int errorLog)
		{
			propertyBag.ReadProperty("Enabled", value => Enabled = value);
			Load(propertyBag);
		}

		/// <summary>
		/// Saves the pipeline component configuration. This base class implementation must be called by derived classes
		/// if they override it.
		/// </summary>
		/// <param name="propertyBag"></param>
		/// <param name="clearDirty"></param>
		/// <param name="saveAllProperties"></param>
		public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
		{
			propertyBag.WriteProperty("Enabled", Enabled);
			Save(propertyBag);
		}

		#endregion

		/// <summary>
		/// Enables or disables the pipeline component.
		/// </summary>
		/// <remarks>
		/// Whether to let this pipeline component execute or not.
		/// </remarks>
		[Browsable(true)]
		[Description("Enables or disables the pipeline component.")]
		public virtual bool Enabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}

		/// <summary>
		/// Loads configuration properties for the component
		/// </summary>
		/// <param name="propertyBag">Configuration property bag</param>
		protected abstract void Load(IPropertyBag propertyBag);

		/// <summary>
		/// Saves the pipeline component configuration. This base class implementation must be called by derived classes
		/// if they override it.
		/// </summary>
		/// <param name="propertyBag"></param>
		protected abstract void Save(IPropertyBag propertyBag);

		/// <summary>
		/// Executes a pipeline component to process the input message and get the resulting message.
		/// </summary>
		/// <param name="pipelineContext">
		/// The <see cref="IPipelineContext" /> that contains the current pipeline context.
		/// </param>
		/// <param name="message">
		/// The <see cref="IBaseMessage" /> that contains the message to process.
		/// </param>
		/// <returns>
		/// The <see cref="IBaseMessage" /> that contains the resulting message.
		/// </returns>
		protected internal abstract IBaseMessage ExecuteCore(IPipelineContext pipelineContext, IBaseMessage message);

		private static readonly ResourceManager _resourceManager = new ResourceManager(typeof(PipelineComponent));
		private static readonly ILog _logger = LogManager.GetLogger(typeof(PipelineComponent));
		private bool _enabled;
	}
}
