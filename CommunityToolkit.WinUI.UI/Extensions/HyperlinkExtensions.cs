// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Documents;

namespace CommunityToolkit.WinUI.UI
{
    /// <summary>
    /// Provides attached dependency properties for the <see cref="Microsoft.UI.Xaml.Documents.Hyperlink"/> content element that allows
    /// it to invoke a <see cref="ICommand"/> when clicked
    /// </summary>
    public static class HyperlinkExtensions
    {
        /// <summary>
        /// Attached <see cref="DependencyProperty"/> for binding an <see cref="ICommand"/> instance to a <see cref="Microsoft.UI.Xaml.Documents.Hyperlink"/>
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(Hyperlink), new PropertyMetadata(null, OnCommandPropertyChanged));

        /// <summary>
        /// Attached <see cref="DependencyProperty"/> for binding a command parameter to a <see cref="Microsoft.UI.Xaml.Documents.Hyperlink"/>
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(Hyperlink), new PropertyMetadata(null));

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance associated with the specified <see cref="Microsoft.UI.Xaml.Documents.Hyperlink"/>
        /// </summary>
        /// <param name="obj">The <see cref="Microsoft.UI.Xaml.Documents.Hyperlink"/> from which to get the associated <see cref="ICommand"/> instance</param>
        /// <returns>The <see cref="ICommand"/> instance associated with the <see cref="Microsoft.UI.Xaml.Documents.Hyperlink"/> or null</returns>
        public static ICommand GetCommand(Microsoft.UI.Xaml.Documents.Hyperlink obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        /// <summary>
        /// Sets the <see cref="ICommand"/> instance associated with the specified <see cref="Microsoft.UI.Xaml.Documents.Hyperlink"/>
        /// </summary>
        /// <param name="obj">The <see cref="Microsoft.UI.Xaml.Documents.Hyperlink"/> to associated the <see cref="ICommand"/> instance to</param>
        /// <param name="value">The <see cref="ICommand"/> instance to bind to the <see cref="Microsoft.UI.Xaml.Documents.Hyperlink"/></param>
        public static void SetCommand(Microsoft.UI.Xaml.Documents.Hyperlink obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets the <see cref="CommandProperty"/> instance associated with the specified <see cref="Microsoft.UI.Xaml.Documents.Hyperlink"/>
        /// </summary>
        /// <param name="obj">The <see cref="Microsoft.UI.Xaml.Documents.Hyperlink"/> from which to get the associated <see cref="CommandProperty"/> value</param>
        /// <returns>The <see cref="CommandProperty"/> value associated with the <see cref="Microsoft.UI.Xaml.Documents.Hyperlink"/> or null</returns>
        public static object GetCommandParameter(Microsoft.UI.Xaml.Documents.Hyperlink obj)
        {
            return obj.GetValue(CommandParameterProperty);
        }

        /// <summary>
        /// Sets the <see cref="CommandProperty"/> associated with the specified <see cref="Microsoft.UI.Xaml.Documents.Hyperlink"/>
        /// </summary>
        /// <param name="obj">The <see cref="Microsoft.UI.Xaml.Documents.Hyperlink"/> to associated the <see cref="CommandProperty"/> instance to</param>
        /// <param name="value">The <see cref="object"/> to set the <see cref="CommandProperty"/> to</param>
        public static void SetCommandParameter(Microsoft.UI.Xaml.Documents.Hyperlink obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }

        private static void OnCommandPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            Microsoft.UI.Xaml.Documents.Hyperlink hyperlink = sender as Microsoft.UI.Xaml.Documents.Hyperlink;

            if (hyperlink != null)
            {
                hyperlink.Click -= OnHyperlinkClicked;

                if (args.NewValue is ICommand)
                {
                    hyperlink.Click += OnHyperlinkClicked;
                }
            }
        }

        private static void OnHyperlinkClicked(Microsoft.UI.Xaml.Documents.Hyperlink sender, Microsoft.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            var command = GetCommand(sender);
            var parameter = GetCommandParameter(sender);

            if (command?.CanExecute(parameter) == true)
            {
                command.Execute(parameter);
            }
        }
    }
}