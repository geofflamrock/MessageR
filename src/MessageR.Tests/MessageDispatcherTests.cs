using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageR.Tests
{
	/// <summary>
	/// Test cases for <see cref="MessageR.MessageDispatcher"/>
	/// </summary>
	[TestFixture]
	public class MessageDispatcherTests
	{
		/// <summary>
		/// Checks for an argument null check for the MessageBroker
		/// </summary>
		[Test]
		public void MessageDispatcher_NullMessageBroker()
		{
			Assert.Catch<ArgumentNullException>(() =>
				{
					new MessageDispatcher(null);
				}, "MessageDispatcher is not checking for null MessageBroker");
		}

		/// <summary>
		/// Checks for an argument null check for the Message in the Dispatch method
		/// </summary>
		[Test]
		public void MessageDispatcher_Dispatch_NullMessage()
		{
			Assert.Catch<ArgumentNullException>(() =>
			{
				MessageBroker broker = new MessageBroker();
				MessageDispatcher dispatcher = new MessageDispatcher(broker);
				dispatcher.Dispatch(null);
			}, "MessageDispatcher is not checking for null Message in Dispatch method");
		}

		/// <summary>
		/// Checks for an argument null check for the Message in the DispatchAsync method
		/// </summary>
		[Test]
		public async void MessageDispatcher_DispatchAsync_NullMessage()
		{
			await Task.Run(() =>
			{
				Assert.Catch<ArgumentNullException>(async () =>
				{
					MessageBroker broker = new MessageBroker();
					MessageDispatcher dispatcher = new MessageDispatcher(broker);
					await dispatcher.DispatchAsync(null);
				}, "MessageDispatcher is not checking for null Message in DispatchAsync method");
			});			
		}

		/// <summary>
		/// Checks that the Dispatch method is passing the message through to the broker
		/// </summary>
		[Test]
		public void MessageDispatcher_Dispatch_PassesMessageToBroker()
		{
			Message m = new Message();
			Mock<IMessageBroker> broker = new Mock<IMessageBroker>();
			broker.Setup(mb => mb.Receive(It.IsAny<Message>())).Verifiable();
			MessageDispatcher dispatcher = new MessageDispatcher(broker.Object);
			dispatcher.Dispatch(m);
			broker.VerifyAll();
		}

		/// <summary>
		/// Checks that the DispatchAsync method is passing the message through to the broker
		/// </summary>
		[Test]
		public async void MessageDispatcher_DispatchAsync_PassesMessageToBroker()
		{
			Message m = new Message();
			Mock<IMessageBroker> broker = new Mock<IMessageBroker>();
			broker.Setup(mb => mb.ReceiveAsync(It.IsAny<Message>())).ReturnsAsync(true).Verifiable();
			MessageDispatcher dispatcher = new MessageDispatcher(broker.Object);
			await dispatcher.DispatchAsync(m);
			broker.VerifyAll();
		}
	}
}
