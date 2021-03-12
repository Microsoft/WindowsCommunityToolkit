// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <summary>
    /// Provides the Command attached dependency property for the <see cref="Windows.UI.Xaml.Controls.ListViewBase"/>
    /// </summary>
    public static partial class ListViewExtensions
    {
        /// <summary>
        /// Attached <see cref="DependencyProperty"/> for binding an <see cref="System.Windows.Input.ICommand"/> to handle ListViewBase Item interaction by means of <see cref="Windows.UI.Xaml.Controls.ListViewBase"/> ItemClick event. ListViewBase IsItemClickEnabled must be set to true.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ListViewExtensions), new PropertyMetadata(null, OnCommandPropertyChanged));

        /// <summary>
        /// Gets the <see cref="ICommand"/> associated with the specified <see cref="ListViewBase"/>
        /// </summary>
        /// <param name="obj">The <see cref="Windows.UI.Xaml.Controls.ListViewBase"/> to get the associated <see cref="ICommand"/> from</param>
        /// <returns>The <see cref="ICommand"/> associated with the <see cref="ListViewBase"/></returns>
        public static ICommand GetCommand(ListViewBase obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        /// <summary>
        /// Sets the <see cref="ICommand"/> associated with the specified <see cref="ListViewBase"/>
        /// </summary>
        /// <param name="obj">The <see cref="Windows.UI.Xaml.Controls.ListViewBase"/> to associate the <see cref="ICommand"/> with</param>
        /// <param name="value">The <see cref="ICommand"/> for binding to the <see cref="ListViewBase"/></param>
        public static void SetCommand(ListViewBase obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        private static void OnCommandPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var listViewBase = sender as ListViewBase;

            if (listViewBase == null)
            {
                return;
            }

            var oldCommand = args.OldValue as ICommand;
            if (oldCommand != null)
            {
                listViewBase.ItemClick -= OnListViewBaseItemClick;
            }

            var newCommand = args.NewValue as ICommand;
            if (newCommand != null)
            {
                listViewBase.ItemClick += OnListViewBaseItemClick;
            }
        }

        private static void OnListViewBaseItemClick(object sender, ItemClickEventArgs e)
        {
            var listViewBase = sender as ListViewBase;
            var command = GetCommand(listViewBase);
            if (listViewBase == null || command == null)
            {
                return;
            }

            if (command.CanExecute(e.ClickedItem))
            {
                command.Execute(e.ClickedItem);
            }
        }
    }
}
