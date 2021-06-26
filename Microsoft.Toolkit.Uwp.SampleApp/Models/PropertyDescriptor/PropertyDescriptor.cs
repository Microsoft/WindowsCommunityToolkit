// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Dynamic;

namespace Microsoft.Toolkit.Uwp.SampleApp.Models
{
    public class PropertyDescriptor
    {
        public ExpandoObject Expando { get; set; }

        public List<PropertyOptions> Options { get; private set; }

        public PropertyDescriptor()
        {
            Options = new List<PropertyOptions>();
        }
    }
}