// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WIN32

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace CommunityToolkit.WinUI.Notifications
{
    internal class Win32AppInfo
    {
        /// <summary>
        /// If an AUMID is greater than 129 characters, scheduled toast notification APIs will throw an exception.
        /// </summary>
        private const int AUMID_MAX_LENGTH = 129;

        public string Aumid { get; set; }

        /// <summary>
        /// Gets the AUMID before it was fixed up with the backslash issue
        /// </summary>
        public string Pre7_0_1Aumid { get; private set; }

        public string DisplayName { get; set; }

        public string IconPath { get; set; }

        public static Win32AppInfo Get()
        {
            var process = Process.GetCurrentProcess();

            // First get the app ID
            IApplicationResolver appResolver = (IApplicationResolver)new CAppResolver();
            appResolver.GetAppIDForProcess(Convert.ToUInt32(process.Id), out string appId, out _, out _, out _);

            string aumid;
            string pre7_0_1Aumid = null;

            // If the app ID is too long
            if (appId.Length > AUMID_MAX_LENGTH)
            {
                // Hash the AUMID
                aumid = HashAppId(appId);
            }

            // Else if it contains a backslash
            else if (appId.Contains('\\'))
            {
                // For versions 19042 and older of Windows 10, we can't use backslashes - Issue #3870
                // So we change it to not include those
                aumid = appId.Replace('\\', '/');
                pre7_0_1Aumid = appId;
            }
            else
            {
                // Use as-is
                aumid = appId;
            }

            // Then try to get the shortcut (for display name and icon)
            IShellItem shortcutItem = null;
            try
            {
                appResolver.GetBestShortcutForAppID(appId, out shortcutItem);
            }
            catch
            {
            }

            string displayName = null;
            string iconPath = null;

            // First we attempt to use display assets from the shortcut itself
            if (shortcutItem != null)
            {
                try
                {
                    shortcutItem.GetDisplayName(0, out displayName);

                    ((IShellItemImageFactory)shortcutItem).GetImage(new SIZE(48, 48), SIIGBF.IconOnly | SIIGBF.BiggerSizeOk, out IntPtr nativeHBitmap);

                    if (nativeHBitmap != IntPtr.Zero)
                    {
                        try
                        {
                            Bitmap bmp = Bitmap.FromHbitmap(nativeHBitmap);

                            if (IsAlphaBitmap(bmp, out var bmpData))
                            {
                                var alphaBitmap = GetAlphaBitmapFromBitmapData(bmpData);
                                iconPath = SaveIconToAppPath(alphaBitmap, aumid);
                            }
                            else
                            {
                                iconPath = SaveIconToAppPath(bmp, aumid);
                            }
                        }
                        catch
                        {
                        }

                        NativeMethods.DeleteObject(nativeHBitmap);
                    }
                }
                catch
                {
                }
            }

            // If we didn't get a display name from shortcut
            if (string.IsNullOrWhiteSpace(displayName))
            {
                // We use the one from the process
                displayName = GetDisplayNameFromCurrentProcess(process);
            }

            // If we didn't get an icon from shortcut
            if (string.IsNullOrWhiteSpace(iconPath))
            {
                // We use the one from the process
                iconPath = ExtractAndObtainIconFromCurrentProcess(process, aumid);
            }

            return new Win32AppInfo()
            {
                Aumid = aumid,
                Pre7_0_1Aumid = pre7_0_1Aumid,
                DisplayName = displayName,
                IconPath = iconPath
            };
        }

        private static string HashAppId(string appId)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hashedBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(appId));
                return string.Join(string.Empty, hashedBytes.Select(b => b.ToString("X2")));
            }
        }

        private static string GetDisplayNameFromCurrentProcess(Process process)
        {
            // If AssemblyTitle is set, use that
            var assemblyTitleAttr = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyTitleAttribute>();
            if (assemblyTitleAttr != null)
            {
                return assemblyTitleAttr.Title;
            }

            // Otherwise, fall back to process name
            return process.ProcessName;
        }

        private static string ExtractAndObtainIconFromCurrentProcess(Process process, string aumid)
        {
            return ExtractAndObtainIconFromPath(process.MainModule.FileName, aumid);
        }

        private static string ExtractAndObtainIconFromPath(string pathToExtract, string aumid)
        {
            try
            {
                // Extract the icon
                var icon = Icon.ExtractAssociatedIcon(pathToExtract);

                using (var bmp = icon.ToBitmap())
                {
                    return SaveIconToAppPath(bmp, aumid);
                }
            }
            catch
            {
                return null;
            }
        }

        private static string SaveIconToAppPath(Bitmap bitmap, string aumid)
        {
            try
            {
                var path = Path.Combine(GetAppDataFolderPath(aumid), "Icon.png");

                // Ensure the directories exist
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                bitmap.Save(path, ImageFormat.Png);

                return path;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the app data folder path within the ToastNotificationManagerCompat folder, used for storing icon assets and any additional data.
        /// </summary>
        /// <returns>Returns a string of the absolute folder path.</returns>
        public static string GetAppDataFolderPath(string aumid)
        {
            string conciseAumid = aumid.Contains('\\') || aumid.Contains('/') ? GenerateGuid(aumid) : aumid;

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ToastNotificationManagerCompat", "Apps", conciseAumid);
        }

        // From https://stackoverflow.com/a/9291151
        private static Bitmap GetAlphaBitmapFromBitmapData(BitmapData bmpData)
        {
            return new Bitmap(
                    bmpData.Width,
                    bmpData.Height,
                    bmpData.Stride,
                    PixelFormat.Format32bppArgb,
                    bmpData.Scan0);
        }

        // From https://stackoverflow.com/a/9291151
        private static bool IsAlphaBitmap(Bitmap bmp, out BitmapData bmpData)
        {
            var bmBounds = new Rectangle(0, 0, bmp.Width, bmp.Height);

            bmpData = bmp.LockBits(bmBounds, ImageLockMode.ReadOnly, bmp.PixelFormat);

            try
            {
                for (int y = 0; y <= bmpData.Height - 1; y++)
                {
                    for (int x = 0; x <= bmpData.Width - 1; x++)
                    {
                        var pixelColor = Color.FromArgb(
                            Marshal.ReadInt32(bmpData.Scan0, (bmpData.Stride * y) + (4 * x)));

                        if (pixelColor.A > 0 & pixelColor.A < 255)
                        {
                            return true;
                        }
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }

            return false;
        }

        /// <summary>
        /// From https://stackoverflow.com/a/41622689/1454643
        /// Generates Guid based on String. Key assumption for this algorithm is that name is unique (across where it it's being used)
        /// and if name byte length is less than 16 - it will be fetched directly into guid, if over 16 bytes - then we compute sha-1
        /// hash from string and then pass it to guid.
        /// </summary>
        /// <param name="name">Unique name which is unique across where this guid will be used.</param>
        /// <returns>For example "706C7567-696E-7300-0000-000000000000" for "plugins"</returns>
        public static string GenerateGuid(string name)
        {
            byte[] buf = Encoding.UTF8.GetBytes(name);
            byte[] guid = new byte[16];
            if (buf.Length < 16)
            {
                Array.Copy(buf, guid, buf.Length);
            }
            else
            {
                using (SHA1 sha1 = SHA1.Create())
                {
                    byte[] hash = sha1.ComputeHash(buf);

                    // Hash is 20 bytes, but we need 16. We loose some of "uniqueness", but I doubt it will be fatal
                    Array.Copy(hash, guid, 16);
                }
            }

            // Don't use Guid constructor, it tends to swap bytes. We want to preserve original string as hex dump.
            string guidS = $"{guid[0]:X2}{guid[1]:X2}{guid[2]:X2}{guid[3]:X2}-{guid[4]:X2}{guid[5]:X2}-{guid[6]:X2}{guid[7]:X2}-{guid[8]:X2}{guid[9]:X2}-{guid[10]:X2}{guid[11]:X2}{guid[12]:X2}{guid[13]:X2}{guid[14]:X2}{guid[15]:X2}";

            return guidS;
        }
    }
}

#endif