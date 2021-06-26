// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Drawing;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Referencable class object we can use to have a reference shared between
    /// our <see cref="UniformGrid.MeasureOverride"/> and
    /// <see cref="UniformGrid.GetFreeSpot"/> iterator.
    /// This is used so we can better isolate our logic and make it easier to test.
    /// </summary>
    internal sealed class TakenSpotsReferenceHolder
    {
        /// <summary>
        /// The <see cref="BitArray"/> instance used to efficiently track empty spots.
        /// </summary>
        private readonly BitArray spotsTaken;

        /// <summary>
        /// Initializes a new instance of the <see cref="TakenSpotsReferenceHolder"/> class.
        /// </summary>
        /// <param name="rows">The number of rows to track.</param>
        /// <param name="columns">The number of columns to track.</param>
        public TakenSpotsReferenceHolder(int rows, int columns)
        {
            Height = rows;
            Width = columns;

            this.spotsTaken = new BitArray(rows * columns);
        }

        /// <summary>
        /// Gets the height of the grid to monitor.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the width of the grid to monitor.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets or sets the value of a specified grid cell.
        /// </summary>
        /// <param name="i">The vertical offset.</param>
        /// <param name="j">The horizontal offset.</param>
        public bool this[int i, int j]
        {
            get => this.spotsTaken[(i * Width) + j];
            set => this.spotsTaken[(i * Width) + j] = value;
        }

        /// <summary>
        /// Fills the specified area in the current grid with a given value.
        /// If invalid coordinates are given, they will simply be ignored and no exception will be thrown.
        /// </summary>
        /// <param name="value">The value to fill the target area with.</param>
        /// <param name="row">The row to start on (inclusive, 0-based index).</param>
        /// <param name="column">The column to start on (inclusive, 0-based index).</param>
        /// <param name="width">The positive width of area to fill.</param>
        /// <param name="height">The positive height of area to fill.</param>
        public void Fill(bool value, int row, int column, int width, int height)
        {
            Rectangle bounds = new Rectangle(0, 0, Width, Height);

            // Precompute bounds to skip branching in main loop
            bounds.Intersect(new Rectangle(column, row, width, height));

            for (int i = bounds.Top; i < bounds.Bottom; i++)
            {
                for (int j = bounds.Left; j < bounds.Right; j++)
                {
                    this[i, j] = value;
                }
            }
        }

        /// <summary>
        /// Resets the current reference holder.
        /// </summary>
        public void Reset()
        {
            this.spotsTaken.SetAll(false);
        }
    }
}