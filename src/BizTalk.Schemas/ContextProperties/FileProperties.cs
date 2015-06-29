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
using FILE;

namespace Be.Stateless.BizTalk.ContextProperties
{
	// ReSharper disable InconsistentNaming
	public static class FileProperties
	{
		public static readonly MessageContextProperty<Password, string> Password
			= new MessageContextProperty<Password, string>();

		public static readonly MessageContextProperty<Username, string> Username
			= new MessageContextProperty<Username, string>();

		public static readonly MessageContextProperty<FileCreationTime, DateTime> FileCreationTime
			= new MessageContextProperty<FileCreationTime, DateTime>();

		public static readonly MessageContextProperty<ReceivedFileName, string> ReceivedFileName
			= new MessageContextProperty<ReceivedFileName, string>();
	}
}