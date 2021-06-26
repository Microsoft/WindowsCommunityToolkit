// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;

namespace Microsoft.Toolkit.Uwp.SampleApp
{
    public class LandingPageLinks
    {
        [JsonPropertyName("new-section-title")]
        public string NewSectionTitle { get; set; }

        [JsonPropertyName("new-samples")]
        public string[] NewSamples { get; set; }

        [JsonPropertyName("resources")]
        public LandingPageResource[] Resources { get; set; }
    }
}