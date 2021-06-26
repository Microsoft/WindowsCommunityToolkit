// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Mvvm
{
    [TestClass]
    public class Test_ObservableRecipient
    {
        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_ObservableRecipient_Activation(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var viewmodel = new SomeRecipient<int>(messenger);

            Assert.IsFalse(viewmodel.IsActivatedCheck);

            viewmodel.IsActive = true;

            Assert.IsTrue(viewmodel.IsActivatedCheck);
            Assert.IsTrue(viewmodel.CurrentMessenger.IsRegistered<SampleMessage>(viewmodel));

            viewmodel.IsActive = false;

            Assert.IsFalse(viewmodel.IsActivatedCheck);
            Assert.IsFalse(viewmodel.CurrentMessenger.IsRegistered<SampleMessage>(viewmodel));
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_ObservableRecipient_IsSame(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var viewmodel = new SomeRecipient<int>(messenger);

            Assert.AreSame(viewmodel.CurrentMessenger, messenger);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ObservableRecipient_Default()
        {
            var viewmodel = new SomeRecipient<int>();

            Assert.AreSame(viewmodel.CurrentMessenger, WeakReferenceMessenger.Default);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_ObservableRecipient_Injection(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var viewmodel = new SomeRecipient<int>(messenger);

            Assert.AreSame(viewmodel.CurrentMessenger, messenger);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_ObservableRecipient_Broadcast(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var viewmodel = new SomeRecipient<int>(messenger);

            PropertyChangedMessage<int> message = null;

            messenger.Register<PropertyChangedMessage<int>>(messenger, (r, m) => message = m);

            viewmodel.Data = 42;

            Assert.IsNotNull(message);
            Assert.AreSame(message.Sender, viewmodel);
            Assert.AreEqual(message.OldValue, 0);
            Assert.AreEqual(message.NewValue, 42);
            Assert.AreEqual(message.PropertyName, nameof(SomeRecipient<int>.Data));
        }

        public class SomeRecipient<T> : ObservableRecipient
        {
            public SomeRecipient()
            {
            }

            public SomeRecipient(IMessenger messenger)
                : base(messenger)
            {
            }

            public IMessenger CurrentMessenger => Messenger;

            private T data;

            public T Data
            {
                get => data;
                set => SetProperty(ref data, value, true);
            }

            public bool IsActivatedCheck { get; private set; }

            protected override void OnActivated()
            {
                IsActivatedCheck = true;

                Messenger.Register<SampleMessage>(this, (r, m) => { });
            }

            protected override void OnDeactivated()
            {
                base.OnDeactivated();

                IsActivatedCheck = false;
            }
        }

        public class SampleMessage
        {
        }
    }
}