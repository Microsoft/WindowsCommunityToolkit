// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <summary>
    /// Provides attached dependency properties for the <see cref="Windows.UI.Xaml.Controls.WebView"/>
    /// that allows attaching HTML string content/>.
    /// </summary>
    public static class WebViewExtensions
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for HTML content. This enables binding html string content.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.RegisterAttached("Content", typeof(string), typeof(WebViewExtensions), new PropertyMetadata(string.Empty, OnContentChanged));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Content Uri.  This binding Content Uri.
        /// </summary>
        public static readonly DependencyProperty ContentUriProperty = DependencyProperty.RegisterAttached("ContentUri", typeof(Uri), typeof(WebViewExtensions), new PropertyMetadata(null, OnContentUriChanged));

        /// <summary>
        /// Gets Content associated with the <see cref="Windows.UI.Xaml.Controls.WebView"/>
        /// </summary>
        /// <param name="obj">The <see cref="Windows.UI.Xaml.Controls.WebView"/> that has the content</param>
        /// <returns>HTML content</returns>
        public static string GetContent(DependencyObject obj)
        {
            return (string)obj.GetValue(ContentProperty);
        }

        /// <summary>
        /// Sets HTML from the <see cref="Windows.UI.Xaml.Controls.WebView"/>
        /// </summary>
        /// <param name="obj">The <see cref="Windows.UI.Xaml.Controls.WebView"/> that content is being set to.</param>
        /// <param name="value">HTML content</param>
        public static void SetContent(DependencyObject obj, string value)
        {
            obj.SetValue(ContentProperty, value);
        }

        /// <summary>
        /// Gets Uri source associated with the <see cref="Windows.UI.Xaml.Controls.WebView"/>
        /// </summary>
        /// <param name="obj">The <see cref="Windows.UI.Xaml.Controls.WebView"/> that has the content uri.</param>
        /// <returns>HTML content</returns>
        public static Uri GetContentUri(DependencyObject obj)
        {
            return (Uri)obj.GetValue(ContentUriProperty);
        }

        /// <summary>
        /// Sets HTML from the <see cref="Windows.UI.Xaml.Controls.WebView"/>
        /// </summary>
        /// <param name="obj">The <see cref="Windows.UI.Xaml.Controls.WebView"/> that content uri is being set to.</param>
        /// <param name="value">HTML content</param>
        public static void SetContentUri(DependencyObject obj, Uri value)
        {
            obj.SetValue(ContentUriProperty, value);
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Windows.UI.Xaml.Controls.WebView wv = d as Windows.UI.Xaml.Controls.WebView;

            var content = e.NewValue as string;

            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            wv?.NavigateToString(content);
        }

        private static void OnContentUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Windows.UI.Xaml.Controls.WebView wv = d as Windows.UI.Xaml.Controls.WebView;

            var uri = e.NewValue as Uri;

            if (uri == null)
            {
                return;
            }

            wv?.Navigate(uri);
        }
    }
}
