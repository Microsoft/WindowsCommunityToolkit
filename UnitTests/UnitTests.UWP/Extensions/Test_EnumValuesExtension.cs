// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace UnitTests.Extensions
{
    [TestClass]
    public class Test_EnumValuesExtension
    {
        [TestCategory("EnumValuesExtension")]
        [UITestMethod]
        public void Test_EnumValuesExtension_MarkupExtension()
        {
            var treeRoot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:ui=""using:CommunityToolkit.WinUI.UI""
    xmlns:local=""using:UnitTests.Extensions"">
        <ListView x:Name=""Check"" ItemsSource=""{ui:EnumValues Type=local:Animal}""/>
</Page>") as FrameworkElement;

            var list = treeRoot.FindChild("Check") as ListView;

            Assert.IsNotNull(list, "Could not find ListView control in tree.");

            Animal[] items = list.ItemsSource as Animal[];

            Assert.IsNotNull(items, "The items were not created correctly");

            CollectionAssert.AreEqual(items, Enum.GetValues(typeof(Animal)));
        }
    }

    public enum Animal
    {
        Cat,
        Dog,
        Bunny
    }
}