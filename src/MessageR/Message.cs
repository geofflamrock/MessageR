using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageR
{
	/// <summary>
	/// The message class provides the basis for all messages within the MessageR infrastructure
	/// </summary>
	public class Message
	{
		//////////////////////////////////////////////////////////////////////

		#region Properties

		/// <summary>
		/// Gets the type of the message
		/// </summary>
		//public string Type
		//{
		//	get;
		//	private set;
		//}

		/// <summary>
		/// Gets the id of the message
		/// </summary>
		public Guid Id
		{
			get;
			private set;
		}

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region Constructors

		/// <summary>
		/// Initialises a new instance of the Message class
		/// </summary>
		public Message()
		{
			this.Id = Guid.NewGuid();
		}

		#endregion

		//////////////////////////////////////////////////////////////////////
	}
}
