// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Messaging;
using UITests.App.Pages;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UITests.App
{
    /// <summary>
    /// This file contains part of the app related to the AppService for communication between this test host and the test harness processes.
    /// </summary>
    public sealed partial class App
    {
        private static readonly ValueSet BadResult = new() { { "Status", "BAD" } };
        private static readonly ValueSet OkResult = new() { { "Status", "OK" } };

        private AppServiceConnection _appServiceConnection;
        private BackgroundTaskDeferral _appServiceDeferral;

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);
            IBackgroundTaskInstance taskInstance = args.TaskInstance;
            AppServiceTriggerDetails appService = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            _appServiceDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnAppServicesCanceled;
            _appServiceConnection = appService.AppServiceConnection;
            _appServiceConnection.RequestReceived += OnAppServiceRequestReceived;
            _appServiceConnection.ServiceClosed += AppServiceConnection_ServiceClosed;
        }

        private async void OnAppServiceRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var messageDeferral = args.GetDeferral();
            var message = args.Request.Message;
            if(!TryGetValueAndLog(message, "Command", out var cmd))
            {
                await args.Request.SendResponseAsync(BadResult);
                messageDeferral.Complete();
                return;
            }

            Log.Comment("Received Command: {0}", cmd);

            switch (cmd)
            {
                case "OpenPage":
                    if (!TryGetValueAndLog(message, "Page", out var pageName))
                    {
                        await args.Request.SendResponseAsync(BadResult);
                        break;
                    }

                    Log.Comment("Received request for Page: {0}", pageName);

                    // We await the OpenPage method to ensure the navigation has finished.
                    var pageResponse = await WeakReferenceMessenger.Default.Send(new RequestPageMessage(pageName));

                    await args.Request.SendResponseAsync(pageResponse ? OkResult : BadResult);

                    break;
                case "Custom":
                    if (!TryGetValueAndLog(message, "Id", out var id) || !_customCommands.ContainsKey(id))
                    {
                        await args.Request.SendResponseAsync(BadResult);
                        break;
                    }

                    Log.Comment("Received request for custom command: {0}", id);

                    try
                    {
                        ValueSet response = await _customCommands[id].Invoke(message);

                        if (response != null)
                        {
                            response.Add("Status", "OK");
                        }
                        else
                        {
                            await args.Request.SendResponseAsync(BadResult);
                            break;
                        }

                        await args.Request.SendResponseAsync(response);
                    }
                    catch (Exception e)
                    {
                        ValueSet errmsg = new() { { "Status", "BAD" }, { "Exception", e.Message }, { "StackTrace", e.StackTrace } };
                        if (e.InnerException != null)
                        {
                            errmsg.Add("InnerException", e.InnerException.Message);
                            errmsg.Add("InnerExceptionStackTrace", e.InnerException.StackTrace);
                        }

                        await args.Request.SendResponseAsync(errmsg);
                    }

                    break;
                default:
                    break;
            }

            // Complete the deferral so that the platform knows that we're done responding to the app service call.
            // Note: for error handling: this must be called even if SendResponseAsync() throws an exception.
            messageDeferral.Complete();
        }

        private void OnAppServicesCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            Log.Error("Background Task Instance Canceled. Reason: {0}", reason.ToString());

            _appServiceDeferral.Complete();
        }

        private void AppServiceConnection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Log.Error("AppServiceConnection Service Closed. AppServicesClosedStatus: {0}", args.Status.ToString());

            _appServiceDeferral.Complete();
        }

        public async Task SendLogMessage(string level, string msg)
        {
            var message = new ValueSet
            {
                { "Command", "Log" },
                { "Level", level },
                { "Message", msg }
            };

            await _appServiceConnection.SendMessageAsync(message);

            // TODO: do we care if we have a problem here?
        }

        private static bool TryGetValueAndLog(ValueSet message, string key, out string value)
        {
            value = null;
            if (!message.TryGetValue(key, out var o))
            {
                Log.Error($"Could not find the key \"{key}\" in the message.");
                return false;
            }

            if (o is not string s)
            {
                Log.Error($"{key}'s value is not a string");
                return false;
            }

            value = s;

            return true;
        }

        private Dictionary<string, Func<ValueSet, Task<ValueSet>>> _customCommands = new();

        internal void RegisterCustomCommand(string id, Func<ValueSet, Task<ValueSet>> customCommandFunction)
        {
            _customCommands.Add(id, customCommandFunction);
        }
    }
}
