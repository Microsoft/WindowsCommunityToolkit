// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.Input.GazeControls
{
    internal class KeyboardPage
    {
        public FrameworkElement Page { get; set; }

        public KeyboardPage Parent { get; set; }

        public List<string> ChildrenNames { get; set; }

        public List<KeyboardPage> Children { get; set; }

        public KeyboardPage CurrentChild { get; set; }

        public KeyboardPage PrevChild { get; set; }

        public KeyboardPage(FrameworkElement page, KeyboardPage parent)
        {
            Page = page;
            Parent = parent;
            ChildrenNames = new List<string>();
            Children = new List<KeyboardPage>();
        }
    }
}
