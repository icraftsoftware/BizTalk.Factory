#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
using System.Collections.Generic;
using System.Linq;
using Microsoft.BizTalk.PipelineOM;
using Stages = Microsoft.BizTalk.PipelineOM.Stage;

namespace Be.Stateless.BizTalk.Dsl.Pipeline
{
	public sealed class StageCategory : IEquatable<StageCategory>
	{
		public static bool operator ==(StageCategory left, StageCategory right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(StageCategory left, StageCategory right)
		{
			return !Equals(left, right);
		}

		static StageCategory()
		{
			_stages = new Dictionary<Guid, StageCategory> {
				{ Stages.Any, new StageCategory("Any", Stages.Any) },
				{ Stages.AssemblingSerializer, new StageCategory("AssemblingSerializer", Stages.AssemblingSerializer) },
				{ Stages.Decoder, new StageCategory("Decoder", Stages.Decoder) },
				{ Stages.DisassemblingParser, new StageCategory("DisassemblingParser", Stages.DisassemblingParser, ExecutionMode.firstRecognized) },
				{ Stages.Encoder, new StageCategory("Encoder", Stages.Encoder) },
				{ Stages.PartyResolver, new StageCategory("PartyResolver", Stages.PartyResolver) },
				{ Stages.Validator, new StageCategory("Validator", Stages.Validator) }
			};
		}

		public static StageCategory Any
		{
			get { return _stages[Stages.Any]; }
		}

		public static StageCategory AssemblingSerializer
		{
			get { return _stages[Stages.AssemblingSerializer]; }
		}

		public static StageCategory Decoder
		{
			get { return _stages[Stages.Decoder]; }
		}

		public static StageCategory DisassemblingParser
		{
			get { return _stages[Stages.DisassemblingParser]; }
		}

		public static StageCategory Encoder
		{
			get { return _stages[Stages.Encoder]; }
		}

		public static StageCategory PartyResolver
		{
			get { return _stages[Stages.PartyResolver]; }
		}

		public static StageCategory Validator
		{
			get { return _stages[Stages.Validator]; }
		}

		public static bool IsKnownCategoryId(Guid categoryId)
		{
			return _stages.ContainsKey(categoryId);
		}

		public static StageCategory FromKnownCategoryId(Guid categoryId)
		{
			return _stages[categoryId];
		}

		internal StageCategory(string name, Guid id) : this(name, id, ExecutionMode.all) { }

		internal StageCategory(string name, Guid id, ExecutionMode executionMode)
		{
			ExecutionMode = executionMode;
			Id = id;
			Name = name;
		}

		#region IEquatable<StageCategory> Members

		public bool Equals(StageCategory other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.Id.Equals(Id);
		}

		#endregion

		#region Base Class Member Overrides

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(StageCategory)) return false;
			return Equals((StageCategory) obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		#endregion

		// ReSharper disable once MemberCanBePrivate.Global
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		public ExecutionMode ExecutionMode { get; private set; }

		public Guid Id { get; private set; }

		public string Name { get; private set; }

		public bool IsCompatibleWith(IEnumerable<StageCategory> stageCategories)
		{
			return stageCategories.Any(sc => sc == Any || sc == this);
		}

		private static readonly IDictionary<Guid, StageCategory> _stages;
	}
}
