// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// <see cref="DataTemplateSelector"/> used by <see cref="TabbedCommandBar"/> for determining the style of normal vs. contextual <see cref="TabbedCommandBarItem"/>s.
    /// </summary>
    public class TabbedCommandBarItemTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the <see cref="Style"/> of a normal <see cref="TabbedCommandBarItem"/>.
        /// </summary>
        public DataTemplate Normal { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> of a contextual <see cref="TabbedCommandBarItem"/>.
        /// </summary>
        public DataTemplate Contextual { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item is TabbedCommandBarItem t && t.IsContextual ? Contextual : Normal;
        }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}