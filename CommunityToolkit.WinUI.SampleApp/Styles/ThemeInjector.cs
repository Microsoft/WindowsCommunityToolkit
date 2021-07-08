// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace CommunityToolkit.WinUI.SampleApp.Styles
{
    public static class ThemeInjector
    {
        private static ResourceDictionary darkTheme;

        private static ResourceDictionary lightTheme;

        public static void InjectThemeResources(ResourceDictionary dict)
        {
            var themes = dict.MergedDictionaries.FirstOrDefault(d => d.Source.AbsoluteUri.Contains("Themes.xaml"));
            darkTheme = themes.ThemeDictionaries["Dark"] as ResourceDictionary;
            lightTheme = themes.ThemeDictionaries["Light"] as ResourceDictionary;

            AddAcrylic(new ThemeAcrylic
            {
                Name = "Background-AboutPage-SidePane",
                DarkAcrylic = new AcrylicBrush
                {
                    TintColor = Helpers.ColorHelper.ToColor("#FF333333"),
                    TintOpacity = 0.8,
                    // BackgroundSource = AcrylicBackgroundSource.Backdrop,
                    FallbackColor = Helpers.ColorHelper.ToColor("#FF333333")
                },
                LightAcrylic = new AcrylicBrush
                {
                    TintColor = Colors.White,
                    TintOpacity = 0.8,
                    // BackgroundSource = AcrylicBackgroundSource.Backdrop,
                    FallbackColor = Colors.White
                }
            });

            AddAcrylic(new ThemeAcrylic
            {
                Names = new[] { "Commands-Background" },
                DarkAcrylic = new AcrylicBrush
                {
                    TintColor = Helpers.ColorHelper.ToColor("#FF111111"),
                    TintOpacity = 0.7,
                    // BackgroundSource = AcrylicBackgroundSource.Backdrop,
                    FallbackColor = Helpers.ColorHelper.ToColor("#FF111111")
                },
                LightAcrylic = new AcrylicBrush
                {
                    TintColor = Helpers.ColorHelper.ToColor("#FFDDDDDD"),
                    TintOpacity = 0.6,
                    // BackgroundSource = AcrylicBackgroundSource.Backdrop,
                    FallbackColor = Helpers.ColorHelper.ToColor("#FFDDDDDD")
                }
            });
        }

        private static void AddAcrylic(ThemeAcrylic resource)
        {
            var light = resource?.LightAcrylic;
            var dark = resource?.DarkAcrylic;

            var names = resource.Names;
            if (names == null)
            {
                names = new string[] { resource.Name };
            }

            foreach (var res in names)
            {
                if (light != null)
                {
                    if (light.FallbackColor == null && lightTheme[res] is SolidColorBrush brush)
                    {
                        light.FallbackColor = brush.Color;
                    }

                    lightTheme[res] = light;
                }

                if (dark != null)
                {
                    if (dark.FallbackColor == null && darkTheme[res] is SolidColorBrush brush)
                    {
                        dark.FallbackColor = brush.Color;
                    }

                    darkTheme[res] = dark;
                }
            }
        }

        private class ThemeAcrylic
        {
            public string Name { get; set; }

            public string[] Names { get; set; }

            public AcrylicBrush LightAcrylic { get; set; }

            public AcrylicBrush DarkAcrylic { get; set; }
        }
    }
}