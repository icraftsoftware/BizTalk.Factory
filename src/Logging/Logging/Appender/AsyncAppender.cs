#region Copyright & License

// Copyright © 2012 - 2014 François Chabot, Yves Dierick
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
using System.Threading;
using Be.Stateless.Extensions;
using log4net.Appender;
using log4net.Core;
using log4net.Util;

namespace Be.Stateless.Logging.Appender
{
	/// <summary>
	/// Appender that forwards LoggingEvents asynchronously.
	/// </summary>
	/// <remarks>
	/// This appender forwards LoggingEvents to a list of attached appenders. The events are forwarded asynchronously
	/// using the ThreadPool. This allows the calling thread to be released quickly, however it does not guarantee the
	/// ordering of events delivered to the attached appenders.
	/// </remarks>
	public sealed class AsyncAppender : IBulkAppender, IOptionHandler, IAppenderAttachable
	{
		#region IAppenderAttachable Members

		public void AddAppender(IAppender appender)
		{
			if (appender == null) throw new ArgumentNullException("appender");
			lock (_lockObject)
			{
				if (_appenderAttachedImpl == null) _appenderAttachedImpl = new AppenderAttachedImpl();
				_appenderAttachedImpl.AddAppender(appender);
			}
		}

		public AppenderCollection Appenders
		{
			get
			{
				lock (_lockObject)
				{
					return _appenderAttachedImpl == null
						? AppenderCollection.EmptyCollection
						: _appenderAttachedImpl.Appenders;
				}
			}
		}

		public IAppender GetAppender(string name)
		{
			lock (_lockObject)
			{
				return name == null || _appenderAttachedImpl == null
					? null
					: _appenderAttachedImpl.GetAppender(name);
			}
		}

		public void RemoveAllAppenders()
		{
			lock (_lockObject)
			{
				if (_appenderAttachedImpl != null)
				{
					_appenderAttachedImpl.RemoveAllAppenders();
					_appenderAttachedImpl = null;
				}
			}
		}

		public IAppender RemoveAppender(IAppender appender)
		{
			lock (_lockObject)
			{
				if (appender != null && _appenderAttachedImpl != null) return _appenderAttachedImpl.RemoveAppender(appender);
			}
			return null;
		}

		public IAppender RemoveAppender(string name)
		{
			lock (_lockObject)
			{
				if (name != null && _appenderAttachedImpl != null) return _appenderAttachedImpl.RemoveAppender(name);
			}
			return null;
		}

		#endregion

		#region IBulkAppender Members

		public string Name { get; set; }

		public void Close()
		{
			// signal we are closing
			_closePending = true;
			// Remove all the attached appenders, but wait until there is no async write pending.
			while (Thread.VolatileRead(ref _asyncWritePending) != 0)
			{
				Thread.Sleep(100);
			}
			lock (_lockObject)
			{
				if (_appenderAttachedImpl != null) _appenderAttachedImpl.RemoveAllAppenders();
			}
		}

		public void DoAppend(LoggingEvent loggingEvent)
		{
			Interlocked.Increment(ref _asyncWritePending);
			// sorry, we're losing events here, as we are busy closing
			if (_closePending)
			{
				Interlocked.Decrement(ref _asyncWritePending);
				return;
			}
			loggingEvent.Fix = _fixFlags;
			ThreadPool.QueueUserWorkItem(AsyncAppend, loggingEvent);
		}

		public void DoAppend(LoggingEvent[] loggingEvents)
		{
			Interlocked.Increment(ref _asyncWritePending);
			// sorry, we're losing events here, as we are busy closing
			if (_closePending)
			{
				Interlocked.Decrement(ref _asyncWritePending);
				return;
			}
			foreach (var loggingEvent in loggingEvents)
			{
				loggingEvent.Fix = _fixFlags;
			}
			ThreadPool.QueueUserWorkItem(AsyncAppend, loggingEvents);
		}

		#endregion

		#region IOptionHandler Members

		public void ActivateOptions() { }

		#endregion

		public FixFlags Fix
		{
			get { return _fixFlags; }
			set { _fixFlags = value; }
		}

		private void AsyncAppend(object state)
		{
			try
			{
				if (_appenderAttachedImpl != null)
				{
					var loggingEvent = state as LoggingEvent;
					if (loggingEvent != null)
					{
						_appenderAttachedImpl.AppendLoopOnAppenders(loggingEvent);
					}
					else
					{
						var loggingEvents = state as LoggingEvent[];
						if (loggingEvents != null)
						{
							_appenderAttachedImpl.AppendLoopOnAppenders(loggingEvents);
						}
					}
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(typeof(AsyncAppender), "An error occured while trying to call appenders asynchronously.", exception);
				if (exception.IsFatal()) throw;
			}
			finally
			{
				Interlocked.Decrement(ref _asyncWritePending);
			}
		}

		private readonly object _lockObject = new object();

		private AppenderAttachedImpl _appenderAttachedImpl;
		private int _asyncWritePending;
		private volatile bool _closePending;
		private FixFlags _fixFlags = FixFlags.All;
	}
}
