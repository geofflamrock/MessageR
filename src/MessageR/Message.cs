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
		/// Gets the id of the message
		/// </summary>
		public Guid Id
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the id of the message this message relates to
		/// </summary>
		public Guid ReferenceId
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets any exception associated with the message
		/// </summary>
		public Exception Exception
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

		/// <summary>
		/// Initialises a new instance of the Message class, referencing another message
		/// </summary>
		/// <param name="referenceMessage"></param>
		public Message(Message referenceMessage)
			: this()
		{
			if (referenceMessage == null) throw new ArgumentNullException("referenceMessage");

			ReferenceId = referenceMessage.Id;
		}

		/// <summary>
		/// Initialises a new instance of the Message class with a referenced message and an exception
		/// </summary>
		/// <param name="exception"></param>
		public Message(Message referenceMessage, Exception exception)
			: this(referenceMessage)
		{
			if (exception == null) throw new ArgumentNullException("exception");

			this.Exception = exception;
		}

		#endregion

		//////////////////////////////////////////////////////////////////////
	}
}
