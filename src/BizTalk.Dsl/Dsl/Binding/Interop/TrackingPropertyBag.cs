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

using System.Collections.Generic;
using System.Linq;

namespace Be.Stateless.BizTalk.Dsl.Binding.Interop
{
	/// <summary>
	/// Property bag that takes an initial snapshot of the default state so as to be able to track the properties that do
	/// not have their default values anymore.
	/// </summary>
	internal class TrackingPropertyBag : PropertyBag
	{
		#region Base Class Member Overrides

		protected override IEnumerable<KeyValuePair<string, object>> Properties
		{
			get { return base.Properties.Except(_defaultProperties); }
		}

		#endregion

		internal void TrackChanges()
		{
			_defaultProperties = Bag.ToArray();
			Bag.Clear();
		}

		private IEnumerable<KeyValuePair<string, object>> _defaultProperties;
	}
}
