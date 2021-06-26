// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Toolkit.Uwp.Helpers;

namespace UnitTests.Helpers
{
    [TestClass]
    public class Test_WeakEventListener
    {
        public class SampleClass
        {
            public event EventHandler<EventArgs> Raisevent;

            public void DoSomething()
            {
                OnRaiseEvent();
            }

            protected virtual void OnRaiseEvent()
            {
                Raisevent?.Invoke(this, EventArgs.Empty);
            }
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_WeakEventListener_Events()
        {
            bool isOnEventTriggered = false;
            bool isOnDetachTriggered = false;

            SampleClass sample = new SampleClass();

            WeakEventListener<SampleClass, object, EventArgs> weak = new WeakEventListener<SampleClass, object, EventArgs>(sample);
            weak.OnEventAction = (instance, source, eventArgs) => { isOnEventTriggered = true; };
            weak.OnDetachAction = (listener) => { isOnDetachTriggered = true; };

            sample.Raisevent += weak.OnEvent;

            sample.DoSomething();
            Assert.IsTrue(isOnEventTriggered);

            weak.Detach();
            Assert.IsTrue(isOnDetachTriggered);
        }
    }
}