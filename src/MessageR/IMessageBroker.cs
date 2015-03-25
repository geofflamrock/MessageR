using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageR
{
	/// <summary>
	/// Represents a broker of messages between senders and listeners
	/// </summary>
	public interface IMessageBroker
	{
		/// <summary>
		/// Adds a dispatcher to the broker which handles all messages
		/// </summary>
		/// <param name="dispatcher">The dispatcher to add</param>
		void AddDispatcher(IMessageDispatcher dispatcher);

		/// <summary>
		/// Adds a dispatcher to the broker which handles messages matching the predicate
		/// </summary>
		/// <param name="dispatcher">The dispatcher to add</param>
		/// <param name="predicate">The predicate used to match the dispatcher to messages</param>
		void AddDispatcher(IMessageDispatcher dispatcher, Predicate<Message> predicate);

		/// <summary>
		/// Removes a dispatcher from the broker
		/// </summary>
		/// <param name="dispatcher">The dispatcher to remove</param>
		void RemoveDispatcher(IMessageDispatcher dispatcher);

		/// <summary>
		/// Sends a message through the broker
		/// </summary>
		/// <typeparam name="T">Must inherit from <see cref="Message"/></typeparam>
		/// <param name="message">The message to be sent</param>
		/// <returns>A <see cref="MessageToken"/> which allows interaction with the sent message</returns>
		MessageToken Send<T>(T message) where T : Message;

		/// <summary>
		/// Sends a message through the broker asynchronously
		/// </summary>
		/// <typeparam name="T">Must inherit from <see cref="Message"/></typeparam>
		/// <param name="message">The message to be sent</param>
		/// <returns>A <see cref="MessageToken"/> which allows interaction with the sent message</returns>
		Task<MessageToken> SendAsync<T>(T message) where T : Message;

		/// <summary>
		/// Receives a message into the broker
		/// </summary>
		/// <param name="message">The message to be received into the broker</param>
		/// <returns>A boolean indicating whether the message was received successfully</returns>
		bool Receive(Message message);

		/// <summary>
		/// Receives a message into the broker asynchronously
		/// </summary>
		/// <param name="message">The message to be received into the broker</param>
		/// <returns>A boolean indicating whether the message was received successfully</returns>
		Task<bool> ReceiveAsync(Message message);

		/// <summary>
		/// Listens to a message of type T, providing an awaitable handler for the message
		/// </summary>
		/// <typeparam name="T">The type of message to listen to</typeparam>
		/// <param name="handler">The handler for messages received</param>
		/// <returns>A <see cref="ListenerToken"/> which can be used to stop listening</returns>
		ListenerToken Listen<T>(Func<T, Task> handler) where T : Message;

		/// <summary>
		/// Listens to a message of type T matching the given <see cref="Predicate{T}"/>, providing an awaitable handler for the message
		/// </summary>
		/// <typeparam name="T">The type of message to listen to</typeparam>
		/// <param name="handler">The handler for messages received</param>
		/// <param name="predicate">The Predicate used to match the received message to the listener</param>
		/// <returns>A <see cref="ListenerToken"/> which can be used to stop listening</returns>
		ListenerToken Listen<T>(Predicate<T> predicate, Func<T, Task> handler) where T : Message;

		/// <summary>
		/// Listens to a message of type T, providing a handler for the message
		/// </summary>
		/// <typeparam name="T">The type of message to listen to</typeparam>
		/// <param name="handler">The handler for messages received</param>
		/// <returns>A <see cref="ListenerToken"/> which can be used to stop listening</returns>
		ListenerToken Listen<T>(Action<T> handler) where T : Message;

		/// <summary>
		/// Listens to a message of type T matching the given <see cref="Predicate{T}"/>, providing a handler for the message
		/// </summary>
		/// <typeparam name="T">The type of message to listen to</typeparam>
		/// <param name="handler">The handler for messages received</param>
		/// <param name="predicate">The Predicate used to match the received message to the listener</param>
		/// <returns>A <see cref="ListenerToken"/> which can be used to stop listening</returns>
		ListenerToken Listen<T>(Predicate<T> predicate, Action<T> handler) where T : Message;

		/// <summary>
		/// Listens for message(s) matching the given <see cref="System.Predicate{Message}"/>, providing a handler for messages received
		/// </summary>
		/// <param name="predicate">The Predicate used to match the received message to the listener</param>
		/// <param name="handler">The handler for messages received</param>
		ListenerToken Listen(Predicate<Message> predicate, Action<Message> handler);

		/// <summary>
		/// Listens for message(s) matching the given <see cref="System.Predicate{Message}"/>, providing an awaitable handler for messages received
		/// </summary>
		/// <param name="predicate">The Predicate used to match the received message to the listener</param>
		/// <param name="handler">The handler for messages received</param>
		ListenerToken Listen(Predicate<Message> predicate, Func<Message, Task> handler);
	}
}
