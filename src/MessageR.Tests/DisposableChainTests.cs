using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageR.Tests
{
	[TestFixture]
	public class DisposableChainTests
	{
		/// <summary>
		/// Checks that the AddDisposable method checks for nulls
		/// </summary>
		[Test]
		public void DisposableChain_AddDisposableArgumentNullChecks()
		{
			Assert.Catch<ArgumentNullException>(() =>
			{
				DisposableChain disposableChain = new DisposableChain();
				disposableChain.AddDisposable(null);
			});			
		}

		/// <summary>
		/// Checks that the AddDisposables method checks for nulls
		/// </summary>
		[Test]
		public void DisposableChain_AddDisposablesArgumentNullChecks()
		{
			Assert.Catch<ArgumentNullException>(() =>
			{
				DisposableChain disposableChain = new DisposableChain();
				disposableChain.AddDisposables(null);
			});
		}

		/// <summary>
		/// Checks that the AddDisposable method returns the same chain
		/// </summary>
		[Test]
		public void DisposableChain_AddDisposableReturnsSameObject()
		{
			DisposableChain disposableChain = new DisposableChain();
			Mock<IDisposable> disposable = new Mock<IDisposable>();
			DisposableChain returnedChain = disposableChain.AddDisposable(disposable.Object);

			Assert.AreEqual(disposableChain, returnedChain);
		}

		/// <summary>
		/// Checks that the AddDisposables method checks for nulls
		/// </summary>
		[Test]
		public void DisposableChain_AddDisposablesReturnsSameObject()
		{
			DisposableChain disposableChain = new DisposableChain();
			Mock<IDisposable> disposable = new Mock<IDisposable>();
			Mock<IDisposable> disposable2 = new Mock<IDisposable>();
			DisposableChain returnedChain = disposableChain.AddDisposables(new IDisposable[] { disposable.Object, disposable2.Object });

			Assert.AreEqual(disposableChain, returnedChain);
		}

		/// <summary>
		/// Checks that the DisposableChain calls dispose on all elements added to it
		/// </summary>
		[Test]
		public void DisposableChain_DisposesAllObjects()
		{
			DisposableChain disposableChain = new DisposableChain();
			Mock<IDisposable> disposable = new Mock<IDisposable>();
			Mock<IDisposable> disposable2 = new Mock<IDisposable>();
			disposable.Setup(d => d.Dispose()).Verifiable();
			disposable2.Setup(d => d.Dispose()).Verifiable();
			disposableChain.AddDisposable(disposable.Object);
			disposableChain.AddDisposable(disposable2.Object);
			disposableChain.Dispose();
			disposable.VerifyAll();
			disposable2.VerifyAll();
		}
	}
}
