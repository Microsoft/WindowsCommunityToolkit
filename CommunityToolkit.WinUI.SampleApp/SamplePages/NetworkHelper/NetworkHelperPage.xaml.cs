// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Connectivity;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NetworkHelperPage : Page
    {
        public NetworkHelperPage()
        {
            InitializeComponent();
            Load();
        }

        private void Load()
        {
            IsInternetAvailableText.Text = NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable ? "Yes" : "No";
            IsInternetOnMeteredConnectionText.Text = NetworkHelper.Instance.ConnectionInformation.IsInternetOnMeteredConnection ? "Yes" : "No";
            ConnectionTypeText.Text = NetworkHelper.Instance.ConnectionInformation.ConnectionType.ToString();
            SignalBarsText.Text = NetworkHelper.Instance.ConnectionInformation.SignalStrength.GetValueOrDefault(0).ToString();
            NetworkNamesText.Text = string.Join(", ", NetworkHelper.Instance.ConnectionInformation.NetworkNames);
        }
    }
}