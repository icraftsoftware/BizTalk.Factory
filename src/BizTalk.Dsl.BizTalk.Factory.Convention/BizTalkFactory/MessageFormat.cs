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

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention.BizTalkFactory
{
	public class MessageFormat<TNamingConvention>
	{
		internal MessageFormat(TNamingConvention convention)
		{
			_convention = convention;
		}

		public TNamingConvention Csv
		{
			get { return Custom("CSV"); }
		}

		public TNamingConvention Edi
		{
			get { return Custom("EDI"); }
		}

		public TNamingConvention FF
		{
			get { return Custom("FF"); }
		}

		public TNamingConvention Idoc
		{
			get { return Custom("IDOC"); }
		}

		public TNamingConvention Irrelevant
		{
			get { return None; }
		}

		public TNamingConvention Mime
		{
			get { return Custom("MIME"); }
		}

		public TNamingConvention None
		{
			get { return Custom(string.Empty); }
		}

		public TNamingConvention Xml
		{
			get { return Custom("XML"); }
		}

		public TNamingConvention Custom<T>(T messageFormat)
		{
			((IMessageFormatMemento<T>) _convention).MessageFormat = messageFormat;
			return _convention;
		}

		private readonly TNamingConvention _convention;
	}
}
