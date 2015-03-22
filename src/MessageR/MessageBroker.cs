using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MessageR
{
	/// <summary>
	/// The MessageBroker class provides brokering between 
	/// code sending and listening to messages within a system.
	/// 
	/// Messages are sent with a message type, and must implement
	/// the IMessage interface. Messages are sent via dispatchers, 
	/// which implement the IMessageDispatcher interface.
	/// 
	/// A dispatcher is added to the broker using the Register methods, 
	/// and may optionally use a regular expression to dictate which 
	/// types of messages should be sent via the dispatcher.
	/// 
	/// A message may be sent to multiple dispatchers as configuration allows.
	/// 
	/// Clients of the broker may listen for messages of certain types 
	/// by using the Listen methods and its overloads.
	/// 
	/// Messages that are received into the system are handled by the 
	/// OnMessageReceived event(?) and will be propagated out to matching listeners.
	/// 
	/// Listeners may optionally specify a marshaller to control which thread they
	/// receive a message on.
	/// 
	/// References held to listeners are weak references.
	/// </summary>
	public class MessageBroker
	{
		//////////////////////////////////////////////////////////////////////

		#region Private Classes

		/// <summary>
		/// Private class to hold message dispatchers
		/// </summary>
		private class MessageDispatcher
		{
			/// <summary>
			/// Gets and sets the regular expression that matches the dispatcher to messages
			/// </summary>
			public Regex TypeRegex
			{
				get;
				set;
			}

			/// <summary>
			/// Gets and sets the dispatcher
			/// </summary>
			public IMessageDispatcher Dispatcher
			{
				get;
				set;
			}
		}

		private class MessageListener
		{
			/// <summary>
			/// Gets and sets the regular expression that matches the listener to messages
			/// </summary>
			public Regex TypeRegex
			{
				get;
				set;
			}

			public Delegate Delegate
			{
				get;
				set;
			}
		}

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region Private Members

		/// <summary>
		/// Dispatchers registered with the broker
		/// </summary>
		private ConcurrentBag<MessageDispatcher> dispatchers
			= new ConcurrentBag<MessageDispatcher>();

		/// <summary>
		/// Listeners registered with the broker
		/// </summary>
		private ConcurrentBag<MessageListener> listeners
			= new ConcurrentBag<MessageListener>();

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region Constructors

		/// <summary>
		/// Initialises a new instance of the MessageBroker class
		/// </summary>
		public MessageBroker()
		{
			// NoOp
		}

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region Public Methods
		
		/// <summary>
		/// Adds a dispatcher to the broker which handles all message types
		/// </summary>
		/// <param name="dispatcher"></param>
		public void AddDispatcher(IMessageDispatcher dispatcher)
		{
			if (dispatcher == null) throw new ArgumentNullException("dispatcher");

			AddDispatcher(".*", dispatcher);
		}

		/// <summary>
		/// Adds a dispatcher to the broker which handles messages according to the regular expression
		/// </summary>
		/// <param name="typeRegex"></param>
		/// <param name="dispatcher"></param>
		public void AddDispatcher(string typeRegex, IMessageDispatcher dispatcher)
		{
			if (typeRegex == null) throw new ArgumentNullException("typeRegex");
			if (dispatcher == null) throw new ArgumentNullException("dispatcher");

			Regex regex = new Regex(typeRegex, RegexOptions.Compiled);
			dispatchers.Add(new MessageDispatcher()
				{
					TypeRegex = regex,
					Dispatcher = dispatcher					
				});
		}

		/// <summary>
		/// Removes a dispatcher from the broker
		/// </summary>
		/// <param name="dispatcher"></param>
		public void RemoveDispatcher(IMessageDispatcher dispatcher)
		{
			
		}

		/// <summary>
		/// Sends a message with the given type and no payload
		/// </summary>
		/// <param name="type"></param>
		/// <returns>A Task object, which can be used to await completion</returns>
		public async Task Send(string type)
		{
			if (type == null) throw new ArgumentNullException("type");

			await SendMessage(new Message(type));
		}

		/// <summary>
		/// Converts an object into a message and sends it, specifying it's message type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <param name="payload"></param>
		/// <returns>A Task object, which can be used to await completion</returns>
		public async Task Send<T>(string type, T payload)
		{
			if (type == null) throw new ArgumentNullException("type");
			if (payload == null) throw new ArgumentNullException("payload");

			await SendMessage(new Message<T>(type, payload));
		}

		/// <summary>
		/// Converts an object into a message and sends it, setting it's type
		/// to the fully qualified name of the type of the object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="payload"></param>
		/// <returns>A Task object, which may be awaited to wait on the processing of the message</returns>
		public async Task Send<T>(T payload)
		{
			if (payload == null) throw new ArgumentNullException("payload");

			await Send<T>(typeof(T).ToString(), payload);
		}

		/// <summary>
		/// Converts an object into a message and sends it, specifying it's message type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <param name="payload"></param>
		/// <returns>A Task object, which can be used to await completion</returns>
		public async Task<TResult> Send<TPayload, TResult>(string type, TPayload payload)
		{
			if (type == null) throw new ArgumentNullException("type");
			if (payload == null) throw new ArgumentNullException("payload");

			return await SendMessage<TPayload, TResult>(new Message<TPayload>(type, payload));
		}

		/// <summary>
		/// Converts an object into a message and sends it, specifying it's message type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <param name="payload"></param>
		/// <returns>A Task object, which can be used to await completion</returns>
		//public async Task<TResult> Send<TResult>(string type)
		//{
		//	if (type == null) throw new ArgumentNullException("type");

		//	return await SendMessage<TResult>(new Message(type));
		//}

		/// <summary>
		/// Converts an object into a message and sends it, setting it's type
		/// to the fully qualified name of the type of the object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="payload"></param>
		/// <returns>A Task object, which may be awaited to wait on the processing of the message</returns>
		public async Task<TResult> Send<TPayload, TResult>(TPayload payload)
		{
			if (payload == null) throw new ArgumentNullException("payload");

			return await Send<TPayload, TResult>(typeof(TPayload).ToString(), payload);
		}

		/// <summary>
		/// Listens to a message of the given type and provides a handler to process it. The type
		/// parameter may be a regular expression to match multiple message types.
		/// </summary>
		/// <param name="handler"></param>
		public void Listen(string type, Func<Task> handler)
		{
			if (type == null) throw new ArgumentNullException("type");
			if (handler == null) throw new ArgumentNullException("handler");

			Regex regex = new Regex(type, RegexOptions.Compiled);
			listeners.Add(new MessageListener()
			{
				TypeRegex = regex,
				Delegate = handler
			});
		}

		/// <summary>
		/// Listens to a message of the given type and provides a handler to process it. The type
		/// parameter may be a regular expression to match multiple message types.
		/// </summary>
		/// <param name="handler"></param>
		public void Listen<TResult>(string type, Func<Task<TResult>> handler)
		{
			if (type == null) throw new ArgumentNullException("type");
			if (handler == null) throw new ArgumentNullException("handler");

			Regex regex = new Regex(type, RegexOptions.Compiled);
			listeners.Add(new MessageListener()
			{
				TypeRegex = regex,
				Delegate = handler
			});
		}

		/// <summary>
		/// Listens to a message of the given type and provides a handler to process it. The type
		/// parameter may be a regular expression to match multiple message types.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="handler"></param>
		public void Listen<T>(string type, Func<T, Task> handler)
		{
			if (type == null) throw new ArgumentNullException("type");
			if (handler == null) throw new ArgumentNullException("handler");

			Regex regex = new Regex(type, RegexOptions.Compiled);
			listeners.Add(new MessageListener()
			{
				TypeRegex = regex,
				Delegate = handler
			});
		}

		/// <summary>
		/// Listens to a message of the given type and provides a handler to process it, returning the given result The type
		/// parameter may be a regular expression to match multiple message types.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="handler"></param>
		public void Listen<T, TResult>(string type, Func<T, Task<TResult>> handler)
		{
			if (type == null) throw new ArgumentNullException("type");
			if (handler == null) throw new ArgumentNullException("handler");

			Regex regex = new Regex(type, RegexOptions.Compiled);
			listeners.Add(new MessageListener()
			{
				TypeRegex = regex,
				Delegate = handler
			});
		}

		/// <summary>
		/// Processes a message, sending it to listeners
		/// </summary>
		/// <param name="message"></param>
		public async Task ProcessMessage(Message message)
		{
			if (message == null) throw new ArgumentNullException("message");

			IEnumerable<MessageListener> matchingListeners = GetListeners(message.Type);

			List<Task> tasks = new List<Task>();

			foreach (MessageListener listener in matchingListeners)
			{
				Func<Task> func = (Func<Task>)listener.Delegate;
				tasks.Add(func());
			}

			await Task.Factory.ContinueWhenAll(tasks.ToArray(), t => { });
		}

		/// <summary>
		/// Processes a message, sending it to listeners
		/// </summary>
		/// <param name="message"></param>
		public async Task<TResult> ProcessMessage<TResult>(Message message)
		{
			if (message == null) throw new ArgumentNullException("message");

			IEnumerable<MessageListener> matchingListeners = GetListeners(message.Type);

			List<Task<TResult>> tasks = new List<Task<TResult>>();

			foreach (MessageListener listener in matchingListeners)
			{
				Func<Task<TResult>> func = (Func<Task<TResult>>)listener.Delegate;
				tasks.Add(func());
			}

			TResult result = default(TResult);

			await Task.Factory.ContinueWhenAny(tasks.ToArray(), t => { result = t.Result; });

			return result;
		}

		/// <summary>
		/// Processes a message, sending it to listeners
		/// </summary>
		/// <param name="message"></param>
		public async Task<TResult> ProcessMessage<T, TResult>(Message<T> message)
		{
			if (message == null) throw new ArgumentNullException("message");

			IEnumerable<MessageListener> matchingListeners = GetListeners(message.Type);

			List<Task<TResult>> tasks = new List<Task<TResult>>();

			foreach (MessageListener listener in matchingListeners)
			{
				Func<T, Task<TResult>> func = (Func<T, Task<TResult>>)listener.Delegate;
				tasks.Add(func(message.Payload));
			}

			TResult result = default(TResult);

			await Task.Factory.ContinueWhenAny(tasks.ToArray(), t => { result = t.Result; });

			return result;
		}

		/// <summary>
		/// Processes a message, sending it to listeners
		/// </summary>
		/// <param name="message"></param>
		public async Task ProcessMessage<T>(Message<T> message)
		{
			if (message == null) throw new ArgumentNullException("message");

			IEnumerable<MessageListener> matchingListeners = GetListeners(message.Type);

			List<Task> tasks = new List<Task>();

			foreach (MessageListener listener in matchingListeners)
			{
				Func<T, Task> func = (Func<T, Task>)listener.Delegate;
				tasks.Add(func(message.Payload));
			}

			await Task.Factory.ContinueWhenAll(tasks.ToArray(), t => { });
		}

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region Private Methods

		/// <summary>
		/// Sends a message through the broker
		/// </summary>
		/// <param name="message">The message to send</param>
		/// <returns>A Task object, which may be awaited to wait on the processing of the message</returns>
		private async Task SendMessage(Message message)
		{
			IEnumerable<IMessageDispatcher> matchingDispatchers =
				GetDispatchers(message.Type);

			List<Task> tasks = new List<Task>();

			foreach (IMessageDispatcher dispatcher in matchingDispatchers)
			{
				tasks.Add(dispatcher.Dispatch(message));
			}

			await Task.Factory.ContinueWhenAll(tasks.ToArray(), t => { });
		}

		/// <summary>
		/// Sends a message through the broker
		/// </summary>
		/// <param name="message">The message to send</param>
		/// <returns>A Task object, which may be awaited to wait on the processing of the message</returns>
		private async Task<TResult> SendMessage<T, TResult>(Message<T> message)
		{
			IEnumerable<IMessageDispatcher> matchingDispatchers =
				GetDispatchers(message.Type);

			List<Task<TResult>> tasks = new List<Task<TResult>>();

			foreach (IMessageDispatcher dispatcher in matchingDispatchers)
			{
				tasks.Add(dispatcher.Dispatch<T, TResult>(message));
			}

			TResult result = default(TResult);

			await Task.Factory.ContinueWhenAny<TResult>(tasks.ToArray(), t => { result = t.Result; });

			return result;
		}

		/// <summary>
		/// Get dispatchers that match the given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private IEnumerable<IMessageDispatcher> GetDispatchers(string type)
		{
			return dispatchers.Where(a => a.TypeRegex.IsMatch(type))
				.Select<MessageDispatcher, IMessageDispatcher>(md => md.Dispatcher);
		}

		/// <summary>
		/// Get listeners that match the given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private IEnumerable<MessageListener> GetListeners(string type)
		{
			return listeners.Where(a => a.TypeRegex.IsMatch(type));
		}

		#endregion

		//////////////////////////////////////////////////////////////////////
	}
}
