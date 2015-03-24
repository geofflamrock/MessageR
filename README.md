# MessageR
A simple but extensible message broker for .NET

Provides message passing between loosely coupled components (often referred to as an Actor pattern).

**Please note that this is a work in progress and the API may change at any time before hitting a 1.0 stable version.**

*Features*

* Generic typed methods for compile time safety
* Supports asynchronous programming model (async/await)
* Send through multiple different protocols at once (via different dispatchers)

*Usage*

Construct `MessageBroker` or use singleton `Instance` property if desired. 

    MessageBroker broker = new MessageBroker();

Add a dispatcher to the broker. To broker messages in-process, use the `SimpleMessageDispatcher`.

    broker.AddDispatcher(new SimpleMessageDispatcher(broker));

To send a message, create a class inherited from the `Message` class and call one of `Send` or `SendAsync` on the broker. To send a generic payload use the generic `Message<T>` class.

    public class TestMessage : Message
    {	    
    }
    
    TestMessage m = new TestMessage();
    broker.Send(m);

To listen to a message, use the Listen methods on the broker, passing a handler in the form of either an `Action<T>` delegate or `Func<T, Task>` delegate (to handle the message in an asynchronous fashion).

    broker.Listen<TestMessage>(m => Console.WriteLine("Test message received");

To wait for a response to a message, use the `AwaitResponse` method on the returned object from the `Send` method.

    TestMessage m = new TestMessage();
    MessageToken token = broker.Send(m);
    await TestResponseMessage response = token.AwaitResponse<TestResponseMessage>();

*Note that `AwaitResponse` only supports await at the moment, there is no synchronous equivalent. This may be changed in the future.*

**More docs to come.**