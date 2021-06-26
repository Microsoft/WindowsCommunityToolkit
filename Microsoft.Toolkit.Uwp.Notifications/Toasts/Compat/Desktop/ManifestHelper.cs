// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WIN32

using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Windows.ApplicationModel;

namespace Microsoft.Toolkit.Uwp.Notifications
{
    internal class ManifestHelper
    {
        private XmlDocument _doc;
        private XmlNamespaceManager _namespaceManager;

        public ManifestHelper()
        {
            var appxManifestPath = Path.Combine(Package.Current.InstalledLocation.Path, "AppxManifest.xml");
            var doc = new XmlDocument();
            doc.Load(appxManifestPath);

            var namespaceManager = new XmlNamespaceManager(doc.NameTable);
            namespaceManager.AddNamespace("default", "http://schemas.microsoft.com/appx/manifest/foundation/windows10");
            namespaceManager.AddNamespace("desktop", "http://schemas.microsoft.com/appx/manifest/desktop/windows10");
            namespaceManager.AddNamespace("com", "http://schemas.microsoft.com/appx/manifest/com/windows10");

            _doc = doc;
            _namespaceManager = namespaceManager;
        }

        internal string GetAumidFromPackageManifest()
        {
            var appNode = _doc.SelectSingleNode("/default:Package/default:Applications/default:Application[1]", _namespaceManager);

            if (appNode == null)
            {
                throw new InvalidOperationException("Your MSIX app manifest must have an <Application> entry.");
            }

            return Package.Current.Id.FamilyName + "!" + appNode.Attributes["Id"].Value;
        }

        internal string GetClsidFromPackageManifest()
        {
            var activatorClsidNode = _doc.SelectSingleNode("/default:Package/default:Applications/default:Application[1]/default:Extensions/desktop:Extension[@Category='windows.toastNotificationActivation']/desktop:ToastNotificationActivation/@ToastActivatorCLSID", _namespaceManager);

            if (activatorClsidNode == null)
            {
                throw new InvalidOperationException("Your app manifest must have a toastNotificationActivation extension with a valid ToastActivatorCLSID specified.");
            }

            var clsid = activatorClsidNode.Value;

            // Make sure they have a COM class registration matching the CLSID
            var comClassNode = _doc.SelectSingleNode($"/default:Package/default:Applications/default:Application[1]/default:Extensions/com:Extension[@Category='windows.comServer']/com:ComServer/com:ExeServer/com:Class[@Id='{clsid}']", _namespaceManager);

            if (comClassNode == null)
            {
                throw new InvalidOperationException("Your app manifest must have a comServer extension with a class ID matching your toastNotificationActivator's CLSID.");
            }

            // Make sure they have a COM class registration matching their current process executable
            var comExeServerNode = comClassNode.ParentNode;

            var specifiedExeRelativePath = comExeServerNode.Attributes["Executable"].Value;
            var specifiedExeFullPath = Path.Combine(Package.Current.InstalledLocation.Path, specifiedExeRelativePath);
            var actualExeFullPath = Process.GetCurrentProcess().MainModule.FileName;

            if (specifiedExeFullPath != actualExeFullPath)
            {
                var correctExeRelativePath = actualExeFullPath.Substring(Package.Current.InstalledLocation.Path.Length + 1);
                throw new InvalidOperationException($"Your app manifest's comServer extension's Executable value is incorrect. It should be \"{correctExeRelativePath}\".");
            }

            // Make sure their arguments are set correctly
            var argumentsNode = comExeServerNode.Attributes.GetNamedItem("Arguments");
            if (argumentsNode == null || argumentsNode.Value != "-ToastActivated")
            {
                throw new InvalidOperationException("Your app manifest's comServer extension for toast activation must have its Arguments set exactly to \"-ToastActivated\"");
            }

            return clsid;
        }
    }
}

#endif