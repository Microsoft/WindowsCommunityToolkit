// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.SampleApp.Data
{
    public class PhotoDataItem
    {
        public string Title { get; set; }

        public string Category { get; set; }

        public string Thumbnail { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
