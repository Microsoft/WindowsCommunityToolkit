// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.Networking.Connectivity;
using Windows.System;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI.Triggers
{
    /// <summary>
    /// Trigger for switching when the network availability changes
    /// </summary>
    public class NetworkConnectionStateTrigger : StateTriggerBase
    {
        private DispatcherQueue _dispatcherQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkConnectionStateTrigger"/> class.
        /// </summary>
        public NetworkConnectionStateTrigger()
        {
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            var weakEvent =
                new WeakEventListener<NetworkConnectionStateTrigger, object>(this)
                {
                    OnEventAction = (instance, source) => NetworkInformation_NetworkStatusChanged(source),
                    OnDetachAction = (weakEventListener) => NetworkInformation.NetworkStatusChanged -= weakEventListener.OnEvent
                };
            NetworkInformation.NetworkStatusChanged += weakEvent.OnEvent;
            UpdateState();
        }

        private void NetworkInformation_NetworkStatusChanged(object sender)
        {
            _ = _dispatcherQueue.EnqueueAsync(UpdateState, DispatcherQueuePriority.Normal);
        }

        private void UpdateState()
        {
            bool isConnected = false;
            var profile = NetworkInformation.GetInternetConnectionProfile();
            if (profile != null)
            {
                isConnected = profile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            }

            SetActive((isConnected && ConnectionState == ConnectionState.Connected) || (!isConnected && ConnectionState == ConnectionState.Disconnected));
        }

        /// <summary>
        /// Gets or sets the state of the connection to trigger on.
        /// </summary>
        public ConnectionState ConnectionState
        {
            get { return (ConnectionState)GetValue(ConnectionStateProperty); }
            set { SetValue(ConnectionStateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ConnectionState"/> DependencyProperty
        /// </summary>
        public static readonly DependencyProperty ConnectionStateProperty =
            DependencyProperty.Register(nameof(ConnectionState), typeof(ConnectionState), typeof(NetworkConnectionStateTrigger), new PropertyMetadata(ConnectionState.Connected, OnConnectionStatePropertyChanged));

        private static void OnConnectionStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (NetworkConnectionStateTrigger)d;
            obj.UpdateState();
        }

        private class WeakEventListener<TInstance, TSource>
            where TInstance : class
        {
            /// <summary>
            /// WeakReference to the instance listening for the event.
            /// </summary>
            private WeakReference _weakInstance;

            /// <summary>
            /// Gets or sets the method to call when the event fires.
            /// </summary>
            public Action<TInstance, TSource> OnEventAction { get; set; }

            /// <summary>
            /// Gets or sets the method to call when detaching from the event.
            /// </summary>
            public Action<WeakEventListener<TInstance, TSource>> OnDetachAction { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="WeakEventListener{TInstance, TSource}"/> class.
            /// </summary>
            /// <param name="instance">Instance subscribing to the event.</param>
            public WeakEventListener(TInstance instance)
            {
                if (instance == null)
                {
                    throw new ArgumentNullException("instance");
                }

                _weakInstance = new WeakReference(instance);
            }

            /// <summary>
            /// Handler for the subscribed event calls OnEventAction to handle it.
            /// </summary>
            /// <param name="source">Event source.</param>
            public void OnEvent(TSource source)
            {
                TInstance target = (TInstance)_weakInstance.Target;
                if (target != null)
                {
                    // Call registered action
                    OnEventAction?.Invoke(target, source);
                }
                else
                {
                    // Detach from event
                    Detach();
                }
            }

            /// <summary>
            /// Detaches from the subscribed event.
            /// </summary>
            public void Detach()
            {
                if (OnDetachAction != null)
                {
                    OnDetachAction(this);
                    OnDetachAction = null;
                }
            }
        }
    }

    /// <summary>
    /// ConnectionStates
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// Connected
        /// </summary>
        Connected,

        /// <summary>
        /// Disconnected
        /// </summary>
        Disconnected,
    }
}
