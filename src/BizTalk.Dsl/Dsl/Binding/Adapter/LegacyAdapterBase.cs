#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Microsoft.BizTalk.Adapter.Common;
using Microsoft.BizTalk.Adapter.Framework;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class LegacyAdapterBase : AdapterBase
	{
		#region PollingUnitOfMeasure Enum

		public enum PollingUnitOfMeasure
		{
			Seconds,
			Minutes,
			Hours,
			Days
		}

		#endregion

		protected LegacyAdapterBase(ProtocolType protocolType) : base(protocolType) { }

		#region Base Class Member Overrides

		protected override void Save(IPropertyBag propertyBag)
		{
			Uri = GetAddress();
			var config = Serialize();
			propertyBag.WriteAdapterCustomProperty("AdapterConfig", config);
		}

		#endregion

		[EditorBrowsable(EditorBrowsableState.Never)]
		[XmlElement("uri")]
		public string Uri { get; set; }

		[SuppressMessage("ReSharper", "RedundantCaseLabel")]
		protected TimeSpan BuildTimeSpan(int quantity, PollingUnitOfMeasure unit)
		{
			switch (unit)
			{
				case PollingUnitOfMeasure.Seconds:
					return TimeSpan.FromSeconds(quantity);
				case PollingUnitOfMeasure.Minutes:
					return TimeSpan.FromMinutes(quantity);
				case PollingUnitOfMeasure.Hours:
					return TimeSpan.FromHours(quantity);
				case PollingUnitOfMeasure.Days:
				default:
					return TimeSpan.FromDays(quantity);
			}
		}

		protected void UnbuildTimeSpan(TimeSpan interval, Action<int, PollingUnitOfMeasure> quantityAndUnitSetter)
		{
			if (interval.Seconds != 0)
			{
				quantityAndUnitSetter((int) interval.TotalSeconds, PollingUnitOfMeasure.Seconds);
			}
			else if (interval.Minutes != 0)
			{
				quantityAndUnitSetter((int) interval.TotalMinutes, PollingUnitOfMeasure.Minutes);
			}
			else if (interval.Hours != 0)
			{
				quantityAndUnitSetter((int) interval.TotalHours, PollingUnitOfMeasure.Hours);
			}
			else // if (interval.Days != 0)
			{
				quantityAndUnitSetter((int) interval.TotalDays, PollingUnitOfMeasure.Days);
			}
		}

		protected string Serialize()
		{
			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
			{
				// http://stackoverflow.com/questions/625927/omitting-all-xsi-and-xsd-namespaces-when-serializing-an-object-in-net
				var ns = new XmlSerializerNamespaces();
				ns.Add(string.Empty, string.Empty);
				var serializer = new XmlSerializer(GetType());
				serializer.Serialize(writer, this, ns);
			}
			return builder.ToString();
		}
	}

	public abstract class LegacyAdapterBase<TValidator> : LegacyAdapterBase
		where TValidator : IAdapterConfigValidation, new()
	{
		protected LegacyAdapterBase(ProtocolType protocolType) : base(protocolType) { }

		#region Base Class Member Overrides

		protected override void Validate()
		{
			var config = Serialize();
			var validator = new TValidator();
			try
			{
				validator.ValidateConfiguration(this is IInboundAdapter ? ConfigType.ReceiveLocation : ConfigType.TransmitLocation, config);
			}
			catch (ValidationException exception)
			{
				throw new BindingException(exception.Message.Trim('\r', '\n', ' '), exception);
			}
		}

		#endregion
	}
}
