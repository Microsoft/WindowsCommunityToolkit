// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.CompilerServices
{
    // HACK (2021.05.07): Included as a workaround for multiple occurrences of
    // error CS0518: Predefined type 'System.Runtime.CompilerServices.IsExternalInit' is not defined or imported
    // in UnitTests\UnitTests.UWP\UI\Controls\Test_RangeSelector.cs.
    // This is caused by using [Positional Records](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-9#record-types)
    // which use [Init Only setters](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-9#init-only-setters) under the hood for all the properties,
    // and currently the type is only included net5.0 and up.
    // The recommended action is to include the type manually. https://developercommunity.visualstudio.com/t/error-cs0518-predefined-type-systemruntimecompiler/1244809#T-N1249582
    internal static class IsExternalInit
    {
    }
}