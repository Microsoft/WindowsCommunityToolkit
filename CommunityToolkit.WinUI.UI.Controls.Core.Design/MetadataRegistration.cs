// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using CommunityToolkit.WinUI.Design.Common;
using CommunityToolkit.WinUI.UI.Controls.Design;

using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;

[assembly: ProvideMetadata(typeof(MetadataRegistration))]

namespace CommunityToolkit.WinUI.UI.Controls.Design
{
    public class MetadataRegistration : MetadataRegistrationBase
    {
        public MetadataRegistration() : base()
        {
            // Note:
            // The default constructor sets value of 'AssemblyFullName' and
            // 'XmlResourceName' used by 'MetadataRegistrationBase.AddDescriptions()'.
            // The convention here is that the <RootNamespace> in '.DesignTools.csproj'
            // (or Default namespace in Project -> Properties -> Application tab)
            // must be the same as runtime assembly's main namespace plus ".Design".
            Type thisType = this.GetType();
            AssemblyName designLib = thisType.Assembly.GetName();

            string annexString = ".DesignTools";
            int annexStart = designLib.Name.LastIndexOf(annexString);
            string controlLibName = designLib.Name.Remove(annexStart, annexString.Length);

            AssemblyFullName = designLib.FullName;
            XmlResourceName = $"{thisType.Namespace}{controlLibName}.xml";
        }
    }
}