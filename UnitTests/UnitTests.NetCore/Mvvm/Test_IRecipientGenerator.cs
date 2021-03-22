// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        public void Test_Messenger_UnregisterRecipientWithMessageType(Type type)
        {
            var messenger = (IMessenger)Activator.CreateInstance(type);
            var recipient = new object();

            messenger.Unregister<MessageA>(recipient);
        }

        public sealed class RecipientWithSomeMessages :
            IRecipient<MessageA>,
            IRecipient<MessageB>
        {
            public void Receive(MessageA message)
            {
            }

            public void Receive(MessageB message)
            {
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
