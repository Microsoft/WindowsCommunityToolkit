// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Arguments relating to a change of Editor
    /// </summary>
    public class EditorChangedArgs
    {
        internal EditorChangedArgs()
        {
        }

        /// <summary>
        /// Gets the old Instance that is being Replaced
        /// </summary>
        public RichEditBox Old { get; internal set; }

        /// <summary>
        /// Gets the new Instance that is being Set
        /// </summary>
        public RichEditBox New { get; internal set; }
    }
}