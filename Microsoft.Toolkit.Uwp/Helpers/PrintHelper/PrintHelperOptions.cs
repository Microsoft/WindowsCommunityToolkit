// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.Graphics.Printing;

namespace Microsoft.Toolkit.Uwp.Helpers
{
    /// <summary>
    /// Represents options used in a print task.
    /// </summary>
    public class PrintHelperOptions
    {
        /// <summary>
        /// Gets or sets the bordering option for the print task.
        /// </summary>
        public PrintBordering Bordering { get; set; }

        /// <summary>
        /// Gets or sets the media type option for the print task.
        /// </summary>
        public PrintMediaType MediaType { get; set; }

        /// <summary>
        /// Gets or sets the media size option of the print task.
        /// </summary>
        public PrintMediaSize MediaSize { get; set; }

        /// <summary>
        /// Gets or sets the hole punch option of the print task.
        /// </summary>
        public PrintHolePunch HolePunch { get; set; }

        /// <summary>
        /// Gets or sets the binding option for the print task.
        /// </summary>
        public PrintBinding Binding { get; set; }

        /// <summary>
        /// Gets or sets the duplex option of the print task.
        /// </summary>
        public PrintDuplex Duplex { get; set; }

        /// <summary>
        /// Gets or sets the color mode option of the print task.
        /// </summary>
        public PrintColorMode ColorMode { get; set; }

        /// <summary>
        /// Gets or sets the collation option of the print tasks.
        /// </summary>
        public PrintCollation Collation { get; set; }

        /// <summary>
        /// Gets or sets the print quality option for the print task.
        /// </summary>
        public PrintQuality PrintQuality { get; set; }

        /// <summary>
        /// Gets or sets the staple option for the print task.
        /// </summary>
        public PrintStaple Staple { get; set; }

        /// <summary>
        /// Gets or sets the orientation option for the print task.
        /// </summary>
        public PrintOrientation Orientation { get; set; }

        /// <summary>
        /// Gets the options that will be displayed in the printing dialog.
        /// </summary>
        public IList<string> DisplayedOptions { get; private set; }

        /// <summary>
        /// The possible display options.
        /// </summary>
        private IEnumerable<string> _possibleDisplayOptions;

        /// <summary>
        /// Gets or sets a value indicating whether the default displayed options should be kept.
        /// Defaults to <c>true</c>.
        /// </summary>
        public bool ExtendDisplayedOptions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintHelperOptions"/> class.
        /// </summary>
        /// <param name="extendDisplayedOptions">Boolean used to set up the <see cref="ExtendDisplayedOptions"/> property</param>
        public PrintHelperOptions(bool extendDisplayedOptions = true)
        {
            ExtendDisplayedOptions = extendDisplayedOptions;
            DisplayedOptions = new List<string>();

            InitializePossibleDisplayOptions();
        }

        /// <summary>
        /// Adds a display option.
        /// </summary>
        /// <param name="displayOption">Display option to add. Must be a part of the <see cref="StandardPrintTaskOptions"/> class</param>
        public void AddDisplayOption(string displayOption)
        {
            if (!_possibleDisplayOptions.Contains(displayOption))
            {
                throw new ArgumentException("The display option must be a part of the StandardPrintTaskOptions class");
            }

            if (DisplayedOptions.Contains(displayOption))
            {
                return;
            }

            DisplayedOptions.Add(displayOption);
        }

        /// <summary>
        /// Removes a display option.
        /// </summary>
        /// <param name="displayOption">Display option to add. Must be a part of the <see cref="StandardPrintTaskOptions"/> class</param>
        public void RemoveDisplayOption(string displayOption)
        {
            if (!_possibleDisplayOptions.Contains(displayOption))
            {
                throw new ArgumentException("The display option must be a part of the StandardPrintTaskOptions class");
            }

            if (!DisplayedOptions.Contains(displayOption))
            {
                return;
            }

            DisplayedOptions.Remove(displayOption);
        }

        private void InitializePossibleDisplayOptions()
        {
            _possibleDisplayOptions = typeof(StandardPrintTaskOptions).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                                                      .Where(p => p.PropertyType == typeof(string))
                                                                      .Select(p => (string)p.GetValue(null));
        }
    }
}