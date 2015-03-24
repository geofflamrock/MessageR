using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MessageR
{
	/// <summary>
	/// Represents a token that a listener can use to performs actions on their listening to messages
	/// </summary>
	public class ListenerToken : IDisposable
	{
		//////////////////////////////////////////////////////////////////////

		#region Members

		/// <summary>
		/// Tracks whether the class has been disposed
		/// </summary>
		private bool isDisposed = false;

		/// <summary>
		/// Blocks used as part of the listening
		/// </summary>
		private List<IDataflowBlock> blocks = new List<IDataflowBlock>();

		/// <summary>
		/// Disposable references for block linkage
		/// </summary>
		private IDisposable disposable;

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region Properties

		/// <summary>
		/// Gets a <see cref="System.Threading.Tasks.Task"/> that represents the completion of the listening reference
		/// </summary>
		public Task Completion
		{
			get
			{
				return Task.WhenAll(blocks.Select<IDataflowBlock, Task>(dfb => dfb.Completion));
			}
		}

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region Constructor

		/// <summary>
		/// Initialises a new instance of the ListenerToken class
		/// </summary>
		internal ListenerToken(IEnumerable<IDataflowBlock> blocks, IDisposable disposable)
		{
			if (blocks == null) throw new ArgumentNullException("blocks");
			if (disposable == null) throw new ArgumentNullException("disposable");

			this.blocks.AddRange(blocks);
			this.disposable = disposable;
		}

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region Destructor

		/// <summary>
		/// Destructor, used to implement IDisposable
		/// </summary>
		~ListenerToken()
		{
			//
			// Call Dispose()
			//
			Dispose(false);
		}

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region Public Methods

		/// <summary>
		/// Stops listening on any messages
		/// </summary>
		public void StopListening()
		{
			foreach (IDataflowBlock block in blocks)
			{
				block.Complete();
			}
		}

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);

			//
			// This object will be cleaned up by the Dispose method.
			// Therefore, you should call GC.SupressFinalize to
			// take this object off the finalization queue
			// and prevent finalization code for this object
			// from executing a second time.
			//

			GC.SuppressFinalize(this);
		}

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region Private Methods

		/// <summary>
		/// Do the dispose
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
		{
			// Check to see if Dispose has already been called.
			if (!this.isDisposed)
			{
				// If disposing equals true, dispose all managed
				// and unmanaged resources.
				if (disposing)
				{
					//
					// Release any resources required
					//
					StopListening();
					disposable.Dispose();
				}
			}

			this.isDisposed = true;
		}

		#endregion
		
		//////////////////////////////////////////////////////////////////////
	}
}
