// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Mvvm
{
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649", Justification = "Generic type")]
    public class Test_AsyncRelayCommandOfT
    {
        [TestCategory("Mvvm")]
        [TestMethod]
        public async Task Test_AsyncRelayCommandOfT_AlwaysEnabled()
        {
            int ticks = 0;

            var command = new AsyncRelayCommand<string>(async s =>
            {
                await Task.Delay(1000);
                ticks = int.Parse(s);
                await Task.Delay(1000);
            });

            Assert.IsTrue(command.CanExecute(null));
            Assert.IsTrue(command.CanExecute("1"));

            (object, EventArgs) args = default;

            command.CanExecuteChanged += (s, e) => args = (s, e);

            command.NotifyCanExecuteChanged();

            Assert.AreSame(args.Item1, command);
            Assert.AreSame(args.Item2, EventArgs.Empty);

            Assert.IsNull(command.ExecutionTask);
            Assert.IsFalse(command.IsRunning);

            Task task = command.ExecuteAsync((object)"42");

            Assert.IsNotNull(command.ExecutionTask);
            Assert.AreSame(command.ExecutionTask, task);
            Assert.IsTrue(command.IsRunning);

            await task;

            Assert.IsFalse(command.IsRunning);

            Assert.AreEqual(ticks, 42);

            command.Execute("2");

            await command.ExecutionTask!;

            Assert.AreEqual(ticks, 2);

            Assert.ThrowsException<InvalidCastException>(() => command.Execute(new object()));
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_AsyncRelayCommandOfT_WithCanExecuteFunctionTrue()
        {
            int ticks = 0;

            var command = new AsyncRelayCommand<string>(
                s =>
            {
                ticks = int.Parse(s);
                return Task.CompletedTask;
            }, s => true);

            Assert.IsTrue(command.CanExecute(null));
            Assert.IsTrue(command.CanExecute("1"));

            command.Execute("42");

            Assert.AreEqual(ticks, 42);

            command.Execute("2");

            Assert.AreEqual(ticks, 2);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_AsyncRelayCommandOfT_WithCanExecuteFunctionFalse()
        {
            int ticks = 0;

            var command = new AsyncRelayCommand<string>(
                s =>
            {
                ticks = int.Parse(s);
                return Task.CompletedTask;
            }, s => false);

            Assert.IsFalse(command.CanExecute(null));
            Assert.IsFalse(command.CanExecute("1"));

            command.Execute("2");

            Assert.AreEqual(ticks, 0);

            command.Execute("42");

            Assert.AreEqual(ticks, 0);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_AsyncRelayCommandOfT_NullWithValueType()
        {
            int n = 0;

            var command = new AsyncRelayCommand<int>(i =>
            {
                n = i;
                return Task.CompletedTask;
            });

            Assert.IsFalse(command.CanExecute(null));
            Assert.ThrowsException<NullReferenceException>(() => command.Execute(null));

            command = new AsyncRelayCommand<int>(
                i =>
            {
                n = i;
                return Task.CompletedTask;
            }, i => i > 0);

            Assert.IsFalse(command.CanExecute(null));
            Assert.ThrowsException<NullReferenceException>(() => command.Execute(null));
        }
    }
}