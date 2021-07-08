// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using Windows.Devices.Input.Preview;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmokeTest
{
    public sealed partial class MainPage
    {
        private GazeInputSourcePreview gazeInputSourcePreview;

        public ObservableCollection<Point> GazeHistory { get; } = new ObservableCollection<Point>();

        public int TracePointDiameter { get; set; }

        public int MaxGazeHistorySize { get; set; }

        public bool ShowIntermediatePoints { get; set; }

        public MainPage()
        {
            InitializeComponent();

            ShowIntermediatePoints = false;
            MaxGazeHistorySize = 100;

            gazeInputSourcePreview = GazeInputSourcePreview.GetForCurrentView();
            gazeInputSourcePreview.GazeMoved += GazeInputSourcePreview_GazeMoved;
        }

        private void UpdateGazeHistory(GazePointPreview pt)
        {
            if (!pt.EyeGazePosition.HasValue)
            {
                return;
            }

            var transform = (Window.Current.Content as Frame).TransformToVisual(this);
            var point = transform.TransformPoint(pt.EyeGazePosition.Value);
            GazeHistory.Add(point);
            if (GazeHistory.Count > MaxGazeHistorySize)
            {
                GazeHistory.RemoveAt(0);
            }
        }

        private void GazeInputSourcePreview_GazeMoved(GazeInputSourcePreview sender, GazeMovedPreviewEventArgs args)
        {
            if (!ShowIntermediatePoints)
            {
                UpdateGazeHistory(args.CurrentPoint);
                return;
            }

            var points = args.GetIntermediatePoints();
            foreach (var pt in points)
            {
                UpdateGazeHistory(pt);
            }
        }
    }
}