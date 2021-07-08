// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using CommunityToolkit.WinUI.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

namespace CommunityToolkit.WinUI.UI.Utilities
{
    internal static class Extensions
    {
        private static Dictionary<DependencyObject, Dictionary<DependencyProperty, int>> _suspendedHandlers = new Dictionary<DependencyObject, Dictionary<DependencyProperty, int>>();

        public static bool IsHandlerSuspended(this DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            return _suspendedHandlers.ContainsKey(dependencyObject) ? _suspendedHandlers[dependencyObject].ContainsKey(dependencyProperty) : false;
        }

        /// <summary>
        /// Walks the visual tree to determine if a particular child is contained within a parent DependencyObject.
        /// </summary>
        /// <param name="element">Parent DependencyObject</param>
        /// <param name="child">Child DependencyObject</param>
        /// <returns>True if the parent element contains the child</returns>
        internal static bool ContainsChild(this DependencyObject element, DependencyObject child)
        {
            if (element != null)
            {
                while (child != null)
                {
                    if (child == element)
                    {
                        return true;
                    }

                    // Walk up the visual tree.  If the root is hit, try using the framework element's
                    // parent.  This is done because Popups behave differently with respect to the visual tree,
                    // and it could have a parent even if the VisualTreeHelper doesn't find it.
                    DependencyObject parent = VisualTreeHelper.GetParent(child);
                    if (parent == null)
                    {
                        FrameworkElement childElement = child as FrameworkElement;
                        if (childElement != null)
                        {
                            parent = childElement.Parent;
                        }
                    }

                    child = parent;
                }
            }

            return false;
        }

        /// <summary>
        /// Walks the visual tree to determine if the currently focused element is contained within
        /// a parent DependencyObject. The FocusManager's GetFocusedElement method is used to determine
        /// the currently focused element, which is updated synchronously.
        /// </summary>
        /// <param name="element">Parent DependencyObject</param>
        /// <param name="uiElement">Parent UIElement. Used to query the element's XamlRoot.</param>
        /// <returns>True if the currently focused element is within the visual tree of the parent</returns>
        internal static bool ContainsFocusedElement(this DependencyObject element, UIElement uiElement)
        {
            return (element == null) ? false : element.ContainsChild(GetFocusedElement(uiElement) as DependencyObject);
        }

        private static object GetFocusedElement(UIElement uiElement)
        {
            if (uiElement.XamlRoot != null)
            {
                return FocusManager.GetFocusedElement(uiElement.XamlRoot);
            }
            else
            {
                return FocusManager.GetFocusedElement();
            }
        }

        /// <summary>
        /// Checks a MemberInfo object (e.g. a Type or PropertyInfo) for the ReadOnly attribute
        /// and returns the value of IsReadOnly if it exists.
        /// </summary>
        /// <param name="memberInfo">MemberInfo to check</param>
        /// <returns>true if MemberInfo is read-only, false otherwise</returns>
        internal static bool GetIsReadOnly(this MemberInfo memberInfo)
        {
            if (memberInfo != null)
            {
                // Check if ReadOnlyAttribute is defined on the member
                object[] attributes = memberInfo.GetCustomAttributes(typeof(ReadOnlyAttribute), true);
                if (attributes != null && attributes.Length > 0)
                {
                    ReadOnlyAttribute readOnlyAttribute = attributes[0] as ReadOnlyAttribute;
                    Debug.Assert(readOnlyAttribute != null, "Expected non-null readOnlyAttribute.");
                    return readOnlyAttribute.IsReadOnly;
                }
            }

            return false;
        }

        internal static Type GetItemType(this IEnumerable list)
        {
            Type listType = list.GetType();
            Type itemType = null;
            bool isICustomTypeProvider = false;

            // If it's a generic enumerable, get the generic type.

            // Unfortunately, if data source is fed from a bare IEnumerable, TypeHelper will report an element type of object,
            // which is not particularly interesting.  It is dealt with it further on.
            if (listType.IsEnumerableType())
            {
                itemType = listType.GetEnumerableItemType();
                if (itemType != null)
                {
                    isICustomTypeProvider = typeof(ICustomTypeProvider).IsAssignableFrom(itemType);
                }
            }

            // Bare IEnumerables mean that result type will be object.  In that case, try to get something more interesting.
            // Or, if the itemType implements ICustomTypeProvider, try to retrieve the custom type from one of the object instances.
            if (itemType == null || itemType == typeof(object) || isICustomTypeProvider)
            {
                // No type was located yet. Does the list have anything in it?
                Type firstItemType = null;
                IEnumerator en = list.GetEnumerator();
                if (en.MoveNext() && en.Current != null)
                {
                    firstItemType = en.Current.GetCustomOrCLRType();
                }
                else
                {
                    firstItemType = list
                        .Cast<object>() // cast to convert IEnumerable to IEnumerable<object>
                        .Select(x => x.GetType()) // get the type
                        .FirstOrDefault(); // get only the first thing to come out of the sequence, or null if empty
                }

                if (firstItemType != typeof(object))
                {
                    return firstItemType;
                }
            }

            // Couldn't get the CustomType because there were no items.
            if (isICustomTypeProvider)
            {
                return null;
            }

            return itemType;
        }

        public static void SetStyleWithType(this FrameworkElement element, Style style)
        {
            if (element.Style != style && (style == null || style.TargetType != null))
            {
                element.Style = style;
            }
        }

        public static void SetValueNoCallback(this DependencyObject obj, DependencyProperty property, object value)
        {
            obj.SuspendHandler(property, true);
            try
            {
                obj.SetValue(property, value);
            }
            finally
            {
                obj.SuspendHandler(property, false);
            }
        }

        internal static Point Translate(this UIElement fromElement, UIElement toElement, Point fromPoint)
        {
            if (fromElement == toElement)
            {
                return fromPoint;
            }
            else
            {
                return fromElement.TransformToVisual(toElement).TransformPoint(fromPoint);
            }
        }

        // If the parent element goes into a background tab, the elements need to be remeasured
        // or they will report 0 height.
        internal static UIElement EnsureMeasured(this UIElement element)
        {
            if (element.DesiredSize.Height == 0)
            {
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            return element;
        }

        private static void SuspendHandler(this DependencyObject obj, DependencyProperty dependencyProperty, bool incrementSuspensionCount)
        {
            if (_suspendedHandlers.ContainsKey(obj))
            {
                Dictionary<DependencyProperty, int> suspensions = _suspendedHandlers[obj];

                if (incrementSuspensionCount)
                {
                    if (suspensions.ContainsKey(dependencyProperty))
                    {
                        suspensions[dependencyProperty]++;
                    }
                    else
                    {
                        suspensions[dependencyProperty] = 1;
                    }
                }
                else
                {
                    Debug.Assert(suspensions.ContainsKey(dependencyProperty), "Expected existing key for dependencyProperty.");
                    if (suspensions[dependencyProperty] == 1)
                    {
                        suspensions.Remove(dependencyProperty);
                    }
                    else
                    {
                        suspensions[dependencyProperty]--;
                    }

                    if (suspensions.Count == 0)
                    {
                        _suspendedHandlers.Remove(obj);
                    }
                }
            }
            else
            {
                Debug.Assert(incrementSuspensionCount, "Expected incrementSuspensionCount==true.");
                _suspendedHandlers[obj] = new Dictionary<DependencyProperty, int>();
                _suspendedHandlers[obj][dependencyProperty] = 1;
            }
        }
    }
}