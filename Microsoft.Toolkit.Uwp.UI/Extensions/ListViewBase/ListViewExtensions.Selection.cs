// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <summary>
    /// Provides attached dependency properties for the <see cref="Windows.UI.Xaml.Controls.ListViewBase"/>
    /// </summary>
    public static partial class ListViewExtensions
    {
        /// <summary>
        /// Deselects the provided item.
        /// </summary>
        /// <typeparam name="T">Type of item.</typeparam>
        /// <param name="list"><see cref="ListViewBase"/></param>
        /// <param name="item">Item to deselect.</param>
        public static void DeselectItem<T>(this ListViewBase list, T item)
            where T : DependencyObject
        {
            switch (list.SelectionMode)
            {
                case ListViewSelectionMode.Single:
                    if (list.SelectedItem == item)
                    {
                        list.SelectedItem = null;
                    }

                    break;
                case ListViewSelectionMode.Multiple:
                case ListViewSelectionMode.Extended:
                    list.DeselectRange(new ItemIndexRange(list.IndexFromContainer(item), 1));
                    break;
            }
        }

        /// <summary>
        /// Deselects all items in list.
        /// </summary>
        /// <param name="list"><see cref="ListViewBase"/></param>
        public static void DeselectAll(this ListViewBase list)
        {
            switch (list.SelectionMode)
            {
                case ListViewSelectionMode.Single:
                    list.SelectedItem = null;
                    break;
                case ListViewSelectionMode.Multiple:
                case ListViewSelectionMode.Extended:
                    list.DeselectRange(new ItemIndexRange(0, (uint)list.Items.Count));
                    break;
            }
        }

        /// <summary>
        /// Selects all items in the list (or first one), if possible.
        /// </summary>
        /// <param name="list"><see cref="ListViewBase"/></param>
        public static void SelectAllSafe(this ListViewBase list)
        {
            switch (list.SelectionMode)
            {
                case ListViewSelectionMode.Single:
                    list.SelectedItem = list.Items.FirstOrDefault();
                    break;
                case ListViewSelectionMode.Multiple:
                case ListViewSelectionMode.Extended:
                    list.SelectRange(new ItemIndexRange(0, (uint)list.Items.Count));
                    break;
            }
        }
    }
}