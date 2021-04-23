// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable CS0618

using System;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Mvvm
{
    [TestClass]
    public partial class Test_IRecipientGenerator
    {
        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_IRecipientGenerator_GeneratedRegistration()
        {
            var messenger = new StrongReferenceMessenger();
            var recipient = new RecipientWithSomeMessages();

            var messageA = new MessageA();
            var messageB = new MessageB();

            Action<IMessenger, object, int> registrator = Microsoft.Toolkit.Mvvm.Messaging.__Internals.__IMessengerExtensions.CreateAllMessagesRegistratorWithToken<int>(recipient);

            registrator(messenger, recipient, 42);

            Assert.IsTrue(messenger.IsRegistered<MessageA, int>(recipient, 42));
            Assert.IsTrue(messenger.IsRegistered<MessageB, int>(recipient, 42));

            Assert.IsNull(recipient.A);
            Assert.IsNull(recipient.B);

            messenger.Send(messageA, 42);

            Assert.AreSame(recipient.A, messageA);
            Assert.IsNull(recipient.B);

            messenger.Send(messageB, 42);

            Assert.AreSame(recipient.A, messageA);
            Assert.AreSame(recipient.B, messageB);
        }

        public sealed class RecipientWithSomeMessages :
            IRecipient<MessageA>,
            IRecipient<MessageB>
        {
            public MessageA A { get; private set; }

            public MessageB B { get; private set; }

            public void Receive(MessageA message)
            {
                A = message;
            }

            public void Receive(MessageB message)
            {
                B = message;
            }
        }

        public sealed class MessageA
        {
        }

        public sealed class MessageB
        {
        }
    }
}
