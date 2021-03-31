// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable CS0618

using System;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Mvvm
{
    [TestClass]
    public partial class Test_ICommandAttribute
    {
        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ICommandAttribute_RelayCommand()
        {
            var model = new MyViewModel();

            Assert.AreEqual(model.Counter, 0);

            model.IncrementCounterCommand.Execute(null);

            Assert.AreEqual(model.Counter, 1);
        }

        public sealed partial class MyViewModel
        {
            public int Counter { get; private set; }

            [ICommand]
            private void IncrementCounter()
            {
                Counter++;
            }
        }
    }
}
