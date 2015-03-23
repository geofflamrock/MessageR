using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageR
{
	public class MessageToken
	{
		//////////////////////////////////////////////////////////////////////

		#region Members

		/// <summary>
		/// The broker we will use to listen for responses with
		/// </summary>
		private MessageBroker broker;

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region Constructor

		/// <summary>
		/// Initialises an instance of the MessageToken with a reference to a MessageBroker
		/// </summary>
		/// <param name="broker"></param>
		public MessageToken(MessageBroker broker)
		{
			if (broker == null) throw new ArgumentNullException("broker");

			this.broker = broker;
		}

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region Public Methods

		/// <summary>
		/// Awaits a response to the message associated with the MessageToken
		/// </summary>
		/// <returns>A Task object which can be awaited on until a response is received to the Message</returns>
		//public async Task AwaitResponse()
		//{
			
		//}

		/// <summary>
		/// Awaits a response to the message associated with the MessageToken with a given response message
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <returns>A Task object which can be awaited on until a response is received to the Message</returns>
		//public async Task<TResult> AwaitResponse<TResult>()
		//{

		//}

		#endregion

		//////////////////////////////////////////////////////////////////////
	}
}
