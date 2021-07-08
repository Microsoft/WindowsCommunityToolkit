// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Connectivity;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.WinUI.SampleApp
{
    internal static class Tools
    {
        internal static async Task<bool> CheckInternetConnectionAsync(XamlRoot xamlRoot)
        {
            if (!NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
            {
                await new ContentDialog
                {
                    Title = "Windows Community Toolkit Sample App",
                    Content = "Internet connection not detected. Please try again later.",
                    CloseButtonText = "Close",
                    XamlRoot = xamlRoot
                }.ShowAsync();

                return false;
            }

            return true;
        }
    }
}