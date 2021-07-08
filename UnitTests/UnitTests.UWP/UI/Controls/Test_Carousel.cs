// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.UI.Xaml.Automation.Peers;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Automation.Peers;
using CommunityToolkit.WinUI.UI.Controls;

namespace UnitTests.UWP.UI.Controls
{
    [TestClass]
    [TestCategory("Test_Carousel")]
    public class Test_Carousel : VisualUITestBase
    {
        [TestMethod]
        public async Task ShouldConfigureCarouselAutomationPeerAsync()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                const int expectedSelectedIndex = 1;
                const string expectedCarouselAutomationName = "MyAutomationPhotoItems";
                const string expectedCarouselName = "MyPhotoItems";

                var items = new ObservableCollection<PhotoDataItem> { new PhotoDataItem { Title = "Hello" }, new PhotoDataItem { Title = "World" } };

                var carousel = new Carousel { ItemsSource = items };

                await SetTestContentAsync(carousel);

                // Sets the selected item to "World" from the items above.
                carousel.SelectedIndex = expectedSelectedIndex;

                var carouselAutomationPeer =
                    FrameworkElementAutomationPeer.CreatePeerForElement(carousel) as CarouselAutomationPeer;

                Assert.IsNotNull(carouselAutomationPeer, "Verify that the AutomationPeer is CarouselAutomationPeer.");
                Assert.IsFalse(carouselAutomationPeer.CanSelectMultiple, "Verify that CarouselAutomationPeer.CanSelectMultiple is false.");
                Assert.IsTrue(carouselAutomationPeer.IsSelectionRequired, "Verify that CarouselAutomationPeer.IsSelectionRequired is true.");

                // Asserts the automation peer name based on the Automation Property Name value.
                carousel.SetValue(AutomationProperties.NameProperty, expectedCarouselAutomationName);
                Assert.IsTrue(carouselAutomationPeer.GetName().Contains(expectedCarouselAutomationName), "Verify that the UIA name contains the given AutomationProperties.Name of the Carousel.");

                // Asserts the automation peer name based on the element Name property.
                carousel.Name = expectedCarouselName;
                Assert.IsTrue(carouselAutomationPeer.GetName().Contains(expectedCarouselName), "Verify that the UIA name contains the given Name of the Carousel.");

                var carouselItemAutomationPeers = carouselAutomationPeer.GetChildren().Cast<CarouselItemAutomationPeer>().ToList();
                Assert.AreEqual(items.Count, carouselItemAutomationPeers.Count);

                // Asserts the default calculated position in set and size of set values
                for (var i = 0; i < carouselItemAutomationPeers.Count; i++)
                {
                    var peer = carouselItemAutomationPeers[i];
                    Assert.AreEqual(i + 1, peer.GetPositionInSet());
                    Assert.AreEqual(items.Count, peer.GetSizeOfSet());
                }

                // Asserts the CarouselItemAutomationPeer properties
                var selectedItemPeer = carouselItemAutomationPeers.FirstOrDefault(peer => peer.IsSelected);
                Assert.IsNotNull(selectedItemPeer);
                Assert.IsTrue(selectedItemPeer.GetName().Contains(items[expectedSelectedIndex].ToString()));
            });
        }

        public class PhotoDataItem
        {
            public string Title { get; set; }

            public string Category { get; set; }

            public string Thumbnail { get; set; }

            public override string ToString()
            {
                return Title;
            }
        }
    }
}