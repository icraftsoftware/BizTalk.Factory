#region Copyright & License

// Copyright © 2012 - 2014 François Chabot, Yves Dierick
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

namespace Be.Stateless.BizTalk.RuleEngine
{
	public class Context
	{
		public Context(IBaseMessageContext context)
		{
			if (context == null) throw new ArgumentNullException("context");
			_context = context;
		}

		public void Promote(XmlQualifiedName qname, object value)
		{
			_context.Promote(qname.Name, qname.Namespace, value);
		}

		public void Promote(string name, string @namespace, object value)
		{
			_context.Promote(name, @namespace, value);
		}

		public object Read(XmlQualifiedName qname)
		{
			return _context.Read(qname.Name, qname.Namespace);
		}

		public object Read(string name, string @namespace)
		{
			return _context.Read(name, @namespace);
		}

		public void Write(XmlQualifiedName qname, object value)
		{
			_context.Write(qname.Name, qname.Namespace, value);
		}

		public void Write(string name, string @namespace, object value)
		{
			_context.Write(name, @namespace, value);
		}

		private readonly IBaseMessageContext _context;
	}
}
