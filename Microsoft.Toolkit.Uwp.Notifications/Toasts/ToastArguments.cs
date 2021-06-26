// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Toolkit.Uwp.Notifications
{
    /// <summary>
    /// A class that supports serializing simple key/value pairs into a format that's friendly for being used within toast notifications. The serialized format is similar to a query string, however optimized for being placed within an XML property (uses semicolons instead of ampersands since those don't need to be XML-escaped, doesn't url-encode all special characters since not being used within a URL, etc).
    /// </summary>
    public sealed class ToastArguments : IEnumerable<KeyValuePair<string, string>>
    {
        private Dictionary<string, string> _dictionary = new Dictionary<string, string>();

        internal ToastArguments Clone()
        {
            return new ToastArguments()
            {
                _dictionary = new Dictionary<string, string>(_dictionary)
            };
        }

#if !WINRT
        /// <summary>
        /// Gets the value of the specified key. Throws <see cref="KeyNotFoundException"/> if the key could not be found.
        /// </summary>
        /// <param name="key">The key to find.</param>
        /// <returns>The value of the specified key.</returns>
        public string this[string key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (TryGetValue(key, out string value))
                {
                    return value;
                }

                throw new KeyNotFoundException($"A key with name '{key}' could not be found.");
            }

            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                _dictionary[key] = value;
            }
        }
#endif

        /// <summary>
        /// Attempts to get the value of the specified key. If no key exists, returns false.
        /// </summary>
        /// <param name="key">The key to find.</param>
        /// <param name="value">The key's value will be written here if found.</param>
        /// <returns>True if found the key and set the value, otherwise false.</returns>
#if WINRT
        [return: System.Runtime.InteropServices.WindowsRuntime.ReturnValueName("found")]
#endif
        public bool TryGetValue(string key, out string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _dictionary.TryGetValue(key, out value);
        }

#if !WINRT
        /// <summary>
        /// Attempts to get the value of the specified key. If no key exists, returns false.
        /// </summary>
        /// <typeparam name="T">The enum to parse.</typeparam>
        /// <param name="key">The key to find.</param>
        /// <param name="value">The key's value will be written here if found.</param>
        /// <returns>True if found the key and set the value, otherwise false.</returns>
        public bool TryGetValue<T>(string key, out T value)
            where T : struct, Enum
        {
            if (TryGetValue(key, out string strValue))
            {
                return Enum.TryParse(strValue, out value);
            }

            value = default(T);
            return false;
        }
#endif

        /// <summary>
        /// Gets the value of the specified key, or throws <see cref="KeyNotFoundException"/> if key didn't exist.
        /// </summary>
        /// <param name="key">The key to get.</param>
        /// <returns>The value of the key.</returns>
        public string Get(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_dictionary.TryGetValue(key, out string value))
            {
                return value;
            }

            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Gets the value of the specified key, or throws <see cref="KeyNotFoundException"/> if key didn't exist.
        /// </summary>
        /// <param name="key">The key to get.</param>
        /// <returns>The value of the key.</returns>
        public int GetInt(string key)
        {
            return int.Parse(Get(key));
        }

        /// <summary>
        /// Gets the value of the specified key, or throws <see cref="KeyNotFoundException"/> if key didn't exist.
        /// </summary>
        /// <param name="key">The key to get.</param>
        /// <returns>The value of the key.</returns>
        public double GetDouble(string key)
        {
            return double.Parse(Get(key));
        }

        /// <summary>
        /// Gets the value of the specified key, or throws <see cref="KeyNotFoundException"/> if key didn't exist.
        /// </summary>
        /// <param name="key">The key to get.</param>
        /// <returns>The value of the key.</returns>
        public float GetFloat(string key)
        {
            return float.Parse(Get(key));
        }

        /// <summary>
        /// Gets the value of the specified key, or throws <see cref="KeyNotFoundException"/> if key didn't exist.
        /// </summary>
        /// <param name="key">The key to get.</param>
        /// <returns>The value of the key.</returns>
        public byte GetByte(string key)
        {
            return byte.Parse(Get(key));
        }

        /// <summary>
        /// Gets the value of the specified key, or throws <see cref="KeyNotFoundException"/> if key didn't exist.
        /// </summary>
        /// <param name="key">The key to get.</param>
        /// <returns>The value of the key.</returns>
        public bool GetBool(string key)
        {
            return Get(key) == "1" ? true : false;
        }

#if !WINRT
        /// <summary>
        /// Gets the value of the specified key, or throws <see cref="KeyNotFoundException"/> if key didn't exist.
        /// </summary>
        /// <typeparam name="T">The enum to parse.</typeparam>
        /// <param name="key">The key to get.</param>
        /// <returns>The value of the key.</returns>
        public T GetEnum<T>(string key)
            where T : struct, Enum
        {
            if (TryGetValue(key, out T value))
            {
                return value;
            }

            throw new KeyNotFoundException();
        }
#endif

        /// <summary>
        /// Gets the number of key/value pairs contained in the toast arguments.
        /// </summary>
        public int Count => _dictionary.Count;

        /// <summary>
        /// Adds a key. If there is an existing key, it is replaced.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The current <see cref="ToastArguments"/> object.</returns>
#if WINRT
        [return: System.Runtime.InteropServices.WindowsRuntime.ReturnValueName("toastArguments")]
#endif
        public ToastArguments Add(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _dictionary[key] = null;

            return this;
        }

        /// <summary>
        /// Adds a key and optional value. If there is an existing key, it is replaced.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The optional value of the key.</param>
        /// <returns>The current <see cref="ToastArguments"/> object.</returns>
#if WINRT
        [Windows.Foundation.Metadata.DefaultOverload]
        [return: System.Runtime.InteropServices.WindowsRuntime.ReturnValueName("toastArguments")]
#endif
        public ToastArguments Add(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _dictionary[key] = value;

            return this;
        }

        /// <summary>
        /// Adds a key and value. If there is an existing key, it is replaced.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value of the key.</param>
        /// <returns>The current <see cref="ToastArguments"/> object.</returns>
#if WINRT
        [return: System.Runtime.InteropServices.WindowsRuntime.ReturnValueName("toastArguments")]
#endif
        public ToastArguments Add(string key, int value)
        {
            return AddHelper(key, value);
        }

        /// <summary>
        /// Adds a key and value. If there is an existing key, it is replaced.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value of the key.</param>
        /// <returns>The current <see cref="ToastArguments"/> object.</returns>
#if WINRT
        [return: System.Runtime.InteropServices.WindowsRuntime.ReturnValueName("toastArguments")]
#endif
        public ToastArguments Add(string key, double value)
        {
            return AddHelper(key, value);
        }

        /// <summary>
        /// Adds a key and value. If there is an existing key, it is replaced.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value of the key.</param>
        /// <returns>The current <see cref="ToastArguments"/> object.</returns>
#if WINRT
        [return: System.Runtime.InteropServices.WindowsRuntime.ReturnValueName("toastArguments")]
#endif
        public ToastArguments Add(string key, float value)
        {
            return AddHelper(key, value);
        }

        /// <summary>
        /// Adds a key and value. If there is an existing key, it is replaced.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value of the key.</param>
        /// <returns>The current <see cref="ToastArguments"/> object.</returns>
#if WINRT
        [return: System.Runtime.InteropServices.WindowsRuntime.ReturnValueName("toastArguments")]
#endif
        public ToastArguments Add(string key, bool value)
        {
            return Add(key, value ? "1" : "0"); // Encode as 1 or 0 to save string space
        }

#if !WINRT
        /// <summary>
        /// Adds a key and value. If there is an existing key, it is replaced.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value of the key. Note that the enums are stored using their numeric value, so be aware that changing your enum number values might break existing activation of toasts currently in Action Center.</param>
        /// <returns>The current <see cref="ToastArguments"/> object.</returns>
        public ToastArguments Add(string key, Enum value)
        {
            return Add(key, (int)(object)value);
        }
#endif

        private ToastArguments AddHelper(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _dictionary[key] = value.ToString();

            return this;
        }

        /// <summary>
        /// Determines if the specified key is present.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>True if the key is present, otherwise false.</returns>
        public bool Contains(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Determines if specified key and value are present.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <param name="value">The value to look for when the key has been matched.</param>
        /// <returns>True if the key and value were found, else false.</returns>
#if WINRT
        [return: System.Runtime.InteropServices.WindowsRuntime.ReturnValueName("doesContain")]
#endif
        public bool Contains(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _dictionary.TryGetValue(key, out string actualValue) && actualValue == value;
        }

        /// <summary>
        /// Removes the specified key and its associated value.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns>True if the key was removed, else false.</returns>
        public bool Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _dictionary.Remove(key);
        }

        private static string Encode(string str)
        {
            return str
                .Replace("%", "%25")
                .Replace(";", "%3B")
                .Replace("=", "%3D");
        }

        private static string Decode(string str)
        {
            return str
                .Replace("%25", "%")
                .Replace("%3B", ";")
                .Replace("%3D", "=");
        }

        /// <summary>
        /// Parses a string that was generated using ToastArguments into a <see cref="ToastArguments"/> object.
        /// </summary>
        /// <param name="toastArgumentsStr">The toast arguments string to deserialize.</param>
        /// <returns>The parsed toast arguments.</returns>
        public static ToastArguments Parse(string toastArgumentsStr)
        {
            if (string.IsNullOrWhiteSpace(toastArgumentsStr))
            {
                return new ToastArguments();
            }

            string[] pairs = toastArgumentsStr.Split(';');

            ToastArguments answer = new ToastArguments();

            foreach (string pair in pairs)
            {
                string name;
                string value;

                int indexOfEquals = pair.IndexOf('=');

                if (indexOfEquals == -1)
                {
                    name = Decode(pair);
                    value = null;
                }
                else
                {
                    name = Decode(pair.Substring(0, indexOfEquals));
                    value = Decode(pair.Substring(indexOfEquals + 1));
                }

                answer.Add(name, value);
            }

            return answer;
        }

        /// <summary>
        /// Serializes the key-value pairs into a string that can be used within a toast notification.
        /// </summary>
        /// <returns>A string that can be used within a toast notification.</returns>
        public sealed override string ToString()
        {
            return string.Join(Separator, this.Select(pair => EncodePair(pair.Key, pair.Value)));
        }

        internal static string EncodePair(string key, string value)
        {
            // Key
            return Encode(key) +

                // Write value if not null
                ((value == null) ? string.Empty : ("=" + Encode(value)));
        }

        internal const string Separator = ";";

        /// <summary>
        /// Gets an enumerator to enumerate the arguments. Note that order of the arguments is NOT preserved.
        /// </summary>
        /// <returns>An enumeartor of the key/value pairs.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator to enumerate the query string parameters.
        /// </summary>
        /// <returns>An enumeartor of the key/value pairs.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}