// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using Microsoft.Toolkit.Uwp.UI;
using Windows.Devices.Input.Preview;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GazeTracingPage : IXamlRenderListener
    {
        private GazeInputSourcePreview gazeInputSourcePreview;
        private Frame rootFrame;

        public ObservableCollection<Point> GazeHistory { get; set; } = new ObservableCollection<Point>();

        public int TracePointDiameter { get; set; }

        public int MaxGazeHistorySize { get; set; }

        public bool ShowIntermediatePoints { get; set; }

        public GazeTracingPage()
        {
            this.InitializeComponent();
            DataContext = this;

            ShowIntermediatePoints = false;
            MaxGazeHistorySize = 100;

            rootFrame = Window.Current.Content as Frame;
            gazeInputSourcePreview = GazeInputSourcePreview.GetForCurrentView();
            gazeInputSourcePreview.GazeMoved += GazeInputSourcePreview_GazeMoved;
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            if (control.FindChild("Points") is ItemsControl itemsControl)
            {
                itemsControl.ItemsSource = GazeHistory;
            }
        }

        private void UpdateGazeHistory(GazePointPreview pt)
        {
            if (!pt.EyeGazePosition.HasValue)
            {
                return;
            }

            var transform = rootFrame.TransformToVisual(this);
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