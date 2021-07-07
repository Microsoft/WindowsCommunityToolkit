// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Properties of the <see cref="ContentSizer"/> control.
    /// </summary>
    public partial class ContentSizer
    {
        /// <summary>
        /// Gets or sets the content of the sizer, by default is the grip symbol.
        /// </summary>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Content"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(object), typeof(ContentSizer), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the content template for the <see cref="Content"/>. By default is a TextBlock.
        /// </summary>
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(nameof(ContentTemplate), typeof(DataTemplate), typeof(ContentSizer), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the cursor to use when hovering over the sizer.
        /// </summary>
        public CoreCursorType GripperCursor
        {
            get { return (CoreCursorType)GetValue(GripperCursorProperty); }
            set { SetValue(GripperCursorProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="GripperCursor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GripperCursorProperty =
            DependencyProperty.Register(nameof(GripperCursor), typeof(CoreCursorType), typeof(ContentSizer), new PropertyMetadata(CoreCursorType.SizeWestEast));

        /// <summary>
        /// Gets or sets the foreground color of sizer grip.
        /// </summary>
        public Brush GripperForeground
        {
            get { return (Brush)GetValue(GripperForegroundProperty); }
            set { SetValue(GripperForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="GripperForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GripperForegroundProperty =
            DependencyProperty.Register(nameof(GripperForeground), typeof(Brush), typeof(ContentSizer), new PropertyMetadata(default(Brush)));

        /// <summary>
        /// Gets or sets the direction that the sizer will interact with.
        /// </summary>
        public ContentResizeDirection ResizeDirection
        {
            get { return (ContentResizeDirection)GetValue(ResizeDirectionProperty); }
            set { SetValue(ResizeDirectionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ResizeDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ResizeDirectionProperty =
            DependencyProperty.Register(nameof(ResizeDirection), typeof(ContentResizeDirection), typeof(ContentSizer), new PropertyMetadata(ContentResizeDirection.Vertical));

        /// <summary>
        /// Gets or sets the control that the <see cref="ContentSizer"/> is resizing. Be default, this will be the visual ancestor of the <see cref="ContentSizer"/>.
        /// </summary>
        public FrameworkElement TargetControl
        {
            get { return (FrameworkElement)GetValue(TargetControlProperty); }
            set { SetValue(TargetControlProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="TargetControl"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetControlProperty =
            DependencyProperty.Register(nameof(TargetControl), typeof(FrameworkElement), typeof(ContentSizer), new PropertyMetadata(null, OnTargetControlChanged));

        private static void OnTargetControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Check if our width can be manipulated
            if (d is ContentSizer sizer && e.NewValue is FrameworkElement element)
            {
                // TODO: For Auto we might want to do detection logic (TBD) here first?
                if (sizer.ResizeDirection != ContentResizeDirection.Horizontal && double.IsNaN(element.Width))
                {
                    element.Width = element.DesiredSize.Width;
                }

                if (sizer.ResizeDirection != ContentResizeDirection.Vertical && double.IsNaN(element.Height))
                {
                    element.Height = element.DesiredSize.Height;
                }
            }
        }
    }
}
