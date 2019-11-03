#region Copyright & License

// Copyright © 2012 - 2019 François Chabot
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

using System.IO;
using System.Xml.Schema;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.Xml.Xsl;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	public interface ITransformFixtureInputSetup : IFluentInterface
	{
		ITransformFixtureInputSetup Arguments(XsltArgumentList arguments);

		ITransformFixtureInputSetup Context(IBaseMessageContext context);

		ITransformFixtureInputSetup Message(Stream message);

		ITransformFixtureInputSetup Message<TSchema>(Stream message)
			where TSchema : SchemaBase, new();

		ITransformFixtureInputSetup Message<TSchema>(Stream message, XmlSchemaContentProcessing contentProcessing)
			where TSchema : SchemaBase, new();
	}
}
