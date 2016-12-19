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
using System.Xml;
using Microsoft.BizTalk.Message.Interop;
using Moq;

namespace Be.Stateless.BizTalk.Unit.RuleEngine
{
	/// <summary>
	/// ContextFact with a value typed after its corresponding MessageContextProperty.
	/// </summary>
	/// <typeparam name="T">
	/// The ContextFact's value type.
	/// </typeparam>
	public class ContextFact<T>
	{
		public ContextFact(XmlQualifiedName qname)
		{
			_qname = qname;
		}

		public ContextFact<T> HasBeenPromoted()
		{
			_promoted = true;
			return this;
		}

		public ContextFact<T> HasBeenWritten()
		{
			_written = true;
			return this;
		}

		public ContextFact<T> HasNotBeenSet()
		{
			_promoted = false;
			_written = false;
			return this;
		}

		public ContextFact<T> HasNotBeenPromoted()
		{
			_promoted = false;
			return this;
		}

		public ContextFact<T> HasNotBeenWritten()
		{
			_written = false;
			return this;
		}

		public ContextFact<T> WithAnyValue()
		{
			_anyValue = true;
			return this;
		}

		public ContextFact<T> WithValue(T value)
		{
			_value = value;
			return this;
		}

		internal void Assert(Mock<IBaseMessageContext> context)
		{
			context
				.Setup(c => c.Read(_qname.Name, _qname.Namespace))
				.Returns(_value);
		}

		internal void Verify(Mock<IBaseMessageContext> context)
		{
			if (!_promoted.HasValue && !_written.HasValue)
				throw new InvalidOperationException(
					string.Format(
						"No Promote or Write expectation has been defined for property {0}.",
						_qname));

			if (_promoted.HasValue && _written.HasValue && _promoted.Value && _written.Value)
				throw new InvalidOperationException(
					string.Format(
						"Conflicting Promote and Write expectations have been defined for property {0}.",
						_qname));

			if (_promoted.HasValue)
			{
				if (!_promoted.Value)
					context.Verify(
						c => c.Promote(_qname.Name, _qname.Namespace, It.IsAny<T>()),
						Times.Never(),
						string.Format("For property whose QName is '{0}'", _qname));
				else if (_anyValue)
					context.Verify(
						c => c.Promote(_qname.Name, _qname.Namespace, It.IsAny<T>()),
						string.Format("For property whose QName is '{0}'", _qname));
				else
					context.Verify(
						c => c.Promote(_qname.Name, _qname.Namespace, _value),
						string.Format("For property whose QName is '{0}'", _qname));
			}

			if (_written.HasValue)
			{
				if (!_written.Value)
					context.Verify(
						c => c.Write(_qname.Name, _qname.Namespace, It.IsAny<T>()),
						Times.Never(),
						string.Format("For property whose QName is '{0}'", _qname));
				else if (_anyValue)
					context.Verify(
						c => c.Write(_qname.Name, _qname.Namespace, It.IsAny<T>()),
						string.Format("For property whose QName is '{0}'", _qname));
				else
					context.Verify(
						c => c.Write(_qname.Name, _qname.Namespace, _value),
						string.Format("For property whose QName is '{0}'", _qname));
			}
		}

		private readonly XmlQualifiedName _qname;
		private bool _anyValue;
		private bool? _promoted;
		private T _value;
		private bool? _written;
	}
}
