// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.SampleApp.Models
{
    public class PropertyOptions
    {
        public string Name { get; set; }

        public string Label { get; set; }

        public string OriginalString { get; set; }

        public PropertyKind Kind { get; set; }

        public object DefaultValue { get; set; }

        public bool IsTwoWayBinding { get; set; }
    }
}