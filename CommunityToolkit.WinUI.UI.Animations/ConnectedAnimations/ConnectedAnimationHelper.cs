// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// Connected Animation Helper used with the <see cref="Connected"/> class
    /// Attaches to a <see cref="Frame"/> navigation events to handle connected animations
    /// <seealso cref="Connected"/>
    /// </summary>
    internal class ConnectedAnimationHelper
    {
        private readonly Dictionary<string, ConnectedAnimationProperties> _previousPageConnectedAnimationProps = new Dictionary<string, ConnectedAnimationProperties>();

        private object _nextParameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedAnimationHelper"/> class.
        /// </summary>
        /// <param name="frame">The <see cref="Frame"/> hosting the content</param>
        public ConnectedAnimationHelper(Frame frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame));
            }

            frame.Navigating += Frame_Navigating;
            frame.Navigated += Frame_Navigated;
        }

        internal void SetParameterForNextFrameNavigation(object parameter)
        {
            _nextParameter = parameter;
        }

        private void Frame_Navigating(object sender, Microsoft.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        {
            object parameter;
            if (_nextParameter != null)
            {
                parameter = _nextParameter;
            }
            else
            {
                parameter = e.Parameter != null && !(e.Parameter is string str && string.IsNullOrEmpty(str)) ? e.Parameter : null;
            }

            var cas = ConnectedAnimationService.GetForCurrentView();

            var page = (sender as Frame).Content as Page;
            var connectedAnimationsProps = Connected.GetPageConnectedAnimationProperties(page);

            foreach (var props in connectedAnimationsProps.Values)
            {
                ConnectedAnimation animation = null;

                if (props.IsListAnimation && parameter != null)
                {
                    foreach (var listAnimProperty in props.ListAnimProperties)
                    {
                        if (listAnimProperty.ListViewBase.ItemsSource is IEnumerable<object> items &&
                            items.Contains(parameter))
                        {
                            try
                            {
                                animation = listAnimProperty.ListViewBase.PrepareConnectedAnimation(props.Key, parameter, listAnimProperty.ElementName);
                            }
                            catch
                            {
                                animation = null;
                            }
                        }
                    }
                }
                else if (!props.IsListAnimation)
                {
                    animation = cas.PrepareToAnimate(props.Key, props.Element);
                }
                else
                {
                    continue;
                }

                if (animation != null &&
                    e.NavigationMode == Microsoft.UI.Xaml.Navigation.NavigationMode.Back)
                {
                    UseDirectConnectedAnimationConfiguration(animation);
                }

                _previousPageConnectedAnimationProps[props.Key] = props;
            }
        }

        private void Frame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var navigatedPage = (sender as Frame).Content as Page;

            if (navigatedPage == null)
            {
                return;
            }

            void LoadedHandler(object s, RoutedEventArgs args)
            {
                var page = s as Page;
                page.Loaded -= LoadedHandler;

                object parameter;
                if (_nextParameter != null)
                {
                    parameter = _nextParameter;
                }
                else if (e.NavigationMode == Microsoft.UI.Xaml.Navigation.NavigationMode.Back)
                {
                    var sourcePage = (sender as Frame).ForwardStack.LastOrDefault();
                    parameter = sourcePage?.Parameter;
                }
                else
                {
                    parameter = e.Parameter;
                }

                var cas = ConnectedAnimationService.GetForCurrentView();

                var connectedAnimationsProps = Connected.GetPageConnectedAnimationProperties(page);
                var coordinatedAnimationElements = Connected.GetPageCoordinatedAnimationElements(page);

                foreach (var props in connectedAnimationsProps.Values)
                {
                    var connectedAnimation = cas.GetAnimation(props.Key);
                    var animationHandled = false;
                    if (connectedAnimation != null)
                    {
                        if (props.IsListAnimation && parameter != null)
                        {
                            foreach (var listAnimProperty in props.ListAnimProperties)
                            {
                                if (listAnimProperty.ListViewBase.ItemsSource is IEnumerable<object> items && items.Contains(parameter))
                                {
                                    listAnimProperty.ListViewBase.ScrollIntoView(parameter);

                                    // give time to the UI thread to scroll the list
                                    var t = listAnimProperty.ListViewBase.DispatcherQueue.EnqueueAsync(
                                        async () =>
                                        {
                                            try
                                            {
                                                var success = await listAnimProperty.ListViewBase.TryStartConnectedAnimationAsync(connectedAnimation, parameter, listAnimProperty.ElementName);
                                            }
                                            catch (Exception)
                                            {
                                                connectedAnimation.Cancel();
                                            }
                                        }, Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal);

                                    animationHandled = true;
                                }
                            }
                        }
                        else if (!props.IsListAnimation)
                        {
                            if (coordinatedAnimationElements.TryGetValue(props.Element, out var coordinatedElements))
                            {
                                connectedAnimation.TryStart(props.Element, coordinatedElements);
                            }
                            else
                            {
                                connectedAnimation.TryStart(props.Element);
                            }

                            animationHandled = true;
                        }
                    }

                    if (_previousPageConnectedAnimationProps.ContainsKey(props.Key) && animationHandled)
                    {
                        _previousPageConnectedAnimationProps.Remove(props.Key);
                    }
                }

                // if there are animations that were prepared on previous page but no elements on this page have the same key - cancel
                foreach (var previousProps in _previousPageConnectedAnimationProps)
                {
                    var connectedAnimation = cas.GetAnimation(previousProps.Key);
                    connectedAnimation?.Cancel();
                }

                _previousPageConnectedAnimationProps.Clear();
                _nextParameter = null;
            }

            navigatedPage.Loaded += LoadedHandler;
        }

        private void UseDirectConnectedAnimationConfiguration(ConnectedAnimation animation)
        {
            animation.Configuration = new DirectConnectedAnimationConfiguration();
        }
    }
}