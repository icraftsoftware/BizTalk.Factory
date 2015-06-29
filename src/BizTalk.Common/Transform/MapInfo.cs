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
using System.Xml.Xsl;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Transform
{
	internal class MapInfo
	{
		internal MapInfo(XslCompiledTransform xslTransform, TransformBase bizTalkMapInstance)
		{
			if (xslTransform == null) throw new ArgumentNullException("xslTransform");
			if (bizTalkMapInstance == null) throw new ArgumentNullException("bizTalkMapInstance");
			_xslTransform = xslTransform;
			_bizTalkMapInstance = bizTalkMapInstance;
			_arguments = bizTalkMapInstance.TransformArgs;
		}

		internal XslCompiledTransform Transform
		{
			get { return _xslTransform; }
		}

		internal XsltArgumentList Arguments
		{
			get { return _arguments; }
		}

		internal XsltArgumentList GetTransformArgumentListCopy()
		{
			return _bizTalkMapInstance.TransformArgs;
		}

		private readonly XsltArgumentList _arguments;
		private readonly TransformBase _bizTalkMapInstance;
		private readonly XslCompiledTransform _xslTransform;
	}
}