using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageR.Tests
{
	/// <summary>
	/// Test cases for <see cref="MessageR.Message"/>
	/// </summary>
	[TestFixture]
	public class MessageTests
	{
		/// <summary>
		/// Checks that the Message constructor is setting an Id
		/// </summary>
		[Test]
		public void Message_IdIsSet()
		{
			Message m = new Message();
			Assert.AreNotEqual(Guid.Empty, m.Id, "Message.Id is not being set correctly");
		}

		/// <summary>
		/// Checks that the Message constructor overload for a reference message does an argument null check.
		/// </summary>
		[Test]
		public void Message_ReferenceMessageArgumentNull()
		{
			Assert.Catch<ArgumentNullException>(() =>
			{
				Message m = new Message(null);
			}, "Message constructor is not checking null on referenceMessage");
		}

		/// <summary>
		/// Checks that the Message constructor overload for a reference message does sets properties correctly
		/// </summary>
		[Test]
		public void Message_ReferenceMessagePropertiesAreSet()
		{
			Message m1 = new Message();
			Message m2 = new Message(m1);

			Assert.AreNotEqual(Guid.Empty, m1.Id, "Message1.Id is not being set correctly");
			Assert.AreNotEqual(Guid.Empty, m2.Id, "Message2.Id is not being set correctly");
			Assert.AreEqual(m1.Id, m2.ReferenceId, "Message.ReferenceId is not being set to the reference message Id");
		}

		/// <summary>
		/// Checks that the Message constructor overload for a reference message and exception does null checks.
		/// </summary>
		[Test]
		public void Message_ExceptionArgumentNull()
		{
			Assert.Catch<ArgumentNullException>(() =>
			{
				Message m = new Message(new Message(), null);
			}, "Message constructor is not checking null on exception");
		}

		/// <summary>
		/// Checks that the Message constructor overload for a reference message and exception sets properties correctly
		/// </summary>
		[Test]
		public void Message_AllPropertiesAreSet()
		{
			Message m1 = new Message();
			Exception ex = new Exception("Exception");
			Message m2 = new Message(m1, ex);

			Assert.AreNotEqual(Guid.Empty, m1.Id, "Message1.Id is not being set correctly");
			Assert.AreNotEqual(Guid.Empty, m2.Id, "Message2.Id is not being set correctly");
			Assert.AreEqual(m1.Id, m2.ReferenceId, "Message.ReferenceId is not being set to the reference message Id");
			Assert.AreEqual(ex, m2.Exception, "Message.Exception is not being set correctly");
		}
	}
}
