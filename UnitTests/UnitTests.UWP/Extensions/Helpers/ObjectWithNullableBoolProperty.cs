// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UnitTests.Extensions.Helpers
{
    public class ObjectWithNullableBoolProperty : DependencyObject
    {
        public bool? NullableBool
        {
            get { return (bool?)GetValue(NullableBoolProperty); }
            set { SetValue(NullableBoolProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NullableBool.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NullableBoolProperty =
            DependencyProperty.Register(nameof(NullableBool), typeof(bool?), typeof(ObjectWithNullableBoolProperty), new PropertyMetadata(null));
    }
}