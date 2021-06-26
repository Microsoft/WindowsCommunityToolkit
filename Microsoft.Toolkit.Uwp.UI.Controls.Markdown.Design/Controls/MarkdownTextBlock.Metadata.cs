// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

using Microsoft.Toolkit.Uwp.UI.Controls.Design.Properties;
using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Design
{
    internal class MarkdownTextBlockMetadata : AttributeTableBuilder
    {
        public MarkdownTextBlockMetadata()
            : base()
        {
            AddCallback(ControlTypes.MarkdownTextBlock,
                b =>
                {
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.ImageStretch), new CategoryAttribute(Resources.CategoryMarkdownStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.Text), new CategoryAttribute(Resources.CategoryMarkdownStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.IsTextSelectionEnabled), new CategoryAttribute(Resources.CategoryMarkdownStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.LinkForeground), new CategoryAttribute(Resources.CategoryMarkdownStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.TextWrapping), new CategoryAttribute(Resources.CategoryMarkdownStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.ParagraphMargin), new CategoryAttribute(Resources.CategoryMarkdownStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.HorizontalRuleBrush), new CategoryAttribute(Resources.CategoryMarkdownStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.HorizontalRuleMargin), new CategoryAttribute(Resources.CategoryMarkdownStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.HorizontalRuleThickness), new CategoryAttribute(Resources.CategoryMarkdownStyle));

                    b.AddCustomAttributes(nameof(MarkdownTextBlock.CodeBackground), new CategoryAttribute(Resources.CategoryMarkdownCodeStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.CodeBorderBrush), new CategoryAttribute(Resources.CategoryMarkdownCodeStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.CodeBorderThickness), new CategoryAttribute(Resources.CategoryMarkdownCodeStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.CodeForeground), new CategoryAttribute(Resources.CategoryMarkdownCodeStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.CodeBackground), new CategoryAttribute(Resources.CategoryMarkdownCodeStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.CodeFontFamily), new CategoryAttribute(Resources.CategoryMarkdownCodeStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.CodeMargin), new CategoryAttribute(Resources.CategoryMarkdownCodeStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.CodePadding), new CategoryAttribute(Resources.CategoryMarkdownCodeStyle));

                    for (int i = 1; i <= 6; i++)
                    {
                        b.AddCustomAttributes($"Header{i}FontWeight", new CategoryAttribute(string.Format(Resources.CategoryMarkdownHeaderStyle, i)));
                        b.AddCustomAttributes($"Header{i}FontSize", new CategoryAttribute(string.Format(Resources.CategoryMarkdownHeaderStyle, i)));
                        b.AddCustomAttributes($"Header{i}Margin", new CategoryAttribute(string.Format(Resources.CategoryMarkdownHeaderStyle, i)));
                        b.AddCustomAttributes($"Header{i}Foreground", new CategoryAttribute(string.Format(Resources.CategoryMarkdownHeaderStyle, i)));
                    }

                    b.AddCustomAttributes(nameof(MarkdownTextBlock.ListMargin), new CategoryAttribute(Resources.CategoryMarkdownListStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.ListGutterWidth), new CategoryAttribute(Resources.CategoryMarkdownListStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.ListBulletSpacing), new CategoryAttribute(Resources.CategoryMarkdownListStyle));

                    b.AddCustomAttributes(nameof(MarkdownTextBlock.QuoteBackground), new CategoryAttribute(Resources.CategoryMarkdownQuoteStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.QuoteBorderBrush), new CategoryAttribute(Resources.CategoryMarkdownQuoteStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.QuoteBorderThickness), new CategoryAttribute(Resources.CategoryMarkdownQuoteStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.QuoteForeground), new CategoryAttribute(Resources.CategoryMarkdownQuoteStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.QuoteMargin), new CategoryAttribute(Resources.CategoryMarkdownQuoteStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.QuotePadding), new CategoryAttribute(Resources.CategoryMarkdownQuoteStyle));

                    b.AddCustomAttributes(nameof(MarkdownTextBlock.TableBorderBrush), new CategoryAttribute(Resources.CategoryMarkdownTableStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.TableBorderThickness), new CategoryAttribute(Resources.CategoryMarkdownTableStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.TableCellPadding), new CategoryAttribute(Resources.CategoryMarkdownTableStyle));
                    b.AddCustomAttributes(nameof(MarkdownTextBlock.TableMargin), new CategoryAttribute(Resources.CategoryMarkdownTableStyle));

                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Toolkit, false));
                }
            );
        }
    }
}