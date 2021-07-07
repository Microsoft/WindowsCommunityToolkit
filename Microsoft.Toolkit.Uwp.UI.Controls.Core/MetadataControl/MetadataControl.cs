// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Display <see cref="MetadataItem"/>s separated by bullets.
    /// </summary>
    [TemplatePart(Name = TextContainerPart, Type = typeof(TextBlock))]
    public sealed class MetadataControl : Control
    {
        /// <summary>
        /// The DP to store the <see cref="Separator"/> property value.
        /// </summary>
        public static readonly DependencyProperty SeparatorProperty = DependencyProperty.Register(
            nameof(Separator),
            typeof(string),
            typeof(MetadataControl),
            new PropertyMetadata(" • ", OnPropertyChanged));

        /// <summary>
        /// The DP to store the <see cref="AccessibleSeparator"/> property value.
        /// </summary>
        public static readonly DependencyProperty AccessibleSeparatorProperty = DependencyProperty.Register(
            nameof(AccessibleSeparator),
            typeof(string),
            typeof(MetadataControl),
            new PropertyMetadata(", ", OnPropertyChanged));

        /// <summary>
        /// The DP to store the <see cref="Items"/> property value.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            nameof(Items),
            typeof(IEnumerable<MetadataItem>),
            typeof(MetadataControl),
            new PropertyMetadata(null, OnMetadataItemsChanged));

        /// <summary>
        /// The DP to store the TextBlockStyle value.
        /// </summary>
        public static readonly DependencyProperty TextBlockStyleProperty = DependencyProperty.Register(
            nameof(TextBlockStyle),
            typeof(Style),
            typeof(MetadataControl),
            new PropertyMetadata(null));

        private const string TextContainerPart = "TextContainer";

        private TextBlock _textContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataControl"/> class.
        /// </summary>
        public MetadataControl()
        {
            DefaultStyleKey = typeof(MetadataControl);
            ActualThemeChanged += OnActualThemeChanged;
        }

        /// <summary>
        /// Gets or sets the separator to display between the <see cref="MetadataItem"/>.
        /// </summary>
        public string Separator
        {
            get => (string)GetValue(SeparatorProperty);
            set => SetValue(SeparatorProperty, value);
        }

        /// <summary>
        /// Gets or sets the separator that will be used to generate the accessible string representing the control content.
        /// </summary>
        public string AccessibleSeparator
        {
            get => (string)GetValue(AccessibleSeparatorProperty);
            set => SetValue(AccessibleSeparatorProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="MetadataItem"/> to display in the control.
        /// If it implements <see cref="INotifyCollectionChanged"/>, the control will automatically update itself.
        /// </summary>
        public IEnumerable<MetadataItem> Items
        {
            get => (IEnumerable<MetadataItem>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> to use on the inner <see cref="TextBlock"/> control.
        /// </summary>
        public Style TextBlockStyle
        {
            get => (Style)GetValue(TextBlockStyleProperty);
            set => SetValue(TextBlockStyleProperty, value);
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            _textContainer = GetTemplateChild(TextContainerPart) as TextBlock;
            Update();
        }

        private static void OnMetadataItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (MetadataControl)d;
            void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) => control.Update();

            if (e.OldValue is INotifyCollectionChanged oldNcc)
            {
                oldNcc.CollectionChanged -= OnCollectionChanged;
            }

            if (e.NewValue is INotifyCollectionChanged newNcc)
            {
                newNcc.CollectionChanged += OnCollectionChanged;
            }

            control.Update();
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((MetadataControl)d).Update();

        private void OnActualThemeChanged(FrameworkElement sender, object args) => Update();

        private void Update()
        {
            if (_textContainer is null)
            {
                // The template is not ready yet.
                return;
            }

            _textContainer.Inlines.Clear();

            if (Items is null)
            {
                AutomationProperties.SetName(_textContainer, string.Empty);
                NotifyLiveRegionChanged();
                return;
            }

            Inline unitToAppend;
            var accessibleString = new StringBuilder();
            foreach (var unit in Items)
            {
                if (_textContainer.Inlines.Count > 0)
                {
                    _textContainer.Inlines.Add(new Run { Text = Separator });
                    accessibleString.Append(AccessibleSeparator ?? Separator);
                }

                unitToAppend = new Run
                {
                    Text = unit.Label,
                };

                if (unit.Command != null)
                {
                    var hyperLink = new Hyperlink
                    {
                        UnderlineStyle = UnderlineStyle.None,
                        Foreground = _textContainer.Foreground,
                    };
                    hyperLink.Inlines.Add(unitToAppend);

                    void OnHyperlinkClicked(Hyperlink sender, HyperlinkClickEventArgs args)
                    {
                        if (unit.Command.CanExecute(unit.CommandParameter))
                        {
                            unit.Command.Execute(unit.CommandParameter);
                        }
                    }

                    hyperLink.Click += OnHyperlinkClicked;

                    unitToAppend = hyperLink;
                }

                var unitAccessibleLabel = unit.AccessibleLabel ?? unit.Label;
                AutomationProperties.SetName(unitToAppend, unitAccessibleLabel);
                accessibleString.Append(unitAccessibleLabel);

                _textContainer.Inlines.Add(unitToAppend);
            }

            AutomationProperties.SetName(_textContainer, accessibleString.ToString());
            NotifyLiveRegionChanged();
        }

        private void NotifyLiveRegionChanged()
        {
            if (AutomationPeer.ListenerExists(AutomationEvents.LiveRegionChanged))
            {
                var peer = FrameworkElementAutomationPeer.FromElement(this);
                peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
            }
        }
    }
}
