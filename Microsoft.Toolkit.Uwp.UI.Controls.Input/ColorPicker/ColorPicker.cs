// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Controls.ColorPickerConverters;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using ColorPickerSlider = Microsoft.Toolkit.Uwp.UI.Controls.Primitives.ColorPickerSlider;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Presents a color spectrum, a palette of colors, and color channel sliders for user selection of a color.
    /// </summary>
    [TemplatePart(Name = nameof(ColorPicker.AlphaChannelSlider),          Type = typeof(ColorPickerSlider))]
    [TemplatePart(Name = nameof(ColorPicker.AlphaChannelTextBox),         Type = typeof(TextBox))]
    [TemplatePart(Name = nameof(ColorPicker.Channel1Slider),              Type = typeof(ColorPickerSlider))]
    [TemplatePart(Name = nameof(ColorPicker.Channel1TextBox),             Type = typeof(TextBox))]
    [TemplatePart(Name = nameof(ColorPicker.Channel2Slider),              Type = typeof(ColorPickerSlider))]
    [TemplatePart(Name = nameof(ColorPicker.Channel2TextBox),             Type = typeof(TextBox))]
    [TemplatePart(Name = nameof(ColorPicker.Channel3Slider),              Type = typeof(ColorPickerSlider))]
    [TemplatePart(Name = nameof(ColorPicker.Channel3TextBox),             Type = typeof(TextBox))]
    [TemplatePart(Name = nameof(ColorPicker.CheckeredBackground1Border),  Type = typeof(Border))]
    [TemplatePart(Name = nameof(ColorPicker.CheckeredBackground2Border),  Type = typeof(Border))]
    [TemplatePart(Name = nameof(ColorPicker.CheckeredBackground3Border),  Type = typeof(Border))]
    [TemplatePart(Name = nameof(ColorPicker.CheckeredBackground4Border),  Type = typeof(Border))]
    [TemplatePart(Name = nameof(ColorPicker.CheckeredBackground5Border),  Type = typeof(Border))]
    [TemplatePart(Name = nameof(ColorPicker.CheckeredBackground6Border),  Type = typeof(Border))]
    [TemplatePart(Name = nameof(ColorPicker.CheckeredBackground7Border),  Type = typeof(Border))]
    [TemplatePart(Name = nameof(ColorPicker.CheckeredBackground8Border),  Type = typeof(Border))]
    [TemplatePart(Name = nameof(ColorPicker.CheckeredBackground9Border),  Type = typeof(Border))]
    [TemplatePart(Name = nameof(ColorPicker.CheckeredBackground10Border), Type = typeof(Border))]
    [TemplatePart(Name = nameof(ColorPicker.ColorSpectrumControl),        Type = typeof(ColorSpectrum))]
    [TemplatePart(Name = nameof(ColorPicker.ColorSpectrumAlphaSlider),    Type = typeof(ColorPickerSlider))]
    [TemplatePart(Name = nameof(ColorPicker.ColorSpectrumThirdDimensionSlider), Type = typeof(ColorPickerSlider))]
    [TemplatePart(Name = nameof(ColorPicker.HexInputTextBox),             Type = typeof(TextBox))]
    [TemplatePart(Name = nameof(ColorPicker.HsvToggleButton),             Type = typeof(ToggleButton))]
    [TemplatePart(Name = nameof(ColorPicker.RgbToggleButton),             Type = typeof(ToggleButton))]
    [TemplatePart(Name = nameof(ColorPicker.P1PreviewBorder),             Type = typeof(Border))]
    [TemplatePart(Name = nameof(ColorPicker.P2PreviewBorder),             Type = typeof(Border))]
    [TemplatePart(Name = nameof(ColorPicker.N1PreviewBorder),             Type = typeof(Border))]
    [TemplatePart(Name = nameof(ColorPicker.N2PreviewBorder),             Type = typeof(Border))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1501:Statement should not be on a single line", Justification = "Inline brackets are used to improve code readability with repeated null checks.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1025:Code should not contain multiple whitespace in a row", Justification = "Whitespace is used to align code in columns for readability.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:Field names should begin with lower-case letter", Justification = "Only template parts start with a capital letter. This differentiates them from other fields.")]
    public partial class ColorPicker : Windows.UI.Xaml.Controls.ColorPicker
    {
        internal Color CheckerBackgroundColor { get; set; } = Color.FromArgb(0x19, 0x80, 0x80, 0x80); // Overridden later

        /// <summary>
        /// The period that scheduled color updates will be applied.
        /// This is only used when updating colors using the ScheduleColorUpdate() method.
        /// Color changes made directly to the Color property will apply instantly.
        /// </summary>
        private const int ColorUpdateInterval = 30; // Milliseconds

        private long tokenColor;
        private long tokenCustomPalette;
        private long tokenIsColorPaletteVisible;

        private bool callbacksConnected = false;
        private bool eventsConnected    = false;
        private bool isInitialized      = false;

        // Color information for updates
        private HsvColor?       savedHsvColor              = null;
        private Color?          savedHsvColorRgbEquivalent = null;
        private Color?          updatedRgbColor            = null;
        private DispatcherQueueTimer dispatcherQueueTimer            = null;

        private ColorSpectrum     ColorSpectrumControl;
        private ColorPickerSlider ColorSpectrumAlphaSlider;
        private ColorPickerSlider ColorSpectrumThirdDimensionSlider;
        private TextBox           HexInputTextBox;
        private ToggleButton      HsvToggleButton;
        private ToggleButton      RgbToggleButton;

        private TextBox           Channel1TextBox;
        private TextBox           Channel2TextBox;
        private TextBox           Channel3TextBox;
        private TextBox           AlphaChannelTextBox;
        private ColorPickerSlider Channel1Slider;
        private ColorPickerSlider Channel2Slider;
        private ColorPickerSlider Channel3Slider;
        private ColorPickerSlider AlphaChannelSlider;

        private Border N1PreviewBorder;
        private Border N2PreviewBorder;
        private Border P1PreviewBorder;
        private Border P2PreviewBorder;

        // Up to 10 checkered backgrounds may be used by name anywhere in the template
        private Border CheckeredBackground1Border;
        private Border CheckeredBackground2Border;
        private Border CheckeredBackground3Border;
        private Border CheckeredBackground4Border;
        private Border CheckeredBackground5Border;
        private Border CheckeredBackground6Border;
        private Border CheckeredBackground7Border;
        private Border CheckeredBackground8Border;
        private Border CheckeredBackground9Border;
        private Border CheckeredBackground10Border;

        /***************************************************************************************
         *
         * Constructor/Destructor
         *
         ***************************************************************************************/

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorPicker"/> class.
        /// </summary>
        public ColorPicker()
        {
            this.DefaultStyleKey = typeof(ColorPicker);

            // Setup collections
            this.SetValue(CustomPaletteColorsProperty, new ObservableCollection<Color>());
            this.CustomPaletteColors.CollectionChanged += CustomPaletteColors_CollectionChanged;

            this.Loaded += ColorPickerButton_Loaded;

            // Checkered background color is found only one time for performance
            // This may need to change in the future if theme changes should be supported
            this.CheckerBackgroundColor = (Color)Application.Current.Resources["SystemListLowColor"];

            this.ConnectCallbacks(true);
            this.SetDefaultPalette();
            this.StartDispatcherQueueTimer();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ColorPicker"/> class.
        /// </summary>
        ~ColorPicker()
        {
            this.StopDispatcherQueueTimer();
            this.CustomPaletteColors.CollectionChanged -= CustomPaletteColors_CollectionChanged;
        }

        /***************************************************************************************
         *
         * Methods
         *
         ***************************************************************************************/

        /// <summary>
        /// Gets whether or not the color is considered empty (all fields zero).
        /// In the future Color.IsEmpty will hopefully be added to UWP.
        /// </summary>
        /// <param name="color">The Windows.UI.Color to calculate with.</param>
        /// <returns>Whether the color is considered empty.</returns>
        private static bool IsColorEmpty(Color color)
        {
            return color.A == 0x00 &&
                   color.R == 0x00 &&
                   color.G == 0x00 &&
                   color.B == 0x00;
        }

        /// <summary>
        /// Overrides when a template is applied in order to get the required controls.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            // We need to disconnect old events first
            this.ConnectEvents(false);

            this.ColorSpectrumControl              = this.GetTemplateChild<ColorSpectrum>(nameof(ColorSpectrumControl));
            this.ColorSpectrumAlphaSlider          = this.GetTemplateChild<ColorPickerSlider>(nameof(ColorSpectrumAlphaSlider));
            this.ColorSpectrumThirdDimensionSlider = this.GetTemplateChild<ColorPickerSlider>(nameof(ColorSpectrumThirdDimensionSlider));

            this.HexInputTextBox = this.GetTemplateChild<TextBox>(nameof(HexInputTextBox));
            this.HsvToggleButton = this.GetTemplateChild<ToggleButton>(nameof(HsvToggleButton));
            this.RgbToggleButton = this.GetTemplateChild<ToggleButton>(nameof(RgbToggleButton));

            this.Channel1TextBox     = this.GetTemplateChild<TextBox>(nameof(Channel1TextBox));
            this.Channel2TextBox     = this.GetTemplateChild<TextBox>(nameof(Channel2TextBox));
            this.Channel3TextBox     = this.GetTemplateChild<TextBox>(nameof(Channel3TextBox));
            this.AlphaChannelTextBox = this.GetTemplateChild<TextBox>(nameof(AlphaChannelTextBox));

            this.Channel1Slider     = this.GetTemplateChild<ColorPickerSlider>(nameof(Channel1Slider));
            this.Channel2Slider     = this.GetTemplateChild<ColorPickerSlider>(nameof(Channel2Slider));
            this.Channel3Slider     = this.GetTemplateChild<ColorPickerSlider>(nameof(Channel3Slider));
            this.AlphaChannelSlider = this.GetTemplateChild<ColorPickerSlider>(nameof(AlphaChannelSlider));

            this.N1PreviewBorder = this.GetTemplateChild<Border>(nameof(N1PreviewBorder));
            this.N2PreviewBorder = this.GetTemplateChild<Border>(nameof(N2PreviewBorder));
            this.P1PreviewBorder = this.GetTemplateChild<Border>(nameof(P1PreviewBorder));
            this.P2PreviewBorder = this.GetTemplateChild<Border>(nameof(P2PreviewBorder));

            this.CheckeredBackground1Border  = this.GetTemplateChild<Border>(nameof(CheckeredBackground1Border));
            this.CheckeredBackground2Border  = this.GetTemplateChild<Border>(nameof(CheckeredBackground2Border));
            this.CheckeredBackground3Border  = this.GetTemplateChild<Border>(nameof(CheckeredBackground3Border));
            this.CheckeredBackground4Border  = this.GetTemplateChild<Border>(nameof(CheckeredBackground4Border));
            this.CheckeredBackground5Border  = this.GetTemplateChild<Border>(nameof(CheckeredBackground5Border));
            this.CheckeredBackground6Border  = this.GetTemplateChild<Border>(nameof(CheckeredBackground6Border));
            this.CheckeredBackground7Border  = this.GetTemplateChild<Border>(nameof(CheckeredBackground7Border));
            this.CheckeredBackground8Border  = this.GetTemplateChild<Border>(nameof(CheckeredBackground8Border));
            this.CheckeredBackground9Border  = this.GetTemplateChild<Border>(nameof(CheckeredBackground9Border));
            this.CheckeredBackground10Border = this.GetTemplateChild<Border>(nameof(CheckeredBackground10Border));

            // Must connect after controls are resolved
            this.ConnectEvents(true);

            base.OnApplyTemplate();
            this.UpdateVisualState(false);
            this.isInitialized = true;
            this.SetActiveColorRepresentation(ColorRepresentation.Rgba);
            this.UpdateColorControlValues(); // TODO: This will also connect events after, can we optimize vs. doing it twice with the ConnectEvents above?
        }

        /// <summary>
        /// Retrieves the named element in the instantiated ControlTemplate visual tree.
        /// </summary>
        /// <param name="childName">The name of the element to find.</param>
        /// <param name="isRequired">Whether the element is required and will throw an exception if missing.</param>
        /// <returns>The template child matching the given name and type.</returns>
        private T GetTemplateChild<T>(string childName, bool isRequired = false)
            where T : DependencyObject
        {
            T child = this.GetTemplateChild(childName) as T;
            if ((child == null) && isRequired)
            {
                ThrowArgumentNullException();
            }

            return child;

            static void ThrowArgumentNullException() => throw new ArgumentNullException(nameof(childName));
        }

        /// <summary>
        /// Connects or disconnects all dependency property callbacks.
        /// </summary>
        /// <param name="connected">True to connect callbacks, otherwise false.</param>
        private void ConnectCallbacks(bool connected)
        {
            if ((connected == true) &&
                (this.callbacksConnected == false))
            {
                // Add callbacks for dependency properties
                this.tokenColor                 = this.RegisterPropertyChangedCallback(ColorProperty,                 OnColorChanged);
                this.tokenCustomPalette         = this.RegisterPropertyChangedCallback(CustomPaletteProperty,         OnCustomPaletteChanged);
                this.tokenIsColorPaletteVisible = this.RegisterPropertyChangedCallback(IsColorPaletteVisibleProperty, OnIsColorPaletteVisibleChanged);

                this.callbacksConnected = true;
            }
            else if ((connected == false) &&
                     (this.callbacksConnected == true))
            {
                // Remove callbacks for dependency properties
                this.UnregisterPropertyChangedCallback(ColorProperty,                 this.tokenColor);
                this.UnregisterPropertyChangedCallback(CustomPaletteProperty,         this.tokenCustomPalette);
                this.UnregisterPropertyChangedCallback(IsColorPaletteVisibleProperty, this.tokenIsColorPaletteVisible);

                this.callbacksConnected = false;
            }

            return;
        }

        /// <summary>
        /// Connects or disconnects all control event handlers.
        /// </summary>
        /// <param name="connected">True to connect event handlers, otherwise false.</param>
        private void ConnectEvents(bool connected)
        {
            if ((connected == true) &&
                (this.eventsConnected == false))
            {
                // Add all events
                if (this.ColorSpectrumControl != null) { this.ColorSpectrumControl.ColorChanged += ColorSpectrum_ColorChanged; }
                if (this.ColorSpectrumControl != null) { this.ColorSpectrumControl.GotFocus     += ColorSpectrum_GotFocus; }
                if (this.HexInputTextBox      != null) { this.HexInputTextBox.KeyDown           += HexInputTextBox_KeyDown; }
                if (this.HexInputTextBox      != null) { this.HexInputTextBox.LostFocus         += HexInputTextBox_LostFocus; }
                if (this.HsvToggleButton      != null) { this.HsvToggleButton.Checked           += ColorRepToggleButton_CheckedUnchecked; }
                if (this.HsvToggleButton      != null) { this.HsvToggleButton.Unchecked         += ColorRepToggleButton_CheckedUnchecked; }
                if (this.RgbToggleButton      != null) { this.RgbToggleButton.Checked           += ColorRepToggleButton_CheckedUnchecked; }
                if (this.RgbToggleButton      != null) { this.RgbToggleButton.Unchecked         += ColorRepToggleButton_CheckedUnchecked; }

                if (this.Channel1TextBox     != null) { this.Channel1TextBox.KeyDown       += ChannelTextBox_KeyDown; }
                if (this.Channel2TextBox     != null) { this.Channel2TextBox.KeyDown       += ChannelTextBox_KeyDown; }
                if (this.Channel3TextBox     != null) { this.Channel3TextBox.KeyDown       += ChannelTextBox_KeyDown; }
                if (this.AlphaChannelTextBox != null) { this.AlphaChannelTextBox.KeyDown   += ChannelTextBox_KeyDown; }
                if (this.Channel1TextBox     != null) { this.Channel1TextBox.LostFocus     += ChannelTextBox_LostFocus; }
                if (this.Channel2TextBox     != null) { this.Channel2TextBox.LostFocus     += ChannelTextBox_LostFocus; }
                if (this.Channel3TextBox     != null) { this.Channel3TextBox.LostFocus     += ChannelTextBox_LostFocus; }
                if (this.AlphaChannelTextBox != null) { this.AlphaChannelTextBox.LostFocus += ChannelTextBox_LostFocus; }

                if (this.Channel1Slider                    != null) { this.Channel1Slider.ValueChanged                    += ChannelSlider_ValueChanged; }
                if (this.Channel2Slider                    != null) { this.Channel2Slider.ValueChanged                    += ChannelSlider_ValueChanged; }
                if (this.Channel3Slider                    != null) { this.Channel3Slider.ValueChanged                    += ChannelSlider_ValueChanged; }
                if (this.AlphaChannelSlider                != null) { this.AlphaChannelSlider.ValueChanged                += ChannelSlider_ValueChanged; }
                if (this.ColorSpectrumAlphaSlider          != null) { this.ColorSpectrumAlphaSlider.ValueChanged          += ChannelSlider_ValueChanged; }
                if (this.ColorSpectrumThirdDimensionSlider != null) { this.ColorSpectrumThirdDimensionSlider.ValueChanged += ChannelSlider_ValueChanged; }

                if (this.Channel1Slider                    != null) { this.Channel1Slider.Loaded                    += ChannelSlider_Loaded; }
                if (this.Channel2Slider                    != null) { this.Channel2Slider.Loaded                    += ChannelSlider_Loaded; }
                if (this.Channel3Slider                    != null) { this.Channel3Slider.Loaded                    += ChannelSlider_Loaded; }
                if (this.AlphaChannelSlider                != null) { this.AlphaChannelSlider.Loaded                += ChannelSlider_Loaded; }
                if (this.ColorSpectrumAlphaSlider          != null) { this.ColorSpectrumAlphaSlider.Loaded          += ChannelSlider_Loaded; }
                if (this.ColorSpectrumThirdDimensionSlider != null) { this.ColorSpectrumThirdDimensionSlider.Loaded += ChannelSlider_Loaded; }

                if (this.N1PreviewBorder != null) { this.N1PreviewBorder.PointerPressed += PreviewBorder_PointerPressed; }
                if (this.N2PreviewBorder != null) { this.N2PreviewBorder.PointerPressed += PreviewBorder_PointerPressed; }
                if (this.P1PreviewBorder != null) { this.P1PreviewBorder.PointerPressed += PreviewBorder_PointerPressed; }
                if (this.P2PreviewBorder != null) { this.P2PreviewBorder.PointerPressed += PreviewBorder_PointerPressed; }

                if (this.CheckeredBackground1Border  != null) { this.CheckeredBackground1Border.Loaded  += CheckeredBackgroundBorder_Loaded; }
                if (this.CheckeredBackground2Border  != null) { this.CheckeredBackground2Border.Loaded  += CheckeredBackgroundBorder_Loaded; }
                if (this.CheckeredBackground3Border  != null) { this.CheckeredBackground3Border.Loaded  += CheckeredBackgroundBorder_Loaded; }
                if (this.CheckeredBackground4Border  != null) { this.CheckeredBackground4Border.Loaded  += CheckeredBackgroundBorder_Loaded; }
                if (this.CheckeredBackground5Border  != null) { this.CheckeredBackground5Border.Loaded  += CheckeredBackgroundBorder_Loaded; }
                if (this.CheckeredBackground6Border  != null) { this.CheckeredBackground6Border.Loaded  += CheckeredBackgroundBorder_Loaded; }
                if (this.CheckeredBackground7Border  != null) { this.CheckeredBackground7Border.Loaded  += CheckeredBackgroundBorder_Loaded; }
                if (this.CheckeredBackground8Border  != null) { this.CheckeredBackground8Border.Loaded  += CheckeredBackgroundBorder_Loaded; }
                if (this.CheckeredBackground9Border  != null) { this.CheckeredBackground9Border.Loaded  += CheckeredBackgroundBorder_Loaded; }
                if (this.CheckeredBackground10Border != null) { this.CheckeredBackground10Border.Loaded += CheckeredBackgroundBorder_Loaded; }

                this.eventsConnected = true;
            }
            else if ((connected == false) &&
                     (this.eventsConnected == true))
            {
                // Remove all events
                if (this.ColorSpectrumControl != null) { this.ColorSpectrumControl.ColorChanged -= ColorSpectrum_ColorChanged; }
                if (this.ColorSpectrumControl != null) { this.ColorSpectrumControl.GotFocus     -= ColorSpectrum_GotFocus; }
                if (this.HexInputTextBox      != null) { this.HexInputTextBox.KeyDown           -= HexInputTextBox_KeyDown; }
                if (this.HexInputTextBox      != null) { this.HexInputTextBox.LostFocus         -= HexInputTextBox_LostFocus; }
                if (this.HsvToggleButton      != null) { this.HsvToggleButton.Checked           -= ColorRepToggleButton_CheckedUnchecked; }
                if (this.HsvToggleButton      != null) { this.HsvToggleButton.Unchecked         -= ColorRepToggleButton_CheckedUnchecked; }
                if (this.RgbToggleButton      != null) { this.RgbToggleButton.Checked           -= ColorRepToggleButton_CheckedUnchecked; }
                if (this.RgbToggleButton      != null) { this.RgbToggleButton.Unchecked         -= ColorRepToggleButton_CheckedUnchecked; }

                if (this.Channel1TextBox     != null) { this.Channel1TextBox.KeyDown       -= ChannelTextBox_KeyDown; }
                if (this.Channel2TextBox     != null) { this.Channel2TextBox.KeyDown       -= ChannelTextBox_KeyDown; }
                if (this.Channel3TextBox     != null) { this.Channel3TextBox.KeyDown       -= ChannelTextBox_KeyDown; }
                if (this.AlphaChannelTextBox != null) { this.AlphaChannelTextBox.KeyDown   -= ChannelTextBox_KeyDown; }
                if (this.Channel1TextBox     != null) { this.Channel1TextBox.LostFocus     -= ChannelTextBox_LostFocus; }
                if (this.Channel2TextBox     != null) { this.Channel2TextBox.LostFocus     -= ChannelTextBox_LostFocus; }
                if (this.Channel3TextBox     != null) { this.Channel3TextBox.LostFocus     -= ChannelTextBox_LostFocus; }
                if (this.AlphaChannelTextBox != null) { this.AlphaChannelTextBox.LostFocus -= ChannelTextBox_LostFocus; }

                if (this.Channel1Slider                    != null) { this.Channel1Slider.ValueChanged                    -= ChannelSlider_ValueChanged; }
                if (this.Channel2Slider                    != null) { this.Channel2Slider.ValueChanged                    -= ChannelSlider_ValueChanged; }
                if (this.Channel3Slider                    != null) { this.Channel3Slider.ValueChanged                    -= ChannelSlider_ValueChanged; }
                if (this.AlphaChannelSlider                != null) { this.AlphaChannelSlider.ValueChanged                -= ChannelSlider_ValueChanged; }
                if (this.ColorSpectrumAlphaSlider          != null) { this.ColorSpectrumAlphaSlider.ValueChanged          -= ChannelSlider_ValueChanged; }
                if (this.ColorSpectrumThirdDimensionSlider != null) { this.ColorSpectrumThirdDimensionSlider.ValueChanged -= ChannelSlider_ValueChanged; }

                if (this.Channel1Slider                    != null) { this.Channel1Slider.Loaded                    -= ChannelSlider_Loaded; }
                if (this.Channel2Slider                    != null) { this.Channel2Slider.Loaded                    -= ChannelSlider_Loaded; }
                if (this.Channel3Slider                    != null) { this.Channel3Slider.Loaded                    -= ChannelSlider_Loaded; }
                if (this.AlphaChannelSlider                != null) { this.AlphaChannelSlider.Loaded                -= ChannelSlider_Loaded; }
                if (this.ColorSpectrumAlphaSlider          != null) { this.ColorSpectrumAlphaSlider.Loaded          -= ChannelSlider_Loaded; }
                if (this.ColorSpectrumThirdDimensionSlider != null) { this.ColorSpectrumThirdDimensionSlider.Loaded -= ChannelSlider_Loaded; }

                if (this.N1PreviewBorder != null) { this.N1PreviewBorder.PointerPressed -= PreviewBorder_PointerPressed; }
                if (this.N2PreviewBorder != null) { this.N2PreviewBorder.PointerPressed -= PreviewBorder_PointerPressed; }
                if (this.P1PreviewBorder != null) { this.P1PreviewBorder.PointerPressed -= PreviewBorder_PointerPressed; }
                if (this.P2PreviewBorder != null) { this.P2PreviewBorder.PointerPressed -= PreviewBorder_PointerPressed; }

                if (this.CheckeredBackground1Border  != null) { this.CheckeredBackground1Border.Loaded  -= CheckeredBackgroundBorder_Loaded; }
                if (this.CheckeredBackground2Border  != null) { this.CheckeredBackground2Border.Loaded  -= CheckeredBackgroundBorder_Loaded; }
                if (this.CheckeredBackground3Border  != null) { this.CheckeredBackground3Border.Loaded  -= CheckeredBackgroundBorder_Loaded; }
                if (this.CheckeredBackground4Border  != null) { this.CheckeredBackground4Border.Loaded  -= CheckeredBackgroundBorder_Loaded; }
                if (this.CheckeredBackground5Border  != null) { this.CheckeredBackground5Border.Loaded  -= CheckeredBackgroundBorder_Loaded; }
                if (this.CheckeredBackground6Border  != null) { this.CheckeredBackground6Border.Loaded  -= CheckeredBackgroundBorder_Loaded; }
                if (this.CheckeredBackground7Border  != null) { this.CheckeredBackground7Border.Loaded  -= CheckeredBackgroundBorder_Loaded; }
                if (this.CheckeredBackground8Border  != null) { this.CheckeredBackground8Border.Loaded  -= CheckeredBackgroundBorder_Loaded; }
                if (this.CheckeredBackground9Border  != null) { this.CheckeredBackground9Border.Loaded  -= CheckeredBackgroundBorder_Loaded; }
                if (this.CheckeredBackground10Border != null) { this.CheckeredBackground10Border.Loaded -= CheckeredBackgroundBorder_Loaded; }

                this.eventsConnected = false;
            }

            return;
        }

        /// <summary>
        /// Updates all visual states based on current control properties.
        /// </summary>
        /// <param name="useTransitions">Whether transitions should occur when changing states.</param>
        private void UpdateVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, this.IsEnabled ? "Normal" : "Disabled", useTransitions);
            VisualStateManager.GoToState(this, this.GetActiveColorRepresentation() == ColorRepresentation.Hsva ? "HsvSelected" : "RgbSelected", useTransitions);
            VisualStateManager.GoToState(this, this.IsColorPaletteVisible ? "ColorPaletteVisible" : "ColorPaletteCollapsed", useTransitions);

            return;
        }

        /// <summary>
        /// Gets the active representation of the color: HSV or RGB.
        /// </summary>
        private ColorRepresentation GetActiveColorRepresentation()
        {
            // If the HSV representation control is missing for whatever reason,
            // the default will be RGB
            if (this.HsvToggleButton != null &&
                this.HsvToggleButton.IsChecked == true)
            {
                return ColorRepresentation.Hsva;
            }

            return ColorRepresentation.Rgba;
        }

        /// <summary>
        /// Sets the active color representation in the UI controls.
        /// </summary>
        /// <param name="colorRepresentation">The color representation to set.
        /// Setting to null (the default) will attempt to keep the current state.</param>
        private void SetActiveColorRepresentation(ColorRepresentation? colorRepresentation = null)
        {
            bool eventsDisconnectedByMethod = false;

            if (colorRepresentation == null)
            {
                // Use the control's current value
                colorRepresentation = this.GetActiveColorRepresentation();
            }

            // Disable events during the update
            if (this.eventsConnected)
            {
                this.ConnectEvents(false);
                eventsDisconnectedByMethod = true;
            }

            // Sync the UI controls and visual state
            // The default is always RGBA
            if (colorRepresentation == ColorRepresentation.Hsva)
            {
                if (this.RgbToggleButton != null &&
                    (bool)this.RgbToggleButton.IsChecked)
                {
                    this.RgbToggleButton.IsChecked = false;
                }

                if (this.HsvToggleButton != null &&
                    (bool)this.HsvToggleButton.IsChecked == false)
                {
                    this.HsvToggleButton.IsChecked = true;
                }
            }
            else
            {
                if (this.RgbToggleButton != null &&
                    (bool)this.RgbToggleButton.IsChecked == false)
                {
                    this.RgbToggleButton.IsChecked = true;
                }

                if (this.HsvToggleButton != null &&
                    (bool)this.HsvToggleButton.IsChecked)
                {
                    this.HsvToggleButton.IsChecked = false;
                }
            }

            this.UpdateVisualState(false);

            if (eventsDisconnectedByMethod)
            {
                this.ConnectEvents(true);
            }

            return;
        }

        /// <summary>
        /// Gets the active third dimension in the color spectrum: Hue, Saturation or Value.
        /// </summary>
        private ColorChannel GetActiveColorSpectrumThirdDimension()
        {
            switch (this.ColorSpectrumComponents)
            {
                case Windows.UI.Xaml.Controls.ColorSpectrumComponents.SaturationValue:
                case Windows.UI.Xaml.Controls.ColorSpectrumComponents.ValueSaturation:
                    {
                        // Hue
                        return ColorChannel.Channel1;
                    }

                case Windows.UI.Xaml.Controls.ColorSpectrumComponents.HueValue:
                case Windows.UI.Xaml.Controls.ColorSpectrumComponents.ValueHue:
                    {
                        // Saturation
                        return ColorChannel.Channel2;
                    }

                case Windows.UI.Xaml.Controls.ColorSpectrumComponents.HueSaturation:
                case Windows.UI.Xaml.Controls.ColorSpectrumComponents.SaturationHue:
                    {
                        // Value
                        return ColorChannel.Channel3;
                    }
            }

            return ColorChannel.Alpha; // Error, should never get here
        }

        /// <summary>
        /// Declares a new color to set to the control.
        /// Application of this color will be scheduled to avoid overly rapid updates.
        /// </summary>
        /// <param name="newColor">The new color to set to the control. </param>
        private void ScheduleColorUpdate(Color newColor)
        {
            // Coerce the value as needed
            if (this.IsAlphaEnabled == false)
            {
                newColor = new Color()
                {
                    R = newColor.R,
                    G = newColor.G,
                    B = newColor.B,
                    A = 255
                };
            }

            this.updatedRgbColor = newColor;

            return;
        }

        /// <summary>
        /// Applies the value of the given color channel TextBox to the current color.
        /// </summary>
        /// <param name="channelTextBox">The color channel TextBox to apply the value from.</param>
        private void ApplyChannelTextBoxValue(TextBox channelTextBox)
        {
            double channelValue;

            if (channelTextBox != null)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(channelTextBox.Text))
                    {
                        // An empty string is allowed and happens when the clear TextBox button is pressed
                        // This case should be interpreted as zero
                        channelValue = 0;
                    }
                    else
                    {
                        channelValue = double.Parse(channelTextBox.Text, CultureInfo.CurrentUICulture);
                    }

                    if (object.ReferenceEquals(channelTextBox, this.Channel1TextBox))
                    {
                        this.SetColorChannel(this.GetActiveColorRepresentation(), ColorChannel.Channel1, channelValue);
                    }
                    else if (object.ReferenceEquals(channelTextBox, this.Channel2TextBox))
                    {
                        this.SetColorChannel(this.GetActiveColorRepresentation(), ColorChannel.Channel2, channelValue);
                    }
                    else if (object.ReferenceEquals(channelTextBox, this.Channel3TextBox))
                    {
                        this.SetColorChannel(this.GetActiveColorRepresentation(), ColorChannel.Channel3, channelValue);
                    }
                    else if (object.ReferenceEquals(channelTextBox, this.AlphaChannelTextBox))
                    {
                        this.SetColorChannel(this.GetActiveColorRepresentation(), ColorChannel.Alpha, channelValue);
                    }
                }
                catch
                {
                    // Reset TextBox values
                    this.UpdateColorControlValues();
                    this.UpdateChannelSliderBackgrounds();
                }
            }

            return;
        }

        /// <summary>
        /// Updates the color values in all editing controls to match the current color.
        /// </summary>
        private void UpdateColorControlValues()
        {
            bool eventsDisconnectedByMethod = false;
            Color rgbColor = this.Color;
            HsvColor hsvColor;

            if (this.isInitialized)
            {
                // Disable events during the update
                if (this.eventsConnected)
                {
                    this.ConnectEvents(false);
                    eventsDisconnectedByMethod = true;
                }

                if (this.HexInputTextBox != null)
                {
                    if (this.IsAlphaEnabled)
                    {
                        // Remove only the "#" sign
                        this.HexInputTextBox.Text = rgbColor.ToHex().Replace("#", string.Empty);
                    }
                    else
                    {
                        // Remove the "#" sign and alpha hex
                        this.HexInputTextBox.Text = rgbColor.ToHex().Replace("#", string.Empty).Substring(2);
                    }
                }

                // Regardless of the active color representation, the spectrum is always HSV
                // Therefore, always calculate HSV color here
                // Warning: Always maintain/use HSV information in the saved HSV color
                // This avoids loss of precision and drift caused by continuously converting to/from RGB
                if (this.savedHsvColor == null)
                {
                    hsvColor = rgbColor.ToHsv();

                    // Round the channels, be sure rounding matches with the scaling next
                    // Rounding of SVA requires at MINIMUM 2 decimal places
                    int decimals = 0;
                    hsvColor = new HsvColor()
                    {
                        H = Math.Round(hsvColor.H, decimals),
                        S = Math.Round(hsvColor.S, 2 + decimals),
                        V = Math.Round(hsvColor.V, 2 + decimals),
                        A = Math.Round(hsvColor.A, 2 + decimals)
                    };

                    // Must update HSV color
                    this.savedHsvColor              = hsvColor;
                    this.savedHsvColorRgbEquivalent = rgbColor;
                }
                else
                {
                    hsvColor = this.savedHsvColor.Value;
                }

                // Update the color spectrum
                // Remember the spectrum is always HSV and must be updated as such to avoid
                // conversion errors
                if (this.ColorSpectrumControl != null)
                {
                    this.ColorSpectrumControl.HsvColor = new System.Numerics.Vector4()
                    {
                        X = Convert.ToSingle(hsvColor.H),
                        Y = Convert.ToSingle(hsvColor.S),
                        Z = Convert.ToSingle(hsvColor.V),
                        W = Convert.ToSingle(hsvColor.A)
                    };
                }

                // Update the color spectrum third dimension channel
                if (this.ColorSpectrumThirdDimensionSlider != null)
                {
                    // Convert the channels into a usable range for the user
                    double hue         = hsvColor.H;
                    double staturation = hsvColor.S * 100;
                    double value       = hsvColor.V * 100;

                    switch (this.GetActiveColorSpectrumThirdDimension())
                    {
                        case ColorChannel.Channel1:
                            {
                                // Hue
                                this.ColorSpectrumThirdDimensionSlider.Minimum = 0;
                                this.ColorSpectrumThirdDimensionSlider.Maximum = 360;
                                this.ColorSpectrumThirdDimensionSlider.Value   = hue;
                                break;
                            }

                        case ColorChannel.Channel2:
                            {
                                // Saturation
                                this.ColorSpectrumThirdDimensionSlider.Minimum = 0;
                                this.ColorSpectrumThirdDimensionSlider.Maximum = 100;
                                this.ColorSpectrumThirdDimensionSlider.Value   = staturation;
                                break;
                            }

                        case ColorChannel.Channel3:
                            {
                                // Value
                                this.ColorSpectrumThirdDimensionSlider.Minimum = 0;
                                this.ColorSpectrumThirdDimensionSlider.Maximum = 100;
                                this.ColorSpectrumThirdDimensionSlider.Value   = value;
                                break;
                            }
                    }
                }

                // Update all other color channels
                if (this.GetActiveColorRepresentation() == ColorRepresentation.Hsva)
                {
                    // Convert the channels into a usable range for the user
                    double hue         = hsvColor.H;
                    double staturation = hsvColor.S * 100;
                    double value       = hsvColor.V * 100;
                    double alpha       = hsvColor.A * 100;

                    // Hue
                    if (this.Channel1TextBox != null)
                    {
                        this.Channel1TextBox.MaxLength = 3;
                        this.Channel1TextBox.Text = hue.ToString(CultureInfo.CurrentUICulture);
                    }

                    if (this.Channel1Slider != null)
                    {
                        this.Channel1Slider.Minimum = 0;
                        this.Channel1Slider.Maximum = 360;
                        this.Channel1Slider.Value   = hue;
                    }

                    // Saturation
                    if (this.Channel2TextBox != null)
                    {
                        this.Channel2TextBox.MaxLength = 3;
                        this.Channel2TextBox.Text = staturation.ToString(CultureInfo.CurrentUICulture);
                    }

                    if (this.Channel2Slider != null)
                    {
                        this.Channel2Slider.Minimum = 0;
                        this.Channel2Slider.Maximum = 100;
                        this.Channel2Slider.Value   = staturation;
                    }

                    // Value
                    if (this.Channel3TextBox != null)
                    {
                        this.Channel3TextBox.MaxLength = 3;
                        this.Channel3TextBox.Text = value.ToString(CultureInfo.CurrentUICulture);
                    }

                    if (this.Channel3Slider != null)
                    {
                        this.Channel3Slider.Minimum = 0;
                        this.Channel3Slider.Maximum = 100;
                        this.Channel3Slider.Value   = value;
                    }

                    // Alpha
                    if (this.AlphaChannelTextBox != null)
                    {
                        this.AlphaChannelTextBox.MaxLength = 3;
                        this.AlphaChannelTextBox.Text = alpha.ToString(CultureInfo.CurrentUICulture);
                    }

                    if (this.AlphaChannelSlider != null)
                    {
                        this.AlphaChannelSlider.Minimum = 0;
                        this.AlphaChannelSlider.Maximum = 100;
                        this.AlphaChannelSlider.Value   = alpha;
                    }

                    // Color spectrum alpha
                    if (this.ColorSpectrumAlphaSlider != null)
                    {
                        this.ColorSpectrumAlphaSlider.Minimum = 0;
                        this.ColorSpectrumAlphaSlider.Maximum = 100;
                        this.ColorSpectrumAlphaSlider.Value   = alpha;
                    }
                }
                else
                {
                    // Red
                    if (this.Channel1TextBox != null)
                    {
                        this.Channel1TextBox.MaxLength = 3;
                        this.Channel1TextBox.Text = rgbColor.R.ToString(CultureInfo.CurrentUICulture);
                    }

                    if (this.Channel1Slider != null)
                    {
                        this.Channel1Slider.Minimum = 0;
                        this.Channel1Slider.Maximum = 255;
                        this.Channel1Slider.Value   = Convert.ToDouble(rgbColor.R);
                    }

                    // Green
                    if (this.Channel2TextBox != null)
                    {
                        this.Channel2TextBox.MaxLength = 3;
                        this.Channel2TextBox.Text = rgbColor.G.ToString(CultureInfo.CurrentUICulture);
                    }

                    if (this.Channel2Slider != null)
                    {
                        this.Channel2Slider.Minimum = 0;
                        this.Channel2Slider.Maximum = 255;
                        this.Channel2Slider.Value   = Convert.ToDouble(rgbColor.G);
                    }

                    // Blue
                    if (this.Channel3TextBox != null)
                    {
                        this.Channel3TextBox.MaxLength = 3;
                        this.Channel3TextBox.Text = rgbColor.B.ToString(CultureInfo.CurrentUICulture);
                    }

                    if (this.Channel3Slider != null)
                    {
                        this.Channel3Slider.Minimum = 0;
                        this.Channel3Slider.Maximum = 255;
                        this.Channel3Slider.Value   = Convert.ToDouble(rgbColor.B);
                    }

                    // Alpha
                    if (this.AlphaChannelTextBox != null)
                    {
                        this.AlphaChannelTextBox.MaxLength = 3;
                        this.AlphaChannelTextBox.Text = rgbColor.A.ToString(CultureInfo.CurrentUICulture);
                    }

                    if (this.AlphaChannelSlider != null)
                    {
                        this.AlphaChannelSlider.Minimum = 0;
                        this.AlphaChannelSlider.Maximum = 255;
                        this.AlphaChannelSlider.Value   = Convert.ToDouble(rgbColor.A);
                    }

                    // Color spectrum alpha
                    if (this.ColorSpectrumAlphaSlider != null)
                    {
                        this.ColorSpectrumAlphaSlider.Minimum = 0;
                        this.ColorSpectrumAlphaSlider.Maximum = 255;
                        this.ColorSpectrumAlphaSlider.Value   = Convert.ToDouble(rgbColor.A);
                    }
                }

                if (eventsDisconnectedByMethod)
                {
                    this.ConnectEvents(true);
                }
            }

            return;
        }

        /// <summary>
        /// Sets a new color channel value to the current color.
        /// Only the specified color channel will be modified.
        /// </summary>
        /// <param name="colorRepresentation">The color representation of the given channel.</param>
        /// <param name="channel">The specified color channel to modify.</param>
        /// <param name="newValue">The new color channel value.</param>
        private void SetColorChannel(
            ColorRepresentation colorRepresentation,
            ColorChannel channel,
            double newValue)
        {
            Color oldRgbColor = this.Color;
            Color newRgbColor;
            HsvColor oldHsvColor;

            if (colorRepresentation == ColorRepresentation.Hsva)
            {
                // Warning: Always maintain/use HSV information in the saved HSV color
                // This avoids loss of precision and drift caused by continuously converting to/from RGB
                if (this.savedHsvColor == null)
                {
                    oldHsvColor = oldRgbColor.ToHsv();
                }
                else
                {
                    oldHsvColor = this.savedHsvColor.Value;
                }

                double hue        = oldHsvColor.H;
                double saturation = oldHsvColor.S;
                double value      = oldHsvColor.V;
                double alpha      = oldHsvColor.A;

                switch (channel)
                {
                    case ColorChannel.Channel1:
                        {
                            hue = Math.Clamp(double.IsNaN(newValue) ? 0 : newValue, 0, 360);
                            break;
                        }

                    case ColorChannel.Channel2:
                        {
                            saturation = Math.Clamp((double.IsNaN(newValue) ? 0 : newValue) / 100, 0, 1);
                            break;
                        }

                    case ColorChannel.Channel3:
                        {
                            value = Math.Clamp((double.IsNaN(newValue) ? 0 : newValue) / 100, 0, 1);
                            break;
                        }

                    case ColorChannel.Alpha:
                        {
                            // Unlike color channels, default to no transparency
                            alpha = Math.Clamp((double.IsNaN(newValue) ? 100 : newValue) / 100, 0, 1);
                            break;
                        }
                }

                newRgbColor = Uwp.Helpers.ColorHelper.FromHsv(
                    hue,
                    saturation,
                    value,
                    alpha);

                // Must update HSV color
                this.savedHsvColor = new HsvColor()
                {
                    H = hue,
                    S = saturation,
                    V = value,
                    A = alpha
                };
                this.savedHsvColorRgbEquivalent = newRgbColor;
            }
            else
            {
                byte red   = oldRgbColor.R;
                byte green = oldRgbColor.G;
                byte blue  = oldRgbColor.B;
                byte alpha = oldRgbColor.A;

                switch (channel)
                {
                    case ColorChannel.Channel1:
                        {
                            red = Convert.ToByte(Math.Clamp(double.IsNaN(newValue) ? 0 : newValue, 0, 255));
                            break;
                        }

                    case ColorChannel.Channel2:
                        {
                            green = Convert.ToByte(Math.Clamp(double.IsNaN(newValue) ? 0 : newValue, 0, 255));
                            break;
                        }

                    case ColorChannel.Channel3:
                        {
                            blue = Convert.ToByte(Math.Clamp(double.IsNaN(newValue) ? 0 : newValue, 0, 255));
                            break;
                        }

                    case ColorChannel.Alpha:
                        {
                            // Unlike color channels, default to no transparency
                            alpha = Convert.ToByte(Math.Clamp(double.IsNaN(newValue) ? 255 : newValue, 0, 255));
                            break;
                        }
                }

                newRgbColor = new Color()
                {
                    R = red,
                    G = green,
                    B = blue,
                    A = alpha
                };

                // Must clear saved HSV color
                this.savedHsvColor              = null;
                this.savedHsvColorRgbEquivalent = null;
            }

            this.ScheduleColorUpdate(newRgbColor);
            return;
        }

        /// <summary>
        /// Updates all channel slider control backgrounds.
        /// </summary>
        private void UpdateChannelSliderBackgrounds()
        {
            this.UpdateChannelSliderBackground(this.Channel1Slider);
            this.UpdateChannelSliderBackground(this.Channel2Slider);
            this.UpdateChannelSliderBackground(this.Channel3Slider);
            this.UpdateChannelSliderBackground(this.AlphaChannelSlider);
            this.UpdateChannelSliderBackground(this.ColorSpectrumAlphaSlider);
            this.UpdateChannelSliderBackground(this.ColorSpectrumThirdDimensionSlider);
            return;
        }

        /// <summary>
        /// Updates a specific channel slider control background.
        /// </summary>
        /// <param name="slider">The color channel slider to update the background for.</param>
        private void UpdateChannelSliderBackground(ColorPickerSlider slider)
        {
            if (slider != null)
            {
                // Regardless of the active color representation, the sliders always use HSV
                // Therefore, always calculate HSV color here
                // Warning: Always maintain/use HSV information in the saved HSV color
                // This avoids loss of precision and drift caused by continuously converting to/from RGB
                if (this.savedHsvColor ==  null)
                {
                    var rgbColor = this.Color;

                    // Must update HSV color
                    this.savedHsvColor = rgbColor.ToHsv();
                    this.savedHsvColorRgbEquivalent = rgbColor;
                }

                slider.IsAutoUpdatingEnabled = false;

                if (object.ReferenceEquals(slider, this.Channel1Slider))
                {
                    slider.ColorChannel = ColorChannel.Channel1;
                    slider.ColorRepresentation = this.GetActiveColorRepresentation();
                }
                else if (object.ReferenceEquals(slider, this.Channel2Slider))
                {
                    slider.ColorChannel = ColorChannel.Channel2;
                    slider.ColorRepresentation = this.GetActiveColorRepresentation();
                }
                else if (object.ReferenceEquals(slider, this.Channel3Slider))
                {
                    slider.ColorChannel = ColorChannel.Channel3;
                    slider.ColorRepresentation = this.GetActiveColorRepresentation();
                }
                else if (object.ReferenceEquals(slider, this.AlphaChannelSlider))
                {
                    slider.ColorChannel = ColorChannel.Alpha;
                    slider.ColorRepresentation = this.GetActiveColorRepresentation();
                }
                else if (object.ReferenceEquals(slider, this.ColorSpectrumAlphaSlider))
                {
                    slider.ColorChannel = ColorChannel.Alpha;
                    slider.ColorRepresentation = this.GetActiveColorRepresentation();
                }
                else if (object.ReferenceEquals(slider, this.ColorSpectrumThirdDimensionSlider))
                {
                    slider.ColorChannel = this.GetActiveColorSpectrumThirdDimension();
                    slider.ColorRepresentation = ColorRepresentation.Hsva; // Always HSV
                }

                slider.HsvColor = this.savedHsvColor.Value;
                slider.UpdateColors();
            }

            return;
        }

        /// <summary>
        /// Sets the default color palette to the control.
        /// </summary>
        private void SetDefaultPalette()
        {
            this.CustomPalette = new FluentColorPalette();

            return;
        }

        /***************************************************************************************
         *
         * Color Update Timer
         *
         ***************************************************************************************/

        private void StartDispatcherQueueTimer()
        {
            this.dispatcherQueueTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();
            this.dispatcherQueueTimer.Interval = new TimeSpan(0, 0, 0, 0, ColorUpdateInterval);
            this.dispatcherQueueTimer.Tick += DispatcherQueueTimer_Tick;
            this.dispatcherQueueTimer.Start();

            return;
        }

        private void StopDispatcherQueueTimer()
        {
            if (this.dispatcherQueueTimer != null)
            {
                this.dispatcherQueueTimer.Stop();
            }

            return;
        }

        private void DispatcherQueueTimer_Tick(object sender, object e)
        {
            if (this.updatedRgbColor != null)
            {
                var newColor = this.updatedRgbColor.Value;

                // Clear first to avoid timing issues if it takes longer than the timer interval to set the new color
                this.updatedRgbColor = null;

                // An equality check here is important
                // Without it, OnColorChanged would continuously be invoked and preserveHsvColor overwritten when not wanted
                if (object.Equals(newColor, this.GetValue(ColorProperty)) == false)
                {
                    // Disable events here so the color update isn't repeated as other controls in the UI are updated through binding.
                    // For example, the Spectrum should be bound to Color, as soon as Color is changed here the Spectrum is updated.
                    // Then however, the ColorSpectrum.ColorChanged event would fire which would schedule a new color update --
                    // with the same color. This causes several problems:
                    //   1. Layout cycle that may crash the app
                    //   2. A performance hit recalculating for no reason
                    //   3. preserveHsvColor gets overwritten unexpectedly by the ColorChanged handler
                    this.ConnectEvents(false);
                    this.SetValue(ColorProperty, newColor);
                    this.ConnectEvents(true);
                }
            }

            return;
        }

        /***************************************************************************************
         *
         * Callbacks
         *
         ***************************************************************************************/

        /// <summary>
        /// Callback for when the <see cref="Windows.UI.Xaml.Controls.ColorPicker.Color"/> dependency property value changes.
        /// </summary>
        private void OnColorChanged(DependencyObject d, DependencyProperty e)
        {
            // TODO: Coerce the value if Alpha is disabled, is this handled in the base ColorPicker?
            if ((this.savedHsvColor != null) &&
                (object.Equals(d.GetValue(e), this.savedHsvColorRgbEquivalent) == false))
            {
                // The color was updated from an unknown source
                // The RGB and HSV colors are no longer in sync so the HSV color must be cleared
                this.savedHsvColor              = null;
                this.savedHsvColorRgbEquivalent = null;
            }

            this.UpdateColorControlValues();
            this.UpdateChannelSliderBackgrounds();

            return;
        }

        /// <summary>
        /// Callback for when the <see cref="CustomPalette"/> dependency property value changes.
        /// </summary>
        private void OnCustomPaletteChanged(DependencyObject d, DependencyProperty e)
        {
            IColorPalette palette = this.CustomPalette;

            if (palette != null)
            {
                this.CustomPaletteColumnCount = palette.ColorCount;
                this.CustomPaletteColors.Clear();

                for (int shadeIndex = 0; shadeIndex < palette.ShadeCount; shadeIndex++)
                {
                    for (int colorIndex = 0; colorIndex < palette.ColorCount; colorIndex++)
                {
                        this.CustomPaletteColors.Add(palette.GetColor(colorIndex, shadeIndex));
                    }
                }
            }

            return;
        }

        /// <summary>
        /// Callback for when the <see cref="IsColorPaletteVisible"/> dependency property value changes.
        /// </summary>
        private void OnIsColorPaletteVisibleChanged(DependencyObject d, DependencyProperty e)
        {
            this.UpdateVisualState(false);
            return;
        }

        /***************************************************************************************
         *
         * Event Handling
         *
         ***************************************************************************************/

        /// <summary>
        /// Event handler for when the control has finished loaded.
        /// </summary>
        private void ColorPickerButton_Loaded(object sender, RoutedEventArgs e)
        {
            // Available but not currently used
            return;
        }

        /// <summary>
        /// Event handler for when a color channel slider is loaded.
        /// This will draw an initial background.
        /// </summary>
        private void ChannelSlider_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateChannelSliderBackground(sender as ColorPickerSlider);
            return;
        }

        /// <summary>
        /// Event handler to draw checkered backgrounds on-demand as controls are loaded.
        /// </summary>
        private async void CheckeredBackgroundBorder_Loaded(object sender, RoutedEventArgs e)
        {
            await ColorPickerRenderingHelpers.UpdateBorderBackgroundWithCheckerAsync(
                sender as Border,
                CheckerBackgroundColor);
        }

        /// <summary>
        /// Event handler for when the list of custom palette colors is changed.
        /// </summary>
        private void CustomPaletteColors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Available but not currently used
            return;
        }

        /// <summary>
        /// Event handler for when the color spectrum color is changed.
        /// This occurs when the user presses on the spectrum to select a new color.
        /// </summary>
        private void ColorSpectrum_ColorChanged(ColorSpectrum sender, Windows.UI.Xaml.Controls.ColorChangedEventArgs args)
        {
            // It is OK in this case to use the RGB representation
            this.ScheduleColorUpdate(this.ColorSpectrumControl.Color);
            return;
        }

        /// <summary>
        /// Event handler for when the color spectrum is focused.
        /// This is used only to work around some bugs that cause usability problems.
        /// </summary>
        private void ColorSpectrum_GotFocus(object sender, RoutedEventArgs e)
        {
            Color rgbColor = this.ColorSpectrumControl.Color;

            /* If this control has a color that is currently empty (#00000000),
             * selecting a new color directly in the spectrum will fail. This is
             * a bug in the color spectrum. Selecting a new color in the spectrum will
             * keep zero for all channels (including alpha and the third dimension).
             *
             * In practice this means a new color cannot be selected using the spectrum
             * until both the alpha and third dimension slider are raised above zero.
             * This is extremely user unfriendly and must be corrected as best as possible.
             *
             * In order to work around this, detect when the color spectrum has selected
             * a new color and then automatically set the alpha and third dimension
             * channel to maximum. However, the color spectrum has a second bug, the
             * ColorChanged event is never raised if the color is empty. This prevents
             * automatically setting the other channels where it normally should be done
             * (in the ColorChanged event).
             *
             * In order to work around this second bug, the GotFocus event is used
             * to detect when the spectrum is engaged by the user. It's somewhat equivalent
             * to ColorChanged for this purpose. Then when the GotFocus event is fired
             * set the alpha and third channel values to maximum. The problem here is that
             * the GotFocus event does not have access to the new color that was selected
             * in the spectrum. It is not available due to the afore mentioned bug or due to
             * timing. This means the best that can be done is to just set a 'neutral'
             * color such as white.
             *
             * There is still a small usability issue with this as it requires two
             * presses to set a color. That's far better than having to slide up both
             * sliders though.
             *
             *  1. If the color is empty, the first press on the spectrum will set white
             *     and ignore the pressed color on the spectrum
             *  2. The second press on the spectrum will be correctly handled.
             *
             */

            // In the future Color.IsEmpty will hopefully be added to UWP
            if (IsColorEmpty(rgbColor))
            {
                /* The following code may be used in the future if ever the selected color is available

                Color newColor = this.ColorSpectrum.Color;
                HsvColor newHsvColor = newColor.ToHsv();

                switch (this.GetActiveColorSpectrumThirdDimension())
                {
                    case ColorChannel.Channel1:
                        {
                            newColor = Microsoft.Toolkit.Uwp.Helpers.ColorHelper.FromHsv
                            (
                                360.0,
                                newHsvColor.S,
                                newHsvColor.V,
                                100.0
                            );
                            break;
                        }

                    case ColorChannel.Channel2:
                        {
                            newColor = Microsoft.Toolkit.Uwp.Helpers.ColorHelper.FromHsv
                            (
                                newHsvColor.H,
                                100.0,
                                newHsvColor.V,
                                100.0
                            );
                            break;
                        }

                    case ColorChannel.Channel3:
                        {
                            newColor = Microsoft.Toolkit.Uwp.Helpers.ColorHelper.FromHsv
                            (
                                newHsvColor.H,
                                newHsvColor.S,
                                100.0,
                                100.0
                            );
                            break;
                        }
                }
                */

                this.ScheduleColorUpdate(Colors.White);
            }
            else if (rgbColor.A == 0x00)
            {
                // As an additional usability improvement, reset alpha to maximum when the spectrum is used.
                // The color spectrum has no alpha channel and it is much more intuitive to do this for the user
                // especially if the picker was initially set with Colors.Transparent.
                this.ScheduleColorUpdate(Color.FromArgb(0xFF, rgbColor.R, rgbColor.G, rgbColor.B));
            }

            return;
        }

        /// <summary>
        /// Event handler for when the selected color representation changes.
        /// This will convert between RGB and HSV.
        /// </summary>
        private void ColorRepToggleButton_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if (object.ReferenceEquals(sender, this.HsvToggleButton))
            {
                this.SetActiveColorRepresentation(ColorRepresentation.Hsva);
            }
            else
            {
                this.SetActiveColorRepresentation(ColorRepresentation.Rgba);
            }

            this.UpdateColorControlValues();
            this.UpdateChannelSliderBackgrounds();

            return;
        }

        /// <summary>
        /// Event handler for when a preview color panel is pressed.
        /// This will update the color to the background of the pressed panel.
        /// </summary>
        private void PreviewBorder_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Border border = sender as Border;

            if (border?.Background is SolidColorBrush brush)
            {
                this.ScheduleColorUpdate(brush.Color);
            }

            return;
        }

        /// <summary>
        /// Event handler for when a key is pressed within the Hex RGB value TextBox.
        /// This is used to trigger a re-evaluation of the color based on the TextBox value.
        /// </summary>
        private void HexInputTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                try
                {
                    ColorToHexConverter converter = new ColorToHexConverter();
                    this.Color = (Color)converter.ConvertBack(((TextBox)sender).Text, typeof(TextBox), null, null);
                }
                catch
                {
                    // Reset hex value
                    this.UpdateColorControlValues();
                    this.UpdateChannelSliderBackgrounds();
                }
            }

            return;
        }

        /// <summary>
        /// Event handler for when the Hex RGB value TextBox looses focus.
        /// This is used to trigger a re-evaluation of the color based on the TextBox value.
        /// </summary>
        private void HexInputTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ColorToHexConverter converter = new ColorToHexConverter();
                this.Color = (Color)converter.ConvertBack(((TextBox)sender).Text, typeof(TextBox), null, null);
            }
            catch
            {
                // Reset hex value
                this.UpdateColorControlValues();
                this.UpdateChannelSliderBackgrounds();
            }

            return;
        }

        /// <summary>
        /// Event handler for when a key is pressed within a color channel TextBox.
        /// This is used to trigger a re-evaluation of the color based on the TextBox value.
        /// </summary>
        private void ChannelTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                this.ApplyChannelTextBoxValue(sender as TextBox);
            }

            return;
        }

        /// <summary>
        /// Event handler for when a color channel TextBox loses focus.
        /// This is used to trigger a re-evaluation of the color based on the TextBox value.
        /// </summary>
        private void ChannelTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.ApplyChannelTextBoxValue(sender as TextBox);
            return;
        }

        /// <summary>
        /// Event handler for when the value within one of the channel Sliders is changed.
        /// </summary>
        private void ChannelSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            double senderValue = (sender as Slider)?.Value ?? double.NaN;

            if (object.ReferenceEquals(sender, this.Channel1Slider))
            {
                this.SetColorChannel(this.GetActiveColorRepresentation(), ColorChannel.Channel1, senderValue);
            }
            else if (object.ReferenceEquals(sender, this.Channel2Slider))
            {
                this.SetColorChannel(this.GetActiveColorRepresentation(), ColorChannel.Channel2, senderValue);
            }
            else if (object.ReferenceEquals(sender, this.Channel3Slider))
            {
                this.SetColorChannel(this.GetActiveColorRepresentation(), ColorChannel.Channel3, senderValue);
            }
            else if (object.ReferenceEquals(sender, this.AlphaChannelSlider))
            {
                this.SetColorChannel(this.GetActiveColorRepresentation(), ColorChannel.Alpha, senderValue);
            }
            else if (object.ReferenceEquals(sender, this.ColorSpectrumAlphaSlider))
            {
                this.SetColorChannel(this.GetActiveColorRepresentation(), ColorChannel.Alpha, senderValue);
            }
            else if (object.ReferenceEquals(sender, this.ColorSpectrumThirdDimensionSlider))
            {
                // Regardless of the active color representation, the spectrum is always HSV
                this.SetColorChannel(ColorRepresentation.Hsva, this.GetActiveColorSpectrumThirdDimension(), senderValue);
            }

            return;
        }
    }
}
