using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MessageR
{
	/// <summary>
	/// Extension methods for the <see cref="IMessageBroker"/> interface
	/// </summary>
	public static class MessageBrokerExtensions
	{
		/// <summary>
		/// Listens for a message with the given type once and then stops listening, without providing a handler
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="broker"></param>
		/// <returns></returns>
		public static ListenerToken ListenOnce<T>(this IMessageBroker broker) where T : Message
		{
			return ListenOnce<T>(broker, Functions.True, m => { });
		}

		/// <summary>
		/// Listens for a message with the given type once and then stops listening, providing a predicate but no handler
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="predicate"></param>
		/// <param name="broker"></param>
		/// <returns></returns>
		public static ListenerToken ListenOnce<T>(this IMessageBroker broker, Predicate<T> predicate) where T : Message
		{
			return ListenOnce<T>(broker, predicate, m => { });
		}

		/// <summary>
		/// Listens for a message with the given type once and then stops listening
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="broker"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		public static ListenerToken ListenOnce<T>(this IMessageBroker broker, Action<T> handler) where T : Message
		{
			if (handler == null) throw new ArgumentNullException("handler");

			return ListenOnce<T>(broker, Functions.True, handler);			
		}

		/// <summary>
		/// Listens for a message with the given type and predicate once and then stops listening
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="broker"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		public static ListenerToken ListenOnce<T>(this IMessageBroker broker, Predicate<T> predicate, Action<T> handler) where T : Message
		{
			if (handler == null) throw new ArgumentNullException("handler");

			ListenerToken token = null;

			Action<T> handlerWrapped = new Action<T>(t =>
			{
				// Stop listening first then pass to the handler
				if (t.Exception != null)
				{
					token.StopListening(t.Exception);
					return;
				}

				token.StopListening();
				handler(t);
			});

			token = broker.Listen<T>(predicate, handlerWrapped);

			return token;
		}
	}
}
