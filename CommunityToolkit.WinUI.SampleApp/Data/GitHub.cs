// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommunityToolkit.WinUI.SampleApp.Data
{
    public static class GitHub
    {
        private const string _root = "https://api.github.com";
        private const string _repoName = "WindowsCommunityToolkit";
        private const string _repoOwner = "Microsoft";

        private static List<GitHubRelease> _releases;

        public static async Task<List<GitHubRelease>> GetPublishedReleases()
        {
            if (_releases == null)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var uri = $"{_root}/repos/{_repoOwner}/{_repoName}/releases";

                        var request = new HttpRequestMessage(HttpMethod.Get, uri);
                        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");

                        using (request)
                        {
                            using (var response = await client.SendAsync(request))
                            {
                                if (response.IsSuccessStatusCode)
                                {
                                    var result = await response.Content.ReadAsStringAsync();
                                    _releases = JsonSerializer.Deserialize<List<GitHubRelease>>(result).Take(5).ToList();
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            return _releases;
        }
    }
}