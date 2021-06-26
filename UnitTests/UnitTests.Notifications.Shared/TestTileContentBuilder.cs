// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Notifications
{
#if !WINRT
    [TestClass]
    public class TestTileContentBuilder
    {
        [TestMethod]
        public void AddTextTest_OnSmallTileOnly()
        {
            // Arrange
            string text = "text on small tile";
            TileContentBuilder builder = new TileContentBuilder();
            builder.AddTile(TileSize.Small);

            // Act
            builder.AddText(text);

            // Assert
            var tileText = GetTileAdaptiveText(builder, TileSize.Small);
            Assert.IsNotNull(tileText);
            Assert.AreSame("text on small tile", (string)tileText.Text);
        }

        [TestMethod]
        public void AddTextTest_OnMediumTileOnly()
        {
            // Arrange
            string text = "text on medium tile";
            TileContentBuilder builder = new TileContentBuilder();
            builder.AddTile(TileSize.Medium);

            // Act
            builder.AddText(text);

            // Assert
            var tileText = GetTileAdaptiveText(builder, TileSize.Medium);
            Assert.IsNotNull(tileText);
            Assert.AreSame("text on medium tile", (string)tileText.Text);
        }

        [TestMethod]
        public void AddTextTest_OnWideTileOnly()
        {
            // Arrange
            string text = "text on wide tile";
            TileContentBuilder builder = new TileContentBuilder();
            builder.AddTile(TileSize.Wide);

            // Act
            builder.AddText(text);

            // Assert
            var tileText = GetTileAdaptiveText(builder, TileSize.Wide);
            Assert.IsNotNull(tileText);
            Assert.AreSame("text on wide tile", (string)tileText.Text);
        }

        [TestMethod]
        public void AddTextTest_OnLargeTileOnly()
        {
            // Arrange
            string text = "text on large tile";
            TileContentBuilder builder = new TileContentBuilder();
            builder.AddTile(TileSize.Large);

            // Act
            builder.AddText(text);

            // Assert
            var tileText = GetTileAdaptiveText(builder, TileSize.Large);
            Assert.IsNotNull(tileText);
            Assert.AreSame("text on large tile", (string)tileText.Text);
        }

        private static AdaptiveText GetTileAdaptiveText(TileContentBuilder builder, TileSize size)
        {
            TileBinding tileBinding;
            switch (size)
            {
                case TileSize.Small:
                    tileBinding = builder.Content.Visual.TileSmall;
                    break;

                case TileSize.Medium:
                    tileBinding = builder.Content.Visual.TileMedium;
                    break;

                case TileSize.Wide:
                    tileBinding = builder.Content.Visual.TileWide;
                    break;

                case TileSize.Large:
                    tileBinding = builder.Content.Visual.TileLarge;
                    break;

                default:
                    return null;
            }

            var content = (TileBindingContentAdaptive)tileBinding.Content;
            return content.Children.FirstOrDefault() as AdaptiveText;
        }
    }
#endif
}