// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

namespace CommunityToolkit.WinUI.DeveloperTools
{
    /// <summary>
    /// FocusTracker can be used to display information about the current focused XAML element.
    /// </summary>
    [TemplatePart(Name = "ControlName", Type = typeof(TextBlock))]
    [TemplatePart(Name = "ControlType", Type = typeof(TextBlock))]
    [TemplatePart(Name = "ControlAutomationName", Type = typeof(TextBlock))]
    [TemplatePart(Name = "ControlFirstParentWithName", Type = typeof(TextBlock))]
    public partial class FocusTracker : Control
    {
        /// <summary>
        /// Defines the <see cref="IsActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(FocusTracker), new PropertyMetadata(false, OnIsActiveChanged));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FocusTracker focusTracker)
            {
                if (e.NewValue != null && (bool)e.NewValue)
                {
                    focusTracker.Start();
                }
                else
                {
                    focusTracker.Stop();
                }
            }
        }

        private TextBlock controlName;
        private TextBlock controlType;
        private TextBlock controlAutomationName;
        private TextBlock controlFirstParentWithName;

        /// <summary>
        /// Gets or sets a value indicating whether the tracker is running or not.
        /// </summary>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FocusTracker"/> class.
        /// </summary>
        public FocusTracker()
        {
            DefaultStyleKey = typeof(FocusTracker);
        }

        /// <summary>
        /// Update the visual state of the control when its template is changed.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            controlName = GetTemplateChild("ControlName") as TextBlock;
            controlType = GetTemplateChild("ControlType") as TextBlock;
            controlAutomationName = GetTemplateChild("ControlAutomationName") as TextBlock;
            controlFirstParentWithName = GetTemplateChild("ControlFirstParentWithName") as TextBlock;
        }

        private void Start()
        {
            // Get currently focused control once when we start
            if (XamlRoot != null)
            {
                FocusOnControl(FocusManager.GetFocusedElement(XamlRoot) as FrameworkElement);
            }
            else
            {
                FocusOnControl(FocusManager.GetFocusedElement() as FrameworkElement);
            }

            // Then use FocusManager event from 1809 to listen to updates
            FocusManager.GotFocus += FocusManager_GotFocus;
        }

        private void Stop()
        {
            FocusManager.GotFocus -= FocusManager_GotFocus;
            ClearContent();
        }

        private void FocusManager_GotFocus(object sender, FocusManagerGotFocusEventArgs e)
        {
            FocusOnControl(e.NewFocusedElement as FrameworkElement);
        }

        private void ClearContent()
        {
            controlName.Text = string.Empty;
            controlType.Text = string.Empty;
            controlAutomationName.Text = string.Empty;
            controlFirstParentWithName.Text = string.Empty;
        }

        private void FocusOnControl(FrameworkElement focusedControl)
        {
            if (focusedControl == null)
            {
                ClearContent();
                return;
            }

            if (controlName != null)
            {
                controlName.Text = focusedControl.Name;
            }

            if (controlType != null)
            {
                controlType.Text = focusedControl.GetType().Name;
            }

            if (controlAutomationName != null)
            {
                controlAutomationName.Text = AutomationProperties.GetName(focusedControl);
            }

            if (controlFirstParentWithName != null)
            {
                var parentWithName = FindVisualAscendantWithName(focusedControl);
                controlFirstParentWithName.Text = parentWithName?.Name ?? string.Empty;
            }
        }

        private FrameworkElement FindVisualAscendantWithName(FrameworkElement element)
        {
            var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;

            if (parent == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(parent.Name))
            {
                return parent;
            }

            return FindVisualAscendantWithName(parent);
        }
    }
}