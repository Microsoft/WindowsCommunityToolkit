// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Toolkit;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI;
using UITests.App.Pages;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UITests.App.Commands
{
    public static class VisualTreeHelperCommands
    {
        private static DispatcherQueue Queue { get; set; }

        private static JsonSerializerOptions SerializerOptions { get; } = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        };

        public static void Initialize(DispatcherQueue uiThread)
        {
            Queue = uiThread;

            (App.Current as App).RegisterCustomCommand("VisualTreeHelper.FindElementProperty", FindElementProperty);
        }

        public static async Task<ValueSet> FindElementProperty(ValueSet arguments)
        {
            ValueSet results = new ValueSet();

            if (Queue == null)
            {
                Log.Error("VisualTreeHelper - Missing UI DispatcherQueue");
                return null;
            }

            await Queue.EnqueueAsync(() =>
            {
                // Dispatch?
                var content = Window.Current.Content as Frame;

                if (content == null)
                {
                    Log.Error("VisualTreeHelper.FindElementProperty - Window has no content.");
                    return;
                }

                if (arguments.TryGetValue("ElementName", out object value) && value is string name &&
                    arguments.TryGetValue("Property", out object value2) && value2 is string propertyName)
                {
                    Log.Comment("VisualTreeHelper.FindElementProperty('{0}', '{1}')", name, propertyName);

                    // 1. Find Element in Visual Tree
                    var element = content.FindDescendant(name);

                    try
                    {
                        Log.Comment("VisualTreeHelper.FindElementProperty - Found Element? {0}", element != null);

                        var typeinfo = element.GetType().GetTypeInfo();

                        Log.Comment("Element Type: {0}", typeinfo.FullName);

                        var prop = element.GetType().GetTypeInfo().GetProperty(propertyName);

                        if (prop == null)
                        {
                            Log.Error("VisualTreeHelper.FindElementProperty - Couldn't find Property named {0} on type {1}", propertyName, typeinfo.FullName);
                            return;
                        }

                        // 2. Get the property using reflection
                        var propValue = prop.GetValue(element);

                        // 3. Serialize and return the result
                        results.Add("Result", JsonSerializer.Serialize(propValue, SerializerOptions));
                    }
                    catch (Exception e)
                    {
                        Log.Error("Error {0}", e.Message);
                        Log.Error("StackTrace:\n{0}", e.StackTrace);
                    }
                }
            });

            if (results.Count > 0)
            {
                return results;
            }

            return null; // Failure
        }
    }
}
