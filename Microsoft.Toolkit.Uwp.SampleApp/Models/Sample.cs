// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// TODO Reintroduce graph controls
// using Microsoft.Toolkit.Graph.Converters;
// using Microsoft.Toolkit.Graph.Providers;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.Input.GazeInteraction;
using Microsoft.Toolkit.Uwp.SampleApp.Models;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Toolkit.Uwp.UI.Media;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.SampleApp
{
    public class Sample
    {
        private const string _docsOnlineRoot = "https://raw.githubusercontent.com/MicrosoftDocs/WindowsCommunityToolkitDocs/";
        private const string _cacheSHAKey = "docs-cache-sha";

        private static HttpClient client = new HttpClient();

        public static async void EnsureCacheLatest()
        {
            var settingsStorage = new LocalObjectStorageHelper(new SystemSerializer());

            var onlineDocsSHA = await GetDocsSHA();
            var cacheSHA = settingsStorage.Read<string>(_cacheSHAKey);

            bool outdatedCache = onlineDocsSHA != null && cacheSHA != null && onlineDocsSHA != cacheSHA;
            bool noCache = onlineDocsSHA != null && cacheSHA == null;

            if (outdatedCache || noCache)
            {
                // Delete everything in the Cache Folder. Could be Pre 3.0.0 Cache data.
                foreach (var item in await ApplicationData.Current.LocalCacheFolder.GetItemsAsync())
                {
                    try
                    {
                        await item.DeleteAsync(StorageDeleteOption.Default);
                    }
                    catch
                    {
                    }
                }

                // Update Cache Version info.
                settingsStorage.Save(_cacheSHAKey, onlineDocsSHA);
            }
        }

        private string _cachedDocumentation = string.Empty;

        internal static async Task<Sample> FindAsync(string category, string name)
        {
            var categories = await Samples.GetCategoriesAsync();

            // Replace any spaces in the category name as it's used for the host part of the URI in deep links and that can't have spaces.
            return categories?
                .FirstOrDefault(c => c.Name.Replace(" ", string.Empty).Equals(category, StringComparison.OrdinalIgnoreCase))?
                .Samples
                .FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        private PropertyDescriptor _propertyDescriptor;

        public string Name { get; set; }

        public string Type { get; set; }

        public string Subcategory { get; set; }

        public string About { get; set; }

        private string _codeUrl;

        /// <summary>
        /// Gets the Page Type.
        /// </summary>
        public Type PageType => System.Type.GetType("Microsoft.Toolkit.Uwp.SampleApp.SamplePages." + Type);

        /// <summary>
        /// Gets or sets the Category Name.
        /// </summary>
        public string CategoryName { get; set; }

        public string CodeUrl
        {
            get
            {
                return _codeUrl;
            }

            set
            {
#if !REMOTE_DOCS
                _codeUrl = value;
#else
                var regex = new Regex("^https://github.com/windows-toolkit/WindowsCommunityToolkit/(tree|blob)/(?<branch>.+?)/(?<path>.*)");
                var docMatch = regex.Match(value);

                var branch = string.Empty;
                var path = string.Empty;
                if (docMatch.Success)
                {
                    branch = docMatch.Groups["branch"].Value;
                    path = docMatch.Groups["path"].Value;
                }

                if (string.IsNullOrWhiteSpace(branch))
                {
                    _codeUrl = value;
                }
                else
                {
                    var packageVersion = Package.Current.Id.Version.ToFormattedString(3);
                    _codeUrl = $"https://github.com/Microsoft/WindowsCommunityToolkit/tree/rel/{packageVersion}/{path}";
                }
#endif
            }
        }

        public string CodeFile { get; set; }

        public string XamlCodeFile { get; set; }

        public bool DisableXamlEditorRendering { get; set; }

        public string XamlCode { get; private set; }

        /// <summary>
        /// Gets or sets the path set in the samples.json pointing to the doc for the sample.
        /// </summary>
        public string DocumentationUrl { get; set; }

        /// <summary>
        /// Gets or sets the absolute local doc path for cached file in app.
        /// </summary>
        public string LocalDocumentationFilePath { get; set; }

        /// <summary>
        /// Gets or sets the base path segment to the current document location.
        /// </summary>
        public string RemoteDocumentationPath { get; set; }

        public string Icon { get; set; }

        public string BadgeUpdateVersionRequired { get; set; }

        public string DeprecatedWarning { get; set; }

        public string ApiCheck { get; set; }

        public bool HasType => !string.IsNullOrWhiteSpace(Type);

        public bool HasXAMLCode => !string.IsNullOrEmpty(XamlCodeFile);

        public bool HasCSharpCode => !string.IsNullOrEmpty(CodeFile);

        public bool HasDocumentation => !string.IsNullOrEmpty(DocumentationUrl);

        public bool IsSupported
        {
            get
            {
                if (ApiCheck == null)
                {
                    return true;
                }

                return ApiInformation.IsTypePresent(ApiCheck);
            }
        }

        public async Task<string> GetCSharpSourceAsync()
        {
            using (var codeStream = await StreamHelper.GetPackagedFileStreamAsync(CodeFile.StartsWith('/') ? CodeFile : $"SamplePages/{Name}/{CodeFile}"))
            {
                using (var streamReader = new StreamReader(codeStream.AsStream()))
                {
                    return await streamReader.ReadToEndAsync();
                }
            }
        }

        public async Task<string> GetDocumentationAsync()
        {
            if (!string.IsNullOrWhiteSpace(_cachedDocumentation))
            {
                return _cachedDocumentation;
            }

            var filepath = string.Empty;
            var filename = string.Empty;
            LocalDocumentationFilePath = string.Empty;

            var docRegex = new Regex("^" + _docsOnlineRoot + "(?<branch>.+?)/docs/(?<file>.+)");
            var docMatch = docRegex.Match(DocumentationUrl);
            if (docMatch.Success)
            {
                filepath = docMatch.Groups["file"].Value;
                filename = Path.GetFileName(filepath);

                RemoteDocumentationPath = Path.GetDirectoryName(filepath);
                LocalDocumentationFilePath = $"ms-appx:///docs/{filepath}/";
            }

#if REMOTE_DOCS // use the docs repo in release mode
            string modifiedDocumentationUrl = $"{_docsOnlineRoot}live/docs/{filepath}";

            // Read from Cache if available.
            try
            {
                _cachedDocumentation = await StorageFileHelper.ReadTextFromLocalCacheFileAsync(filename);
            }
            catch (Exception)
            {
            }

            // Grab from docs repo if not.
            if (string.IsNullOrWhiteSpace(_cachedDocumentation))
            {
                try
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(modifiedDocumentationUrl)))
                    {
                        using (var response = await client.SendAsync(request).ConfigureAwait(false))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var result = await response.Content.ReadAsStringAsync();
                                _cachedDocumentation = ProcessDocs(result);

                                if (!string.IsNullOrWhiteSpace(_cachedDocumentation))
                                {
                                    await StorageFileHelper.WriteTextToLocalCacheFileAsync(_cachedDocumentation, filename);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
#endif

            // Grab the local copy in Debug mode, allowing you to preview changes made.
            if (string.IsNullOrWhiteSpace(_cachedDocumentation))
            {
                try
                {
                    using (var localDocsStream = await StreamHelper.GetPackagedFileStreamAsync($"docs/{filepath}"))
                    {
                        var result = await localDocsStream.ReadTextAsync(Encoding.UTF8);
                        _cachedDocumentation = ProcessDocs(result);
                    }
                }
                catch (Exception)
                {
                }
            }

            return _cachedDocumentation;
        }

        public Uri GetOnlineResourcePath(string relativePath)
        {
            return new Uri($"{_docsOnlineRoot}live/docs/{RemoteDocumentationPath}/{relativePath}");
        }

        /// <summary>
        /// Gets the image data from a Uri, with Caching.
        /// </summary>
        /// <param name="uri">Image Uri</param>
        /// <returns>Image Stream</returns>
        public async Task<IRandomAccessStream> GetImageStream(Uri uri)
        {
            async Task<Stream> CopyStream(HttpContent source)
            {
                var stream = new MemoryStream();
                await source.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }

            IRandomAccessStream imageStream = null;
            var localPath = $"{uri.Host}/{uri.LocalPath}".Replace("//", "/");

            if (localPath.StartsWith(_docsOnlineRoot.Substring(8)))
            {
                // If we're looking for docs we should look in our local area first.
                localPath = localPath.Substring(_docsOnlineRoot.Length - 3); // want to chop "live/" but missing https:// as well.
            }

            // Try cache only in Release (using remote docs)
#if REMOTE_DOCS
            try
            {
                imageStream = await StreamHelper.GetLocalCacheFileStreamAsync(localPath, Windows.Storage.FileAccessMode.Read);
            }
            catch
            {
            }

            if (imageStream == null)
            {
                try
                {
                    // Our docs don't reference any external images, this should only be for getting latest image from repo.
                    using (var response = await client.GetAsync(uri))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var imageCopy = await CopyStream(response.Content);
                            imageStream = imageCopy.AsRandomAccessStream();

                            // Takes a second copy of the image stream, so that is can save the image data to cache.
                            using (var saveStream = await CopyStream(response.Content))
                            {
                                await SaveImageToCache(localPath, saveStream);
                            }
                        }
                    }
                }
                catch
                {
                }
            }
#endif

            // If we don't have internet, then try to see if we have a packaged copy
            if (imageStream == null)
            {
                try
                {
                    imageStream = await StreamHelper.GetPackagedFileStreamAsync(localPath);
                }
                catch
                {
                }
            }

            return imageStream;
        }

        private async Task SaveImageToCache(string localPath, Stream imageStream)
        {
            var folder = ApplicationData.Current.LocalCacheFolder;
            localPath = Path.Combine(folder.Path, localPath);

            // Resort to creating using traditional methods to avoid iteration for folder creation.
            Directory.CreateDirectory(Path.GetDirectoryName(localPath));

            using (var fileStream = File.Create(localPath))
            {
                await imageStream.CopyToAsync(fileStream);
            }
        }

        private string ProcessDocs(string docs)
        {
            string result = docs;

            var metadataRegex = new Regex("^---(.+?)---", RegexOptions.Singleline);
            var metadataMatch = metadataRegex.Match(result);
            if (metadataMatch.Success)
            {
                result = result.Remove(metadataMatch.Index, metadataMatch.Index + metadataMatch.Length);
            }

            // Images
            var regex = new Regex("## Example Image.+?##", RegexOptions.Singleline);
            result = regex.Replace(result, "##");

            return result;
        }

        /// <summary>
        /// Gets a version of the XamlCode with the explicit values of the option controls.
        /// </summary>
        public string UpdatedXamlCode
        {
            get
            {
                if (_propertyDescriptor == null)
                {
                    return string.Empty;
                }

                var result = XamlCode;
                var proxy = (IDictionary<string, object>)_propertyDescriptor.Expando;
                foreach (var option in _propertyDescriptor.Options)
                {
                    if (proxy[option.Name] is ValueHolder value)
                    {
                        var newString = value.Value is Windows.UI.Xaml.Media.SolidColorBrush brush ?
                                            brush.Color.ToString() : value.Value.ToString();

                        result = result.Replace(option.OriginalString, newString);
                        result = result.Replace("@[" + option.Label + "]@", newString);
                        result = result.Replace("@[" + option.Label + "]", newString);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a version of the XamlCode bound directly to the slider/option controls.
        /// </summary>
        public string BindedXamlCode
        {
            get
            {
                if (_propertyDescriptor == null)
                {
                    return string.Empty;
                }

                var result = XamlCode;
                var proxy = (IDictionary<string, object>)_propertyDescriptor.Expando;
                foreach (var option in _propertyDescriptor.Options)
                {
                    if (proxy[option.Name] is ValueHolder value)
                    {
                        result = result.Replace(
                            option.OriginalString,
                            "{Binding " + option.Name + ".Value, Mode=" + (option.IsTwoWayBinding ? "TwoWay" : "OneWay") + "}");
                        result = result.Replace(
                            "@[" + option.Label + "]@",
                            "{Binding " + option.Name + ".Value, Mode=TwoWay}");
                        result = result.Replace(
                            "@[" + option.Label + "]",
                            "{Binding " + option.Name + ".Value, Mode=OneWay}"); // Order important here.
                    }
                }

                return result;
            }
        }

        public PropertyDescriptor PropertyDescriptor => _propertyDescriptor;

        public async Task PreparePropertyDescriptorAsync()
        {
            if (string.IsNullOrEmpty(XamlCodeFile))
            {
                return;
            }

            if (_propertyDescriptor == null)
            {
                // Get Xaml code
                using (var codeStream = await StreamHelper.GetPackagedFileStreamAsync(XamlCodeFile.StartsWith('/') ? XamlCodeFile : $"SamplePages/{Name}/{XamlCodeFile}"))
                {
                    XamlCode = await codeStream.ReadTextAsync(Encoding.UTF8);

                    // Look for @[] values and generate associated properties
                    var regularExpression = new Regex("(?<=\\\")@\\[(?<name>.+?)(:(?<type>.+?):(?<value>.+?)(:(?<parameters>.+?))?(:(?<options>.*))*)?\\]@?(?=\\\")");

                    _propertyDescriptor = new PropertyDescriptor { Expando = new ExpandoObject() };
                    var proxy = (IDictionary<string, object>)_propertyDescriptor.Expando;

                    foreach (Match match in regularExpression.Matches(XamlCode))
                    {
                        var label = match.Groups["name"].Value;
                        var name = label.Replace(" ", string.Empty); // Allow us to have nicer display names, but create valid properties.
                        var type = match.Groups["type"].Value;
                        var value = match.Groups["value"].Value;

                        var existingOption = _propertyDescriptor.Options.Where(o => o.Name == name).FirstOrDefault();

                        if (existingOption == null && string.IsNullOrWhiteSpace(type))
                        {
                            throw new NotSupportedException($"Unrecognized short identifier '{name}'; Define type and parameters of property in first occurrence in {XamlCodeFile}.");
                        }

                        if (Enum.TryParse(type, out PropertyKind kind))
                        {
                            if (existingOption != null)
                            {
                                if (existingOption.Kind != kind)
                                {
                                    throw new NotSupportedException($"Multiple options with same name but different type not supported: {XamlCodeFile}:{name}");
                                }

                                continue;
                            }

                            PropertyOptions options;

                            switch (kind)
                            {
                                case PropertyKind.Slider:
                                case PropertyKind.DoubleSlider:
                                    try
                                    {
                                        var sliderOptions = new SliderPropertyOptions { DefaultValue = double.Parse(value, CultureInfo.InvariantCulture) };
                                        var parameters = match.Groups["parameters"].Value;
                                        var split = parameters.Split('-');
                                        int minIndex = 0;
                                        int minMultiplier = 1;
                                        if (string.IsNullOrEmpty(split[0]))
                                        {
                                            minIndex = 1;
                                            minMultiplier = -1;
                                        }

                                        sliderOptions.MinValue = minMultiplier * double.Parse(split[minIndex], CultureInfo.InvariantCulture);
                                        sliderOptions.MaxValue = double.Parse(split[minIndex + 1], CultureInfo.InvariantCulture);
                                        if (split.Length > 2 + minIndex)
                                        {
                                            sliderOptions.Step = double.Parse(split[split.Length - 1], CultureInfo.InvariantCulture);
                                        }

                                        options = sliderOptions;
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine($"Unable to extract slider info from {value}({ex.Message})");
                                        TrackingManager.TrackException(ex);
                                        continue;
                                    }

                                    break;

                                case PropertyKind.TimeSpan:
                                    try
                                    {
                                        var sliderOptions = new SliderPropertyOptions { DefaultValue = TimeSpan.FromMilliseconds(double.Parse(value, CultureInfo.InvariantCulture)) };
                                        var parameters = match.Groups["parameters"].Value;
                                        var split = parameters.Split('-');
                                        int minIndex = 0;
                                        int minMultiplier = 1;
                                        if (string.IsNullOrEmpty(split[0]))
                                        {
                                            minIndex = 1;
                                            minMultiplier = -1;
                                        }

                                        sliderOptions.MinValue = minMultiplier * double.Parse(split[minIndex], CultureInfo.InvariantCulture);
                                        sliderOptions.MaxValue = double.Parse(split[minIndex + 1], CultureInfo.InvariantCulture);
                                        if (split.Length > 2 + minIndex)
                                        {
                                            sliderOptions.Step = double.Parse(split[split.Length - 1], CultureInfo.InvariantCulture);
                                        }

                                        options = sliderOptions;
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine($"Unable to extract slider info from {value}({ex.Message})");
                                        TrackingManager.TrackException(ex);
                                        continue;
                                    }

                                    break;

                                case PropertyKind.Enum:
                                    try
                                    {
                                        options = new PropertyOptions();
                                        var split = value.Split('.');
                                        var typeName = string.Join(".", split.Take(split.Length - 1));
                                        var enumType = LookForTypeByName(typeName);
                                        options.DefaultValue = Enum.Parse(enumType, split.Last());
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine($"Unable to parse enum from {value}({ex.Message})");
                                        TrackingManager.TrackException(ex);
                                        continue;
                                    }

                                    break;

                                case PropertyKind.Bool:
                                    try
                                    {
                                        options = new PropertyOptions { DefaultValue = bool.Parse(value) };
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine($"Unable to parse bool from {value}({ex.Message})");
                                        continue;
                                    }

                                    break;

                                case PropertyKind.Brush:
                                    try
                                    {
                                        options = new PropertyOptions { DefaultValue = value };
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine($"Unable to parse bool from {value}({ex.Message})");
                                        TrackingManager.TrackException(ex);
                                        continue;
                                    }

                                    break;

                                case PropertyKind.Thickness:
                                    try
                                    {
                                        var thicknessOptions = new ThicknessPropertyOptions { DefaultValue = value };
                                        options = thicknessOptions;
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine($"Unable to extract slider info from {value}({ex.Message})");
                                        TrackingManager.TrackException(ex);
                                        continue;
                                    }

                                    break;

                                default:
                                    options = new PropertyOptions { DefaultValue = value };
                                    break;
                            }

                            options.Label = label;
                            options.Name = name;
                            options.OriginalString = match.Value;
                            options.Kind = kind;
                            options.IsTwoWayBinding = options.OriginalString.EndsWith("@");
                            proxy[name] = new ValueHolder(options.DefaultValue);

                            _propertyDescriptor.Options.Add(options);
                        }
                    }
                }
            }
        }

        private static Type LookForTypeByName(string typeName)
        {
            // First search locally
            if (System.Type.GetType(typeName) is Type systemType)
            {
                return systemType;
            }

            var targets = new Type[]
            {
                VerticalAlignment.Center.GetType(), // Windows
                StackMode.Replace.GetType(), // Microsoft.Toolkit.Uwp.UI.Controls.Core

              // TODO Reintroduce graph controls
              // typeof(UserToPersonConverter)) // Search in Microsoft.Toolkit.Graph.Controls
                ScrollItemPlacement.Default.GetType(), // Search in Microsoft.Toolkit.Uwp.UI
                EasingType.Default.GetType(), // Microsoft.Toolkit.Uwp.UI.Animations
                ImageBlendMode.Multiply.GetType(), // Search in Microsoft.Toolkit.Uwp.UI.Media
                Interaction.Enabled.GetType(), // Microsoft.Toolkit.Uwp.Input.GazeInteraction
                DataGridGridLinesVisibility.None.GetType(), // Microsoft.Toolkit.Uwp.UI.Controls.DataGrid
                GridSplitter.GridResizeDirection.Auto.GetType(), // Microsoft.Toolkit.Uwp.UI.Controls.Layout
                typeof(MarkdownTextBlock), // Microsoft.Toolkit.Uwp.UI.Controls.Markdown
                BitmapFileFormat.Bmp.GetType(), // Microsoft.Toolkit.Uwp.UI.Controls.Media
                typeof(AlphaMode), // Microsoft.Toolkit.Uwp.UI.Media
                StretchChild.Last.GetType() // Microsoft.Toolkit.Uwp.UI.Controls.Primitivs
            };

            return targets.SelectMany(t => t.Assembly.ExportedTypes)
                .FirstOrDefault(t => t.Name == typeName);
        }

        private static async Task<string> GetDocsSHA()
        {
            try
            {
                var branchEndpoint = "https://api.github.com/repos/microsoftdocs/windowscommunitytoolkitdocs/git/refs/heads/live";

                var request = new HttpRequestMessage(HttpMethod.Get, branchEndpoint);
                request.Headers.Add("User-Agent", "Windows Community Toolkit Sample App");

                using (request)
                {
                    using (var response = await client.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var raw = await response.Content.ReadAsStringAsync();
                            Debug.WriteLine(raw);
                            var json = JsonSerializer.Deserialize<GitRef>(raw);
                            return json?.RefObject?.Sha;
                        }
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        public class GitRef
        {
            [JsonPropertyName("object")]
            public GitRefObject RefObject { get; set; }
        }

        public class GitRefObject
        {
            [JsonPropertyName("sha")]
            public string Sha { get; set; }
        }

        public override string ToString()
        {
            return $"SampleApp.Sample<{CategoryName}.{Subcategory}.{Name}>";
        }
    }
}