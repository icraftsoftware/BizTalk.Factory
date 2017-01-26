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

using System;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Install;
using Be.Stateless.BizTalk.Orchestrations.Dummy;
using Be.Stateless.BizTalk.Pipelines;
using Microsoft.BizTalk.Deployment.Binding;
using Microsoft.BizTalk.ExplorerOM;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	[TestFixture]
	public class BindingInfoBuilderVisitorFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			BindingGenerationContext.TargetEnvironment = "ANYTHING";
		}

		[TearDown]
		public void TearDown()
		{
			BindingGenerationContext.TargetEnvironment = null;
		}

		#endregion

		[Test]
		public void CreateBindingInfo()
		{
			var visitor = BindingInfoBuilderVisitor.Create();
			visitor.VisitApplicationBinding(new TestApplication());
			var binding = visitor.BindingInfo;

			Assert.That(binding.BindingParameters.BindingActions, Is.EqualTo(BindingParameters.BindingActionTypes.Bind));
			Assert.That(binding.BindingParameters.BindingItems, Is.EqualTo(BindingParameters.BindingItemTypes.All));
			Assert.That(binding.BindingParameters.BindingScope, Is.EqualTo(BindingParameters.BindingScopeType.Application));
			Assert.That(binding.BindingParameters.BindingSetState, Is.EqualTo(BindingParameters.BindingSetStateType.UseServiceState));
			Assert.That(binding.BindingParameters.BindingsSourceVersion.ToString(), Is.EqualTo(new BindingInfo().Version));
			Assert.That(binding.Description, Is.EqualTo("Some Useless Test Application"));
			Assert.That(binding.ModuleRefCollection.Count, Is.EqualTo(1));
			Assert.That(binding.ModuleRefCollection[0].Name, Is.EqualTo("[Application:TestApplication]"));
		}

		[Test]
		public void CreateModuleRef()
		{
			var visitor = BindingInfoBuilderVisitor.Create();
			// initialize BindingInfo
			visitor.VisitApplicationBinding(new TestApplication());

			var binding = visitor.CreateOrFindModuleRef(new ProcessOrchestrationBinding());

			Assert.That(binding.FullName, Is.EqualTo(typeof(Process).Assembly.FullName));
		}

		[Test]
		public void CreateReceiveLocationOneWay()
		{
			var dsl = new TestApplication.OneWayReceiveLocation();

			var visitor = BindingInfoBuilderVisitor.Create();
			var binding = visitor.CreateReceiveLocation(dsl);

			Assert.That(binding.Name, Is.EqualTo("OneWayReceiveLocation"));
			Assert.That(binding.Address, Is.EqualTo(@"c:\files\drops\*.xml"));
			Assert.That(binding.Description, Is.EqualTo("Some Useless One-Way Test Receive Location"));
			Assert.That(binding.Enable, Is.False);
			Assert.That(binding.EndDate, Is.EqualTo(dsl.Transport.Schedule.StopDate));
			Assert.That(binding.EndDateEnabled, Is.True);
			Assert.That(binding.FromTime, Is.EqualTo((DateTime) dsl.Transport.Schedule.ServiceWindow.StartTime));
			Assert.That(binding.ReceiveHandler.Name, Is.EqualTo("Receive Host Name"));
			Assert.That(binding.ReceivePipeline.Name, Is.EqualTo(typeof(PassThruReceive).FullName));
			Assert.That(binding.ReceivePipeline.FullyQualifiedName, Is.EqualTo(typeof(PassThruReceive).AssemblyQualifiedName));
			Assert.That(binding.ReceivePipeline.TrackingOption, Is.EqualTo(PipelineTrackingTypes.None));
			Assert.That(binding.ReceiveHandler.TransportType.Name, Is.EqualTo("Test Dummy"));
			Assert.That(binding.ReceivePipeline.Type, Is.EqualTo(PipelineRef.ReceivePipelineRef().Type));
			Assert.That(binding.ReceivePipelineData, Is.Not.Null.And.Not.Empty);
			Assert.That(binding.SendPipeline, Is.Null);
			Assert.That(binding.SendPipelineData, Is.Null);
			Assert.That(binding.ServiceWindowEnabled, Is.True);
			Assert.That(binding.StartDate, Is.EqualTo(dsl.Transport.Schedule.StartDate));
			Assert.That(binding.StartDateEnabled, Is.True);
			Assert.That(binding.ToTime, Is.EqualTo((DateTime) dsl.Transport.Schedule.ServiceWindow.StopTime));
		}

		[Test]
		public void CreateReceiveLocationTwoWay()
		{
			var visitor = BindingInfoBuilderVisitor.Create();
			var binding = visitor.CreateReceiveLocation(new TestApplication.TwoWayReceiveLocation());

			Assert.That(binding.Name, Is.EqualTo("TwoWayReceiveLocation"));
			Assert.That(binding.Address, Is.EqualTo(@"c:\files\drops\*.xml"));
			Assert.That(binding.Description, Is.EqualTo("Some Useless Two-Way Test Receive Location"));
			Assert.That(binding.Enable, Is.False);
			Assert.That(binding.EndDate, Is.EqualTo(Schedule.None.StopDate));
			Assert.That(binding.EndDateEnabled, Is.False);
			Assert.That(binding.FromTime, Is.EqualTo((DateTime) ServiceWindow.None.StartTime));
			Assert.That(binding.ReceiveHandler.Name, Is.EqualTo("Receive Host Name"));
			Assert.That(binding.ReceivePipeline.Name, Is.EqualTo(typeof(PassThruReceive).FullName));
			Assert.That(binding.ReceivePipeline.FullyQualifiedName, Is.EqualTo(typeof(PassThruReceive).AssemblyQualifiedName));
			Assert.That(binding.ReceivePipeline.TrackingOption, Is.EqualTo(PipelineTrackingTypes.None));
			Assert.That(binding.ReceiveHandler.TransportType.Name, Is.EqualTo("Test Dummy"));
			Assert.That(binding.ReceivePipeline.Type, Is.EqualTo(PipelineRef.ReceivePipelineRef().Type));
			Assert.That(binding.ReceivePipelineData, Is.Empty);
			Assert.That(binding.SendPipeline.Name, Is.EqualTo(typeof(PassThruTransmit).FullName));
			Assert.That(binding.SendPipeline.FullyQualifiedName, Is.EqualTo(typeof(PassThruTransmit).AssemblyQualifiedName));
			Assert.That(binding.SendPipeline.TrackingOption, Is.EqualTo(PipelineTrackingTypes.None));
			Assert.That(binding.SendPipeline.Type, Is.EqualTo(PipelineRef.TransmitPipelineRef().Type));
			Assert.That(binding.SendPipelineData, Is.Not.Null.And.Not.Empty);
			Assert.That(binding.ServiceWindowEnabled, Is.False);
			Assert.That(binding.StartDate, Is.EqualTo(Schedule.None.StartDate));
			Assert.That(binding.StartDateEnabled, Is.False);
			Assert.That(binding.ToTime, Is.EqualTo((DateTime) ServiceWindow.None.StopTime));
		}

		[Test]
		public void CreateReceivePortOneWay()
		{
			var visitor = BindingInfoBuilderVisitor.Create();
			// initialize BindingInfoBuilderVisitor.ApplicationName
			visitor.VisitApplicationBinding(new TestApplication());
			var binding = visitor.CreateReceivePort(new TestApplication.OneWayReceivePort());

			Assert.That(binding.ApplicationName, Is.EqualTo("TestApplication"));
			Assert.That(binding.Description, Is.EqualTo("Some Useless One-Way Test Receive Port"));
			Assert.That(binding.IsTwoWay, Is.False);
			Assert.That(binding.Name, Is.EqualTo("OneWayReceivePort"));
			Assert.That(binding.ReceiveLocations.Count, Is.EqualTo(0));
		}

		[Test]
		public void CreateReceivePortTwoWay()
		{
			var visitor = BindingInfoBuilderVisitor.Create();
			// initialize BindingInfoBuilderVisitor.ApplicationName
			visitor.VisitApplicationBinding(new TestApplication());
			var binding = visitor.CreateReceivePort(new TestApplication.TwoWayReceivePort());

			Assert.That(binding.ApplicationName, Is.EqualTo("TestApplication"));
			Assert.That(binding.Description, Is.EqualTo("Some Useless Two-Way Test Receive Port"));
			Assert.That(binding.IsTwoWay, Is.True);
			Assert.That(binding.Name, Is.EqualTo("TwoWayReceivePort"));
			Assert.That(binding.ReceiveLocations.Count, Is.EqualTo(0));
		}

		[Test]
		public void CreateSendPortOneWay()
		{
			var dsl = new TestApplication.OneWaySendPort();

			var visitor = BindingInfoBuilderVisitor.Create();
			// initialize BindingInfoBuilderVisitor.ApplicationName
			visitor.VisitApplicationBinding(new TestApplication());
			var binding = visitor.CreateSendPort(dsl);

			Assert.That(binding.ApplicationName, Is.EqualTo("TestApplication"));
			Assert.That(binding.Description, Is.EqualTo("Some Useless One-Way Test Send Port"));
			Assert.That(binding.Filter, Is.Not.Null.And.Not.Empty);
			Assert.That(binding.IsDynamic, Is.False);
			Assert.That(binding.IsStatic, Is.True);
			Assert.That(binding.IsTwoWay, Is.False);
			Assert.That(binding.Name, Is.EqualTo("OneWaySendPort"));
			Assert.That(binding.OrderedDelivery, Is.True);
			Assert.That(binding.PrimaryTransport.Address, Is.EqualTo(@"c:\files\drops\*.xml"));
			Assert.That(binding.PrimaryTransport.FromTime, Is.EqualTo((DateTime) dsl.Transport.ServiceWindow.StartTime));
			Assert.That(binding.PrimaryTransport.OrderedDelivery, Is.True);
			Assert.That(binding.PrimaryTransport.Primary, Is.True);
			Assert.That(binding.PrimaryTransport.RetryCount, Is.EqualTo(dsl.Transport.RetryPolicy.Count));
			Assert.That(binding.PrimaryTransport.RetryInterval, Is.EqualTo(dsl.Transport.RetryPolicy.Interval.TotalMinutes));
			Assert.That(binding.PrimaryTransport.SendHandler.Name, Is.EqualTo("Send Host Name"));
			Assert.That(binding.PrimaryTransport.SendHandler.TransportType.Name, Is.EqualTo("Test Dummy"));
			Assert.That(binding.PrimaryTransport.ServiceWindowEnabled, Is.True);
			Assert.That(binding.PrimaryTransport.ToTime, Is.EqualTo((DateTime) dsl.Transport.ServiceWindow.StopTime));
			Assert.That(binding.Priority, Is.EqualTo(1));
			Assert.That(binding.ReceivePipeline, Is.Null);
			Assert.That(binding.ReceivePipelineData, Is.Null);
			Assert.That(binding.SecondaryTransport, Is.Null);
			Assert.That(binding.SendPipelineData, Is.Not.Null.And.Not.Empty);
			Assert.That(binding.StopSendingOnFailure, Is.True);
			Assert.That(binding.TransmitPipeline.Name, Is.EqualTo(typeof(PassThruTransmit).FullName));
			Assert.That(binding.TransmitPipeline.FullyQualifiedName, Is.EqualTo(typeof(PassThruTransmit).AssemblyQualifiedName));
			Assert.That(binding.TransmitPipeline.TrackingOption, Is.EqualTo(PipelineTrackingTypes.None));
			Assert.That(binding.TransmitPipeline.Type, Is.EqualTo(PipelineRef.TransmitPipelineRef().Type));
		}

		[Test]
		public void CreateSendPortTwoWay()
		{
			var visitor = BindingInfoBuilderVisitor.Create();
			// initialize BindingInfoBuilderVisitor.ApplicationName
			visitor.VisitApplicationBinding(new TestApplication());
			var binding = visitor.CreateSendPort(new TestApplication.TwoWaySendPort());

			Assert.That(binding.ApplicationName, Is.EqualTo("TestApplication"));
			Assert.That(binding.Description, Is.EqualTo("Some Useless Two-Way Test Send Port"));
			Assert.That(binding.Filter, Is.Null);
			Assert.That(binding.IsDynamic, Is.False);
			Assert.That(binding.IsStatic, Is.True);
			Assert.That(binding.IsTwoWay, Is.True);
			Assert.That(binding.Name, Is.EqualTo("TwoWaySendPort"));
			Assert.That(binding.PrimaryTransport.FromTime, Is.EqualTo((DateTime) ServiceWindow.None.StartTime));
			Assert.That(binding.PrimaryTransport.Primary, Is.True);
			Assert.That(binding.PrimaryTransport.RetryCount, Is.EqualTo(RetryPolicy.Default.Count));
			Assert.That(binding.PrimaryTransport.RetryInterval, Is.EqualTo(RetryPolicy.Default.Interval.TotalMinutes));
			Assert.That(binding.PrimaryTransport.SendHandler.Name, Is.EqualTo("Send Host Name"));
			Assert.That(binding.PrimaryTransport.SendHandler.TransportType.Name, Is.EqualTo("Test Dummy"));
			Assert.That(binding.PrimaryTransport.ServiceWindowEnabled, Is.False);
			Assert.That(binding.PrimaryTransport.ToTime, Is.EqualTo((DateTime) ServiceWindow.None.StopTime));
			Assert.That(binding.ReceivePipeline.Name, Is.EqualTo(typeof(PassThruReceive).FullName));
			Assert.That(binding.ReceivePipeline.FullyQualifiedName, Is.EqualTo(typeof(PassThruReceive).AssemblyQualifiedName));
			Assert.That(binding.ReceivePipeline.TrackingOption, Is.EqualTo(PipelineTrackingTypes.None));
			Assert.That(binding.ReceivePipeline.Type, Is.EqualTo(PipelineRef.ReceivePipelineRef().Type));
			Assert.That(binding.ReceivePipelineData, Is.Not.Null.And.Not.Empty);
			Assert.That(binding.SecondaryTransport, Is.Null);
			Assert.That(binding.SendPipelineData, Is.Empty);
			Assert.That(binding.TransmitPipeline.Name, Is.EqualTo(typeof(PassThruTransmit).FullName));
			Assert.That(binding.TransmitPipeline.FullyQualifiedName, Is.EqualTo(typeof(PassThruTransmit).AssemblyQualifiedName));
			Assert.That(binding.TransmitPipeline.TrackingOption, Is.EqualTo(PipelineTrackingTypes.None));
			Assert.That(binding.TransmitPipeline.Type, Is.EqualTo(PipelineRef.TransmitPipelineRef().Type));
		}

		[Test]
		public void CreateServiceRef()
		{
			var visitor = BindingInfoBuilderVisitor.Create();

			var orchestrationBinding = new ProcessOrchestrationBinding {
				Description = "Some Useless Orchestration.",
				Host = "Processing Host Name",
				ReceivePort = new TestApplication.OneWayReceivePort(),
				RequestResponsePort = new TestApplication.TwoWayReceivePort(),
				SendPort = new TestApplication.OneWaySendPort(),
				SolicitResponsePort = new TestApplication.TwoWaySendPort()
			};
			var binding = visitor.CreateServiceRef(orchestrationBinding);

			Assert.That(binding.Description, Is.EqualTo("Some Useless Orchestration."));
			Assert.That(binding.Host.Name, Is.EqualTo("Processing Host Name"));
			Assert.That(binding.Host.Trusted, Is.False);
			Assert.That(binding.Host.Type, Is.EqualTo((int) HostType.Invalid));
			Assert.That(binding.Name, Is.EqualTo(typeof(Process).FullName));
			Assert.That(binding.State, Is.EqualTo(ServiceRef.ServiceRefState.Enlisted));
			Assert.That(binding.TrackingOption, Is.EqualTo(OrchestrationTrackingTypes.None));
			Assert.That(binding.Ports.Count, Is.EqualTo(4));

			Assert.That(binding.Ports[0].Modifier, Is.EqualTo((int) PortModifier.Import));
			Assert.That(binding.Ports[0].Name, Is.EqualTo("SendPort"));
			Assert.That(binding.Ports[0].ReceivePortRef, Is.Null);
			Assert.That(binding.Ports[0].SendPortRef.Name, Is.EqualTo(((ISupportNamingConvention) new TestApplication.OneWaySendPort()).Name));
			Assert.That(binding.Ports[1].Modifier, Is.EqualTo((int) PortModifier.Export));

			Assert.That(binding.Ports[1].Name, Is.EqualTo("ReceivePort"));
			Assert.That(binding.Ports[1].ReceivePortRef.Name, Is.EqualTo(((ISupportNamingConvention) new TestApplication.OneWayReceivePort()).Name));
			Assert.That(binding.Ports[1].SendPortRef, Is.Null);

			Assert.That(binding.Ports[2].Modifier, Is.EqualTo((int) PortModifier.Export));
			Assert.That(binding.Ports[2].Name, Is.EqualTo("RequestResponsePort"));
			Assert.That(binding.Ports[2].ReceivePortRef.Name, Is.EqualTo(((ISupportNamingConvention) new TestApplication.TwoWayReceivePort()).Name));
			Assert.That(binding.Ports[2].SendPortRef, Is.Null);

			Assert.That(binding.Ports[3].Modifier, Is.EqualTo((int) PortModifier.Import));
			Assert.That(binding.Ports[3].Name, Is.EqualTo("SolicitResponsePort"));
			Assert.That(binding.Ports[3].ReceivePortRef, Is.Null);
			Assert.That(binding.Ports[3].SendPortRef.Name, Is.EqualTo(((ISupportNamingConvention) new TestApplication.TwoWaySendPort()).Name));
		}

		[Test]
		public void VisitReferencedApplicationBindingDoesNotPropagateVisitor()
		{
			var applicationBindingMock = new Mock<IVisitable<IApplicationBindingVisitor>>();

			var visitor = BindingInfoBuilderVisitor.Create();
			visitor.VisitReferencedApplicationBinding(applicationBindingMock.Object);

			applicationBindingMock.Verify(a => a.Accept(visitor), Times.Never);
		}
	}
}
