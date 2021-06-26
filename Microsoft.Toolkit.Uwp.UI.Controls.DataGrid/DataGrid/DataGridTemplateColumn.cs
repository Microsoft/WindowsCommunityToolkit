// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI.Controls.DataGridInternals;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Represents a <see cref="DataGrid"/> column that hosts template-specified
    /// content in its cells.
    /// </summary>
    public class DataGridTemplateColumn : DataGridColumn
    {
        private DataTemplate _cellTemplate;
        private DataTemplate _cellEditingTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridTemplateColumn"/> class.
        /// </summary>
        public DataGridTemplateColumn()
        {
        }

        /// <summary>
        /// Gets or sets the template that is used to display the contents of a cell that is in editing mode.
        /// </summary>
        public DataTemplate CellEditingTemplate
        {
            get
            {
                return _cellEditingTemplate;
            }

            set
            {
                if (_cellEditingTemplate != value)
                {
                    this.RemoveEditingElement();
                    _cellEditingTemplate = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the template that is used to display the contents of a cell that is not in editing mode.
        /// </summary>
        public DataTemplate CellTemplate
        {
            get
            {
                return _cellTemplate;
            }

            set
            {
                if (_cellTemplate != value)
                {
                    if (_cellEditingTemplate == null)
                    {
                        this.RemoveEditingElement();
                    }

                    _cellTemplate = value;
                }
            }
        }

        internal bool HasDistinctTemplates
        {
            get
            {
                return this.CellTemplate != this.CellEditingTemplate;
            }
        }

        /// <summary>
        /// CancelCellEdit
        /// </summary>
        /// <param name="editingElement">The element that the column displays for a cell in editing mode.</param>
        /// <param name="uneditedValue">The previous, unedited value in the cell being edited.</param>
        protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
        {
            _ = GenerateEditingElement(null, null);
        }

        /// <summary>
        /// Gets an element defined by the <see cref="P:Microsoft.Toolkit.Uwp.UI.Controls.DataGridTemplateColumn.CellEditingTemplate"/> that is bound to the column's <see cref="P:Microsoft.Toolkit.Uwp.UI.Controls.DataGridBoundColumn.Binding"/> property value.
        /// </summary>
        /// <returns>A new editing element that is bound to the column's <see cref="P:Microsoft.Toolkit.Uwp.UI.Controls.DataGridBoundColumn.Binding"/> property value.</returns>
        /// <param name="cell">The cell that will contain the generated element.</param>
        /// <param name="dataItem">The data item represented by the row that contains the intended cell.</param>
        /// <exception cref="T:System.TypeInitializationException">
        /// The <see cref="P:Microsoft.Toolkit.Uwp.UI.Controls.DataGridTemplateColumn.CellEditingTemplate"/> is null.
        /// </exception>
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            if (this.CellEditingTemplate != null)
            {
                return this.CellEditingTemplate.LoadContent() as FrameworkElement;
            }

            if (this.CellTemplate != null)
            {
                return this.CellTemplate.LoadContent() as FrameworkElement;
            }

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return null;
            }
            else
            {
                throw DataGridError.DataGridTemplateColumn.MissingTemplateForType(typeof(DataGridTemplateColumn));
            }
        }

        /// <summary>
        /// Gets an element defined by the <see cref="P:Microsoft.Toolkit.Uwp.UI.Controls.DataGridTemplateColumn.CellTemplate"/> that is bound to the column's <see cref="P:Microsoft.Toolkit.Uwp.UI.Controls.DataGridBoundColumn.Binding"/> property value.
        /// </summary>
        /// <returns>A new, read-only element that is bound to the column's <see cref="P:Microsoft.Toolkit.Uwp.UI.Controls.DataGridBoundColumn.Binding"/> property value.</returns>
        /// <param name="cell">The cell that will contain the generated element.</param>
        /// <param name="dataItem">The data item represented by the row that contains the intended cell.</param>
        /// <exception cref="T:System.TypeInitializationException">
        /// The <see cref="P:Microsoft.Toolkit.Uwp.UI.Controls.DataGridTemplateColumn.CellTemplate"/> is null.
        /// </exception>
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            if (this.CellTemplate != null)
            {
                return this.CellTemplate.LoadContent() as FrameworkElement;
            }

            if (this.CellEditingTemplate != null)
            {
                return this.CellEditingTemplate.LoadContent() as FrameworkElement;
            }

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return null;
            }
            else
            {
                throw DataGridError.DataGridTemplateColumn.MissingTemplateForType(typeof(DataGridTemplateColumn));
            }
        }

        /// <summary>
        /// Called when a cell in the column enters editing mode.
        /// </summary>
        /// <param name="editingElement">The element that the column displays for a cell in editing mode.</param>
        /// <param name="editingEventArgs">Information about the user gesture that is causing a cell to enter editing mode.</param>
        /// <returns>null in all cases.</returns>
        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            return null;
        }
    }
}