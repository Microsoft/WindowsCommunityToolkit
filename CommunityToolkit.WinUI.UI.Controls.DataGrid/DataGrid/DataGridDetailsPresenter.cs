// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.UI.Automation.Peers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

namespace CommunityToolkit.WinUI.UI.Controls.Primitives
{
    /// <summary>
    /// Used within the template of a <see cref="DataGrid"/> to specify the location in the control's visual tree
    /// where the row details are to be added.
    /// </summary>
    public sealed class DataGridDetailsPresenter : Panel
    {
        /// <summary>
        /// Gets or sets the height of the content.
        /// </summary>
        /// <returns>
        /// The height of the content.
        /// </returns>
        public double ContentHeight
        {
            get { return (double)GetValue(ContentHeightProperty); }
            set { SetValue(ContentHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the ContentHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentHeightProperty =
            DependencyProperty.Register(
                "ContentHeight",
                typeof(double),
                typeof(DataGridDetailsPresenter),
                new PropertyMetadata(0.0, OnContentHeightPropertyChanged));

        /// <summary>
        /// ContentHeightProperty property changed handler.
        /// </summary>
        /// <param name="d">DataGridDetailsPresenter.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnContentHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridDetailsPresenter detailsPresenter = d as DataGridDetailsPresenter;
            detailsPresenter.InvalidateMeasure();
        }

        private DataGrid OwningGrid
        {
            get
            {
                if (this.OwningRow != null)
                {
                    return this.OwningRow.OwningGrid;
                }

                return null;
            }
        }

        internal DataGridRow OwningRow
        {
            get;
            set;
        }

        /// <summary>
        /// Arranges the content of the <see cref="T:System.Windows.Controls.Primitives.DataGridDetailsPresenter"/>.
        /// </summary>
        /// <returns>
        /// The actual size used by the <see cref="T:System.Windows.Controls.Primitives.DataGridDetailsPresenter"/>.
        /// </returns>
        /// <param name="finalSize">
        /// The final area within the parent that this element should use to arrange itself and its children.
        /// </param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.OwningGrid == null)
            {
                return base.ArrangeOverride(finalSize);
            }

            double rowGroupSpacerWidth = this.OwningGrid.ColumnsInternal.RowGroupSpacerColumn.Width.Value;
            double xClip = this.OwningGrid.AreRowGroupHeadersFrozen ? rowGroupSpacerWidth : 0;
            double leftEdge = rowGroupSpacerWidth;
            double width;
            if (this.OwningGrid.AreRowDetailsFrozen)
            {
                leftEdge += this.OwningGrid.HorizontalOffset;
                width = this.OwningGrid.CellsWidth;
            }
            else
            {
                xClip += this.OwningGrid.HorizontalOffset;
                width = Math.Max(this.OwningGrid.CellsWidth, this.OwningGrid.ColumnsInternal.VisibleEdgedColumnsWidth);
            }

            // Details should not extend through the indented area
            width -= rowGroupSpacerWidth;
            double height = Math.Max(0, double.IsNaN(this.ContentHeight) ? 0 : this.ContentHeight);

            foreach (UIElement child in this.Children)
            {
                child.Arrange(new Rect(leftEdge, 0, width, height));
            }

            if (this.OwningGrid.AreRowDetailsFrozen)
            {
                // Frozen Details should not be clipped, similar to frozen cells
                this.Clip = null;
            }
            else
            {
                // Clip so Details doesn't obstruct elements to the left (the RowHeader by default) as we scroll to the right
                RectangleGeometry rg = new RectangleGeometry();
                rg.Rect = new Rect(xClip, 0, Math.Max(0, width - xClip + rowGroupSpacerWidth), height);
                this.Clip = rg;
            }

            return finalSize;
        }

        /// <summary>
        /// Measures the children of a <see cref="T:System.Windows.Controls.Primitives.DataGridDetailsPresenter"/> to
        /// prepare for arranging them during the <see cref="M:System.Windows.FrameworkElement.ArrangeOverride(System.Windows.Size)"/> pass.
        /// </summary>
        /// <param name="availableSize">
        /// The available size that this element can give to child elements. Indicates an upper limit that child elements should not exceed.
        /// </param>
        /// <returns>
        /// The size that the <see cref="T:System.Windows.Controls.Primitives.DataGridDetailsPresenter"/> determines it needs during layout, based on its calculations of child object allocated sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.OwningGrid == null || this.Children.Count == 0)
            {
                return new Size(0.0, 0.0);
            }

            double desiredWidth = this.OwningGrid.AreRowDetailsFrozen ?
                this.OwningGrid.CellsWidth :
                Math.Max(this.OwningGrid.CellsWidth, this.OwningGrid.ColumnsInternal.VisibleEdgedColumnsWidth);

            desiredWidth -= this.OwningGrid.ColumnsInternal.RowGroupSpacerColumn.Width.Value;

            foreach (UIElement child in this.Children)
            {
                child.Measure(new Size(desiredWidth, double.PositiveInfinity));
            }

            double desiredHeight = Math.Max(0, double.IsNaN(this.ContentHeight) ? 0 : this.ContentHeight);

            return new Size(desiredWidth, desiredHeight);
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        /// <returns>An automation peer for this <see cref="T:System.Windows.Controls.Primitives.DataGridDetailsPresenter"/>.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataGridDetailsPresenterAutomationPeer(this);
        }
    }
}