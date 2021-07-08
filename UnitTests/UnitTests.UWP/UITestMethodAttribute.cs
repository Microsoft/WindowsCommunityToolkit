// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnitTests;

namespace Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer
{
    public class UITestMethodAttribute : TestMethodAttribute
    {
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            var attrib = testMethod.GetAttributes<AsyncStateMachineAttribute>(false);
            if (attrib.Length > 0)
            {
                throw new NotSupportedException("async TestMethod with UITestMethodAttribute are not supported. Either remove async or use TestMethodAttribute.");
            }

            var taskCompletionSource = new TaskCompletionSource<TestResult>();

            try
            {
                WinRT.ComWrappersSupport.InitializeComWrappers();
                global::Microsoft.UI.Xaml.Application.Start((p) => new App(testMethod, taskCompletionSource));
            }
            catch (Exception e)
            {
                taskCompletionSource.SetException(e);
            }

            return new TestResult[] { taskCompletionSource.Task.GetAwaiter().GetResult() };
        }
    }
}
