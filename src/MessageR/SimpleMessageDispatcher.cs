using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageR
{
	/// <summary>
	/// The SimpleMessageDispatcher takes a message sent to it and sends it back
	/// to the MessageBroker for processing.
	/// </summary>
	public class SimpleMessageDispatcher : IMessageDispatcher
	{
		//////////////////////////////////////////////////////////////////////

		#region Members

		/// <summary>
		/// The broker to send messages to
		/// </summary>
		private MessageBroker broker;

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region Constructor

		/// <summary>
		/// Initialises a new instance of the SimpleMessageDispatcher
		/// </summary>
		/// <param name="broker"></param>
		public SimpleMessageDispatcher(MessageBroker broker)
		{
			if (broker == null) throw new ArgumentNullException("broker");

			this.broker = broker;
		}

		#endregion

		//////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Dispatches the message by sending it straight back into 
		/// the MessageBroker for processing
		/// </summary>
		/// <param name="message"></param>
		public void Dispatch(Message message)
		{
			if (message == null) throw new ArgumentNullException("message");

			broker.Receive(message);
		}

		/// <summary>
		/// Dispatches the message by sending it straight back into 
		/// the MessageBroker for processing
		/// </summary>
		/// <param name="message"></param>
		public async Task DispatchAsync(Message message)
		{
			if (message == null) throw new ArgumentNullException("message");

			await broker.ReceiveAsync(message);
		}
	}
}
