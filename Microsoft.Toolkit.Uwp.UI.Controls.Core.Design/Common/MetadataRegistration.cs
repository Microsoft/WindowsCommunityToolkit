// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using Microsoft.Toolkit.Uwp.Design.Common;
using Microsoft.Toolkit.Uwp.UI.Controls.Design;

using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;

[assembly: ProvideMetadata(typeof(MetadataRegistration))]

namespace Microsoft.Toolkit.Uwp.UI.Controls.Design
{
    public class MetadataRegistration : MetadataRegistrationBase
    {
        public MetadataRegistration() : base()
        {
            // Note:
            // The default constructor sets value of 'AssemblyFullName' and
            // 'XmlResourceName' used by 'MetadataRegistrationBase.AddDescriptions()'.
            // The convention here is that the root namespace plus the Controls category.
            // Example:
            // "Microsoft.Toolkit.Uwp.UI.Controls" (root namespace) + "." + "Primitives" (Controls category) + ".xml"

            Type thisType = this.GetType();
            AssemblyName designLib = thisType.Assembly.GetName();

            string annexString = ".DesignTools";
            int annexStart = designLib.Name.LastIndexOf(annexString);
            string controlLibName = designLib.Name.Remove(annexStart, annexString.Length);

            AssemblyFullName = designLib.FullName;
            XmlResourceName = $"{controlLibName}.xml";
        }
    }
}