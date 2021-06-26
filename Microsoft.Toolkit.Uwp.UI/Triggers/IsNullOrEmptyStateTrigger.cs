// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI.Triggers
{
    /// <summary>
    /// Enables a state if an Object is <c>null</c> or a String/IEnumerable is empty
    /// </summary>
    public class IsNullOrEmptyStateTrigger : StateTriggerBase
    {
        /// <summary>
        /// Gets or sets the value used to check for <c>null</c> or empty.
        /// </summary>
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Value"/> DependencyProperty
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(IsNullOrEmptyStateTrigger), new PropertyMetadata(true, OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (IsNullOrEmptyStateTrigger)d;
            var val = e.NewValue;

            obj.SetActive(IsNullOrEmpty(val));

            if (val == null)
            {
                return;
            }

            // Try to listen for various notification events
            // Starting with INorifyCollectionChanged
            var valNotifyCollection = val as INotifyCollectionChanged;
            if (valNotifyCollection != null)
            {
                var weakEvent = new WeakEventListener<INotifyCollectionChanged, object, NotifyCollectionChangedEventArgs>(valNotifyCollection)
                {
                    OnEventAction = (instance, source, args) => obj.SetActive(IsNullOrEmpty(instance)),
                    OnDetachAction = (weakEventListener) => valNotifyCollection.CollectionChanged -= weakEventListener.OnEvent
                };

                valNotifyCollection.CollectionChanged += weakEvent.OnEvent;
                return;
            }

            // Not INotifyCollectionChanged, try IObservableVector
            var valObservableVector = val as IObservableVector<object>;
            if (valObservableVector != null)
            {
                var weakEvent = new WeakEventListener<IObservableVector<object>, object, IVectorChangedEventArgs>(valObservableVector)
                {
                    OnEventAction = (instance, source, args) => obj.SetActive(IsNullOrEmpty(instance)),
                    OnDetachAction = (weakEventListener) => valObservableVector.VectorChanged -= weakEventListener.OnEvent
                };

                valObservableVector.VectorChanged += weakEvent.OnEvent;
                return;
            }

            // Not INotifyCollectionChanged, try IObservableMap
            var valObservableMap = val as IObservableMap<object, object>;
            if (valObservableMap != null)
            {
                var weakEvent = new WeakEventListener<IObservableMap<object, object>, object, IMapChangedEventArgs<object>>(valObservableMap)
                {
                    OnEventAction = (instance, source, args) => obj.SetActive(IsNullOrEmpty(instance)),
                    OnDetachAction = (weakEventListener) => valObservableMap.MapChanged -= weakEventListener.OnEvent
                };

                valObservableMap.MapChanged += weakEvent.OnEvent;
            }
        }

        private static bool IsNullOrEmpty(object val)
        {
            if (val == null)
            {
                return true;
            }

            // Object is not null, check for an empty string
            var valString = val as string;
            if (valString != null)
            {
                return valString.Length == 0;
            }

            // Object is not a string, check for an empty ICollection (faster)
            var valCollection = val as ICollection;
            if (valCollection != null)
            {
                return valCollection.Count == 0;
            }

            // Object is not an ICollection, check for an empty IEnumerable
            var valEnumerable = val as IEnumerable;
            if (valEnumerable != null)
            {
                foreach (var item in valEnumerable)
                {
                    // Found an item, not empty
                    return false;
                }

                return true;
            }

            // Not null and not a known type to test for emptiness
            return false;
        }
    }
}