// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.XamlIslands.UWPApp
{
    public class STATestClassAttribute : TestClassAttribute
    {
        /*
        public override TestMethodAttribute GetTestMethodAttribute(TestMethodAttribute testMethodAttribute)
        {
            if (testMethodAttribute is STATestMethodAttribute)
            {
                return testMethodAttribute;
            }

            return new STATestMethodAttribute(base.GetTestMethodAttribute(testMethodAttribute));
        }
        */
    }
}