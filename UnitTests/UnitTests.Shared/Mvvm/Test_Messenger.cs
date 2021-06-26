// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Mvvm
{
    [TestClass]
    public partial class Test_Messenger
    {
        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_UnregisterRecipientWithMessageType(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var recipient = new object();

            messenger.Unregister<MessageA>(recipient);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_UnregisterRecipientWithMessageTypeAndToken(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var recipient = new object();

            messenger.Unregister<MessageA, string>(recipient, nameof(MessageA));
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_UnregisterRecipientWithToken(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var recipient = new object();

            messenger.UnregisterAll(recipient, nameof(MessageA));
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_UnregisterRecipientWithRecipient(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var recipient = new object();

            messenger.UnregisterAll(recipient);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_RegisterAndUnregisterRecipientWithMessageType(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var recipient = new object();

            messenger.Register<MessageA>(recipient, (r, m) => { });

            messenger.Unregister<MessageA>(recipient);

            Assert.IsFalse(messenger.IsRegistered<MessageA>(recipient));
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_RegisterAndUnregisterRecipientWithMessageTypeAndToken(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var recipient = new object();

            messenger.Register<MessageA, string>(recipient, nameof(MessageA), (r, m) => { });

            messenger.Unregister<MessageA, string>(recipient, nameof(MessageA));

            Assert.IsFalse(messenger.IsRegistered<MessageA, string>(recipient, nameof(MessageA)));
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_RegisterAndUnregisterRecipientWithToken(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var recipient = new object();

            messenger.Register<MessageA, string>(recipient, nameof(MessageA), (r, m) => { });

            messenger.UnregisterAll(recipient, nameof(MessageA));

            Assert.IsFalse(messenger.IsRegistered<MessageA, string>(recipient, nameof(MessageA)));
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_RegisterAndUnregisterRecipientWithRecipient(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var recipient = new object();

            messenger.Register<MessageA, string>(recipient, nameof(MessageA), (r, m) => { });

            messenger.UnregisterAll(recipient);

            Assert.IsFalse(messenger.IsRegistered<MessageA, string>(recipient, nameof(MessageA)));
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_IsRegistered_Register_Send_UnregisterOfTMessage_WithNoToken(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            object a = new object();

            Assert.IsFalse(messenger.IsRegistered<MessageA>(a));

            object recipient = null;
            string result = null;

            messenger.Register<MessageA>(a, (r, m) =>
            {
                recipient = r;
                result = m.Text;
            });

            Assert.IsTrue(messenger.IsRegistered<MessageA>(a));

            messenger.Send(new MessageA { Text = nameof(MessageA) });

            Assert.AreSame(recipient, a);
            Assert.AreEqual(result, nameof(MessageA));

            messenger.Unregister<MessageA>(a);

            Assert.IsFalse(messenger.IsRegistered<MessageA>(a));

            recipient = null;
            result = null;

            messenger.Send(new MessageA { Text = nameof(MessageA) });

            Assert.IsNull(recipient);
            Assert.IsNull(result);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_IsRegistered_Register_Send_UnregisterRecipient_WithNoToken(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            object a = new object();

            Assert.IsFalse(messenger.IsRegistered<MessageA>(a));

            string result = null;
            messenger.Register<MessageA>(a, (r, m) => result = m.Text);

            Assert.IsTrue(messenger.IsRegistered<MessageA>(a));

            messenger.Send(new MessageA { Text = nameof(MessageA) });

            Assert.AreEqual(result, nameof(MessageA));

            messenger.UnregisterAll(a);

            Assert.IsFalse(messenger.IsRegistered<MessageA>(a));

            result = null;
            messenger.Send(new MessageA { Text = nameof(MessageA) });

            Assert.IsNull(result);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_IsRegistered_Register_Send_UnregisterOfTMessage_WithToken(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            object a = new object();

            Assert.IsFalse(messenger.IsRegistered<MessageA>(a));

            string result = null;
            messenger.Register<MessageA, string>(a, nameof(MessageA), (r, m) => result = m.Text);

            Assert.IsTrue(messenger.IsRegistered<MessageA, string>(a, nameof(MessageA)));

            messenger.Send(new MessageA { Text = nameof(MessageA) }, nameof(MessageA));

            Assert.AreEqual(result, nameof(MessageA));

            messenger.Unregister<MessageA, string>(a, nameof(MessageA));

            Assert.IsFalse(messenger.IsRegistered<MessageA, string>(a, nameof(MessageA)));

            result = null;
            messenger.Send(new MessageA { Text = nameof(MessageA) }, nameof(MessageA));

            Assert.IsNull(result);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_DuplicateRegistrationWithMessageType(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var recipient = new object();

            messenger.Register<MessageA>(recipient, (r, m) => { });

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                messenger.Register<MessageA>(recipient, (r, m) => { });
            });
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_DuplicateRegistrationWithMessageTypeAndToken(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var recipient = new object();

            messenger.Register<MessageA, string>(recipient, nameof(MessageA), (r, m) => { });

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                messenger.Register<MessageA, string>(recipient, nameof(MessageA), (r, m) => { });
            });
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_IRecipient_NoMessages(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var recipient = new RecipientWithNoMessages();

            messenger.RegisterAll(recipient);

            // We just need to verify we got here with no errors, this
            // recipient has no declared handlers so there's nothing to do
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_IRecipient_SomeMessages_NoToken(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var recipient = new RecipientWithSomeMessages();

            messenger.RegisterAll(recipient);

            Assert.IsTrue(messenger.IsRegistered<MessageA>(recipient));
            Assert.IsTrue(messenger.IsRegistered<MessageB>(recipient));

            Assert.AreEqual(recipient.As, 0);
            Assert.AreEqual(recipient.Bs, 0);

            messenger.Send<MessageA>();

            Assert.AreEqual(recipient.As, 1);
            Assert.AreEqual(recipient.Bs, 0);

            messenger.Send<MessageB>();

            Assert.AreEqual(recipient.As, 1);
            Assert.AreEqual(recipient.Bs, 1);

            messenger.UnregisterAll(recipient);

            Assert.IsFalse(messenger.IsRegistered<MessageA>(recipient));
            Assert.IsFalse(messenger.IsRegistered<MessageB>(recipient));
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_IRecipient_SomeMessages_WithToken(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var recipient = new RecipientWithSomeMessages();
            var token = nameof(Test_Messenger_IRecipient_SomeMessages_WithToken);

            messenger.RegisterAll(recipient, token);

            Assert.IsTrue(messenger.IsRegistered<MessageA, string>(recipient, token));
            Assert.IsTrue(messenger.IsRegistered<MessageB, string>(recipient, token));

            Assert.IsFalse(messenger.IsRegistered<MessageA>(recipient));
            Assert.IsFalse(messenger.IsRegistered<MessageB>(recipient));

            Assert.AreEqual(recipient.As, 0);
            Assert.AreEqual(recipient.Bs, 0);

            messenger.Send<MessageB, string>(token);

            Assert.AreEqual(recipient.As, 0);
            Assert.AreEqual(recipient.Bs, 1);

            messenger.Send<MessageA, string>(token);

            Assert.AreEqual(recipient.As, 1);
            Assert.AreEqual(recipient.Bs, 1);

            messenger.UnregisterAll(recipient, token);

            Assert.IsFalse(messenger.IsRegistered<MessageA>(recipient));
            Assert.IsFalse(messenger.IsRegistered<MessageB>(recipient));
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_RegisterWithTypeParameter(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var recipient = new RecipientWithNoMessages { Number = 42 };

            int number = 0;

            messenger.Register<RecipientWithNoMessages, MessageA>(recipient, (r, m) => number = r.Number);

            messenger.Send<MessageA>();

            Assert.AreEqual(number, 42);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger), false)]
        [DataRow(typeof(WeakReferenceMessenger), true)]
        public void Test_Messenger_Collect_Test(Type type, bool isWeak)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);

            WeakReference weakRecipient;

            void Test()
            {
                var recipient = new RecipientWithNoMessages { Number = 42 };
                weakRecipient = new WeakReference(recipient);

                messenger.Register<MessageA>(recipient, (r, m) => { });

                Assert.IsTrue(messenger.IsRegistered<MessageA>(recipient));
                Assert.IsTrue(weakRecipient.IsAlive);

                GC.KeepAlive(recipient);
            }

            Test();

            GC.Collect();

            Assert.AreEqual(!isWeak, weakRecipient.IsAlive);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_Reset(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var recipient = new RecipientWithSomeMessages();

            messenger.RegisterAll(recipient);

            Assert.IsTrue(messenger.IsRegistered<MessageA>(recipient));
            Assert.IsTrue(messenger.IsRegistered<MessageB>(recipient));

            messenger.Reset();

            Assert.IsFalse(messenger.IsRegistered<MessageA>(recipient));
            Assert.IsFalse(messenger.IsRegistered<MessageB>(recipient));
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_Default(Type type)
        {
            PropertyInfo defaultInfo = type.GetProperty("Default");

            var default1 = defaultInfo!.GetValue(null);
            var default2 = defaultInfo!.GetValue(null);

            Assert.IsNotNull(default1);
            Assert.IsNotNull(default2);
            Assert.AreSame(default1, default2);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_Cleanup(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var recipient = new RecipientWithSomeMessages();

            messenger.Register<MessageA>(recipient);

            Assert.IsTrue(messenger.IsRegistered<MessageA>(recipient));

            void Test()
            {
                var recipient2 = new RecipientWithSomeMessages();

                messenger.Register<MessageB>(recipient2);

                Assert.IsTrue(messenger.IsRegistered<MessageB>(recipient2));

                GC.KeepAlive(recipient2);
            }

            Test();

            GC.Collect();

            // Here we just check that calling Cleanup doesn't alter the state
            // of the messenger. This method shouldn't really do anything visible
            // to consumers, it's just a way for messengers to compact their data.
            messenger.Cleanup();

            Assert.IsTrue(messenger.IsRegistered<MessageA>(recipient));
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        [DataRow(typeof(StrongReferenceMessenger))]
        [DataRow(typeof(WeakReferenceMessenger))]
        public void Test_Messenger_ManyRecipients(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);

            void Test()
            {
                var recipients = Enumerable.Range(0, 512).Select(_ => new RecipientWithSomeMessages()).ToArray();

                foreach (var recipient in recipients)
                {
                    messenger.RegisterAll(recipient);
                }

                foreach (var recipient in recipients)
                {
                    Assert.IsTrue(messenger.IsRegistered<MessageA>(recipient));
                    Assert.IsTrue(messenger.IsRegistered<MessageB>(recipient));
                }

                messenger.Send<MessageA>();
                messenger.Send<MessageB>();
                messenger.Send<MessageB>();

                foreach (var recipient in recipients)
                {
                    Assert.AreEqual(recipient.As, 1);
                    Assert.AreEqual(recipient.Bs, 2);
                }

                foreach (ref var recipient in recipients.AsSpan())
                {
                    recipient = null;
                }
            }

            Test();

            GC.Collect();

            // Just invoke a final cleanup to improve coverage, this is unrelated to this test in particular
            messenger.Cleanup();
        }

        public sealed class RecipientWithNoMessages
        {
            public int Number { get; set; }
        }

        public sealed class RecipientWithSomeMessages :
            IRecipient<MessageA>,
            IRecipient<MessageB>,
            ICloneable
        {
            public int As { get; private set; }

            public void Receive(MessageA message)
            {
                As++;
            }

            public int Bs { get; private set; }

            public void Receive(MessageB message)
            {
                Bs++;
            }

            // We also add the ICloneable interface to test that the message
            // interfaces are all handled correctly even when inteleaved
            // by other unrelated interfaces in the type declaration.
            public object Clone() => throw new NotImplementedException();
        }

        public sealed class MessageA
        {
            public string Text { get; set; }
        }

        public sealed class MessageB
        {
        }
    }
}