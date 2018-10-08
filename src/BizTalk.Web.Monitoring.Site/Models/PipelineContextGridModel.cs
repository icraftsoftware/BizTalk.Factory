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

using System.Web;
using System.Web.Mvc;
using Be.Stateless.BizTalk.Web.Monitoring.Site.Helpers;

namespace Be.Stateless.BizTalk.Web.Monitoring.Site.Models
{
	public class PipelineContextGridModel : ContextGridModelBase
	{
		public PipelineContextGridModel(HtmlHelper helper)
			: base(helper)
		{
			Attributes(@class => "pipeline-config");
			Column.For(
				p => string.Format(
					"<div>{0}</div>",
					p.Name.EndsWith("Config")
						? helper.AsXml(p.Value.ToString())
						: new HtmlString(helper.Encode(p.Value))))
				.Named("Value")
				.Encode(false);
		}
	}
}
