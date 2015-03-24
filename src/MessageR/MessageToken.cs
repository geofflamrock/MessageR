using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageR
{
	/// <summary>
	/// Represents a token for interacting with a <see cref="MessageR.Message"/> that has been sent
	/// </summary>
	public class MessageToken
	{
		//////////////////////////////////////////////////////////////////////

		#region Members

		/// <summary>
		/// The broker we will use to listen for responses with
		/// </summary>
		private MessageBroker broker;

		/// <summary>
		/// The message this token relates
		/// </summary>
		private Message message;

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region Constructor

		/// <summary>
		/// Initialises an instance of the MessageToken with a reference to a MessageBroker
		/// </summary>
		/// <param name="broker"></param>
		public MessageToken(MessageBroker broker, Message message)
		{
			if (broker == null) throw new ArgumentNullException("broker");
			if (message == null) throw new ArgumentNullException("message");
			
			this.broker = broker;
			this.message = message;
		}

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region Public Methods

		/// <summary>
		/// Awaits a response to the message associated with the MessageToken
		/// </summary>
		/// <returns>A Task object which can be awaited on until a response is received to the Message</returns>
		public async Task AwaitResponse()
		{
			ListenerToken token = null;

			token = broker.Listen(m => m.ReferenceId == message.Id, m =>
			{
				token.StopListening();
			});

			await token.Completion;
		}

		/// <summary>
		/// Awaits a response to the message associated with the MessageToken with a given response message
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <returns>A Task object which can be awaited on until a response is received to the Message</returns>
		public async Task<T> AwaitResponse<T>() where T : Message
		{
			ListenerToken token = null;

			T result = default(T);

			token = broker.ListenOnce<T>(m => m.ReferenceId == message.Id, m =>
			{
				result = m;
			});

			await token.Completion;

			return result;
		}

		#endregion

		//////////////////////////////////////////////////////////////////////
	}
}
