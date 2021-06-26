// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace UITests.Tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class TestPageAttribute : Attribute
    {
        public TestPageAttribute(string xamlFile)
        {
            if (string.IsNullOrWhiteSpace(xamlFile))
            {
                throw new ArgumentException($"'{nameof(xamlFile)}' cannot be null or whitespace", nameof(xamlFile));
            }

            XamlFile = xamlFile;
        }

        public string XamlFile { get; private set; }
    }
}