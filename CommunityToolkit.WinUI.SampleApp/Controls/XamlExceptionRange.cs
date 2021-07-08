// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityToolkit.WinUI.SampleApp.Controls
{
    /// <summary>
    /// Xaml Parsing Error Message and Location.
    /// </summary>
    public sealed class XamlExceptionRange : Exception
    {
        public uint StartLine { get; set; }

        public uint StartColumn { get; set; }

        public uint EndLine { get; set; }

        public uint EndColumn { get; set; }

        public XamlExceptionRange(string message, Exception error, uint startline, uint startcol, uint endline, uint endcol)
            : base(message, error)
        {
            StartLine = startline;
            StartColumn = startcol;
            EndLine = endline;
            EndColumn = endcol;
        }
    }
}