// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WIN32

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Windows.UI.Notifications;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// Text and selection values that the user entered on your notification. The Key is the ID of the input, and the Value is what the user entered.
    /// </summary>
    public class NotificationUserInput : IReadOnlyDictionary<string, string>
    {
#pragma warning disable CS0618 // Type or member is obsolete
        private NotificationActivator.NOTIFICATION_USER_INPUT_DATA[] _data;
#pragma warning restore CS0618 // Type or member is obsolete

#pragma warning disable CS0618 // Type or member is obsolete
        internal NotificationUserInput(NotificationActivator.NOTIFICATION_USER_INPUT_DATA[] data)
#pragma warning restore CS0618 // Type or member is obsolete
        {
            _data = data;
        }

        /// <summary>
        /// Gets the value of an input with the given key.
        /// </summary>
        /// <param name="key">The key of the input.</param>
        /// <returns>The value of the input.</returns>
        public string this[string key] => _data.First(i => i.Key == key).Value;

        /// <summary>
        /// Gets all the keys of the inputs.
        /// </summary>
        public IEnumerable<string> Keys => _data.Select(i => i.Key);

        /// <summary>
        /// Gets all the values of the inputs.
        /// </summary>
        public IEnumerable<string> Values => _data.Select(i => i.Value);

        /// <summary>
        /// Gets how many inputs there were.
        /// </summary>
        public int Count => _data.Length;

        /// <summary>
        /// Checks whether any inputs have the given key.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>A boolean representing whether any inputs have the given key.</returns>
        public bool ContainsKey(string key)
        {
            return _data.Any(i => i.Key == key);
        }

        /// <summary>
        /// Gets an enumerator of the inputs.
        /// </summary>
        /// <returns>An enumerator of the inputs.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _data.Select(i => new KeyValuePair<string, string>(i.Key, i.Value)).GetEnumerator();
        }

        /// <summary>
        /// Tries to get the input value for the given key.
        /// </summary>
        /// <param name="key">The key of the input to look for.</param>
        /// <param name="value">The value of the input.</param>
        /// <returns>True if found an input with the specified key, else false.</returns>
        public bool TryGetValue(string key, out string value)
        {
            foreach (var item in _data)
            {
                if (item.Key == key)
                {
                    value = item.Value;
                    return true;
                }
            }

            value = null;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

#endif