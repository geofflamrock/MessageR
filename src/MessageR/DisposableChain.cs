using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageR
{
	/// <summary>
	/// Represents a chain of disposable objects that will be disposed together
	/// </summary>
	internal class DisposableChain : IDisposable
	{
		//////////////////////////////////////////////////////////////////////

		#region Members

		/// <summary>
		/// Tracks whether the class has been disposed
		/// </summary>
		private bool isDisposed = false;

		/// <summary>
		/// List of objects to be disposed
		/// </summary>
		private List<IDisposable> disposables = new List<IDisposable>();

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region Constructor

		/// <summary>
		/// Initialises a new instance of the DisposableChain class
		/// </summary>
		public DisposableChain()
		{

		}

		#endregion

		//////////////////////////////////////////////////////////////////////

		#region Destructor

		/// <summary>
		/// Destructor, used to implement IDisposable
		/// </summary>
		~DisposableChain()
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
		/// Adds an IDisposable object to the chain, and returns the chain itself
		/// </summary>
		/// <returns></returns>
		public DisposableChain AddDisposable(IDisposable disposable)
		{
			disposables.Add(disposable);
			return this;
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
					foreach (IDisposable disposable in disposables)
					{
						disposable.Dispose();
					}
				}
			}

			this.isDisposed = true;
		}

		#endregion

		///////////////////////////////////////////////////////////////////////
	}
}
