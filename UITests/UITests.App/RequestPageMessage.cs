// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Mvvm.Messaging.Messages;

namespace UITests.App
{
    public sealed class RequestPageMessage : AsyncRequestMessage<bool>
    {
        public RequestPageMessage(string name)
        {
            PageName = name;
        }

        public string PageName { get; }
    }
}