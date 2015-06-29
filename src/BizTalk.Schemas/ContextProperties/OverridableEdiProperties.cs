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

using EdiOverride;

namespace Be.Stateless.BizTalk.ContextProperties
{
	// ReSharper disable InconsistentNaming
	public static class OverridableEdiProperties
	{
		public static readonly MessageContextProperty<OverrideEDIHeader, bool> Enabled
			= new MessageContextProperty<OverrideEDIHeader, bool>();

		public static readonly MessageContextProperty<UNB2_1, string> UNB2_1
			= new MessageContextProperty<UNB2_1, string>();

		public static readonly MessageContextProperty<UNB3_1, string> UNB3_1
			= new MessageContextProperty<UNB3_1, string>();

		public static readonly MessageContextProperty<UNB5, string> UNB5
			= new MessageContextProperty<UNB5, string>();

		public static readonly MessageContextProperty<UNB7, string> UNB7
			= new MessageContextProperty<UNB7, string>();

		public static readonly MessageContextProperty<UNB9, string> UNB9
			= new MessageContextProperty<UNB9, string>();

		public static readonly MessageContextProperty<UNB11, string> UNB11
			= new MessageContextProperty<UNB11, string>();
	}
}
