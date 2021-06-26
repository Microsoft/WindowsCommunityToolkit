// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// A class that can be used to provide data context for <see cref="OrbitView"></see> items by providing distance and diameter values
    /// </summary>
    public partial class OrbitViewDataItem : DependencyObject
    {
        /// <summary>
        /// Gets or sets a value indicating the distance from the center.
        /// Expected value between 0 and 1
        /// </summary>
        public double Distance
        {
            get { return (double)GetValue(DistanceProperty); }
            set { SetValue(DistanceProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Distance"/> property
        /// </summary>
        public static readonly DependencyProperty DistanceProperty =
            DependencyProperty.Register(nameof(Distance), typeof(double), typeof(OrbitViewDataItem), new PropertyMetadata(0.5));

        /// <summary>
        /// Gets or sets a value indicating the name of the item.
        /// Used for <see cref="AutomationProperties"/>
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Label"/> property
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(OrbitViewDataItem), new PropertyMetadata("OrbitViewDataItem"));

        /// <summary>
        /// Gets or sets a value indicating the diameter of the item.
        /// Expected value between 0 and 1
        /// </summary>
        public double Diameter
        {
            get { return (double)GetValue(DiameterProperty); }
            set { SetValue(DiameterProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Diameter"/> property
        /// </summary>
        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register(nameof(Diameter), typeof(double), typeof(OrbitViewDataItem), new PropertyMetadata(-1d));

        /// <summary>
        /// Gets or sets a value indicating the image of the item.
        /// </summary>
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Image"/> property
        /// </summary>
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register(nameof(Image), typeof(ImageSource), typeof(OrbitViewDataItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value of an object that can be used to store model data.
        /// </summary>
        public object Item
        {
            get { return (object)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Item"/> property
        /// </summary>
        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register(nameof(Item), typeof(object), typeof(OrbitViewDataItem), new PropertyMetadata(null));
    }
}