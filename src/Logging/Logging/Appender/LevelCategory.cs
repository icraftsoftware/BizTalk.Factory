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

namespace Be.Stateless.Logging.Appender
{
	/// <summary>
	/// This enumeration contains the different category values that can be written in the category property of an event log entry.
	/// </summary>
	public enum LevelCategory : short
	{
		Emergency = 20000,
		Fatal = 19900,
		Alert = 19800,
		Critical = 19700,
		Severe = 19600,
		Error = 19500,
		Warn = 19400,
		Notice = 19300,
		Info = 19200,
		Debug = 19100,
		Fine = 19000,
		Trace = 18900,
		Finer = 18800,
		Verbose = 18700,
		Finest = 18600
	}
}
