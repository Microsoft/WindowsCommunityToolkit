// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI;

namespace Microsoft.Toolkit.Uwp.SampleApp.Common
{
    public static class Constants
    {
        public const string TestBackgroundTaskName = "TestBackgroundTaskName";

        public static readonly Color ApplicationBackgroundColor = Color.FromArgb(255, 51, 51, 51);

        public static readonly Uri Square44x44Logo = new Uri("ms-appx:///Assets/Square44x44Logo.png");
        public static readonly Uri Square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.png");
        public static readonly Uri Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.png");
        public static readonly Uri Square310x310Logo = new Uri("ms-appx:///Assets/Square310x310Logo.png");

        public static string ApplicationDisplayName { get; set; }
    }
}