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
		void Dispatch(Message message);

		/// <summary>
		/// Dispatches a message
		/// </summary>
		/// <param name="message">The message to dispatch</param>
		/// <returns>A task object which can be awaited</returns>
		Task DispatchAsync(Message message);
	}
}
