// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Mvvm
{
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649", Justification = "Generic type")]
    public class Test_RelayCommandOfT
    {
        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_RelayCommandOfT_AlwaysEnabled()
        {
            string text = string.Empty;

            var command = new RelayCommand<string>(s => text = s);

            Assert.IsTrue(command.CanExecute("Text"));
            Assert.IsTrue(command.CanExecute(null));

            Assert.ThrowsException<InvalidCastException>(() => command.CanExecute(new object()));

            (object, EventArgs) args = default;

            command.CanExecuteChanged += (s, e) => args = (s, e);

            command.NotifyCanExecuteChanged();

            Assert.AreSame(args.Item1, command);
            Assert.AreSame(args.Item2, EventArgs.Empty);

            command.Execute((object)"Hello");

            Assert.AreEqual(text, "Hello");

            command.Execute(null);

            Assert.AreEqual(text, null);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_RelayCommand_WithCanExecuteFunction()
        {
            string text = string.Empty;

            var command = new RelayCommand<string>(s => text = s, s => s != null);

            Assert.IsTrue(command.CanExecute("Text"));
            Assert.IsFalse(command.CanExecute(null));

            Assert.ThrowsException<InvalidCastException>(() => command.CanExecute(new object()));

            command.Execute((object)"Hello");

            Assert.AreEqual(text, "Hello");

            command.Execute(null);

            Assert.AreEqual(text, "Hello");
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_RelayCommand_NullWithValueType()
        {
            int n = 0;

            var command = new RelayCommand<int>(i => n = i);

            Assert.IsFalse(command.CanExecute(null));
            Assert.ThrowsException<NullReferenceException>(() => command.Execute(null));

            command = new RelayCommand<int>(i => n = i, i => i > 0);

            Assert.IsFalse(command.CanExecute(null));
            Assert.ThrowsException<NullReferenceException>(() => command.Execute(null));
        }
    }
}