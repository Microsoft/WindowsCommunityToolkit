// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml.Controls;
using Windows.System.RemoteSystems;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    public sealed partial class RemoteDeviceHelperPage : Page
    {
        private RemoteDeviceHelper _remoteDeviceHelper;

        public RemoteDeviceHelperPage()
        {
            this.InitializeComponent();

            var selectionEnum = Enum.GetNames(typeof(RemoteSystemDiscoveryType)).Cast<string>().OrderBy(q => q).ToList();
            discoveryType.ItemsSource = selectionEnum;
            discoveryType.SelectedIndex = 0;

            selectionEnum = Enum.GetNames(typeof(RemoteSystemAuthorizationKind)).Cast<string>().OrderBy(q => q).ToList();
            authorizationType.ItemsSource = selectionEnum;
            authorizationType.SelectedIndex = 0;

            selectionEnum = Enum.GetNames(typeof(RemoteSystemStatusType)).Cast<string>().OrderBy(q => q).ToList();
            statusType.ItemsSource = selectionEnum;
            statusType.SelectedIndex = 0;
        }

        private void Button_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var filters = new List<IRemoteSystemFilter>
            {
                new RemoteSystemDiscoveryTypeFilter((RemoteSystemDiscoveryType)Enum.Parse(typeof(RemoteSystemDiscoveryType), discoveryType?.SelectedValue.ToString())),
                new RemoteSystemAuthorizationKindFilter((RemoteSystemAuthorizationKind)Enum.Parse(typeof(RemoteSystemAuthorizationKind), authorizationType?.SelectedValue.ToString())),
                new RemoteSystemStatusTypeFilter((RemoteSystemStatusType)Enum.Parse(typeof(RemoteSystemStatusType), statusType?.SelectedValue.ToString()))
            };

            _remoteDeviceHelper = new RemoteDeviceHelper(filters);
            DevicesList.DataContext = _remoteDeviceHelper;
        }
    }
}