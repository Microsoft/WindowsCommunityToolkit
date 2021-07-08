// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Utilities
{
    internal class Range<T>
    {
        public Range(int lowerBound, int upperBound, T value)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
            Value = value;
        }

        public int Count
        {
            get
            {
                return UpperBound - LowerBound + 1;
            }
        }

        public int LowerBound
        {
            get;
            set;
        }

        public int UpperBound
        {
            get;
            set;
        }

        public T Value
        {
            get;
            set;
        }

        public bool ContainsIndex(int index)
        {
            return LowerBound <= index && UpperBound >= index;
        }

        public bool ContainsValue(object value)
        {
            return (this.Value == null) ? value == null : this.Value.Equals(value);
        }

        public Range<T> Copy()
        {
            return new Range<T>(LowerBound, UpperBound, Value);
        }
    }
}