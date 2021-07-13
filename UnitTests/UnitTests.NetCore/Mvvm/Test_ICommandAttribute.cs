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
        public async Task Test_ICommandAttribute_RelayCommand()
        {
            var model = new MyViewModel();

            Assert.AreEqual(model.Counter, 0);

            model.IncrementCounterCommand.Execute(null);

            Assert.AreEqual(model.Counter, 1);

            model.IncrementCounterWithValueCommand.Execute(5);

            Assert.AreEqual(model.Counter, 6);

            await model.DelayAndIncrementCounterCommand.ExecuteAsync(null);

            Assert.AreEqual(model.Counter, 7);

            await model.DelayAndIncrementCounterWithTokenCommand.ExecuteAsync(null);

            Assert.AreEqual(model.Counter, 8);

            await model.DelayAndIncrementCounterWithValueCommand.ExecuteAsync(5);

            Assert.AreEqual(model.Counter, 13);

            await model.DelayAndIncrementCounterWithValueAndTokenCommand.ExecuteAsync(5);

            Assert.AreEqual(model.Counter, 18);
        }

        public sealed partial class MyViewModel
        {
            public int Counter { get; private set; }

            /// <summary>This is a single line summary.</summary>
            [ICommand]
            private void IncrementCounter()
            {
                Counter++;
            }

            /// <summary>
            /// This is a multiline summary
            /// </summary>
            [ICommand]
            private void IncrementCounterWithValue(int count)
            {
                Counter += count;
            }

            /// <summary>This is single line with also other stuff below</summary>
            /// <returns>Foo bar baz</returns>
            /// <returns>A task</returns>
            [ICommand]
            private async Task DelayAndIncrementCounterAsync()
            {
                await Task.Delay(50);

                Counter += 1;
            }

            /// <summary>
            /// This is multi line with also other stuff below
            /// </summary>
            /// <returns>Foo bar baz</returns>
            /// <returns>A task</returns>
            [ICommand]
            private async Task DelayAndIncrementCounterWithTokenAsync(CancellationToken token)
            {
                await Task.Delay(50);

                Counter += 1;
            }

            // This should not be ported over
            [ICommand]
            private async Task DelayAndIncrementCounterWithValueAsync(int count)
            {
                await Task.Delay(50);

                Counter += count;
            }

            [ICommand]
            private async Task DelayAndIncrementCounterWithValueAndTokenAsync(int count, CancellationToken token)
            {
                await Task.Delay(50);

                Counter += count;
            }
        }
    }
}
