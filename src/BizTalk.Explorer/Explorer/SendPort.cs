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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.Linq.Extensions;
using BizMock;
using Microsoft.BizTalk.B2B.PartnerManagement;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Explorer
{
	public class SendPort
	{
		/// <summary>
		/// Bridges <see cref="SendPort"/> and <see cref="SendPortArtifact"/> so as not to break <see cref="Expect"/> API.
		/// </summary>
		/// <returns>A <see cref="SendPortArtifact"/> equivalent.</returns>
		public static implicit operator SendPortArtifact(SendPort sendPort)
		{
			return new SendPortArtifact(sendPort.Name, sendPort.Timeout);
		}

		public SendPort(Application application, string name, int timeout)
			: this(application, name, null, null, timeout) { }

		public SendPort(Application application, string name, string pipelineName, int timeout)
			: this(application, name, pipelineName, null, timeout) { }

		public SendPort(Application application, string name, string pipelineName, string pipelineData, int timeout)
		{
			_application = application;
			_name = name;
			_pipelineName = pipelineName;
			_pipelineData = pipelineData;
			_timeout = timeout;
		}

		private Microsoft.BizTalk.ExplorerOM.SendPort BizTalkSendPort
		{
			get
			{
				var btsApplication = (Microsoft.BizTalk.ExplorerOM.Application) _application;
				return btsApplication.SendPorts.Cast<Microsoft.BizTalk.ExplorerOM.SendPort>().Single(sp => sp.Name.Equals(_name));
			}
		}

		public string Name
		{
			get { return _name; }
		}

		public int Timeout
		{
			get { return _timeout; }
		}

		public void Delete()
		{
			BizTalkExplorerHelper.RemoveSendPort(_application, _name);
		}

		public void Deploy()
		{
			BizTalkExplorerHelper.CreateNewSendPort(_application, _name, _pipelineName, _pipelineData, null, null, _timeout, false, false);
		}

		public SendPort SubscribesTo<T, TR>(MessageContextProperty<T, TR> property) where T : MessageContextPropertyBase, new()
		{
			BizTalkSendPort.Filter = BuildPropertySubscriptionFilter(property.Type.FullName);
			return this;
		}

		public SendPort SubscribesTo<T, TR>(MessageContextProperty<T, TR> property, TR value) where T : MessageContextPropertyBase, new()
		{
			BizTalkSendPort.Filter = BuildPropertySubscriptionFilter(property.Type.FullName, value.ToString());
			return this;
		}

		public SendPort SubscribesTo(params ReceiveLocation[] receiveLocations)
		{
			BizTalkSendPort.Filter = BuildReceivePortSubscriptionFilter(receiveLocations.Select(rl => rl.Name));
			return this;
		}

		private string BuildPropertySubscriptionFilter(string propertyName)
		{
			var statement = new FilterStatement(propertyName, FilterOperator.Exists, null);
			var @group = new FilterGroup();
			@group.Statements.Add(statement);
			return BuildFilter(@group);
		}

		private string BuildPropertySubscriptionFilter(string propertyName, string value)
		{
			var statement = new FilterStatement(propertyName, FilterOperator.Exists, value);
			var @group = new FilterGroup();
			@group.Statements.Add(statement);
			return BuildFilter(@group);
		}

		private string BuildReceivePortSubscriptionFilter(IEnumerable<string> portNames)
		{
			return BuildFilter(
				portNames.Select(
					n => {
						var @group = new FilterGroup();
						@group.Statements.Add(new FilterStatement(BtsProperties.ReceivePortName.Type.FullName, FilterOperator.Equals, n));
						return @group;
					}));
		}

		private string BuildFilter(params FilterGroup[] @groups)
		{
			return BuildFilter(@groups.AsEnumerable());
		}

		private string BuildFilter(IEnumerable<FilterGroup> groups)
		{
			var predicate = new FilterPredicate();
			groups.Each(g => predicate.Groups.Add(g));

			var writer = new StringWriter();
			var serializer = new XmlSerializer(typeof(FilterPredicate));
			serializer.Serialize(writer, predicate);
			return writer.ToString();
		}

		private readonly Application _application;
		private readonly string _name;
		private readonly string _pipelineData;
		private readonly string _pipelineName;
		private readonly int _timeout = 90;
	}
}
