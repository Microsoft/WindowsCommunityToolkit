// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace Microsoft.Toolkit.Uwp.UI.Controls.DataGridInternals
{
    internal class DataGridCellCoordinates
    {
        public DataGridCellCoordinates(int columnIndex, int slot)
        {
            this.ColumnIndex = columnIndex;
            this.Slot = slot;
        }

        public DataGridCellCoordinates(DataGridCellCoordinates dataGridCellCoordinates)
            : this(dataGridCellCoordinates.ColumnIndex, dataGridCellCoordinates.Slot)
        {
        }

        public int ColumnIndex
        {
            get;
            set;
        }

        public int Slot
        {
            get;
            set;
        }

        public override bool Equals(object o)
        {
            DataGridCellCoordinates dataGridCellCoordinates = o as DataGridCellCoordinates;
            if (dataGridCellCoordinates != null)
            {
                return dataGridCellCoordinates.ColumnIndex == this.ColumnIndex && dataGridCellCoordinates.Slot == this.Slot;
            }

            return false;
        }

        // Avoiding build warning CS0659: 'DataGridCellCoordinates' overrides Object.Equals(object o) but does not override Object.GetHashCode()
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

#if DEBUG
        public override string ToString()
        {
            return "DataGridCellCoordinates {ColumnIndex = " + this.ColumnIndex.ToString(CultureInfo.CurrentCulture) +
                   ", Slot = " + this.Slot.ToString(CultureInfo.CurrentCulture) + "}";
        }
#endif
    }
}