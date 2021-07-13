// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.Storage;

namespace Microsoft.Toolkit.Uwp.Helpers
{
    /// <summary>
    /// Store data in the Local environment (only on the current device).
    /// </summary>
    [Obsolete("LocalObjectStorageHelper is deprecated and has been superceded by the ApplicationDataStorageHelper.")]
    public class LocalObjectStorageHelper : BaseObjectStorageHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalObjectStorageHelper"/> class,
        /// which can read and write data using the provided <see cref="IObjectSerializer"/>;
        /// In 6.1 and older the default Serializer was based on Newtonsoft.Json.
        /// To implement an <see cref="IObjectSerializer"/> based on System.Text.Json, Newtonsoft.Json, or DataContractJsonSerializer see https://aka.ms/wct/storagehelper-migration
        /// </summary>
        /// <param name="objectSerializer">The serializer to use.</param>
        public LocalObjectStorageHelper(IObjectSerializer objectSerializer)
            : base(objectSerializer)
        {
            Settings = ApplicationData.Current.LocalSettings;
            Folder = ApplicationData.Current.LocalFolder;
        }
    }
}