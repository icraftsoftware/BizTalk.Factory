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
using System.Xml;
using Be.Stateless.BizTalk.Message.Extensions;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.BaseTypes;

#pragma warning disable 660,661

namespace Be.Stateless.BizTalk.ContextProperties
{
	/// <summary>
	/// Strong-typing wrapper for any <see cref="MessageContextPropertyBase"/>-derived property.
	/// </summary>
	/// <typeparam name="T">
	/// The <see cref="MessageContextPropertyBase"/>-derived property to wrap.
	/// </typeparam>
	/// <typeparam name="TR">
	/// The type of the <see cref="MessageContextPropertyBase"/>-derived property.
	/// </typeparam>
	/// <remarks>
	/// To offer a truly strong-typed API over <see cref="IBaseMessageContext"/> properties, this wrapper is meant to be
	/// used in conjunction with the <see cref="IBaseMessage"/> extension methods provided by <see
	/// cref="BaseMessage"/>.
	/// </remarks>
	/// <seealso cref="BaseMessage.GetProperty{T}(IBaseMessage,MessageContextProperty{T,string})"/>
	/// <seealso cref="BaseMessage.GetProperty{T,TResult}(IBaseMessage,MessageContextProperty{T,TResult})"/>
	/// <seealso cref="BaseMessage.SetProperty{T}(IBaseMessage,ContextProperties.MessageContextProperty{T,string},string)"/>
	/// <seealso cref="BaseMessage.SetProperty{T,TV}(IBaseMessage,ContextProperties.MessageContextProperty{T,TV},TV)"/>
	/// <seealso cref="BaseMessage.Promote{T}(IBaseMessage,ContextProperties.MessageContextProperty{T,string},string)"/>
	/// <seealso cref="BaseMessage.Promote{T,TV}(IBaseMessage,ContextProperties.MessageContextProperty{T,TV},TV)"/>
	public class MessageContextProperty<T, TR> : IMessageContextProperty
		where T : MessageContextPropertyBase, new()
	{
		#region Operators

		public static bool operator ==(MessageContextProperty<T, TR> property, TR value)
		{
			throw new NotSupportedException("Exists only to support the writing of SendPort's Filter predicates in Binding DSL.");
		}

		public static bool operator >(MessageContextProperty<T, TR> property, TR value)
		{
			throw new NotSupportedException("Exists only to support the writing of SendPort's Filter predicates in Binding DSL.");
		}

		public static bool operator >=(MessageContextProperty<T, TR> property, TR value)
		{
			throw new NotSupportedException("Exists only to support the writing of SendPort's Filter predicates in Binding DSL.");
		}

		public static bool operator !=(MessageContextProperty<T, TR> property, TR value)
		{
			throw new NotSupportedException("Exists only to support the writing of SendPort's Filter predicates in Binding DSL.");
		}

		public static bool operator <(MessageContextProperty<T, TR> property, TR value)
		{
			throw new NotSupportedException("Exists only to support the writing of SendPort's Filter predicates in Binding DSL.");
		}

		public static bool operator <=(MessageContextProperty<T, TR> property, TR value)
		{
			throw new NotSupportedException("Exists only to support the writing of SendPort's Filter predicates in Binding DSL.");
		}

		#endregion

		static MessageContextProperty()
		{
			var t = new T();
			if (t.Type != typeof(TR))
				throw new ArgumentException(
					string.Format(
						"Message context property '{0}' is of type '{1}' but MessageContextProperty<{0}, {2}> declares it of type '{2}'.",
						t.Name.Name,
						t.Type.Name,
						typeof(TR).Name));
		}

		public MessageContextProperty()
		{
			var t = new T();
			QName = t.Name;
		}

		#region IMessageContextProperty Members

		public XmlQualifiedName QName { get; private set; }

		public string Name
		{
			get { return QName.Name; }
		}

		public string Namespace
		{
			get { return QName.Namespace; }
		}

		public Type Type
		{
			get { return typeof(T); }
		}

		#endregion
	}
}
