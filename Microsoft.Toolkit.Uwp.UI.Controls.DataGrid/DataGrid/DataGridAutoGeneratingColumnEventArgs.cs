// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Provides data for the <see cref="E:Microsoft.Toolkit.Uwp.UI.Controls.DataGrid.AutoGeneratingColumn" /> event.
    /// </summary>
    public class DataGridAutoGeneratingColumnEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridAutoGeneratingColumnEventArgs"/> class.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property bound to the generated column.
        /// </param>
        /// <param name="propertyType">
        /// The <see cref="T:System.Type" /> of the property bound to the generated column.
        /// </param>
        /// <param name="column">
        /// The generated column.
        /// </param>
        public DataGridAutoGeneratingColumnEventArgs(string propertyName, Type propertyType, DataGridColumn column)
        {
            this.Column = column;
            this.PropertyName = propertyName;
            this.PropertyType = propertyType;
        }

        /// <summary>
        /// Gets or sets the generated column.
        /// </summary>
        public DataGridColumn Column
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the property bound to the generated column.
        /// </summary>
        public string PropertyName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref="T:System.Type"/> of the property bound to the generated column.
        /// </summary>
        public Type PropertyType
        {
            get;
            private set;
        }
    }
}