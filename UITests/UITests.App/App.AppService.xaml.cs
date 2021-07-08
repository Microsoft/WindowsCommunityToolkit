// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Grpc.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using UITests.App.Pages;
using UITests.App.Protos;

namespace UITests.App
{
    /// <summary>
    /// This file contains part of the app related to the AppService for communication between this test host and the test harness processes.
    /// </summary>
    public sealed partial class App
    {
        private static readonly OpenPageReply BadResult = new() { Status = "BAD" };
        private static readonly OpenPageReply OkResult = new() { Status = "OK" };

        private Task _host;

        private async void InitAppService()
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
            _host = builder.Build().RunAsync();
            await _host;
        }

        public class AppServiceServer : AppService.AppServiceBase
        {
            internal static BlockingCollection<LogUpdate> LogUpdates { get; private set; }

            public AppServiceServer(BlockingCollection<LogUpdate> logUpdates)
            {
                LogUpdates = logUpdates;
            }

            public override async Task<OpenPageReply> OpenPage(OpenPageRequest request, ServerCallContext context)
            {
                Log.Comment("Received request for Page: {0}", request.PageName);

                // We await the OpenPage method to ensure the navigation has finished.
                var pageResponse = await WeakReferenceMessenger.Default.Send(new RequestPageMessage(request.PageName));

                return pageResponse ? OkResult : BadResult;
            }

            public override async Task SubscribeLog(SubscribeLogRequest request, IServerStreamWriter<LogUpdate> responseStream, ServerCallContext context)
            {
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    var message = LogUpdates.Take(context.CancellationToken);

                    try
                    {
                        await responseStream.WriteAsync(message);
                    }
                    catch (Exception ex)
                    {
                        // TODO: do we care if we have a problem here?
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                }
            }
        }

        public void SendLogMessage(string level, string msg)
        {
            AppServiceServer.LogUpdates.Add(new LogUpdate
            {
                Level = level,
                Message = msg
            });
        }
    }
}