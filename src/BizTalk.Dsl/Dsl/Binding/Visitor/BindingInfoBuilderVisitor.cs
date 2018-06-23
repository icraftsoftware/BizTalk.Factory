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
using System.Globalization;
using System.Linq;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Pipeline;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Deployment.Binding;
using Microsoft.BizTalk.ExplorerOM;
using BtsReceiveLocation = Microsoft.BizTalk.Deployment.Binding.ReceiveLocation;
using BtsReceivePort = Microsoft.BizTalk.Deployment.Binding.ReceivePort;
using BtsSendPort = Microsoft.BizTalk.Deployment.Binding.SendPort;
using TransportInfo = Microsoft.BizTalk.Deployment.Binding.TransportInfo;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	/// <summary>
	/// <see cref="IApplicationBindingVisitor"/> implementation that generates BizTalk Server bindings file.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See <see href="https://msdn.microsoft.com/en-us/library/microsoft.biztalk.deployment.binding.aspx">Microsoft.BizTalk.Deployment.Binding
	/// Namespace</see>
	/// </para>
	/// </remarks>
	/// <seealso href="https://msdn.microsoft.com/en-us/library/microsoft.biztalk.deployment.binding.aspx"/>
	// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
	public class BindingInfoBuilderVisitor : IApplicationBindingVisitor
	{
		public static BindingInfoBuilderVisitor Create()
		{
			return new BindingInfoBuilderVisitor();
		}

		private BindingInfoBuilderVisitor() { }

		#region IApplicationBindingVisitor Members

		public void VisitReferencedApplicationBinding(IVisitable<IApplicationBindingVisitor> applicationBinding)
		{
			// do not generate BindingInfo for referenced applications
		}

		public void VisitApplicationBinding<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding)
			where TNamingConvention : class
		{
			// ensure referenced application is settled for environment
			var applicationBindingEnvironmentSettlerVisitor = new ApplicationBindingEnvironmentSettlerVisitor();
			((IVisitable<IApplicationBindingVisitor>) applicationBinding).Accept(applicationBindingEnvironmentSettlerVisitor);

			ApplicationName = ((ISupportNamingConvention) applicationBinding).Name;
			BindingInfo = CreateBindingInfo(applicationBinding);
		}

		public void VisitOrchestration(IOrchestrationBinding orchestrationBinding)
		{
			var moduleRef = CreateOrFindModuleRef(orchestrationBinding);
			// a ModuleRef just created has no ServiceRef in its Services collection yet
			if (moduleRef.Services.Count == 0) BindingInfo.ModuleRefCollection.Add(moduleRef);
			var serviceRef = CreateServiceRef(orchestrationBinding);
			moduleRef.Services.Add(serviceRef);
		}

		public void VisitReceivePort<TNamingConvention>(IReceivePort<TNamingConvention> receivePort)
			where TNamingConvention : class
		{
			_lastVisitedReceivePort = CreateReceivePort(receivePort);
			if (BindingInfo.ReceivePortCollection.Find(_lastVisitedReceivePort.Name) != null)
				throw new InvalidOperationException(
					string.Format(
						"Duplicate receive port name: '{0}'.",
						_lastVisitedReceivePort.Name));
			BindingInfo.ReceivePortCollection.Add(_lastVisitedReceivePort);
		}

		public void VisitReceiveLocation<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation)
			where TNamingConvention : class
		{
			var visitedReceiveLocation = CreateReceiveLocation(receiveLocation);
			if (_lastVisitedReceivePort.ReceiveLocations.Cast<BtsReceiveLocation>().Any(rl => rl.Name == visitedReceiveLocation.Name))
				throw new InvalidOperationException(
					string.Format(
						"Duplicate receive location name: '{0}'.",
						visitedReceiveLocation.Name));
			_lastVisitedReceivePort.ReceiveLocations.Add(visitedReceiveLocation);
		}

		public void VisitSendPort<TNamingConvention>(ISendPort<TNamingConvention> sendPort)
			where TNamingConvention : class
		{
			var visitedSendPort = CreateSendPort(sendPort);
			if (BindingInfo.SendPortCollection.Find(visitedSendPort.Name) != null)
				throw new InvalidOperationException(
					string.Format(
						"Duplicate send port name: '{0}'.",
						visitedSendPort.Name));
			BindingInfo.SendPortCollection.Add(visitedSendPort);
		}

		#endregion

		public string ApplicationName { get; private set; }

		public BindingInfo BindingInfo { get; private set; }

		protected virtual BindingInfo CreateBindingInfo<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding)
			where TNamingConvention : class
		{
			((ISupportValidation) applicationBinding).Validate();

			var bi = new BindingInfo();
			bi.BindingParameters = new BindingParameters(new Version(bi.Version)) {
				BindingActions = BindingParameters.BindingActionTypes.Bind,
				BindingItems = BindingParameters.BindingItemTypes.All,
				BindingScope = BindingParameters.BindingScopeType.Application,
				BindingSetState = BindingParameters.BindingSetStateType.UseServiceState
			};
			bi.Description = applicationBinding.Description;
			bi.ModuleRefCollection = new ModuleRefCollection {
				new ModuleRef(string.Format("[Application:{0}]", ApplicationName), string.Empty, string.Empty, string.Empty)
				// TODO ref schemas, transforms, and other artifacts
			};
			bi.Timestamp = applicationBinding.Timestamp;
			return bi;
		}

		protected internal virtual ModuleRef CreateOrFindModuleRef(IOrchestrationBinding orchestrationBinding)
		{
			var serviceAssemblyName = orchestrationBinding.Type.Assembly.GetName();
			var name = serviceAssemblyName.Name;
			var version = serviceAssemblyName.Version.ToString();
			// see BizTalkFactory.Management.Automation.BtsCatalog.ExportBinding, BizTalkFactory.Management.Automation
			var culture = serviceAssemblyName.CultureInfo == null || serviceAssemblyName.CultureInfo.Name.IsNullOrEmpty()
				? "neutral"
				: serviceAssemblyName.CultureInfo.Name;
			var publicKeyTokenBytes = serviceAssemblyName.GetPublicKeyToken();
			// see BizTalkFactory.Management.Automation.BtsCatalog.ExportBinding, BizTalkFactory.Management.Automation
			var publicKeyToken = publicKeyTokenBytes == null || publicKeyTokenBytes.Length == 0
				? null
				: publicKeyTokenBytes.Aggregate(string.Empty, (k, token) => k + token.ToString("x2", CultureInfo.InvariantCulture));
			var module = BindingInfo.ModuleRefCollection.Find(name, version, culture, publicKeyToken);
			return module ?? new ModuleRef(name, version, culture, publicKeyToken);
		}

		protected internal virtual ServiceRef CreateServiceRef(IOrchestrationBinding orchestrationBinding)
		{
			// see https://msdn.microsoft.com/en-us/library/microsoft.biztalk.deployment.binding.serviceref.aspx
			var serviceRef = new ServiceRef {
				Description = orchestrationBinding.Description,
				// TODO EndpointInfo = 
				Host = new HostRef {
					Name = orchestrationBinding.Host,
					// TODO NTGroupName = "",
					// TODO Trusted = false,
					// TODO Type = (int) HostType.InProcess
				},
				Name = orchestrationBinding.Type.FullName,
				// TODO allow to change State
				State = ServiceRef.ServiceRefState.Enlisted,
				// TODO allow to change TackingOption
				TrackingOption = OrchestrationTrackingTypes.None
			};
			// ensure service ref port collection is initialized even if there are only direct ports
			var serviceRefPorts = serviceRef.Ports;
			foreach (var portBinding in orchestrationBinding.PortBindings)
			{
				serviceRefPorts.Add(CreateServicePortRef(portBinding));
			}
			// TODO Roles = 
			return serviceRef;
		}

		protected virtual ServicePortRef CreateServicePortRef(IOrchestrationPortBinding portBinding)
		{
			// see https://msdn.microsoft.com/en-us/library/microsoft.biztalk.deployment.binding.serviceportref.aspx
			if (portBinding.IsInbound)
				return new ServicePortRef {
					// see Microsoft.BizTalk.Deployment.Assembly.BtsOrchestrationPort.GetBindingOption(IBizTalkPort port)
					// see Microsoft.BizTalk.OrchestrationDesigner.PortBinding, Microsoft.BizTalk.OrchestrationDesigner
					// where 0=Physical, 1=Logical, 2=Direct, 3=Dynamic
					// however it never seems to be set to an other value than 1 in binding exports
					//	TODO support BindingOption other values
					BindingOption = 1,
					// see Microsoft.BizTalk.ExplorerOM.PortModifier (Import Indicates an Outbound port of an orchestration,
					// Export Indicates an Inbound port of an orchestration)
					Modifier = (int) PortModifier.Export,
					Name = portBinding.LogicalPortName,
					ReceivePortRef = new ReceivePortRef { Name = portBinding.ActualPortName }
				};

			return new ServicePortRef {
				Modifier = (int) PortModifier.Import,
				Name = portBinding.LogicalPortName,
				SendPortRef = new SendPortRef { Name = portBinding.ActualPortName }
			};
		}

		protected internal virtual BtsReceivePort CreateReceivePort<TNamingConvention>(IReceivePort<TNamingConvention> receivePort)
			where TNamingConvention : class
		{
			((ISupportValidation) receivePort).Validate();

			var port = new BtsReceivePort {
				ApplicationName = ApplicationName,
				// TODO Authentication =
				// TODO BindingOption =
				Description = receivePort.Description,
				IsTwoWay = receivePort.IsTwoWay,
				Name = ((ISupportNamingConvention) receivePort).Name,
				// TODO OutboundTransforms =
				ReceiveLocations = new Microsoft.BizTalk.Deployment.Binding.ReceiveLocationCollection()
				// TODO RouteFailedMessage =
				// TODO SendPipelineData =
				// TODO Tracking =
				// TODO Transforms =
				// TODO TransmitPipeline =
			};
			return port;
		}

		protected internal virtual BtsReceiveLocation CreateReceiveLocation<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation)
			where TNamingConvention : class
		{
			((ISupportValidation) receiveLocation).Validate();

			var location = new BtsReceiveLocation {
				Name = ((ISupportNamingConvention) receiveLocation).Name,
				Address = receiveLocation.Transport.Adapter.Address,
				Description = receiveLocation.Description,
				Enable = false, // receiveLocation.Enabled is the responsibility of BizTalkServiceConfiguratorVisitor
				EndDate = receiveLocation.Transport.Schedule.StopDate,
				EndDateEnabled = receiveLocation.Transport.Schedule.StopDateEnabled,
				// TODO EncryptionCert = 
				FromTime = receiveLocation.Transport.Schedule.ServiceWindow.StartTime,
				// TODO Primary = 
				PublicAddress = receiveLocation.Transport.Adapter.PublicAddress,
				ReceiveHandler = new ReceiveHandlerRef {
					// TODO HostTrusted = ,
					Name = receiveLocation.Transport.Host,
					TransportType = receiveLocation.Transport.Adapter.ProtocolType
				},
				ReceivePipeline = CreateReceivePipelineRef(receiveLocation.ReceivePipeline),
				ReceivePipelineData = ((IPipelineSerializerFactory) receiveLocation.ReceivePipeline).GetPipelineBindingSerializer().Serialize(),
				ServiceWindowEnabled = receiveLocation.Transport.Schedule.ServiceWindow.Enabled,
				StartDate = receiveLocation.Transport.Schedule.StartDate,
				StartDateEnabled = receiveLocation.Transport.Schedule.StartDateEnabled,
				ToTime = receiveLocation.Transport.Schedule.ServiceWindow.StopTime,
				TransportType = receiveLocation.Transport.Adapter.ProtocolType,
				TransportTypeData = ((IAdapterBindingSerializerFactory) receiveLocation.Transport.Adapter).GetAdapterBindingSerializer().Serialize()
			};
			if (receiveLocation.SendPipeline != null)
			{
				location.SendPipeline = CreateSendPipelineRef(receiveLocation.SendPipeline);
				location.SendPipelineData = ((IPipelineSerializerFactory) receiveLocation.SendPipeline).GetPipelineBindingSerializer().Serialize();
			}
			return location;
		}

		protected internal virtual BtsSendPort CreateSendPort<TNamingConvention>(ISendPort<TNamingConvention> sendPort)
			where TNamingConvention : class
		{
			((ISupportValidation) sendPort).Validate();

			var port = new BtsSendPort {
				ApplicationName = ApplicationName,
				// TODO BindingOption =
				Description = sendPort.Description,
				// TODO EncryptionCert =
				Filter = sendPort.Filter.IfNotNull(f => f.ToString()),
				// TODO InboundTransforms =
				// TODO IsDynamic =
				IsDynamic = false,
				Name = ((ISupportNamingConvention) sendPort).Name,
				OrderedDelivery = sendPort.OrderedDelivery,
				PrimaryTransport = CreateTransportInfo(sendPort.Transport, true, sendPort.OrderedDelivery),
				Priority = (int) sendPort.Priority,
				// TODO RouteFailedMessage =
				SecondaryTransport = sendPort.BackupTransport.Adapter is SendPortTransport.UnknownOutboundAdapter
					? null
					: CreateTransportInfo(sendPort.BackupTransport, false, false),
				SendPipelineData = ((IPipelineSerializerFactory) sendPort.SendPipeline).GetPipelineBindingSerializer().Serialize(),
				// sendPort.Status is the responsibility of BizTalkServiceConfiguratorVisitor
				StopSendingOnFailure = sendPort.StopSendingOnOrderedDeliveryFailure,
				// TODO Tracking =
				// TODO Transforms =
				TransmitPipeline = CreateSendPipelineRef(sendPort.SendPipeline)
			};
			if (sendPort.IsTwoWay)
			{
				port.IsTwoWay = true;
				port.ReceivePipeline = CreateReceivePipelineRef(sendPort.ReceivePipeline);
				port.ReceivePipelineData = ((IPipelineSerializerFactory) sendPort.ReceivePipeline).GetPipelineBindingSerializer().Serialize();
			}
			return port;
		}

		protected virtual TransportInfo CreateTransportInfo(SendPortTransport transport, bool primary, bool orderedDelivery)
		{
			var transportInfo = new TransportInfo {
				Address = transport.Adapter.Address,
				// TODO DeliveryNotification = 0,
				FromTime = transport.ServiceWindow.StartTime,
				OrderedDelivery = orderedDelivery,
				Primary = primary,
				RetryCount = transport.RetryPolicy.Count,
				RetryInterval = (int) transport.RetryPolicy.Interval.TotalMinutes,
				SendHandler = new SendHandlerRef {
					// TODO HostTrusted = ,
					Name = transport.Host,
					TransportType = transport.Adapter.ProtocolType
				},
				ServiceWindowEnabled = transport.ServiceWindow.Enabled,
				ToTime = transport.ServiceWindow.StopTime,
				TransportType = transport.Adapter.ProtocolType,
				TransportTypeData = ((IAdapterBindingSerializerFactory) transport.Adapter).GetAdapterBindingSerializer().Serialize()
			};
			return transportInfo;
		}

		protected virtual PipelineRef CreateReceivePipelineRef(ReceivePipeline receivePipeline)
		{
			var rp = PipelineRef.ReceivePipelineRef();
			receivePipeline.Description.IfNotNullOrEmpty(d => rp.Description = d);
			rp.Name = ((ITypeDescriptor) receivePipeline).FullName;
			rp.FullyQualifiedName = ((ITypeDescriptor) receivePipeline).AssemblyQualifiedName;
			// TODO allow to change TackingOption
			rp.TrackingOption = PipelineTrackingTypes.None;
			return rp;
		}

		protected virtual PipelineRef CreateSendPipelineRef(SendPipeline sendPipeline)
		{
			var tp = PipelineRef.TransmitPipelineRef();
			sendPipeline.Description.IfNotNullOrEmpty(d => tp.Description = d);
			tp.Name = ((ITypeDescriptor) sendPipeline).FullName;
			tp.FullyQualifiedName = ((ITypeDescriptor) sendPipeline).AssemblyQualifiedName;
			// TODO allow to change TackingOption
			tp.TrackingOption = PipelineTrackingTypes.None;
			return tp;
		}

		private BtsReceivePort _lastVisitedReceivePort;
	}
}
