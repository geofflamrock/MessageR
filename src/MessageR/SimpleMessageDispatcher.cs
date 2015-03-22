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
		/// <summary>
		/// Dispatches the message by sending it straight back into 
		/// the MessageBroker for processing
		/// </summary>
		/// <param name="message"></param>
		public async Task Dispatch(Message message)
		{
			throw new NotImplementedException();
		}


		public async Task<TResult> Dispatch<T, TResult>(Message<T> message)
		{
			throw new NotImplementedException();
		}
	}
}
