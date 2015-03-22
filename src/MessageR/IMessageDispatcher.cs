using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageR
{
	/// <summary>
	/// Represents a component which can dispatch a message
	/// </summary>
	public interface IMessageDispatcher
	{
		/// <summary>
		/// Dispatches a message
		/// </summary>
		/// <param name="message">The message to dispatch</param>
		/// <returns>A task object which can be awaited</returns>
		Task Dispatch(Message message);

		/// <summary>
		/// Dispatches a message with an expected response type of T
		/// </summary>
		/// <typeparam name="T">The type of the response to the message</typeparam>
		/// <param name="message">The message to dispatch</param>
		/// <returns>A task object with type T which can be awaited</returns>
		Task<TResult> Dispatch<T, TResult>(Message<T> message);
	}
}
