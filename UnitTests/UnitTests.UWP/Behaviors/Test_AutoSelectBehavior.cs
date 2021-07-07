// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace UnitTests.Behaviors
{
    [TestClass]
    public class Test_AutoSelectBehavior : VisualUITestBase
    {
        [TestCategory("Behaviors")]
        [TestMethod]
        public async Task Test_AutoSelectBehavior_SelectsAllContent()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:behaviors=""using:Microsoft.Toolkit.Uwp.UI.Behaviors""
    xmlns:interactivity=""using:Microsoft.Xaml.Interactivity"">
    <Grid>
        <TextBox x:Name=""TargetElement"" Text=""ThisShouldBeSelectedWhenLoaded"">
            <interactivity:Interaction.Behaviors>
                <behaviors:AutoSelectBehavior />
            </interactivity:Interaction.Behaviors>
        </TextBox>
    </Grid>
</Page>") as Page;

                // Test Setup
                Assert.IsNotNull(treeRoot, "XAML Failed to Load");

                // Initialize Visual Tree
                await SetTestContentAsync(treeRoot);

                // Main Test
                var textBox = treeRoot.FindName("TargetElement") as TextBox;

                Assert.IsNotNull(textBox, "TextBox was not found in the tree.");
                textBox.SelectedText.Should().Be("ThisShouldBeSelectedWhenLoaded");
            });
        }
    }
}
