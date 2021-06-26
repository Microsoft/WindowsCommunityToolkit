// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.SampleApp.Models
{
    public class SliderPropertyOptions : PropertyOptions
    {
        public double MinValue { get; set; }

        public double MaxValue { get; set; }

        public double Step { get; set; } = 1;
    }
}