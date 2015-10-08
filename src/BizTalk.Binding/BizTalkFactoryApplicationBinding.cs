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

using System.Transactions;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Subscription;
using Be.Stateless.BizTalk.EnvironmentSettings;
using Be.Stateless.BizTalk.Pipelines;
using Be.Stateless.BizTalk.Transforms.ToSql.Procedures.Batch;
using Microsoft.Adapters.Sql;
using NamingConvention = Be.Stateless.BizTalk.Dsl.Binding.Convention.BizTalkFactory.NamingConvention<string, string>;

namespace Be.Stateless.BizTalk
{
	public class BizTalkFactoryApplicationBinding : Dsl.Binding.Convention.BizTalkFactory.ApplicationBinding<NamingConvention>
	{
		public BizTalkFactoryApplicationBinding()
		{
			Name = ApplicationName.Is("BizTalk.Factory");
			Description = "Library to speed up the development of BizTalk Server applications.";
			SendPorts.Add(
				SendPort(
					sp => {
						sp.Name = SendPortName.Towards("Batch").About("AddPart").FormattedAs.Xml;
						// TODO ApplicationBinding should not be visible on intellisense sp.ApplicationBinding
						sp.SendPipeline = new SendPipeline<XmlTransmit>(
							pipeline => {
								pipeline
									.Encoder<ActivityTrackerComponent>(pc => { pc.TrackingResolutionPolicy = Policy<Policies.Send.Batch.AggregateProcessResolver>.Name; })
									.Encoder<XsltRunnerComponent>(pc => { pc.Map = typeof(AnyToAddPart); })
									.Encoder<XmlTranslatorComponent>(pc => { pc.Enabled = false; });
							});
						sp.Transport.Adapter = new WcfSqlAdapter.Outbound(
							a => {
								a.Address = new SqlAdapterConnectionUri { InitialCatalog = "BizTalkFactoryTransientStateDb", Server = "localhost" };
								a.IsolationLevel = IsolationLevel.ReadCommitted;
								a.StaticAction = "TypedProcedure/dbo/usp_batch_AddPart";
							});
						sp.Transport.Host = CommonSettings.TransmitHost;
						sp.Transport.RetryPolicy = Dsl.Binding.Convention.BizTalkFactory.RetryPolicy.ShortRunning;
						sp.Filter = new Filter(() => BizTalkFactoryProperties.EnvelopeSpecName != null);
					}));
		}
	}
}
