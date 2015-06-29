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
using System.ComponentModel;
using System.Globalization;

namespace Be.Stateless.BizTalk.Web.Monitoring.Model
{
	/// <summary>
	/// Provides a unified way of converting types of values to other types, as
	/// well as for accessing standard values and subproperties.
	/// </summary>
	public class StatusConverter : TypeConverter
	{
		/// <summary>
		/// Returns whether this converter can convert an object of the given type
		/// to the type of this converter, using the specified context.
		/// </summary>
		/// <returns>
		/// true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		/// <param name="context">
		/// An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that
		/// provides a format context.</param>
		/// <param name="sourceType">
		/// A <see cref="System.Type"/> that represents the type you want to
		/// convert from.
		/// </param>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// Converts the given object to the type of this converter, using the
		/// specified context and culture information.
		/// </summary>
		/// <returns>
		/// An <see cref="System.Object"/> that represents the converted value.
		/// </returns>
		/// <param name="context">
		/// An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that
		/// provides a format context.
		/// </param>
		/// <param name="culture">The <see
		/// cref="System.Globalization.CultureInfo"/> to use as the current
		/// culture.
		/// </param>
		/// <param name="value">The <see cref="System.Object"/> to convert.
		/// </param>
		/// <exception cref="System.NotSupportedException">The conversion cannot
		/// be performed.
		/// </exception>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value == null) throw new ArgumentNullException("value");

			if (value.GetType() != typeof(string)) return base.ConvertFrom(context, culture, value);

			var text = (string) value;

			if (text.Equals("*", StringComparison.OrdinalIgnoreCase)) return Status.Any;

			if (text.Equals("c", StringComparison.OrdinalIgnoreCase)) return Status.Completed;

			if (text.Equals("f", StringComparison.OrdinalIgnoreCase)) return Status.Failed;

			if (text.Equals("fm", StringComparison.OrdinalIgnoreCase)) return Status.FailedMessage;

			if (text.Equals("fs", StringComparison.OrdinalIgnoreCase)) return Status.FailedStep;

			if (text.Equals("r", StringComparison.OrdinalIgnoreCase)) return Status.Received;

			if (text.Equals("p", StringComparison.OrdinalIgnoreCase)) return Status.Pending;

			if (text.Equals("s", StringComparison.OrdinalIgnoreCase)) return Status.Sent;

			return (Status) Enum.Parse(typeof(Status), text, true);
		}
	}
}