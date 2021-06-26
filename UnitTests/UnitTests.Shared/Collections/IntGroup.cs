// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace UnitTests.Collections
{
    public class IntGroup : List<int>, IGrouping<string, int>
    {
        public IntGroup(string key, IEnumerable<int> collection)
            : base(collection)
        {
            Key = key;
        }

        public string Key { get; }
    }
}