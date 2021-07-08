// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using CommunityToolkit.Common.Parsers.Markdown;
using CommunityToolkit.Common.Parsers.Markdown.Blocks;
using CommunityToolkit.Common.Parsers.Markdown.Inlines;

namespace UnitTests.Markdown.Parse
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class ParseTestExtensionMethods
    {
#pragma warning disable CS0618 // Type or member is obsolete
        /// <summary>
        /// Adds one or more child elements to the given parent object.
        /// </summary>
        /// <typeparam name="T">the type</typeparam>
        /// <param name="parent">the parent</param>
        /// <param name="elements">the elements to add</param>
        /// <returns>parent</returns>
        public static T AddChildren<T>(this T parent, params object[] elements)
        {
            foreach (var child in elements)
                AddChild(parent, child);
            return parent;
        }

        private static void AddChild<T>(T parent, object child)
        {
            if (parent is MarkdownDocument)
                AddChild(() => ((MarkdownDocument)(object)parent).Blocks, (value) => ((MarkdownDocument)(object)parent).Blocks = value, (MarkdownBlock)child);
            else if (parent is HeaderBlock)
                AddChild(() => ((HeaderBlock)(object)parent).Inlines, (value) => ((HeaderBlock)(object)parent).Inlines = value, (MarkdownInline)child);
            else if (parent is ListBlock)
                AddChild(() => ((ListBlock)(object)parent).Items, (value) => ((ListBlock)(object)parent).Items = value, (ListItemBlock)child);
            else if (parent is ListItemBlock)
                AddChild(() => ((ListItemBlock)(object)parent).Blocks, (value) => ((ListItemBlock)(object)parent).Blocks = value, (MarkdownBlock)child);
            else if (parent is ParagraphBlock)
                AddChild(() => ((ParagraphBlock)(object)parent).Inlines, (value) => ((ParagraphBlock)(object)parent).Inlines = value, (MarkdownInline)child);
            else if (parent is QuoteBlock)
                AddChild(() => ((QuoteBlock)(object)parent).Blocks, (value) => ((QuoteBlock)(object)parent).Blocks = value, (MarkdownBlock)child);
            else if (parent is TableBlock)
                AddChild(() => ((TableBlock)(object)parent).Rows, (value) => ((TableBlock)(object)parent).Rows = value, (TableBlock.TableRow)child);
            else if (parent is TableBlock.TableRow)
                AddChild(() => ((TableBlock.TableRow)(object)parent).Cells, (value) => ((TableBlock.TableRow)(object)parent).Cells = value, (TableBlock.TableCell)child);
            else if (parent is TableBlock.TableCell)
                AddChild(() => ((TableBlock.TableCell)(object)parent).Inlines, (value) => ((TableBlock.TableCell)(object)parent).Inlines = value, (MarkdownInline)child);
            else if (parent is BoldTextInline)
                AddChild(() => ((BoldTextInline)(object)parent).Inlines, (value) => ((BoldTextInline)(object)parent).Inlines = value, (MarkdownInline)child);
            else if (parent is ItalicTextInline)
                AddChild(() => ((ItalicTextInline)(object)parent).Inlines, (value) => ((ItalicTextInline)(object)parent).Inlines = value, (MarkdownInline)child);
            else if (parent is MarkdownLinkInline)
                AddChild(() => ((MarkdownLinkInline)(object)parent).Inlines, (value) => ((MarkdownLinkInline)(object)parent).Inlines = value, (MarkdownInline)child);
            else if (parent is StrikethroughTextInline)
                AddChild(() => ((StrikethroughTextInline)(object)parent).Inlines, (value) => ((StrikethroughTextInline)(object)parent).Inlines = value, (MarkdownInline)child);
            else if (parent is SuperscriptTextInline)
                AddChild(() => ((SuperscriptTextInline)(object)parent).Inlines, (value) => ((SuperscriptTextInline)(object)parent).Inlines = value, (MarkdownInline)child);
            else if (parent is SubscriptTextInline)
                AddChild(() => ((SubscriptTextInline)(object)parent).Inlines, (value) => ((SubscriptTextInline)(object)parent).Inlines = value, (MarkdownInline)child);
            else
                throw new NotSupportedException(string.Format("Unsupported type {0}", typeof(T).Name));
        }

        private static void AddChild<T>(Func<IList<T>> getter, Action<IList<T>> setter, T child)
        {
            var list = getter();
            if (list == null)
            {
                list = new List<T>();
                setter(list);
            }
            list.Add(child);
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}