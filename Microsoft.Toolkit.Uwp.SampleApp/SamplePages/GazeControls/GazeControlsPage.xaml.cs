// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Input.GazeControls;
using Microsoft.Toolkit.Uwp.Input.GazeInteraction;
using Microsoft.Toolkit.Uwp.UI;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GazeControlsPage : IXamlRenderListener
    {
        private const int NUM_PREDICTIONS = 3;
        private MediaElement _mediaElement;
        private SpeechSynthesizer _speechSynthesizer;
        private Button[] _predictions;
        private TextBox _textControl;
        private GazeKeyboard _gazeKeyboard;

        public GazeControlsPage()
        {
            this.InitializeComponent();
            _predictions = new Button[NUM_PREDICTIONS];
            _mediaElement = new MediaElement();
            _speechSynthesizer = new SpeechSynthesizer();
        }

        public async void OnXamlRendered(FrameworkElement control)
        {
            GazeInput.IsDeviceAvailableChanged += GazeInput_IsDeviceAvailableChanged;

            _predictions[0] = control.FindChild("Prediction0") as Button;
            _predictions[1] = control.FindChild("Prediction1") as Button;
            _predictions[2] = control.FindChild("Prediction2") as Button;
            _textControl = control.FindChild("TextControl") as TextBox;
            _gazeKeyboard = control.FindChild("GazeKeyboard") as GazeKeyboard;
            _gazeKeyboard = control.FindChild("GazeKeyboard") as GazeKeyboard;

            _gazeKeyboard.Target = _textControl;
            _gazeKeyboard.PredictionTargets = _predictions;

            var speakButton = control.FindChild("SpeakButton") as Button;
            speakButton.Click += OnSpeak;

            var fileOpenButton = control.FindChild("FileOpenButton") as Button;
            fileOpenButton.Click += OnFileOpen;

            var fileSaveButton = control.FindChild("FileSaveButton") as Button;
            fileSaveButton.Click += OnFileSave;

            WarnUserToPlugInDevice();
        }

        private async void OnSpeak(object sender, RoutedEventArgs e)
        {
            var text = _textControl.Text.ToString();
            var stream = await _speechSynthesizer.SynthesizeTextToStreamAsync(text);
            _mediaElement.SetSource(stream, stream.ContentType);
            _mediaElement.AutoPlay = true;
            _mediaElement.Play();
        }

        private async void OnFileOpen(object sender, RoutedEventArgs e)
        {
            var file = await ShowFilePicker(false);
            if (file != null)
            {
                _textControl.Text = await FileIO.ReadTextAsync(file);
            }
        }

        private async void OnFileSave(object sender, RoutedEventArgs e)
        {
            var file = await ShowFilePicker(true);
            if (file != null)
            {
                await FileIO.WriteTextAsync(file, _textControl.Text);
            }
        }

        private async Task<StorageFile> ShowFilePicker(bool saveMode)
        {
            var picker = new GazeFilePicker();
            var library = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Documents);
            picker.SaveMode = saveMode;
            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add(".html");
            picker.FileTypeFilter.Add(".log");
            picker.CurrentFolder = library.SaveFolder;
            await picker.ShowAsync();
            return picker.SelectedItem;
        }

        private void GazeInput_IsDeviceAvailableChanged(object sender, object e)
        {
            WarnUserToPlugInDevice();
        }

        private void WarnUserToPlugInDevice()
        {
            if (GazeInput.IsDeviceAvailable)
            {
                //WarningText.Visibility = Visibility.Collapsed;
            }
            else
            {
                //WarningText.Visibility = Visibility.Visible;
            }
        }

    }
}