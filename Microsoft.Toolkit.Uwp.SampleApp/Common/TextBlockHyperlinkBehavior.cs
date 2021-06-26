// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text.RegularExpressions;
using Microsoft.Toolkit.Uwp.UI.Behaviors;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.SampleApp.Common
{
    public class TextBlockHyperlinkBehavior : BehaviorBase<TextBlock>
    {
        //// From: http://urlregex.com/
        private static readonly Regex UrlRegex = new Regex(@"(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?", RegexOptions.Compiled);

        private long? _textChangedRegistration;

        public Brush Foreground { get; set; }

        protected override bool Initialize()
        {
            if (_textChangedRegistration == null)
            {
                _textChangedRegistration = AssociatedObject.RegisterPropertyChangedCallback(TextBlock.TextProperty, Text_PropertyChanged);
            }

            return base.Initialize();
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null && _textChangedRegistration != null && _textChangedRegistration.HasValue)
            {
                AssociatedObject.UnregisterPropertyChangedCallback(TextBlock.TextProperty, _textChangedRegistration.Value);
            }

            base.OnDetaching();
        }

        private void Text_PropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (AssociatedObject != null && !string.IsNullOrWhiteSpace(AssociatedObject.Text))
            {
                var first = true;
                var text = AssociatedObject.Text;
                var last_index = 0;

                foreach (Match match in UrlRegex.Matches(text))
                {
                    if (first)
                    {
                        AssociatedObject.Inlines.Clear();

                        first = false;
                    }

                    var left_text = text.Substring(last_index, match.Index - last_index);
                    last_index = match.Index + match.Length;

                    AssociatedObject.Inlines.Add(new Run() { Text = left_text });
                    AssociatedObject.Inlines.Add(new Hyperlink()
                    {
                        Foreground = this.Foreground,
                        NavigateUri = new Uri(match.Value),
                        Inlines =
                        {
                            new Run() { Text = match.Value }
                        }
                    });
                }

                if (!first && last_index < text.Length)
                {
                    var right_text = text.Substring(last_index);

                    AssociatedObject.Inlines.Add(new Run() { Text = right_text });
                }
            }
        }
    }
}