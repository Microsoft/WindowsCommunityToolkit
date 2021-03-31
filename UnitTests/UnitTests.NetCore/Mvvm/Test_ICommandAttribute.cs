// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable CS0618

using System.Threading;
using System.Threading.Tasks;
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

            model.IncrementCounterWithValueCommand.Execute(5);

            Assert.AreEqual(model.Counter, 6);

            model.IncrementCounterAsyncCommand.Execute(null);

            Assert.AreEqual(model.Counter, 7);

            model.IncrementCounterWithTokenAsyncCommand.Execute(null);

            Assert.AreEqual(model.Counter, 8);

            model.IncrementCounterWithValueAsyncCommand.Execute(5);

            Assert.AreEqual(model.Counter, 13);

            model.IncrementCounterWithValueAndTokenAsyncCommand.Execute(5);

            Assert.AreEqual(model.Counter, 18);
        }

        public sealed partial class MyViewModel
        {
            public int Counter { get; private set; }

            [ICommand]
            private void IncrementCounter()
            {
                Counter++;
            }

            [ICommand]
            private void IncrementCounterWithValue(int count)
            {
                Counter += count;
            }

            [ICommand]
            private Task IncrementCounterAsync()
            {
                Counter += 1;

                return Task.CompletedTask;
            }

            [ICommand]
            private Task IncrementCounterWithTokenAsync(CancellationToken token)
            {
                Counter += 1;

                return Task.CompletedTask;
            }

            [ICommand]
            private Task IncrementCounterWithValueAsync(int count)
            {
                Counter += count;

                return Task.CompletedTask;
            }

            [ICommand]
            private Task IncrementCounterWithValueAndTokenAsync(int count, CancellationToken token)
            {
                Counter += count;

                return Task.CompletedTask;
            }
        }
    }
}
