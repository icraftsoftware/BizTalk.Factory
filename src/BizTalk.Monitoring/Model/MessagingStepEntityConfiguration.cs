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

using System.Data.Entity.ModelConfiguration;

namespace Be.Stateless.BizTalk.Monitoring.Model
{
	public class MessagingStepEntityConfiguration : EntityTypeConfiguration<MessagingStep>
	{
		public MessagingStepEntityConfiguration()
		{
			HasKey(ms => ms.ActivityID);
			Property(ms => ms.BeginTime).HasColumnName("Time");
			Property(ms => ms.Name).HasColumnName("PortName");
			ToTable("bam_MessagingStep_AllInstances");
		}
	}
}
