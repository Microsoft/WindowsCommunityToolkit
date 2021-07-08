// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.SampleApp.Common
{
    /// <summary>
    /// Command with a name for Sample Shell Commands.
    /// </summary>
    public class SampleCommand : DelegateCommand
    {
        public string Label { get; set; }

        public SampleCommand(string name, Action action)
            : base(action)
        {
            Label = name;
        }
    }
}