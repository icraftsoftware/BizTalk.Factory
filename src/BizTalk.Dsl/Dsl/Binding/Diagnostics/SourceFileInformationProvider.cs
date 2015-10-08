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
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Diagnostics
{
	internal class SourceFileInformationProvider : IProvideSourceFileInformation
	{
		public SourceFileInformationProvider() { }

		public SourceFileInformationProvider(IProvideSourceFileInformation sourceFileInformationProvider)
		{
			if (sourceFileInformationProvider != null)
			{
				_columnNumber = sourceFileInformationProvider.Column;
				_fileName = sourceFileInformationProvider.Name;
				_lineNumber = sourceFileInformationProvider.Line;
			}
		}

		#region IProvideSourceFileInformation Members

		public string Name
		{
			get { return _fileName; }
		}

		public int Line
		{
			get { return _lineNumber; }
		}

		public int Column
		{
			get { return _columnNumber; }
		}

		#endregion

		internal void Capture()
		{
			if (_fileName.IsNullOrEmpty())
			{
				// walk up the stack frames...
				var stackFrame = Enumerable.Range(4, 20)
					.Select(i => new StackFrame(i, true))
					.FirstOrDefault(
						f => f.GetFileName() != null && f.GetMethod().IfNotNull(
							m => m.DeclaringType.IfNotNull(
								// until reaching out of Be.Stateless.BizTalk.Dsl assembly up to its calling assembly
								t => t.Assembly != typeof(SourceFileInformationProvider).Assembly
									// unless it is generated code, i.e qualified by a GeneratedCodeAttribute
									&& !t.GetCustomAttributes(typeof(GeneratedCodeAttribute), false).Any()
								)
							));

				if (stackFrame != null)
				{
					_fileName = stackFrame.GetFileName();
					_lineNumber = stackFrame.GetFileLineNumber();
					_columnNumber = stackFrame.GetFileColumnNumber();
				}

				// throwing InvalidOperationException means we might need to increase the Enumerable.Range count argument
				if (_fileName.IsNullOrEmpty() && _lineNumber == 0) throw new InvalidOperationException("Cannot determine source file information.");
			}
		}

		private int _columnNumber;
		private string _fileName;
		private int _lineNumber;
	}
}
