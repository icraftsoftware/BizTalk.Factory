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
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public class RetryPolicy
	{
		static RetryPolicy()
		{
			var ti = new TransportInfo();
			_none = new RetryPolicy(ti.RetryCount, ti.RetryInterval);
		}

		public static RetryPolicy None
		{
			get { return _none; }
		}

		public RetryPolicy() { }

		private RetryPolicy(int count, int interval)
		{
			_count = count;
			_interval = TimeSpan.FromMinutes(interval);
		}

		public virtual int Count
		{
			get { return _count; }
			set { _count = value; }
		}

		public virtual TimeSpan Interval
		{
			get { return _interval; }
			set { _interval = value; }
		}

		private static readonly RetryPolicy _none;
		private int _count;
		private TimeSpan _interval;
	}
}
