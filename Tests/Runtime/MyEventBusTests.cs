using System;
using NUnit.Framework;

namespace Dreamy.Core.Tests
{
    public sealed class MyEventBusTests
    {
        [SetUp]
        public void SetUp()
        {
            MyEventBus<TestEvent>.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            MyEventBus<TestEvent>.Clear();
        }

        [Test]
        public void Raise_WhenNested_DoesNotApplyPendingChangesUntilOutermostRaiseCompletes()
        {
            int firstListenerCalls = 0;
            int deferredListenerCalls = 0;
            EventBinding<TestEvent> deferredBinding =
                new EventBinding<TestEvent>(() => deferredListenerCalls++);

            EventBinding<TestEvent> firstBinding = new EventBinding<TestEvent>(@event =>
            {
                firstListenerCalls++;
                if (@event.Depth == 0)
                {
                    MyEventBus<TestEvent>.Register(deferredBinding);
                    MyEventBus<TestEvent>.Raise(new TestEvent { Depth = 1 });
                }
            });

            MyEventBus<TestEvent>.Register(firstBinding);

            Assert.DoesNotThrow(() =>
                MyEventBus<TestEvent>.Raise(new TestEvent { Depth = 0 }));
            Assert.That(firstListenerCalls, Is.EqualTo(2));
            Assert.That(deferredListenerCalls, Is.Zero);

            MyEventBus<TestEvent>.Raise(new TestEvent { Depth = 2 });

            Assert.That(deferredListenerCalls, Is.EqualTo(1));
        }

        [Test]
        public void Clear_RemovesPendingChangesCreatedDuringRaise()
        {
            int deferredListenerCalls = 0;
            EventBinding<TestEvent> deferredBinding =
                new EventBinding<TestEvent>(() => deferredListenerCalls++);
            EventBinding<TestEvent> clearingBinding = new EventBinding<TestEvent>(() =>
            {
                MyEventBus<TestEvent>.Register(deferredBinding);
                MyEventBus<TestEvent>.Clear();
            });

            MyEventBus<TestEvent>.Register(clearingBinding);
            MyEventBus<TestEvent>.Raise(default);
            MyEventBus<TestEvent>.Raise(default);

            Assert.That(deferredListenerCalls, Is.Zero);
        }

        private struct TestEvent : IEvent
        {
            public int Depth;
        }
    }
}
