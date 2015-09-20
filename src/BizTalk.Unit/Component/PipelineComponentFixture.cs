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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.Linq.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.Component
{
	/// <summary>
	/// This base class provides utility methods as well as non-regression test cases for <see
	/// cref="PipelineComponent"/>-derived classes.
	/// </summary>
	/// <typeparam name="T">
	/// The <see cref="PipelineComponent"/> derived class to test.
	/// </typeparam>
	public abstract class PipelineComponentFixture<T> where T : PipelineComponent, new()
	{
		protected virtual IEnumerable<PropertyInfo> BrowsableProperties
		{
			get
			{
				// all public properties that are either Browsable[(true)] qualified or not qualified at all (.All() is true
				// if the collection of BrowsableAttribute is empty)
				return typeof(T).GetProperties()
					.Where(
						propertyInfo => propertyInfo.GetCustomAttributes(typeof(BrowsableAttribute), true)
							.Cast<BrowsableAttribute>()
							.All(ba => ba.Browsable))
					.ToArray();
			}
		}

		protected Message.Mock<IBaseMessage> MessageMock { get; set; }

		protected Mock<IPipelineContext> PipelineContextMock { get; set; }

		protected T CreatePipelineComponent()
		{
			return new T();
		}

		protected virtual object GetValueForProperty(string name)
		{
			throw new NotSupportedException(
				string.Format(
					"No value has been provided for {0} property. {1} should either override GetValueForProperty(string name) or return a value for {0} property.",
					name,
					GetType().Name));
		}

		[SetUp]
		public void PipelineComponentFixtureOfTSetUp()
		{
			MessageMock = new Message.Mock<IBaseMessage> { DefaultValue = DefaultValue.Mock };
			PipelineContextMock = new Mock<IPipelineContext> { DefaultValue = DefaultValue.Mock };
			// default behaviour analogous to actual IPipelineContext implementation
			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(It.IsAny<string>()))
				.Callback<string>(t => { throw new COMException("Could not locate document specification with type: " + t); });
		}

		/// <summary>
		/// Ensure all properties are saved to property bags, it mainly protects pipeline component authors from stupid
		/// copy/paste mistakes in <see cref="PipelineComponent"/>-derived classes...
		/// </summary>
		[Test]
		public void AllPropertiesAreSavedToPropertyBag()
		{
			var sut = CreatePipelineComponent();

			var propertyBag = new PropertyBag();
			sut.Save(propertyBag, false, false);

			BrowsableProperties.Each(
				p => Assert.That(
					propertyBag.Contains(p.Name),
					string.Format(
						"{0} property has not been written to IPropertyBag. Notice that, for a string property, an empty string value should always be written even if it is null or empty.",
						p.Name)));
		}

		/// <summary>
		/// Ensure all properties are loaded from property bags, it mainly protects pipeline component authors from stupid
		/// copy/paste mistakes in <see cref="PipelineComponent"/>-derived classes...
		/// </summary>
		[Test]
		public void AllPropertyValuesAreLoadedFromPropertyBag()
		{
			var sut = CreatePipelineComponent();

			var propertyBagMock = new Mock<PropertyBag> { CallBase = true };
			BrowsableProperties.Each(
				p => {
					var baggableProperty = new BaggableProperty(p, GetValueForProperty).GenerateValue(p.GetValue(sut, null));
					propertyBagMock.Object.Add(p.Name, baggableProperty.BaggedValue);
					propertyBagMock
						.Setup(pb => pb.Read(p.Name))
						.Returns(baggableProperty.BaggedValue)
						.Verifiable(
							string.Format(
								"{0} property has not been read from IPropertyBag. Apply a [Browsable(false)] attribute to the property if it is intended.",
								p.Name));
				});

			sut.Load(propertyBagMock.Object, 0);

			propertyBagMock.Verify();
		}

		/// <summary>
		/// Ensure all properties are saved to property bags, it mainly protects pipeline component authors from stupid
		/// copy/paste mistakes in <see cref="PipelineComponent"/>-derived classes...
		/// </summary>
		[Test]
		public void AllPropertyValuesAreSavedToPropertyBag()
		{
			var sut = CreatePipelineComponent();

			var propertyBagMock = new Mock<PropertyBag> { CallBase = true };
			BrowsableProperties.Each(
				p => {
					var baggableProperty = new BaggableProperty(p, GetValueForProperty).GenerateValue(p.GetValue(sut, null));
					// assign value to property to ensure sut.Save() will save it to the bag
					p.SetValue(sut, baggableProperty.ActualValue, null);
					propertyBagMock
						.Setup(pb => pb.Write(p.Name, baggableProperty.BaggedValue))
						.Verifiable(
							string.Format(
								"{0} property has not been written to IPropertyBag. Apply a [Browsable(false)] attribute to the property if it is intended.",
								p.Name));
				});

			sut.Save(propertyBagMock.Object, true, true);

			propertyBagMock.Verify();
		}

		[Test]
		public void AllPropertiesConvertToStringAndBack()
		{
			var sut = CreatePipelineComponent();
			BrowsableProperties.Each(p => new BaggableProperty(p, GetValueForProperty).EnsureConvertToStringAndBack(p.GetValue(sut, null)));
		}

		[Test]
		public void ReturnsImmediatelyWhenMessageIsNull()
		{
			var sut = new Mock<T> { CallBase = true };

			var resultMessage = sut.Object.Execute(null, null);

			sut.Verify(pc => pc.ExecuteCore(It.IsAny<IPipelineContext>(), It.IsAny<IBaseMessage>()), Times.Never());
			Assert.That(resultMessage, Is.Null);
		}

		[Test]
		public void SkipsExecuteCoreWhenNotEnabled()
		{
			var sut = new Mock<T> { CallBase = true };

			sut.Object.Enabled = false;

			var resultMessage = sut.Object.Execute(PipelineContextMock.Object, MessageMock.Object);

			sut.Verify(pc => pc.ExecuteCore(It.IsAny<IPipelineContext>(), It.IsAny<IBaseMessage>()), Times.Never());
			Assert.That(resultMessage, Is.SameAs(MessageMock.Object));
		}

		[Test]
		public void ThrowsWhenPipelineContextIsNull()
		{
			var sut = CreatePipelineComponent();
			sut.Enabled = false;

			Assert.That(
				() => sut.Execute(null, MessageMock.Object),
				Throws.InstanceOf<ArgumentNullException>().With.Property("ParamName").EqualTo("pipelineContext"));
		}
	}
}
