// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text.Json.Serialization;

namespace Microsoft.Toolkit.Uwp.SampleApp
{
    public class GitHubRelease
    {
        [JsonPropertyName("published_at")]
        public DateTime Published { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }

        [JsonPropertyName("tag_name")]
        public string Tag { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        public string FullName => $"Version {Name.Substring(1)} notes"; // Skip the initial 'v' we put at the front. If we replace all 'v's then we hit 'preview'.

        [JsonPropertyName("draft")]
        public bool IsDraft { get; set; }

        [JsonPropertyName("html_url")]
        public string Url { get; set; }
    }
}