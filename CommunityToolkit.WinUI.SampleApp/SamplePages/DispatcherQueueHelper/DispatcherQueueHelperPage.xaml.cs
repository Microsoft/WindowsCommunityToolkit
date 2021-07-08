// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    public sealed partial class DispatcherQueueHelperPage
    {
        public DispatcherQueueHelperPage()
        {
            this.InitializeComponent();
        }

        private async void ExecuteFromDifferentThreadButton_Click(object sender, RoutedEventArgs e)
        {
            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            int crossThreadReturnedValue = await Task.Run<int>(async () =>
            {
                int returnedFromUIThread = await dispatcherQueue.EnqueueAsync<int>(() =>
                {
                    NormalTextBlock.Text = "Updated from a random thread!";
                    return Task.FromResult(1);
                });

                return returnedFromUIThread + 1;
            });

            await Task.Delay(200);

            NormalTextBlock.Text += $" And the value {crossThreadReturnedValue} was also returned successfully!";
        }
    }
}