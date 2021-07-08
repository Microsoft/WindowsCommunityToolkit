// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using ColorCode;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

namespace CommunityToolkit.WinUI.SampleApp.Controls
{
    public class CodeRenderer : Control
    {
        private RichTextBlockFormatter _formatter;
        private RichTextBlock _codeView;
        private Button _printButton;
        private Button _copyButton;
        private PrintHelper _printHelper;
        private Grid _container;

        private string _displayedText;
        private ILanguage _language;
        private bool _rendered;
        private ElementTheme _theme;

        public CodeRenderer()
        {
            DefaultStyleKey = typeof(CodeRenderer);
            _theme = SystemTheme();
            SampleController.Current.ThemeChanged += Current_ThemeChanged;
        }

        /// <summary>
        /// Renders the Code with Syntax Highlighting of the provided Language Alias.
        /// </summary>
        /// <param name="code">Code to Render</param>
        /// <param name="language">Language Alias</param>
        public void SetCode(string code, string language)
        {
            _rendered = false;
            _language = Languages.FindById(language);
            _displayedText = code;

            if (_codeView != null)
            {
                RenderDocument();
            }
        }

        protected override void OnApplyTemplate()
        {
            if (_printButton != null)
            {
                _printButton.Click -= PrintButton_Click;
            }

            if (_copyButton != null)
            {
                _copyButton.Click -= CopyButton_Click;
            }

            _codeView = GetTemplateChild("codeView") as RichTextBlock;
            _printButton = GetTemplateChild("PrintButton") as Button;
            _copyButton = GetTemplateChild("CopyButton") as Button;
            _container = GetTemplateChild("Container") as Grid;

            if (_printButton != null)
            {
                _printButton.Click += PrintButton_Click;
            }

            if (_copyButton != null)
            {
                _copyButton.Click += CopyButton_Click;
            }

            if (!_rendered)
            {
                RenderDocument();
            }

            base.OnApplyTemplate();
        }

        private void RenderDocument()
        {
            if (_codeView != null)
            {
                _codeView.Blocks?.Clear();
                _formatter = new RichTextBlockFormatter(_theme);

                _formatter.FormatRichTextBlock(_displayedText, _language, _codeView);
                _rendered = true;
            }
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            TrackingManager.TrackEvent("Copy", _displayedText);

            var content = new DataPackage();
            content.SetText(_displayedText);
            Clipboard.SetContent(content);
        }

        private async void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            SampleController.Current.DisplayWaitRing = true;

            var printBlock = new RichTextBlock
            {
                FontFamily = _codeView.FontFamily,
                RequestedTheme = ElementTheme.Light
            };
            var printFormatter = new RichTextBlockFormatter(ElementTheme.Light);
            printFormatter.FormatRichTextBlock(_displayedText, _language, printBlock);

            _printHelper = new PrintHelper(_container);
            _printHelper.AddFrameworkElementToPrint(printBlock);

            _printHelper.OnPrintFailed += PrintHelper_OnPrintFailed;
            _printHelper.OnPrintSucceeded += PrintHelper_OnPrintSucceeded;
            _printHelper.OnPrintCanceled += PrintHelper_OnPrintCanceled;

            await _printHelper.ShowPrintUIAsync("Windows Community Toolkit Sample App");
        }

        private void ReleasePrintHelper()
        {
            _printHelper.OnPrintFailed -= PrintHelper_OnPrintFailed;
            _printHelper.OnPrintSucceeded -= PrintHelper_OnPrintSucceeded;
            _printHelper.OnPrintCanceled -= PrintHelper_OnPrintCanceled;

            _printHelper.Dispose();

            SampleController.Current.DisplayWaitRing = false;
        }

        private async void PrintHelper_OnPrintSucceeded()
        {
            ReleasePrintHelper();
            var dialog = new ContentDialog
            {
                Title = "Windows Community Toolkit Sample App",
                Content = "Printing done.",
                CloseButtonText = "Close",
                XamlRoot = XamlRoot
            };
            await dialog.ShowAsync();
        }

        private async void PrintHelper_OnPrintFailed()
        {
            ReleasePrintHelper();
            var dialog = new ContentDialog
            {
                Title = "Windows Community Toolkit Sample App",
                Content = "Printing failed.",
                CloseButtonText = "Close",
                XamlRoot = XamlRoot
            };
            await dialog.ShowAsync();
        }

        private void PrintHelper_OnPrintCanceled()
        {
            ReleasePrintHelper();
        }

        private void Current_ThemeChanged(object sender, Models.ThemeChangedArgs e)
        {
            // Only Handle System Theme Changes.
            if (e.CustomSet)
            {
                return;
            }

            _theme = SystemTheme();
            try
            {
                _rendered = false;
                RenderDocument();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Casts Application Theme to Element Theme, as Formatter accepts ElementTheme.
        /// </summary>
        /// <returns>Element Theme</returns>
        private ElementTheme SystemTheme()
        {
            return SampleController.Current.SystemTheme() == ApplicationTheme.Dark ? ElementTheme.Dark : ElementTheme.Light;
        }
    }
}