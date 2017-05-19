#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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
using Be.Stateless.BizTalk.Tracking;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Transform
{
	[Obsolete("Consider using both Be.Stateless.BizTalk.Transform.XlangTransformHelper and Be.Stateless.BizTalk.Transform.XlangMessageCollection instead.")]
	public static class XslCompiledTransformHelper
	{
		public static XLANGMessage Transform(XLANGMessage sourceMessage, Type mapType, TrackingContext trackingContext)
		{
			return XlangTransformHelper.Transform(
				sourceMessage,
				mapType,
				trackingContext);
		}

		public static XLANGMessage Transform(TrackingContext trackingContext, Type mapType, XLANGMessage sourceMessage, params object[] transformArguments)
		{
			return XlangTransformHelper.Transform(
				sourceMessage,
				mapType,
				trackingContext,
				transformArguments);
		}

		public static XLANGMessage Transform(
			XLANGMessage sourceMessage1,
			XLANGMessage sourceMessage2,
			Type mapType,
			TrackingContext trackingContext,
			params object[] transformArguments)
		{
			return XlangTransformHelper.Transform(
				new XlangMessageCollection(sourceMessage1, sourceMessage2),
				mapType,
				trackingContext,
				transformArguments);
		}

		public static XLANGMessage Transform(
			XLANGMessage sourceMessage1,
			XLANGMessage sourceMessage2,
			XLANGMessage sourceMessage3,
			Type mapType,
			TrackingContext trackingContext,
			params object[] transformArguments)
		{
			return XlangTransformHelper.Transform(
				new XlangMessageCollection(sourceMessage1, sourceMessage2, sourceMessage3),
				mapType,
				trackingContext,
				transformArguments);
		}

		public static XLANGMessage Transform(
			XLANGMessage sourceMessage1,
			XLANGMessage sourceMessage2,
			XLANGMessage sourceMessage3,
			XLANGMessage sourceMessage4,
			Type mapType,
			TrackingContext trackingContext,
			params object[] transformArguments)
		{
			return XlangTransformHelper.Transform(
				new XlangMessageCollection(sourceMessage1, sourceMessage2, sourceMessage3, sourceMessage4),
				mapType,
				trackingContext,
				transformArguments);
		}

		public static void TransformAndAddPart(XLANGMessage sourceMessage, Type mapType, XLANGMessage destinationMessage, string destinationPartName)
		{
			XlangTransformHelper.TransformAndAddPart(sourceMessage, mapType, destinationMessage, destinationPartName);
		}
	}
}
