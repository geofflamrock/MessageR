using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

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
		//private ConcurrentBag<MessageDispatcher> dispatchers
		//	= new ConcurrentBag<MessageDispatcher>();

		/// <summary>
		/// Listeners registered with the broker
		/// </summary>
		//private ConcurrentBag<MessageListener> listeners
		//	= new ConcurrentBag<MessageListener>();

		/// <summary>
		/// Block to send messages from
		/// </summary>
		private BufferBlock<Message> sendBlock 
			= new BufferBlock<Message>();

		/// <summary>
		/// Block to receive messages into
		/// </summary>
		private BufferBlock<Message> receiveBlock
			= new BufferBlock<Message>();

		/// <summary>
		/// References to disposable objects for our dispatchers
		/// </summary>
		private ConcurrentDictionary<IMessageDispatcher, IDisposable> dispatchers
			= new ConcurrentDictionary<IMessageDispatcher, IDisposable>();

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

			if (dispatchers.ContainsKey(dispatcher))
			{
				throw new Exception("Dispatcher has already been added");
			}

			ActionBlock<Message> dispatcherBlock = new ActionBlock<Message>(m => dispatcher.DispatchAsync(m));
			dispatchers.TryAdd(dispatcher, sendBlock.LinkTo(dispatcherBlock));				
		}

		/// <summary>
		/// Adds a dispatcher to the broker which handles all message types
		/// </summary>
		/// <param name="dispatcher"></param>
		public void AddDispatcher(IMessageDispatcher dispatcher, Predicate<Message> predicate)
		{
			if (dispatcher == null) throw new ArgumentNullException("dispatcher");
			if (predicate == null) throw new ArgumentNullException("predicate");

			if (dispatchers.ContainsKey(dispatcher))
			{
				throw new Exception("Dispatcher has already been added");
			}

			ActionBlock<Message> dispatcherBlock = new ActionBlock<Message>(m => dispatcher.DispatchAsync(m));
			dispatchers.TryAdd(dispatcher, sendBlock.LinkTo(dispatcherBlock, predicate));
		}

		/// <summary>
		/// Adds a dispatcher to the broker which handles messages according to the regular expression
		/// </summary>
		/// <param name="typeRegex"></param>
		/// <param name="dispatcher"></param>
		//public void AddDispatcher(string typeRegex, IMessageDispatcher dispatcher)
		//{
		//	if (typeRegex == null) throw new ArgumentNullException("typeRegex");
		//	if (dispatcher == null) throw new ArgumentNullException("dispatcher");

		//	Regex regex = new Regex(typeRegex, RegexOptions.Compiled);
		//	dispatchers.Add(new MessageDispatcher()
		//		{
		//			TypeRegex = regex,
		//			Dispatcher = dispatcher					
		//		});
		//}

		/// <summary>
		/// Removes a dispatcher from the broker
		/// </summary>
		/// <param name="dispatcher"></param>
		public void RemoveDispatcher(IMessageDispatcher dispatcher)
		{
			if (dispatcher == null) throw new ArgumentNullException("dispatcher");

			IDisposable dispatcherRef;

			if (dispatchers.TryRemove(dispatcher, out dispatcherRef))
			{
				dispatcherRef.Dispose();
			}
		}

		/// <summary>
		/// Sends a message through the broker
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool Send<T>(T message) where T : Message
		{
			if (message == null) throw new ArgumentNullException("message");

			return sendBlock.Post(message);
		}

		/// <summary>
		/// Sends a message through the broker asynchronously
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message"></param>
		/// <returns></returns>
		public async Task<bool> SendAsync<T>(T message) where T : Message
		{
			if (message == null) throw new ArgumentNullException("message");

			return await sendBlock.SendAsync(message);
		}

		/// <summary>
		/// Receives a message into the broker
		/// </summary>
		/// <param name="message"></param>
		public bool Receive(Message message)
		{
			if (message == null) throw new ArgumentNullException("message");

			return receiveBlock.Post(message);
		}

		/// <summary>
		/// Receives a message into the broker
		/// </summary>
		/// <param name="message"></param>
		public async Task<bool> ReceiveAsync(Message message)
		{
			if (message == null) throw new ArgumentNullException("message");

			return await receiveBlock.SendAsync(message);
		}

		/// <summary>
		/// Sends a message through the broker
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message"></param>
		/// <returns></returns>
		//public MessageToken Send<T>(T message) where T : Message
		//{
		//	if (message == null) throw new ArgumentNullException("message");

		//	IEnumerable<IMessageDispatcher> matchingDispatchers =
		//		GetDispatchers(typeof(T).ToString());

		//	foreach (IMessageDispatcher dispatcher in matchingDispatchers)
		//	{
		//		dispatcher.Dispatch(message);
		//	}

		//	return new MessageToken(this);
		//}

		/// <summary>
		/// Sends a message through the broker asynchronously
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message"></param>
		/// <returns>A Task object which can be awaited on for when the message has been dispatched by all matching dispatchers</returns>
		//public async Task<MessageToken> SendAsync<T>(T message) where T : Message
		//{
		//	if (message == null) throw new ArgumentNullException("message");

		//	IEnumerable<IMessageDispatcher> matchingDispatchers =
		//		GetDispatchers(typeof(T).ToString());

		//	List<Task> tasks = new List<Task>();

		//	foreach (IMessageDispatcher dispatcher in matchingDispatchers)
		//	{
		//		tasks.Add(dispatcher.DispatchAsync(message));
		//	}

		//	await Task.WhenAll(tasks);

		//	return new MessageToken(this);
		//}

		/// <summary>
		/// Listens to a message of type T, providing a handler for the message
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="handler"></param>
		public IDisposable Listen<T>(Action<T> handler) where T : Message
		{
			if (handler == null) throw new ArgumentNullException("handler");

			DisposableChain disposableChain = new DisposableChain();

			TransformBlock<Message, T> transformBlock = new TransformBlock<Message, T>(m => (T)m);
			ActionBlock<T> handlerBlock = new ActionBlock<T>(t => handler(t));
			disposableChain.AddDisposable(transformBlock.LinkTo(handlerBlock));
			disposableChain.AddDisposable(receiveBlock.LinkTo(transformBlock, m => typeof(T).IsAssignableFrom(m.GetType())));

			return disposableChain;
		}

		/// <summary>
		/// Listens to a message of type T once and then unsubscribes, providing a handler for the message
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="handler"></param>
		public IDisposable ListenOnce<T>(Action<T> handler) where T : Message
		{
			if (handler == null) throw new ArgumentNullException("handler");

			DisposableChain disposableChain = new DisposableChain();

			TransformBlock<Message, T> transformBlock = new TransformBlock<Message, T>(m => (T)m);
			ActionBlock<T> handlerBlock = new ActionBlock<T>(t => {
				
				// Pass through to the handler, but then complete the transformBlock and dispose all the blocks
				// to remove the links from the receive queue, meaning we will only ever handle
				// the message once
				handler(t);

				transformBlock.Complete();
				
				disposableChain.Dispose();
			});
			disposableChain.AddDisposable(transformBlock.LinkTo(handlerBlock));
			disposableChain.AddDisposable(receiveBlock.LinkTo(transformBlock, m => typeof(T).IsAssignableFrom(m.GetType())));

			return disposableChain;
		}

		/// <summary>
		/// Listens for message(s) of type given, providing a handler for messages received
		/// </summary>
		/// <param name="type"></param>
		/// <param name="handler"></param>
		public IDisposable Listen(Predicate<Message> predicate, Action<Message> handler)
		{
			if (predicate == null) throw new ArgumentNullException("predicate");
			if (handler == null) throw new ArgumentNullException("handler");

			ActionBlock<Message> handlerBlock = new ActionBlock<Message>(m =>
			{
				handler(m);
			});

			return receiveBlock.LinkTo(handlerBlock, predicate);
		}

		/// <summary>
		/// Processes a message, sending it to listeners
		/// </summary>
		/// <param name="message"></param>
		//public async Task ProcessMessage(Message message)
		//{
		//	if (message == null) throw new ArgumentNullException("message");

		//	IEnumerable<MessageListener> matchingListeners = GetListeners(message.Type);

		//	List<Task> tasks = new List<Task>();

		//	foreach (MessageListener listener in matchingListeners)
		//	{
		//		Func<Task> func = (Func<Task>)listener.Delegate;
		//		tasks.Add(func());
		//	}

		//	await Task.Factory.ContinueWhenAll(tasks.ToArray(), t => { });
		//}

		/// <summary>
		/// Processes a message, sending it to listeners
		/// </summary>
		/// <param name="message"></param>
		//public async Task<TResult> ProcessMessage<TResult>(Message message)
		//{
		//	if (message == null) throw new ArgumentNullException("message");

		//	IEnumerable<MessageListener> matchingListeners = GetListeners(message.Type);

		//	List<Task<TResult>> tasks = new List<Task<TResult>>();

		//	foreach (MessageListener listener in matchingListeners)
		//	{
		//		Func<Task<TResult>> func = (Func<Task<TResult>>)listener.Delegate;
		//		tasks.Add(func());
		//	}

		//	TResult result = default(TResult);

		//	await Task.Factory.ContinueWhenAny(tasks.ToArray(), t => { result = t.Result; });

		//	return result;
		//}

		/// <summary>
		/// Processes a message, sending it to listeners
		/// </summary>
		/// <param name="message"></param>
		//public async Task<TResult> ProcessMessage<T, TResult>(Message<T> message)
		//{
		//	if (message == null) throw new ArgumentNullException("message");

		//	IEnumerable<MessageListener> matchingListeners = GetListeners(message.Type);

		//	List<Task<TResult>> tasks = new List<Task<TResult>>();

		//	foreach (MessageListener listener in matchingListeners)
		//	{
		//		Func<T, Task<TResult>> func = (Func<T, Task<TResult>>)listener.Delegate;
		//		tasks.Add(func(message.Payload));
		//	}

		//	TResult result = default(TResult);

		//	await Task.Factory.ContinueWhenAny(tasks.ToArray(), t => { result = t.Result; });

		//	return result;
		//}

		/// <summary>
		/// Processes a message, sending it to listeners
		/// </summary>
		/// <param name="message"></param>
		//public async Task ProcessMessage<T>(Message<T> message)
		//{
		//	if (message == null) throw new ArgumentNullException("message");

		//	IEnumerable<MessageListener> matchingListeners = GetListeners(message.Type);

		//	List<Task> tasks = new List<Task>();

		//	foreach (MessageListener listener in matchingListeners)
		//	{
		//		Func<T, Task> func = (Func<T, Task>)listener.Delegate;
		//		tasks.Add(func(message.Payload));
		//	}

		//	await Task.Factory.ContinueWhenAll(tasks.ToArray(), t => { });
		//}

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region Private Methods

		/// <summary>
		/// Sends a message through the broker
		/// </summary>
		/// <param name="message">The message to send</param>
		/// <returns>A Task object, which may be awaited to wait on the processing of the message</returns>
		//private async Task SendMessage(Message message)
		//{
		//	IEnumerable<IMessageDispatcher> matchingDispatchers =
		//		GetDispatchers(message.Type);

		//	List<Task> tasks = new List<Task>();

		//	foreach (IMessageDispatcher dispatcher in matchingDispatchers)
		//	{
		//		tasks.Add(dispatcher.Dispatch(message));
		//	}

		//	await Task.Factory.ContinueWhenAll(tasks.ToArray(), t => { });
		//}

		/// <summary>
		/// Sends a message through the broker
		/// </summary>
		/// <param name="message">The message to send</param>
		/// <returns>A Task object, which may be awaited to wait on the processing of the message</returns>
		//private async Task<TResult> SendMessage<T, TResult>(Message<T> message)
		//{
		//	IEnumerable<IMessageDispatcher> matchingDispatchers =
		//		GetDispatchers(message.Type);

		//	List<Task<TResult>> tasks = new List<Task<TResult>>();

		//	foreach (IMessageDispatcher dispatcher in matchingDispatchers)
		//	{
		//		tasks.Add(dispatcher.Dispatch<T, TResult>(message));
		//	}

		//	TResult result = default(TResult);

		//	await Task.Factory.ContinueWhenAny<TResult>(tasks.ToArray(), t => { result = t.Result; });

		//	return result;
		//}

		/// <summary>
		/// Get dispatchers that match the given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		//private IEnumerable<IMessageDispatcher> GetDispatchers(string type)
		//{
		//	return dispatchers.Where(a => a.TypeRegex.IsMatch(type))
		//		.Select<MessageDispatcher, IMessageDispatcher>(md => md.Dispatcher);
		//}

		/// <summary>
		/// Get listeners that match the given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		//private IEnumerable<MessageListener> GetListeners(string type)
		//{
		//	return listeners.Where(a => a.TypeRegex.IsMatch(type));
		//}

		#endregion

		//////////////////////////////////////////////////////////////////////
	}
}
