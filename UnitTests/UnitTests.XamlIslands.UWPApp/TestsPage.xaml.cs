// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace UnitTests.XamlIslands.UWPApp
{
    public sealed partial class TestsPage
    {
        internal static TestsPage Instance { get; private set; }

        internal void SetMainTestContent(UIElement content)
        {
            MainTestContent.Child = content;
        }

        public TestsPage()
        {
            Instance = this;
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await RunTestsAsync();
        }

        public async Task RunTestsAsync()
        {
            richTextBlock.Blocks.Clear();
            App.Dispatcher = DispatcherQueue.GetForCurrentThread();
            App.XamlRoot = XamlRoot;

            await Task.Run(async () =>
            {
                TestResult testResult = default;

                var sw = new Stopwatch();

                await WriteLineAsync("--- Starting Tests Execution ---");

                sw.Start();

                foreach (var testClass in typeof(XamlIslandsTest_SystemInformation).Assembly.GetTypes())
                {
                    Attribute[] attributes = Attribute.GetCustomAttributes(testClass);

                    foreach (var attribute in attributes)
                    {
                        if (attribute is STATestClassAttribute || attribute is TestClassAttribute)
                        {
                            var partialTestResult = await RunTestsAsync(testClass);
                            testResult += partialTestResult;
                            break;
                        }
                    }
                }

                sw.Stop();

                var color = testResult.Failed > 0 ? Colors.Red : Colors.Green;
                await WriteLineAsync($"--- Finished Tests Execution ---", color);

                if (testResult.Ignored > 0)
                {
                    await WriteLineAsync($"--- \tIgnored: {testResult.Ignored} ---", Colors.Orange);
                }

                await WriteLineAsync($"--- \tPassed: {testResult.Passed} ---", Colors.Green);
                await WriteLineAsync($"--- \tFailed: {testResult.Failed} ---", Colors.Red);
                await WriteLineAsync($"--- \tTotal: {testResult.Count} ---");

                await WriteLineAsync($"--- Duration - {sw.Elapsed} ---");
            });
        }

        private async Task<TestResult> RunTestsAsync(Type type)
        {
            int count = 0;
            int passed = 0;
            int ignored = 0;
            var initMethod = GetFirstMethod(type, typeof(TestInitializeAttribute));
            var cleanupMethod = GetFirstMethod(type, typeof(TestCleanupAttribute));

            foreach (var method in type.GetMethods())
            {
                Attribute[] attributes = Attribute.GetCustomAttributes(method);

                if (attributes.Any(a => a is STATestMethodAttribute || a is TestMethodAttribute))
                {
                    count++;

                    if (attributes.Any(a => a is IgnoreAttribute))
                    {
                        ignored++;
                        continue;
                    }

                    try
                    {
                        await WriteLineAsync($"{type.FullName}.{method.Name}\t - \t Running...");

                        var instance = Activator.CreateInstance(type);

                        if (initMethod != null)
                        {
                            var resultInit = initMethod.Invoke(instance, null);
                            if (resultInit is Task taskInit)
                            {
                                await taskInit;
                            }
                        }

                        var result = method.Invoke(instance, null);
                        if (result is Task t)
                        {
                            await t;
                        }

                        if (cleanupMethod != null)
                        {
                            var resultCleanup = cleanupMethod.Invoke(instance, null);
                            if (resultCleanup is Task taskCleanup)
                            {
                                await taskCleanup;
                            }
                        }

                        await WriteLineAsync($"{type.FullName}.{method.Name}\t - \t PASS      ", Colors.Green, true);
                        passed++;
                    }
                    catch (Exception ex)
                    {
                        await WriteLineAsync($"{type.FullName}.{method.Name}\t - \t FAIL      :{Environment.NewLine}{ex}", Colors.Red, true);
                    }
                    finally
                    {
                        await App.Dispatcher.EnqueueAsync(() =>
                        {
                            SetMainTestContent(null);
                        });
                    }
                }
            }

            return new TestResult(count, passed, ignored);
        }

        private Task WriteLineAsync(string message, Color? color = null, bool deleteLastLine = false)
        {
            Debug.WriteLine(message);
            return App.Dispatcher.EnqueueAsync(() =>
            {
                if (deleteLastLine)
                {
                    richTextBlock.Blocks.Remove(richTextBlock.Blocks.Last());
                }

                var paragraph = new Paragraph();
                if (color != null)
                {
                    paragraph.Foreground = new SolidColorBrush(color.Value);
                }

                paragraph.Inlines.Add(new Run
                {
                    Text = message
                });
                richTextBlock.Blocks.Add(paragraph);
            });
        }

        private struct TestResult
        {
            public int Count { get; }

            public int Passed { get; }

            public int Ignored { get; }

            public int Failed => Count - Passed - Ignored;

            public TestResult(int count, int passed, int ignored)
            {
                Count = count;
                Passed = passed;
                Ignored = ignored;
            }

            public static TestResult operator +(TestResult a, TestResult b) => new TestResult(a.Count + b.Count, a.Passed + b.Passed, a.Ignored + b.Ignored);
        }

        private MethodInfo GetFirstMethod(Type ofType, Type attributeType)
        {
            foreach (var method in ofType.GetMethods())
            {
                Attribute[] attributes = Attribute.GetCustomAttributes(method);

                foreach (Attribute attribute in attributes)
                {
                    if (attribute.GetType() == attributeType)
                    {
                        return method;
                    }
                }
            }

            return null;
        }
    }
}