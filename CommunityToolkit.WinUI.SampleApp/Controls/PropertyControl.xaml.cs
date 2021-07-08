// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommunityToolkit.WinUI.SampleApp.Common;
using CommunityToolkit.WinUI.SampleApp.Models;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace CommunityToolkit.WinUI.SampleApp.Controls
{
    public sealed partial class PropertyControl
    {
        private static Dictionary<Windows.UI.Color, string> _colorNames;

        private Sample _currentSample;

        public PropertyControl()
        {
            if (_colorNames == null)
            {
                _colorNames = new Dictionary<Windows.UI.Color, string>();
                foreach (var color in typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static))
                {
                    _colorNames[(Windows.UI.Color)color.GetValue(null)] = color.Name;
                }
            }

            InitializeComponent();
        }

        private void PropertyControl_OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue == _currentSample)
            {
                return;
            }

            _currentSample = DataContext as Sample;

            RootPanel.Children.Clear();

            if (_currentSample != null)
            {
                var propertyDesc = _currentSample.PropertyDescriptor;

                if (propertyDesc == null)
                {
                    return;
                }

                foreach (var option in propertyDesc.Options)
                {
                    // Label
                    var label = new TextBlock
                    {
                        Text = option.Label,
                        FontSize = 15,
                        FontWeight = FontWeights.Bold
                    };
                    RootPanel.Children.Add(label);

                    // Control
                    Control controlToAdd;
                    DependencyProperty dependencyProperty;
                    IValueConverter converter = null;

                    IDictionary<string, object> propertyDict = propertyDesc.Expando;

                    switch (option.Kind)
                    {
                        case PropertyKind.Slider:
                        case PropertyKind.DoubleSlider:
                            var slider = new Slider();
                            var sliderOption = option as SliderPropertyOptions;
                            if (sliderOption != null)
                            {
                                slider.Minimum = sliderOption.MinValue;
                                slider.Maximum = sliderOption.MaxValue;
                                slider.StepFrequency = sliderOption.Step;
                            }

                            if (option.Kind == PropertyKind.DoubleSlider)
                            {
                                slider.StepFrequency = 0.01;
                            }

                            if ((propertyDict[option.Name] as ValueHolder).Value is double value)
                            {
                                slider.Value = value;
                            }

                            controlToAdd = slider;
                            dependencyProperty = RangeBase.ValueProperty;

                            break;

                        case PropertyKind.Enum:
                            var enumType = option.DefaultValue.GetType();
                            var comboBox = new ComboBox
                            {
                                ItemsSource = Enum.GetNames(enumType),
                                SelectedItem = (propertyDict[option.Name] as ValueHolder).Value.ToString()
                            };

                            converter = new EnumConverter(enumType);
                            controlToAdd = comboBox;
                            dependencyProperty = Selector.SelectedItemProperty;
                            break;

                        case PropertyKind.Bool:
                            var checkBox = new ToggleSwitch();

                            if ((propertyDict[option.Name] as ValueHolder).Value is bool isOn)
                            {
                                checkBox.IsOn = isOn;
                            }

                            controlToAdd = checkBox;
                            dependencyProperty = ToggleSwitch.IsOnProperty;
                            break;

                        case PropertyKind.Brush:
                            var colorComboBox = new ComboBox();
                            var dataSource = typeof(Colors).GetTypeInfo().DeclaredProperties.Where(p => p.GetMethod != null && p.GetMethod.IsPublic).Select(p => p.Name).ToList();
                            colorComboBox.ItemsSource = dataSource;

                            if ((propertyDict[option.Name] as ValueHolder).Value is SolidColorBrush brush &&
                                _colorNames.TryGetValue(brush.Color, out var color))
                            {
                                colorComboBox.SelectedIndex = dataSource.IndexOf(color);
                            }
                            else
                            {
                                colorComboBox.SelectedIndex = dataSource.IndexOf(option.DefaultValue.ToString());
                            }

                            converter = new SolidColorBrushConverter();

                            controlToAdd = colorComboBox;
                            dependencyProperty = Selector.SelectedItemProperty;
                            break;

                        case PropertyKind.TimeSpan:
                            var timeSlider = new Slider();
                            var timeSliderOption = option as SliderPropertyOptions;
                            if (timeSliderOption != null)
                            {
                                timeSlider.Minimum = timeSliderOption.MinValue;
                                timeSlider.Maximum = timeSliderOption.MaxValue;
                                timeSlider.StepFrequency = timeSliderOption.Step;
                            }

                            if ((propertyDict[option.Name] as ValueHolder).Value is double timeValue)
                            {
                                timeSlider.Value = timeValue;
                            }

                            controlToAdd = timeSlider;
                            dependencyProperty = RangeBase.ValueProperty;
                            converter = new TimeSpanConverter();

                            break;

                        case PropertyKind.Thickness:
                            var thicknessTextBox = new TextBox { Text = (propertyDict[option.Name] as ValueHolder).Value.ToString() };

                            controlToAdd = thicknessTextBox;
                            dependencyProperty = TextBox.TextProperty;
                            converter = new ThicknessConverter();
                            break;

                        default:
                            var textBox = new TextBox { Text = (propertyDict[option.Name] as ValueHolder).Value.ToString() };

                            controlToAdd = textBox;
                            dependencyProperty = TextBox.TextProperty;
                            break;
                    }

                    var binding = new Binding
                    {
                        Source = propertyDesc.Expando,
                        Path = new PropertyPath(option.Name + ".Value"),
                        Mode = BindingMode.TwoWay,
                        Converter = converter
                    };

                    // Make textboxes instantly respond to text rather than waiting for lost focus.
                    if (controlToAdd is TextBox)
                    {
                        binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    }

                    controlToAdd.SetBinding(dependencyProperty, binding);
                    controlToAdd.Margin = new Thickness(0, 5, 0, 20);
                    RootPanel.Children.Add(controlToAdd);
                }
            }
        }
    }
}