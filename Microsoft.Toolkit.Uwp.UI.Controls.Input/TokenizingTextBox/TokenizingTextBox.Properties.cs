// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// A text input control that auto-suggests and displays token items.
    /// </summary>
    public partial class TokenizingTextBox : ListViewBase
    {
        /// <summary>
        /// Identifies the <see cref="AutoSuggestBoxStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty AutoSuggestBoxStyleProperty = DependencyProperty.Register(
            nameof(AutoSuggestBoxStyle),
            typeof(Style),
            typeof(TokenizingTextBox),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="AutoSuggestBoxTextBoxStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty AutoSuggestBoxTextBoxStyleProperty = DependencyProperty.Register(
            nameof(AutoSuggestBoxTextBoxStyle),
            typeof(Style),
            typeof(TokenizingTextBox),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TextMemberPath"/> property.
        /// </summary>
        public static readonly DependencyProperty TextMemberPathProperty = DependencyProperty.Register(
            nameof(TextMemberPath),
            typeof(string),
            typeof(TokenizingTextBox),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TokenItemTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty TokenItemTemplateProperty = DependencyProperty.Register(
            nameof(TokenItemTemplate),
            typeof(DataTemplate),
            typeof(TokenizingTextBox),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TokenItemTemplateSelector"/> property.
        /// </summary>
        public static readonly DependencyProperty TokenItemTemplateSelectorProperty = DependencyProperty.Register(
            nameof(TokenItemTemplateSelector),
            typeof(DataTemplateSelector),
            typeof(TokenizingTextBox),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TokenDelimiter"/> property.
        /// </summary>
        public static readonly DependencyProperty TokenDelimiterProperty = DependencyProperty.Register(
            nameof(TokenDelimiter),
            typeof(string),
            typeof(TokenizingTextBox),
            new PropertyMetadata(" "));

        /// <summary>
        /// Identifies the <see cref="TokenSpacing"/> property.
        /// </summary>
        public static readonly DependencyProperty TokenSpacingProperty = DependencyProperty.Register(
            nameof(TokenSpacing),
            typeof(double),
            typeof(TokenizingTextBox),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="PlaceholderText"/> property.
        /// </summary>
        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(
            nameof(PlaceholderText),
            typeof(string),
            typeof(TokenizingTextBox),
            new PropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies the <see cref="QueryIcon"/> property.
        /// </summary>
        public static readonly DependencyProperty QueryIconProperty = DependencyProperty.Register(
            nameof(QueryIcon),
            typeof(IconSource),
            typeof(TokenizingTextBox),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Text"/> property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(TokenizingTextBox),
            new PropertyMetadata(string.Empty, TextPropertyChanged));

        private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TokenizingTextBox ttb && ttb._currentTextEdit != null)
            {
                ttb._currentTextEdit.Text = e.NewValue as string;
            }
        }

        /// <summary>
        /// Identifies the <see cref="SuggestedItemsSource"/> property.
        /// </summary>
        public static readonly DependencyProperty SuggestedItemsSourceProperty = DependencyProperty.Register(
            nameof(SuggestedItemsSource),
            typeof(object),
            typeof(TokenizingTextBox),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SuggestedItemTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty SuggestedItemTemplateProperty = DependencyProperty.Register(
            nameof(SuggestedItemTemplate),
            typeof(DataTemplate),
            typeof(TokenizingTextBox),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SuggestedItemTemplateSelector"/> property.
        /// </summary>
        public static readonly DependencyProperty SuggestedItemTemplateSelectorProperty = DependencyProperty.Register(
            nameof(SuggestedItemTemplateSelector),
            typeof(DataTemplateSelector),
            typeof(TokenizingTextBox),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SuggestedItemContainerStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty SuggestedItemContainerStyleProperty = DependencyProperty.Register(
            nameof(SuggestedItemContainerStyle),
            typeof(Style),
            typeof(TokenizingTextBox),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TabNavigateBackOnArrow"/> property.
        /// </summary>
        public static readonly DependencyProperty TabNavigateBackOnArrowProperty = DependencyProperty.Register(
            nameof(TabNavigateBackOnArrow),
            typeof(bool),
            typeof(TokenizingTextBox),
            new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the Style for the contained AutoSuggestBox template part.
        /// </summary>
        public Style AutoSuggestBoxStyle
        {
            get => (Style)GetValue(AutoSuggestBoxStyleProperty);
            set => SetValue(AutoSuggestBoxStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the Style for the TextBox part of the AutoSuggestBox template part.
        /// </summary>
        public Style AutoSuggestBoxTextBoxStyle
        {
            get => (Style)GetValue(AutoSuggestBoxStyleProperty);
            set => SetValue(AutoSuggestBoxStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the TextMemberPath of the AutoSuggestBox template part.
        /// </summary>
        public string TextMemberPath
        {
            get => (string)GetValue(TextMemberPathProperty);
            set => SetValue(TextMemberPathProperty, value);
        }

        /// <summary>
        /// Gets or sets the template for token items.
        /// </summary>
        public DataTemplate TokenItemTemplate
        {
            get => (DataTemplate)GetValue(TokenItemTemplateProperty);
            set => SetValue(TokenItemTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the template selector for token items.
        /// </summary>
        public DataTemplateSelector TokenItemTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(TokenItemTemplateSelectorProperty);
            set => SetValue(TokenItemTemplateSelectorProperty, value);
        }

        /// <summary>
        /// Gets or sets delimiter used to determine when to process text input as a new token item.
        /// </summary>
        public string TokenDelimiter
        {
            get => (string)GetValue(TokenDelimiterProperty);
            set => SetValue(TokenDelimiterProperty, value);
        }

        /// <summary>
        /// Gets or sets the spacing value used to separate token items.
        /// </summary>
        public double TokenSpacing
        {
            get => (double)GetValue(TokenSpacingProperty);
            set => SetValue(TokenSpacingProperty, value);
        }

        /// <summary>
        /// Gets or sets the PlaceholderText for the AutoSuggestBox template part.
        /// </summary>
        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        /// <summary>
        /// Gets or sets the icon to display in the AutoSuggestBox template part.
        /// </summary>
        public IconSource QueryIcon
        {
            get => (IconSource)GetValue(QueryIconProperty);
            set => SetValue(QueryIconProperty, value);
        }

        /// <summary>
        /// Gets or sets the input text of the currently active text edit.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// Gets or sets the items source for token suggestions.
        /// </summary>
        public object SuggestedItemsSource
        {
            get => GetValue(SuggestedItemsSourceProperty);
            set => SetValue(SuggestedItemsSourceProperty, value);
        }

        /// <summary>
        /// Gets or sets the template for displaying suggested tokens.
        /// </summary>
        public DataTemplate SuggestedItemTemplate
        {
            get => (DataTemplate)GetValue(SuggestedItemTemplateProperty);
            set => SetValue(SuggestedItemTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the template selector for displaying suggested tokens.
        /// </summary>
        public DataTemplateSelector SuggestedItemTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(SuggestedItemTemplateSelectorProperty);
            set => SetValue(SuggestedItemTemplateSelectorProperty, value);
        }

        /// <summary>
        /// Gets or sets the item container style for displaying suggested tokens.
        /// </summary>
        public Style SuggestedItemContainerStyle
        {
            get => (Style)GetValue(SuggestedItemContainerStyleProperty);
            set => SetValue(SuggestedItemContainerStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control will move focus to the previous
        /// control when an arrow key is pressed and selection is at one of the limits in the control.
        /// </summary>
        public bool TabNavigateBackOnArrow
        {
            get => (bool)GetValue(TabNavigateBackOnArrowProperty);
            set => SetValue(TabNavigateBackOnArrowProperty, value);
        }

        /// <summary>
        /// Gets the complete text value of any selection in the control. The result is the same text as would be copied to the clipboard.
        /// </summary>
        public string SelectedTokenText
        {
            get
            {
                return PrepareSelectionForClipboard();
            }
        }
    }
}