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
using Be.Stateless.Logging;
using Be.Stateless.ServiceModel.Channels;

namespace Be.Stateless.ServiceModel
{
	/// <summary>
	/// Provides the base implementation used to create service relay for <see cref="XmlMessage"/>-based messages.
	/// </summary>
	public abstract class ServiceRelay
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceRelay"/> class using the default target endpoint from the
		/// application configuration file.
		/// </summary>
		protected ServiceRelay()
		{
			_clientRelayFactory = () => new ClientRelay();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceRelay"/> class using the configuration information
		/// specified in the application configuration file by <paramref name="endpointConfigurationName"/>.
		/// </summary>
		/// <param name="endpointConfigurationName">
		/// The name of the endpoint in the application configuration file.
		/// </param>
		protected ServiceRelay(string endpointConfigurationName)
		{
			_clientRelayFactory = () => new ClientRelay(endpointConfigurationName);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceRelay"/> class using the configuration information
		/// specified in the application configuration file by <paramref name="endpointConfigurationName"/>.
		/// </summary>
		/// <param name="endpointConfigurationName">
		/// The name of the endpoint in the application configuration file.
		/// </param>
		/// <param name="remoteAddress">
		/// The address of the service.
		/// </param>
		protected ServiceRelay(string endpointConfigurationName, string remoteAddress)
		{
			_clientRelayFactory = () => new ClientRelay(endpointConfigurationName, remoteAddress);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceRelay"/> class using the specified <paramref
		/// name="binding"/> and target <paramref name="endpointAddress"/>.
		/// </summary>
		/// <param name="binding">
		/// The binding with which to make calls to the service.
		/// </param>
		/// <param name="endpointAddress">
		/// The address of the service endpoint.
		/// </param>
		protected ServiceRelay(Binding binding, EndpointAddress endpointAddress)
		{
			_clientRelayFactory = () => new ClientRelay(binding, endpointAddress);
		}

		/// <summary>
		/// Relays an <see cref="XmlMessage"/>-based request and returns the correlated <see cref="XmlMessage"/>-based
		/// response.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/>-based request.
		/// </typeparam>
		/// <typeparam name="TResponse">
		/// The type of the <see cref="XmlMessage"/>-based response.
		/// </typeparam>
		/// <param name="request">
		/// The <see cref="XmlMessage"/>-based request.
		/// </param>
		/// <returns>
		/// The <see cref="XmlMessage"/>-based response.
		/// </returns>
		protected TResponse RelayRequest<TRequest, TResponse>(TRequest request)
			where TRequest : XmlMessage
			where TResponse : XmlMessage, new()
		{
			return RelayRequest<TRequest, TResponse>(request, relay => relay.Endpoint.Binding.SendTimeout, DefaultConverter.Instance);
		}

		/// <summary>
		/// Converts and relays an <see cref="XmlMessage"/>-based request and, converts and returns the correlated
		/// <see cref="XmlMessage"/>-based response.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/>-based request.
		/// </typeparam>
		/// <typeparam name="TResponse">
		/// The type of the <see cref="XmlMessage"/>-based response.
		/// </typeparam>
		/// <param name="request">
		/// The <see cref="XmlMessage"/>-based request to convert and relay.
		/// </param>
		/// <param name="converter">
		/// The <see cref="IXmlMessageConverter"/> to use to convert the request and response.
		/// </param>
		/// <returns>
		/// The converted <see cref="XmlMessage"/>-based response.
		/// </returns>
		protected TResponse RelayRequest<TRequest, TResponse>(TRequest request, IXmlMessageConverter converter)
			where TRequest : XmlMessage
			where TResponse : XmlMessage, new()
		{
			return RelayRequest<TRequest, TResponse>(request, relay => relay.Endpoint.Binding.SendTimeout, converter);
		}

		/// <summary>
		/// Relays an <see cref="XmlMessage"/>-based request and returns the correlated <see cref="XmlMessage"/>-based
		/// response within a specified interval of time.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/>-based request.
		/// </typeparam>
		/// <typeparam name="TResponse">
		/// The type of the <see cref="XmlMessage"/>-based response.
		/// </typeparam>
		/// <param name="request">
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
		/// <see cref="ClientRelay.Request{TRequest,TResponse}(TRequest,TimeSpan)"/>
		protected TResponse RelayRequest<TRequest, TResponse>(TRequest request, TimeSpan timeout)
			where TRequest : XmlMessage
			where TResponse : XmlMessage, new()
		{
			return RelayRequest<TRequest, TResponse>(request, relay => timeout, DefaultConverter.Instance);
		}

		/// <summary>
		/// Converts and relays an <see cref="XmlMessage"/>-based request and, converts and returns the correlated
		/// <see cref="XmlMessage"/>-based response within a specified interval of time.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/>-based request.
		/// </typeparam>
		/// <typeparam name="TResponse">
		/// The type of the <see cref="XmlMessage"/>-based response.
		/// </typeparam>
		/// <param name="request">
		/// The <see cref="XmlMessage"/>-based request to convert and relay.
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
		/// <see cref="ClientRelay.Request{TRequest,TResponse}(TRequest,TimeSpan)"/>
		protected TResponse RelayRequest<TRequest, TResponse>(TRequest request, TimeSpan timeout, IXmlMessageConverter converter)
			where TRequest : XmlMessage
			where TResponse : XmlMessage, new()
		{
			return RelayRequest<TRequest, TResponse>(request, relay => timeout, converter);
		}

		private TResponse RelayRequest<TRequest, TResponse>(TRequest request, Func<ClientRelay, TimeSpan> getTimeout, IXmlMessageConverter converter)
			where TRequest : XmlMessage
			where TResponse : XmlMessage, new()
		{
			if (_logger.IsInfoEnabled) _logger.InfoFormat("Relaying synchronous request {0}.", request.ToString());
			ClientRelay clientRelay = null;
			try
			{
				clientRelay = _clientRelayFactory();
				var response = clientRelay.Request<TRequest, TResponse>(request, getTimeout(clientRelay), converter);
				clientRelay.Close();
				if (_logger.IsInfoEnabled) _logger.InfoFormat("Relaying synchronous response {0} succeeded.", response.ToString());
				return response;
			}
			catch (Exception exception)
			{
				if (_logger.IsWarnEnabled) _logger.Warn(string.Format("Relaying synchronous response {0} failed.", typeof(TResponse).Name), exception);
				if (clientRelay != null) clientRelay.Abort();
				throw;
			}
		}

		/// <summary>
		/// Begins an asynchronous operation to relay an <see cref="XmlMessage"/>-based request to the reply side of a
		/// request-reply message exchange.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/>-based request.
		/// </typeparam>
		/// <param name="request">
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
		protected IAsyncResult BeginRelayRequest<TRequest>(TRequest request, AsyncCallback asyncCallback, object asyncState)
			where TRequest : XmlMessage
		{
			return BeginRelayRequest(request, relay => relay.Endpoint.Binding.SendTimeout, DefaultConverter.Instance, asyncCallback, asyncState);
		}

		/// <summary>
		/// Begins an asynchronous operation to convert and relay an <see cref="XmlMessage"/>-based request to the reply
		/// side of a request-reply message exchange.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/>-based request.
		/// </typeparam>
		/// <param name="request">
		/// The <see cref="XmlMessage"/>-based request to convert and relay.
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
		protected IAsyncResult BeginRelayRequest<TRequest>(TRequest request, IXmlMessageConverter converter, AsyncCallback asyncCallback, object asyncState)
			where TRequest : XmlMessage
		{
			return BeginRelayRequest(request, relay => relay.Endpoint.Binding.SendTimeout, converter, asyncCallback, asyncState);
		}

		/// <summary>
		/// Begins an asynchronous operation to relay an <see cref="XmlMessage"/>-based request to the reply side of a
		/// request-reply message exchange within a specified interval of time.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/>-based request.
		/// </typeparam>
		/// <param name="request">
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
		/// <see cref="ClientRelay.BeginRequest{TRequest}(TRequest,TimeSpan,AsyncCallback,object)"/>
		protected IAsyncResult BeginRelayRequest<TRequest>(TRequest request, TimeSpan timeout, AsyncCallback asyncCallback, object asyncState)
			where TRequest : XmlMessage
		{
			return BeginRelayRequest(request, relay => timeout, DefaultConverter.Instance, asyncCallback, asyncState);
		}

		/// <summary>
		/// Begins an asynchronous operation to convert and relay an <see cref="XmlMessage"/>-based request to the reply
		/// side of a request-reply message exchange within a specified interval of time.
		/// </summary>
		/// <typeparam name="TRequest">
		/// The type of the <see cref="XmlMessage"/>-based request.
		/// </typeparam>
		/// <param name="request">
		/// The <see cref="XmlMessage"/>-based request to convert and relay.
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
		/// <see cref="ClientRelay.BeginRequest{TRequest}(TRequest,TimeSpan,AsyncCallback,object)"/>
		protected IAsyncResult BeginRelayRequest<TRequest>(TRequest request, TimeSpan timeout, IXmlMessageConverter converter, AsyncCallback asyncCallback, object asyncState)
			where TRequest : XmlMessage
		{
			return BeginRelayRequest(request, relay => timeout, converter, asyncCallback, asyncState);
		}

		private IAsyncResult BeginRelayRequest<TRequest>(TRequest request, Func<ClientRelay, TimeSpan> getTimeout, IXmlMessageConverter converter, AsyncCallback asyncCallback, object asyncState)
			where TRequest : XmlMessage
		{
			if (_logger.IsInfoEnabled) _logger.InfoFormat("Beginning relaying asynchronous request {0}.", request.ToString());
			ClientRelay clientRelay = null;
			try
			{
				clientRelay = _clientRelayFactory();
				var ar = clientRelay.BeginRequest(
					request,
					getTimeout(clientRelay),
					converter,
					asyncResult => asyncCallback(new AsyncResultWrapper(asyncResult, asyncState)),
					clientRelay);
				if (_logger.IsInfoEnabled) _logger.InfoFormat("Relaying asynchronous request {0} succeeded.", typeof(TRequest).Name);
				return ar;
			}
			catch (Exception exception)
			{
				if (_logger.IsWarnEnabled) _logger.Warn(string.Format("Relaying asynchronous request {0} failed.", typeof(TRequest).Name), exception);
				if (clientRelay != null) clientRelay.Abort();
				throw;
			}
		}

		/// <summary>
		/// Completes an asynchronous operation to return a, possibly converted, <see cref="XmlMessage"/>-based response
		/// to a relayed <see cref="XmlMessage"/>-based request.
		/// </summary>
		/// <typeparam name="TResponse">
		/// The type of the <see cref="XmlMessage"/>-based response.
		/// </typeparam>
		/// <param name="asyncResult">
		/// The <see cref="IAsyncResult"/> returned by a call to the <see
		/// cref="BeginRelayRequest{TRequest}(TRequest,AsyncCallback,object)"/> method.
		/// </param>
		/// <returns>
		/// The <see cref="XmlMessage"/>-based response.
		/// </returns>
		protected TResponse EndRelayRequest<TResponse>(IAsyncResult asyncResult)
			where TResponse : XmlMessage, new()
		{
			if (_logger.IsInfoEnabled) _logger.InfoFormat("Beginning relaying asynchronous response {0}.", typeof(TResponse).Name);
			ClientRelay clientRelay = null;
			try
			{
				var wrappedAsyncResult = ((AsyncResultWrapper) asyncResult).WrappedAsyncResult;
				clientRelay = (ClientRelay) wrappedAsyncResult.AsyncState;
				var response = clientRelay.EndRequest<TResponse>(wrappedAsyncResult);
				clientRelay.Close();
				if (_logger.IsInfoEnabled) _logger.InfoFormat("Relaying asynchronous response {0} succeeded.", response.ToString());
				return response;
			}
			catch (Exception exception)
			{
				if (_logger.IsWarnEnabled) _logger.Warn(string.Format("Relaying asynchronous response {0} failed.", typeof(TResponse).Name), exception);
				if (clientRelay != null) clientRelay.Abort();
				throw;
			}
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(ServiceRelay));
		private readonly Func<ClientRelay> _clientRelayFactory;
	}
}
