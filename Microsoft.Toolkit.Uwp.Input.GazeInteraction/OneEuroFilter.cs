// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
 * http://www.lifl.fr/~casiez/1euro/
 * http://www.lifl.fr/~casiez/publications/CHI2012-casiez.pdf
*/

using System;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    internal class OneEuroFilter : IGazeFilter
    {
        private const float ONEEUROFILTER_DEFAULT_BETA = 5.0f;
        private const float ONEEUROFILTER_DEFAULT_CUTOFF = 0.1f;
        private const float ONEEUROFILTER_DEFAULT_VELOCITY_CUTOFF = 1.0f;

        public OneEuroFilter()
        {
            _lastTimestamp = TimeSpan.Zero;
            Beta = ONEEUROFILTER_DEFAULT_BETA;
            Cutoff = ONEEUROFILTER_DEFAULT_CUTOFF;
            VelocityCutoff = ONEEUROFILTER_DEFAULT_VELOCITY_CUTOFF;
        }

        public OneEuroFilter(float cutoff, float beta)
        {
            _lastTimestamp = TimeSpan.Zero;
            Beta = beta;
            Cutoff = cutoff;
            VelocityCutoff = ONEEUROFILTER_DEFAULT_VELOCITY_CUTOFF;
        }

        public virtual GazeFilterArgs Update(GazeFilterArgs args)
        {
            if (_lastTimestamp == TimeSpan.Zero)
            {
                _lastTimestamp = args.Timestamp;
                _pointFilter = new LowpassFilter(args.Location);
                _deltaFilter = new LowpassFilter(default);
                return new GazeFilterArgs(args.Location, args.Timestamp);
            }

            Point gazePoint = args.Location;

            // Reducing _beta increases lag. Increasing beta decreases lag and improves response time
            // But a really high value of beta also contributes to jitter
            float beta = Beta;

            // This simply represents the cutoff frequency. A lower value reduces jiiter
            // and higher value increases jitter
            float cf = Cutoff;
            Point cutoff = new Point(cf, cf);

            // determine sampling frequency based on last time stamp
            float samplingFrequency = 100000000.0f / Math.Max(1, (args.Timestamp - _lastTimestamp).Ticks);
            _lastTimestamp = args.Timestamp;

            // calculate change in distance...
            Point deltaDistance;
            deltaDistance.X = gazePoint.X - _pointFilter.Previous.X;
            deltaDistance.Y = gazePoint.Y - _pointFilter.Previous.Y;

            // ...and velocity
            var velocity = new Point(deltaDistance.X * samplingFrequency, deltaDistance.Y * samplingFrequency);

            // find the alpha to use for the velocity filter
            float velocityAlpha = Alpha(samplingFrequency, VelocityCutoff);
            var velocityAlphaPoint = new Point(velocityAlpha, velocityAlpha);

            // find the filtered velocity
            Point filteredVelocity = _deltaFilter.Update(velocity, velocityAlphaPoint);

            // ignore sign since it will be taken care of by deltaDistance
            filteredVelocity.X = Math.Abs(filteredVelocity.X);
            filteredVelocity.Y = Math.Abs(filteredVelocity.Y);

            // compute new cutoff to use based on velocity
            cutoff.X += beta * filteredVelocity.X;
            cutoff.Y += beta * filteredVelocity.Y;

            // find the new alpha to use to filter the points
            var distanceAlpha = new Point(Alpha(samplingFrequency, (float)cutoff.X), Alpha(samplingFrequency, (float)cutoff.Y));

            // find the filtered point
            Point filteredPoint = _pointFilter.Update(gazePoint, distanceAlpha);

            // compute the new args
            var fa = new GazeFilterArgs(filteredPoint, args.Timestamp);
            return fa;
        }

        public virtual void LoadSettings(ValueSet settings)
        {
            if (settings.ContainsKey("OneEuroFilter.Beta"))
            {
                Beta = (float)settings["OneEuroFilter.Beta"];
            }

            if (settings.ContainsKey("OneEuroFilter.Cutoff"))
            {
                Cutoff = (float)settings["OneEuroFilter.Cutoff"];
            }

            if (settings.ContainsKey("OneEuroFilter.VelocityCutoff"))
            {
                VelocityCutoff = (float)settings["OneEuroFilter.VelocityCutoff"];
            }
        }

        public float Beta { get; set; }

        public float Cutoff { get; set; }

        public float VelocityCutoff { get; set; }

        private float Alpha(float rate, float cutoff)
        {
            const float PI = 3.14159265f;
            float te = 1.0f / rate;
            float tau = (float)(1.0f / (2 * PI * cutoff));
            float alpha = te / (te + tau);
            return alpha;
        }

        private TimeSpan _lastTimestamp;
        private LowpassFilter _pointFilter;
        private LowpassFilter _deltaFilter;
    }
}