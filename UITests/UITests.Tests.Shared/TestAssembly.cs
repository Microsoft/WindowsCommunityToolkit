// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.UI.Xaml.Tests.MUXControls.InteractionTests.Infra;
using UITests.App.Protos;

#if USING_TAEF
using WEX.Logging.Interop;
using WEX.TestExecution;
using WEX.TestExecution.Markup;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace UITests.Tests
{
    // This is marked as a test class to make sure our AssemblyInitialize and AssemblyCleanup
    // fixtures get executed.  It won't actually host any tests.
    [TestClass]
    public class TestAssembly
    {
        private static GrpcChannel _channel;

        private static AppService.AppServiceClient _communicationService;

        private static CancellationTokenSource _subscribeLogTokenSource;
        private static Task _subscribeLogTask;
        private static AsyncServerStreamingCall<LogUpdate> _logStream;

        [AssemblyInitialize]
        [TestProperty("CoreClrProfile", ".")]
        [TestProperty("RunFixtureAs:Assembly", "ElevatedUserOrSystem")]
        [TestProperty("Hosting:Mode", "UAP")]
        public static void AssemblyInitialize(TestContext testContext)
        {
            TestEnvironment.AssemblyInitialize(testContext, "UITests.App.pfx");
        }

        [AssemblyCleanup]
        public static async Task AssemblyCleanup()
        {
            _subscribeLogTokenSource?.Cancel();
            _subscribeLogTokenSource = null;

            if (_subscribeLogTask != null)
            {
                await _subscribeLogTask;
                _subscribeLogTask = null;
            }

            _logStream?.Dispose();
            _logStream = null;

            TestEnvironment.AssemblyCleanupWorker(UITestBase.UITestsAppSampleApp);
        }

        private static Task InitalizeComService()
        {
            Log.Comment("[Harness] Trying to connect...");

            _channel = GrpcChannel.ForAddress(
                "https://localhost:5001",
                new GrpcChannelOptions
                {
                    HttpHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    }
                });

            Log.Comment("[Harness] Trying to connect 2...");

            _communicationService = new AppService.AppServiceClient(_channel);

            Log.Comment("[Harness] Connected!");

            _logStream = _communicationService.SubscribeLog(new SubscribeLogRequest());

            _subscribeLogTokenSource = new CancellationTokenSource();

            static async Task SubscribeLog(IAsyncStreamReader<LogUpdate> stream, CancellationToken token)
            {
                try
                {
                    await foreach (var update in stream.ReadAllAsync(token))
                    {
                        switch (update.Level)
                        {
                            case "Comment":
                                Log.Comment("[Host] {0}", update.Message);
                                break;
                            case "Warning":
                                Log.Warning("[Host] {0}", update.Message);
                                break;
                            case "Error":
                                Log.Error("[Host] {0}", update.Message);
                                break;
                        }
                    }
                }
                catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
                {
                    return;
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Finished.");
                }
            }

            _subscribeLogTask = SubscribeLog(_logStream.ResponseStream, _subscribeLogTokenSource.Token);
            return Task.CompletedTask;
        }

        internal static async Task<bool> OpenPage(string pageName)
        {
            Log.Comment("[Harness] Sending Host Page Request: {0}", pageName);

            if (_channel is null)
            {
                await InitalizeComService();
            }

            var response = await _communicationService.OpenPageAsync(new OpenPageRequest
            {
                PageName = pageName
            });
            string result = string.Empty;

            // Get the data  that the service sent to us.
            return response.Status == "OK";
        }
    }
}