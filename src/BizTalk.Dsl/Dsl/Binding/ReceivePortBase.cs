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
using System.Linq;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Diagnostics;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public abstract class ReceivePortBase<TNamingConvention>
		: IReceivePort<TNamingConvention>,
			ISupportEnvironmentSensitivity,
			ISupportNamingConvention,
			ISupportValidation,
			IProvideSourceFileInformation,
			IVisitable<IApplicationBindingVisitor>
		where TNamingConvention : class
	{
		protected internal ReceivePortBase()
		{
			_receiveLocations = new ReceiveLocationCollection<TNamingConvention>(this);
			_sourceFileInformationProvider = new SourceFileInformationProvider();
			_sourceFileInformationProvider.Capture();
		}

		protected internal ReceivePortBase(Action<IReceivePort<TNamingConvention>> receivePortConfigurator) : this()
		{
			receivePortConfigurator(this);
			((ISupportValidation) this).Validate();
		}

		#region IProvideSourceFileInformation Members

		int IProvideSourceFileInformation.Line
		{
			get { return _sourceFileInformationProvider.Line; }
		}

		int IProvideSourceFileInformation.Column
		{
			get { return _sourceFileInformationProvider.Column; }
		}

		string IProvideSourceFileInformation.Name
		{
			get { return _sourceFileInformationProvider.Name; }
		}

		#endregion

		#region IReceivePort<TNamingConvention> Members

		public IApplicationBinding<TNamingConvention> ApplicationBinding { get; internal set; }

		public bool IsTwoWay
		{
			get { return ComputeIsTwoWay(); }
		}

		public string Description { get; set; }

		public TNamingConvention Name { get; set; }

		public IReceiveLocationCollection<TNamingConvention> ReceiveLocations
		{
			get { return _receiveLocations; }
		}

		#endregion

		#region ISupportEnvironmentSensitivity Members

		void ISupportEnvironmentOverride.ApplyEnvironmentOverrides(string environment)
		{
			if (!environment.IsNullOrEmpty() && ((ISupportEnvironmentDeploymentPredicate) this).IsDeployableForEnvironment(environment))
			{
				ApplyEnvironmentOverrides(environment);
			}
		}

		bool ISupportEnvironmentDeploymentPredicate.IsDeployableForEnvironment(string environment)
		{
			return environment.IsNullOrEmpty() || IsDeployableForEnvironment(environment);
		}

		#endregion

		#region ISupportNamingConvention Members

		string ISupportNamingConvention.Name
		{
			get { return NamingConventionThunk.ComputeReceivePortName(this); }
		}

		#endregion

		#region ISupportValidation Members

		void ISupportValidation.Validate()
		{
			if (Name == null) throw new BindingException("Receive Port's Name is not defined.", this);
			if (!_receiveLocations.Any()) throw new BindingException("Receive Port has no Receive Locations.", this);
			ComputeIsTwoWay();
		}

		#endregion

		#region IVisitable<IApplicationBindingVisitor> Members

		void IVisitable<IApplicationBindingVisitor>.Accept(IApplicationBindingVisitor visitor)
		{
			visitor.VisitReceivePort(this);
			((IVisitable<IApplicationBindingVisitor>) _receiveLocations).Accept(visitor);
		}

		#endregion

		protected virtual void ApplyEnvironmentOverrides(string environment) { }

		protected virtual bool IsDeployableForEnvironment(string environment)
		{
			return true;
		}

		private bool ComputeIsTwoWay()
		{
			if (!_receiveLocations.Any()) throw new BindingException("Receive Port has no Receive Location.", this);
			var isOneWay = _receiveLocations.All(rl => rl.SendPipeline == null);
			var isTwoWay = _receiveLocations.All(rl => rl.SendPipeline != null);
			if (!isOneWay && !isTwoWay) throw new BindingException("Receive Port has a mix of one-way and two-way Receive Locations.", this);
			return isTwoWay;
		}

		private readonly ReceiveLocationCollection<TNamingConvention> _receiveLocations;
		private readonly SourceFileInformationProvider _sourceFileInformationProvider;
	}
}
