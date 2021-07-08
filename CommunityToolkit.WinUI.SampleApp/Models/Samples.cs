// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Helpers;
using Windows.ApplicationModel;

namespace CommunityToolkit.WinUI.SampleApp
{
    public static class Samples
    {
        private const string _recentSamplesStorageKey = "uct-recent-samples";

        private static List<SampleCategory> _samplesCategories;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private static LinkedList<Sample> _recentSamples;
        private static LocalObjectStorageHelper _localObjectStorageHelper = new LocalObjectStorageHelper(new SystemSerializer());

        public static async Task<SampleCategory> GetCategoryBySample(Sample sample)
        {
            return (await GetCategoriesAsync()).FirstOrDefault(c => c.Samples.Contains(sample));
        }

        public static async Task<SampleCategory> GetCategoryByName(string name)
        {
            return (await GetCategoriesAsync()).FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static async Task<Sample> GetSampleByName(string name)
        {
            return (await GetCategoriesAsync()).SelectMany(c => c.Samples).FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static async Task<Sample[]> FindSample(string name)
        {
            var query = name.ToLower();
            return (await GetCategoriesAsync())
                .SelectMany(c => c.Samples)
                .Where(s => s.Name.ToLower().Contains(query) ||
                            s.Subcategory?.ToLower()?.Contains(query) == true ||
                            s.About.ToLower().Contains(query))
                .ToArray();
        }

        private static string _installedLocationPath = null;

        public static async Task<MemoryStream> LoadLocalFile(string fileName)
        {
            if (_installedLocationPath == null)
            {
                var workingFolder = Package.Current.InstalledLocation;
                _installedLocationPath = workingFolder.Path;
            }

            if (fileName.StartsWith('/'))
            {
                fileName = fileName.TrimStart('/');
            }

            var fullFileName = Path.Combine(_installedLocationPath, fileName);

            var stream = new MemoryStream();
            if (!File.Exists(fullFileName))
            {
                throw new FileNotFoundException("File not found.", fullFileName);
            }

            await File.OpenRead(fullFileName).CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        public static async Task<List<SampleCategory>> GetCategoriesAsync()
        {
            await _semaphore.WaitAsync();
            if (_samplesCategories == null)
            {
                List<SampleCategory> allCategories;
                using (var jsonStream = await LoadLocalFile("SamplePages/samples.json"))
                {
                    allCategories = await JsonSerializer.DeserializeAsync<List<SampleCategory>>(jsonStream, new JsonSerializerOptions
                    {
                        ReadCommentHandling = JsonCommentHandling.Skip
                    });
                }

                // Check API
                var supportedCategories = new List<SampleCategory>();
                foreach (var category in allCategories)
                {
                    var finalSamples = new List<Sample>();

                    foreach (var sample in category.Samples)
                    {
                        sample.CategoryName = category.Name;

                        if (sample.IsSupported)
                        {
                            finalSamples.Add(sample);
                        }
                    }

                    if (finalSamples.Count > 0)
                    {
                        supportedCategories.Add(category);
                        category.Samples = finalSamples.OrderBy(s => s.Name).ToArray();
                    }
                }

                _samplesCategories = supportedCategories.ToList();
            }

            _semaphore.Release();
            return _samplesCategories;
        }

        public static async Task<LinkedList<Sample>> GetRecentSamples()
        {
            if (_recentSamples == null)
            {
                _recentSamples = new LinkedList<Sample>();
                var savedSamples = _localObjectStorageHelper.Read<string>(_recentSamplesStorageKey);

                if (savedSamples != null)
                {
                    var sampleNames = savedSamples.Split(';').Reverse();
                    foreach (var name in sampleNames)
                    {
                        var sample = await GetSampleByName(name);
                        if (sample != null)
                        {
                            _recentSamples.AddFirst(sample);
                        }
                    }
                }
            }

            return _recentSamples;
        }

        public static async Task PushRecentSample(Sample sample)
        {
            var samples = await GetRecentSamples();

            var duplicates = samples.Where(s => s.Name == sample.Name).ToList();
            foreach (var duplicate in duplicates)
            {
                samples.Remove(duplicate);
            }

            samples.AddFirst(sample);
            while (samples.Count > 10)
            {
                samples.RemoveLast();
            }

            SaveRecentSamples();
        }

        private static void SaveRecentSamples()
        {
            if (_recentSamples == null)
            {
                return;
            }

            var str = string.Join(";", _recentSamples.Take(10).Select(s => s.Name).ToArray());
            _localObjectStorageHelper.Save<string>(_recentSamplesStorageKey, str);
        }
    }
}