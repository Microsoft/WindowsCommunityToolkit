// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Media.Geometry;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CanvasPathGeometryPage : Page
    {
        private const string Sample1 =
            "F0 M 656.500,400.500 C 656.500,350.637 598.572,307.493 514.292,286.708 C 493.507,202.428 450.363,144.500 400.500,144.500 C 350.637,144.500 307.493,202.428 286.708,286.708 C 202.428,307.493 144.500,350.637 144.500,400.500 C 144.500,450.363 202.428,493.507 286.708,514.292 C 307.493,598.572 350.637,656.500 400.500,656.500 C 450.363,656.500 493.507,598.572 514.292,514.292 C 598.572,493.507 656.500,450.363 656.500,400.500 ZM 581.519,219.481 C 546.261,184.222 474.793,194.676 400.500,239.574 C 326.207,194.676 254.739,184.222 219.481,219.481 C 184.222,254.739 194.676,326.207 239.574,400.500 C 194.676,474.792 184.222,546.261 219.481,581.519 C 254.739,616.778 326.207,606.324 400.500,561.426 C 474.793,606.324 546.261,616.778 581.519,581.519 C 616.778,546.261 606.324,474.792 561.426,400.500 C 606.324,326.207 616.778,254.739 581.519,219.481 ZU 112.5 112.5 570 570 36 36";

        private const string Sample2 =
            "F1 M 331.341,81.975 L 398.766,218.593 L 549.533,240.500 L 440.437,346.842 L 466.191,497.000 L 331.341,426.105 L 196.491,497.000 L 222.245,346.842 L 113.150,240.500 L 263.916,218.593 L 331.341,81.975 Z";

        private const string Sample3 =
            "F1 M 545.497,397.058 C 454.492,512.882 286.824,533.003 171.000,441.998 C 78.340,369.194 62.244,235.059 135.048,142.400 C 193.291,68.272 300.599,55.395 374.726,113.639 C 434.028,160.233 444.330,246.079 397.736,305.381 C 360.460,352.823 291.783,361.064 244.341,323.788 C 206.388,293.968 199.795,239.026 229.616,201.073 C 253.472,170.711 297.425,165.436 327.788,189.293 C 352.078,208.378 356.297,243.540 337.212,267.830 C 321.944,287.262 293.814,290.638 274.382,275.370 C 258.836,263.155 256.136,240.651 268.350,225.106 C 278.122,212.669 296.125,210.509 308.562,220.280 C 318.511,228.098 320.239,242.500 312.422,252.449";

        private const string Sample4 =
            "F1 M 311.717,332.110 C 285.669,332.110 264.552,310.994 264.552,284.945 C 264.552,258.897 285.669,237.781 311.717,237.781 C 337.765,237.781 358.881,258.897 358.881,284.945 C 358.881,310.994 337.765,332.110 311.717,332.110 Z M 505.712,232.846 C 634.939,203.833 411.705,171.395 371.772,213.383 C 411.705,171.395 311.872,-30.889 311.872,92.013 C 311.872,-30.889 212.038,171.395 251.972,213.383 C 212.038,171.395 -11.196,203.833 118.031,232.846 C -11.196,203.833 150.338,361.289 214.951,327.320 C 150.338,361.289 112.205,583.622 192.072,460.719 C 112.205,583.622 311.872,478.651 311.872,397.737 C 311.872,478.651 511.538,583.622 431.672,460.719 C 511.538,583.622 473.405,361.289 408.792,327.320 C 473.405,361.289 634.939,203.833 505.712,232.846 Z";

        private const string Sample5 =
            "F1 M 391.853,348.284 C 391.853,357.113 384.696,364.271 375.867,364.271 L 301.927,364.271 C 293.098,364.271 285.940,357.113 285.940,348.284 L 285.940,274.345 C 285.940,265.515 293.098,258.358 301.927,258.358 L 375.867,258.358 C 384.696,258.358 391.853,265.515 391.853,274.345 L 391.853,348.284 Z M 544.748,282.990 L 485.488,267.081 C 472.521,263.600 466.301,248.839 472.866,237.128 L 502.867,183.604 C 512.642,166.166 494.336,146.433 476.214,154.872 L 420.592,180.776 C 408.421,186.445 394.169,179.136 391.670,165.944 L 380.248,105.658 C 376.526,86.017 349.819,82.667 341.362,100.780 L 315.403,156.378 C 309.723,168.543 294.107,172.105 283.714,163.607 L 236.213,124.767 C 220.737,112.113 198.125,126.714 203.289,146.025 L 219.141,205.301 C 222.610,218.271 212.937,231.038 199.512,231.208 L 138.159,231.988 C 118.170,232.242 110.233,257.962 126.602,269.436 L 176.847,304.655 C 187.841,312.361 188.638,328.358 178.464,337.118 L 131.965,377.153 C 116.816,390.196 127.269,415.001 147.184,413.268 L 208.312,407.950 C 221.687,406.786 232.580,418.529 230.417,431.779 L 220.531,492.336 C 217.310,512.066 241.261,524.348 255.403,510.220 L 298.811,466.854 C 308.310,457.365 324.202,459.358 331.062,470.899 L 362.415,523.643 C 372.629,540.827 398.872,534.840 400.624,514.927 L 406.001,453.804 C 407.178,440.430 420.634,431.742 433.307,436.173 L 491.227,456.425 C 510.098,463.022 526.353,441.568 514.895,425.187 L 479.725,374.908 C 472.030,363.906 476.753,348.601 489.310,343.850 L 546.697,322.133 C 565.393,315.057 564.054,288.173 544.748,282.990 Z";

        private const string ErrorString =
            "F1 M 19.648,24.605 L 19.648,30.220 L 29.404,30.220 L 29.404,28.149 C 29.404,27.229 29.581,26.573 29.936,26.181 C 30.290,25.790 30.753,25.594 31.325,25.594 C 31.885,25.594 " +
            "32.342,25.790 32.696,26.181 C 33.051,26.573 33.228,27.229 33.228,28.149 L 33.228,34.044 L 15.227,34.044 C 14.307,34.044 13.651,33.867 13.259,33.512 C 12.867,33.158 12.672,32.695 " +
            "12.672,32.122 C 12.672,31.563 12.870,31.106 13.268,30.751 C 13.666,30.397 14.319,30.220 15.227,30.220 L 15.824,30.220 L 15.824,15.260 L 15.227,15.260 C 14.307,15.260 13.651,15.082 " +
            "13.259,14.728 C 12.867,14.373 12.672,13.910 12.672,13.338 C 12.672,12.766 12.867,12.303 13.259,11.948 C 13.651,11.594 14.307,11.417 15.227,11.417 L 32.388,11.436 L 32.388,17.255 C " +
            "32.388,18.163 32.214,18.813 31.866,19.205 C 31.518,19.596 31.058,19.792 30.486,19.792 C 29.914,19.792 29.451,19.600 29.096,19.214 C 28.742,18.829 28.564,18.176 28.564,17.255 L " +
            "28.564,15.260 L 19.648,15.260 L 19.648,20.781 L 23.006,20.781 C 23.006,19.786 23.099,19.146 23.285,18.860 C 23.671,18.250 24.218,17.946 24.927,17.946 C 25.487,17.946 25.944,18.142 " +
            "26.298,18.533 C 26.652,18.925 26.830,19.581 26.830,20.501 L 26.830,24.903 C 26.830,25.737 26.730,26.297 26.531,26.582 C 26.133,27.167 25.599,27.459 24.927,27.459 C 24.218,27.459 " +
            "23.671,27.155 23.285,26.545 C 23.099,26.259 23.006,25.612 23.006,24.605 L 19.648,24.605 ZM 45.707,17.106 L 45.707,19.494 C 47.311,18.337 48.577,17.567 49.503,17.181 C 50.430,16.795 " +
            "51.297,16.603 52.105,16.603 C 53.349,16.603 54.555,17.063 55.724,17.983 C 56.520,18.605 56.918,19.239 56.918,19.886 C 56.918,20.433 56.728,20.896 56.349,21.275 C 55.970,21.655 " +
            "55.513,21.844 54.978,21.844 C 54.505,21.844 54.008,21.608 53.486,21.135 C 52.963,20.663 52.497,20.427 52.087,20.427 C 51.552,20.427 50.753,20.762 49.690,21.434 C 48.626,22.105 " +
            "47.299,23.113 45.707,24.456 L 45.707,30.220 L 51.154,30.220 C 52.074,30.220 52.730,30.397 53.122,30.751 C 53.514,31.106 53.710,31.569 53.710,32.141 C 53.710,32.701 53.514,33.158 " +
            "53.122,33.512 C 52.730,33.867 52.074,34.044 51.154,34.044 L 39.607,34.044 C 38.687,34.044 38.031,33.867 37.639,33.512 C 37.248,33.158 37.052,32.695 37.052,32.122 C 37.052,31.563 " +
            "37.248,31.106 37.639,30.751 C 38.031,30.397 38.687,30.220 39.607,30.220 L 41.883,30.220 L 41.883,20.930 L 40.503,20.930 C 39.582,20.930 38.927,20.753 38.535,20.399 C 38.143,20.044 " +
            "37.947,19.581 37.947,19.009 C 37.947,18.449 38.143,17.992 38.535,17.638 C 38.927,17.283 39.582,17.106 40.503,17.106 L 45.707,17.106 ZM 68.633,17.106 L 68.633,19.494 C 70.237,18.337 " +
            "71.502,17.567 72.429,17.181 C 73.355,16.795 74.222,16.603 75.031,16.603 C 76.274,16.603 77.480,17.063 78.650,17.983 C 79.445,18.605 79.843,19.239 79.843,19.886 C 79.843,20.433 " +
            "79.654,20.896 79.274,21.275 C 78.895,21.655 78.438,21.844 77.903,21.844 C 77.431,21.844 76.933,21.608 76.411,21.135 C 75.889,20.663 75.423,20.427 75.012,20.427 C 74.477,20.427 " +
            "73.678,20.762 72.615,21.434 C 71.552,22.105 70.224,23.113 68.633,24.456 L 68.633,30.220 L 74.079,30.220 C 74.999,30.220 75.656,30.397 76.047,30.751 C 76.439,31.106 76.635,31.569 " +
            "76.635,32.141 C 76.635,32.701 76.439,33.158 76.047,33.512 C 75.656,33.867 74.999,34.044 74.079,34.044 L 62.533,34.044 C 61.612,34.044 60.957,33.867 60.565,33.512 C 60.173,33.158 " +
            "59.977,32.695 59.977,32.122 C 59.977,31.563 60.173,31.106 60.565,30.751 C 60.957,30.397 61.612,30.220 62.533,30.220 L 64.809,30.220 L 64.809,20.930 L 63.428,20.930 C 62.508,20.930 " +
            "61.852,20.753 61.460,20.399 C 61.069,20.044 60.873,19.581 60.873,19.009 C 60.873,18.449 61.069,17.992 61.460,17.638 C 61.852,17.283 62.508,17.106 63.428,17.106 L 68.633,17.106 ZM " +
            "98.460,25.911 C 98.460,24.680 98.018,23.548 97.135,22.516 C 95.929,21.123 94.343,20.427 92.379,20.427 C 90.650,20.427 89.208,20.980 88.051,22.087 C 86.895,23.194 86.316,24.474 " +
            "86.316,25.929 C 86.316,27.123 86.901,28.239 88.070,29.278 C 89.239,30.316 90.675,30.835 92.379,30.835 C 94.095,30.835 95.537,30.316 96.706,29.278 C 97.875,28.239 98.460,27.117 " +
            "98.460,25.911 Z M 102.284,25.892 C 102.284,27.360 101.876,28.780 101.062,30.154 C 100.247,31.529 99.035,32.623 97.425,33.438 C 95.814,34.252 94.132,34.659 92.379,34.659 C " +
            "90.638,34.659 88.971,34.258 87.380,33.456 C 85.788,32.654 84.575,31.563 83.742,30.182 C 82.909,28.802 82.492,27.360 82.492,25.855 C 82.492,24.325 82.915,22.824 83.761,21.350 C " +
            "84.606,19.876 85.822,18.717 87.408,17.871 C 88.993,17.026 90.650,16.603 92.379,16.603 C 94.119,16.603 95.795,17.035 97.406,17.899 C 99.016,18.763 100.232,19.926 101.053,21.387 C " +
            "101.873,22.849 102.284,24.350 102.284,25.892 ZM 114.483,17.106 L 114.483,19.494 C 116.088,18.337 117.353,17.567 118.279,17.181 C 119.206,16.795 120.073,16.603 120.882,16.603 C " +
            "122.125,16.603 123.331,17.063 124.500,17.983 C 125.296,18.605 125.694,19.239 125.694,19.886 C 125.694,20.433 125.504,20.896 125.125,21.275 C 124.746,21.655 124.289,21.844 " +
            "123.754,21.844 C 123.282,21.844 122.784,21.608 122.262,21.135 C 121.740,20.663 121.273,20.427 120.863,20.427 C 120.328,20.427 119.529,20.762 118.466,21.434 C 117.403,22.105 " +
            "116.075,23.113 114.483,24.456 L 114.483,30.220 L 119.930,30.220 C 120.850,30.220 121.506,30.397 121.898,30.751 C 122.290,31.106 122.486,31.569 122.486,32.141 C 122.486,32.701 " +
            "122.290,33.158 121.898,33.512 C 121.506,33.867 120.850,34.044 119.930,34.044 L 108.384,34.044 C 107.463,34.044 106.807,33.867 106.416,33.512 C 106.024,33.158 105.828,32.695 " +
            "105.828,32.122 C 105.828,31.563 106.024,31.106 106.416,30.751 C 106.807,30.397 107.463,30.220 108.384,30.220 L 110.659,30.220 L 110.659,20.930 L 109.279,20.930 C 108.359,20.930 " +
            "107.703,20.753 107.311,20.399 C 106.919,20.044 106.723,19.581 106.723,19.009 C 106.723,18.449 106.919,17.992 107.311,17.638 C 107.703,17.283 108.359,17.106 109.279,17.106 L " +
            "114.483,17.106 ZM 140.431,32.645 C 140.431,33.192 140.225,33.655 139.815,34.034 C 139.405,34.414 138.838,34.603 138.118,34.603 C 137.396,34.603 136.830,34.414 136.420,34.034 C " +
            "136.010,33.655 135.804,33.192 135.804,32.645 C 135.804,32.110 136.006,31.653 136.411,31.274 C 136.815,30.895 137.384,30.705 138.118,30.705 C 138.851,30.705 139.420,30.891 " +
            "139.824,31.264 C 140.228,31.637 140.431,32.098 140.431,32.645 Z M 141.046,13.655 L 139.983,25.183 C 139.933,25.780 139.734,26.244 139.386,26.573 C 139.038,26.903 138.603,27.067 " +
            "138.080,27.067 C 137.558,27.067 137.123,26.903 136.774,26.573 C 136.426,26.244 136.227,25.780 136.178,25.183 L 135.096,13.655 C 135.046,13.071 135.021,12.685 135.021,12.499 C " +
            "135.021,11.529 135.313,10.749 135.898,10.158 C 136.482,9.567 137.210,9.272 138.080,9.272 C 138.938,9.272 139.662,9.570 140.253,10.167 C 140.844,10.764 141.139,11.516 141.139,12.424 " +
            "C 141.139,12.611 141.108,13.021 141.046,13.655 Z";

        private const string ParseError1 = "Parameter \"(pathData matches.Count == 0)\" must be false, was true: ";
        private const string ParseError2 = "Parameter \"(pathData matches.Count > 1)\" must be false, was true: ";
        private const string ParseError3 = "PATH_ERR003";

        private DispatcherQueueTimer _typeTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();

        private List<Color> _colors;
        private List<string> _samples;

        private string _data = string.Empty;
        private StringBuilder _logger;

        private float _strokeThickness;
        private Color _strokeColor;
        private Color _fillColor;
        private bool _selectionChanged = false;

        private CanvasGeometry _errorGeometry;
        private GeometryStreamReader _reader;

        private SolidColorBrush _commandBrush;
        private SolidColorBrush _commandErrorBrush;

        public string InputText { get; set; }

        public CanvasPathGeometryPage()
        {
            this.InitializeComponent();
            _reader = new GeometryStreamReader();
            _logger = new StringBuilder();
            _colors = new List<Color>()
            {
                Colors.Transparent,
                Colors.Black,
                Colors.White,
                Colors.Crimson,
                CanvasPathGeometry.CreateColor("#bf5af2"),
                CanvasPathGeometry.CreateColor("#0a84ff"),
                CanvasPathGeometry.CreateColor("#32d74b"),
                CanvasPathGeometry.CreateColor("#ff9500"),
                CanvasPathGeometry.CreateColor("#ffd60a")
            };

            _commandBrush = new SolidColorBrush(Colors.White);
            _commandErrorBrush = new SolidColorBrush(Colors.Red);

            var colorList = new List<string>()
            {
                "Transparent",
                "Black",
                "White",
                "Crimson",
                "Purple",
                "LightBlue",
                "LightGreen",
                "Orange",
                "Yellow"
            };

            this._samples = new List<string>()
            {
                string.Empty,
                Sample1,
                Sample2,
                Sample3,
                Sample4,
                Sample5
            };

            var sampleList = new List<string>()
            {
                "None",
                "Sample 1",
                "Sample 2",
                "Sample 3",
                "Sample 4",
                "Sample 5",
            };

            StrokeList.ItemsSource = colorList;
            FillList.ItemsSource = colorList;

            StrokeThickness.Value = 1;
            StrokeList.SelectedIndex = 1;
            FillList.SelectedIndex = 0;

            _selectionChanged = false;
        }

        private void ParseData()
        {
            _data = InputData.Text;
            RenderCanvas.Invalidate();
        }

        private void OnCanvasDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(_data))
            {
                CommandsList.Text = string.Empty;
                return;
            }

            this._errorGeometry ??= CanvasPathGeometry.CreateGeometry(sender, ErrorString);

            _logger?.Clear();
            CommandsList.Text = string.Empty;

            try
            {
                _logger?.AppendLine("// The following commands represent the CanvasPathBuilder command(s) needed");
                _logger?.AppendLine("// to create the CanvasGeometry from the specified Win2d Path Mini Language.");
                var geometry = CanvasPathGeometry.CreateGeometry(sender, _data);
                _reader.StartLogging();
                geometry.SendPathTo(_reader);
                _logger?.AppendLine(_reader.EndLogging());
                CommandsList.Text = _logger?.ToString() ?? string.Empty;

                args.DrawingSession.FillGeometry(geometry, _fillColor);
                args.DrawingSession.DrawGeometry(geometry, _strokeColor, _strokeThickness);
                RootPivot.SelectedIndex = 0;
                CommandsList.Foreground = _commandBrush;
            }
            catch (ArgumentException argEx)
            {
                var message = argEx.Message;
                var errorCode = message.Substring(0, 11);
                var parseError = string.Empty;
                if (message.StartsWith(ParseError1))
                {
                    parseError = "Parse Error: No matching data!";
                }
                else if (message.StartsWith(ParseError2))
                {
                    parseError = "Parse Error: Multiple FillRule elements present in Path Data!";
                }
                else if (message.StartsWith(ParseError3))
                {
                    var tokens = message.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length == 3)
                    {
                        parseError = $"Parse Error at {tokens[1]}. Cannot parse '{tokens[2]}'.";
                    }
                }
                else
                {
                    parseError = "Parsing error! Invalid input data!";
                }

                args.DrawingSession.FillGeometry(_errorGeometry, Colors.Black);
                CommandsList.Text = parseError;
                RootPivot.SelectedIndex = 1;
                CommandsList.Foreground = _commandErrorBrush;
            }
            catch (Exception)
            {
                args.DrawingSession.FillGeometry(_errorGeometry, Colors.Black);
                CommandsList.Text = "Parsing error! Invalid input data!";
                RootPivot.SelectedIndex = 1;
                CommandsList.Foreground = _commandErrorBrush;
            }
        }

        private void OnStrokeThicknessChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            _strokeThickness = (float)StrokeThickness.Value;
            _selectionChanged = true;
            RenderCanvas.Invalidate();
        }

        private void OnStrokeColorChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StrokeList.SelectedIndex > -1)
            {
                _strokeColor = _colors[StrokeList.SelectedIndex];
                _selectionChanged = true;
            }

            RenderCanvas.Invalidate();
        }

        private void OnFillColorChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FillList.SelectedIndex > -1)
            {
                _fillColor = _colors[FillList.SelectedIndex];
                _selectionChanged = true;
            }

            RenderCanvas.Invalidate();
        }

        private void ShowSample(int index)
        {
            InputData.Text = _samples.ElementAt(index);

            if (!_selectionChanged)
            {
                StrokeThickness.Value = 4;
                StrokeList.SelectedIndex = 1;
                FillList.SelectedIndex = 5;
                _selectionChanged = false;
            }

            _data = InputData.Text;
            RenderCanvas.Invalidate();
        }

        private void OnClearCanvas(object sender, RoutedEventArgs e)
        {
            InputData.Text = string.Empty;
            ParseData();
        }

        private void OnShowRoundedStarSample(object sender, RoutedEventArgs e)
        {
            ShowSample(1);
        }

        private void OnShowStarSample(object sender, RoutedEventArgs e)
        {
            ShowSample(2);
        }

        private void OnShowSpiralSample(object sender, RoutedEventArgs e)
        {
            ShowSample(3);
        }

        private void OnShowFlowerSample(object sender, RoutedEventArgs e)
        {
            ShowSample(4);
        }

        private void OnShowGearSample(object sender, RoutedEventArgs e)
        {
            ShowSample(5);
        }

        public void OnInputTextChanged(object sender, RoutedEventArgs e)
        {
            // Call the ParseData method only after 0.3 seconds have elapsed since last trigger.
            _typeTimer.Debounce(ParseData, TimeSpan.FromSeconds(0.3));
        }
    }
}