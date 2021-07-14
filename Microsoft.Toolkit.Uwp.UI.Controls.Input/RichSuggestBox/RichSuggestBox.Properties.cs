// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// The RichSuggestBox control extends <see cref="RichEditBox"/> control that suggests and embeds custom data in a rich document.
    /// </summary>
    public partial class RichSuggestBox
    {
        /// <summary>
        /// Identifies the <see cref="PlaceholderText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register(
                nameof(PlaceholderText),
                typeof(string),
                typeof(RichSuggestBox),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies the <see cref="RichEditBoxStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RichEditBoxStyleProperty =
            DependencyProperty.Register(
                nameof(RichEditBoxStyle),
                typeof(Style),
                typeof(RichSuggestBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                nameof(Header),
                typeof(object),
                typeof(RichSuggestBox),
                new PropertyMetadata(null, OnHeaderChanged));

        /// <summary>
        /// Identifies the <see cref="HeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
                nameof(HeaderTemplate),
                typeof(DataTemplate),
                typeof(RichSuggestBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Description"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(
                nameof(Description),
                typeof(object),
                typeof(RichSuggestBox),
                new PropertyMetadata(null, OnDescriptionChanged));

        /// <summary>
        /// Identifies the <see cref="PopupPlacement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupPlacementProperty =
            DependencyProperty.Register(
                nameof(PopupPlacement),
                typeof(SuggestionPopupPlacementMode),
                typeof(RichSuggestBox),
                new PropertyMetadata(SuggestionPopupPlacementMode.Floating, OnSuggestionPopupPlacementChanged));

        /// <summary>
        /// Identifies the <see cref="PopupCornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupCornerRadiusProperty =
            DependencyProperty.Register(
                nameof(PopupCornerRadius),
                typeof(CornerRadius),
                typeof(RichSuggestBox),
                new PropertyMetadata(default(CornerRadius)));

        /// <summary>
        /// Identifies the <see cref="PopupHeader"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupHeaderProperty =
            DependencyProperty.Register(
                nameof(PopupHeader),
                typeof(object),
                typeof(RichSuggestBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="PopupHeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupHeaderTemplateProperty =
            DependencyProperty.Register(
                nameof(PopupHeaderTemplate),
                typeof(DataTemplate),
                typeof(RichSuggestBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="PopupFooter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupFooterProperty =
            DependencyProperty.Register(
                nameof(PopupFooter),
                typeof(object),
                typeof(RichSuggestBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="PopupFooterTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupFooterTemplateProperty =
            DependencyProperty.Register(
                nameof(PopupFooterTemplate),
                typeof(DataTemplate),
                typeof(RichSuggestBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TokenBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TokenBackgroundProperty =
            DependencyProperty.Register(
                nameof(TokenBackground),
                typeof(SolidColorBrush),
                typeof(RichSuggestBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TokenForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TokenForegroundProperty =
            DependencyProperty.Register(
                nameof(TokenForeground),
                typeof(SolidColorBrush),
                typeof(RichSuggestBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Prefixes"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PrefixesProperty =
            DependencyProperty.Register(
                nameof(Prefixes),
                typeof(string),
                typeof(RichSuggestBox),
                new PropertyMetadata(string.Empty, OnPrefixesChanged));

        /// <summary>
        /// Identifies the <see cref="ClipboardPasteFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ClipboardPasteFormatProperty =
            DependencyProperty.Register(
                nameof(ClipboardPasteFormat),
                typeof(RichEditClipboardFormat),
                typeof(RichSuggestBox),
                new PropertyMetadata(RichEditClipboardFormat.AllFormats));

        /// <summary>
        /// Identifies the <see cref="ClipboardCopyFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ClipboardCopyFormatProperty =
            DependencyProperty.Register(
                nameof(ClipboardCopyFormat),
                typeof(RichEditClipboardFormat),
                typeof(RichSuggestBox),
                new PropertyMetadata(RichEditClipboardFormat.AllFormats));

        /// <summary>
        /// Identifies the <see cref="DisabledFormattingAccelerators"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisabledFormattingAcceleratorsProperty =
            DependencyProperty.Register(
                nameof(DisabledFormattingAccelerators),
                typeof(DisabledFormattingAccelerators),
                typeof(RichSuggestBox),
                new PropertyMetadata(DisabledFormattingAccelerators.None));

        /// <summary>
        /// Gets or sets the text that is displayed in the control until the value is changed by a user action or some other operation.
        /// </summary>
        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        /// <summary>
        /// Gets or sets the style of the underlying <see cref="RichEditBox"/>.
        /// </summary>
        public Style RichEditBoxStyle
        {
            get => (Style)GetValue(RichEditBoxStyleProperty);
            set => SetValue(RichEditBoxStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the content for the control's header.
        /// </summary>
        /// <remarks>
        /// Suggestion popup relies on the actual size of the text control to calculate its placement on the screen.
        /// It is recommended to set the header using this property instead of using <see cref="RichEditBox.Header"/>.
        /// </remarks>
        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display the content of the control's header.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets content that is shown below the control. The content should provide guidance about the input expected by the control.
        /// </summary>
        /// <remarks>
        /// Suggestion popup relies on the actual size of the text control to calculate its placement on the screen.
        /// It is recommended to set the description using this property instead of using <see cref="RichEditBox.Description"/>.
        /// </remarks>
        public object Description
        {
            get => GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        /// <summary>
        /// Gets or sets suggestion popup placement to either Floating or Attached to the text box.
        /// </summary>
        public SuggestionPopupPlacementMode PopupPlacement
        {
            get => (SuggestionPopupPlacementMode)GetValue(PopupPlacementProperty);
            set => SetValue(PopupPlacementProperty, value);
        }

        /// <summary>
        /// Gets or sets the radius for the corners of the popup control's border.
        /// </summary>
        public CornerRadius PopupCornerRadius
        {
            get => (CornerRadius)GetValue(PopupCornerRadiusProperty);
            set => SetValue(PopupCornerRadiusProperty, value);
        }

        /// <summary>
        /// Gets or sets the content for the suggestion popup control's header.
        /// </summary>
        public object PopupHeader
        {
            get => GetValue(PopupHeaderProperty);
            set => SetValue(PopupHeaderProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display the content of the suggestion popup control's header.
        /// </summary>
        public DataTemplate PopupHeaderTemplate
        {
            get => (DataTemplate)GetValue(PopupHeaderTemplateProperty);
            set => SetValue(PopupHeaderTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the content for the suggestion popup control's footer.
        /// </summary>
        public object PopupFooter
        {
            get => GetValue(PopupFooterProperty);
            set => SetValue(PopupFooterProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display the content of the suggestion popup control's footer.
        /// </summary>
        public DataTemplate PopupFooterTemplate
        {
            get => (DataTemplate)GetValue(PopupFooterTemplateProperty);
            set => SetValue(PopupFooterTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the default brush used to color the suggestion token background.
        /// </summary>
        public SolidColorBrush TokenBackground
        {
            get => (SolidColorBrush)GetValue(TokenBackgroundProperty);
            set => SetValue(TokenBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the default brush used to color the suggestion token foreground.
        /// </summary>
        public SolidColorBrush TokenForeground
        {
            get => (SolidColorBrush)GetValue(TokenForegroundProperty);
            set => SetValue(TokenForegroundProperty, value);
        }

        /// <summary>
        /// Gets or sets prefix characters to start a query.
        /// </summary>
        public string Prefixes
        {
            get => (string)GetValue(PrefixesProperty);
            set => SetValue(PrefixesProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that specifies whether pasted text preserves all formats, or as plain text only.
        /// </summary>
        public RichEditClipboardFormat ClipboardPasteFormat
        {
            get => (RichEditClipboardFormat)GetValue(ClipboardPasteFormatProperty);
            set => SetValue(ClipboardPasteFormatProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that specifies whether text is copied with all formats, or as plain text only.
        /// </summary>
        public RichEditClipboardFormat ClipboardCopyFormat
        {
            get => (RichEditClipboardFormat)GetValue(ClipboardCopyFormatProperty);
            set => SetValue(ClipboardCopyFormatProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates which keyboard shortcuts for formatting are disabled.
        /// </summary>
        public DisabledFormattingAccelerators DisabledFormattingAccelerators
        {
            get => (DisabledFormattingAccelerators)GetValue(DisabledFormattingAcceleratorsProperty);
            set => SetValue(DisabledFormattingAcceleratorsProperty, value);
        }

        /// <summary>
        /// Gets object used for lock
        /// </summary>
        protected object LockObj { get; }

        /// <summary>
        /// Gets an object that enables access to the text object model for the text contained in a <see cref="RichEditBox"/>.
        /// </summary>
        public RichEditTextDocument TextDocument => _richEditBox?.TextDocument;

        /// <summary>
        /// Gets the distance the content has been scrolled horizontally from the underlying <see cref="ScrollViewer"/>.
        /// </summary>
        public double HorizontalOffset => this._scrollViewer?.HorizontalOffset ?? 0;

        /// <summary>
        /// Gets the distance the content has been scrolled vertically from the underlying <see cref="ScrollViewer"/>.
        /// </summary>
        public double VerticalOffset => this._scrollViewer?.VerticalOffset ?? 0;

        /// <summary>
        /// Gets a collection of suggestion tokens that are present in the document.
        /// </summary>
        public ReadOnlyObservableCollection<RichSuggestToken> Tokens { get; }
    }
}
