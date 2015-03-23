using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageR.ConsoleTest
{
	class Program
	{
		private class TestMessage : Message
		{
			public int Variable
			{
				get;
				set;
			}
		}

		static void Main(string[] args)
		{
			MessageBroker broker = new MessageBroker();
			broker.AddDispatcher(new SimpleMessageDispatcher(broker));

			TestMessage test = new TestMessage() { Variable = 1 };
			bool run = true;
			broker.Listen<TestMessage>(t => 
			{
				Console.Out.WriteLine(t.Variable);
				run = false;
			});
			broker.Send(test);

			while(run)
			{
				Thread.Sleep(1000);
			}

			Console.ReadLine();
		}
	}
}
