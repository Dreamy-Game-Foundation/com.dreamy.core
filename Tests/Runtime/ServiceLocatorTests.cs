using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Dreamy.Core.Tests
{
    public sealed class ServiceLocatorTests
    {
        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Clear();
        }

        [Test]
        public void Register_WhenDeferredCallbackThrows_StillInvokesRemainingCallbacks()
        {
            bool secondCallbackCalled = false;
            ServiceLocator.Get<ITestService>(_ => throw new InvalidOperationException("Expected test exception."));
            ServiceLocator.Get<ITestService>(_ => secondCallbackCalled = true);

            LogAssert.Expect(LogType.Exception, "InvalidOperationException: Expected test exception.");
            Assert.DoesNotThrow(() => ServiceLocator.Register<ITestService>(new TestService()));

            Assert.That(secondCallbackCalled, Is.True);
        }

        private interface ITestService
        {
        }

        private sealed class TestService : ITestService
        {
        }
    }
}
