// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Panel that allows for a List/Details pattern.
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.ItemsControl" />
    public partial class ListDetailsView
    {
        /// <summary>
        /// Identifies the <see cref="SelectedItem"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="SelectedItem"/> dependency property.</returns>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            nameof(SelectedItem),
            typeof(object),
            typeof(ListDetailsView),
            new PropertyMetadata(null, OnSelectedItemChanged));

        /// <summary>
        /// Identifies the <see cref="DetailsTemplate"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="DetailsTemplate"/> dependency property.</returns>
        public static readonly DependencyProperty DetailsTemplateProperty = DependencyProperty.Register(
            nameof(DetailsTemplate),
            typeof(DataTemplate),
            typeof(ListDetailsView),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ListPaneBackground"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ListPaneBackground"/> dependency property.</returns>
        public static readonly DependencyProperty ListPaneBackgroundProperty = DependencyProperty.Register(
            nameof(ListPaneBackground),
            typeof(Brush),
            typeof(ListDetailsView),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ListHeader"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ListHeader"/> dependency property.</returns>
        public static readonly DependencyProperty ListHeaderProperty = DependencyProperty.Register(
            nameof(ListHeader),
            typeof(object),
            typeof(ListDetailsView),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ListHeaderTemplate"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ListHeaderTemplate"/> dependency property.</returns>
        public static readonly DependencyProperty ListHeaderTemplateProperty = DependencyProperty.Register(
            nameof(ListHeaderTemplate),
            typeof(DataTemplate),
            typeof(ListDetailsView),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DetailsHeader"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="DetailsHeader"/> dependency property.</returns>
        public static readonly DependencyProperty DetailsHeaderProperty = DependencyProperty.Register(
            nameof(DetailsHeader),
            typeof(object),
            typeof(ListDetailsView),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DetailsHeaderTemplate"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="DetailsHeaderTemplate"/> dependency property.</returns>
        public static readonly DependencyProperty DetailsHeaderTemplateProperty = DependencyProperty.Register(
            nameof(DetailsHeaderTemplate),
            typeof(DataTemplate),
            typeof(ListDetailsView),
            new PropertyMetadata(null));


        /// <summary>
        /// Identifies the <see cref="DetailsPaneBackground"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="DetailsPaneBackground"/> dependency property.</returns>
        public static readonly DependencyProperty DetailsPaneBackgroundProperty = DependencyProperty.Register(
            nameof(DetailsPaneBackground),
            typeof(Brush),
            typeof(ListDetailsView),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ListPaneNoItemsContent"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ListPaneNoItemsContent"/> dependency property.</returns>
        public static readonly DependencyProperty ListPaneNoItemsContentProperty = DependencyProperty.Register(
            nameof(ListPaneNoItemsContent),
            typeof(object),
            typeof(ListDetailsView),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ListPaneNoItemsContentTemplate"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ListPaneNoItemsContentTemplate"/> dependency property.</returns>
        public static readonly DependencyProperty ListPaneNoItemsContentTemplateProperty = DependencyProperty.Register(
            nameof(ListPaneNoItemsContentTemplate),
            typeof(DataTemplate),
            typeof(ListDetailsView),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ListPaneWidth"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ListPaneWidth"/> dependency property.</returns>
        public static readonly DependencyProperty ListPaneWidthProperty = DependencyProperty.Register(
            nameof(ListPaneWidth),
            typeof(GridLength),
            typeof(ListDetailsView),
            new PropertyMetadata(new GridLength(320)));

        /// <summary>
        /// Identifies the <see cref="NoSelectionContent"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="NoSelectionContent"/> dependency property.</returns>
        public static readonly DependencyProperty NoSelectionContentProperty = DependencyProperty.Register(
            nameof(NoSelectionContent),
            typeof(object),
            typeof(ListDetailsView),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="NoSelectionContentTemplate"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="NoSelectionContentTemplate"/> dependency property.</returns>
        public static readonly DependencyProperty NoSelectionContentTemplateProperty = DependencyProperty.Register(
            nameof(NoSelectionContentTemplate),
            typeof(DataTemplate),
            typeof(ListDetailsView),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ViewState"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ViewState"/> dependency property.</returns>
        public static readonly DependencyProperty ViewStateProperty = DependencyProperty.Register(
            nameof(ViewState),
            typeof(ListDetailsViewState),
            typeof(ListDetailsView),
            new PropertyMetadata(default(ListDetailsViewState)));

        /// <summary>
        /// Identifies the <see cref="ListPaneCommandBar"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ListPaneCommandBar"/> dependency property.</returns>
        public static readonly DependencyProperty ListPaneCommandBarProperty = DependencyProperty.Register(
            nameof(ListPaneCommandBar),
            typeof(CommandBar),
            typeof(ListDetailsView),
            new PropertyMetadata(null, OnListPaneCommandBarChanged));

        /// <summary>
        /// Identifies the <see cref="DetailsPaneCommandBar"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="DetailsPaneCommandBar"/> dependency property.</returns>
        public static readonly DependencyProperty DetailsPaneCommandBarProperty = DependencyProperty.Register(
            nameof(DetailsPaneCommandBar),
            typeof(CommandBar),
            typeof(ListDetailsView),
            new PropertyMetadata(null, OnDetailsPaneCommandBarChanged));

        /// <summary>
        /// Identifies the <see cref="CompactModeThresholdWidth"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="CompactModeThresholdWidth"/> dependency property.</returns>
        public static readonly DependencyProperty CompactModeThresholdWidthProperty = DependencyProperty.Register(
            nameof(CompactModeThresholdWidth),
            typeof(double),
            typeof(ListDetailsView),
            new PropertyMetadata(640d));

        /// <summary>
        /// Identifies the <see cref="BackButtonBehavior"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="BackButtonBehavior"/> dependency property.</returns>
        public static readonly DependencyProperty BackButtonBehaviorProperty = DependencyProperty.Register(
            nameof(BackButtonBehavior),
            typeof(BackButtonBehavior),
            typeof(ListDetailsView),
            new PropertyMetadata(null, OnBackButtonBehaviorChanged));

        /// <summary>
        /// Identifies the <see cref="DetailsContentTemplateSelector"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="DetailsContentTemplateSelector"/> dependency property.</returns>
        public static readonly DependencyProperty DetailsContentTemplateSelectorProperty = DependencyProperty.Register(
            nameof(DetailsContentTemplateSelector),
            typeof(DataTemplateSelector),
            typeof(ListDetailsView),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ListPaneItemTemplateSelector"/> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="ListPaneItemTemplateSelector"/> dependency property.</returns>
        public static readonly DependencyProperty ListPaneItemTemplateSelectorProperty = DependencyProperty.Register(
            nameof(ListPaneItemTemplateSelector),
            typeof(DataTemplateSelector),
            typeof(ListDetailsView),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <returns>The selected item. The default is null.</returns>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets the DataTemplate used to display the details.
        /// </summary>
        public DataTemplate DetailsTemplate
        {
            get { return (DataTemplate)GetValue(DetailsTemplateProperty); }
            set { SetValue(DetailsTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Brush to apply to the background of the list area of the control.
        /// </summary>
        /// <returns>The Brush to apply to the background of the list area of the control.</returns>
        public Brush ListPaneBackground
        {
            get { return (Brush)GetValue(ListPaneBackgroundProperty); }
            set { SetValue(ListPaneBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Brush to apply to the background of the details area of the control.
        /// </summary>
        /// <returns>The Brush to apply to the background of the details area of the control.</returns>
        public Brush DetailsPaneBackground
        {
            get { return (Brush)GetValue(DetailsPaneBackgroundProperty); }
            set { SetValue(DetailsPaneBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content for the list pane's no items presenter.
        /// </summary>
        /// <returns>
        /// The content of the list pane's header. The default is null.
        /// </returns>
        public object ListPaneNoItemsContent
        {
            get { return GetValue(ListPaneNoItemsContentProperty); }
            set { SetValue(ListPaneNoItemsContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the DataTemplate used to display the list pane's no items presenter.
        /// </summary>
        /// <returns>
        /// The template that specifies the visualization of the list pane no items object. The default is null.
        /// </returns>
        public DataTemplate ListPaneNoItemsContentTemplate
        {
            get { return (DataTemplate)GetValue(ListPaneNoItemsContentTemplateProperty); }
            set { SetValue(ListPaneNoItemsContentTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content for the list pane's header.
        /// </summary>
        /// <returns>
        /// The content of the list pane's header. The default is null.
        /// </returns>
        public object ListHeader
        {
            get { return GetValue(ListHeaderProperty); }
            set { SetValue(ListHeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the DataTemplate used to display the content of the list pane's header.
        /// </summary>
        /// <returns>
        /// The template that specifies the visualization of the list pane header object. The default is null.
        /// </returns>
        public DataTemplate ListHeaderTemplate
        {
            get { return (DataTemplate)GetValue(ListHeaderTemplateProperty); }
            set { SetValue(ListHeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content for the details pane's header
        /// </summary>
        /// <returns>
        /// The content of the details pane's header. The default is null.
        /// </returns>
        public object DetailsHeader
        {
            get { return GetValue(DetailsHeaderProperty); }
            set { SetValue(DetailsHeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the DataTemplate used to display the content of the details pane's header.
        /// </summary>
        /// <returns>
        /// The template that specifies the visualization of the details pane header object. The default is null.
        /// </returns>
        public DataTemplate DetailsHeaderTemplate
        {
            get { return (DataTemplate)GetValue(DetailsHeaderTemplateProperty); }
            set { SetValue(DetailsHeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the list pane when the view is expanded.
        /// </summary>
        /// <returns>
        /// The width of the SplitView pane when it's fully expanded. The default is 320
        /// device-independent pixel (DIP).
        /// </returns>
        public GridLength ListPaneWidth
        {
            get { return (GridLength)GetValue(ListPaneWidthProperty); }
            set { SetValue(ListPaneWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content to display when there is no item selected in the list pane.
        /// </summary>
        public object NoSelectionContent
        {
            get { return GetValue(NoSelectionContentProperty); }
            set { SetValue(NoSelectionContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the DataTemplate used to display the content when there is no selection.
        /// </summary>
        /// <returns>
        /// The template that specifies the visualization of the content when there is no
        /// selection. The default is null.
        /// </returns>
        public DataTemplate NoSelectionContentTemplate
        {
            get { return (DataTemplate)GetValue(NoSelectionContentTemplateProperty); }
            set { SetValue(NoSelectionContentTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets gets the current visual state of the control.
        /// </summary>
        public ListDetailsViewState ViewState
        {
            get { return (ListDetailsViewState)GetValue(ViewStateProperty); }
            set { SetValue(ViewStateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="CommandBar"/> for the list pane.
        /// </summary>
        public CommandBar ListPaneCommandBar
        {
            get { return (CommandBar)GetValue(ListPaneCommandBarProperty); }
            set { SetValue(ListPaneCommandBarProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="CommandBar"/> for the details pane.
        /// </summary>
        public CommandBar DetailsPaneCommandBar
        {
            get { return (CommandBar)GetValue(DetailsPaneCommandBarProperty); }
            set { SetValue(DetailsPaneCommandBarProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Threshold width that will trigger the control to go into compact mode.
        /// </summary>
        public double CompactModeThresholdWidth
        {
            get { return (double)GetValue(CompactModeThresholdWidthProperty); }
            set { SetValue(CompactModeThresholdWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the behavior to use for the back button.
        /// </summary>
        /// <returns>The current BackButtonBehavior. The default is System.</returns>
        public BackButtonBehavior BackButtonBehavior
        {
            get { return (BackButtonBehavior)GetValue(BackButtonBehaviorProperty); }
            set { SetValue(BackButtonBehaviorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplateSelector"/> for the details presenter.
        /// </summary>
        public DataTemplateSelector DetailsContentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(DetailsContentTemplateSelectorProperty); }
            set { SetValue(DetailsContentTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplateSelector"/> for the list pane items.
        /// </summary>
        public DataTemplateSelector ListPaneItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ListPaneItemTemplateSelectorProperty); }
            set { SetValue(ListPaneItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Gets or sets a function for mapping the selected item to a different model.
        /// This new model will be the DataContext of the Details area.
        /// </summary>
        public Func<object, object> MapDetails { get; set; }

        private static void OnDetailsPaneCommandBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ListDetailsView)d).OnDetailsPaneCommandBarChanged();
        }

        private static void OnListPaneCommandBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ListDetailsView)d).OnListPaneCommandBarChanged();
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ListDetailsView)d).OnSelectedItemChanged(e);
        }

        private static void OnBackButtonBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ListDetailsView)d).SetBackButtonVisibility();
        }
    }
}
