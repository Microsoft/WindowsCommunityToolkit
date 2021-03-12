// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Uwp.SampleApp.SamplePages.TextToolbarSamples;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarButtons;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarFormats.MarkDown;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarFormats.RichText;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    public sealed partial class TextToolbarPage : IXamlRenderListener
    {
        private TextToolbar _toolbar;
        private MarkdownTextBlock _previewer;

        public TextToolbarPage()
        {
            InitializeComponent();
            Load();
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            _toolbar = control.FindChild("Toolbar") as TextToolbar;

            if (control.FindChild("EditZone") is RichEditBox editZone)
            {
                editZone.TextChanged += EditZone_TextChanged;
            }

            if (control.FindChild("Previewer") is MarkdownTextBlock previewer)
            {
                _previewer = previewer;
                _previewer.LinkClicked += Previewer_LinkClicked;
            }
        }

        private void Load()
        {
            SampleController.Current.RegisterNewCommand("Add/Remove Bold Button", (sender, args) =>
            {
                var button = _toolbar?.GetDefaultButton(ButtonType.Bold);
                if (button != null)
                {
                    button.Visibility = button.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                }
            });

            SampleController.Current.RegisterNewCommand("Add Custom Button", (sender, args) =>
            {
                AddCustomButton();
            });

            SampleController.Current.RegisterNewCommand("Use RichText Formatter", (sender, args) =>
            {
                UseRichTextFormatter();
            });

            SampleController.Current.RegisterNewCommand("Use MarkDown Formatter", (sender, args) =>
            {
                UseMarkDownFormatter();
            });

            SampleController.Current.RegisterNewCommand("Use Custom Formatter", (sender, args) =>
            {
                UseCustomFormatter();
            });

            SampleController.Current.RegisterNewCommand("Reset Layout", (sender, args) =>
            {
                ResetLayout();
            });
        }

        private void ResetLayout()
        {
            if (_toolbar == null)
            {
                return;
            }

            _toolbar.CustomButtons.Clear();
            if (_toolbar.DefaultButtons != null)
            {
                foreach (var item in _toolbar.DefaultButtons)
                {
                    if (item is ToolbarButton button)
                    {
                        button.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void UseRichTextFormatter()
        {
            if (_toolbar == null)
            {
                return;
            }

            _toolbar.Formatter = new RichTextFormatter();
        }

        private void UseMarkDownFormatter()
        {
            if (_toolbar == null)
            {
                return;
            }

            _toolbar.Formatter = new MarkDownFormatter();
        }

        private void UseCustomFormatter()
        {
            if (_toolbar == null)
            {
                return;
            }

            _toolbar.Formatter = new SampleFormatter();
        }

        private void AddCustomButton()
        {
            if (_toolbar == null)
            {
                return;
            }

            string demoText = "Demo";
            demoText = DemoCounter > 0 ? demoText + DemoCounter : demoText;

            int keycode = (int)VirtualKey.Number0 + DemoCounter;
            VirtualKey? shortcut = null;

            if (keycode <= (int)VirtualKey.Number9)
            {
                shortcut = (VirtualKey)keycode;
            }

            DemoCounter++;

            var demoButton = new ToolbarButton
            {
                Icon = new SymbolIcon { Symbol = Symbol.ReportHacked },
                ToolTip = demoText,
                ShortcutKey = shortcut,
                Activation = (b) =>
                {
                    if (_toolbar.Formatter is MarkDownFormatter md)
                    {
                        md.SetSelection($"[{demoText}]", $"[/{demoText}]");
                    }
                    else
                    {
                        _toolbar.Formatter.Selected.Text = $"This was filled by {demoText} button ";

                        _toolbar.Formatter.Selected.CharacterFormat.Size = 40;
                    }
                }
            };

            _toolbar.CustomButtons.Add(demoButton);
        }

        private void Previewer_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                var link = new Uri(e.Link);
                _ = Launcher.LaunchUriAsync(link);
            }
            catch
            {
            }
        }

        private void EditZone_TextChanged(object sender, RoutedEventArgs e)
        {
            if (_toolbar == null || _previewer == null)
            {
                return;
            }

            if (_toolbar.Formatter is MarkDownFormatter md)
            {
                string text = md.Text;
                _previewer.Text = string.IsNullOrWhiteSpace(text) ? "Nothing to Preview" : text;
            }
        }

        private int DemoCounter { get; set; } = 0;
    }
}
