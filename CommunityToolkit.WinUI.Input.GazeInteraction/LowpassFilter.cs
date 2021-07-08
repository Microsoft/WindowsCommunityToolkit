// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
 * http://www.lifl.fr/~casiez/1euro/
 * http://www.lifl.fr/~casiez/publications/CHI2012-casiez.pdf
*/

using Windows.Foundation;

namespace CommunityToolkit.WinUI.Input.GazeInteraction
{
    internal class LowpassFilter
    {
        public LowpassFilter()
        {
            Previous = new Point(0, 0);
        }

        public LowpassFilter(Point initial)
        {
            Previous = initial;
        }

        public Point Previous { get; set; }

        public Point Update(Point point, Point alpha)
        {
            Point pt = default;
            pt.X = (alpha.X * point.X) + ((1 - alpha.X) * Previous.X);
            pt.Y = (alpha.Y * point.Y) + ((1 - alpha.Y) * Previous.Y);
            Previous = pt;
            return Previous;
        }
    }
}