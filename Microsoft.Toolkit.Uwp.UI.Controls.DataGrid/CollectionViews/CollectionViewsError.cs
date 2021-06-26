// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;

namespace Microsoft.Toolkit.Uwp.UI.Data.Utilities
{
    internal static class CollectionViewsError
    {
        public static class CollectionView
        {
            public static InvalidOperationException EnumeratorVersionChanged()
            {
                return new InvalidOperationException("Collection was modified; enumeration operation cannot execute.");
            }

            public static InvalidOperationException MemberNotAllowedDuringAddOrEdit(string paramName)
            {
                return new InvalidOperationException(Format("'{0}' is not allowed during an AddNew or EditItem transaction.", paramName));
            }

            public static InvalidOperationException NoAccessWhileChangesAreDeferred()
            {
                return new InvalidOperationException("This value cannot be accessed while changes are deferred.");
            }

            public static InvalidOperationException ItemNotAtIndex(string paramName)
            {
                return new InvalidOperationException(Format("The {0} item is not in the collection.", paramName));
            }
        }

        public static class EnumerableCollectionView
        {
            public static InvalidOperationException RemovedItemNotFound()
            {
                return new InvalidOperationException("The removed item is not found in the source collection.");
            }
        }

        public static class ListCollectionView
        {
            public static InvalidOperationException CollectionChangedOutOfRange()
            {
                return new InvalidOperationException("The collection change is out of bounds of the original collection.");
            }

            public static InvalidOperationException AddedItemNotInCollection()
            {
                return new InvalidOperationException("The added item is not in the collection.");
            }

#if FEATURE_IEDITABLECOLLECTIONVIEW
            public static InvalidOperationException CancelEditNotSupported()
            {
                return new InvalidOperationException("CancelEdit is not supported for the current edit item.");
            }

            public static InvalidOperationException MemberNotAllowedDuringTransaction(string paramName1, string paramName2)
            {
                return new InvalidOperationException(Format("'{0}' is not allowed during a transaction started by '{1}'.", paramName1, paramName2));
            }

            public static InvalidOperationException MemberNotAllowedForView(string paramName)
            {
                return new InvalidOperationException(Format("'{0}' is not allowed for this view.", paramName));
            }
#endif
        }

        private static string Format(string formatString, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, formatString, args);
        }
    }
}