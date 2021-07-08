// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace CommunityToolkit.WinUI.Input.GazeInteraction
{
    /// <summary>
    /// A helper class to read a ValueSet and retrieve relevant settings
    /// </summary>
    public class GazeSettingsHelper
    {
        /// <summary>
        /// Retrieves settings as a ValueSet from a shared store.
        /// </summary>
        /// <returns>An <see cref="IAsyncAction"/> representing the asynchronous operation.</returns>
        public static IAsyncAction RetrieveSharedSettings(ValueSet settings)
        {
            async Task InternalRetrieveSharedSettings()
            {
                // Setup a new app service connection
                AppServiceConnection connection = new AppServiceConnection();
                connection.AppServiceName = "com.microsoft.ectksettings";
                connection.PackageFamilyName = "Microsoft.EyeControlToolkitSettings_s9y1p3hwd5qda";

                // open the connection
                var status = await connection.OpenAsync();
                switch (status)
                {
                    case AppServiceConnectionStatus.Success:
                        // The new connection opened successfully
                        // Set up the inputs and send a message to the service
                        var response = await connection.SendMessageAsync(new ValueSet());

                        if (response == null)
                        {
                            return;
                        }

                        switch (response.Status)
                        {
                            case AppServiceResponseStatus.Success:
                                foreach (var item in response.Message)
                                {
                                    settings.Add(item.Key, item.Value);
                                }

                                break;
                            default:
                            case AppServiceResponseStatus.Failure:
                            case AppServiceResponseStatus.ResourceLimitsExceeded:
                            case AppServiceResponseStatus.Unknown:
                                break;
                        }

                        break;

                    default:
                    case AppServiceConnectionStatus.AppNotInstalled:
                    case AppServiceConnectionStatus.AppUnavailable:
                    case AppServiceConnectionStatus.AppServiceUnavailable:
                    case AppServiceConnectionStatus.Unknown:
                        return;
                }
            }

            return InternalRetrieveSharedSettings().AsAsyncAction();
        }

        private GazeSettingsHelper()
        {
        }
    }
}