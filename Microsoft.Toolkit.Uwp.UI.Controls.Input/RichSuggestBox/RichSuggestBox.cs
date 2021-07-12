// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Deferred;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Input;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// The RichSuggestBox control extends <see cref="RichEditBox"/> control that suggests and embeds custom data in a rich document.
    /// </summary>
    [TemplatePart(Name = PartRichEditBox, Type = typeof(RichEditBox))]
    [TemplatePart(Name = PartSuggestionsPopup, Type = typeof(Popup))]
    [TemplatePart(Name = PartSuggestionsList, Type = typeof(ListViewBase))]
    [TemplatePart(Name = PartSuggestionsContainer, Type = typeof(Border))]
    [TemplatePart(Name = PartHeaderContentPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PartDescriptionPresenter, Type = typeof(ContentPresenter))]
    public partial class RichSuggestBox : ItemsControl
    {
        private const string PartRichEditBox = "RichEditBox";
        private const string PartSuggestionsPopup = "SuggestionsPopup";
        private const string PartSuggestionsList = "SuggestionsList";
        private const string PartSuggestionsContainer = "SuggestionsContainer";
        private const string PartHeaderContentPresenter = "HeaderContentPresenter";
        private const string PartDescriptionPresenter = "DescriptionPresenter";

        private readonly Dictionary<string, RichSuggestToken> _tokens;
        private readonly ObservableCollection<RichSuggestToken> _visibleTokens;

        private Popup _suggestionPopup;
        private RichEditBox _richEditBox;
        private ListViewBase _suggestionsList;
        private Border _suggestionsContainer;

        private int _suggestionChoice;
        private string _currentQuery;
        private string _currentPrefix;
        private bool _ignoreChange;
        private bool _popupOpenDown;
        private bool _tokenAtStart;
        private bool _textCompositionActive;
        private ITextRange _currentRange;
        private RichSuggestToken _hoveringToken;
        private CancellationTokenSource _suggestionRequestedCancellationSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="RichSuggestBox"/> class.
        /// </summary>
        public RichSuggestBox()
        {
            _tokens = new Dictionary<string, RichSuggestToken>();
            _visibleTokens = new ObservableCollection<RichSuggestToken>();
            Tokens = new ReadOnlyObservableCollection<RichSuggestToken>(_visibleTokens);
            LockObj = new object();

            DefaultStyleKey = typeof(RichSuggestBox);

            RegisterPropertyChangedCallback(ItemsSourceProperty, ItemsSource_PropertyChanged);
            RegisterPropertyChangedCallback(CornerRadiusProperty, OnCornerRadiusChanged);
            RegisterPropertyChangedCallback(PopupCornerRadiusProperty, OnCornerRadiusChanged);
            LostFocus += OnLostFocus;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RichSuggestBox"/> class for unit tests.
        /// </summary>
        /// <param name="richEditBox"><see cref="RichEditBox"/> instance to be used for <see cref="TextDocument"/>.</param>
        internal RichSuggestBox(RichEditBox richEditBox)
            : this()
        {
            _richEditBox = richEditBox;
        }

        /// <summary>
        /// Clear unused tokens and undo/redo history. <see cref="RichSuggestBox"/> saves all of previously committed tokens
        /// even when they are removed from the text. They have to be manually removed using this method.
        /// </summary>
        public void ClearUndoRedoSuggestionHistory()
        {
            TextDocument.ClearUndoRedoHistory();
            lock (LockObj)
            {
                if (_tokens.Count == 0)
                {
                    return;
                }

                var keysToDelete = _tokens.Where(pair => !pair.Value.Active).Select(pair => pair.Key).ToArray();
                foreach (var key in keysToDelete)
                {
                    _tokens.Remove(key);
                }
            }
        }

        /// <summary>
        /// Try getting the token associated with a text range.
        /// </summary>
        /// <param name="range">The range of the token to get.</param>
        /// <param name="token">When this method returns, contains the token associated with the specified range; otherwise, it is null.</param>
        /// <returns>true if there is a token associated with the text range; otherwise false.</returns>
        public bool TryGetTokenFromRange(ITextRange range, out RichSuggestToken token)
        {
            token = null;
            range = range.GetClone();
            return range != null && !string.IsNullOrEmpty(range.Link) && _tokens.TryGetValue(range.Link, out token);
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PointerEventHandler pointerPressedHandler = RichEditBox_OnPointerPressed;
            PointerEventHandler pointerMovedHandler = RichEditBox_OnPointerMoved;

            _suggestionPopup = (Popup)GetTemplateChild(PartSuggestionsPopup);
            _richEditBox = (RichEditBox)GetTemplateChild(PartRichEditBox);
            _suggestionsList = (ListViewBase)GetTemplateChild(PartSuggestionsList);
            _suggestionsContainer = (Border)GetTemplateChild(PartSuggestionsContainer);
            ConditionallyLoadElement(Header, PartHeaderContentPresenter);
            ConditionallyLoadElement(Description, PartDescriptionPresenter);

            _richEditBox.SizeChanged -= RichEditBox_SizeChanged;
            _richEditBox.TextChanging -= RichEditBox_TextChanging;
            _richEditBox.TextChanged -= RichEditBox_TextChanged;
            _richEditBox.TextCompositionStarted -= RichEditBox_TextCompositionStarted;
            _richEditBox.TextCompositionChanged -= RichEditBox_TextCompositionChanged;
            _richEditBox.TextCompositionEnded -= RichEditBox_TextCompositionEnded;
            _richEditBox.SelectionChanging -= RichEditBox_SelectionChanging;
            _richEditBox.SelectionChanged -= RichEditBox_SelectionChanged;
            _richEditBox.Paste -= RichEditBox_Paste;
            _richEditBox.PreviewKeyDown -= RichEditBox_PreviewKeyDown;
            _richEditBox.PointerExited -= RichEditBox_OnPointerExited;
            _richEditBox.RemoveHandler(PointerMovedEvent, pointerMovedHandler);
            _richEditBox.RemoveHandler(PointerPressedEvent, pointerPressedHandler);
            _richEditBox.ProcessKeyboardAccelerators -= RichEditBox_ProcessKeyboardAccelerators;

            _richEditBox.SizeChanged += RichEditBox_SizeChanged;
            _richEditBox.TextChanging += RichEditBox_TextChanging;
            _richEditBox.TextChanged += RichEditBox_TextChanged;
            _richEditBox.TextCompositionStarted += RichEditBox_TextCompositionStarted;
            _richEditBox.TextCompositionChanged += RichEditBox_TextCompositionChanged;
            _richEditBox.TextCompositionEnded += RichEditBox_TextCompositionEnded;
            _richEditBox.SelectionChanging += RichEditBox_SelectionChanging;
            _richEditBox.SelectionChanged += RichEditBox_SelectionChanged;
            _richEditBox.Paste += RichEditBox_Paste;
            _richEditBox.PreviewKeyDown += RichEditBox_PreviewKeyDown;
            _richEditBox.PointerExited += RichEditBox_OnPointerExited;
            _richEditBox.AddHandler(PointerMovedEvent, pointerMovedHandler, true);
            _richEditBox.AddHandler(PointerPressedEvent, pointerPressedHandler, true);
            _richEditBox.ProcessKeyboardAccelerators += RichEditBox_ProcessKeyboardAccelerators;

            _suggestionsList.ItemClick -= SuggestionsList_ItemClick;
            _suggestionsList.SizeChanged -= SuggestionsList_SizeChanged;
            _suggestionsList.GotFocus -= SuggestionList_GotFocus;

            _suggestionsList.ItemClick += SuggestionsList_ItemClick;
            _suggestionsList.SizeChanged += SuggestionsList_SizeChanged;
            _suggestionsList.GotFocus += SuggestionList_GotFocus;
        }

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (RichSuggestBox)d;
            view.ConditionallyLoadElement(e.NewValue, PartHeaderContentPresenter);
        }

        private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (RichSuggestBox)d;
            view.ConditionallyLoadElement(e.NewValue, PartDescriptionPresenter);
        }

        private static void OnSuggestionPopupPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (RichSuggestBox)d;
            view.UpdatePopupWidth();
        }

        private static void OnPrefixesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (RichSuggestBox)d;

            var newValue = (string)e.NewValue;
            var prefixes = EnforcePrefixesRequirements(newValue);

            if (newValue != prefixes)
            {
                view.SetValue(PrefixesProperty, prefixes);
            }
        }

        private void OnCornerRadiusChanged(DependencyObject sender, DependencyProperty dp)
        {
            UpdateCornerRadii();
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            ShowSuggestionsPopup(false);
        }

        private void SuggestionsList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this._suggestionPopup.IsOpen)
            {
                this.UpdatePopupOffset();
            }
        }

        private void SuggestionList_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_richEditBox != null)
            {
                _richEditBox.Focus(FocusState.Programmatic);
            }
        }

        private void RichEditBox_OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            this._hoveringToken = null;
        }

        private void RichEditBox_OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var pointer = e.GetCurrentPoint((UIElement)sender);
            InvokeTokenHovered(pointer.Position, e.GetCurrentPoint(this));
        }

        private void RichEditBox_SelectionChanging(RichEditBox sender, RichEditBoxSelectionChangingEventArgs args)
        {
            TextDocument.BeginUndoGroup();

            var selection = TextDocument.Selection;
            if (selection.Type != SelectionType.InsertionPoint)
            {
                return;
            }

            var range = selection.GetClone();
            range.Expand(TextRangeUnit.Link);
            if (!_tokens.ContainsKey(range.Link))
            {
                return;
            }

            if (selection.StartPosition == 0 && this._tokens.ContainsKey(selection.GetClone().Link))
            {
                selection.CharacterFormat = TextDocument.GetDefaultCharacterFormat();
            }

            if (range.StartPosition < selection.StartPosition && selection.EndPosition < range.EndPosition)
            {
                // Snap selection to token on click
                selection.SetRange(range.StartPosition, range.EndPosition);
                InvokeTokenSelected(selection);
            }
            else if (selection.StartPosition == range.StartPosition)
            {
                // Reset formatting if selection is sandwiched between 2 adjacent links
                // or if the link is at the beginning of the document
                range.MoveStart(TextRangeUnit.Link, -1);
                if (selection.StartPosition != range.StartPosition || selection.StartPosition == 0)
                {
                    selection.CharacterFormat = TextDocument.GetDefaultCharacterFormat();
                }
            }
        }

        private async void RichEditBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
            var selection = TextDocument.Selection;
            _tokenAtStart = selection.StartPosition == 0 &&
                            selection.Type == SelectionType.Normal &&
                            _tokens.ContainsKey(selection.GetClone().Link);

            // During text composition changing (e.g. user typing with an IME),
            // SelectionChanged event is fired multiple times with each keystroke.
            // To reduce the number of suggestion requests, the request is made
            // in TextCompositionChanged handler instead.
            if (_textCompositionActive)
            {
                return;
            }

            await RequestSuggestionsAsync();
        }

        private void RichEditBox_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ShowSuggestionsPopup(false);
        }

        private async void RichEditBox_ProcessKeyboardAccelerators(UIElement sender, ProcessKeyboardAcceleratorEventArgs args)
        {
            var itemsList = _suggestionsList.Items;
            if (!_suggestionPopup.IsOpen || itemsList == null || itemsList.Count == 0)
            {
                return;
            }

            var key = args.Key;
            switch (key)
            {
                case VirtualKey.Up when itemsList.Count == 1:
                case VirtualKey.Down when itemsList.Count == 1:
                    args.Handled = true;
                    UpdateSuggestionsListSelectedItem(1);
                    break;

                case VirtualKey.Up:
                    args.Handled = true;
                    _suggestionChoice = _suggestionChoice <= 0 ? itemsList.Count : _suggestionChoice - 1;
                    UpdateSuggestionsListSelectedItem(this._suggestionChoice);
                    break;

                case VirtualKey.Down:
                    args.Handled = true;
                    _suggestionChoice = _suggestionChoice >= itemsList.Count ? 0 : _suggestionChoice + 1;
                    UpdateSuggestionsListSelectedItem(this._suggestionChoice);
                    break;

                case VirtualKey.Enter when _suggestionsList.SelectedItem != null:
                    args.Handled = true;
                    await OnSuggestionSelectedAsync(_suggestionsList.SelectedItem);
                    break;

                case VirtualKey.Escape:
                    args.Handled = true;
                    ShowSuggestionsPopup(false);
                    break;
            }
        }

        private async void RichEditBox_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Tab && _suggestionsList.SelectedItem != null)
            {
                e.Handled = true;
                await OnSuggestionSelectedAsync(_suggestionsList.SelectedItem);
            }
        }

        private async void SuggestionsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedItem = e.ClickedItem;
            await OnSuggestionSelectedAsync(selectedItem);
        }

        private void RichEditBox_TextChanging(RichEditBox sender, RichEditBoxTextChangingEventArgs args)
        {
            if (_ignoreChange || !args.IsContentChanging)
            {
                return;
            }

            _ignoreChange = true;
            ValidateTokensInDocument();
            ResetFormatAfterTokenRemoveAtStart();
            TextDocument.EndUndoGroup();
            TextDocument.BeginUndoGroup();
            _ignoreChange = false;
        }

        private void RichEditBox_TextChanged(object sender, RoutedEventArgs e)
        {
            TextChanged?.Invoke(this, e);
            UpdateVisibleTokenList();
        }

        private void RichEditBox_TextCompositionStarted(RichEditBox sender, TextCompositionStartedEventArgs args)
        {
            _textCompositionActive = true;
        }

        private async void RichEditBox_TextCompositionChanged(RichEditBox sender, TextCompositionChangedEventArgs args)
        {
            var range = TextDocument.GetRange(args.StartIndex == 0 ? 0 : args.StartIndex - 1, args.StartIndex + args.Length);
            await RequestSuggestionsAsync(range);
        }

        private void RichEditBox_TextCompositionEnded(RichEditBox sender, TextCompositionEndedEventArgs args)
        {
            _textCompositionActive = false;
        }

        private void RichEditBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdatePopupWidth();
            this.UpdatePopupOffset();
        }

        private void ItemsSource_PropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            _suggestionChoice = 0;
            ShowSuggestionsPopup(_suggestionsList?.Items?.Count > 0);
        }

        private async void RichEditBox_Paste(object sender, TextControlPasteEventArgs e)
        {
            Paste?.Invoke(this, e);

            if (e.Handled || TextDocument == null || ClipboardPasteFormat != RichEditClipboardFormat.PlainText)
            {
                return;
            }

            e.Handled = true;
            var dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                var text = await dataPackageView.GetTextAsync();
                TextDocument.Selection.SetText(TextSetOptions.None, text);
                TextDocument.Selection.Collapse(false);
            }
        }

        private void InvokeTokenSelected(ITextSelection selection)
        {
            if (!TryGetTokenFromRange(selection, out var token) || token.RangeEnd != selection.EndPosition)
            {
                return;
            }

            var tokenRect = GetTokenRect(selection);
            TokenSelected?.Invoke(this, new RichSuggestTokenSelectedEventArgs
            {
                Token = token,
                Rect = tokenRect,
                Range = selection.GetClone()
            });
        }

        private void InvokeTokenHovered(Point pointerPosition, PointerPoint passingPointerPoint)
        {
            var padding = _richEditBox.Padding;
            pointerPosition.X -= padding.Left;
            pointerPosition.Y -= padding.Top;
            var range = TextDocument.GetRangeFromPoint(pointerPosition, PointOptions.ClientCoordinates);
            RichSuggestToken token = null;
            if (range.Expand(TextRangeUnit.Link) > 0 && TryGetTokenFromRange(range, out token) &&
                token != _hoveringToken)
            {
                TokenHovered?.Invoke(this, new RichSuggestTokenHoveredEventArgs
                {
                    Token = token,
                    Rect = GetTokenRect(range),
                    Range = range,
                    CurrentPoint = passingPointerPoint
                });
            }

            _hoveringToken = token;
        }

        private async Task RequestSuggestionsAsync(ITextRange range = null)
        {
            string prefix;
            string query;
            var queryFound = range == null
                ? TryExtractQueryFromSelection(out prefix, out query, out range)
                : TryExtractQueryFromRange(range, out prefix, out query);

            if (queryFound && prefix == _currentPrefix && query == _currentQuery &&
                range.EndPosition == _currentRange?.EndPosition && _suggestionPopup.IsOpen)
            {
                return;
            }

            CancelIfNotDisposed(this._suggestionRequestedCancellationSource);
            this._suggestionRequestedCancellationSource = null;

            if (queryFound && SuggestionsRequested != null)
            {
                using var tokenSource = new CancellationTokenSource();
                _suggestionRequestedCancellationSource = tokenSource;
                _currentPrefix = prefix;
                _currentQuery = query;
                _currentRange = range;

                var cancellationToken = tokenSource.Token;
                var eventArgs = new SuggestionsRequestedEventArgs { Query = query, Prefix = prefix };
                try
                {
                    await SuggestionsRequested?.InvokeAsync(this, eventArgs, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    eventArgs.Cancel = true;
                }
            }
            else
            {
                ShowSuggestionsPopup(false);
            }
        }

        private async Task OnSuggestionSelectedAsync(object selectedItem)
        {
            var range = _currentRange?.GetClone();
            var id = Guid.NewGuid();
            var prefix = _currentPrefix;
            var query = _currentQuery;

            // range has length of 0 at the end of the commit.
            // Checking length == 0 to avoid committing twice.
            if (SuggestionChosen == null || prefix == null || query == null || range == null ||
                range.Length == 0)
            {
                return;
            }

            var eventArgs = new SuggestionChosenEventArgs
            {
                Id = id,
                Prefix = prefix,
                Query = query,
                SelectedItem = selectedItem,
                Format = this.CreateTokenFormat()
            };
            var textBefore = range.Text;
            await SuggestionChosen.InvokeAsync(this, eventArgs);
            var text = eventArgs.Text;

            // Since this operation is async, the document may have changed at this point.
            // Double check if the range still has the expected query.
            if (string.IsNullOrEmpty(text) || textBefore != range.Text ||
                !TryExtractQueryFromRange(range, out var testPrefix, out var testQuery) ||
                testPrefix != prefix || testQuery != query)
            {
                return;
            }

            lock (LockObj)
            {
                var displayText = prefix + text;
                if (TryCommitSuggestionIntoDocument(range, displayText, id, eventArgs.Format))
                {
                    var token = new RichSuggestToken(id, displayText) { Active = true, Item = selectedItem };
                    token.UpdateTextRange(range);
                    _tokens.TryAdd(range.Link, token);
                }
            }
        }

        internal bool TryCommitSuggestionIntoDocument(ITextRange range, string displayText, Guid id, RichSuggestTokenFormat format, bool addTrailingSpace = true)
        {
            try
            {
                _ignoreChange = true;
                TextDocument.BeginUndoGroup();

                // We don't want to set text when the display text doesn't change since it may lead to unexpected caret move.
                range.GetText(TextGetOptions.NoHidden, out var existingText);
                if (existingText != displayText)
                {
                    range.SetText(TextSetOptions.Unhide, displayText);
                }

                range.Link = $"\"{id}\"";

                // In some rare case, setting Link can fail. Only observed when the token is at the start of the document.
                if (range.Link != $"\"{id}\"")
                {
                    range.Link = string.Empty;
                    return false;
                }

                ApplyTokenFormat(range.CharacterFormat, format);

                if (addTrailingSpace)
                {
                    var clone = range.GetClone();
                    clone.Collapse(false);
                    clone.SetText(TextSetOptions.Unhide, " ");
                    clone.Collapse(false);
                    TextDocument.Selection.SetRange(clone.EndPosition, clone.EndPosition);
                }

                TextDocument.EndUndoGroup();
                return true;
            }
            finally
            {
                _ignoreChange = false;
            }
        }

        private void ValidateTokensInDocument()
        {
            foreach (var (_, token) in _tokens)
            {
                token.Active = false;
            }

            ForEachLinkInDocument(TextDocument, ValidateTokenFromRange);
        }

        private void ValidateTokenFromRange(ITextRange range)
        {
            if (range.Length == 0 || !TryGetTokenFromRange(range, out var token))
            {
                return;
            }

            // Check for duplicate tokens. This can happen if the user copies and pastes the token multiple times.
            if (token.Active && token.RangeStart != range.StartPosition && token.RangeEnd != range.EndPosition)
            {
                lock (LockObj)
                {
                    var guid = Guid.NewGuid();
                    if (TryCommitSuggestionIntoDocument(range, token.DisplayText, guid, this.CreateTokenFormat(), false))
                    {
                        token = new RichSuggestToken(guid, token.DisplayText) { Active = true, Item = token.Item };
                        token.UpdateTextRange(range);
                        _tokens.Add(range.Link, token);
                    }

                    return;
                }
            }

            if (token.ToString() != range.Text)
            {
                this.ResetFormat(range);
                token.Active = false;
                return;
            }

            token.UpdateTextRange(range);
            token.Active = true;
        }

        /// <summary>
        /// Handle the special case where editing a token at the start of the document does not completely reset the character format.
        /// </summary>
        private void ResetFormatAfterTokenRemoveAtStart()
        {
            if (_tokenAtStart)
            {
                var range = TextDocument.Selection.GetClone();
                range.SetRange(0, range.EndPosition);
                this.ResetFormat(range);
                _tokenAtStart = false;
            }
        }

        private void ResetFormat(ITextRange range)
        {
            // Need to reset both Link and CharacterFormat or the token id will still persist in the RTF text.
            var defaultFormat = TextDocument.GetDefaultCharacterFormat();
            var selection = TextDocument.Selection;
            if (!string.IsNullOrEmpty(range.Link))
            {
                range.Link = string.Empty;
            }

            range.CharacterFormat = defaultFormat;

            if (selection.Type == SelectionType.InsertionPoint && selection.EndPosition == range.EndPosition)
            {
                selection.CharacterFormat = defaultFormat;
            }
        }

        private void ConditionallyLoadElement(object property, string elementName)
        {
            if (property != null && GetTemplateChild(elementName) is UIElement presenter)
            {
                presenter.Visibility = Visibility.Visible;
            }
        }

        private void UpdateSuggestionsListSelectedItem(int choice)
        {
            var itemsList = _suggestionsList.Items;
            if (itemsList == null)
            {
                return;
            }

            _suggestionsList.SelectedItem = choice == 0 ? null : itemsList[choice - 1];
            _suggestionsList.ScrollIntoView(_suggestionsList.SelectedItem);
        }

        private void ShowSuggestionsPopup(bool show)
        {
            if (_suggestionPopup == null)
            {
                return;
            }

            this._suggestionPopup.IsOpen = show;
            if (!show)
            {
                this._suggestionChoice = 0;
                this._suggestionPopup.VerticalOffset = 0;
                this._suggestionPopup.HorizontalOffset = 0;
                UpdateCornerRadii();
            }
        }

        private void UpdatePopupWidth()
        {
            if (this._suggestionsList == null)
            {
                return;
            }

            if (this.PopupPlacement == SuggestionPopupPlacementMode.Attached)
            {
                this._suggestionsList.MaxWidth = double.PositiveInfinity;
                this._suggestionsList.Width = this._richEditBox.ActualWidth;
            }
            else
            {
                this._suggestionsList.MaxWidth = this._richEditBox.ActualWidth;
                this._suggestionsList.Width = double.NaN;
            }
        }

        /// <summary>
        /// Calculate whether to open the suggestion list up or down depends on how much screen space is available
        /// </summary>
        private void UpdatePopupOffset()
        {
            this._richEditBox.TextDocument.Selection.GetRect(PointOptions.None, out var selectionRect, out _);
            Thickness padding = this._richEditBox.Padding;

            // Update horizontal offset
            if (this.PopupPlacement == SuggestionPopupPlacementMode.Attached)
            {
                this._suggestionPopup.HorizontalOffset = 0;
            }
            else
            {
                double editBoxWidth = this._richEditBox.ActualWidth - padding.Left - padding.Right;
                if (this._suggestionPopup.HorizontalOffset == 0 && editBoxWidth > 0)
                {
                    var normalizedX = selectionRect.X / editBoxWidth;
                    this._suggestionPopup.HorizontalOffset =
                        (this._richEditBox.ActualWidth - this._suggestionsList.ActualWidth) * normalizedX;
                }
            }

            // Update vertical offset
            double downOffset = this._richEditBox.ActualHeight;
            double upOffset = -this._suggestionsContainer.ActualHeight;
            if (this.PopupPlacement == SuggestionPopupPlacementMode.Floating)
            {
                downOffset = selectionRect.Bottom + padding.Top + padding.Bottom;
                upOffset += selectionRect.Top;
            }

            if (this._suggestionPopup.VerticalOffset == 0)
            {
                if (IsElementOnScreen(this._suggestionsList, offsetY: downOffset) &&
                    (IsElementInsideWindow(this._suggestionsList, offsetY: downOffset) ||
                     !IsElementInsideWindow(this._suggestionsList, offsetY: upOffset) ||
                     !IsElementOnScreen(this._suggestionsList, offsetY: upOffset)))
                {
                    this._suggestionPopup.VerticalOffset = downOffset;
                    this._popupOpenDown = true;
                }
                else
                {
                    this._suggestionPopup.VerticalOffset = upOffset;
                    this._popupOpenDown = false;
                }

                UpdateCornerRadii();
            }
            else
            {
                this._suggestionPopup.VerticalOffset = this._popupOpenDown ? downOffset : upOffset;
            }
        }

        /// <summary>
        /// Set corner radii so that inner corners, where suggestion list and text box connect, are square.
        /// This only applies when <see cref="PopupPlacement"/> is set to <see cref="SuggestionPopupPlacementMode.Attached"/>.
        /// </summary>
        /// https://docs.microsoft.com/en-us/windows/apps/design/style/rounded-corner#when-not-to-round
        private void UpdateCornerRadii()
        {
            if (this._richEditBox == null || this._suggestionsContainer == null ||
                !ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                return;
            }

            this._richEditBox.CornerRadius = CornerRadius;
            this._suggestionsContainer.CornerRadius = PopupCornerRadius;

            if (this._suggestionPopup.IsOpen && PopupPlacement == SuggestionPopupPlacementMode.Attached)
            {
                if (this._popupOpenDown)
                {
                    var cornerRadius = new CornerRadius(CornerRadius.TopLeft, CornerRadius.TopRight, 0, 0);
                    this._richEditBox.CornerRadius = cornerRadius;
                    var popupCornerRadius =
                        new CornerRadius(0, 0, PopupCornerRadius.BottomRight, PopupCornerRadius.BottomLeft);
                    this._suggestionsContainer.CornerRadius = popupCornerRadius;
                }
                else
                {
                    var cornerRadius = new CornerRadius(0, 0, CornerRadius.BottomRight, CornerRadius.BottomLeft);
                    this._richEditBox.CornerRadius = cornerRadius;
                    var popupCornerRadius =
                        new CornerRadius(PopupCornerRadius.TopLeft, PopupCornerRadius.TopRight, 0, 0);
                    this._suggestionsContainer.CornerRadius = popupCornerRadius;
                }
            }
        }

        private bool TryExtractQueryFromSelection(out string prefix, out string query, out ITextRange range)
        {
            prefix = string.Empty;
            query = string.Empty;
            range = null;
            if (TextDocument.Selection.Type != SelectionType.InsertionPoint)
            {
                return false;
            }

            // Check if selection is on existing link (suggestion)
            var expandCount = TextDocument.Selection.GetClone().Expand(TextRangeUnit.Link);
            if (expandCount != 0)
            {
                return false;
            }

            var selection = TextDocument.Selection.GetClone();
            selection.MoveStart(TextRangeUnit.Word, -1);
            if (selection.Length == 0)
            {
                return false;
            }

            range = selection;
            if (TryExtractQueryFromRange(selection, out prefix, out query))
            {
                return true;
            }

            selection.MoveStart(TextRangeUnit.Word, -1);
            if (TryExtractQueryFromRange(selection, out prefix, out query))
            {
                return true;
            }

            range = null;
            return false;
        }

        private bool TryExtractQueryFromRange(ITextRange range, out string prefix, out string query)
        {
            prefix = string.Empty;
            query = string.Empty;
            range.GetText(TextGetOptions.NoHidden, out var possibleQuery);
            if (possibleQuery.Length > 0 && Prefixes.Contains(possibleQuery[0]) &&
                !possibleQuery.Any(char.IsWhiteSpace) && string.IsNullOrEmpty(range.Link))
            {
                if (possibleQuery.Length == 1)
                {
                    prefix = possibleQuery;
                    return true;
                }

                prefix = possibleQuery[0].ToString();
                query = possibleQuery.Substring(1);
                return true;
            }

            return false;
        }

        private RichSuggestTokenFormat CreateTokenFormat()
        {
            var defaultFormat = TextDocument.GetDefaultCharacterFormat();
            var suggestionFormat = new RichSuggestTokenFormat(defaultFormat);
            if (this.TokenBackground != null)
            {
                suggestionFormat.Background = this.TokenBackground.Color;
            }

            if (this.TokenForeground != null)
            {
                suggestionFormat.Foreground = this.TokenForeground.Color;
            }

            return suggestionFormat;
        }

        private Rect GetTokenRect(ITextRange tokenRange)
        {
            var padding = _richEditBox.Padding;
            tokenRange.GetRect(PointOptions.ClientCoordinates, out var rect, out var hit);
            rect.X += padding.Left;
            rect.Y += padding.Top;
            var transform = _richEditBox.TransformToVisual(this);
            return transform.TransformBounds(rect);
        }

        private void UpdateVisibleTokenList()
        {
            lock (LockObj)
            {
                var toBeRemoved = _visibleTokens.Where(x => !x.Active).ToArray();

                foreach (var elem in toBeRemoved)
                {
                    _visibleTokens.Remove(elem);
                }

                var toBeAdded = _tokens.Where(pair => pair.Value.Active && !_visibleTokens.Contains(pair.Value))
                    .Select(pair => pair.Value).ToArray();

                foreach (var elem in toBeAdded)
                {
                    _visibleTokens.Add(elem);
                }
            }
        }
    }
}
