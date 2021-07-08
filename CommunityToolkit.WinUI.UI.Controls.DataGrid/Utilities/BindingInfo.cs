// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace CommunityToolkit.WinUI.UI.Data.Utilities
{
    /// <summary>
    /// Stores information about a Binding, including the BindingExpression, BindingTarget and associated Element.
    /// </summary>
    internal class BindingInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingInfo"/> class.
        /// </summary>
        public BindingInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingInfo"/> class
        /// with the specified BindingExpression, DependencyProperty and FrameworkElement.
        /// </summary>
        /// <param name="bindingExpression">BindingExpression</param>
        /// <param name="bindingTarget">BindingTarget</param>
        /// <param name="element">Element</param>
        public BindingInfo(BindingExpression bindingExpression, DependencyProperty bindingTarget, FrameworkElement element)
        {
            this.BindingExpression = bindingExpression;
            this.BindingTarget = bindingTarget;
            this.Element = element;
        }

        /// <summary>
        /// Gets or sets the BindingExpression.
        /// </summary>
        public BindingExpression BindingExpression { get; set; }

        /// <summary>
        /// Gets or sets the BindingTarget.
        /// </summary>
        public DependencyProperty BindingTarget { get; set; }

        /// <summary>
        /// Gets or sets the Element.
        /// </summary>
        public FrameworkElement Element { get; set; }
    }
}