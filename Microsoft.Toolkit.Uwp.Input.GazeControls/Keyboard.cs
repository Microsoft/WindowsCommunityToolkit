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
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace Microsoft.Toolkit.Uwp.Input.GazeControls
{
    /// <summary>
    /// Enumeration for the type of key in the layout file.
    /// </summary>
    public enum KeyTypeValue
    {
        /// <summary>
        /// The key is a normal button that injects keys into its target
        /// </summary>
        Normal,

        /// <summary>
        /// The key is a toggle button that is used to change the state of keyboard
        /// </summary>
        Toggle,

        /// <summary>
        /// The key loads a different layout page
        /// </summary>
        Layout
    }

    /// <summary>
    /// Gaze optimized soft keyboard with support for custom layouts and predictions
    /// </summary>
    public sealed partial class GazeKeyboard : UserControl
    {
        /// <summary>
        /// The KeyType property allows users to specify the type of a key in the layout file. <see cref="KeyTypeValue"/>
        /// </summary>
        public static readonly DependencyProperty KeyTypeProperty =
            DependencyProperty.RegisterAttached("KeyType", typeof(KeyTypeValue), typeof(GazeKeyboard), new PropertyMetadata(KeyTypeValue.Normal));

        /// <summary>
        /// Gets the key type
        /// </summary>
        /// <param name="obj"> DependencyObject</param>
        /// <returns>KeyTypeValue</returns>
        public static KeyTypeValue GetKeyType(DependencyObject obj)
        {
            return (KeyTypeValue)obj.GetValue(KeyTypeProperty);
        }

        /// <summary>
        /// Sets the key type
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <param name="value">KeyTypeValue to set</param>
        public static void SetKeyType(DependencyObject obj, KeyTypeValue value)
        {
            obj.SetValue(KeyTypeProperty, value);
        }

        /// <summary>
        /// The VK property specifies the virtual key to inject into the target when a key is pressed
        /// </summary>
        public static readonly DependencyProperty VKProperty =
            DependencyProperty.RegisterAttached("VK", typeof(int), typeof(GazeKeyboard), new PropertyMetadata(0));

        /// <summary>
        /// Gets the virtual key that will be injected
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <returns>The virtual key as an integer</returns>
        public static int GetVK(DependencyObject obj)
        {
            return (int)obj.GetValue(VKProperty);
        }

        /// <summary>
        /// Sets the virtual key to be injected
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <param name="value">The virtual key as an integer</param>
        public static void SetVK(DependencyObject obj, int value)
        {
            obj.SetValue(VKProperty, value);
        }

        /// <summary>
        /// The Unicode property indicates the unicode string that will be injected when a key is pressed
        /// </summary>
        public static readonly DependencyProperty UnicodeProperty =
            DependencyProperty.RegisterAttached("Unicode", typeof(string), typeof(GazeKeyboard), new PropertyMetadata(0));

        /// <summary>
        /// Gets the unicode string that will be injected
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <returns>The unicode value as a string</returns>
        public static string GetUnicode(DependencyObject obj)
        {
            return obj.GetValue(UnicodeProperty) as string;
        }

        /// <summary>
        /// Sets the unicode string to be injected
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <param name="value">The unicode value as a string</param>
        public static void SetUnicode(DependencyObject obj, string value)
        {
            obj.SetValue(UnicodeProperty, value);
        }

        /// <summary>
        /// The VKList property specifies a list of virtual keys to be injected when a key is pressed
        /// </summary>
        public static readonly DependencyProperty VKListProperty =
            DependencyProperty.RegisterAttached("VKList", typeof(List<int>), typeof(GazeKeyboard), new PropertyMetadata(0));

        /// <summary>
        /// Returns the list of virtual keys
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <returns>A list of integers that represent virtual key codes</returns>
        public static List<int> GetVKList(DependencyObject obj)
        {
            var value = obj.GetValue(VKListProperty);
            var list = value as List<int>;
            if (list == null)
            {
                list = new List<int>();
                SetVKList(obj, list);
            }

            return list;
        }

        /// <summary>
        /// Sets the list of virtual key codes to be injected
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <param name="value">List of virtual key codes</param>
        public static void SetVKList(DependencyObject obj, List<int> value)
        {
            obj.SetValue(VKListProperty, value);
        }

        /// <summary>
        /// The PageList property specifies the list of pages in a keyboard layout
        /// </summary>
        public static readonly DependencyProperty PageListProperty =
            DependencyProperty.RegisterAttached("PageList", typeof(List<string>), typeof(GazeKeyboard), new PropertyMetadata(0));

        /// <summary>
        /// Gets the list of pages in a layout
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <returns>List of pages</returns>
        public static List<string> GetPageList(DependencyObject obj)
        {
            var value = obj.GetValue(PageListProperty);
            var list = value as List<string>;
            if (list == null)
            {
                list = new List<string>();
                SetPageList(obj, list);
            }

            return list;
        }

        /// <summary>
        /// Sets the list of pages for a layout
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <param name="value">List of pages</param>
        public static void SetPageList(DependencyObject obj, List<string> value)
        {
            obj.SetValue(PageListProperty, value);
        }

        /// <summary>
        /// The PageContainer property specifies the container page for a layout
        /// </summary>
        public static readonly DependencyProperty PageContainerProperty =
            DependencyProperty.RegisterAttached("PageContainer", typeof(string), typeof(GazeKeyboard), new PropertyMetadata(0));

        /// <summary>
        /// Gets the container for a page in a layout
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <returns>The container for a page</returns>
        public static string GetPageContainer(DependencyObject obj)
        {
            return obj.GetValue(PageContainerProperty) as string;
        }

        /// <summary>
        /// Sets the container for a page in a layout
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <param name="value">The container for a page</param>
        public static void SetPageContainer(DependencyObject obj, string value)
        {
            obj.SetValue(PageContainerProperty, value);
        }

        /// <summary>
        /// The TemporaryPage property specifies a temporary layout to be loaded. This layout is changed back
        /// as soon as a single key is presssed
        /// </summary>
        public static readonly DependencyProperty TemporaryPageProperty =
            DependencyProperty.RegisterAttached("TemporaryPage", typeof(string), typeof(GazeKeyboard), new PropertyMetadata(0));

        /// <summary>
        /// Gets the temporary page to load
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <returns>The temporary page to load</returns>
        public static string GetTemporaryPage(DependencyObject obj)
        {
            return obj.GetValue(TemporaryPageProperty) as string;
        }

        /// <summary>
        /// Sets the temporary page to load
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <param name="value">The temporary page to load</param>
        public static void SetTemporaryPage(DependencyObject obj, string value)
        {
            obj.SetValue(TemporaryPageProperty, value);
        }

        /// <summary>
        /// The NewPage property specifies a new layout to load when a key is a pressed
        /// </summary>
        public static readonly DependencyProperty NewPageProperty =
            DependencyProperty.RegisterAttached("NewPage", typeof(string), typeof(GazeKeyboard), new PropertyMetadata(0));

        /// <summary>
        /// Gets the new page to load
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <returns>The new page</returns>
        public static string GetNewPage(DependencyObject obj)
        {
            return obj.GetValue(NewPageProperty) as string;
        }

        /// <summary>
        /// Sets the new page to load
        /// </summary>
        /// <param name="obj">DependencyObject</param>
        /// <param name="value">The new page</param>
        public static void SetNewPage(DependencyObject obj, string value)
        {
            obj.SetValue(NewPageProperty, value);
        }
    }
}
