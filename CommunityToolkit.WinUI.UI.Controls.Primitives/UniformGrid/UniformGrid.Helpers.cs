// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// The UniformGrid control presents information within a Grid with even spacing.
    /// </summary>
    public partial class UniformGrid : Grid
    {
        // Provides the next spot in the boolean array with a 'false' value.
#pragma warning disable SA1009 // Closing parenthesis must be followed by a space.
        internal static IEnumerable<(int row, int column)> GetFreeSpot(TakenSpotsReferenceHolder arrayref, int firstcolumn, bool topdown)
#pragma warning restore SA1009 // Closing parenthesis must be followed by a space.
        {
            if (topdown)
            {
                var rows = arrayref.Height;

                // Layout spots from Top-Bottom, Left-Right (right-left handled automatically by Grid with Flow-Direction).
                // Effectively transpose the Grid Layout.
                for (int c = 0; c < arrayref.Width; c++)
                {
                    int start = (c == 0 && firstcolumn > 0 && firstcolumn < rows) ? firstcolumn : 0;
                    for (int r = start; r < rows; r++)
                    {
                        if (!arrayref[r, c])
                        {
                            yield return (r, c);
                        }
                    }
                }
            }
            else
            {
                var columns = arrayref.Width;

                // Layout spots as normal from Left-Right.
                // (right-left handled automatically by Grid with Flow-Direction
                // during its layout, internal model is always left-right).
                for (int r = 0; r < arrayref.Height; r++)
                {
                    int start = (r == 0 && firstcolumn > 0 && firstcolumn < columns) ? firstcolumn : 0;
                    for (int c = start; c < columns; c++)
                    {
                        if (!arrayref[r, c])
                        {
                            yield return (r, c);
                        }
                    }
                }
            }
        }

        // Based on the number of visible children,
        // returns the dimensions of the
        // grid we need to hold all elements.
#pragma warning disable SA1008 // Opening parenthesis must be spaced correctly
        internal static (int rows, int columns) GetDimensions(FrameworkElement[] visible, int rows, int cols, int firstColumn)
#pragma warning restore SA1008 // Opening parenthesis must be spaced correctly
        {
            // If a dimension isn't specified, we need to figure out the other one (or both).
            if (rows == 0 || cols == 0)
            {
                // Calculate the size & area of all objects in the grid to know how much space we need.
                var count = Math.Max(1, visible.Sum(item => GetRowSpan(item) * GetColumnSpan(item)));

                if (rows == 0)
                {
                    if (cols > 0)
                    {
                        // Bound check
                        var first = (firstColumn >= cols || firstColumn < 0) ? 0 : firstColumn;

                        // If we have columns but no rows, calculate rows based on column offset and number of children.
                        rows = (count + first + (cols - 1)) / cols;
                        return (rows, cols);
                    }
                    else
                    {
                        // Otherwise, determine square layout if both are zero.
                        var size = (int)Math.Ceiling(Math.Sqrt(count));

                        // Figure out if firstColumn is in bounds
                        var first = (firstColumn >= size || firstColumn < 0) ? 0 : firstColumn;

                        rows = (int)Math.Ceiling(Math.Sqrt(count + first));
                        return (rows, rows);
                    }
                }
                else if (cols == 0)
                {
                    // If we have rows and no columns, then calculate columns needed based on rows
                    cols = (count + (rows - 1)) / rows;

                    // Now that we know a rough size of our shape, see if the FirstColumn effects that:
                    var first = (firstColumn >= cols || firstColumn < 0) ? 0 : firstColumn;

                    cols = (count + first + (rows - 1)) / rows;
                }
            }

            return (rows, cols);
        }

        // Used to interleave specified row dimensions with automatic rows added to use
        // underlying Grid layout for main arrange of UniformGrid.
        internal void SetupRowDefinitions(int rows)
        {
            // Mark initial definitions so we don't erase them.
            foreach (var rd in RowDefinitions)
            {
                if (GetAutoLayout(rd) == null)
                {
                    SetAutoLayout(rd, false);
                }
            }

            // Remove non-autolayout rows we've added and then add them in the right spots.
            if (rows != RowDefinitions.Count)
            {
                for (int r = RowDefinitions.Count - 1; r >= 0; r--)
                {
                    if (GetAutoLayout(RowDefinitions[r]) == true)
                    {
                        RowDefinitions.RemoveAt(r);
                    }
                }

                for (int r = this.RowDefinitions.Count; r < rows; r++)
                {
                    var rd = new RowDefinition();
                    SetAutoLayout(rd, true);
                    this.RowDefinitions.Insert(r, rd);
                }
            }
        }

        // Used to interleave specified column dimensions with automatic columns added to use
        // underlying Grid layout for main arrange of UniformGrid.
        internal void SetupColumnDefinitions(int columns)
        {
            // Mark initial definitions so we don't erase them.
            foreach (var cd in ColumnDefinitions)
            {
                if (GetAutoLayout(cd) == null)
                {
                    SetAutoLayout(cd, false);
                }
            }

            // Remove non-autolayout columns we've added and then add them in the right spots.
            if (columns != ColumnDefinitions.Count)
            {
                for (int c = ColumnDefinitions.Count - 1; c >= 0; c--)
                {
                    if (GetAutoLayout(ColumnDefinitions[c]) == true)
                    {
                        this.ColumnDefinitions.RemoveAt(c);
                    }
                }

                for (int c = ColumnDefinitions.Count; c < columns; c++)
                {
                    var cd = new ColumnDefinition();
                    SetAutoLayout(cd, true);
                    ColumnDefinitions.Insert(c, cd);
                }
            }
        }
    }
}