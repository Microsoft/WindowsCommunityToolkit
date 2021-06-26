// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    /// <summary>
    /// Provides basic stats for eye gaze
    /// </summary>
    internal class GazeStats
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GazeStats"/> class.
        /// </summary>
        /// <param name="maxHistoryLen">Defines the size of the circular history for the calculations of stats</param>
        public GazeStats(int maxHistoryLen)
        {
            _maxHistoryLen = maxHistoryLen;
            _history = new List<Point>();
        }

        /// <summary>
        /// Resets the history, and stats
        /// </summary>
        public void Reset()
        {
            _sumX = 0;
            _sumY = 0;
            _sumSquaredX = 0;
            _sumSquaredY = 0;
            _history.Clear();
        }

        /// <summary>
        /// Adds a new item to the history
        /// </summary>
        /// <param name="x">X axis of the new point</param>
        /// <param name="y">Y axis of the new point</param>
        public void Update(float x, float y)
        {
            var pt = new Point(x, y);
            _history.Add(pt);

            if (_history.Count > _maxHistoryLen)
            {
                var oldest = _history[0];
                _history.RemoveAt(0);

                _sumX -= oldest.X;
                _sumY -= oldest.Y;
                _sumSquaredX -= oldest.X * oldest.X;
                _sumSquaredY -= oldest.Y * oldest.Y;
            }

            _sumX += x;
            _sumY += y;
            _sumSquaredX += x * x;
            _sumSquaredY += y * y;
        }

        /// <summary>
        /// Gets the mean of the history of points
        /// </summary>
        public Point Mean
        {
            get
            {
                var count = _history.Count;
                if (count < _maxHistoryLen)
                {
                    return new Point(0.0f, 0.0f);
                }

                return new Point((float)_sumX / count, (float)_sumY / count);
            }
        }

        /// <summary>
        /// Gets the StandardDeviation of the items on the history => sqrt(Variance) = sqrt(E[X^2] – (E[X])^2)
        /// </summary>
        public Point StandardDeviation
        {
            get
            {
                var count = _history.Count;
                if (count < _maxHistoryLen)
                {
                    return new Point(0.0f, 0.0f);
                }

                double meanX = _sumX / count;
                double meanY = _sumY / count;
                float stddevX = (float)Math.Sqrt((_sumSquaredX / count) - (meanX * meanX));
                float stddevY = (float)Math.Sqrt((_sumSquaredY / count) - (meanY * meanY));
                return new Point(stddevX, stddevY);
            }
        }

        private readonly int _maxHistoryLen;
        private readonly List<Point> _history;
        private double _sumX;
        private double _sumY;
        private double _sumSquaredX;
        private double _sumSquaredY;
    }
}