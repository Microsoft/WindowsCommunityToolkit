// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;
using Windows.Storage;

namespace CommunityToolkit.WinUI.Helpers
{
    /// <summary>
    /// Store data in the Roaming environment (shared across all user devices).
    /// </summary>
    [Obsolete("Package State Roaming will be removed in a futures Windows Update, see https://docs.microsoft.com/windows/deployment/planning/windows-10-deprecated-features for more information.")]
    public class RoamingObjectStorageHelper : BaseObjectStorageHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoamingObjectStorageHelper"/> class,
        /// which can read and write data using the provided <see cref="IObjectSerializer"/>;
        /// In 6.1 and older the default Serializer was based on Newtonsoft.Json.
        /// To implement an <see cref="IObjectSerializer"/> based on System.Text.Json, Newtonsoft.Json, or DataContractJsonSerializer see https://aka.ms/wct/storagehelper-migration
        /// </summary>
        /// <param name="objectSerializer">The serializer to use.</param>
        public RoamingObjectStorageHelper(IObjectSerializer objectSerializer)
            : base(objectSerializer)
        {
            Settings = ApplicationData.Current.RoamingSettings;
            Folder = ApplicationData.Current.RoamingFolder;
        }
    }
}