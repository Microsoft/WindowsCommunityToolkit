// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    internal class InkCanvasExportModel
    {
        public List<IDrawable> DrawableList { get; set; }

        // Create new class from the export model, new version is created with every breaking changes so we could be able to map back from the file when adding new features
        public int Version { get; set; }
    }
}