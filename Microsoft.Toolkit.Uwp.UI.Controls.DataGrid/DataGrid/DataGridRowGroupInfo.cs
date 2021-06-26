// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Microsoft.Toolkit.Uwp.UI.Controls.DataGridInternals
{
    internal class DataGridRowGroupInfo
    {
        public DataGridRowGroupInfo(
            ICollectionViewGroup collectionViewGroup,
            Visibility visibility,
            int level,
            int slot,
            int lastSubItemSlot)
        {
            this.CollectionViewGroup = collectionViewGroup;
            this.Visibility = visibility;
            this.Level = level;
            this.Slot = slot;
            this.LastSubItemSlot = lastSubItemSlot;
        }

        public ICollectionViewGroup CollectionViewGroup
        {
            get;
            private set;
        }

        public int LastSubItemSlot
        {
            get;
            set;
        }

        public int Level
        {
            get;
            private set;
        }

        public int Slot
        {
            get;
            set;
        }

        public Visibility Visibility
        {
            get;
            set;
        }
    }
}