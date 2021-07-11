// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Text;
using Windows.Storage;
using Windows.System;
using Windows.UI.Input.Preview.Injection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace Microsoft.Toolkit.Uwp.Input.GazeControls
{
    /// <summary>
    /// Gaze optimized soft keyboard with support for custom layouts and predictions
    /// </summary>
    [Bindable]
    public sealed partial class GazeKeyboard : UserControl
    {
        private InputInjector _injector;
        private KeyboardPage _rootPage;
        private TextPredictionGenerator _textPredictionGenerator;
        private WordsSegmenter _wordsSegmenter;
        private string _predictionLanguage;
        private Button[] _predictionTargets;

        /// <summary>
        /// Gets or sets the target text box for injecting keys
        /// </summary>
        public TextBox Target { get; set; }

        /// <summary>
        /// Gets or sets the text prediction language
        /// </summary>
        public string PredictionLanguage
        {
            get
            {
                return _predictionLanguage;
            }

            set
            {
                _predictionLanguage = value;
                _textPredictionGenerator = new TextPredictionGenerator(value);
                _textPredictionGenerator.InputScope = Windows.UI.Text.Core.CoreTextInputScope.Text;
                _wordsSegmenter = new WordsSegmenter(value);
            }
        }

        private Uri _layoutUri;

        /// <summary>
        /// Gets or sets the URI of the layout file for the keyboard
        /// </summary>
        public Uri LayoutUri
        {
            get
            {
                return _layoutUri;
            }

            set
            {
                _layoutUri = value;
                _ = LoadLayout(value);
            }
        }

        /// <summary>
        /// Gets or sets the prediction targets
        /// </summary>
        public Button[] PredictionTargets
        {
            get
            {
                return _predictionTargets;
            }

            set
            {
                if (_predictionTargets != null)
                {
                    foreach (var target in _predictionTargets)
                    {
                        target.Click -= OnPredictionSelected;
                    }
                }

                _predictionTargets = value;
                foreach (var target in _predictionTargets)
                {
                    target.Click += OnPredictionSelected;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GazeKeyboard"/> class.
        /// </summary>
        public GazeKeyboard()
        {
            InitializeComponent();
            PredictionLanguage = "en-US";
            _injector = InputInjector.TryCreate();
        }

        private void BuildPageHierarchy(KeyboardPage parent)
        {
            var children = GazeKeyboard.GetPageList(parent.Page);
            foreach (var childName in children)
            {
                var node = parent.Page.FindName(childName) as FrameworkElement;
                if (node != null)
                {
                    var childPage = new KeyboardPage(node, parent);
                    parent.ChildrenNames.Add(childName);
                    parent.Children.Add(childPage);
                    BuildPageHierarchy(childPage);
                }
            }

            if (parent.Children.Count > 0)
            {
                parent.CurrentChild = parent.Children[0];
            }
        }

        /// <summary>
        /// Loads the given keyboard layout from a file
        /// </summary>
        /// <param name="uri"> Uri of the layout file</param>
        /// <returns>Task</returns>
        private async Task LoadLayout(Uri uri)
        {
            try
            {
                var storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
                var xaml = await FileIO.ReadTextAsync(storageFile);
                var xamlNode = XamlReader.Load(xaml) as FrameworkElement;

                foreach (var button in xamlNode.FindDescendants().OfType<ButtonBase>())
                {
                    button.Click += OnKeyboardButtonClick;
                }

                _rootPage = new KeyboardPage(xamlNode, null);
                BuildPageHierarchy(_rootPage);

                LayoutRoot.Children.Add(xamlNode);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private bool HandleVirtualKey(int vk)
        {
            if (vk < 0)
            {
                return false;
            }

            var key = new InjectedInputKeyboardInfo();
            key.VirtualKey = (ushort)vk;
            _injector.InjectKeyboardInput(new[] { key });

            UpdatePredictions();
            return true;
        }

        private bool HandleVirtualKeyList(List<int> vkList)
        {
            var state = new Dictionary<int, bool>();
            var keys = new List<InjectedInputKeyboardInfo>();
            foreach (var vk in vkList)
            {
                var key = new InjectedInputKeyboardInfo();
                key.VirtualKey = (ushort)vk;
                if (state.ContainsKey(vk))
                {
                    key.KeyOptions = InjectedInputKeyOptions.KeyUp;
                    state.Remove(vk);
                }
                else
                {
                    state.Add(vk, true);
                }

                keys.Add(key);
            }

            _injector.InjectKeyboardInput(keys);

            UpdatePredictions();
            return true;
        }

        private bool HandleUnicodeChar(string unicode)
        {
            var curContent = Clipboard.GetContent();
            Clipboard.Clear();
            var dp = new DataPackage();
            dp.SetText(unicode);
            Clipboard.SetContent(dp);

            var keyList = new List<int>();
            keyList.Add((int)VirtualKey.Control);
            keyList.Add((int)VirtualKey.V);
            keyList.Add((int)VirtualKey.V);
            keyList.Add((int)VirtualKey.Control);

            return HandleVirtualKeyList(keyList);
        }

        private KeyboardPage FindContainer(KeyboardPage kbdPage, string containerName)
        {
            if (kbdPage.Page.Name == containerName)
            {
                return kbdPage;
            }

            KeyboardPage containerPage = null;
            for (int i = 0; i < kbdPage.Children.Count; i++)
            {
                containerPage = FindContainer(kbdPage.Children[i], containerName);
                if (containerPage != null)
                {
                    return containerPage;
                }
            }

            return null;
        }

        private void HandlePageChange(string containerName, ButtonBase sender)
        {
            var container = FindContainer(_rootPage, containerName);

            var tempPage = GazeKeyboard.GetTemporaryPage(sender);
            var newPage = GazeKeyboard.GetNewPage(sender);
            string pageName;

            pageName = (tempPage != null) ? tempPage : newPage;

            // find the new page among its siblings
            var children = container.Children;
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].Page.Name == pageName)
                {
                    // hide the current page
                    container.CurrentChild.Page.Visibility = Visibility.Collapsed;
                    container.Children[i].Page.Visibility = Visibility.Visible;
                    if (tempPage != null)
                    {
                        container.PrevChild = container.CurrentChild;
                    }

                    container.CurrentChild = container.Children[i];
                }
            }
        }

        private void RevertTempPage(ButtonBase button)
        {
            // This logic is based on the assumption that the button is placed in a grid or other container
            // which is the parent page. This is the temp page that has to be reverted back. So we navigate
            // to the grandparent of that page to identify the siblings of the parent page
            var page = button.Parent as FrameworkElement;
            if (page == null)
            {
                return;
            }

            var container = page.Parent as FrameworkElement;
            if (container == null)
            {
                return;
            }

            var containerPage = FindContainer(_rootPage, container.Name);
            if ((containerPage == null) || (containerPage.PrevChild == null))
            {
                return;
            }

            containerPage.CurrentChild.Page.Visibility = Visibility.Collapsed;
            containerPage.PrevChild.Page.Visibility = Visibility.Visible;
            containerPage.CurrentChild = containerPage.PrevChild;
            containerPage.PrevChild = null;
        }

        private async void OnKeyboardButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as ButtonBase;
            Target.Focus(FocusState.Programmatic);
            await Task.Delay(1);

            string unicode;
            string container;
            int vk;
            List<int> vkList;
            bool injected = false;

            if ((container = GazeKeyboard.GetPageContainer(button)) != null)
            {
                HandlePageChange(container, button);
            }
            else if ((vk = GazeKeyboard.GetVK(button)) != 0)
            {
                injected = HandleVirtualKey(vk);
            }
            else if (((vkList = GazeKeyboard.GetVKList(button)) != null) && (vkList.Count > 0))
            {
                injected = HandleVirtualKeyList(vkList);
            }
            else if ((unicode = GazeKeyboard.GetUnicode(button)) != null)
            {
                injected = HandleUnicodeChar(unicode);
            }
            else
            {
                var key = new InjectedInputKeyboardInfo();
                key.ScanCode = button.Content.ToString()[0];
                key.KeyOptions = InjectedInputKeyOptions.Unicode;
                _injector.InjectKeyboardInput(new[] { key });
                UpdatePredictions();
                injected = true;
            }

            if (injected)
            {
                RevertTempPage(button);
            }
        }

        private void InjectString(string str, bool addSpace)
        {
            if (addSpace)
            {
                str += " ";
            }

            var keys = new List<InjectedInputKeyboardInfo>();
            foreach (var ch in str)
            {
                var key = new InjectedInputKeyboardInfo()
                {
                    ScanCode = ch,
                    KeyOptions = InjectedInputKeyOptions.Unicode
                };
                keys.Add(key);

                key = new InjectedInputKeyboardInfo()
                {
                    ScanCode = ch,
                    KeyOptions = InjectedInputKeyOptions.Unicode | InjectedInputKeyOptions.KeyUp
                };

                keys.Add(key);
            }

            // Injecting too many keys at once can result in ArgumentException.
            // So inject a max of 8 keys at a time. 8 seems to work for now.
            for (int i = 0; i < keys.Count; i += 8)
            {
                var count = Math.Min(8, keys.Count - i);
                _injector.InjectKeyboardInput(keys.GetRange(i, count));
            }
        }

        private void OnPredictionSelected(object sender, RoutedEventArgs e)
        {
            WordSegment replaceSegment = null;
            if (Target.SelectionStart < Target.Text.Length)
            {
                replaceSegment = _wordsSegmenter.GetTokenAt(Target.Text, (uint)Target.SelectionStart);
            }
            else if (Target.Text[Target.Text.Length - 1] != ' ')
            {
                var tokens = _wordsSegmenter.GetTokens(Target.Text);
                if (tokens == null || tokens.Count == 0)
                {
                    return;
                }

                replaceSegment = tokens[tokens.Count - 1];
            }

            if (replaceSegment != null)
            {
                Target.Select((int)replaceSegment.SourceTextSegment.StartPosition, (int)replaceSegment.SourceTextSegment.Length);
            }

            string prediction = (sender as Button).Content.ToString();
            InjectString(prediction, true);
            UpdatePredictions();
        }

        private List<string> GetPrevWords()
        {
            var segments = _wordsSegmenter.GetTokens(Target.Text);
            if ((segments == null) || (segments.Count == 0))
            {
                return null;
            }

            int i = 0;
            var words = new List<string>(segments.Count);
            foreach (var segment in segments)
            {
                words.Add(segment.Text);
                i++;
            }

            words.Reverse();
            return words;
        }

        private async void UpdateNextWordPredictions()
        {
            var prevWords = GetPrevWords();
            var predictions = await _textPredictionGenerator.GetNextWordCandidatesAsync((uint)PredictionTargets.Length, prevWords);
            DisplayPredictions(predictions);
        }

        private async void UpdatePredictions()
        {
            // IMPORTANT: Wait for the text box to be updated with the injected keys
            await Task.Delay(1);

            if ((PredictionTargets == null) || (PredictionTargets.Length <= 0))
            {
                return;
            }

            var prevWords = GetPrevWords();

            if ((prevWords == null) || (prevWords.Count == 0))
            {
                return;
            }

            IReadOnlyList<string> predictions;
            var prevWordsExceptLast = prevWords.GetRange(1, prevWords.Count - 1);

            // It looks like we need to send in a larger number than necessary to get good quality predictions.
            uint maxCandidates = (uint)PredictionTargets.Length * 2;
            predictions = await _textPredictionGenerator.GetCandidatesAsync(
                            prevWords[0],
                            maxCandidates,
                            TextPredictionOptions.Corrections | TextPredictionOptions.Predictions,
                            prevWordsExceptLast);

            DisplayPredictions(predictions);
        }

        private void DisplayPredictions(IReadOnlyList<string> predictions)
        {
            int i;
            for (i = 0; (i < predictions.Count) && (i < PredictionTargets.Length); i++)
            {
                PredictionTargets[i].Content = predictions[i];
            }

            for (; i < PredictionTargets.Length; i++)
            {
                PredictionTargets[i].Content = string.Empty;
            }
        }
    }
}
