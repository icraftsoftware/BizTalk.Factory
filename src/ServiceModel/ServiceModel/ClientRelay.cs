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
using System.ServiceModel;
using System.ServiceModel.Channels;
using Be.Stateless.ServiceModel.Channels;

namespace Be.Stateless.ServiceModel
{
	/// <summary>
	/// Client proxy that can call relay <see cref="XmlMessage"/>-derived messages to any WCF service endpoint.
	/// </summary>
	/// <seealso href="http://msdn.microsoft.com/en-us/library/ms734675.aspx" />
	/// <seealso href="http://blogs.msdn.com/b/wenlong/archive/2007/10/27/performance-improvement-of-wcf-client-proxy-creation-and-best-practices.aspx" />
	internal class ClientRelay : ClientBase<IRequestChannel>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ClientRelay"/> class using the default target endpoint from the
		/// application configuration file.
		/// </summary>
		public ClientRelay() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ClientRelay"/> class using the configuration information
		/// specified in the application configuration file by <paramref name="endpointConfigurationName"/>.
		/// </summary>
		/// <param name="endpointConfigurationName">
		/// The name of the endpoint in the application configuration file.
		/// </param>
		public ClientRelay(string endpointConfigurationName)
			: base(endpointConfigurationName) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ClientRelay"/> class using the configuration information
		/// specified in the application configuration file by <paramref name="endpointConfigurationName"/>.
		/// </summary>
		/// <param name="endpointConfigurationName">
		/// The name of the endpoint in the application configuration file.
		/// </param>
		/// <param name="remoteAddress">
		/// The address of the service.
		/// </param>
		public ClientRelay(string endpointConfigurationName, string remoteAddress)
			: base(endpointConfigurationName, remoteAddress) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ClientRelay"/> class using the specified <paramref
		/// name="binding"/> and target <paramref name="endpointAddress"/>.
		/// </summary>
		/// <param name="binding">
		/// The binding with which to make calls to the service.
		/// </param>
		/// <param name="endpointAddress">
		/// The address of the service endpoint.
		/// </param>
		public ClientRelay(Binding binding, EndpointAddress endpointAddress)
			: base(binding, endpointAddress) { }

		/// <summary>
		/// Gets the remote address to which the <see cref="ClientRelay"/> sends messages.
		/// </summary>
		/// <returns>
		/// The <see cref="EndpointAddress"/> to which the <see cref="ClientRelay"/> sends messages.
		/// </returns>
		public EndpointAddress RemoteAddress
		{
			get { return Channel.RemoteAddress; }
		}

		/// <summary>
		/// Sends an <see cref="XmlMessage"/>-based request and returns the correlated <see cref="XmlMessage"/>-based
		/// response.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/>-based request.
		/// </typeparam>
		/// <typeparam name="TResponse">
		/// The type of the <see cref="XmlMessage"/>-based response.
		/// </typeparam>
		/// <param name="xmlRequest">
		/// The <see cref="XmlMessage"/>-based request.
		/// </param>
		/// <returns>
		/// The <see cref="XmlMessage"/>-based response.
		/// </returns>
		public TResponse Request<TRequest, TResponse>(TRequest xmlRequest)
			where TRequest : XmlMessage
			where TResponse : XmlMessage, new()
		{
			return Request<TRequest, TResponse>(xmlRequest, DefaultConverter.Instance);
		}

		/// <summary>
		/// Converts and sends an <see cref="XmlMessage"/>-based request and, converts and returns the correlated <see
		/// cref="XmlMessage"/>-based response.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/>-based request.
		/// </typeparam>
		/// <typeparam name="TResponse">
		/// The type of the <see cref="XmlMessage"/>-based response.
		/// </typeparam>
		/// <param name="xmlRequest">
		/// The <see cref="XmlMessage"/>-based request to convert and send.
		/// </param>
		/// <param name="converter">
		/// The <see cref="IXmlMessageConverter"/> to use to convert the request and response.
		/// </param>
		/// <returns>
		/// The converted <see cref="XmlMessage"/>-based response.
		/// </returns>
		public TResponse Request<TRequest, TResponse>(TRequest xmlRequest, IXmlMessageConverter converter)
			where TRequest : XmlMessage
			where TResponse : XmlMessage, new()
		{
			return Request<TRequest, TResponse>(xmlRequest, Endpoint.Binding.SendTimeout, converter);
		}

		/// <summary>
		/// Sends an <see cref="XmlMessage"/>-based request and returns the correlated <see cref="XmlMessage"/>-based
		/// response within a specified interval of time.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/>-based request.
		/// </typeparam>
		/// <typeparam name="TResponse">
		/// The type of the <see cref="XmlMessage"/>-based response.
		/// </typeparam>
		/// <param name="xmlRequest">
		/// The <see cref="XmlMessage"/>-based request.
		/// </param>
		/// <param name="timeout">
		/// The <see cref="TimeSpan"/> that specifies the interval of time within which a response must be received.
		/// </param>
		/// <returns>
		/// The <see cref="XmlMessage"/>-based response.
		/// </returns>
		/// <remarks>
		/// <para>
		/// If a timeout is passed while calling the function then that value is used. If the <see
		/// cref="Binding.SendTimeout"/> is set on the binding, then the value on the binding is used if no timeout is
		/// specified while calling the function.
		/// </para>
		/// <para>
		/// The <see cref="ChannelBase.DefaultSendTimeout"/> is used if no timeout is specified on either the binding or
		/// while calling the function. This default value is 1 minute.
		/// </para>
		/// </remarks>
		/// <see cref="IRequestChannel.Request(Message,TimeSpan)"/>
		public TResponse Request<TRequest, TResponse>(TRequest xmlRequest, TimeSpan timeout)
			where TRequest : XmlMessage
			where TResponse : XmlMessage, new()
		{
			return Request<TRequest, TResponse>(xmlRequest, timeout, DefaultConverter.Instance);
		}

		/// <summary>
		/// Converts and sends an <see cref="XmlMessage"/>-based request and, converts and returns the correlated <see
		/// cref="XmlMessage"/>-based response within a specified interval of time.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/>-based request.
		/// </typeparam>
		/// <typeparam name="TResponse">
		/// The type of the <see cref="XmlMessage"/>-based response.
		/// </typeparam>
		/// <param name="xmlRequest">
		/// The <see cref="XmlMessage"/>-based request to convert and send.
		/// </param>
		/// <param name="timeout">
		/// The <see cref="TimeSpan"/> that specifies the interval of time within which a response must be received.
		/// </param>
		/// <param name="converter">
		/// The <see cref="IXmlMessageConverter"/> to use to convert the request and response.
		/// </param>
		/// <returns>
		/// The converted <see cref="XmlMessage"/>-based response.
		/// </returns>
		public TResponse Request<TRequest, TResponse>(TRequest xmlRequest, TimeSpan timeout, IXmlMessageConverter converter)
			where TRequest : XmlMessage
			where TResponse : XmlMessage, new()
		{
			if (xmlRequest == null) throw new ArgumentNullException("xmlRequest");
			if (converter == null) throw new ArgumentNullException("converter");

			using (var request = converter.CreateMessageRequestFromXmlRequest(xmlRequest))
			using (var response = Channel.Request(request, timeout))
			{
				return converter.CreateXmlResponseFromMessageResponse<TResponse>(response);
			}
		}

		/// <summary>
		/// Begins an asynchronous operation to transmit an <see cref="XmlMessage"/>-based request to the reply side of a
		/// request-reply message exchange.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/>-based request.
		/// </typeparam>
		/// <param name="xmlRequest">
		/// The <see cref="XmlMessage"/>-based request.
		/// </param>
		/// <param name="asyncCallback">
		/// The <see cref="AsyncCallback"/> delegate that receives the notification of the completion of the asynchronous
		/// operation transmitting a request message.
		/// </param>
		/// <param name="asyncState">
		/// An object, specified by the application, that contains state information associated with the asynchronous
		/// operation transmitting a request message.
		/// </param>
		/// <returns>
		/// The <see cref="IAsyncResult"/> that references the asynchronous message transmission.
		/// </returns>
		public IAsyncResult BeginRequest<TRequest>(TRequest xmlRequest, AsyncCallback asyncCallback, object asyncState)
			where TRequest : XmlMessage
		{
			return BeginRequest(xmlRequest, Endpoint.Binding.SendTimeout, asyncCallback, asyncState);
		}

		/// <summary>
		/// Begins an asynchronous operation to convert and transmit an <see cref="XmlMessage"/>-based request to the
		/// reply side of a request-reply message exchange.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/>-based request.
		/// </typeparam>
		/// <param name="xmlRequest">
		/// The <see cref="XmlMessage"/>-based request.
		/// </param>
		/// <param name="converter">
		/// The <see cref="IXmlMessageConverter"/> to use to convert the request and response.
		/// </param>
		/// <param name="asyncCallback">
		/// The <see cref="AsyncCallback"/> delegate that receives the notification of the completion of the asynchronous
		/// operation transmitting a request message.
		/// </param>
		/// <param name="asyncState">
		/// An object, specified by the application, that contains state information associated with the asynchronous
		/// operation transmitting a request message.
		/// </param>
		/// <returns>
		/// The <see cref="IAsyncResult"/> that references the asynchronous message transmission.
		/// </returns>
		public IAsyncResult BeginRequest<TRequest>(TRequest xmlRequest, IXmlMessageConverter converter, AsyncCallback asyncCallback, object asyncState)
			where TRequest : XmlMessage
		{
			return BeginRequest(xmlRequest, Endpoint.Binding.SendTimeout, converter, asyncCallback, asyncState);
		}

		/// <summary>
		/// Begins an asynchronous operation to transmit an <see cref="XmlMessage"/>-based request to the reply side of a
		/// request-reply message exchange within a specified interval of time.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/>-based request.
		/// </typeparam>
		/// <param name="xmlRequest">
		/// The <see cref="XmlMessage"/>-based request.
		/// </param>
		/// <param name="timeout">
		/// The <see cref="TimeSpan"/> that specifies the interval of time within which a response must be received.
		/// </param>
		/// <param name="asyncCallback">
		/// The <see cref="AsyncCallback"/> delegate that receives the notification of the completion of the asynchronous
		/// operation transmitting a request message.
		/// </param>
		/// <param name="asyncState">
		/// An object, specified by the application, that contains state information associated with the asynchronous
		/// operation transmitting a request message.
		/// </param>
		/// <returns>
		/// The <see cref="IAsyncResult"/> that references the asynchronous message transmission.
		/// </returns>
		/// <remarks>
		/// <para>
		/// If a timeout is passed while calling the function then that value is used. If the <see
		/// cref="Binding.SendTimeout"/> is set on the binding, then the value on the binding is used if no timeout is
		/// specified while calling the function.
		/// </para>
		/// <para>
		/// The <see cref="ChannelBase.DefaultSendTimeout"/> is used if no timeout is specified on either the binding or
		/// while calling the function. This default value is 1 minute.
		/// </para>
		/// </remarks>
		/// <seealso cref="IRequestChannel.BeginRequest(Message,TimeSpan,AsyncCallback,object)"/>
		public IAsyncResult BeginRequest<TRequest>(TRequest xmlRequest, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState)
			where TRequest : XmlMessage
		{
			return BeginRequest(xmlRequest, timeout, DefaultConverter.Instance, asyncCallback, asyncState);
		}

		/// <summary>
		/// Begins an asynchronous operation to convert and transmit an <see cref="XmlMessage"/>-based request to the
		/// reply side of a request-reply message exchange within a specified interval of time.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/>-based request.
		/// </typeparam>
		/// <param name="xmlRequest">
		/// The <see cref="XmlMessage"/>-based request.
		/// </param>
		/// <param name="timeout">
		/// The <see cref="TimeSpan"/> that specifies the interval of time within which a response must be received.
		/// </param>
		/// <param name="converter">
		/// The <see cref="IXmlMessageConverter"/> to use to convert the request and response.
		/// </param>
		/// <param name="asyncCallback">
		/// The <see cref="AsyncCallback"/> delegate that receives the notification of the completion of the asynchronous
		/// operation transmitting a request message.
		/// </param>
		/// <param name="asyncState">
		/// An object, specified by the application, that contains state information associated with the asynchronous
		/// operation transmitting a request message.
		/// </param>
		/// <returns>
		/// The <see cref="IAsyncResult"/> that references the asynchronous message transmission.
		/// </returns>
		/// <remarks>
		/// <para>
		/// If a timeout is passed while calling the function then that value is used. If the <see
		/// cref="Binding.SendTimeout"/> is set on the binding, then the value on the binding is used if no timeout is
		/// specified while calling the function.
		/// </para>
		/// <para>
		/// The <see cref="ChannelBase.DefaultSendTimeout"/> is used if no timeout is specified on either the binding or
		/// while calling the function. This default value is 1 minute.
		/// </para>
		/// </remarks>
		/// <seealso cref="IRequestChannel.BeginRequest(Message,TimeSpan,AsyncCallback,object)"/>
		public IAsyncResult BeginRequest<TRequest>(
			TRequest xmlRequest, TimeSpan timeout, IXmlMessageConverter converter, AsyncCallback asyncCallback, object asyncState)
			where TRequest : XmlMessage
		{
			var request = converter.CreateMessageRequestFromXmlRequest(xmlRequest);
			return Channel.BeginRequest(
				request,
				timeout,
				asyncResult => asyncCallback(new AsyncResultWrapper(asyncResult, asyncState)),
				new object[] { request, converter });
		}

		/// <summary>
		/// Completes an asynchronous operation to return a, possibly converted, <see cref="XmlMessage"/>-based response
		/// to a transmitted <see cref="XmlMessage"/>-based request.
		/// </summary>
		/// <typeparam name="TResponse">
		/// The type of the <see cref="XmlMessage"/>-based response.
		/// </typeparam>
		/// <param name="asyncResult">
		/// The <see cref="IAsyncResult"/> returned by a call to the <see
		/// cref="BeginRequest{TRequest}(TRequest,AsyncCallback,object)"/> method.
		/// </param>
		/// <returns>
		/// The <see cref="XmlMessage"/>-based response.
		/// </returns>
		public TResponse EndRequest<TResponse>(IAsyncResult asyncResult)
			where TResponse : XmlMessage, new()
		{
			var wrappedAsyncResult = ((AsyncResultWrapper) asyncResult).WrappedAsyncResult;
			var tuple = (object[]) wrappedAsyncResult.AsyncState;
			var request = (Message) tuple[0];
			request.Close();
			var converter = (IXmlMessageConverter) tuple[1];
			using (var response = Channel.EndRequest(wrappedAsyncResult))
			{
				return converter.CreateXmlResponseFromMessageResponse<TResponse>(response);
			}
		}
	}
}
