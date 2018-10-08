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

using System.Diagnostics.CodeAnalysis;
using Microsoft.Adapters.SAP.BiztalkPropertySchema;

namespace Be.Stateless.BizTalk.ContextProperties
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[SuppressMessage("ReSharper", "IdentifierTypo")]
	public static class SapProperties
	{
		public static readonly MessageContextProperty<DOCNUM, string> DOCNUM
			= new MessageContextProperty<DOCNUM, string>();

		public static readonly MessageContextProperty<MESTYP, string> MESTYP
			= new MessageContextProperty<MESTYP, string>();

		public static readonly MessageContextProperty<RCVPRN, string> RCVPRN
			= new MessageContextProperty<RCVPRN, string>();

		public static readonly MessageContextProperty<STATUS, string> STATUS
			= new MessageContextProperty<STATUS, string>();
	}
}
